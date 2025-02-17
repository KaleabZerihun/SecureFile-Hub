using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileProcessor.Migrations
{
    /// <inheritdoc />
    public partial class AddedFileToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "Items");
        }
    }
}
