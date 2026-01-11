using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymBuddy.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightUnitSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActualWeightUnit",
                table: "SessionExercises",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetWeightUnit",
                table: "SessionExercises",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeightUnit",
                table: "PlannedExercises",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualWeightUnit",
                table: "SessionExercises");

            migrationBuilder.DropColumn(
                name: "TargetWeightUnit",
                table: "SessionExercises");

            migrationBuilder.DropColumn(
                name: "WeightUnit",
                table: "PlannedExercises");
        }
    }
}
