using GymBuddy.Domain.Exercises;

namespace GymBuddy.Api.Features.Exercises.Endpoints;

public record CreateExerciseRequest(
    string Name,
    ExerciseType Type,
    List<MuscleGroup> MuscleGroups,
    string? Description = null);

public record CreateExerciseResponse(Guid Id);

public class CreateExerciseEndpoint(ApplicationDbContext dbContext)
    : Endpoint<CreateExerciseRequest, CreateExerciseResponse>
{
    public override void Configure()
    {
        Post("");
        Group<ExercisesGroup>();
        Description(x => x.WithName("CreateExercise")
            .Produces(StatusCodes.Status201Created));
    }

    public override async Task HandleAsync(CreateExerciseRequest req, CancellationToken ct)
    {
        var exerciseResult = Exercise.Create(
            req.Name,
            req.Type,
            req.MuscleGroups,
            req.Description);

        if (exerciseResult.IsError)
        {
            exerciseResult.Errors.ForEach(e => AddError(e.Description, e.Code));
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var exercise = exerciseResult.Value;
        dbContext.Exercises.Add(exercise);
        await dbContext.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<GetExerciseEndpoint>(
            new { id = exercise.Id.Value },
            new CreateExerciseResponse(exercise.Id.Value),
            cancellation: ct);
    }
}

public class CreateExerciseRequestValidator : Validator<CreateExerciseRequest>
{
    public CreateExerciseRequestValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(Exercise.NameMaxLength)
            .WithMessage($"Name must be {Exercise.NameMaxLength} characters or less");

        RuleFor(v => v.Type)
            .IsInEnum()
            .WithMessage("Type must be a valid ExerciseType");

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

public class CreateExerciseSummary : Summary<CreateExerciseEndpoint>
{
    public CreateExerciseSummary()
    {
        Summary = "Create a new exercise";
        Description = "Creates a new exercise in the exercise library. Returns the ID of the created exercise.";

        // Request example
        ExampleRequest = new CreateExerciseRequest(
            Name: "Bench Press",
            Type: ExerciseType.RepsAndWeight,
            MuscleGroups: [MuscleGroup.Chest, MuscleGroup.Triceps],
            Description: "A compound exercise targeting chest and triceps");
    }
}
