using PowerPlantCodingChallenge.Application.Models;
using PowerPlantCodingChallenge.Application.Planning;

namespace PowerPlantCodingChallenge.UnitTests.Planning;

public sealed class ProductionCostCalculatorTests
{
	private readonly ProductionCostCalculator _calculator = new();

	[Fact]
	public void CalculateCostPerMwh_WhenPowerPlantIsWindTurbine_ShouldReturnZero()
	{
		var powerPlant = new PowerPlantInput
		{
			Name = "windpark1",
			Type = "windturbine",
			Efficiency = 1m,
			Pmin = 0m,
			Pmax = 150m
		};

		var fuels = new FuelInput
		{
			GasEuroPerMwh = 13.4m,
			KerosineEuroPerMwh = 50.8m,
			Co2EuroPerTon = 20m,
			WindPercentage = 60m
		};

		var result = _calculator.CalculateCostPerMwh(powerPlant, fuels);

		Assert.Equal(0m, result);
	}

	[Fact]
	public void CalculateCostPerMwh_WhenPowerPlantIsGasFired_ShouldIncludeFuelCostAndCo2Cost()
	{
		var powerPlant = new PowerPlantInput
		{
			Name = "gasfiredbig1",
			Type = "gasfired",
			Efficiency = 0.53m,
			Pmin = 100m,
			Pmax = 460m
		};

		var fuels = new FuelInput
		{
			GasEuroPerMwh = 13.4m,
			KerosineEuroPerMwh = 50.8m,
			Co2EuroPerTon = 20m,
			WindPercentage = 60m
		};

		var result = _calculator.CalculateCostPerMwh(powerPlant, fuels);

		var expected = 13.4m / 0.53m + 20m * 0.3m;

		Assert.Equal(expected, result);
	}

	[Fact]
	public void CalculateCostPerMwh_WhenPowerPlantIsTurbojet_ShouldUseKerosineCost()
	{
		var powerPlant = new PowerPlantInput
		{
			Name = "tj1",
			Type = "turbojet",
			Efficiency = 0.3m,
			Pmin = 0m,
			Pmax = 16m
		};

		var fuels = new FuelInput
		{
			GasEuroPerMwh = 13.4m,
			KerosineEuroPerMwh = 50.8m,
			Co2EuroPerTon = 20m,
			WindPercentage = 60m
		};

		var result = _calculator.CalculateCostPerMwh(powerPlant, fuels);

		var expected = 50.8m / 0.3m;

		Assert.Equal(expected, result);
	}

	[Fact]
	public void CalculateCostPerMwh_WhenEfficiencyIsInvalid_ShouldThrowArgumentException()
	{
		var powerPlant = new PowerPlantInput
		{
			Name = "invalid-gas-plant",
			Type = "gasfired",
			Efficiency = 0m,
			Pmin = 100m,
			Pmax = 460m
		};

		var fuels = new FuelInput
		{
			GasEuroPerMwh = 13.4m,
			KerosineEuroPerMwh = 50.8m,
			Co2EuroPerTon = 20m,
			WindPercentage = 60m
		};

		Assert.Throws<ArgumentException>(() => _calculator.CalculateCostPerMwh(powerPlant, fuels));
	}
}