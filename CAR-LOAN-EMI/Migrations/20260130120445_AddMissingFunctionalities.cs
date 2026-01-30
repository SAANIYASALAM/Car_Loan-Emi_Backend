using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAR_LOAN_EMI.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingFunctionalities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_KycDocuments",
                table: "KycDocuments");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "KycDocuments");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "KycDocuments");

            migrationBuilder.RenameColumn(
                name: "DocumentPath",
                table: "KycDocuments",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "KycDocuments",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "KycDocuments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "KycDocumentId",
                table: "KycDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "KycDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "KycDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_KycDocuments",
                table: "KycDocuments",
                column: "KycDocumentId");

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_ContactMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_UserId",
                table: "ContactMessages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KycDocuments",
                table: "KycDocuments");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "KycDocumentId",
                table: "KycDocuments");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "KycDocuments");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "KycDocuments");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "KycDocuments",
                newName: "DocumentId");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "KycDocuments",
                newName: "DocumentPath");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "KycDocuments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "KycDocuments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "KycDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_KycDocuments",
                table: "KycDocuments",
                column: "DocumentId");
        }
    }
}
