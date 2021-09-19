using AppCoreApi.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AppCoreApi.Infrastructure.Extension;
using System.Threading;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Extensions;
using System.Reflection;

namespace xlmap
{
    public class ObjectConverter : IObjectConverter
    {
        private string _baseConfigPath;
        private readonly AppDbContext _context;

        public ObjectConverter(AppDbContext context)
        {
            _context = context;
        }

        public ObjectConverter(AppDbContext context, string baseConfigPath)
        {
            _context = context;
            _baseConfigPath = baseConfigPath;
        }

        public string BaseConfigPath => _baseConfigPath;

        // method digunakan untuk mendapatkan daftar table master data dan propertynya
        // nantinya digunakan untuk mengambil daftar master data dari database
        private Dictionary<string, List<object>> GetMasterDataKey<T>(ObjectMapConfig config, List<T> sourceObj,
            out Dictionary<string, Dictionary<object, object>> MasterDataKeyValueMap,
            out Dictionary<string, string> propertyDictMap)
        {
            Type destType = Type.GetType(config.FullAssemblyName);
            Type sourceType = typeof(T);

            // dictionary untuk menyimpan pasangan antara PRIMARY VALUE vs PRIMARY KEY suatu master data
            MasterDataKeyValueMap = new Dictionary<string, Dictionary<object, object>>();
            propertyDictMap = new Dictionary<string, string>();

            Dictionary<string, List<Object>> dict = new Dictionary<string, List<object>>();
            foreach (var propInfo in config.ParentFields)
            {
                if (propInfo.DestinationPropertyDataType != ObjectMapDetailConfig.DataType.OBJECT) continue;

                // pastikan bahwa data unique key-nya berdasarkan nama table dan terisi seluruh primary value dari object yang ada
                if (!dict.ContainsKey(propInfo.RefTableName))
                {
                    dict.Add(propInfo.RefTableName, new List<object>());
                    MasterDataKeyValueMap.Add(propInfo.RefTableName, new Dictionary<object, object>());
                    propertyDictMap.Add(propInfo.RefTableName, propInfo.ObjectPrimaryKeyProperty);
                }

                // ambil property object
                var prop = sourceType.GetProperty(propInfo.SourceProperty);

                // ambil seluruh primary value dari object tersebut dan masukkan ke dalam list
                List<object> allDatas = new List<object>();
                foreach (var item in sourceObj)
                {
                    var datas = sourceObj.Select(e => prop.GetValue(item)).Where(e => prop.GetValue(item) != null).Distinct().ToList();
                    allDatas.AddRange(datas);
                }
                dict[propInfo.RefTableName].AddRange(allDatas.Distinct().ToList());

                // add value ke master data key
                foreach (var val in allDatas.Where(e => e != null).Distinct())
                {
                    if (!MasterDataKeyValueMap[propInfo.RefTableName].ContainsKey(val))
                    {
                        MasterDataKeyValueMap[propInfo.RefTableName].Add(val, null);
                    }
                }
            }

            foreach (var propInfo in config.ChildFields)
            {
                if (propInfo.DestinationPropertyDataType != ObjectMapDetailConfig.DataType.OBJECT) continue;

                if (!dict.ContainsKey(propInfo.RefTableName))
                {
                    dict.Add(propInfo.RefTableName, new List<object>());
                    MasterDataKeyValueMap.Add(propInfo.RefTableName, new Dictionary<object, object>());
                    propertyDictMap.Add(propInfo.RefTableName, propInfo.ObjectPrimaryKeyProperty);
                }

                var prop = sourceType.GetProperty(propInfo.SourceProperty);

                List<object> allDatas = new List<object>();
                foreach (var item in sourceObj)
                {
                    var datas = sourceObj.Select(e => prop.GetValue(item)).Where(e => prop.GetValue(item) != null).Distinct().ToList();
                    allDatas.AddRange(datas);
                }
                dict[propInfo.RefTableName].AddRange(allDatas.Distinct().ToList());

                // add value ke master data key
                foreach (var val in allDatas.Where(e => e != null).Distinct())
                {
                    if (!MasterDataKeyValueMap[propInfo.RefTableName].ContainsKey(val))
                    {
                        MasterDataKeyValueMap[propInfo.RefTableName].Add(val, null);
                    }
                }
            }

            return dict;
        }

