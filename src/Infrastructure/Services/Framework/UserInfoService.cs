using Ardalis.Specification;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Exceptions;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using AppCoreApi.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Services
{
	public class UserInfoService : AsyncBaseService<UserInfo>, IUserInfoService
	{
		private readonly IDownloadProcessService _downloadProcessService;
		public UserInfoService(
			IUnitOfWork unitOfWork, 
			IDownloadProcessService downloadProcessService) : base(unitOfWork)
		{
			_downloadProcessService = downloadProcessService;
		}

		public async Task<UserInfo> AddAsync(UserInfo entity, CancellationToken cancellationToken = default)
		{
			var filterSpec = new UserInfoFilterSpecification(entity.UserName, string.Empty, string.Empty);
			var rowCount = await _unitOfWork.UserInfoRepository.CountAsync(filterSpec, cancellationToken);
			if (rowCount > 0)
				throw new EntityAlreadyExistsException();

			AssignCreatorAndCompany(entity);
			await _unitOfWork.UserInfoRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<UserInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserInfoRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(UserInfo entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.UserInfoRepository.DeleteAsync(entity);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<UserInfo> FirstAsync(ISpecification<UserInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserInfoRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<UserInfo> FirstOrDefaultAsync(ISpecification<UserInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserInfoRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task GenerateExcel(int? refId = null, string firstName = "", string lastName = "", CancellationToken cancellationToken = default)
		{
			try
			{
				string excelDestinationPath = Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory, 
					"DownloadMonitor", 
					DateTime.Now.ToString("yyyy"), 
					DateTime.Now.ToString("MM")
				);

				var filterSpec = new UserInfoFilterSpecification(string.Empty, firstName, lastName);
				var results = await _unitOfWork.UserInfoRepository.ListAsync(filterSpec, null, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				using (var package = new ExcelPackage())
				{
					var ws = package.Workbook.Worksheets.Add("Sheet 1");

					// add header
					ws.Cells[1, 1].Value = "User Name";
					ws.Cells[1, 2].Value = "First Name";
					ws.Cells[1, 3].Value = "Last Name";

					// format header
					using (var range = ws.Cells[1, 1, 1, 4])
					{
						range.Style.Font.Bold = true;
						range.Style.Fill.PatternType = ExcelFillStyle.Solid;
						range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
						range.Style.Font.Color.SetColor(System.Drawing.Color.White);
					}

					if (results?.Count > 0)
					{
						// write excel content
						int row = 2;
						foreach (var item in results)
						{
							ws.Cells[row, 1].Value = item.UserName;
							ws.Cells[row, 2].Value = item.FirstName;
							ws.Cells[row, 3].Value = item.LastName;
							row++;
						}
					}

					ws.Cells.AutoFitColumns(0);

					string fileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".xlsx";
					FileOutputUtil.OutputDir = new DirectoryInfo(excelDestinationPath);
					var xFile = FileOutputUtil.GetFileInfo(fileName);
					package.SaveAs(xFile);

					// update database information (if needed)
					if (refId.HasValue)
						await _downloadProcessService.SuccessfullyGenerated(refId.Value, fileName);
				}
			}
			catch (Exception ex)
			{
				if(refId.HasValue)
					await _downloadProcessService.FailedToGenerate(refId.Value, ex.Message);

				throw;
			}
		}

		public async Task<UserInfo> GetByIdAsync(string id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserInfoRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<UserInfo>> ListAllAsync(List<SortingInformation<UserInfo>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserInfoRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<UserInfo>> ListAsync(ISpecification<UserInfo> spec, List<SortingInformation<UserInfo>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.UserInfoRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(UserInfo entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			AssignUpdater(entity);
			await _unitOfWork.UserInfoRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private bool ValidateBase(UserInfo entity)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(UserInfo entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(UserInfo entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
