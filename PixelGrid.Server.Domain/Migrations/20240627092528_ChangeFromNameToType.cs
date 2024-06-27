using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelGrid.Server.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFromNameToType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "RenderClientProgramVersionEntity");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "RenderClientProgramVersionEntity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "RenderClientProgramVersionEntity");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RenderClientProgramVersionEntity",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