        // method untuk meload seluruh master data yang dibutuhkan dari database
        private async Task<Dictionary<string, List<object>>> LoadMasterDatas(
            ObjectMapConfig config,
            Dictionary<string, List<object>> dictMap,
            Dictionary<string, Dictionary<object, object>> MasterDataKeyValueMap,
            Dictionary<string, string> propertyDictMap,
            CancellationToken ct)
        {
            Dictionary<string, List<object>> masterdatas = new Dictionary<string, List<object>>();
            foreach (var tableName in dictMap.Keys)
            {
                var inKeys = dictMap[tableName];
                var fqan = config.ParentFields.Where(e => e.RefTableName == tableName).FirstOrDefault();
                if (fqan == null)
                    fqan = config.ChildFields.Where(e => e.RefTableName == tableName).FirstOrDefault();
                if (fqan == null) continue;

                string sql = $@"select * 
                from {tableName} 
                where IsDraftRecord = 0 and coalesce(DraftFromUpload,0) = 0 and DeletedAt is null
                and {fqan.RefFieldName} in @inparam";

                var t = Type.GetType(fqan.DestinationAssembly);
                var result = await _context.QueryAsync(t, ct, sql, new { inparam = inKeys });
                masterdatas.Add(tableName, result.ToList());

                // add primary key value ke collection primary value

            }
            return masterdatas;
        }

        private async Task<T> GetExistingData<T>(string key, List<ObjectMapDetailConfig> configDetails,
            ObjectMapConfig config, Dictionary<string, string> propertyDictMap, bool isLoadChild = true)
        {
            // proses parent
            string separator = "";
            string orderby = "";
            if(!string.IsNullOrEmpty(config.OrderBy))
            {
                orderby = $" ORDER BY  {config.OrderBy} ";

                if(!string.IsNullOrEmpty(config.OrderByType))
                {
                    orderby += config.OrderByType;
                }
            }
           
            string concat = "CONCAT(";
            foreach (var item in configDetails)
            {
                if (item.DestinationPropertyDataType == ObjectMapDetailConfig.DataType.OBJECT)
                {
                    concat += separator + item.DestinationKeyProperty;
                }
                else
                {
                    concat += separator + item.FieldName;
                }
                separator = ",';',";
            }
            concat += ")";
            if (configDetails.Count == 1) concat = configDetails[0].FieldName;

            string sql = $@"SELECT * FROM {config.TableName} WHERE {concat} = @concatvalue AND IsDraftRecord = 0 and coalesce(DraftFromUpload,0) = 0 and DeletedAt is null {orderby}";
            var t = Type.GetType(config.FullAssemblyName);
            var parentResult = await _context.QueryAsync(t, default, sql, new { concatvalue = key });
            if (parentResult == null || parentResult.Count() <= 0) return default;
            var parent = parentResult.First();
            if (string.IsNullOrEmpty(config.ChildFullAssemblyName))
                return (T)parent;

            // proses childnya
            if (isLoadChild)
            {
                var c = Type.GetType(config.ChildFullAssemblyName);

                var pkValue = t.GetProperty(config.DestinationPrimaryKeyProperty).GetValue(parent);
                sql = $@"select * from {config.ChildTableName} where {config.ForeignKeyField} = @foreignkey and IsDraftRecord = 0 and coalesce(DraftFromUpload,0) = 0 and DeletedAt is null";
                var childResults = await _context.QueryAsync(c, default, sql, new { foreignkey = pkValue });
                var concreteChilds = ListUtil.ConvertToList(childResults, c);
                t.GetProperty(config.DestinationChildPropertyName).SetValue(parent, concreteChilds);
            }
            return (T)parent;
        }

