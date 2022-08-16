using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ObsidianToVuePress.Migrations
{
    public partial class updatefiledomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    SrcDir = table.Column<string>(type: "TEXT", nullable: false),
                    DestDir = table.Column<string>(type: "TEXT", nullable: false),
                    Sha256 = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Path);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
