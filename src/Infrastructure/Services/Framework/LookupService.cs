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
	public class LookupService : AsyncBaseService<Lookup>, ILookupService
	{
		public LookupService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<Lookup> AddAsync(Lookup entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);

			foreach (var item in entity.LookupDetails)
				AssignCreatorAndCompany(item);

			await _unitOfWork.LookupRepository.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<Lookup> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(Lookup entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.LookupRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<Lookup> FirstAsync(ISpecification<Lookup> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<Lookup> FirstOrDefaultAsync(ISpecification<Lookup> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<Lookup> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<Lookup>> ListAllAsync(List<SortingInformation<Lookup>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupRepository.ListAllAsync(sorting, cancellationToken); throw new NotImplementedException();
		}

		public async Task<IReadOnlyList<Lookup>> ListAsync(ISpecification<Lookup> spec, List<SortingInformation<Lookup>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.LookupRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(Lookup entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			AssignUpdater(entity);
			await _unitOfWork.LookupRepository.ReplaceAsync(entity, entity.Id, cancellationToken);

			var specFilter = new LookupFilterSpecification(entity.Id);
			var oldEntity = await _unitOfWork.LookupRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (oldEntity == null)
			{
				AddError($"Could not load {nameof(Lookup)} data with id {entity.Id}.");
				return false;
			}

			// smart update
			await SmartUpdateLookupDetail(oldEntity, entity, cancellationToken);

			await _unitOfWork.LookupRepository.UpdateAsync(entity);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private async Task SmartUpdateLookupDetail(Lookup oldEntity, Lookup entity, CancellationToken cancellationToken = default)
		{
			List<LookupDetail> oldEntityToBeDeleted = new List<LookupDetail>();
			if (oldEntity.LookupDetails.Count > 0)
			{
				foreach (var item in oldEntity.LookupDetails)
					oldEntityToBeDeleted.Add(item);
			}

			if (entity.LookupDetails.Count > 0)
			{
				foreach (var item in entity.LookupDetails)
				{
					var hasUpdate = false;
					if (oldEntity.LookupDetails.Count > 0)
					{
						var data = oldEntity.LookupDetails.SingleOrDefault(p => p.Id == item.Id);
						if (data != null)
						{
							AssignUpdater(item);
							await _unitOfWork.LookupDetailRepository.ReplaceAsync(item, item.Id, cancellationToken);

							oldEntityToBeDeleted.Remove(data);
						}
					}

					if (!hasUpdate)
					{
						AssignCreatorAndCompany(item);
						oldEntity.AddOrReplaceLookupDetail(item);
					}
				}
			}

			if (oldEntityToBeDeleted.Count > 0)
			{
				foreach (var item in oldEntityToBeDeleted)
					oldEntity.RemoveLookupDetail(item);
			}
		}

		private bool ValidateBase(Lookup entity)
		{
			if (string.IsNullOrEmpty(entity.Name))
				AddError("Name harus diisi.");

			if (entity.LookupDetails.Count <= 0)
				AddError("Lookup Detail harus diisi.");

			int row = 0;
			foreach(var item in entity.LookupDetails)
			{
				row++;
				if (string.IsNullOrEmpty(item.Name))
					AddError($"Name pada lookup detail baris ke {row} harus diisi.");

				if (string.IsNullOrEmpty(item.Value))
					AddError($"Value pada lookup detail baris ke {row} harus diisi.");
			}

			return ServiceState;
		}

		private bool ValidateOnInsert(Lookup entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(Lookup entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
