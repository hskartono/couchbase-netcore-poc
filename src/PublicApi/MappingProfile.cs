using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.Infrastructure.Utility;
using AppCoreApi.PublicApi.Features.Attachments;
using AppCoreApi.PublicApi.Features.DownloadProcesses;
using AppCoreApi.PublicApi.Features.Emails;
using AppCoreApi.PublicApi.Features.FunctionInfos;
using AppCoreApi.PublicApi.Features.JobConfigurations;
using AppCoreApi.PublicApi.Features.ModuleInfos;
using AppCoreApi.PublicApi.Features.RoleManagements;
using AppCoreApi.PublicApi.Features.Roles;
using AppCoreApi.PublicApi.Features.SchedulerConfigurations;
using AppCoreApi.PublicApi.Features.SchedulerCronIntervals;
using AppCoreApi.PublicApi.Features.UserInfos;
using AppCoreApi.PublicApi.Features.UserRoles;
using AutoMapper;
using System;

using AppCoreApi.PublicApi.Features.Soals;
using AppCoreApi.PublicApi.Features.Kuisioners;
namespace AppCoreApi.PublicApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // function info
            CreateMap<FunctionInfo, FunctionInfoDTO>()
                .ForMember(dto => dto.ModuleName, opt => opt.MapFrom(src => src.ModuleInfo.Name)).ReverseMap(); ;
            //.ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name))
            //.ForMember(dto => dto.Uri, opt => opt.MapFrom(src => src.Uri))
            //.ForMember(dto => dto.IconName, opt => opt.MapFrom(src => src.IconName))
            //.ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name));

            // email
            CreateMap<Email, EmailDTO>();
            CreateMap<EmailAttachment, EmailAttachmentDTO>()
                .ForMember(dto => dto.AttachmentId, opt => opt.MapFrom(src => src.AttachmentId))
                .ForMember(dto => dto.FileExtension, opt => opt.MapFrom(src => src.Attachment.FileExtension))
                .ForMember(dto => dto.FileSize, opt => opt.MapFrom(src => src.Attachment.FileSize))
                .ForMember(dto => dto.OriginalFileName, opt => opt.MapFrom(src => src.Attachment.OriginalFileName))
                .ForMember(dto => dto.SavedFileName, opt => opt.MapFrom(src => src.Attachment.SavedFileName));

            // role
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<RoleDetail, RoleDetailDTO>()
                .ForMember(dto => dto.FunctionName, opt => opt.MapFrom(src => src.FunctionInfo.Name))
                .ForMember(dto => dto.ModuleName, opt => opt.MapFrom(src => src.FunctionInfo.ModuleInfo.Name));
            CreateMap<RoleDetailDTO, RoleDetail>();

            // user info
            CreateMap<UserInfo, UserInfoDTO>().ReverseMap();

            // module info
            CreateMap<ModuleInfo, ModuleInfoDTO>().ReverseMap();

            // user role
            CreateMap<UserRole, UserRoleDTO>().ReverseMap();
            CreateMap<UserRoleDetail, UserRoleDetailDTO>()
                .ForMember(dto => dto.RoleName, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<UserRoleDetailDTO, UserRoleDetail>();

            // scheduler cron interval
            CreateMap<SchedulerCronInterval, SchedulerCronIntervalDTO>().ReverseMap();

            // job configuration
            CreateMap<JobConfiguration, JobConfigurationDTO>().ReverseMap();

            // scheduler configuration
            CreateMap<SchedulerConfiguration, SchedulerConfigurationDTO>().ReverseMap();

            // download process
            CreateMap<DownloadProcess, DownloadProcessDTO>()
                .ForMember(dto => dto.FunctionName, opt => opt.MapFrom(src => src.FunctionInfo.Name))
                .ReverseMap();

            // attachment
            var baseUri = new Uri(ConfigurationManager.AppSetting["PathConfig:BaseURI"]);
            var downloadUri = new Uri(baseUri, ConfigurationManager.AppSetting["PathConfig:UploadURI"]);
            CreateMap<Attachment, AttachmentDTO>()
                .ForMember(dto => dto.DownloadUrl, opt => opt.MapFrom(
                      (s, d) => d.DownloadUrl = downloadUri.AbsoluteUri + s.SavedFileName)
                )
                .ReverseMap();

            CreateMap<RoleManagement, RoleManagementDTO>().ReverseMap();
            CreateMap<RoleManagementDetail, RoleManagementDetailDTO>().ForMember(dto => dto.FunctionInfoName, opt => opt.MapFrom(src => src.FunctionInfo.Name)).ReverseMap();

            // do not remove region marker. this marker is used by code generator
            #region Application Entity

			CreateMap<Soal, SoalDTO>().ReverseMap();

			CreateMap<Kuisioner, KuisionerDTO>().ReverseMap();
			CreateMap<KuisionerDetail, KuisionerDetailDTO>().ForMember(dto => dto.SoalKonten, opt => opt.MapFrom(src => src.Soal.Konten)).ReverseMap();

            #endregion
        }
    }
}
