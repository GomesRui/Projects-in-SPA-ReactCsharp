using Microsoft.EntityFrameworkCore.Migrations;

namespace XCNTWebServer.Migrations
{
    public partial class setup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    UUID = table.Column<string>(nullable: false),
                    Last_Name = table.Column<string>(nullable: false),
                    First_Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.UUID);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    UUID = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Created_At = table.Column<string>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    EmployeeUUID = table.Column<string>(nullable: true),
                    Approved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.UUID);
                    table.ForeignKey(
                        name: "FK_Expenses_Employees_EmployeeUUID",
                        column: x => x.EmployeeUUID,
                        principalTable: "Employees",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_EmployeeUUID",
                table: "Expenses",
                column: "EmployeeUUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
