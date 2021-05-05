using Microsoft.EntityFrameworkCore.Migrations;

namespace FarmHub.Data.Migrations
{
    public partial class US153_ChangedCustomerGenderToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            UPDATE dbo.Customers
            SET Gender =
                CASE Gender
                    WHEN 'Male' THEN 1
                    WHEN 'Female' THEN 2
                    ELSE 3
                END
            ");
            
            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Customers",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            UPDATE dbo.Customers
            SET Gender =
                CASE Gender
                    WHEN 1 THEN 'Male'
                    WHEN 2 THEN 'Female'
                    ELSE 'Others'
                END
            ");
            
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}