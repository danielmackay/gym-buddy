namespace GymBuddy.Domain.WorkoutPlans;

public sealed class WorkoutPlanByIdSpec : SingleResultSpecification<WorkoutPlan>
{
    public WorkoutPlanByIdSpec(WorkoutPlanId workoutPlanId)
    {
        Query.Where(wp => wp.Id == workoutPlanId)
            .Include(wp => wp.Exercises);
    }
}
