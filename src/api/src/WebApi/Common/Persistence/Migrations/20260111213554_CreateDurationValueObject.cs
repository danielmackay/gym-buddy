using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymBuddy.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDurationValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetDurationSeconds",
                table: "SessionExercises",
                newName: "TargetDuration");

            migrationBuilder.RenameColumn(
                name: "ActualDurationSeconds",
                table: "SessionExercises",
                newName: "ActualDuration");

            migrationBuilder.RenameColumn(
                name: "DurationSeconds",
                table: "PlannedExercises",
                newName: "Duration");

            migrationBuilder.AlterColumn<int>(
                name: "TargetWeightUnit",
                table: "SessionExercises",
                type: "int",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ActualWeightUnit",
                table: "SessionExercises",
                type: "int",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "WeightUnit",
                table: "PlannedExercises",
                type: "int",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetDuration",
                table: "SessionExercises",
                newName: "TargetDurationSeconds");

            migrationBuilder.RenameColumn(
                name: "ActualDuration",
                table: "SessionExercises",
                newName: "ActualDurationSeconds");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "PlannedExercises",
                newName: "DurationSeconds");

            migrationBuilder.AlterColumn<int>(
                name: "TargetWeightUnit",
                table: "SessionExercises",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "ActualWeightUnit",
                table: "SessionExercises",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "WeightUnit",
                table: "PlannedExercises",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 1);
        }
    }
}