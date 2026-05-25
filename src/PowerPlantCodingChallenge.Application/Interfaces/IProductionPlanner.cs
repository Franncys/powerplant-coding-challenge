using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Interfaces
{
	public interface IProductionPlanner
	{
		IReadOnlyCollection<ProductionPlanOutput> Calculate(ProductionPlanInput input);
	}
}
