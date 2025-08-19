using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITI_GProject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBoolExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExam",
                table: "Assessments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExam",
                table: "Assessments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
