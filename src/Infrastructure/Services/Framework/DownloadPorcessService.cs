using Ardalis.Specification;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Exceptions;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Services
{
	public class DownloadProcessService : AsyncBaseService<DownloadProcess>, IDownloadProcessService
	{

		public DownloadProcessService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<DownloadProcess> AddAsync(DownloadProcess entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);
			await _unitOfWork.DownloadProcessRepository.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<DownloadProcess> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.DownloadProcessRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(DownloadProcess entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.DownloadProcessRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task FailedToGenerate(int Id, string description, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var filterSpec = new DownloadProcessFilterSpecification(Id);
			var entity = await _unitOfWork.DownloadProcessRepository.FirstOrDefaultAsync(filterSpec, cancellationToken);
			if (entity == null)
				throw new EntityNotFoundException(nameof(DownloadProcess), Id);

			entity.Status = "FAILED";
			entity.ErrorMessage = description;
			entity.FileName = "";
			entity.EndTime = DateTime.Now;
			AssignUpdater(entity);
			await _unitOfWork.DownloadProcessRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
		}

		public async Task<DownloadProcess> FirstAsync(ISpecification<DownloadProcess> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.DownloadProcessRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<DownloadProcess> FirstOrDefaultAsync(ISpecification<DownloadProcess> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.DownloadProcessRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<DownloadProcess> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			var filterSpec = new DownloadProcessFilterSpecification(id, true);
			return await _unitOfWork.DownloadProcessRepository.FirstOrDefaultAsync(filterSpec, cancellationToken);
			// return await _unitOfWork.DownloadProcessRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<DownloadProcess>> ListAllAsync(List<SortingInformation<DownloadProcess>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.DownloadProcessRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<DownloadProcess>> ListAsync(ISpecification<DownloadProcess> spec, List<SortingInformation<DownloadProcess>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.DownloadProcessRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task SuccessfullyGenerated(int Id, string fileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var filterSpec = new DownloadProcessFilterSpecification(Id);
			var entity = await _unitOfWork.DownloadProcessRepository.FirstOrDefaultAsync(filterSpec, cancellationToken);
			if (entity == null)
				throw new EntityNotFoundException(nameof(DownloadProcess), Id);

			entity.Status = "SUCCESS";
			entity.ErrorMessage = string.Empty;
			entity.FileName = fileName;
			entity.EndTime = DateTime.Now;
			AssignUpdater(entity);
			await _unitOfWork.DownloadProcessRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
		}

		public async Task<string> RegisterDownloadProcess(string functionId, string jobId, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var newItem = new DownloadProcess(functionId);
			newItem.JobId = jobId;
			await AddAsync(newItem);
			return newItem.JobId;
		}

		public async Task<DownloadProcess> GetByJobIdAsync(string jobId, CancellationToken cancellationToken = default)
		{
			var spec = new DownloadProcessFilterSpecification(jobId, string.Empty, string.Empty);
			return await _unitOfWork.DownloadProcessRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<bool> UpdateAsync(DownloadProcess entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			await _unitOfWork.DownloadProcessRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private bool ValidateBase(DownloadProcess entity)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(DownloadProcess entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(DownloadProcess entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

	}
}
