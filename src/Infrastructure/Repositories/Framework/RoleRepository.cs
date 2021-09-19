using Microsoft.EntityFrameworkCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
    public class RoleRepository : AsyncRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context) { }

        #region appgen: constructor
        private AppDbContext MyDbContext
        {
            get { return Context as AppDbContext; }
        }
        #endregion

        public async Task<Role> GetUserGroup(string userName, CancellationToken cancellationToken = default)
        {
            string sql = $@"select R.* from UserRoles as UR 
							inner join UserRoleDetails as URD on UR.Id = URD.UserRoleId
							inner join Roles as R on R.Id = URD.RoleId
							where UR.UserInfoId = '{userName}'";
            List<Role> list;
            try
            {
                list = await MyDbContext.Set<Role>().FromSqlRaw(sql).ToListAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
    }
}
