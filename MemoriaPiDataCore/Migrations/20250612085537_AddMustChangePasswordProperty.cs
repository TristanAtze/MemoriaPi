using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoriaPiDataCore.Migrations
{
    /// <inheritdoc />
    public partial class AddMustChangePasswordProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "letzte_aktivitaet",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "letzte_aktivitaet",
                table: "AspNetUsers");
        }
    }
}
