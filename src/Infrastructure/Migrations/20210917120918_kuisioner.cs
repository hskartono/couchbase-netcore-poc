using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppCoreApi.Infrastructure.Migrations
{
    public partial class kuisioner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Kuisioners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Judul = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AktifDari = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifSampai = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Kuisioners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KuisionerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KuisionerId = table.Column<int>(type: "int", nullable: true),
                    SoalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    KontenSoal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pilihan1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PIlihan2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pilihan3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KunciJawaban = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_KuisionerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KuisionerDetails_Kuisioners_KuisionerId",
                        column: x => x.KuisionerId,
                        principalTable: "Kuisioners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KuisionerDetails_Soals_SoalId",
                        column: x => x.SoalId,
                        principalTable: "Soals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KuisionerDetails_KuisionerId",
                table: "KuisionerDetails",
                column: "KuisionerId");

            migrationBuilder.CreateIndex(
                name: "IX_KuisionerDetails_SoalId",
                table: "KuisionerDetails",
                column: "SoalId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "KuisionerDetails");

            migrationBuilder.DropTable(
                name: "Kuisioners");

        }
    }
}
