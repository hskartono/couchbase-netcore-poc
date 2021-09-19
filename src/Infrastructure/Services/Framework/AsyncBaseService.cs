using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using QRCoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Services
{
    public class AsyncBaseService<T> : IAsyncBaseService<T> where T : CoreEntity
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly List<string> _errors = new();
        private string _functionId;
        protected UserInfo _user = null;
        protected Dictionary<String, Role> _roles = null;
        protected string _userName;
        protected int _companyId;

        public AsyncBaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Error message holder

        public IReadOnlyList<string> Errors => _errors;

        public void AddError(string errorMessage)
        {
            if (_errors.Contains(errorMessage)) return;
            _errors.Add(errorMessage);
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }

        public bool ServiceState => _errors.Count == 0;

        #endregion;

        #region public methods
        public UserInfo UserInfo
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                if (_user != null)
                {
                    _userName = _user.UserName;
                    _companyId = _user.CompanyId;
                }
            }
        }

        public string UserName
        {
            get
            {
                return _user.UserName;
            }
            set
            {
                _userName = value;
                if (_user == null && !String.IsNullOrEmpty(_userName))
                {
                    _user = _unitOfWork.UserInfoRepository.FirstOrDefaultAsync(new UserInfoFilterSpecification(_userName)).Result;
                }

                if (_user != null)
                {
                    _userName = _user.UserName;
                    _companyId = _user.CompanyId;
                }
            }
        }

        public Dictionary<string, Role> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

        public string FunctionId { get => _functionId; set => _functionId = value; }

        public IUnitOfWork UnitOfWork => _unitOfWork;

        public string BuildHtmlTemplate(string htmlTemplate, Dictionary<string, object> content)
        {
            if (string.IsNullOrEmpty(htmlTemplate) || content == null)
                return string.Empty;

            List<string> keyCollections = new();

            string result = htmlTemplate;
            foreach (string key in content.Keys)
            {
                if (content[key] is IList)
                {
                    keyCollections.Add(key);
                    continue;
                }
                result = result.Replace("{" + key + "}", content[key]?.ToString());
            }

            if (keyCollections.Count == 0) return result;

            // jika ada data many, proses template nya
            string[] lineTemplates = result.Split(Environment.NewLine);
            foreach (string key in keyCollections)
            {
                string replaceAbleLoopContent = "";
                string lineLoopTemplate = "";
                bool isInLoop = false;
                foreach (string lineValue in lineTemplates)
                {
                    if (lineValue.Contains("[" + key + "]"))
                    {
                        lineLoopTemplate = "";
                        replaceAbleLoopContent = lineValue + Environment.NewLine;
                        isInLoop = true;
                        continue;
                    }

                    if (isInLoop)
                    {
                        if (lineValue.Contains("[/" + key + "]"))
                        {
                            replaceAbleLoopContent += lineValue + Environment.NewLine;

                            // proses lineLoop ini
                            string loopResult = "";
                            List<Dictionary<string, string>> childLoops = (List<Dictionary<string, string>>)content[key];
                            foreach (var childRow in childLoops)
                            {
                                string rowResult = lineLoopTemplate;
                                foreach (string childKey in childRow.Keys)
                                {
                                    rowResult = rowResult.Replace("{" + childKey + "}", childRow[childKey]);
                                }
                                loopResult += rowResult + Environment.NewLine;
                            }

                            result = result.Replace(replaceAbleLoopContent, loopResult);

                            // move ke key berikutnya
                            break;
                        }
                        else
                        {
                            lineLoopTemplate += lineValue + Environment.NewLine;
                            replaceAbleLoopContent += lineValue + Environment.NewLine;
                        }
                    }
                }
            }

            return result;
        }

        public async Task<bool> SendToPrintServer(string fileToSend, string functionId, CancellationToken token = default)
        {
            if (!System.IO.File.Exists(fileToSend))
            {
                AddError($"File {fileToSend} tidak dapat ditemukan");
                return false;
            }

            string printerName = "Default";
            var filter = new LookupFilterSpecification("FunctionToPrinterMap");
            var lookup = await _unitOfWork.LookupRepository.FirstOrDefaultAsync(filter, token);
            if (lookup != null)
            {
                var item = lookup.LookupDetails.Where(e => e.Name.ToLower() == functionId.ToLower()).FirstOrDefault();
                if (item != null) printerName = item.Value;
            }

            return await SendToPrintServerManual(fileToSend, printerName, token);
        }

        public async Task<bool> SendToPrintServerManual(string fileToSend, string printerName, CancellationToken token = default)
        {
            if (!System.IO.File.Exists(fileToSend))
            {
                AddError($"File {fileToSend} tidak dapat ditemukan");
                return false;
            }

            // prepare payload
            Byte[] bytes = System.IO.File.ReadAllBytes(fileToSend);
            String document = Convert.ToBase64String(bytes);
            string filename = System.IO.Path.GetFileName(fileToSend);
            string userName = "ppos";
            string apiUrl = "";

            #region Load konfigurasi print server
            var filter = new LookupDetailFilterSpecification("printerUserName");
            var lookupDetail = await _unitOfWork.LookupDetailRepository.FirstOrDefaultAsync(filter, token);
            if (lookupDetail != null)
            {
                userName = lookupDetail.Value;
            }

            filter = new LookupDetailFilterSpecification("printerApiUrl");
            lookupDetail = await _unitOfWork.LookupDetailRepository.FirstOrDefaultAsync(filter, token);
            if (lookupDetail != null)
            {
                apiUrl = lookupDetail.Value;
            }
            #endregion

            // hit API untuk remote print
            PrintRequest request = new(document, filename, printerName, userName);
            HttpClient client = new();
            var response = await client.PostAsJsonAsync(apiUrl, request, token);
            if (!response.IsSuccessStatusCode)
            {
                AddError("Gagal mengirimkan request print ke print server");
            }

            return response.IsSuccessStatusCode;
        }

        #endregion

        #region protected methods

        protected void InitEPPlusLicense()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        protected void AssignCreatorAndCompany(BaseEntity entity)
        {
            entity.CompanyId = _companyId;
            entity.CreatedBy = _userName;
            entity.CreatedDate = DateTime.Now;
        }

        protected void AssignUpdater(BaseEntity entity)
        {
            entity.CompanyId = _companyId;
            entity.UpdatedBy = _userName;
            entity.UpdatedDate = DateTime.Now;
        }

        protected async Task<string> GenerateNumber(string functionId, int recordId, CancellationToken cancellationToken = default)
        {
            var spLookupDetail = await _unitOfWork.LookupDetailRepository.FirstOrDefaultAsync(new LookupDetailFilterSpecification(functionId, 1), cancellationToken);
            if (spLookupDetail == null) return string.Empty;

            var result = _unitOfWork.GenericRepository.ExecStoredProcedureNumberGenerator(spLookupDetail.Value, recordId);
            if (result == null) return string.Empty;
            return result.NumberGenerated;
        }

        protected string GenerateQrCodeBase64(string content)
        {
            QRCodeGenerator qrCodeGenerator = new();
            QRCodeData data = qrCodeGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(data);
            Bitmap val = qrCode.GetGraphic(20);
            System.IO.MemoryStream ms = new();
            val.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            var base64 = Convert.ToBase64String(byteImage);
            return base64;
        }

        #endregion

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        #region excel helper

        protected ExcelPackage DataTableToExcel(DataTable dt, List<string> columnKeys = null)
        {
            if (dt == null)
                return null;

            var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Worksheet 1");
            int row = 1, col = 1, dtCol = 0;
            List<string> headerInfo = new();

            // draw excel header
            foreach (DataColumn colInfo in dt.Columns)
            {
                if (columnKeys == null)
                {
                    ws.Cells[row, col].Value = colInfo.ColumnName;
                    headerInfo.Add(colInfo.ColumnName);
                    col++;
                }
                else if (columnKeys.Contains(colInfo.ColumnName))
                {
                    ws.Cells[row, col].Value = colInfo.ColumnName;
                    headerInfo.Add(colInfo.ColumnName);
                    col++;
                }
                dtCol++;
            }
            ws.Cells[1, 1, 1, col].Style.Font.Bold = true;

            // draw excel content
            row = 2;
            foreach (DataRow rowInfo in dt.Rows)
            {
                col = 0;
                int excelCol = 1;
                foreach (string title in headerInfo)
                {
                    if (columnKeys == null)
                    {
                        ws.Cells[row, excelCol].Value = rowInfo[col];
                        excelCol++;
                    }
                    else if (columnKeys.Contains(title))
                    {

                        ws.Cells[row, excelCol].Value = rowInfo[col];
                        excelCol++;
                    }
                    col++;
                }
                row++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return package;
        }

        protected List<Dictionary<string, Object>> ExcelToDictionary(String excelFileName, bool firstRowTitle = true)
        {
            List<Dictionary<string, Object>> results = new();

            if (!System.IO.File.Exists(excelFileName))
                return results;

            using (var package = new ExcelPackage(new FileInfo(excelFileName)))
            {
                var ws = package.Workbook.Worksheets[0];
                if (ws == null)
                    return results;

                if (ws.Dimension == null || ws.Dimension.End == null)
                    return results;

                int maxRow = ws.Dimension.End.Row;
                int maxCol = ws.Dimension.End.Column;

                // prepare header title, make sure there is no duplicate header title
                int startRow = 1;
                List<string> title = new();
                Dictionary<string, int> headerDuplicateCounter = new();
                if (firstRowTitle)
                {
                    for (int col = 1; col <= maxCol; col++)
                    {
                        var value = ws.Cells[startRow, col].Value;
                        if (value == null) value = "NULL";
                        if (!headerDuplicateCounter.ContainsKey((string)value))
                            headerDuplicateCounter.Add((string)value, 1);

                        if (title.Contains((string)value))
                        {
                            title.Add($"{(string)value}{headerDuplicateCounter[(string)value]}");
                            headerDuplicateCounter[(string)value]++;
                        }
                        else
                        {
                            title.Add((string)value);
                        }
                    }
                    startRow = 2;
                }
                else
                {
                    for (int col = 1; col <= maxCol; col++)
                        title.Add(col.ToString());
                }

                // read all content and add to list results
                for (int row = startRow; row <= maxRow; row++)
                {
                    var rowValue = new Dictionary<string, object>();
                    for (int col = 1; col <= maxCol; col++)
                    {
                        var value = ws.Cells[row, col].Value;
                        rowValue.Add(title[col - 1], value);
                    }
                    results.Add(rowValue);
                }
            }

            return results;
        }

        #endregion

        protected string AutorenameFileName(string fullPath)
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
    }
}
