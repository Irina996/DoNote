using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNotes.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTaskTableCreationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "TaskItems",
                newName: "CreationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "TaskItems",
                newName: "CreatedDate");
        }
    }
}
