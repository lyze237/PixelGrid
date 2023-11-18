using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelGrid.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Resources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_AspNetUsers_SharedUsersId",
                table: "ClientUser");

            migrationBuilder.RenameColumn(
                name: "SharedUsersId",
                table: "ClientUser",
                newName: "SharedWithId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUser_SharedUsersId",
                table: "ClientUser",
                newName: "IX_ClientUser_SharedWithId");

            migrationBuilder.AddColumn<string>(
                name: "CyclesDevices",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlendFiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Scenes = table.Column<string>(type: "TEXT", nullable: false),
                    Cameras = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlendFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlendFiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectUser",
                columns: table => new
                {
                    SharedProjectsId = table.Column<string>(type: "TEXT", nullable: false),
                    SharedWithId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUser", x => new { x.SharedProjectsId, x.SharedWithId });
                    table.ForeignKey(
                        name: "FK_ProjectUser_AspNetUsers_SharedWithId",
                        column: x => x.SharedWithId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Projects_SharedProjectsId",
                        column: x => x.SharedProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlendFiles_ProjectId",
                table: "BlendFiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_SharedWithId",
                table: "ProjectUser",
                column: "SharedWithId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_AspNetUsers_SharedWithId",
                table: "ClientUser",
                column: "SharedWithId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_AspNetUsers_SharedWithId",
                table: "ClientUser");

            migrationBuilder.DropTable(
                name: "BlendFiles");

            migrationBuilder.DropTable(
                name: "ProjectUser");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropColumn(
                name: "CyclesDevices",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "SharedWithId",
                table: "ClientUser",
                newName: "SharedUsersId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUser_SharedWithId",
                table: "ClientUser",
                newName: "IX_ClientUser_SharedUsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_AspNetUsers_SharedUsersId",
                table: "ClientUser",
                column: "SharedUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
