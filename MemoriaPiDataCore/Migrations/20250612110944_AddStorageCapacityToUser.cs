using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoriaPiDataCore.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageCapacityToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "letzte_aktivitaet",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "StorageCapacityGB",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageCapacityGB",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "letzte_aktivitaet",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }
    }
}
