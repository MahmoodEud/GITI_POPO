using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITI_GProject.Migrations
{
    /// <inheritdoc />
    public partial class uo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Lessons_LessonId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentResponses_Choices_ChoiceId",
                table: "StudentResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentResponses_StudentAttempts_AttemptId",
                table: "StudentResponses");

            migrationBuilder.CreateIndex(
                name: "IX_StudentResponses_AttemptId_QuestionId",
                table: "StudentResponses",
                columns: new[] { "AttemptId", "QuestionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Lessons_LessonId",
                table: "Assessments",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResponses_Choices_ChoiceId",
                table: "StudentResponses",
                column: "ChoiceId",
                principalTable: "Choices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResponses_StudentAttempts_AttemptId",
                table: "StudentResponses",
                column: "AttemptId",
                principalTable: "StudentAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Lessons_LessonId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentResponses_Choices_ChoiceId",
                table: "StudentResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentResponses_StudentAttempts_AttemptId",
                table: "StudentResponses");

            migrationBuilder.DropIndex(
                name: "IX_StudentResponses_AttemptId_QuestionId",
                table: "StudentResponses");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Lessons_LessonId",
                table: "Assessments",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResponses_Choices_ChoiceId",
                table: "StudentResponses",
                column: "ChoiceId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResponses_StudentAttempts_AttemptId",
                table: "StudentResponses",
                column: "AttemptId",
                principalTable: "StudentAttempts",
                principalColumn: "Id");
        }
    }
}
