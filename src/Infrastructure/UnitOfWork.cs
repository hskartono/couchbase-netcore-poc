using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.Infrastructure.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ICouchbaseRepository _couchbaseRepository;

        #region Framework private variable

        private IAttachmentRepository attachmentRepository;
        private ICompanyRepository companyRepository;
        private IEmailRepository emailRepository;
        private IEmailAttachmentRepository emailAttachmentRepository;
        private IFunctionInfoRepository functionInfoRepository;
        private IRoleRepository roleRepository;
        private IRoleDetailRepository roleDetailRepository;
        private IUserInfoRepository userInfoRepository;
        private IUserRoleRepository userRoleRepository;
        private IUserRoleDetailRepository userRoleDetailRepository;
        private ISchedulerCronIntervalRepository schedulerCronIntervalRepository;
        private IJobConfigurationRepository jobConfigurationRepository;
        private ISchedulerConfigurationRepository schedulerConfigurationRepository;
        private IDownloadProcessRepository downloadProcessRepository;
        private ILookupRepository lookupRepository;
        private ILookupDetailRepository lookupDetailRepository;
        private IGenericRepository genericRepository;
        private IModuleInfoRepository moduleInfoRepository;
        private IRoleManagementRepository roleManagementRepository;
        private IRoleManagementDetailRepository roleManagementDetailRepository;

        #endregion

        // do not remove region marker. this marker is used by code generator
        #region Application private variable

        private ISoalRepository soalRepository;
        private IKuisionerRepository kuisionerRepository;
        private IKuisionerDetailRepository kuisionerDetailRepository;
        #endregion

        #region Framework properties

        public ICompanyRepository Companies =>
            companyRepository ??= new CompanyRepository(_context);

        public IAttachmentRepository AttachmentRepository =>
            attachmentRepository ??= new AttachmentRepository(_context);

        public IEmailRepository Emails =>
            emailRepository ??= new EmailRepository(_context);

        public IEmailAttachmentRepository EmailAttachments =>
            emailAttachmentRepository ??= new EmailAttachmentRepository(_context);

        public IFunctionInfoRepository FunctionInfoRepository =>
            functionInfoRepository ??= new FunctionInfoRepository(_context);

        public IRoleDetailRepository RoleDetailRepository =>
            roleDetailRepository ??= new RoleDetailRepository(_context);

        public IRoleRepository RoleRepository =>
            roleRepository ??= new RoleRepository(_context);

        IUserInfoRepository IUnitOfWork.UserInfoRepository =>
            userInfoRepository ??= new UserInfoRepository(_context);

        IUserRoleDetailRepository IUnitOfWork.UserRoleDetailRepository =>
            userRoleDetailRepository ??= new UserRoleDetailRepository(_context);

        public IUserRoleRepository UserRoleRepository =>
            userRoleRepository ??= new UserRoleRepository(_context);

        public ISchedulerCronIntervalRepository SchedulerCronIntervalRepository =>
            schedulerCronIntervalRepository ??= new SchedulerCronIntervalRepository(_context);

        public IJobConfigurationRepository JobConfigurationRepository =>
            jobConfigurationRepository ??= new JobConfigurationRepository(_context);

        public ISchedulerConfigurationRepository SchedulerConfigurationRepository =>
            schedulerConfigurationRepository ??= new SchedulerConfigurationRepository(_context);

        public IDownloadProcessRepository DownloadProcessRepository =>
            downloadProcessRepository ??= new DownloadProcessRepository(_context);

        public ILookupRepository LookupRepository => lookupRepository ??= new LookupRepository(_context);
        public ILookupDetailRepository LookupDetailRepository => lookupDetailRepository ??= new LookupDetailRepository(_context);

        public IModuleInfoRepository ModuleInfoRepository => moduleInfoRepository ??= new ModuleInfoRepository(_context);

        public IGenericRepository GenericRepository => genericRepository ??= new GenericRepository(_context);

        public IRoleManagementRepository RoleManagementRepository => roleManagementRepository ??= new RoleManagementRepository(_context);
        public IRoleManagementDetailRepository RoleManagementDetailRepository => roleManagementDetailRepository ??= new RoleManagementDetailRepository(_context);

        #endregion

        // do not remove region marker. this marker is used by code generator
        #region Application private properties

        //public ISoalRepository SoalRepository => soalRepository = soalRepository ?? new SoalRepository(_context);
        public ISoalRepository SoalRepository => soalRepository = soalRepository ?? new SoalRepository(_couchbaseRepository);
        public IKuisionerRepository KuisionerRepository => kuisionerRepository = kuisionerRepository ?? new KuisionerRepository(_context);
        public IKuisionerDetailRepository KuisionerDetailRepository => kuisionerDetailRepository = kuisionerDetailRepository ?? new KuisionerDetailRepository(_context);
        #endregion

        public UnitOfWork(AppDbContext context, ICouchbaseRepository couchbaseRepository)
        {
            _context = context;
            _couchbaseRepository = couchbaseRepository;
        }

        public UnitOfWork()
        {

        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public IObjectConverter CreateObjectMapConverter(string baseConfigPath)
        {
            var converter = new xlmap.ObjectConverter(_context, baseConfigPath);
            return converter;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _context.Dispose();
        }
    }
}
