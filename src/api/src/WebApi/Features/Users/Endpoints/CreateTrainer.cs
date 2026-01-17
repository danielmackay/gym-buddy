using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.Users.Endpoints;

public record CreateTrainerRequest(
    string Name,
    string Email);

public record CreateTrainerResponse(Guid Id);

public class CreateTrainerEndpoint(ApplicationDbContext dbContext)
    : Endpoint<CreateTrainerRequest, CreateTrainerResponse>
{
    public override void Configure()
    {
        Post("/trainers");
        Group<UsersGroup>();
        Description(x => x.WithName("CreateTrainer")
            .Produces(StatusCodes.Status201Created));
    }

    public override async Task HandleAsync(CreateTrainerRequest req, CancellationToken ct)
    {
        var user = Domain.Users.User.Create(req.Name, req.Email);
        var addRoleResult = user.AddRole(UserRole.Trainer);

        if (addRoleResult.IsError)
        {
            addRoleResult.Errors.ForEach(e => AddError(e.Description, e.Code));
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        await Send.OkAsync(new CreateTrainerResponse(user.Id.Value), ct);
    }
}

public class CreateTrainerRequestValidator : Validator<CreateTrainerRequest>
{
    public CreateTrainerRequestValidator()
    {
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

public class CreateTrainerSummary : Summary<CreateTrainerEndpoint>
{
    public CreateTrainerSummary()
    {
        Summary = "Create a new trainer";
        Description = "Creates a new user with the Trainer role. Returns the ID of the created trainer.";

        // Request example
        ExampleRequest = new CreateTrainerRequest(
            Name: "John Doe",
            Email: "john.doe@example.com");
    }
}
