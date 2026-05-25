using System.Net;
using System.Net.Http.Json;
using PowerPlantCodingChallenge.Api.Models.Requests;
using PowerPlantCodingChallenge.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PowerPlantCodingChallenge.FunctionalTests;

public sealed class ProductionPlanEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public ProductionPlanEndpointTests(WebApplicationFactory<Program> factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task PostProductionPlan_WhenPayload3IsProvided_ShouldReturnExpectedProductionPlan()
	{
		var request = CreatePayload3Request();

		var response = await _client.PostAsJsonAsync("/productionplan", request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<List<ProductionPlanResponse>>();

		Assert.NotNull(result);

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
	public async Task PostProductionPlan_WhenPayload1IsProvided_ShouldReturnTotalProductionEqualToLoad()
	{
		var request = CreatePayload1Request();

		var response = await _client.PostAsJsonAsync("/productionplan", request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<List<ProductionPlanResponse>>();

		Assert.NotNull(result);
		Assert.Equal(request.Load, result.Sum(item => item.Production));
	}

	[Fact]
	public async Task PostProductionPlan_WhenPayload2IsProvided_ShouldRespectPminRedistribution()
	{
		var request = CreatePayload2Request();

		var response = await _client.PostAsJsonAsync("/productionplan", request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<List<ProductionPlanResponse>>();

		Assert.NotNull(result);

		var gasfiredbig1 = result.Single(item => item.Name == "gasfiredbig1");
		var gasfiredbig2 = result.Single(item => item.Name == "gasfiredbig2");
		var turbojet = result.Single(item => item.Name == "tj1");

		Assert.Equal(380.0m, gasfiredbig1.Production);
		Assert.Equal(100.0m, gasfiredbig2.Production);
		Assert.Equal(0.0m, turbojet.Production);
		Assert.Equal(request.Load, result.Sum(item => item.Production));
	}

	//[Fact]
	//public async Task PostProductionPlan_WhenRequestHasNegativeLoad_ShouldReturnBadRequest()
	//{
	//	var request = CreatePayload3Request() with
	//	{
	//		Load = -1m
	//	};

	//	var response = await _client.PostAsJsonAsync("/productionplan", request);

	//	Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	//}

	private static ProductionPlanRequest CreatePayload1Request()
	{
		return new ProductionPlanRequest
		{
			Load = 480m,
			Fuels = new FuelsRequest
			{
				GasEuroPerMwh = 13.4m,
				KerosineEuroPerMwh = 50.8m,
				Co2EuroPerTon = 20m,
				WindPercentage = 60m
			},
			PowerPlants = CreateDefaultPowerPlants()
		};
	}

	private static ProductionPlanRequest CreatePayload2Request()
	{
		return new ProductionPlanRequest
		{
			Load = 480m,
			Fuels = new FuelsRequest
			{
				GasEuroPerMwh = 13.4m,
				KerosineEuroPerMwh = 50.8m,
				Co2EuroPerTon = 20m,
				WindPercentage = 0m
			},
			PowerPlants = CreateDefaultPowerPlants()
		};
	}

	private static ProductionPlanRequest CreatePayload3Request()
	{
		return new ProductionPlanRequest
		{
			Load = 910m,
			Fuels = new FuelsRequest
			{
				GasEuroPerMwh = 13.4m,
				KerosineEuroPerMwh = 50.8m,
				Co2EuroPerTon = 20m,
				WindPercentage = 60m
			},
			PowerPlants = CreateDefaultPowerPlants()
		};
	}

	private static IReadOnlyCollection<PowerPlantRequest> CreateDefaultPowerPlants()
	{
		return
		[
			new PowerPlantRequest
			{
				Name = "gasfiredbig1",
				Type = "gasfired",
				Efficiency = 0.53m,
				Pmin = 100m,
				Pmax = 460m
			},
			new PowerPlantRequest
			{
				Name = "gasfiredbig2",
				Type = "gasfired",
				Efficiency = 0.53m,
				Pmin = 100m,
				Pmax = 460m
			},
			new PowerPlantRequest
			{
				Name = "gasfiredsomewhatsmaller",
				Type = "gasfired",
				Efficiency = 0.37m,
				Pmin = 40m,
				Pmax = 210m
			},
			new PowerPlantRequest
			{
				Name = "tj1",
				Type = "turbojet",
				Efficiency = 0.3m,
				Pmin = 0m,
				Pmax = 16m
			},
			new PowerPlantRequest
			{
				Name = "windpark1",
				Type = "windturbine",
				Efficiency = 1m,
				Pmin = 0m,
				Pmax = 150m
			},
			new PowerPlantRequest
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