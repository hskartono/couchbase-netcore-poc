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
	public class UserRoleService : AsyncBaseService<UserRole>, IUserRoleService
	{
		public UserRoleService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<UserRole> AddAsync(UserRole entity, CancellationToken cancellationToken = default)
		{
			AssignCreatorAndCompany(entity);
			await _unitOfWork.UserRoleRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<UserRole> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserRoleRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(UserRole entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.UserRoleRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<UserRole> FirstAsync(ISpecification<UserRole> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserRoleRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<UserRole> FirstOrDefaultAsync(ISpecification<UserRole> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserRoleRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<UserRole> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			var specFilter = new UserRoleFilterSpecification(id);
			var results = await _unitOfWork.UserRoleRepository.ListAsync(specFilter, null, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
				throw new Exception("Cancellation token requested");

			if (results?.Count > 0) return results[0];
			return null;
		}

		public async Task<IReadOnlyList<UserRole>> ListAllAsync(List<SortingInformation<UserRole>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserRoleRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<UserRole>> ListAsync(ISpecification<UserRole> spec, List<SortingInformation<UserRole>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserRoleRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(UserRole entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateBase(entity))
				return false;

			// update header
			AssignUpdater(entity);
			await _unitOfWork.UserRoleRepository.ReplaceAsync(entity, entity.Id, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
				throw new Exception("Cancellation token requested");

			// ambil old data
			var specFilter = new UserRoleFilterSpecification(entity.Id);
			var oldEntities = await _unitOfWork.UserRoleRepository.ListAsync(specFilter, null, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
				throw new Exception("Cancellation token requested");

			var oldEntity = oldEntities.FirstOrDefault();
			List<UserRoleDetail> oldEntityToBeDeleted = new List<UserRoleDetail>();
			if (oldEntity.UserRoleDetails.Count > 0)
			{
				foreach (var item in oldEntity.UserRoleDetails)
					oldEntityToBeDeleted.Add(item);
			}

			// smart update
			if (entity.UserRoleDetails.Count > 0)
			{
				foreach (var item in entity.UserRoleDetails)
				{
					var hasUpdate = false;
					if (oldEntity.UserRoleDetails.Count > 0)
					{
						var data = oldEntity.UserRoleDetails.SingleOrDefault(p => p.Id == item.Id);
						if (data != null)
						{
							AssignUpdater(item);
							await _unitOfWork.UserRoleDetailRepository.ReplaceAsync(item, item.Id, cancellationToken);

							oldEntityToBeDeleted.Remove(data);

							hasUpdate = true;
						}
					}

					if (!hasUpdate)
					{
						oldEntity.AddOrReplaceRole(item);
					}
				}
			}

			if (oldEntityToBeDeleted.Count > 0)
			{
				foreach (var item in oldEntityToBeDeleted)
					oldEntity.RemoveRole(item);
			}

			await _unitOfWork.UserRoleRepository.UpdateAsync(oldEntity);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public bool ValidateBase(UserRole userRole)
		{
			return ServiceState;
		}

		public bool ValidateOnInsert(UserRole userRole)
		{
			ValidateBase(userRole);

			return ServiceState;
		}

		public bool ValidateOnUpdate(UserRole userRole)
		{
			ValidateOnUpdate(userRole);

			return ServiceState;
		}
	}
}
