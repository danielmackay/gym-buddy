using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.Users.Endpoints;

public record UpdateUserRequest(
    Guid Id,
    string Name,
    string Email);

public class UpdateUserEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdateUserRequest, UserResponse>
{
    public override void Configure()
    {
        Put("/{id}");
        Group<UsersGroup>();
        Description(x => x.WithName("UpdateUser")
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var userId = UserId.From(req.Id);
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        user.Name = req.Name;
        user.Email = req.Email;

        await dbContext.SaveChangesAsync(ct);

        var response = new UserResponse(
            user.Id.Value,
            user.Name,
            user.Email,
            user.Roles.ToList(),
            user.TrainerId.HasValue ? user.TrainerId.Value.Value : null,
            user.AssignedWorkoutPlanIds.Select(id => id.Value).ToList());

        await Send.OkAsync(response, ct);
    }
}

public class UpdateUserRequestValidator : Validator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(User.NameMaxLength)
            .WithMessage($"Name must be {User.NameMaxLength} characters or less");

        RuleFor(v => v.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(User.EmailMaxLength)
            .WithMessage($"Email must be {User.EmailMaxLength} characters or less");
    }
}

public class UpdateUserSummary : Summary<UpdateUserEndpoint>
{
    public UpdateUserSummary()
    {
        Summary = "Update user";
        Description = "Updates an existing user's name and email. Returns the updated user.";

        ExampleRequest = new UpdateUserRequest(
            Id: Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Name: "John Updated",
            Email: "john.updated@example.com");

        Response(200, "User updated successfully");
        Response(404, "User not found");
    }
}
