using Ardalis.Specification;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Services
{
	public class FunctionInfoService : AsyncBaseService<FunctionInfo>, IFunctionInfoService
	{
		public FunctionInfoService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<FunctionInfo> AddAsync(FunctionInfo entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			entity.Id = entity.Name;
			entity.ModuleInfo = null;
			if(entity.IsEnabled == null)
            {
				entity.IsEnabled = false;
            }
			//entity.ModuleInfoId = int.Parse(entity.ModuleName);
			var GetName = await _unitOfWork.ModuleInfoRepository.GetByIdAsync(entity.ModuleInfoId.GetValueOrDefault(), default);
			entity.ModuleName = GetName.Name;


			AssignCreatorAndCompany(entity);
			await _unitOfWork.FunctionInfoRepository.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(FunctionInfo entity, CancellationToken cancellationToken = default)
		{
			var searchFungsionInfo = await _unitOfWork.FunctionInfoRepository.searcFungsionInfos(entity.Id);
			if (searchFungsionInfo == false)
			{
				AddError("Function Info Tidak bisa dihapus karena digunakan di fungsi lain");
				return false;
			}

			_unitOfWork.FunctionInfoRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<FunctionInfo> FirstAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<FunctionInfo> FirstOrDefaultAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<FunctionInfo> GetByIdAsync(string id, CancellationToken cancellationToken = default)
		{
			var filter = new FunctionInfoFilterSpecification(id, true);
			var result = await _unitOfWork.FunctionInfoRepository.FirstOrDefaultAsync(filter, cancellationToken);
			return result;
		}

		public async Task<IReadOnlyList<FunctionInfo>> ListAllAsync(List<SortingInformation<FunctionInfo>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<FunctionInfo>> ListAsync(ISpecification<FunctionInfo> spec, List<SortingInformation<FunctionInfo>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(FunctionInfo entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			AssignUpdater(entity);
			await _unitOfWork.FunctionInfoRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		private bool ValidateBase(FunctionInfo functionInfo)
		{
            if (functionInfo is null)
            {
                throw new System.ArgumentNullException(nameof(functionInfo));
            }

            return ServiceState;
		}

		private bool ValidateOnInsert(FunctionInfo functionInfo)
		{
			ValidateBase(functionInfo);

			return ServiceState;
		}

		private bool ValidateOnUpdate(FunctionInfo functionInfo)
		{
			ValidateBase(functionInfo);

			return ServiceState;
		}
	}
}
