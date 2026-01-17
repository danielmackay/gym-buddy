using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.Users.Endpoints;

public record CreateClientRequest(
    string Name,
    string Email,
    Guid TrainerId);

public record CreateClientResponse(Guid Id);

public class CreateClientEndpoint(ApplicationDbContext dbContext)
    : Endpoint<CreateClientRequest, CreateClientResponse>
{
    public override void Configure()
    {
        Post("/clients");
        Group<UsersGroup>();
        Description(x => x.WithName("CreateClient")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(CreateClientRequest req, CancellationToken ct)
    {
        // Verify trainer exists
        var trainerId = UserId.From(req.TrainerId);
        var trainer = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == trainerId, ct);

        if (trainer is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var user = Domain.Users.User.Create(req.Name, req.Email);
        var addRoleResult = user.AddRole(UserRole.Client);

        if (addRoleResult.IsError)
        {
            addRoleResult.Errors.ForEach(e => AddError(e.Description, e.Code));
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var assignTrainerResult = user.AssignTrainer(trainerId);
        
        if (assignTrainerResult.IsError)
        {
            assignTrainerResult.Errors.ForEach(e => AddError(e.Description, e.Code));
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        await Send.OkAsync(new CreateClientResponse(user.Id.Value), ct);
    }
}

public class CreateClientRequestValidator : Validator<CreateClientRequest>
{
    public CreateClientRequestValidator()
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

        RuleFor(v => v.TrainerId)
            .NotEmpty()
            .WithMessage("TrainerId is required");
    }
}

public class CreateClientSummary : Summary<CreateClientEndpoint>
{
    public CreateClientSummary()
    {
        Summary = "Create a new client";
        Description = "Creates a new user with the Client role and assigns them to the specified trainer. Returns the ID of the created client.";

        // Request example
        ExampleRequest = new CreateClientRequest(
            Name: "Jane Smith",
            Email: "jane.smith@example.com",
            TrainerId: Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"));
    }
}
