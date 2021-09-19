using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore
{
    public interface IUnitOfWork : IDisposable
    {
        #region Framework Unit of Work
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        ICompanyRepository Companies { get; }
        IAttachmentRepository AttachmentRepository { get; }
        IEmailRepository Emails { get; }
        IEmailAttachmentRepository EmailAttachments { get; }
        IFunctionInfoRepository FunctionInfoRepository { get; }
        IRoleDetailRepository RoleDetailRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserInfoRepository UserInfoRepository { get; }
        IUserRoleDetailRepository UserRoleDetailRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        ISchedulerCronIntervalRepository SchedulerCronIntervalRepository { get; }
        IJobConfigurationRepository JobConfigurationRepository { get; }
        ISchedulerConfigurationRepository SchedulerConfigurationRepository { get; }
        IDownloadProcessRepository DownloadProcessRepository { get; }
        ILookupRepository LookupRepository { get; }
        ILookupDetailRepository LookupDetailRepository { get; }
        IGenericRepository GenericRepository { get; }
        IModuleInfoRepository ModuleInfoRepository { get; }
        IRoleManagementRepository RoleManagementRepository { get; }
        IRoleManagementDetailRepository RoleManagementDetailRepository { get; }
        public IObjectConverter CreateObjectMapConverter(string baseConfigPath);

        #endregion

        // do not remove region marker. this marker is used by code generator
        #region Application

			ISoalRepository SoalRepository { get; }
			IKuisionerRepository KuisionerRepository { get; }
			IKuisionerDetailRepository KuisionerDetailRepository { get; }
        #endregion
    }
}
