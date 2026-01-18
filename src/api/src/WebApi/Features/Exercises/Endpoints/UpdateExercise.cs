using GymBuddy.Domain.Exercises;

namespace GymBuddy.Api.Features.Exercises.Endpoints;

public record UpdateExerciseRequest(
    Guid Id,
    string Name,
    List<MuscleGroup> MuscleGroups,
    string? Description = null);

public class UpdateExerciseEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdateExerciseRequest>
{
    public override void Configure()
    {
        Put("/{id:guid}");
        Group<ExercisesGroup>();
        Description(x => x.WithName("UpdateExercise")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(UpdateExerciseRequest req, CancellationToken ct)
    {
        var exerciseId = ExerciseId.From(req.Id);
        var exercise = await dbContext.Exercises.FindAsync([exerciseId], ct);

        if (exercise is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var updateResult = exercise.Update(
            req.Name,
            req.Description,
            req.MuscleGroups);

        if (updateResult.IsError)
        {
            updateResult.Errors.ForEach(e => AddError(e.Description, e.Code));
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        await dbContext.SaveChangesAsync(ct);

        await Send.OkAsync(ct);
    }
}

public class UpdateExerciseRequestValidator : Validator<UpdateExerciseRequest>
{
    public UpdateExerciseRequestValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(Exercise.NameMaxLength)
            .WithMessage($"Name must be {Exercise.NameMaxLength} characters or less");

        RuleFor(v => v.MuscleGroups)
            .NotEmpty()
            .WithMessage("At least one muscle group is required");

        RuleForEach(v => v.MuscleGroups)
            .IsInEnum()
            .WithMessage("All muscle groups must be valid MuscleGroup values");

        When(v => v.Description is not null, () =>
        {
            RuleFor(v => v.Description)
                .MaximumLength(Exercise.DescriptionMaxLength)
                .WithMessage($"Description must be {Exercise.DescriptionMaxLength} characters or less");
        });
    }
}

public class UpdateExerciseSummary : Summary<UpdateExerciseEndpoint>
{
    public UpdateExerciseSummary()
    {
        Summary = "Update an exercise";
        Description = "Updates an existing exercise in the exercise library.";

        // Request example
        ExampleRequest = new UpdateExerciseRequest(
            Id: Guid.NewGuid(),
            Name: "Barbell Bench Press",
            MuscleGroups: [MuscleGroup.Chest, MuscleGroup.Triceps, MuscleGroup.Shoulders],
            Description: "A compound chest exercise using a barbell");
    }
}
