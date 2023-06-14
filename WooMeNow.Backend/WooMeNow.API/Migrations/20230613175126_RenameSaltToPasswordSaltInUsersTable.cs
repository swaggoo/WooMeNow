using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WooMeNow.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameSaltToPasswordSaltInUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Salt",
                table: "Users",
                newName: "PasswordSalt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordSalt",
                table: "Users",
                newName: "Salt");
        }
    }
}
