using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITI_GProject.Migrations
{
    /// <inheritdoc />
    public partial class studentAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgress_Lessons_LessonId",
                table: "StudentLessonProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgress_Students_StudentId",
                table: "StudentLessonProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLessonProgress",
                table: "StudentLessonProgress");

            migrationBuilder.DropColumn(
                name: "AssessmentId",
                table: "StudentResponses");

            migrationBuilder.RenameTable(
                name: "StudentLessonProgress",
                newName: "StudentLessonProgresses");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLessonProgress_StudentId_LessonId",
                table: "StudentLessonProgresses",
                newName: "IX_StudentLessonProgresses_StudentId_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLessonProgress_LessonId",
                table: "StudentLessonProgresses",
                newName: "IX_StudentLessonProgresses_LessonId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "StudentAttempts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "AttemptsNumber",
                table: "StudentAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "StudentAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLessonProgresses",
                table: "StudentLessonProgresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Lessons_LessonId",
                table: "StudentLessonProgresses",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Students_StudentId",
                table: "StudentLessonProgresses",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Lessons_LessonId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Students_StudentId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLessonProgresses",
                table: "StudentLessonProgresses");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "StudentAttempts");

            migrationBuilder.RenameTable(
                name: "StudentLessonProgresses",
                newName: "StudentLessonProgress");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLessonProgresses_StudentId_LessonId",
                table: "StudentLessonProgress",
                newName: "IX_StudentLessonProgress_StudentId_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLessonProgresses_LessonId",
                table: "StudentLessonProgress",
                newName: "IX_StudentLessonProgress_LessonId");

            migrationBuilder.AddColumn<int>(
                name: "AssessmentId",
                table: "StudentResponses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "StudentAttempts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AttemptsNumber",
                table: "StudentAttempts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLessonProgress",
                table: "StudentLessonProgress",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgress_Lessons_LessonId",
                table: "StudentLessonProgress",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgress_Students_StudentId",
                table: "StudentLessonProgress",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
