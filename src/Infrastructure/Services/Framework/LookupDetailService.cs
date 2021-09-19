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
	public class LookupDetailService : AsyncBaseService<LookupDetail>, ILookupDetailService
	{
		public LookupDetailService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<LookupDetail> AddAsync(LookupDetail entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			await _unitOfWork.LookupDetailRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<LookupDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupDetailRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(LookupDetail entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.LookupDetailRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<LookupDetail> FirstAsync(ISpecification<LookupDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupDetailRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<LookupDetail> FirstOrDefaultAsync(ISpecification<LookupDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupDetailRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<LookupDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupDetailRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<LookupDetail>> ListAllAsync(List<SortingInformation<LookupDetail>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupDetailRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<LookupDetail>> ListAsync(ISpecification<LookupDetail> spec, List<SortingInformation<LookupDetail>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupDetailRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(LookupDetail entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			AssignUpdater(entity);
			await _unitOfWork.LookupDetailRepository.UpdateAsync(entity);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private bool ValidateBase(LookupDetail entity)
		{
			if (string.IsNullOrEmpty(entity.Name))
				AddError("Name harus diisi.");

			if (string.IsNullOrEmpty(entity.Value))
				AddError("Value harus diisi.");

			return ServiceState;
		}

		private bool ValidateOnInsert(LookupDetail entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(LookupDetail entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
