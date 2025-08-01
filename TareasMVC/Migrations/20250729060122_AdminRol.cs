using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS(SELECT Id FROM AspNetRoles WHERE Id = 'b01738e1-0176-4ee8-a955-9cb1383a2409')
BEGIN
	INSERT AspNetRoles (Id, [Name], [NormalizedName])
	VALUES ('b01738e1-0176-4ee8-a955-9cb1383a2409', 'admin', 'ADMIN')
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE AspNetRoles WHERE Id = 'b01738e1-0176-4ee8-a955-9cb1383a2409';");
        }
    }
}
