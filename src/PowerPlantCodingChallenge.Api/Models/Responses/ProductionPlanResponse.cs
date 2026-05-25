using System.Text.Json.Serialization;

namespace PowerPlantCodingChallenge.Api.Models.Responses
{
	public sealed record ProductionPlanResponse
	{
		[JsonPropertyName("name")]
		public string Name { get; init; } = string.Empty;

		[JsonPropertyName("p")]
		public decimal Production { get; init; }
	}
}
