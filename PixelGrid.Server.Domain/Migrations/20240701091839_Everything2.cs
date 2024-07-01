using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelGrid.Server.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Everything2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permission",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Permission",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
