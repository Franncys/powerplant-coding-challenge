using System.Text.Json.Serialization;

namespace PowerPlantCodingChallenge.Api.Models.Requests
{
	public sealed class PowerPlantRequest
	{
		[JsonPropertyName("name")]
		public string Name { get; init; } = string.Empty;

		[JsonPropertyName("type")]
		public string Type { get; init; } = string.Empty;

		[JsonPropertyName("efficiency")]
		public decimal Efficiency { get; init; }

		[JsonPropertyName("pmin")]
		public decimal Pmin { get; init; }

		[JsonPropertyName("pmax")]
		public decimal Pmax { get; init; }
	}
}
