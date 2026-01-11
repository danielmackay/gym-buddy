namespace GymBuddy.Domain.Exercises;

public sealed class ExerciseByIdSpec : SingleResultSpecification<Exercise>
{
    public ExerciseByIdSpec(ExerciseId exerciseId)
    {
        Query.Where(e => e.Id == exerciseId);
    }
}
