using PowerPlantCodingChallenge.Application.Interfaces;
using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Planning;

/// <summary>
/// Calculates the production cost per MWh for each type of power plant.
/// 
/// This class is separated from the planner to respect the Single Responsibility Principle:
/// the planner decides the dispatch order, while this class decides how expensive each plant is.
/// </summary>
public sealed class ProductionCostCalculator : IProductionCostCalculator
{
	// Bonus requirement: gas-fired power plants emit 0.3 tons of CO2 per produced MWh.
	private const decimal GasCo2EmissionTonPerMwh = 0.3m;

	/// <summary>
	/// Calculates the production cost per MWh according to the power plant type.
	/// Wind has no fuel cost, gas uses gas price and CO2 cost, and turbojet uses kerosine price.
	/// </summary>
	public decimal CalculateCostPerMwh(PowerPlantInput powerPlant, FuelInput fuels)
	{
		ArgumentNullException.ThrowIfNull(powerPlant);
		ArgumentNullException.ThrowIfNull(fuels);

		return powerPlant.Type switch
		{
			"windturbine" => 0m,

			"gasfired" => CalculateGasFiredCost(powerPlant, fuels),

			"turbojet" => CalculateTurbojetCost(powerPlant, fuels),

			_ => throw new ArgumentException($"Unsupported power plant type '{powerPlant.Type}'.")
		};
	}

	/// <summary>
	/// Calculates gas-fired production cost.
	/// 
	/// The fuel cost is divided by the plant efficiency because a less efficient plant
	/// needs more fuel to produce the same amount of electricity.
	/// 
	/// The CO2 cost is added as a bonus feature described in the challenge.
	/// </summary>
	private static decimal CalculateGasFiredCost(PowerPlantInput powerPlant, FuelInput fuels)
	{
		ValidateEfficiency(powerPlant);

		var fuelCost = fuels.GasEuroPerMwh / powerPlant.Efficiency;
		var co2Cost = fuels.Co2EuroPerTon * GasCo2EmissionTonPerMwh;

		return fuelCost + co2Cost;
	}

	/// <summary>
	/// Calculates turbojet production cost using the kerosine price and plant efficiency.
	/// </summary>
	private static decimal CalculateTurbojetCost(PowerPlantInput powerPlant, FuelInput fuels)
	{
		ValidateEfficiency(powerPlant);

		return fuels.KerosineEuroPerMwh / powerPlant.Efficiency;
	}

	/// <summary>
	/// Ensures the efficiency is valid before using it as a divisor.
	/// </summary>
	private static void ValidateEfficiency(PowerPlantInput powerPlant)
	{
		if (powerPlant.Efficiency <= 0)
		{
			throw new ArgumentException($"Power plant '{powerPlant.Name}' has invalid efficiency.");
		}
	}
}