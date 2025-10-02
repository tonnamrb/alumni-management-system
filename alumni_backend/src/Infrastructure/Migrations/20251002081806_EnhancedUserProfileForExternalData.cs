using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedUserProfileForExternalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExternalDataLastSync",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalMemberID",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalSystemId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GraduationYear",
                table: "AlumniProfiles",
                type: "integer",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AlumniProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExternalDataLastSync",
                table: "AlumniProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalMemberID",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalSystemId",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "AlumniProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameInYearbook",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NickName",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProfileCompletedAt",
                table: "AlumniProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpouseName",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleCode",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkAddress",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "AlumniProfiles",
                type: "text",
                nullable: true);

            // Create indexes for performance
            migrationBuilder.CreateIndex(
                name: "IX_Users_MobilePhone",
                table: "Users",
                column: "MobilePhone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ExternalMemberID",
                table: "Users",
                column: "ExternalMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_AlumniProfiles_ExternalMemberID",
                table: "AlumniProfiles",
                column: "ExternalMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ExternalSystemId",
                table: "Users",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_AlumniProfiles_ExternalSystemId",
                table: "AlumniProfiles",
                column: "ExternalSystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_AlumniProfiles_ExternalSystemId",
                table: "AlumniProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AlumniProfiles_ExternalMemberID",
                table: "AlumniProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Users_ExternalSystemId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ExternalMemberID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_MobilePhone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExternalDataLastSync",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExternalMemberID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExternalSystemId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "District",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "ExternalDataLastSync",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "ExternalMemberID",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "ExternalSystemId",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Lastname",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "NameInYearbook",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "NickName",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileCompletedAt",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "SpouseName",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "TitleCode",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "WorkAddress",
                table: "AlumniProfiles");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "AlumniProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "GraduationYear",
                table: "AlumniProfiles",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
