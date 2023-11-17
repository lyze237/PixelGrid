using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelGrid.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Clients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LastConnected = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastRender = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Public = table.Column<bool>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientUser",
                columns: table => new
                {
                    SharedClientsId = table.Column<string>(type: "TEXT", nullable: false),
                    SharedUsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUser", x => new { x.SharedClientsId, x.SharedUsersId });
                    table.ForeignKey(
                        name: "FK_ClientUser_AspNetUsers_SharedUsersId",
                        column: x => x.SharedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientUser_Clients_SharedClientsId",
                        column: x => x.SharedClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_OwnerId",
                table: "Clients",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_SharedUsersId",
                table: "ClientUser",
                column: "SharedUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientUser");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
