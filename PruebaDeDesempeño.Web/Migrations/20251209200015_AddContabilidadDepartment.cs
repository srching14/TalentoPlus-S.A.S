using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaDeDesempeño.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddContabilidadDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { 8, new DateTime(2025, 12, 9, 20, 0, 15, 187, DateTimeKind.Utc).AddTicks(1176), "Contabilidad y auditoría", true, "Contabilidad", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b7211449-e748-4b9b-a179-896dc8424de5", new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9308), "AQAAAAIAAYagAAAAEEccIG0cwi0mhLbPRJGP9vOPk+w0jF5XQGQWObL+ROp7HL5kdA7+Iwld6XIjl2vVxg==", "c346a0a6-f73e-4d75-8b40-5f1d403cb7bf" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9226));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9228));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9230));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9231));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9232));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9233));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 19, 52, 10, 897, DateTimeKind.Utc).AddTicks(9234));
        }
    }
}
