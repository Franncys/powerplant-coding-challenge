using System.Text.Json.Serialization;

namespace PowerPlantCodingChallenge.Api.Models.Requests
{
	public sealed class ProductionPlanRequest
	{
		[JsonPropertyName("load")]
		public decimal Load { get; init; }

		[JsonPropertyName("fuels")]
		public FuelsRequest Fuels { get; init; } = new();

		[JsonPropertyName("powerplants")]
		public IReadOnlyCollection<PowerPlantRequest> PowerPlants { get; init; } = [];
	}
}
