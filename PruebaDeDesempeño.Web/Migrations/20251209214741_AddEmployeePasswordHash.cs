using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaDeDesempeño.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c1039599-81cb-4cb8-8627-f4e759ce7c36", new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4222), "AQAAAAIAAYagAAAAEE4yJIOJus0FQ/8IbVy++L7lkJw+7y2A/p/fJtazn9XRqoj3Ap4lwfe395g3rsOX5w==", "0e4c9993-1ac1-4119-9d2b-6a540f16dd01" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4138));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4140));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4141));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4142));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4143));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4144));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4145));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 21, 47, 40, 729, DateTimeKind.Utc).AddTicks(4146));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3efbab92-34f8-44a9-a6b7-2ffafc92bf41", new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1269), "AQAAAAIAAYagAAAAEOVx2j7CRdXHWackx/Njn8EauU2hm7ku/oDP/dvRiME2wlxjKr5lkhOO5OqBH36v+Q==", "b0482747-9f05-4ca6-ba88-1cd4950bc0e0" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1167));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1169));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1170));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1171));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1172));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1173));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1174));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1176));
        }
    }
}
