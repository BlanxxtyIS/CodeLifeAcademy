using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeLifeAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Topics",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Lessions",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Courses",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Lessions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Courses");
        }
    }
}
