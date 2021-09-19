using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Utility
{
	public class ExcelMapper
	{
		public static ExcelToObjectMapConfig LoadConfig(string configFileName)
		{
			string jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", configFileName);
			if (!File.Exists(jsonFile))
				throw new FileNotFoundException("Could not load configuration", configFileName);

			return Newtonsoft.Json.JsonConvert.DeserializeObject<ExcelToObjectMapConfig>(File.ReadAllText(jsonFile));
		}

		public static List<T> ReadFromExcel<T>(string excelFileName, string configFileName)
		{
			var mapper = ExcelMapper.LoadConfig(configFileName);
			return ExcelMapper.ReadFromExcel<T>(excelFileName, mapper);
		}

		private static string getWorksheetName(string worksheet)
		{
			string safeWorksheet = worksheet.Substring(0, (worksheet.Length >= 31) ? 31 : worksheet.Length);
			return safeWorksheet;
		}

		public static List<T> ReadFromExcel<T>(string excelFileName, ExcelToObjectMapConfig mapper)
		{
			if (!File.Exists(excelFileName))
				throw new FileNotFoundException("Could not load excel file", excelFileName);

			if (string.IsNullOrEmpty(excelFileName))
				throw new ArgumentNullException(nameof(excelFileName));

			if (mapper == null)
				throw new ArgumentNullException(nameof(mapper));

			List<T> result = new List<T>();
			Dictionary<int, List<string>> validations = new Dictionary<int, List<string>>();
			Dictionary<string, ExcelWorksheet> worksheets = new Dictionary<string, ExcelWorksheet>();
			Dictionary<string, List<object>> childsData = new Dictionary<string, List<object>>();
			Dictionary<string, Dictionary<int, List<string>>> childsValidation = new Dictionary<string, Dictionary<int, List<string>>>();

			// load excel
			initExcelLicense();
			using (var package = new ExcelPackage(new FileInfo(excelFileName)))
			{
				// load main worksheet
				string worksheetName = getWorksheetName(mapper.worksheet);
				// var ws = package.Workbook.Worksheets[mapper.worksheet];
				var ws = package.Workbook.Worksheets[worksheetName];
				if (ws == null)
					return null;

				ProcessReadWorksheet<T>(mapper, result, validations, ws);

				Dictionary<string, ExcelToObjectMapConfig> collectionProperties = new Dictionary<string, ExcelToObjectMapConfig>();
				// load child worksheets
				foreach (var item in mapper.columnsMap)
				{
					if (item.type.ToLower() == "collection")
					{
						var config = item.LoadRelatedCollection();
						collectionProperties.Add(item.property, config);

						worksheetName = getWorksheetName(config.worksheet);
						//ws = package.Workbook.Worksheets[config.worksheet];
						ws = package.Workbook.Worksheets[worksheetName];
						if (ws == null)
							return null;

						worksheets.Add(item.property, ws);
						childsData.Add(item.property, new List<object>());
						childsValidation.Add(item.property, new Dictionary<int, List<string>>());
						ProcessReadWorksheet(config, childsData[item.property], childsValidation[item.property], worksheets[item.property]);
					}
				}

				// assign childs data ke main worksheet
				Type mType = Type.GetType(mapper.assembly);
				foreach (var item in result)
				{
					var primaryKey = mType.GetProperty("_RefPrimaryKey").GetValue(item);
					foreach (var propertyName in collectionProperties.Keys)
					{
						Type childType = Type.GetType(collectionProperties[propertyName].assembly);
						Type genericListType = typeof(List<>);
						Type concreteListType = genericListType.MakeGenericType(childType);
						IList childs = Activator.CreateInstance(concreteListType) as IList;
						foreach (var data in childsData[propertyName])
						{
							var foreignKey = childType.GetProperty("_RefForeignKey").GetValue(data);
							if (foreignKey.Equals(primaryKey))
							{
								childs.Add(data);
							}
						}
						mType.GetProperty(propertyName).SetValue(item, childs);
					}
				}
			}

			return result;
		}

		private static void ProcessReadWorksheet<T>(ExcelToObjectMapConfig mapper, List<T> result, Dictionary<int, List<string>> validations, ExcelWorksheet ws)
		{
			int maxRow = ws.Dimension.End.Row;
			Type mType = Type.GetType(mapper.assembly);
			List<string> numericTypes = new List<string>() { "int", "long", "single", "double", "decimal" };

			for (int y = 2; y <= maxRow; y++)
			{
				int rowNumber = y - 1;
				validations.Add(rowNumber, new List<string>());

				//var itemInstance = Activator.CreateInstance<T>();
				var itemInstance = Activator.CreateInstance(mType);
				foreach (PropertyInfo prop in mType.GetProperties())
				{
					var propConfig = mapper.columnsMap.Where(e => e.property.ToLower() == prop.Name.ToLower()).SingleOrDefault();
					if (propConfig == null) continue;
					if (propConfig.type.ToLower() == "collection") continue;

					var value = ws.Cells[y, propConfig.colIndex].Value;
					if (propConfig.isRequired)
					{
						if (value == null)
						{
							// validations[rowNumber].Add($"{prop.Name} pada baris {rowNumber} harus diisi");
							validations[rowNumber].Add($"{prop.Name} harus diisi.");
							continue;
						}

						// jika tidak null, cek jika numeric atau date/time
						if (numericTypes.Contains(propConfig.type.ToLower()) && (value.ToString() == "" || double.Parse(value.ToString()) == 0))
						{
							//validations[rowNumber].Add($"{prop.Name} pada baris {rowNumber} harus diisi");
							validations[rowNumber].Add($"{prop.Name} harus diisi.");
							continue;
						}

						if (propConfig.type.ToLower() == "datetime" && ((DateTime)value) == DateTime.MinValue)
						{
							//validations[rowNumber].Add($"{prop.Name} pada baris {rowNumber} harus diisi");
							validations[rowNumber].Add($"{prop.Name} harus diisi.");
							continue;
						}
					}

					// object data type
					if (propConfig.type.ToLower() == "object" && value != null)
					{
						if (prop.PropertyType == typeof(int))
						{
							int valueId = 0;
							if (!int.TryParse(value.ToString(), out valueId))
							{
								//validations[rowNumber].Add($"Nilai {prop.Name} pada baris {rowNumber} harus berupa angka.");
								validations[rowNumber].Add($"Nilai {prop.Name} harus berupa angka.");
								continue;
							}
							prop.SetValue(itemInstance, valueId);
						}
						else if (prop.PropertyType == typeof(int?))
						{
							int? valueId = null;
							int parsedValueId = 0;
							if (!int.TryParse(value.ToString(), out parsedValueId))
							{
								validations[rowNumber].Add($"Nilai {prop.Name} harus berupa angka.");
								continue;
							}
							if (parsedValueId > 0) valueId = parsedValueId;
							prop.SetValue(itemInstance, valueId);
						}
						else
						{
							prop.SetValue(itemInstance, value);
						}
					}
					else if (propConfig.type.ToLower() == "collection")
					{
						// collection
					}
					else
					{
						// primitive data type
						if (propConfig.PK || propConfig.FK)
						{
							if (value != null)
								prop.SetValue(itemInstance, value.ToString());
						}
						else
						{
							if (numericTypes.Contains(propConfig.type.ToLower()))
							{
								bool isNull = false;
								if (value == null)
								{
									isNull = true;
									value = 0;
								}
								
								//  "int", "long", "single", "double", "decimal"
								object valueObject;
								if (propConfig.type.ToLower() == "int")
								{
									if (int.TryParse(value.ToString(), out _) == false) value = 0;
								}
								else if (propConfig.type.ToLower() == "long")
								{
									if (long.TryParse(value.ToString(), out _) == false) value = 0;
								}
								else if (propConfig.type.ToLower() == "single")
								{
									if (Single.TryParse(value.ToString(), out _) == false) value = 0;
								}
								else if (propConfig.type.ToLower() == "double")
								{
									if (double.TryParse(value.ToString(), out _) == false) value = 0;
								}
								else if (propConfig.type.ToLower() == "decimal")
								{
									if (decimal.TryParse(value.ToString(), out _) == false) value = 0;
								}


								bool setValue = true;
								if(isNull)
                                {
									if (propConfig.defaultNull)
									{
										setValue = false;
									}
								}

								if (setValue)
                                {
									if (prop.PropertyType.Name == typeof(Nullable<>).Name)
									{
										valueObject = Convert.ChangeType(value, prop.PropertyType.GenericTypeArguments[0]);
									}
									else
									{
										valueObject = Convert.ChangeType(value, prop.PropertyType);
									}
									prop.SetValue(itemInstance, valueObject); // int.Parse(value.ToString()));
								}
							}
							else if (propConfig.type.ToLower() == "datetime" && value != null)
							{
								var valueText = ws.Cells[y, propConfig.colIndex].Text;
								DateTime dtCheck;
								if (DateTime.TryParse(valueText, out dtCheck) == false)
                                {
									validations[rowNumber].Add($"Nilai {prop.Name} harus berupa tanggal/waktu.");
								} else
                                {
									prop.SetValue(itemInstance, dtCheck);
								}

								//if (value.GetType().Name == "DateTime")
								//{
								//	prop.SetValue(itemInstance, value);
								//}
								//else
								//{
								//	long dateNum = Convert.ToInt64(value);
								//	DateTime dtCheck = DateTime.FromOADate(dateNum);
								//	prop.SetValue(itemInstance, dtCheck);
								//}
							}
							else if (propConfig.type.ToLower() == "string")
							{
								prop.SetValue(itemInstance, value?.ToString());
							}
							else if (propConfig.type.ToLower() == "boolean")
                            {
								if(value != null)
                                {
									if(value.ToString().ToLower() == "true")
                                    {
										prop.SetValue(itemInstance, true);
									} else if (value.ToString().ToLower() == "false")
									{
										prop.SetValue(itemInstance, false);
									} else
                                    {
										validations[rowNumber].Add($"Isi pada kolom {propConfig.title} harus TRUE/FALSE.");
										prop.SetValue(itemInstance, default);
                                    }
                                }
                                else
                                {
									validations[rowNumber].Add($"Isi pada kolom {propConfig.title} harus TRUE/FALSE.");
									prop.SetValue(itemInstance, default);
								}
                            }
							else
							{
								prop.SetValue(itemInstance, value);
							}
						}
					}
				}

				result.Add((T)itemInstance);
			}

			int index = 1;
			foreach (var item in result)
			{
				if (mType.GetProperty("UploadValidationStatus") == null) continue;
				mType.GetProperty("UploadValidationStatus").SetValue(item, "Success");
				if (validations[index].Count > 0)
				{
					string err = string.Join(Environment.NewLine, validations[index]);
					mType.GetProperty("UploadValidationMessage").SetValue(item, err);
					mType.GetProperty("UploadValidationStatus").SetValue(item, "Failed");
				}
				index++;
			}
		}

		public static bool WriteToExcel<T>(string excelFileName, string configFileName, IReadOnlyList<T> data)
        {
			return WriteToExcelWithAutoRenameFile<T>(ref excelFileName, configFileName, data, false);
        }

		public static bool WriteToExcelWithAutoRenameFile<T>(ref string excelFileName, string configFileName, IReadOnlyList<T> data, bool autoRenameFile)
		{
			var mapper = ExcelMapper.LoadConfig(configFileName);
			return ExcelMapper.WriteToExcelWithAutoRenameFile<T>(ref excelFileName, mapper, data, autoRenameFile);
		}

		public static bool WriteToExcel<T>(string excelFileName, ExcelToObjectMapConfig mapper, IReadOnlyList<T> data)
        {
			return WriteToExcelWithAutoRenameFile<T>(ref excelFileName, mapper, data, false);
        }

		public static bool WriteToExcelWithAutoRenameFile<T>(ref string excelFileName, ExcelToObjectMapConfig mapper, IReadOnlyList<T> data, bool autoRenameFile)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			if (mapper == null)
				throw new ArgumentNullException(nameof(mapper));
			if (string.IsNullOrEmpty(excelFileName))
				throw new ArgumentNullException(nameof(excelFileName));

            if (autoRenameFile)
            {
				excelFileName = AutorenameFileName(excelFileName);
            }

			initExcelLicense();
			using (var package = new ExcelPackage())
			{
				Dictionary<string, ExcelWorksheet> wsChilds = new Dictionary<string, ExcelWorksheet>();
				Dictionary<string, int> wsChildsRow = new Dictionary<string, int>();
				string worksheetName = getWorksheetName(mapper.worksheet);
				//var ws = package.Workbook.Worksheets.Add(mapper.worksheet);
				var ws = package.Workbook.Worksheets.Add(worksheetName);
				// draw header
				int maxCol = 1;
				foreach (var item in mapper.columnsMap)
				{
					if (item.type.ToLower() == "collection") continue;
					ws.Cells[1, item.colIndex].Value = item.title;
					maxCol++;
				}
				ws.Cells[1, 1, 1, maxCol].Style.Font.Bold = true;

				Type itemType = Type.GetType(mapper.assembly);
				//var refPK = 1;
				var xlRow = 2;
				foreach (var itemObj in data)
				{
					string foreignKey = Convert.ToString(itemType.GetProperty(mapper.columnsMap[0].property).GetValue(itemObj));
					itemType.GetProperty("_RefForeignKey").SetValue(itemObj, foreignKey);
					foreach (var map in mapper.columnsMap)
					{
						if (map.type.ToLower() == "collection")
						{
							var mapChild = map.LoadRelatedCollection();
							worksheetName = getWorksheetName(mapChild.worksheet);
							//if (!wsChilds.ContainsKey(mapChild.worksheet))
							if (!wsChilds.ContainsKey(worksheetName))
							{
								// wsChilds.Add(mapChild.worksheet, package.Workbook.Worksheets.Add(mapChild.worksheet));
								wsChilds.Add(worksheetName, package.Workbook.Worksheets.Add(worksheetName));
								//wsChildsRow.Add(mapChild.worksheet, 2);
								wsChildsRow.Add(worksheetName, 2);

								// draw header
								maxCol = 1;
								foreach (var itemMapChild in mapChild.columnsMap)
								{
									if (itemMapChild.type.ToLower() == "collection") continue;
									//wsChilds[mapChild.worksheet].Cells[1, itemMapChild.colIndex].Value = itemMapChild.title;
									wsChilds[worksheetName].Cells[1, itemMapChild.colIndex].Value = itemMapChild.title;
									maxCol++;
								}
								//wsChilds[mapChild.worksheet].Cells[1, 1, 1, maxCol].Style.Font.Bold = true;
								wsChilds[worksheetName].Cells[1, 1, 1, maxCol].Style.Font.Bold = true;
							}

							//var wsChild = wsChilds[mapChild.worksheet];
							var wsChild = wsChilds[worksheetName];
							var childObj = (IList)itemType.GetProperty(map.property).GetValue(itemObj);
							var itemChildType = Type.GetType(mapChild.assembly);
							foreach (var itemChildObj in childObj)
							{
								itemChildType.GetProperty("_RefPrimaryKey").SetValue(itemChildObj, foreignKey);
								foreach (var itemMap in mapChild.columnsMap)
								{
									if (itemMap.type.ToLower() == "collection")
									{
										// belum support 3 level
									}
									else
									{
										//wsChild.Cells[wsChildsRow[mapChild.worksheet], itemMap.colIndex].Value = itemChildType.GetProperty(itemMap.property).GetValue(itemChildObj);
										wsChild.Cells[wsChildsRow[worksheetName], itemMap.colIndex].Value = itemChildType.GetProperty(itemMap.property).GetValue(itemChildObj);
										if (itemMap.type.ToLower() == "datetime")
										{
											DateTime dt = (DateTime)itemChildType.GetProperty(itemMap.property).GetValue(itemChildObj); ;
											if (dt == DateTime.MinValue || dt.Date == new DateTime(1899, 12, 30))
												//wsChild.Cells[wsChildsRow[mapChild.worksheet], itemMap.colIndex].Value = "";
												wsChild.Cells[wsChildsRow[worksheetName], itemMap.colIndex].Value = "";
											else
												//wsChild.Cells[wsChildsRow[mapChild.worksheet], itemMap.colIndex].Style.Numberformat.Format = "dd-MMM-yyyy";
												wsChild.Cells[wsChildsRow[worksheetName], itemMap.colIndex].Style.Numberformat.Format = "dd-MMM-yyyy";
										}
									}
								}

								//wsChildsRow[mapChild.worksheet]++;
								wsChildsRow[worksheetName]++;
							}

							wsChild.Cells[wsChild.Dimension.Address].AutoFitColumns();
						}
						else
						{
							ws.Cells[xlRow, map.colIndex].Value = itemType.GetProperty(map.property).GetValue(itemObj);
							if (map.type.ToLower() == "datetime")
							{
								var itemVal = itemType.GetProperty(map.property).GetValue(itemObj);
								DateTime? dt = null;
								if(itemVal == null)
                                {
									ws.Cells[xlRow, map.colIndex].Value = "";
								} else
                                {
									dt = (DateTime)itemVal;
									if (dt == DateTime.MinValue || dt.Value.Date == new DateTime(1899, 12, 30))
										ws.Cells[xlRow, map.colIndex].Value = "";
                                    else
                                    {
										if (string.IsNullOrEmpty(map.format)) map.format = "dd-MMM-yyyy";
										ws.Cells[xlRow, map.colIndex].Style.Numberformat.Format = map.format;
									}
								}
							}
						}
					}

					//refPK++;
					xlRow++;
				}
				ws.Cells[ws.Dimension.Address].AutoFitColumns();

				FileOutputUtil.OutputDir = new DirectoryInfo(Path.GetDirectoryName(excelFileName));
				var xFile = FileOutputUtil.GetFileInfo(Path.GetFileName(excelFileName));
				package.SaveAs(xFile);
			}

			return true;
		}

		private static string AutorenameFileName(string fullPath)
        {
			int count = 1;

			string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
			string extension = Path.GetExtension(fullPath);
			string path = Path.GetDirectoryName(fullPath);
			string newFullPath = fullPath;

			while (File.Exists(newFullPath))
			{
				string tempFileName = string.Format("{0}_{1}", fileNameOnly, count++);
				newFullPath = Path.Combine(path, tempFileName + extension);
			}

			return newFullPath;
		}

		public static void initExcelLicense()
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
		}
	}
}
