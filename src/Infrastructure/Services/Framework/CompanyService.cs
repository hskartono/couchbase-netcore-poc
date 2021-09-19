using Ardalis.Specification;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Services
{
	public class CompanyService : AsyncBaseService<Company>, ICompanyService
	{
		public CompanyService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<Company> AddAsync(Company entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			await _unitOfWork.Companies.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<Company> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Companies.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(Company entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.Companies.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<Company> FirstAsync(ISpecification<Company> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Companies.FirstAsync(spec, cancellationToken);
		}

		public async Task<Company> FirstOrDefaultAsync(ISpecification<Company> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Companies.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<Company> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Companies.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<Company>> ListAllAsync(List<SortingInformation<Company>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Companies.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<Company>> ListAsync(ISpecification<Company> spec, List<SortingInformation<Company>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.Companies.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(Company entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			await _unitOfWork.Companies.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		private bool ValidateBase(Company entity)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(Company entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(Company entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
