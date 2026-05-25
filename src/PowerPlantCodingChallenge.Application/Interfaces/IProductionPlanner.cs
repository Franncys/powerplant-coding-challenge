using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Interfaces;

/// <summary>
/// Represents a production planning strategy.
/// This abstraction allows the algorithm to be replaced without changing the application service or API layer.
/// </summary>
public interface IProductionPlanner
{
	/// <summary>
	/// Calculates a production plan using a specific planning strategy.
	/// </summary>
	/// <param name="input">The production plan input.</param>
	/// <returns>The calculated production plan.</returns>
	IReadOnlyCollection<ProductionPlanOutput> Calculate(ProductionPlanInput input);
}