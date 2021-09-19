using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
    public class RoleManagementRepository : AsyncRepository<RoleManagement>, IRoleManagementRepository
    {
        #region appgen: private variable
        public RoleManagementRepository(AppDbContext context) : base(context) { }
        #endregion

        #region appgen: constructor
        private AppDbContext MyDbContext
        {
            get { return Context as AppDbContext; }
        }
        #endregion

        #region appgen: generated methods
        public async Task<RoleManagement> CloneEntity(int id, string userName)
        {
            var entity = await MyDbContext.Set<RoleManagement>().AsQueryable()
                .Where(e => e.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
            entity.Id = 0;
            entity.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
            entity.MainRecordId = id;
            entity.RecordActionDate = DateTime.Now;
            entity.RecordEditedBy = userName;

            var roleManagementDetails = await MyDbContext.Set<RoleManagementDetail>().AsQueryable()
                .Where(e => e.RoleManagement.Id == id)
                .AsNoTracking()
                .ToListAsync();
            entity.ClearRoleManagementDetails();
            entity.AddRangeRoleManagementDetails(roleManagementDetails
                .Select(e =>
                {
                    e.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
                    e.MainRecordId = e.Id;
                    e.RecordActionDate = DateTime.Now;
                    e.RecordEditedBy = userName;
                    e.Id = 0;
                    return e;
                })
                .ToList());


            await MyDbContext.AddAsync(entity);
            await MyDbContext.SaveChangesAsync();
            return entity;
        }
        #endregion
    }
}
