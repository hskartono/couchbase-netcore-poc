using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppCoreApi.Infrastructure.Migrations
{
    public partial class Soal_f2e1465a_4cb3_444f_8217_1c86bf43c2f3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Soals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Konten = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Soals", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Soals");
        }
    }
}
