using Microsoft.EntityFrameworkCore.Migrations;

namespace FarmHub.Data.Migrations
{
    public partial class US105_ProductImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OtherProductsId",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherProductsId",
                table: "Images");
        }
    }
}
