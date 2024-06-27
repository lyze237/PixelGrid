using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelGrid.Server.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedRenderPrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "RenderClients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RenderClientProgramVersionEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    RendererCapabilities = table.Column<int>(type: "INTEGER", nullable: true),
                    RenderClientId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenderClientProgramVersionEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RenderClientProgramVersionEntity_RenderClients_RenderClientId",
                        column: x => x.RenderClientId,
                        principalTable: "RenderClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RenderClientProgramVersionEntity_RenderClientId",
                table: "RenderClientProgramVersionEntity",
                column: "RenderClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RenderClientProgramVersionEntity");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "RenderClients");
        }
    }
}
