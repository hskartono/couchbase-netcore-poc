using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using AppCoreApi.Infrastructure.Utility;
using Ardalis.Specification;
using Hangfire;
using iText.Html2pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace AppCoreApi.Infrastructure.Services
{
    public class RoleManagementDetailService : AsyncBaseService<RoleManagementDetail>, IRoleManagementDetailService
    {

        #region appgen: private variable

        private readonly IDownloadProcessService _downloadProcessService;
        private readonly IUriComposer _uriComposer;

        #endregion

        #region appgen: constructor

        public RoleManagementDetailService(
            IUnitOfWork unitOfWork,
            IDownloadProcessService downloadProcessService,
            IUriComposer uriComposer) : base(unitOfWork)
        {
            _downloadProcessService = downloadProcessService;
            _uriComposer = uriComposer;
        }

        #endregion

        #region CRUD Operations

        #region appgen: add
        public async Task<RoleManagementDetail> AddAsync(RoleManagementDetail entity, CancellationToken cancellationToken = default)
        {
            if (!ValidateOnInsert(entity))
                return null;

            AssignCreatorAndCompany(entity);



            await _unitOfWork.RoleManagementDetailRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return entity;
        }
        #endregion

        #region appgen: count
        public async Task<int> CountAsync(ISpecification<RoleManagementDetail> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementDetailRepository.CountAsync(spec, cancellationToken);
        }
        #endregion

        #region appgen: delete
        public async Task<bool> DeleteAsync(RoleManagementDetail entity, CancellationToken cancellationToken = default)
        {
            _unitOfWork.RoleManagementDetailRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }
        #endregion

        #region appgen: first record
        public async Task<RoleManagementDetail> FirstAsync(ISpecification<RoleManagementDetail> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementDetailRepository.FirstAsync(spec, cancellationToken);
        }
        #endregion

        #region appgen: first or default
        public async Task<RoleManagementDetail> FirstOrDefaultAsync(ISpecification<RoleManagementDetail> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementDetailRepository.FirstOrDefaultAsync(spec, cancellationToken);
        }
        #endregion

        #region appgen: get by id
        public async Task<RoleManagementDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id, false, cancellationToken);
        }

        private async Task<RoleManagementDetail> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
        {
            var specFilter = new RoleManagementDetailFilterSpecification(id, true);
            var roleManagementDetail = await _unitOfWork.RoleManagementDetailRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
            if (roleManagementDetail == null || includeChilds == false)
                return roleManagementDetail;



            return roleManagementDetail;
        }
        #endregion

        #region appgen: list all
        public async Task<IReadOnlyList<RoleManagementDetail>> ListAllAsync(List<SortingInformation<RoleManagementDetail>> sorting, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementDetailRepository.ListAllAsync(sorting, cancellationToken);
        }
        #endregion

        #region appgen: get list
        public async Task<IReadOnlyList<RoleManagementDetail>> ListAsync(
            ISpecification<RoleManagementDetail> spec,
            List<SortingInformation<RoleManagementDetail>> sorting,
            bool withChilds = false,
            CancellationToken cancellationToken = default)
        {
            var roleManagementDetails = await _unitOfWork.RoleManagementDetailRepository.ListAsync(spec, sorting, cancellationToken);
            if (withChilds && roleManagementDetails?.Count > 0)
            {
                var results = new List<RoleManagementDetail>(roleManagementDetails);
                var roleManagementDetailIds = roleManagementDetails.Select(e => e.Id).ToList();



                return results;
            }

            return roleManagementDetails;
        }
        #endregion

        #region appgen: update
        public async Task<bool> UpdateAsync(RoleManagementDetail entity, CancellationToken cancellationToken = default)
        {
            if (!ValidateOnUpdate(entity))
                return false;

            cancellationToken.ThrowIfCancellationRequested();

            // update header
            AssignUpdater(entity);
            await _unitOfWork.RoleManagementDetailRepository.ReplaceAsync(entity, entity.Id, cancellationToken);

            var oldEntity = await _unitOfWork.RoleManagementDetailRepository.GetByIdAsync(entity.Id, cancellationToken);
            if (oldEntity == null)
            {
                AddError($"Could not load {nameof(entity)} data with id {entity.Id}.");
                return false;
            }


            // update & commit
            //await _unitOfWork.RoleManagementDetailRepository.UpdateAsync(oldEntity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }


        #endregion

        #endregion

        #region Validation Operations

        #region appgen: validatebase
        private bool ValidateBase(RoleManagementDetail entity)
        {
            if (entity == null)
                AddError("Tidak dapat menyimpan data kosong.");



            return ServiceState;
        }
        #endregion

        #region appgen: validateoninsert
        private bool ValidateOnInsert(RoleManagementDetail entity)
        {
            ValidateBase(entity);

            return ServiceState;
        }
        #endregion

        #region appgen: validateonupdate
        private bool ValidateOnUpdate(RoleManagementDetail entity)
        {
            ValidateBase(entity);

            return ServiceState;
        }
        #endregion

        #endregion

        #region PDF Related

        #region appgen: generate pdf single
        public string GeneratePdf(RoleManagementDetail entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            // read template
            string templateFile = _uriComposer.ComposeTemplatePath("role_management_detail.html");
            string htmlContent = File.ReadAllText(templateFile);

            htmlContent = MapTemplateValue(htmlContent, entity);

            // prepare destination pdf
            string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
            string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
            ConverterProperties converterProperties = new();
            HtmlConverter.ConvertToPdf(htmlContent, new FileStream(fullPdfFileName, FileMode.Create), converterProperties);

            return fullPdfFileName;
        }
        #endregion

        #region appgen: generate pdf multipage
        public async Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            cancellationToken.ThrowIfCancellationRequested();

            var items = await this.ListAsync(new RoleManagementDetailFilterSpecification(ids), null, true, cancellationToken);
            if (items == null || items.Count <= 0)
            {
                AddError($"Could not get data for list of id {ids.ToArray()}");
                return null;
            }

            string templateFile = _uriComposer.ComposeTemplatePath("role_management_detail.html");
            string htmlContent = File.ReadAllText(templateFile);
            string result = "";

            foreach (var item in items)
            {
                result += MapTemplateValue(htmlContent, item);
            }

            // prepare destination pdf
            string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
            string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
            ConverterProperties converterProperties = new();
            HtmlConverter.ConvertToPdf(result, new FileStream(fullPdfFileName, FileMode.Create), converterProperties);

            if (refId.HasValue && refId.Value > 0)
            {
                await _downloadProcessService.SuccessfullyGenerated(refId.Value, pdfFileName, cancellationToken);
            }

            return fullPdfFileName;
        }
        #endregion

        #region appgen: generate pdf background process
        public async Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // insert dulu ke database
            var downloadProcess = new DownloadProcess("role_management_detail") { StartTime = DateTime.Now };
            var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
            if (result == null)
            {
                AddError("Failed to insert download process");
                return null;
            }

            // lempar ke background process
            var jobId = BackgroundJob.Enqueue(() => GeneratePdfMultiPage(ids, result.Id, cancellationToken));

            // update background job id
            result.JobId = jobId;
            await _downloadProcessService.UpdateAsync(result, cancellationToken);

            // return background job id
            return jobId;
        }
        #endregion

        #region appgen: mapp templates
        private string MapTemplateValue(string htmlContent, RoleManagementDetail entity)
        {
            Dictionary<string, object> mapper = new()
                {
                    {"Id",""},
                    {"AllowCreate",""},
                    {"AllowRead",""},
                    {"AllowUpdate",""},
                    {"AllowDelete",""},
                    {"ShowInMenu",""},
                    {"AllowDownload",""},
                    {"AllowPrint",""},
                    {"AllowUpload",""},

                };

            if (entity != null)
            {
                mapper["Id"] = entity.Id.ToString();
                mapper["RoleManagement"] = entity.RoleManagement.Name;
                mapper["FunctionInfo"] = entity.FunctionInfo.Name;
                mapper["AllowCreate"] = (entity.AllowCreate.HasValue && entity.AllowCreate.Value) ? "Yes" : "No";
                mapper["AllowRead"] = (entity.AllowRead.HasValue && entity.AllowRead.Value) ? "Yes" : "No";
                mapper["AllowUpdate"] = (entity.AllowUpdate.HasValue && entity.AllowUpdate.Value) ? "Yes" : "No";
                mapper["AllowDelete"] = (entity.AllowDelete.HasValue && entity.AllowDelete.Value) ? "Yes" : "No";
                mapper["ShowInMenu"] = (entity.ShowInMenu.HasValue && entity.ShowInMenu.Value) ? "Yes" : "No";
                mapper["AllowDownload"] = (entity.AllowDownload.HasValue && entity.AllowDownload.Value) ? "Yes" : "No";
                mapper["AllowPrint"] = (entity.AllowPrint.HasValue && entity.AllowPrint.Value) ? "Yes" : "No";
                mapper["AllowUpload"] = (entity.AllowUpload.HasValue && entity.AllowUpload.Value) ? "Yes" : "No";

            }

            return BuildHtmlTemplate(htmlContent, mapper);
        }
        #endregion

        #endregion

        #region Excel Related

        #region appgen: generate excel background process
        public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
            int? id = null, List<int> roleManagements = null, List<string> functionInfos = null, List<bool> allowCreates = null, List<bool> allowReads = null, List<bool> allowUpdates = null, List<bool> allowDeletes = null, List<bool> showInMenus = null, List<bool> allowDownloads = null, List<bool> allowPrints = null, List<bool> allowUploads = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // insert dulu ke database
            var downloadProcess = new DownloadProcess("role_management_detail") { StartTime = DateTime.Now };
            var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
            if (result == null)
            {
                AddError("Failed to insert download process");
                return null;
            }

            // lempar ke background process
            var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
                id, roleManagements, functionInfos, allowCreates, allowReads, allowUpdates, allowDeletes, showInMenus, allowDownloads, allowPrints, allowUploads,
                exact,
                cancellationToken));

            result.JobId = jobId;
            await _downloadProcessService.UpdateAsync(result, cancellationToken);

            // return background job id
            return jobId;
        }
        #endregion

        #region appgen: generate excel process
        public async Task<string> GenerateExcel(string excelFilename, int? refId = null,
            int? id = null, List<int> roleManagements = null, List<string> functionInfos = null, List<bool> allowCreates = null, List<bool> allowReads = null, List<bool> allowUpdates = null, List<bool> allowDeletes = null, List<bool> showInMenus = null, List<bool> allowDownloads = null, List<bool> allowPrints = null, List<bool> allowUploads = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                RoleManagementDetailFilterSpecification filterSpec = null;
                if (id.HasValue)
                    filterSpec = new RoleManagementDetailFilterSpecification(id.Value);
                else
                    filterSpec = new RoleManagementDetailFilterSpecification(exact)
                    {

                        Id = id,
                        RoleManagementIds = roleManagements,
                        FunctionInfoIds = functionInfos,
                        AllowCreates = allowCreates,
                        AllowReads = allowReads,
                        AllowUpdates = allowUpdates,
                        AllowDeletes = allowDeletes,
                        ShowInMenus = showInMenus,
                        AllowDownloads = allowDownloads,
                        AllowPrints = allowPrints,
                        AllowUploads = allowUploads
                    }.BuildSpecification();

                var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (ExcelMapper.WriteToExcel<RoleManagementDetail>(excelFilename, "roleManagementDetail.json", results) == false)
                {
                    if (refId.HasValue)
                        await _downloadProcessService.FailedToGenerate(refId.Value, "Failed to generate excel file", cancellationToken);
                    return "";
                }

                // update database information (if needed)
                if (refId.HasValue)
                {
                    excelFilename = Path.GetFileName(excelFilename);
                    await _downloadProcessService.SuccessfullyGenerated(refId.Value, excelFilename, cancellationToken);
                }

                return excelFilename;
            }
            catch (Exception ex)
            {
                if (refId.HasValue)
                    await _downloadProcessService.FailedToGenerate(refId.Value, ex.Message, cancellationToken);

                throw;
            }
        }
        #endregion

        #region appgen: upload excel
        public async Task<List<RoleManagementDetail>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
        {
            var result = ExcelMapper.ReadFromExcel<RoleManagementDetail>(tempExcelFile, "roleManagementDetail.json");
            if (result == null)
            {
                AddError("Format template excel tidak dikenali. Silahkan download template dari menu download.");
                return null;
            }

            SetUploadDraftFlags(result);

            await RunMasterDataValidation(result, cancellationToken);

            foreach (var item in result)
            {
                var id = item.Id;
                if (id > 0)
                    await _unitOfWork.RoleManagementDetailRepository.UpdateAsync(item, cancellationToken);
                else
                    await _unitOfWork.RoleManagementDetailRepository.AddAsync(item, cancellationToken);

            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return result;
        }

        private async Task RunMasterDataValidation(List<RoleManagementDetail> result, CancellationToken cancellationToken)
        {
            var excelRoleManagementIds1 = result.Where(s => s.RoleManagementId.HasValue).Select(s => s.RoleManagementId.Value).ToList();
            var RoleManagements = await _unitOfWork.RoleManagementRepository.ListAsync(new RoleManagementFilterSpecification(excelRoleManagementIds1), null, cancellationToken);
            var RoleManagementIds = RoleManagements.Select(e => e.Id);
            var excelFunctionInfoIds1 = result.Where(s => !string.IsNullOrEmpty(s.FunctionInfoId)).Select(s => s.FunctionInfoId).ToList();
            var FunctionInfos = await _unitOfWork.FunctionInfoRepository.ListAsync(new FunctionInfoFilterSpecification(0, 0), null, cancellationToken);
            var FunctionInfoIds = FunctionInfos.Select(e => e.Id);

        }

        private void SetUploadDraftFlags(List<RoleManagementDetail> result)
        {
            foreach (var item in result)
            {
                item.isFromUpload = true;
                item.DraftFromUpload = true;
                item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
                item.RecordActionDate = DateTime.Now;
                item.RecordEditedBy = _userName;

                item.isFromUpload = true;
                item.DraftFromUpload = true;
                item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
                item.RecordActionDate = DateTime.Now;
                item.RecordEditedBy = _userName;

            }
        }
        #endregion

        #region appgen: commit uploaded excel fiel
        public async Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default)
        {
            var spec = new RoleManagementDetailFilterSpecification()
            {
                RecordEditedBy = _userName,
                DraftFromUpload = true,
                ShowDraftList = BaseEntity.DraftStatus.DraftMode
            }.BuildSpecification(true);
            var draftDatas = await ListAsync(spec, null, true, cancellationToken);
            int rowNum = 1;
            foreach (var item in draftDatas)
            {
                ValidateOnInsert(item);
                rowNum++;
            }

            if (!ServiceState)
                return false;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in draftDatas)
                {
                    var id = await CommitDraft(item.Id, cancellationToken);
                    if (id <= 0)
                    {
                        AddError("Terjadi kesalahan ketika menyimpan data.");
                        return false;
                    }
                }

                scope.Complete();
            }

            return true;
        }
        #endregion

        #region appgen: process uploaded file
        public async Task<bool> ProcessUploadedFile(IEnumerable<RoleManagementDetail> roleManagementDetails, CancellationToken cancellationToken = default)
        {
            if (roleManagementDetails == null)
                throw new ArgumentNullException(nameof(roleManagementDetails));

            cancellationToken.ThrowIfCancellationRequested();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in roleManagementDetails)
                {
                    var id = item.Id;
                    if (id <= 0)
                    {
                        var result = await this.AddAsync(item, cancellationToken);
                        if (result == null)
                            return false;
                    }
                    else
                    {
                        var result = await this.UpdateAsync(item, cancellationToken);
                        if (result == false)
                            return false;
                    }
                }

                scope.Complete();
            }

            return true;
        }
        #endregion

        #region appgen: generate upload log
        public async Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default)
        {
            var spec = new RoleManagementDetailFilterSpecification()
            {
                RecordEditedBy = _userName,
                DraftFromUpload = true,
                ShowDraftList = BaseEntity.DraftStatus.DraftMode
            }.BuildSpecification(true);
            var draftDatas = await ListAsync(spec, null, true, cancellationToken);

            var tempExcelFile = Path.GetTempFileName();
            InitEPPlusLicense();
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("RoleManagementDetail");
                ws.Cells[1, 1].Value = "ID";
                ws.Cells[1, 2].Value = "PK";
                ws.Cells[1, 3].Value = "RoleManagement";
                ws.Cells[1, 4].Value = "Function Info";
                ws.Cells[1, 5].Value = "Allow Create";
                ws.Cells[1, 6].Value = "Allow Read";
                ws.Cells[1, 7].Value = "Allow Update";
                ws.Cells[1, 8].Value = "Allow Delete";
                ws.Cells[1, 9].Value = "Show In Menu";
                ws.Cells[1, 10].Value = "Allow Download";
                ws.Cells[1, 11].Value = "Allow Print";
                ws.Cells[1, 12].Value = "Allow Upload";
                ws.Cells[1, 13].Value = "Status";
                ws.Cells[1, 14].Value = "Message";



                int row = 2;

                int pk = 1;
                foreach (var item in draftDatas)
                {
                    ws.Cells[row, 1].Value = item.Id;
                    ws.Cells[row, 2].Value = pk;
                    ws.Cells[row, 3].Value = item.AllowCreate;
                    ws.Cells[row, 4].Value = item.AllowRead;
                    ws.Cells[row, 5].Value = item.AllowUpdate;
                    ws.Cells[row, 6].Value = item.AllowDelete;
                    ws.Cells[row, 7].Value = item.ShowInMenu;
                    ws.Cells[row, 8].Value = item.AllowDownload;
                    ws.Cells[row, 9].Value = item.AllowPrint;
                    ws.Cells[row, 10].Value = item.AllowUpload;
                    ws.Cells[row, 11].Value = item.UploadValidationStatus;
                    ws.Cells[row, 12].Value = item.UploadValidationMessage;


                    row++;
                    pk++;
                }

                package.SaveAs(new FileInfo(tempExcelFile));
            }

            return tempExcelFile;
        }
        #endregion

        #endregion

        #region Recovery Record Service

        #region appgen: create draft
        public async Task<RoleManagementDetail> CreateDraft(RoleManagementDetail entity, CancellationToken cancellation)
        {
            entity.RoleManagement = null;
            entity.FunctionInfo = null;

            entity.IsDraftRecord = 1;
            entity.MainRecordId = null;
            entity.RecordEditedBy = _userName;
            entity.RecordActionDate = DateTime.Now;

            AssignCreatorAndCompany(entity);

            await _unitOfWork.RoleManagementDetailRepository.AddAsync(entity, cancellation);
            await _unitOfWork.CommitAsync(cancellation);
            return entity;
        }
        #endregion

        #region appgen: create edit draft
        public async Task<RoleManagementDetail> CreateEditDraft(int id, CancellationToken cancellation)
        {

            var count = await this.CountAsync(new RoleManagementDetailFilterSpecification(id), cancellation);
            if (count <= 0)
            {
                AddError($"Data Role Management Detail dengan id {id} tidak ditemukan.");
                return null;
            }

            // cek apakah object dengan mode draft sudah ada
            var spec = new RoleManagementDetailFilterSpecification()
            {
                MainRecordId = id,
                RecordEditedBy = _userName,
                ShowDraftList = BaseEntity.DraftStatus.DraftMode
            }.BuildSpecification();
            var previousDraft = await _unitOfWork.RoleManagementDetailRepository.FirstOrDefaultAsync(spec, cancellation);
            if (previousDraft != null)
                return previousDraft;

            // clone data
            var cloneResult = await _unitOfWork.RoleManagementDetailRepository.CloneEntity(id, _userName);
            if (cloneResult == null)
            {
                AddError($"Gagal membuat record Role Management Detail");
                return null;
            }

            return await _unitOfWork.RoleManagementDetailRepository.GetByIdAsync(cloneResult.Id, cancellation);

        }
        #endregion

        #region appgen: patch draft
        public async Task<bool> PatchDraft(RoleManagementDetail roleManagementDetail, CancellationToken cancellationToken)
        {
            var id = roleManagementDetail.Id;
            var originalValue = await _unitOfWork.RoleManagementDetailRepository.FirstOrDefaultAsync(new RoleManagementDetailFilterSpecification(id), cancellationToken);

            if (originalValue == null)
            {
                AddError($"Data dengan id {id} tidak ditemukan.");
                return false;
            }

            if (!string.IsNullOrEmpty(roleManagementDetail.FunctionInfoId)) originalValue.FunctionInfoId = roleManagementDetail.FunctionInfoId;
            if (roleManagementDetail.AllowCreate.HasValue) originalValue.AllowCreate = roleManagementDetail.AllowCreate.Value;
            if (roleManagementDetail.AllowRead.HasValue) originalValue.AllowRead = roleManagementDetail.AllowRead.Value;
            if (roleManagementDetail.AllowUpdate.HasValue) originalValue.AllowUpdate = roleManagementDetail.AllowUpdate.Value;
            if (roleManagementDetail.AllowDelete.HasValue) originalValue.AllowDelete = roleManagementDetail.AllowDelete.Value;
            if (roleManagementDetail.ShowInMenu.HasValue) originalValue.ShowInMenu = roleManagementDetail.ShowInMenu.Value;
            if (roleManagementDetail.AllowDownload.HasValue) originalValue.AllowDownload = roleManagementDetail.AllowDownload.Value;
            if (roleManagementDetail.AllowPrint.HasValue) originalValue.AllowPrint = roleManagementDetail.AllowPrint.Value;
            if (roleManagementDetail.AllowUpload.HasValue) originalValue.AllowUpload = roleManagementDetail.AllowUpload.Value;


            // pastikan data belongsTo & hasMany tidak ikut
            roleManagementDetail.RoleManagement = null;
            roleManagementDetail.FunctionInfo = null;


            AssignUpdater(originalValue);
            await _unitOfWork.RoleManagementDetailRepository.UpdateAsync(originalValue, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }
        #endregion

        #region appgen: commit draft
        public async Task<int> CommitDraft(int id, CancellationToken cancellationToken)
        {
            int resultId = 0;
            var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
            if (recoveryRecord == null) return 0;

            RoleManagementDetail destinationRecord = null;
            if (recoveryRecord.MainRecordId.HasValue)
            {
                destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
            }

            if (destinationRecord != null)
            {
                // recovery mode edit

                // header
                destinationRecord.RoleManagement = recoveryRecord.RoleManagement;
                destinationRecord.RoleManagement = null;
                destinationRecord.FunctionInfo = recoveryRecord.FunctionInfo;
                destinationRecord.FunctionInfo = null;
                destinationRecord.AllowCreate = recoveryRecord.AllowCreate;
                destinationRecord.AllowRead = recoveryRecord.AllowRead;
                destinationRecord.AllowUpdate = recoveryRecord.AllowUpdate;
                destinationRecord.AllowDelete = recoveryRecord.AllowDelete;
                destinationRecord.ShowInMenu = recoveryRecord.ShowInMenu;
                destinationRecord.AllowDownload = recoveryRecord.AllowDownload;
                destinationRecord.AllowPrint = recoveryRecord.AllowPrint;
                destinationRecord.AllowUpload = recoveryRecord.AllowUpload;


                await _unitOfWork.RoleManagementDetailRepository.UpdateAsync(destinationRecord, cancellationToken);
                resultId = destinationRecord.Id;
            }

            // header recovery
            int draftStatus = (int)BaseEntity.DraftStatus.MainRecord;
            if (destinationRecord != null)
                draftStatus = (int)BaseEntity.DraftStatus.Saved;

            recoveryRecord.IsDraftRecord = draftStatus;
            recoveryRecord.RecordActionDate = DateTime.Now;
            recoveryRecord.DraftFromUpload = false;



            // save ke database
            await _unitOfWork.RoleManagementDetailRepository.UpdateAsync(recoveryRecord, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return (destinationRecord == null) ? recoveryRecord.Id : resultId;
        }


        #endregion

        #region appgen: discard draft
        public async Task<bool> DiscardDraft(int id, CancellationToken cancellationToken)
        {
            var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
            if (recoveryRecord == null) return false;

            // header
            recoveryRecord.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
            recoveryRecord.RecordActionDate = DateTime.Now;


            // save ke database
            await _unitOfWork.RoleManagementDetailRepository.UpdateAsync(recoveryRecord, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return true;
        }
        #endregion

        #region appgen: get draft list
        public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
        {
            var spec = new RoleManagementDetailFilterSpecification()
            {
                ShowDraftList = BaseEntity.DraftStatus.DraftMode,
                RecordEditedBy = _userName
            }.BuildSpecification();

            List<DocumentDraft> documentDrafts = new();
            var datas = await _unitOfWork.RoleManagementDetailRepository.ListAsync(spec, null, cancellationToken);
            if (datas == null) return documentDrafts;

            foreach (var item in datas)
                documentDrafts.Add(
                    new DocumentDraft($"Ada dokumen yang di edit terakhir pada {item.RecordActionDate?.ToString("dd-MMM-yyyy")} Jam {item.RecordActionDate?.ToString("hh:mm")} dan belum di simpan.", item.Id));

            return documentDrafts;
        }
        #endregion

        #region appgen: get current editor
        public async Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken)
        {
            var spec = new RoleManagementDetailFilterSpecification()
            {
                ShowDraftList = BaseEntity.DraftStatus.DraftMode,
                MainRecordId = id
            }.BuildSpecification();

            var datas = await _unitOfWork.RoleManagementDetailRepository.ListAsync(spec, null, cancellationToken);
            if (datas == null) return null;

            List<string> users = new();
            foreach (var item in datas)
                users.Add(item.RecordEditedBy);

            var editorUsers = await _unitOfWork.UserInfoRepository.ListAsync(new UserInfoFilterSpecification(users), null, cancellationToken);
            users.Clear();
            if (editorUsers == null) return users;

            foreach (var item in editorUsers)
                users.Add(item.FullName);

            return users;
        }
        #endregion

        #region appgen: add draft async
        public async Task<List<RoleManagementDetail>> AddDraftAsync(List<RoleManagementDetail> roleManagementDetails, CancellationToken cancellationToken)
        {
            if (roleManagementDetails == null || roleManagementDetails.Count <= 0)
                throw new ArgumentNullException(nameof(roleManagementDetails));

            foreach (var item in roleManagementDetails)
            {
                item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
                item.DraftFromUpload = false;
                item.RecordEditedBy = _userName;
                item.RecordActionDate = DateTime.Now;
                await _unitOfWork.RoleManagementDetailRepository.AddAsync(item, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return roleManagementDetails;
        }
        #endregion

        #region appgen: replace draft async
        public async Task<List<RoleManagementDetail>> ReplaceDraftAsync(List<RoleManagementDetail> roleManagementDetails, CancellationToken cancellationToken)
        {
            if (roleManagementDetails == null || roleManagementDetails.Count <= 0)
                throw new ArgumentNullException(nameof(roleManagementDetails));

            // hapus seluruh data sesuai filter
            int parentId = roleManagementDetails[0].RoleManagementId ?? 0;
            var spec = new RoleManagementDetailFilterSpecification()
            {
                RoleManagementIds = new List<int>() { parentId }
            }.BuildSpecification();
            _unitOfWork.RoleManagementDetailRepository.DeleteAsync(spec, cancellationToken);

            // insert data baru
            foreach (var item in roleManagementDetails)
            {
                item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
                item.DraftFromUpload = false;
                item.RecordEditedBy = _userName;
                item.RecordActionDate = DateTime.Now;
                await _unitOfWork.RoleManagementDetailRepository.AddAsync(item, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return roleManagementDetails;
        }
        #endregion

        #endregion

    }
}
