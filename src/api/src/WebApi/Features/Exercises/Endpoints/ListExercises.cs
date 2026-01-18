using GymBuddy.Domain.Exercises;
using Microsoft.EntityFrameworkCore;

namespace GymBuddy.Api.Features.Exercises.Endpoints;

public record ListExercisesRequest(
    MuscleGroup? MuscleGroup = null,
    ExerciseType? Type = null);

public record ExerciseResponse(
    Guid Id,
    string Name,
    string? Description,
    ExerciseType Type,
    List<MuscleGroup> MuscleGroups);

public class ListExercisesEndpoint(ApplicationDbContext dbContext)
    : Endpoint<ListExercisesRequest, List<ExerciseResponse>>
{
    public override void Configure()
    {
        Get("");
        Group<ExercisesGroup>();
        Description(x => x.WithName("ListExercises")
            .Produces(StatusCodes.Status200OK));
        AllowAnonymous();
    }

    public override async Task HandleAsync(ListExercisesRequest req, CancellationToken ct)
    {
        var query = dbContext.Exercises.AsQueryable();

        // Filter by muscle group if specified
        if (req.MuscleGroup.HasValue)
        {
            // Load to memory first to filter the JSON column
            var allExercises = await query.ToListAsync(ct);
            var filtered = allExercises
                .Where(e => e.MuscleGroups.Contains(req.MuscleGroup.Value))
                .ToList();

            // Filter by type if specified
            if (req.Type.HasValue)
            {
                filtered = filtered.Where(e => e.Type == req.Type.Value).ToList();
            }

            var response = filtered.Select(e => new ExerciseResponse(
                e.Id.Value,
                e.Name,
                e.Description,
                e.Type,
                e.MuscleGroups.ToList()
            )).ToList();

            await Send.OkAsync(response, ct);
            return;
        }

        // Filter by type only if muscle group not specified
        if (req.Type.HasValue)
        {
            query = query.Where(e => e.Type == req.Type.Value);
        }

        var exercises = await query.ToListAsync(ct);

        var responses = exercises.Select(e => new ExerciseResponse(
            e.Id.Value,
            e.Name,
            e.Description,
            e.Type,
            e.MuscleGroups.ToList()
        )).ToList();

        await Send.OkAsync(responses, ct);
    }
}

public class ListExercisesSummary : Summary<ListExercisesEndpoint>
{
    public ListExercisesSummary()
    {
        Summary = "List all exercises";
        Description = "Returns all exercises, optionally filtered by muscle group and/or type.";
    }
}
