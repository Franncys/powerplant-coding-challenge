using PowerPlantCodingChallenge.Application.Interfaces;
using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Planning;

/// <summary>
/// Production planner based on the merit-order principle.
/// 
/// The merit-order approach dispatches the cheapest available power plants first,
/// while respecting the production constraints of each plant.
/// </summary>
public sealed class MeritOrderProductionPlanner : IProductionPlanner
{
	// The challenge requires production values to be multiples of 0.1 MW.
	private const decimal ProductionStep = 0.1m;

	private readonly IProductionCostCalculator _costCalculator;

	public MeritOrderProductionPlanner(IProductionCostCalculator costCalculator)
	{
		_costCalculator = costCalculator;
	}

	/// <summary>
	/// Calculates the production plan for the requested load.
	/// </summary>
	public IReadOnlyCollection<ProductionPlanOutput> Calculate(ProductionPlanInput input)
	{
		ArgumentNullException.ThrowIfNull(input);

		ValidateInput(input);

		var candidates = BuildMeritOrder(input);

		var productionByPlantName = InitializeProduction(input);

		DispatchProduction(input.Load, candidates, productionByPlantName);

		ValidateTotalProduction(input.Load, productionByPlantName);

		return BuildOutput(candidates, productionByPlantName);
	}

	/// <summary>
	/// Creates dispatch candidates and sorts them by production cost.
	/// This is the core of the merit-order strategy.
	/// </summary>
	private IReadOnlyCollection<DispatchCandidate> BuildMeritOrder(ProductionPlanInput input)
	{
		return input.PowerPlants
			.Select((powerPlant, index) => CreateDispatchCandidate(powerPlant, input.Fuels, index))
			.OrderBy(candidate => candidate.CostPerMwh)
			.ThenBy(candidate => candidate.OriginalIndex)
			.ToList();
	}

	/// <summary>
	/// Converts a power plant from the request into a dispatch candidate used by the algorithm.
	/// </summary>
	private DispatchCandidate CreateDispatchCandidate(
		PowerPlantInput powerPlant,
		FuelInput fuels,
		int originalIndex)
	{
		return new DispatchCandidate
		{
			Name = powerPlant.Name,
			Pmin = powerPlant.Pmin,
			AvailablePmax = CalculateAvailablePmax(powerPlant, fuels),
			CostPerMwh = _costCalculator.CalculateCostPerMwh(powerPlant, fuels),
			OriginalIndex = originalIndex
		};
	}

	/// <summary>
	/// Initializes all plants with 0 MW production.
	/// This guarantees that unused plants are still returned in the response.
	/// </summary>
	private static Dictionary<string, decimal> InitializeProduction(ProductionPlanInput input)
	{
		// Dictionary lookup gives O(1) updates during dispatch.
		return input.PowerPlants.ToDictionary(
			powerPlant => powerPlant.Name,
			_ => 0m);
	}

	/// <summary>
	/// Allocates production to each plant in merit order until the requested load is reached.
	/// </summary>
	private static void DispatchProduction(
		decimal requestedLoad,
		IReadOnlyCollection<DispatchCandidate> candidates,
		Dictionary<string, decimal> productionByPlantName)
	{
		var remainingLoad = RoundToStep(requestedLoad);

		foreach (var candidate in candidates)
		{
			if (remainingLoad <= 0)
			{
				break;
			}

			var production = CalculateProductionForCandidate(candidate, remainingLoad);

			// A plant cannot be switched on below its minimum production level.
			if (production <= 0 || production < candidate.Pmin)
			{
				continue;
			}

			productionByPlantName[candidate.Name] = production;
			remainingLoad = RoundToStep(remainingLoad - production);
		}

		// If a small amount remains due to rounding or capacity constraints,
		// try to add it to one already producing plant.
		if (remainingLoad != 0)
		{
			AdjustLastProducingPlant(candidates, productionByPlantName, remainingLoad);
		}
	}

