using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymBuddy.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_ClientId",
                table: "WorkoutSessions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutPlanId",
                table: "WorkoutSessions",
                column: "WorkoutPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlans_TrainerId",
                table: "WorkoutPlans",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionExercises_ExerciseId",
                table: "SessionExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedExercises_ExerciseId",
                table: "PlannedExercises",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedExercises_Exercises_ExerciseId",
                table: "PlannedExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercises_Exercises_ExerciseId",
                table: "SessionExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutPlans_Users_TrainerId",
                table: "WorkoutPlans",
                column: "TrainerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_Users_ClientId",
                table: "WorkoutSessions",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutSessions",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedExercises_Exercises_ExerciseId",
                table: "PlannedExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercises_Exercises_ExerciseId",
                table: "SessionExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutPlans_Users_TrainerId",
                table: "WorkoutPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_Users_ClientId",
                table: "WorkoutSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_ClientId",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_WorkoutPlanId",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutPlans_TrainerId",
                table: "WorkoutPlans");

            migrationBuilder.DropIndex(
                name: "IX_SessionExercises_ExerciseId",
                table: "SessionExercises");

            migrationBuilder.DropIndex(
                name: "IX_PlannedExercises_ExerciseId",
                table: "PlannedExercises");
        }
    }
}
