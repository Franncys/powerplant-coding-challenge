using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Interfaces;

/// <summary>
/// Calculates the production cost of a power plant.
/// Keeping cost calculation isolated makes the planner easier to read and easier to test.
/// </summary>
public interface IProductionCostCalculator
{
	/// <summary>
	/// Calculates the cost of producing one MWh for the given power plant.
	/// </summary>
	/// <param name="powerPlant">The power plant for which the cost is calculated.</param>
	/// <param name="fuels">The fuel prices and wind percentage from the request.</param>
	/// <returns>The cost per produced MWh.</returns>
	decimal CalculateCostPerMwh(PowerPlantInput powerPlant, FuelInput fuels);
}