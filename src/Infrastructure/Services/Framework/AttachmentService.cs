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
	public class AttachmentService : AsyncBaseService<Attachment>, IAttachmentService
	{
		public AttachmentService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<Attachment> AddAsync(Attachment entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			await _unitOfWork.AttachmentRepository.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<Attachment> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.AttachmentRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(Attachment entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.AttachmentRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<Attachment> FirstAsync(ISpecification<Attachment> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.AttachmentRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<Attachment> FirstOrDefaultAsync(ISpecification<Attachment> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.AttachmentRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<Attachment> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.AttachmentRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<Attachment>> ListAllAsync(List<SortingInformation<Attachment>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.AttachmentRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<Attachment>> ListAsync(ISpecification<Attachment> spec, List<SortingInformation<Attachment>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.AttachmentRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(Attachment entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			await _unitOfWork.AttachmentRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		private bool ValidateBase(Attachment entity)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(Attachment entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(Attachment entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
