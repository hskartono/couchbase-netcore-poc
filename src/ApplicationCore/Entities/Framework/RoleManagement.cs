using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
    public class RoleManagement : BaseEntity
    {
        #region appgen: generated constructor
        public RoleManagement() { }

        public RoleManagement(string name, string description)
        {
            Name = name;
            Description = description;
        }


        #endregion

        #region appgen: generated property
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        private IList<RoleManagementDetail> _roleManagementDetail = new List<RoleManagementDetail>();
        public IList<RoleManagementDetail> RoleManagementDetail { get => _roleManagementDetail; set => _roleManagementDetail = value; }

        public void AddOrReplaceRoleManagementDetail(RoleManagementDetail entity)
        {
            try
            {
                RoleManagementDetail selectedItem = null;
                int index = 0;

                foreach (var item in _roleManagementDetail)
                {
                    if (entity.Id > 0)
                    {
                        if (item.Id == entity.Id)
                        {
                            selectedItem = item;
                            break;
                        }
                    }
                    index++;
                }


                if (selectedItem == null)
                {
                    entity.RoleManagement = this;
                    entity.RoleManagementId = this.Id;
                    var newItem = new RoleManagementDetail(entity.FunctionInfoId, entity.AllowCreate, entity.AllowRead, entity.AllowUpdate, entity.AllowDelete, entity.ShowInMenu, entity.AllowDownload, entity.AllowPrint, entity.AllowUpload, this);
                    _roleManagementDetail.Add(newItem);

                }
                else
                {
                    entity.Id = selectedItem.Id;
                    entity.RoleManagement = this;
                    entity.RoleManagementId = this.Id;

                    entity.CompanyId = selectedItem.CompanyId;
                    entity.CreatedBy = selectedItem.CreatedBy;
                    entity.CreatedDate = selectedItem.CreatedDate;
                    entity.UpdatedBy = selectedItem.UpdatedBy;
                    entity.UpdatedDate = selectedItem.UpdatedDate;

                    selectedItem = entity;
                    _roleManagementDetail[index] = selectedItem;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddRoleManagementDetail(string functionInfoId, bool? allowCreate, bool? allowRead, bool? allowUpdate, bool? allowDelete, bool? showInMenu, bool? allowDownload, bool? allowPrint, bool? allowUpload)
        {
            var newItem = new RoleManagementDetail(functionInfoId, allowCreate, allowRead, allowUpdate, allowDelete, showInMenu, allowDownload, allowPrint, allowUpload, this);
            _roleManagementDetail.Add(newItem);
        }

        public void RemoveRoleManagementDetail(RoleManagementDetail entity)
        {
            var selectedItem = _roleManagementDetail.FirstOrDefault(e => e.Id == entity.Id);
            _roleManagementDetail.Remove(selectedItem);
        }

        public void ClearRoleManagementDetails()
        {
            _roleManagementDetail.Clear();
        }

        public void AddRangeRoleManagementDetails(IList<RoleManagementDetail> roleManagementDetails)
        {
            this.ClearRoleManagementDetails();
            ((List<RoleManagementDetail>)_roleManagementDetail).AddRange(roleManagementDetails);
        }

        public new int? MainRecordId { get; set; }
        #endregion

        #region appgen: generated method

        #endregion
    }
}
