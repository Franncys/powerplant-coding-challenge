using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Interfaces;

/// <summary>
/// Defines the application use case responsible for calculating a production plan.
/// The API layer depends on this abstraction instead of directly depending on the planning algorithm.
/// </summary>
public interface IProductionPlanService
{
	/// <summary>
	/// Calculates how much power each plant should produce to satisfy the requested load.
	/// </summary>
	/// <param name="input">The production plan input containing load, fuel prices and power plants.</param>
	/// <returns>The calculated production plan.</returns>
	IReadOnlyCollection<ProductionPlanOutput> CalculateProductionPlan(ProductionPlanInput input);
}