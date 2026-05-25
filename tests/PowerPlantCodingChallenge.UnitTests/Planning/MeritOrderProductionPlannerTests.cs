using PowerPlantCodingChallenge.Application.Models;
using PowerPlantCodingChallenge.Application.Planning;

namespace PowerPlantCodingChallenge.UnitTests.Planning;

public sealed class MeritOrderProductionPlannerTests
{
	private readonly MeritOrderProductionPlanner _planner = new(new ProductionCostCalculator());

	[Fact]
	public void Calculate_WhenUsingPayload1_ShouldReturnExpectedProductionPlan()
	{
		var input = CreatePayload1Input();

		var result = _planner.Calculate(input).ToList();

		Assert.Collection(
			result,
			item =>
			{
				Assert.Equal("windpark1", item.Name);
				Assert.Equal(90.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("windpark2", item.Name);
				Assert.Equal(21.6m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredbig1", item.Name);
				Assert.Equal(368.4m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredbig2", item.Name);
				Assert.Equal(0.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredsomewhatsmaller", item.Name);
				Assert.Equal(0.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("tj1", item.Name);
				Assert.Equal(0.0m, item.Production);
			});
	}

	[Fact]
	public void Calculate_WhenUsingPayload2_ShouldReturnExpectedProductionPlan()
	{
		var input = CreatePayload2Input();

		var result = _planner.Calculate(input).ToList();

		Assert.Collection(
			result,
			item =>
			{
				Assert.Equal("windpark1", item.Name);
				Assert.Equal(0.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("windpark2", item.Name);
				Assert.Equal(0.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredbig1", item.Name);
				Assert.Equal(380.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredbig2", item.Name);
				Assert.Equal(100.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredsomewhatsmaller", item.Name);
				Assert.Equal(0.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("tj1", item.Name);
				Assert.Equal(0.0m, item.Production);
			});
	}

	[Fact]
	public void Calculate_WhenUsingPayload3_ShouldReturnExpectedProductionPlan()
	{
		var input = CreatePayload3Input();

		var result = _planner.Calculate(input).ToList();

		Assert.Collection(
			result,
			item =>
			{
				Assert.Equal("windpark1", item.Name);
				Assert.Equal(90.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("windpark2", item.Name);
				Assert.Equal(21.6m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredbig1", item.Name);
				Assert.Equal(460.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredbig2", item.Name);
				Assert.Equal(338.4m, item.Production);
			},
			item =>
			{
				Assert.Equal("gasfiredsomewhatsmaller", item.Name);
				Assert.Equal(0.0m, item.Production);
			},
			item =>
			{
				Assert.Equal("tj1", item.Name);
				Assert.Equal(0.0m, item.Production);
			});
	}

	[Fact]
	public void Calculate_ShouldReturnTotalProductionEqualToRequestedLoad()
	{
		var input = CreatePayload3Input();

		var result = _planner.Calculate(input);

		Assert.Equal(input.Load, result.Sum(item => item.Production));
	}

	[Fact]
	public void Calculate_ShouldRespectWindPercentage()
	{
		var input = CreatePayload3Input();

		var result = _planner.Calculate(input).ToDictionary(item => item.Name);

		Assert.Equal(90.0m, result["windpark1"].Production);
		Assert.Equal(21.6m, result["windpark2"].Production);
	}

	[Fact]
	public void Calculate_ShouldReturnValuesRoundedToOneDecimalPlace()
	{
		var input = CreatePayload3Input();

		var result = _planner.Calculate(input);

		Assert.All(result, item =>
		{
			var multiplied = item.Production * 10m;
			Assert.Equal(decimal.Truncate(multiplied), multiplied);
		});
	}

	private static ProductionPlanInput CreatePayload1Input()
	{
		return new ProductionPlanInput
		{
			Load = 480m,
			Fuels = new FuelInput
			{
				GasEuroPerMwh = 13.4m,
				KerosineEuroPerMwh = 50.8m,
				Co2EuroPerTon = 20m,
				WindPercentage = 60m
			},
			PowerPlants = CreateDefaultPowerPlants()
		};
	}

	private static ProductionPlanInput CreatePayload2Input()
	{
		return new ProductionPlanInput
		{
			Load = 480m,
			Fuels = new FuelInput
			{
				GasEuroPerMwh = 13.4m,
				KerosineEuroPerMwh = 50.8m,
				Co2EuroPerTon = 20m,
				WindPercentage = 0m
			},
			PowerPlants = CreateDefaultPowerPlants()
		};
	}

	private static ProductionPlanInput CreatePayload3Input()
	{
		return new ProductionPlanInput
		{
			Load = 910m,
			Fuels = new FuelInput
			{
				GasEuroPerMwh = 13.4m,
				KerosineEuroPerMwh = 50.8m,
				Co2EuroPerTon = 20m,
				WindPercentage = 60m
			},
			PowerPlants = CreateDefaultPowerPlants()
		};
	}

	private static IReadOnlyCollection<PowerPlantInput> CreateDefaultPowerPlants()
	{
		return
		[
			new PowerPlantInput
		{
			Name = "gasfiredbig1",
			Type = "gasfired",
			Efficiency = 0.53m,
			Pmin = 100m,
			Pmax = 460m
		},
		new PowerPlantInput
		{
			Name = "gasfiredbig2",
			Type = "gasfired",
			Efficiency = 0.53m,
			Pmin = 100m,
			Pmax = 460m
		},
		new PowerPlantInput
		{
			Name = "gasfiredsomewhatsmaller",
			Type = "gasfired",
			Efficiency = 0.37m,
			Pmin = 40m,
			Pmax = 210m
		},
		new PowerPlantInput
		{
			Name = "tj1",
			Type = "turbojet",
			Efficiency = 0.3m,
			Pmin = 0m,
			Pmax = 16m
		},
		new PowerPlantInput
		{
			Name = "windpark1",
			Type = "windturbine",
			Efficiency = 1m,
			Pmin = 0m,
			Pmax = 150m
		},
		new PowerPlantInput
		{
			Name = "windpark2",
			Type = "windturbine",
			Efficiency = 1m,
			Pmin = 0m,
			Pmax = 36m
		}
		];
	}
}