        public async Task<List<T>> Expand<T, K>(List<K> sourceObj, bool commitTodb = false, CancellationToken ct = default, bool isRemoveChild = true, bool isCombineChild = true)
        {
            #region load konfigurasi filename: <namaClass>Map.json

            string configFile = System.IO.Path.Combine(_baseConfigPath, typeof(T).Name + "Map.json");
            string jsonString = System.IO.File.ReadAllText(configFile);
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ObjectMapConfig>(jsonString);

            #endregion

            #region create instance result, distinct result master data dan tipe data source & destination

            List<T> result = new List<T>();
            Dictionary<string, T> resultFiltered = new Dictionary<string, T>();
            Type destType = Type.GetType(config.FullAssemblyName);
            Type sourceType = typeof(K);
            Type genericListType = typeof(List<>);
            Type concreteListType = null;
            Type childType = null;
            //var refPropertyName = sourceType.Name;
            if (!string.IsNullOrEmpty(config.ChildFullAssemblyName))
                childType = Type.GetType(config.ChildFullAssemblyName);

            #endregion

            #region ambil daftar foreign key untuk di load master datanya

            Dictionary<string, Dictionary<object, object>> MasterDataKeyValueMap = null;
            Dictionary<string, string> propertyDictMap = null;
            var dictMasterKeys = GetMasterDataKey<K>(config, sourceObj, out MasterDataKeyValueMap, out propertyDictMap);
            var dictMasters = await LoadMasterDatas(config, dictMasterKeys, MasterDataKeyValueMap, propertyDictMap, ct);

            #endregion

            #region expand object flat ke one-to-many

            int excelRowIndex = 0;
            foreach (var item in sourceObj)
            {
                #region buat instance dan set upload row index nya

                excelRowIndex++;
                var instance = Activator.CreateInstance(destType);
                destType.GetProperty("UploadRowIndex").SetValue(instance, excelRowIndex);
                sourceType.GetProperty("UploadRowIndex").SetValue(item, excelRowIndex);
                sourceType.GetMethod("AddValidationMessage").Invoke(item, new object[] { "" });
                //sourceType.GetProperty(refPropertyName).SetValue(item, instance);
                #endregion

                #region jika ada child, buat instance penampung untuk child nya

                IList childs = null;
                if (childType != null)
                {
                    // list of childs
                    concreteListType = genericListType.MakeGenericType(childType);
                    childs = Activator.CreateInstance(concreteListType) as IList;
                    destType.GetProperty(config.DestinationChildPropertyName).SetValue(instance, childs);
                }

                #endregion

                #region proses untuk parent data

                List<ObjectMapDetailConfig> keyProperty = new List<ObjectMapDetailConfig>();
                string separator = "";
                string key = "";
                foreach (var propInfo in config.ParentFields)
                {
                    // set property instance
                    var sourceProperty = sourceType.GetProperty(propInfo.SourceProperty);
                    if (sourceProperty == null)
                        throw new Exception($"Property {propInfo.SourceProperty} not found in class {sourceType.Name}");

                    if (propInfo.IsPrimaryKey)
                    {
                        keyProperty.Add(propInfo);
                    }

                    // cek jika kosong & required, maka add error
                    var sourceContent = sourceProperty.GetValue(item);
                    if (sourceContent == null || string.IsNullOrEmpty(sourceContent.ToString()))
                    {
                        var currenMessage = sourceType.GetProperty("UploadValidationMessage").GetValue(item)?.ToString();
                        if(!string.IsNullOrEmpty(currenMessage) && !currenMessage.Contains("harus TRUE/FALSE."))
                        {
                            if (propInfo.isRequired)
                            {
                                sourceType.GetMethod("AddValidationMessage").Invoke(item, new object[] { $"{propInfo.ExcelFieldTitle} harus diisi." });
                            }
                        }
                        
                        continue;
                    }

                    if (propInfo.IsPrimaryKey)
                    {
                        if (propInfo.DestinationPropertyDataType != ObjectMapDetailConfig.DataType.OBJECT)
                        {
                            key += separator + sourceContent.ToString();
                        }
                    }

                    // khusus object, penanganan berbeda karena harus mengisi object & foreign key
                    if (propInfo.DestinationPropertyDataType == ObjectMapDetailConfig.DataType.OBJECT)
                    {
                        #region pengisian object belongs-to

                        var belongsToType = Type.GetType(propInfo.DestinationAssembly);
                        if (belongsToType == null)
                            throw new Exception($"Could not load assembly {propInfo.DestinationAssembly}");


                        var propNames = propInfo.DestinationProperty.Split(".");
                        if (propNames.Count() <= 1)
                            throw new Exception($"Invalid object property definition: {propInfo.DestinationProperty}");

                        #endregion

                        // cek kode primary value belongs-to ke dalam collection master data
                        if (!dictMasters.ContainsKey(propInfo.RefTableName))
                        {
                            string errorMessage = $"{propInfo.ExcelFieldTitle} invalid.";
                            destType.GetMethod("AddValidationMessage").Invoke(instance, new object[] { errorMessage });
                            sourceType.GetMethod("AddValidationMessage").Invoke(item, new object[] { errorMessage });
                        }
                        else
                        {
                            if (!dictMasters[propInfo.RefTableName].Where(e => belongsToType.GetProperty(propNames[1]).GetValue(e).ToString().ToLower() == sourceContent.ToString().ToLower()).Any())
                            {
                                string errorMessage = $"{propInfo.ExcelFieldTitle} invalid.";
                                destType.GetMethod("AddValidationMessage").Invoke(instance, new object[] { errorMessage });
                                sourceType.GetMethod("AddValidationMessage").Invoke(item, new object[] { errorMessage });
                            }
                            else
                            {
                                
                                //    throw new Exception($"Property {propNames[0]} not found in class {destType.Name}");
                                var belongsToInstance = dictMasters[propInfo.RefTableName].Where(e => belongsToType.GetProperty(propNames[1]).GetValue(e).ToString().ToLower() == sourceContent.ToString().ToLower()).First();
                                //destProp.SetValue(instance, belongsToInstance);

                                var destProp = destType.GetProperty(propNames[0]);
                                if (destProp != null)
                                    destProp.SetValue(instance, belongsToInstance);

                                // tunjuk primary key dari object
                                var belongsToPKProp = belongsToType.GetProperty(propInfo.ObjectPrimaryKeyProperty);
                                if (belongsToPKProp == null)
                                    throw new Exception($"Property {propInfo.ObjectPrimaryKeyProperty} not found in class {belongsToType.Name}");

                                // tunjuk foreign key property
                                var fkProp = destType.GetProperty(propInfo.DestinationKeyProperty);
                                if (fkProp == null)
                                    throw new Exception($"Property {propInfo.DestinationAssembly} not found in class {destType.Name}");

                                // assign id foreign key value
                                fkProp.SetValue(instance, belongsToPKProp.GetValue(belongsToInstance));
                                if (propInfo.IsPrimaryKey)
                                {
                                    key += separator + belongsToPKProp.GetValue(belongsToInstance);
                                }
                            }
                        }

                    }
                    else
                    {
                        var destProp = destType.GetProperty(propInfo.DestinationProperty);
                        if (destProp == null)
                            throw new Exception($"Property {propInfo.DestinationProperty} not found in class {destType.Name}");

                        destProp.SetValue(instance, sourceContent);
                    }
                    separator = ";";
                }

                // cek existing data berdasarkan data parent jika belum pernah di cek
                //bool dataExists = false;
                if (!resultFiltered.ContainsKey(key))
                {
                    var existingParent = await GetExistingData<T>(key, keyProperty, config, propertyDictMap, isCombineChild);
                    if (existingParent != null)
                    {
                        //dataExists = true;
                        destType.GetProperty("UploadValidationMessage").SetValue(existingParent, "");
                        foreach (var propInfo in config.ParentFields)
                        {
                            if (propInfo.DestinationPropertyDataType != ObjectMapDetailConfig.DataType.OBJECT)
                            {
                                var destProp = destType.GetProperty(propInfo.DestinationProperty);
                                var propNewValue = destProp.GetValue(instance);
                                destProp.SetValue(existingParent, propNewValue);
                            } else
                            {
                                var propNames = propInfo.DestinationProperty.Split(".");
                                if (propNames.Count() >= 1)
                                {
                                    // isi objek belongs-to
                                    var destProp = destType.GetProperty(propNames[0]);
                                    var propNewValue = destProp.GetValue(instance);
                                    destProp.SetValue(existingParent, propNewValue);

                                    // tunjuk property primary key dari object
                                    var belongsToType = Type.GetType(propInfo.DestinationAssembly);
                                    var belongsToPKProp = belongsToType.GetProperty(propInfo.ObjectPrimaryKeyProperty);

                                    // tunjuk foreign key property
                                    var fkProp = destType.GetProperty(propInfo.DestinationKeyProperty);

                                    // assign primary key dari object belongsto ke primary key objek utama
                                    fkProp.SetValue(existingParent, belongsToPKProp.GetValue(propNewValue));

                                    // assign objectnya
                                    //var destProp = destType.GetProperty(propNames[0]);
                                    //var propNewValue = destProp.GetValue(instance);
                                    //destProp.SetValue(existingParent, propNewValue);

                                    //// asign key nya
                                    //var destPropKey = destType.GetProperty(propInfo.ObjectPrimaryKeyProperty);
                                    //var fkProp = destType.GetProperty(propInfo.DestinationKeyProperty);
                                    //fkProp.SetValue(existingParent, destPropKey.GetValue(instance));
                                }
                            }
                        }
                        instance = existingParent;
                        destType.GetProperty("UploadRowIndex").SetValue(instance, excelRowIndex);

                        // remove dulu semua data childs-nya (jika ada) untuk proses delete insert childs
                        if (isRemoveChild)
                        {
                            if (config.ChildFields != null && config.ChildFields.Count > 0)
                            {
                                if (commitTodb)
                                {
                                    childs = (IList)destType.GetProperty(config.DestinationChildPropertyName).GetValue(instance);
                                    foreach (var childItem in childs)
                                    {
                                        var entry = _context.Entry(childItem);
                                        entry.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                    }
                                }
                                //ClearRackConfigurationDetails
                                //ClearSupplierTimeTableDetails
                                var methodName = $"Clear{config.DestinationChildPropertyName}";
                                destType.GetMethod(methodName).Invoke(instance, null);
                            }
                        }
                    }

                    resultFiltered.Add(key, (T)instance);
                    result.Add((T)instance);
                }

                #endregion

                if (string.IsNullOrEmpty(config.ChildFullAssemblyName))
                    continue;

                instance = resultFiltered[key];
                childs = (IList)destType.GetProperty(config.DestinationChildPropertyName).GetValue(instance);

                // jika tidak ada child, move next
                if (config.ChildFields == null || config.ChildFields.Count <= 0)
                    continue;

                #region proses untuk child data

                object child = null;
                child = Activator.CreateInstance(childType);
                childType.GetProperty("UploadRowIndex").SetValue(child, excelRowIndex);

                foreach (var propInfo in config.ChildFields)
                {
                    // ambil property child
                    var sourceProperty = sourceType.GetProperty(propInfo.SourceProperty);
                    if (sourceProperty == null)
                        throw new Exception($"Property {propInfo.SourceProperty} not found in class {sourceType.Name}");

                    // ambil konten-nya, jika null dan required, maka add error
                    var propContent = sourceProperty.GetValue(item);
                    if (propContent == null || string.IsNullOrEmpty(propContent.ToString()))
                    {
                        if (propInfo.isRequired)
                            sourceType.GetMethod("AddValidationMessage").Invoke(item, new object[] { $"{propInfo.ExcelFieldTitle} harus diisi." });
                        continue;
                    }

                    // jika bertipe object, maka perlu ditangani khusus karena ada insert foreign key id & object
                    if (propInfo.DestinationPropertyDataType == ObjectMapDetailConfig.DataType.OBJECT)
                    {
                        #region pengisian object belongs-to

                        var belongsToType = Type.GetType(propInfo.DestinationAssembly);
                        if (belongsToType == null)
                            throw new Exception($"Could not load assembly {propInfo.DestinationAssembly}");

                        var propNames = propInfo.DestinationProperty.Split(".");
                        if (propNames.Count() <= 1)
                            throw new Exception($"Invalid object property definition: {propInfo.DestinationProperty}");

                        #endregion

                        // cari di dictionary master data, jika tidak ada maka add error
                        if (!dictMasters.ContainsKey(propInfo.RefTableName))
                        {
                            string errorMessage = $"{propInfo.ExcelFieldTitle} invalid.";
                            childType.GetMethod("AddValidationMessage").Invoke(child, new object[] { errorMessage });
                            sourceType.GetMethod("AddValidationMessage").Invoke(item, new object[] { errorMessage });
                        }
                        else
                        {
                            if (!dictMasters[propInfo.RefTableName].Where(e => belongsToType.GetProperty(propNames[1]).GetValue(e).ToString().ToLower() == propContent.ToString().ToLower()).Any())
                            {
                                string errorMessage = $"{propInfo.ExcelFieldTitle} invalid.";
                                childType.GetMethod("AddValidationMessage").Invoke(child, new object[] { errorMessage });
                                sourceType.GetMethod("AddValidationMessage").Invoke(item, new object[] { errorMessage });
                            }
                            else
                            {
                                
                                //    throw new Exception($"Property {propNames[0]} not found in class {childType.Name}");
                                var belongsToInstance = dictMasters[propInfo.RefTableName].Where(e => belongsToType.GetProperty(propNames[1]).GetValue(e).ToString().ToLower() == propContent.ToString().ToLower()).First();
                                
                                var destProp = childType.GetProperty(propNames[0]);
                                if (destProp != null)
                                    destProp.SetValue(child, belongsToInstance);

                                // tunjuk primary key dari object
                                var belongsToPKProp = belongsToType.GetProperty(propInfo.ObjectPrimaryKeyProperty);
                                if (belongsToPKProp == null)
                                    throw new Exception($"Property {propInfo.ObjectPrimaryKeyProperty} not found in class {belongsToType.Name}");

                                // tunjuk foreign key property
                                var fkProp = childType.GetProperty(propInfo.DestinationKeyProperty);
                                if (fkProp == null)
                                    throw new Exception($"Property {propInfo.DestinationKeyProperty} not found in class {childType.Name}");

                                // assign id foreign key value
                                fkProp.SetValue(child, belongsToPKProp.GetValue(belongsToInstance));
                            }
                        }
                    }
                    else
                    {
                        var destProp = childType.GetProperty(propInfo.DestinationProperty);
                        if (destProp == null)
                            throw new Exception($"Property {propInfo.DestinationProperty} not found in class {childType.Name}");

                        destProp.SetValue(child, propContent);
                    }
                }

                if (commitTodb)
                {
                    var childEntry = _context.Entry(child);
                    childEntry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }

                childs.Add(child);

                #endregion
            }

            #endregion

            return result;
        }
    }
}
