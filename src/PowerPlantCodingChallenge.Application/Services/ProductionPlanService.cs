using PowerPlantCodingChallenge.Application.Interfaces;
using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Services;

/// <summary>
/// Application service responsible for coordinating the production plan use case.
/// 
/// This class intentionally does not contain the planning algorithm.
/// Its responsibility is to expose a clear use case to the API layer and delegate
/// the calculation to an injected planning strategy.
/// </summary>
public sealed class ProductionPlanService : IProductionPlanService
{
	private readonly IProductionPlanner _productionPlanner;

	public ProductionPlanService(IProductionPlanner productionPlanner)
	{
		_productionPlanner = productionPlanner;
	}

	/// <summary>
	/// Calculates the production plan by delegating the actual algorithm to IProductionPlanner.
	/// This keeps the service independent from the concrete implementation of the planner.
	/// </summary>
	public IReadOnlyCollection<ProductionPlanOutput> CalculateProductionPlan(ProductionPlanInput input)
	{
		ArgumentNullException.ThrowIfNull(input);

		return _productionPlanner.Calculate(input);
	}
}