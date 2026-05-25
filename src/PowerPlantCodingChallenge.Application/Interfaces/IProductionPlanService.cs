using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Interfaces
{
	public interface IProductionPlanService
	{
		IReadOnlyCollection<ProductionPlanOutput> CalculateProductionPlan(ProductionPlanInput input);
	}
}
