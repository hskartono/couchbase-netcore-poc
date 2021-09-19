using Ardalis.Specification;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace AppCoreApi.Infrastructure.Services
{
    public class RoleService : AsyncBaseService<Role>, IRoleService
    {
        public RoleService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.RoleRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return entity;
        }

        public async Task<int> CountAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleRepository.CountAsync(spec, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Role entity, CancellationToken cancellationToken = default)
        {
            _unitOfWork.RoleRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public async Task<Role> FirstAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleRepository.FirstAsync(spec, cancellationToken);
        }

        public async Task<Role> FirstOrDefaultAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleRepository.FirstOrDefaultAsync(spec, cancellationToken);
        }

        public async Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var specFilter = new RoleFilterSpecification(id);
            var results = await _unitOfWork.RoleRepository.ListAsync(specFilter, null, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                AddError("Cancellation requested");
                return null;
            }
            if (results?.Count > 0) return results[0];
            return null;
        }

        public async Task<IReadOnlyList<Role>> ListAllAsync(List<SortingInformation<Role>> sorting, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleRepository.ListAllAsync(sorting, cancellationToken);
        }

        public async Task<IReadOnlyList<Role>> ListAsync(ISpecification<Role> spec, List<SortingInformation<Role>> sorting, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleRepository.ListAsync(spec, sorting, cancellationToken);
        }

        public async Task<bool> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            if (!ValidateBase(entity))
                return false;

            // update header
            AssignUpdater(entity);
            await _unitOfWork.RoleRepository.ReplaceAsync(entity, entity.Id, cancellationToken);

            // ambil old data
            var specFilter = new RoleFilterSpecification(entity.Id, true);
            var oldEntities = await _unitOfWork.RoleRepository.ListAsync(specFilter, null, cancellationToken);
#pragma warning disable CA1826 // Do not use Enumerable methods on indexable collections
            var oldEntity = oldEntities.FirstOrDefault();
#pragma warning restore CA1826 // Do not use Enumerable methods on indexable collections
            List<RoleDetail> oldEntityToBeDeleted = new();
            if (oldEntity.RoleDetails.Count > 0)
            {
                foreach (var item in oldEntity.RoleDetails)
                    oldEntityToBeDeleted.Add(item);
            }

            // smart update
            if (entity.RoleDetails.Count > 0)
            {
                foreach (var item in entity.RoleDetails)
                {
                    var hasUpdate = false;
                    if (oldEntity.RoleDetails.Count > 0)
                    {
                        var data = oldEntity.RoleDetails.SingleOrDefault(p => p.Id == item.Id);
                        if (data != null)
                        {
                            AssignUpdater(item);
                            await _unitOfWork.RoleDetailRepository.ReplaceAsync(item, item.Id, cancellationToken);

                            oldEntityToBeDeleted.Remove(data);

                            hasUpdate = true;
                        }
                    }

                    if (!hasUpdate)
                    {
                        oldEntity.AddOrUpdateRoleDetail(item);
                    }
                }
            }

            if (oldEntityToBeDeleted.Count > 0)
            {
                foreach (var item in oldEntityToBeDeleted)
                    oldEntity.RemoveRoleDetail(item);
            }

            // update & commit
            await _unitOfWork.RoleRepository.UpdateAsync(oldEntity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        private bool ValidateBase(Role role)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return ServiceState;
        }

        public bool ValidateOnInsert(Role role)
        {
            ValidateBase(role);

            return ServiceState;
        }

        public bool ValidateOnUpdate(Role role)
        {
            ValidateBase(role);

            return ServiceState;
        }

        public async Task<Role> GetUserRole(string userName, CancellationToken cancellationToken = default)
        {
            var userRole = await _unitOfWork.UserRoleRepository.FirstOrDefaultAsync(new UserRoleFilterSpecification(userName, null, true), cancellationToken);
            var roleIds = userRole.UserRoleDetails.Select(e => e.RoleId).ToList();

            var roles = await _unitOfWork.RoleRepository.ListAsync(new RoleFilterSpecification(roleIds), null, cancellationToken);

            var MyRole = new Role()
            {
                Description = "MyRole",
                Name = "MyRole",
                Id = 1
            };

            int counter = 1;
            List<string> functionIds = new();
            foreach (var role in roles)
            {
                foreach (var item in role.RoleDetails)
                {
                    if (functionIds.Contains(item.FunctionInfoId)) continue;

                    var itemRole = new RoleDetail(MyRole, item.FunctionInfoId,
                        item.AllowCreate, item.AllowRead, item.AllowUpdate, item.AllowDelete,
                        item.AllowDownload, item.AllowPrint, item.ShowInMenu, item.AllowUpload)
                    {
                        Id = item.Id,
                        FunctionInfo = item.FunctionInfo
                    };
                    if (item.FunctionInfo != null && item.FunctionInfo.ModuleInfo != null && item.FunctionInfo.ModuleInfo.OrderPosition != null)
                    {
                        itemRole.ModuleOrder = item.FunctionInfo.ModuleInfo.OrderPosition.Value;
                    }

                    itemRole.FunctionName = item.FunctionInfo?.Name;

                    MyRole.AddOrUpdateRoleDetail(itemRole);

                    counter++;
                }
            }

            MyRole.SortRoleDetails();

            return MyRole;
        }

        public async Task<Dictionary<String, Role>> GetUserRoles(string userName, CancellationToken cancellationToken = default)
        {
            var userRole = await _unitOfWork.UserRoleRepository.FirstOrDefaultAsync(new UserRoleFilterSpecification(userName, null, true), cancellationToken);
            var roleIds = userRole.UserRoleDetails.Select(e => e.RoleId).ToList();

            var result = await _unitOfWork.RoleRepository.ListAsync(new RoleFilterSpecification(roleIds), null, cancellationToken);

            Dictionary<string, Role> roles = new();

            foreach (var role in result)
            {
                if (String.IsNullOrEmpty(role.Name)) continue;
                if (!roles.ContainsKey(role.Name.ToLower()))
                {
                    roles.Add(role.Name.ToLower(), role);
                }
            }

            return roles;
        }

        public async Task<Role> GetUserGroup(string userName, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RoleRepository.GetUserGroup(userName, cancellationToken);
        }
    }
}
