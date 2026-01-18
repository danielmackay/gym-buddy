using GymBuddy.Domain.Exercises;

namespace GymBuddy.Api.Features.Exercises.Endpoints;

public record GetExerciseRequest(Guid Id);

public class GetExerciseEndpoint(ApplicationDbContext dbContext)
    : Endpoint<GetExerciseRequest, ExerciseResponse>
{
    public override void Configure()
    {
        Get("/{id:guid}");
        Group<ExercisesGroup>();
        Description(x => x.WithName("GetExercise")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound));
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetExerciseRequest req, CancellationToken ct)
    {
        var exerciseId = ExerciseId.From(req.Id);
        var exercise = await dbContext.Exercises.FindAsync([exerciseId], ct);

        if (exercise is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new ExerciseResponse(
            exercise.Id.Value,
            exercise.Name,
            exercise.Description,
            exercise.Type,
            exercise.MuscleGroups.ToList());

        await Send.OkAsync(response, ct);
    }
}

public class GetExerciseSummary : Summary<GetExerciseEndpoint>
{
    public GetExerciseSummary()
    {
        Summary = "Get exercise by ID";
        Description = "Returns a single exercise by its ID.";
    }
}
