using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ObsidianToVuePress.Migrations
{
    public partial class adddestPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "SrcDir",
                table: "Files",
                newName: "DestPath");

            migrationBuilder.RenameColumn(
                name: "DestDir",
                table: "Files",
                newName: "SrcPath");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "SrcPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "DestPath",
                table: "Files",
                newName: "SrcDir");

            migrationBuilder.RenameColumn(
                name: "SrcPath",
                table: "Files",
                newName: "DestDir");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Path");
        }
    }
}
