using Microsoft.EntityFrameworkCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.Infrastructure.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        #region Framework

        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }
        public DbSet<FunctionInfo> FunctionInfos { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleDetail> RoleDetails { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRoleDetail> UserRoleDetails { get; set; }
        public DbSet<SchedulerCronInterval> SchedulerCronIntervals { get; set; }
        public DbSet<JobConfiguration> JobConfigurations { get; set; }
        public DbSet<SchedulerConfiguration> SchedulerConfigurations { get; set; }
        public DbSet<DownloadProcess> DownloadProcesses { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<LookupDetail> LookupDetails { get; set; }
        public DbSet<ModuleInfo> ModuleInfos { get; set; }
        public DbSet<SpResultNumberGenerator> SpResultNumberGenerators { get; set; }

        public DbSet<RoleManagement> RoleManagements { get; set; }
        public DbSet<RoleManagementDetail> RoleManagementDetails { get; set; }

        #endregion

        // do not remove region marker. this marker is used by code generator
        #region Application Variables

			public DbSet<Soal> Soals { get; set; }
			public DbSet<Kuisioner> Kuisioners { get; set; }
			public DbSet<KuisionerDetail> KuisionerDetails { get; set; }
        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(180);
        }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["DeletedAt"] = null;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["DeletedAt"] = DateTime.Now;
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Framework

            modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new EmailConfiguration());
            modelBuilder.ApplyConfiguration(new EmailAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new FunctionInfoConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleDetailConfiguration());
            modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleDetailConfiguration());
            modelBuilder.ApplyConfiguration(new SchedulerCronIntervalConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new SchedulerConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new DownloadProcessConfiguration());
            modelBuilder.ApplyConfiguration(new LookupConfiguration());
            modelBuilder.ApplyConfiguration(new LookupDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleInfoConfiguration());
            modelBuilder.ApplyConfiguration(new SpResultNumberGeneratorConfiguration());
            modelBuilder.ApplyConfiguration(new RoleManagementConfiguration());
            modelBuilder.ApplyConfiguration(new RoleManagementDetailConfiguration());
            #endregion

            // do not remove region marker. this marker is used by code generator
            #region Application Config

			modelBuilder.ApplyConfiguration(new SoalConfiguration());
			modelBuilder.ApplyConfiguration(new KuisionerConfiguration());
			modelBuilder.ApplyConfiguration(new KuisionerDetailConfiguration());
            #endregion
        }
    }
}
