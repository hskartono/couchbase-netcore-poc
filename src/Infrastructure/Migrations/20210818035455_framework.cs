using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppCoreApi.Infrastructure.Migrations
{
    public partial class framework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SavedFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoreRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sender = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Receiver = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReceiverCC = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MailContent = table.Column<string>(type: "text", nullable: true),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterfaceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsStoredProcedure = table.Column<bool>(type: "bit", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentModuleId = table.Column<int>(type: "int", nullable: true),
                    OrderPosition = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerCronIntervals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainRecordId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerCronIntervals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpResultNumberGenerators",
                columns: table => new
                {
                    NumberGenerated = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "EmailAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailId = table.Column<int>(type: "int", nullable: false),
                    AttachmentId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailAttachments_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LookupDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    LookupId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupDetails_Lookups_LookupId",
                        column: x => x.LookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunctionInfos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IconName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: true),
                    ModuleInfoId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FunctionInfos_ModuleInfos_ModuleInfoId",
                        column: x => x.ModuleInfoId,
                        principalTable: "ModuleInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntervalTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IntervalValue = table.Column<int>(type: "int", nullable: true),
                    IntervalValue2 = table.Column<int>(type: "int", nullable: true),
                    IntervalValue3 = table.Column<int>(type: "int", nullable: true),
                    CronExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTypeId = table.Column<int>(type: "int", nullable: true),
                    RecurringJobId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulerConfigurations_JobConfigurations_JobTypeId",
                        column: x => x.JobTypeId,
                        principalTable: "JobConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchedulerConfigurations_SchedulerCronIntervals_IntervalTypeId",
                        column: x => x.IntervalTypeId,
                        principalTable: "SchedulerCronIntervals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserInfoId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_UserInfos_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "UserInfos",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoreRoleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FunctionInfoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AllowCreate = table.Column<bool>(type: "bit", nullable: false),
                    AllowRead = table.Column<bool>(type: "bit", nullable: false),
                    AllowUpdate = table.Column<bool>(type: "bit", nullable: false),
                    AllowDelete = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMenu = table.Column<bool>(type: "bit", nullable: false),
                    AllowDownload = table.Column<bool>(type: "bit", nullable: false),
                    AllowPrint = table.Column<bool>(type: "bit", nullable: false),
                    AllowUpload = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreRoleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreRoleDetails_CoreRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "CoreRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoreRoleDetails_FunctionInfos_FunctionInfoId",
                        column: x => x.FunctionInfoId,
                        principalTable: "FunctionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DownloadProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunctionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DownloadProcesses_FunctionInfos_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "FunctionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    FunctionInfoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AllowCreate = table.Column<bool>(type: "bit", nullable: true),
                    AllowRead = table.Column<bool>(type: "bit", nullable: true),
                    AllowUpdate = table.Column<bool>(type: "bit", nullable: true),
                    AllowDelete = table.Column<bool>(type: "bit", nullable: true),
                    ShowInMenu = table.Column<bool>(type: "bit", nullable: true),
                    AllowDownload = table.Column<bool>(type: "bit", nullable: true),
                    AllowPrint = table.Column<bool>(type: "bit", nullable: true),
                    AllowUpload = table.Column<bool>(type: "bit", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleDetails_FunctionInfos_FunctionInfoId",
                        column: x => x.FunctionInfoId,
                        principalTable: "FunctionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleDetails_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRoleId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDraftRecord = table.Column<int>(type: "int", nullable: true),
                    RecordActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainRecordId = table.Column<int>(type: "int", nullable: true),
                    RecordEditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftFromUpload = table.Column<bool>(type: "bit", nullable: true),
                    UploadValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoleDetails_CoreRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "CoreRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleDetails_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoreRoleDetails_FunctionInfoId",
                table: "CoreRoleDetails",
                column: "FunctionInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CoreRoleDetails_RoleId",
                table: "CoreRoleDetails",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadProcesses_FunctionId",
                table: "DownloadProcesses",
                column: "FunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAttachments_AttachmentId",
                table: "EmailAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAttachments_EmailId",
                table: "EmailAttachments",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionInfos_ModuleInfoId",
                table: "FunctionInfos",
                column: "ModuleInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupDetails_LookupId",
                table: "LookupDetails",
                column: "LookupId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleDetails_FunctionInfoId",
                table: "RoleDetails",
                column: "FunctionInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleDetails_RoleId",
                table: "RoleDetails",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerConfigurations_IntervalTypeId",
                table: "SchedulerConfigurations",
                column: "IntervalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerConfigurations_JobTypeId",
                table: "SchedulerConfigurations",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleDetails_RoleId",
                table: "UserRoleDetails",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleDetails_UserRoleId",
                table: "UserRoleDetails",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserInfoId",
                table: "UserRoles",
                column: "UserInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "CoreRoleDetails");

            migrationBuilder.DropTable(
                name: "DownloadProcesses");

            migrationBuilder.DropTable(
                name: "EmailAttachments");

            migrationBuilder.DropTable(
                name: "LookupDetails");

            migrationBuilder.DropTable(
                name: "RoleDetails");

            migrationBuilder.DropTable(
                name: "SchedulerConfigurations");

            migrationBuilder.DropTable(
                name: "SpResultNumberGenerators");

            migrationBuilder.DropTable(
                name: "UserRoleDetails");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Lookups");

            migrationBuilder.DropTable(
                name: "FunctionInfos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "JobConfigurations");

            migrationBuilder.DropTable(
                name: "SchedulerCronIntervals");

            migrationBuilder.DropTable(
                name: "CoreRoles");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "ModuleInfos");

            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
