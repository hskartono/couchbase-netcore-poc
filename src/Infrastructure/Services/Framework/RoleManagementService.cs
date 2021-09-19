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
    public class RoleManagementService : AsyncBaseService<RoleManagement>, IRoleManagementService
    {

        #region appgen: private variable

        private readonly IDownloadProcessService _downloadProcessService;
        private readonly IUriComposer _uriComposer;

        #endregion

        #region appgen: constructor

        public RoleManagementService(
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
        public async Task<RoleManagement> AddAsync(RoleManagement entity, CancellationToken cancellationToken = default)
        {
            if (!ValidateOnInsert(entity))
                return null;

            AssignCreatorAndCompany(entity);

            if (entity.RoleManagementDetail?.Count > 0)
                foreach (var item in entity.RoleManagementDetail)
                {
                    AssignCreatorAndCompany(item);
                    item.RoleManagement = entity;
                }


            await _unitOfWork.RoleManagementRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return entity;
        }
        #endregion

        #region appgen: count
        public async Task<int> CountAsync(ISpecification<RoleManagement> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementRepository.CountAsync(spec, cancellationToken);
        }
        #endregion

        #region appgen: delete
        public async Task<bool> DeleteAsync(RoleManagement entity, CancellationToken cancellationToken = default)
        {
            _unitOfWork.RoleManagementRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }
        #endregion

        #region appgen: first record
        public async Task<RoleManagement> FirstAsync(ISpecification<RoleManagement> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementRepository.FirstAsync(spec, cancellationToken);
        }
        #endregion

        #region appgen: first or default
        public async Task<RoleManagement> FirstOrDefaultAsync(ISpecification<RoleManagement> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementRepository.FirstOrDefaultAsync(spec, cancellationToken);
        }
        #endregion

        #region appgen: get by id
        public async Task<RoleManagement> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id, false, cancellationToken);
        }

        private async Task<RoleManagement> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
        {
            var specFilter = new RoleManagementFilterSpecification(id, true);
            var roleManagement = await _unitOfWork.RoleManagementRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
            if (roleManagement == null || includeChilds == false)
                return roleManagement;

            var roleManagementDetailFilter = new RoleManagementDetailFilterSpecification()
            {
                RoleManagementIds = new List<int>() { id },
                //ShowDraftList = BaseEntity.DraftStatus.All
            }.BuildSpecification();
            var roleManagementDetails = await _unitOfWork.RoleManagementDetailRepository.ListAsync(roleManagementDetailFilter, null, cancellationToken);
            roleManagement.RoleManagementDetail = roleManagementDetails.ToList();


            return roleManagement;
        }
        #endregion

        #region appgen: list all
        public async Task<IReadOnlyList<RoleManagement>> ListAllAsync(List<SortingInformation<RoleManagement>> sorting, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleManagementRepository.ListAllAsync(sorting, cancellationToken);
        }
        #endregion

        #region appgen: get list
        public async Task<IReadOnlyList<RoleManagement>> ListAsync(
            ISpecification<RoleManagement> spec,
            List<SortingInformation<RoleManagement>> sorting,
            bool withChilds = false,
            CancellationToken cancellationToken = default)
        {
            var roleManagements = await _unitOfWork.RoleManagementRepository.ListAsync(spec, sorting, cancellationToken);
            if (withChilds && roleManagements?.Count > 0)
            {
                var results = new List<RoleManagement>(roleManagements);
                var roleManagementIds = roleManagements.Select(e => e.Id).ToList();

                var roleManagementDetailFilter = new RoleManagementDetailFilterSpecification()
                {
                    RoleManagementIds = roleManagementIds,
                    ShowDraftList = BaseEntity.DraftStatus.All
                }.BuildSpecification();
                var roleManagementDetails = await _unitOfWork.RoleManagementDetailRepository.ListAsync(roleManagementDetailFilter, null, cancellationToken);
                results.ForEach(c => c.AddRangeRoleManagementDetails(
                    roleManagementDetails
                    .Where(e => e.RoleManagementId == c.Id).ToList()
                    ));


                return results;
            }

            return roleManagements;
        }
        #endregion

        #region appgen: update
        public async Task<bool> UpdateAsync(RoleManagement entity, CancellationToken cancellationToken = default)
        {
            if (!ValidateOnUpdate(entity))
                return false;

            cancellationToken.ThrowIfCancellationRequested();

            // update header
            AssignUpdater(entity);
            await _unitOfWork.RoleManagementRepository.ReplaceAsync(entity, entity.Id, cancellationToken);

            var oldEntity = await GetByIdAsync(entity.Id, true, cancellationToken);
            if (oldEntity == null)
            {
                AddError($"Could not load {nameof(entity)} data with id {entity.Id}.");
                return false;
            }
            await SmartUpdateRoleManagementDetail(oldEntity, entity, cancellationToken);


            // update & commit
            //await _unitOfWork.RoleManagementRepository.UpdateAsync(oldEntity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        private async Task SmartUpdateRoleManagementDetail(RoleManagement oldEntity, RoleManagement entity, CancellationToken cancellationToken = default)
        {
            List<RoleManagementDetail> oldEntityToBeDeleted = new();
            if (oldEntity.RoleManagementDetail.Count > 0)
            {
                foreach (var item in oldEntity.RoleManagementDetail)
                    oldEntityToBeDeleted.Add(item);
            }

            if (entity.RoleManagementDetail.Count > 0)
            {
                foreach (var item in entity.RoleManagementDetail)
                {
                    var hasUpdate = false;
                    item.RoleManagementId = entity.Id;
                    item.RoleManagement = entity;

                    if (item.Id > 0)
                    {
                        if (oldEntity.RoleManagementDetail.Count > 0)
                        {
                            var data = oldEntity.RoleManagementDetail.SingleOrDefault(p => p.Id == item.Id);
                            if (data != null)
                            {
                                AssignUpdater(item);
                                await _unitOfWork.RoleManagementDetailRepository.ReplaceAsync(item, item.Id, cancellationToken);

                                hasUpdate = true;
                                oldEntityToBeDeleted.Remove(data);
                            }
                        }
                    }

                    if (!hasUpdate)
                    {
                        AssignCreatorAndCompany(item);
                        //await _unitOfWork.RoleManagementDetailRepository.AddAsync(item, cancellationToken);
                        oldEntity.AddOrReplaceRoleManagementDetail(item);
                    }
                }
            }

            if (oldEntityToBeDeleted.Count > 0)
            {
                foreach (var item in oldEntityToBeDeleted)
                    oldEntity.RemoveRoleManagementDetail(item);
            }
        }
        #endregion

        #endregion

        #region Validation Operations

        #region appgen: validatebase
        private bool ValidateBase(RoleManagement entity)
        {
            if (entity == null)
                AddError("Tidak dapat menyimpan data kosong.");



            return ServiceState;
        }
        #endregion

        #region appgen: validateoninsert
        private bool ValidateOnInsert(RoleManagement entity)
        {
            ValidateBase(entity);

            return ServiceState;
        }
        #endregion

        #region appgen: validateonupdate
        private bool ValidateOnUpdate(RoleManagement entity)
        {
            ValidateBase(entity);

            return ServiceState;
        }
        #endregion

        #endregion

        #region PDF Related

        #region appgen: generate pdf single
        public string GeneratePdf(RoleManagement entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cancellationToken.ThrowIfCancellationRequested();

            // read template
            string templateFile = _uriComposer.ComposeTemplatePath("role_management.html");
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

            var items = await this.ListAsync(new RoleManagementFilterSpecification(ids), null, true, cancellationToken);
            if (items == null || items.Count <= 0)
            {
                AddError($"Could not get data for list of id {ids.ToArray()}");
                return null;
            }

            string templateFile = _uriComposer.ComposeTemplatePath("role_management.html");
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
            var downloadProcess = new DownloadProcess("role_management") { StartTime = DateTime.Now };
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
        private string MapTemplateValue(string htmlContent, RoleManagement entity)
        {
            Dictionary<string, object> mapper = new()
                {
                    {"Id",""},
                    {"Name",""},
                    {"Description",""},
                    {"RoleManagementDetail", new List<Dictionary<string, string>>()},

                };

            if (entity != null)
            {
                mapper["Id"] = entity.Id.ToString();
                mapper["Name"] = entity.Name;
                mapper["Description"] = entity.Description;
                if (entity.RoleManagementDetail.Count > 0)
                {
                    foreach (var item in entity.RoleManagementDetail)
                    {
                        var roleManagementDetail = new Dictionary<string, string>()
                        {
                            {"RoleManagementDetailId", item.Id.ToString()},
                            {"RoleManagementDetailAllowCreate", (item.AllowCreate.HasValue && item.AllowCreate.Value) ? "Yes" : "No"},
                            {"RoleManagementDetailAllowRead", (item.AllowRead.HasValue && item.AllowRead.Value) ? "Yes" : "No"},
                            {"RoleManagementDetailAllowUpdate", (item.AllowUpdate.HasValue && item.AllowUpdate.Value) ? "Yes" : "No"},
                            {"RoleManagementDetailAllowDelete", (item.AllowDelete.HasValue && item.AllowDelete.Value) ? "Yes" : "No"},
                            {"RoleManagementDetailShowInMenu", (item.ShowInMenu.HasValue && item.ShowInMenu.Value) ? "Yes" : "No"},
                            {"RoleManagementDetailAllowDownload", (item.AllowDownload.HasValue && item.AllowDownload.Value) ? "Yes" : "No"},
                            {"RoleManagementDetailAllowPrint", (item.AllowPrint.HasValue && item.AllowPrint.Value) ? "Yes" : "No"},
                            {"RoleManagementDetailAllowUpload", (item.AllowUpload.HasValue && item.AllowUpload.Value) ? "Yes" : "No"},
                        };
                        ((List<Dictionary<string, string>>)mapper["RoleManagementDetail"]).Add(roleManagementDetail);
                    }
                }


            }

            return BuildHtmlTemplate(htmlContent, mapper);
        }
        #endregion

        #endregion

        #region Excel Related

        #region appgen: generate excel background process
        public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
            int? id = null, List<string> names = null, List<string> descriptions = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // insert dulu ke database
            var downloadProcess = new DownloadProcess("role_management") { StartTime = DateTime.Now };
            var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
            if (result == null)
            {
                AddError("Failed to insert download process");
                return null;
            }

            // lempar ke background process
            var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
                id, names, descriptions,
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
            int? id = null, List<string> names = null, List<string> descriptions = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                RoleManagementFilterSpecification filterSpec = null;
                if (id.HasValue)
                    filterSpec = new RoleManagementFilterSpecification(id.Value);
                else
                    filterSpec = new RoleManagementFilterSpecification(exact)
                    {

                        Id = id,
                        Names = names,
                        Descriptions = descriptions
                    }.BuildSpecification();

                var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (ExcelMapper.WriteToExcel<RoleManagement>(excelFilename, "roleManagement.json", results) == false)
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
        public async Task<List<RoleManagement>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
        {
            var result = ExcelMapper.ReadFromExcel<RoleManagement>(tempExcelFile, "roleManagement.json");
            if (result == null)
            {
                AddError("Format template excel tidak dikenali. Silahkan download template dari menu download.");
                return null;
            }

            SetUploadDraftFlags(result);

            foreach (var item in result)
            {
                var id = item.Id;
                if (id > 0)
                    await _unitOfWork.RoleManagementRepository.UpdateAsync(item, cancellationToken);
                else
                    await _unitOfWork.RoleManagementRepository.AddAsync(item, cancellationToken);

            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return result;
        }

        private void SetUploadDraftFlags(List<RoleManagement> result)
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
                for (int i = 0; i < item.RoleManagementDetail.Count; i++)
                {
                    item.isFromUpload = true;
                    item.DraftFromUpload = true;
                    item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
                    item.RecordActionDate = DateTime.Now;
                    item.RecordEditedBy = _userName;
                }

            }
        }
        #endregion

        #region appgen: commit uploaded excel fiel
        public async Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default)
        {
            var spec = new RoleManagementFilterSpecification()
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
        public async Task<bool> ProcessUploadedFile(IEnumerable<RoleManagement> roleManagements, CancellationToken cancellationToken = default)
        {
            if (roleManagements == null)
                throw new ArgumentNullException(nameof(roleManagements));

            cancellationToken.ThrowIfCancellationRequested();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in roleManagements)
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
            var spec = new RoleManagementFilterSpecification()
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
                var ws = package.Workbook.Worksheets.Add("RoleManagement");
                ws.Cells[1, 1].Value = "ID";
                ws.Cells[1, 2].Value = "PK";
                ws.Cells[1, 3].Value = "Name";
                ws.Cells[1, 4].Value = "Description";
                ws.Cells[1, 5].Value = "Role Management Details";
                ws.Cells[1, 6].Value = "Status";
                ws.Cells[1, 7].Value = "Message";

                var wsRoleManagementDetail = package.Workbook.Worksheets.Add("RoleManagementDetail");
                wsRoleManagementDetail.Cells[1, 1].Value = "ID";
                wsRoleManagementDetail.Cells[1, 2].Value = "PK";
                wsRoleManagementDetail.Cells[1, 3].Value = "RoleManagement";
                wsRoleManagementDetail.Cells[1, 4].Value = "Function Info";
                wsRoleManagementDetail.Cells[1, 5].Value = "Allow Create";
                wsRoleManagementDetail.Cells[1, 6].Value = "Allow Read";
                wsRoleManagementDetail.Cells[1, 7].Value = "Allow Update";
                wsRoleManagementDetail.Cells[1, 8].Value = "Allow Delete";
                wsRoleManagementDetail.Cells[1, 9].Value = "Show In Menu";
                wsRoleManagementDetail.Cells[1, 10].Value = "Allow Download";
                wsRoleManagementDetail.Cells[1, 11].Value = "Allow Print";
                wsRoleManagementDetail.Cells[1, 12].Value = "Allow Upload";
                wsRoleManagementDetail.Cells[1, 13].Value = "Status";
                wsRoleManagementDetail.Cells[1, 14].Value = "Message";


                int row = 2;
                int rowRoleManagementDetail = 2;

                int pk = 1;
                foreach (var item in draftDatas)
                {
                    ws.Cells[row, 1].Value = item.Id;
                    ws.Cells[row, 2].Value = pk;
                    ws.Cells[row, 3].Value = item.Name;
                    ws.Cells[row, 4].Value = item.Description;
                    ws.Cells[row, 5].Value = item.UploadValidationStatus;
                    ws.Cells[row, 6].Value = item.UploadValidationMessage;
                    foreach (var itemRoleManagementDetail in item.RoleManagementDetail)
                    {
                        wsRoleManagementDetail.Cells[rowRoleManagementDetail, 1].Value = itemRoleManagementDetail.Id;
                        wsRoleManagementDetail.Cells[rowRoleManagementDetail, 2].Value = pk;
                        ws.Cells[row, 3].Value = item.UploadValidationStatus;
                        ws.Cells[row, 4].Value = item.UploadValidationMessage;
                    }


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
        public async Task<RoleManagement> CreateDraft(CancellationToken cancellation)
        {
            var spec = new RoleManagementFilterSpecification()
            {
                ShowDraftList = BaseEntity.DraftStatus.DraftMode,
                MainRecordIdIsNull = true,
                RecordEditedBy = _userName
            }.BuildSpecification();
            var count = await _unitOfWork.RoleManagementRepository.CountAsync(spec, cancellation);
            if (count > 0)
                return await _unitOfWork.RoleManagementRepository.FirstOrDefaultAsync(spec, cancellation);

            var entity = new RoleManagement
            {
                IsDraftRecord = 1,
                MainRecordId = null,
                RecordEditedBy = _userName,
                RecordActionDate = DateTime.Now
            };

            AssignCreatorAndCompany(entity);

            await _unitOfWork.RoleManagementRepository.AddAsync(entity, cancellation);
            await _unitOfWork.CommitAsync(cancellation);
            return entity;
        }
        #endregion

        #region appgen: create edit draft
        public async Task<RoleManagement> CreateEditDraft(int id, CancellationToken cancellation)
        {

            var count = await this.CountAsync(new RoleManagementFilterSpecification(id), cancellation);
            if (count <= 0)
            {
                AddError($"Data Role Management dengan id {id} tidak ditemukan.");
                return null;
            }

            // cek apakah object dengan mode draft sudah ada
            var spec = new RoleManagementFilterSpecification()
            {
                MainRecordId = id,
                RecordEditedBy = _userName,
                ShowDraftList = BaseEntity.DraftStatus.DraftMode
            }.BuildSpecification();
            var previousDraft = await _unitOfWork.RoleManagementRepository.FirstOrDefaultAsync(spec, cancellation);
            if (previousDraft != null)
                return previousDraft;

            // clone data
            var cloneResult = await _unitOfWork.RoleManagementRepository.CloneEntity(id, _userName);
            if (cloneResult == null)
            {
                AddError($"Gagal membuat record Role Management");
                return null;
            }

            return await _unitOfWork.RoleManagementRepository.GetByIdAsync(cloneResult.Id, cancellation);

        }
        #endregion

        #region appgen: patch draft
        public async Task<bool> PatchDraft(RoleManagement roleManagement, CancellationToken cancellationToken)
        {
            var id = roleManagement.Id;
            var originalValue = await _unitOfWork.RoleManagementRepository.FirstOrDefaultAsync(new RoleManagementFilterSpecification(id), cancellationToken);

            if (originalValue == null)
            {
                AddError($"Data dengan id {id} tidak ditemukan.");
                return false;
            }

            if (!string.IsNullOrEmpty(roleManagement.Name)) originalValue.Name = roleManagement.Name;
            if (!string.IsNullOrEmpty(roleManagement.Description)) originalValue.Description = roleManagement.Description;


            // pastikan data belongsTo & hasMany tidak ikut
            roleManagement.RoleManagementDetail = null;


            AssignUpdater(originalValue);
            await _unitOfWork.RoleManagementRepository.UpdateAsync(originalValue, cancellationToken);
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

            RoleManagement destinationRecord = null;
            if (recoveryRecord.MainRecordId.HasValue)
            {
                destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
            }

            if (destinationRecord != null)
            {
                // recovery mode edit

                // header
                destinationRecord.Name = recoveryRecord.Name;
                destinationRecord.Description = recoveryRecord.Description;
                this.SmartUpdateRecoveryRoleManagementDetail(destinationRecord, recoveryRecord, cancellationToken);


                await _unitOfWork.RoleManagementRepository.UpdateAsync(destinationRecord, cancellationToken);
                resultId = destinationRecord.Id;
            }

            // header recovery
            int draftStatus = (int)BaseEntity.DraftStatus.MainRecord;
            if (destinationRecord != null)
                draftStatus = (int)BaseEntity.DraftStatus.Saved;

            recoveryRecord.IsDraftRecord = draftStatus;
            recoveryRecord.RecordActionDate = DateTime.Now;
            recoveryRecord.DraftFromUpload = false;

            foreach (var item in recoveryRecord.RoleManagementDetail)
            {
                item.IsDraftRecord = draftStatus;
                item.RecordActionDate = DateTime.Now;
                item.DraftFromUpload = false;
            }


            // save ke database
            await _unitOfWork.RoleManagementRepository.UpdateAsync(recoveryRecord, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return (destinationRecord == null) ? recoveryRecord.Id : resultId;
        }

        private void SmartUpdateRecoveryRoleManagementDetail(RoleManagement destinationRecord, RoleManagement recoveryRecord, CancellationToken cancellationToken)
        {
            List<RoleManagementDetail> destinationToBeDeleted = new();
            if (destinationRecord.RoleManagementDetail.Count > 0)
            {
                foreach (var item in destinationRecord.RoleManagementDetail)
                    destinationToBeDeleted.Add(item);
            }

            if (recoveryRecord.RoleManagementDetail.Count > 0)
            {
                foreach (var item in recoveryRecord.RoleManagementDetail)
                {
                    var hasUpdate = false;
                    if (destinationRecord.RoleManagementDetail.Count > 0)
                    {
                        var data = destinationRecord.RoleManagementDetail.SingleOrDefault(p => p.Id == item.MainRecordId);
                        if (data != null)
                        {
                            data.FunctionInfoId = item.FunctionInfoId;
                            data.AllowCreate = item.AllowCreate;
                            data.AllowRead = item.AllowRead;
                            data.AllowUpdate = item.AllowUpdate;
                            data.AllowDelete = item.AllowDelete;
                            data.ShowInMenu = item.ShowInMenu;
                            data.AllowDownload = item.AllowDownload;
                            data.AllowPrint = item.AllowPrint;
                            data.AllowUpload = item.AllowUpload;

                            AssignUpdater(data);

                            item.IsDraftRecord = (int)BaseEntity.DraftStatus.Saved;
                            item.RecordActionDate = DateTime.Now;

                            hasUpdate = true;
                            destinationToBeDeleted.Remove(data);
                        }
                    }

                    if (!hasUpdate)
                    {
                        //AssignCreatorAndCompany(item);
                        destinationRecord.AddRoleManagementDetail(item.FunctionInfoId, item.AllowCreate, item.AllowRead, item.AllowUpdate, item.AllowDelete, item.ShowInMenu, item.AllowDownload, item.AllowPrint, item.AllowUpload);

                        item.IsDraftRecord = (int)BaseEntity.DraftStatus.Saved;
                        item.RecordActionDate = DateTime.Now;
                    }
                }
            }

            if (destinationToBeDeleted.Count > 0)
            {
                foreach (var item in destinationToBeDeleted)
                    destinationRecord.RemoveRoleManagementDetail(item);
            }
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
            foreach (var item in recoveryRecord.RoleManagementDetail)
            {
                item.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
                item.RecordActionDate = DateTime.Now;
            }


            // save ke database
            await _unitOfWork.RoleManagementRepository.UpdateAsync(recoveryRecord, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return true;
        }
        #endregion

        #region appgen: get draft list
        public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
        {
            var spec = new RoleManagementFilterSpecification()
            {
                ShowDraftList = BaseEntity.DraftStatus.DraftMode,
                RecordEditedBy = _userName
            }.BuildSpecification();

            List<DocumentDraft> documentDrafts = new();
            var datas = await _unitOfWork.RoleManagementRepository.ListAsync(spec, null, cancellationToken);
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
            var spec = new RoleManagementFilterSpecification()
            {
                ShowDraftList = BaseEntity.DraftStatus.DraftMode,
                MainRecordId = id
            }.BuildSpecification();

            var datas = await _unitOfWork.RoleManagementRepository.ListAsync(spec, null, cancellationToken);
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



        #endregion

    }
}
