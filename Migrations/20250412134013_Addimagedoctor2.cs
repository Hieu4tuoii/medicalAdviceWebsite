using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteTuVan.Migrations
{
    /// <inheritdoc />
    public partial class Addimagedoctor2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Doctors");
        }
    }
}