	/// <summary>
	/// Calculates how much the current candidate can produce without exceeding its available Pmax
	/// or the remaining requested load.
	/// </summary>
	private static decimal CalculateProductionForCandidate(
		DispatchCandidate candidate,
		decimal remainingLoad)
	{
		var production = Math.Min(candidate.AvailablePmax, remainingLoad);

		return RoundDownToStep(production);
	}

	/// <summary>
	/// Calculates the maximum available production for a plant.
	/// Wind turbines are limited by the wind percentage from the input payload.
	/// </summary>
	private static decimal CalculateAvailablePmax(PowerPlantInput powerPlant, FuelInput fuels)
	{
		var availablePmax = powerPlant.Type == "windturbine"
			? powerPlant.Pmax * fuels.WindPercentage / 100m
			: powerPlant.Pmax;

		return RoundDownToStep(availablePmax);
	}

	/// <summary>
	/// Adjusts an already producing plant to close the remaining production gap.
	/// The adjustment must stay within the plant's Pmin and Pmax constraints.
	/// </summary>
	private static void AdjustLastProducingPlant(
		IReadOnlyCollection<DispatchCandidate> candidates,
		Dictionary<string, decimal> productionByPlantName,
		decimal remainingLoad)
	{
		// Reverse merit order is used so the cheapest plants remain as fully used as possible.
		var adjustablePlant = candidates
			.Where(candidate => productionByPlantName[candidate.Name] > 0)
			.Reverse()
			.FirstOrDefault(candidate =>
			{
				var adjustedProduction = RoundToStep(
					productionByPlantName[candidate.Name] + remainingLoad);

				return adjustedProduction >= candidate.Pmin &&
					   adjustedProduction <= candidate.AvailablePmax;
			});

		if (adjustablePlant is null)
		{
			throw new InvalidOperationException(
				"Unable to match the requested load with the available power plants.");
		}

		productionByPlantName[adjustablePlant.Name] = RoundToStep(
			productionByPlantName[adjustablePlant.Name] + remainingLoad);
	}

	/// <summary>
	/// Builds the API output in merit-order order, matching the expected response style from the challenge.
	/// </summary>
	private static IReadOnlyCollection<ProductionPlanOutput> BuildOutput(
		IReadOnlyCollection<DispatchCandidate> candidates,
		IReadOnlyDictionary<string, decimal> productionByPlantName)
	{
		return candidates
			.Select(candidate => new ProductionPlanOutput
			{
				Name = candidate.Name,
				Production = productionByPlantName[candidate.Name]
			})
			.ToList();
	}

	/// <summary>
	/// Validates high-level input constraints before starting the calculation.
	/// </summary>
	private static void ValidateInput(ProductionPlanInput input)
	{
		if (input.Load < 0)
		{
			throw new ArgumentException("Load cannot be negative.", nameof(input));
		}

		if (input.Fuels.WindPercentage is < 0 or > 100)
		{
			throw new ArgumentException("Wind percentage must be between 0 and 100.", nameof(input));
		}

		if (input.PowerPlants.Count == 0)
		{
			throw new ArgumentException("At least one power plant is required.", nameof(input));
		}
	}

	/// <summary>
	/// Ensures the calculated production exactly matches the requested load after rounding.
	/// </summary>
	private static void ValidateTotalProduction(
		decimal expectedLoad,
		IReadOnlyDictionary<string, decimal> productionByPlantName)
	{
		var totalProduction = RoundToStep(productionByPlantName.Values.Sum());

		if (totalProduction != RoundToStep(expectedLoad))
		{
			throw new InvalidOperationException(
				$"Total production '{totalProduction}' does not match requested load '{expectedLoad}'.");
		}
	}

	/// <summary>
	/// Rounds a value to the nearest valid production step.
	/// </summary>
	private static decimal RoundToStep(decimal value)
	{
		return Math.Round(value / ProductionStep, MidpointRounding.AwayFromZero) * ProductionStep;
	}

	/// <summary>
	/// Rounds a value down to the nearest valid production step.
	/// This prevents assigning more production than the available capacity.
	/// </summary>
	private static decimal RoundDownToStep(decimal value)
	{
		return Math.Floor(value / ProductionStep) * ProductionStep;
	}
}