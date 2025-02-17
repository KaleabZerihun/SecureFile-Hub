using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileProcessor.Migrations
{
    /// <inheritdoc />
    public partial class SaveFileToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "Items",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FileData",
                table: "Items");
        }
    }
}
