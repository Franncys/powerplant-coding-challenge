using System.Text.Json.Serialization;
namespace PowerPlantCodingChallenge.Api.Models.Requests
{
	public sealed record FuelsRequest
	{
		[JsonPropertyName("gas(euro/MWh)")]
		public decimal GasEuroPerMwh { get; init; }

		[JsonPropertyName("kerosine(euro/MWh)")]
		public decimal KerosineEuroPerMwh { get; init; }

		[JsonPropertyName("co2(euro/ton)")]
		public decimal Co2EuroPerTon { get; init; }

		[JsonPropertyName("wind(%)")]
		public decimal WindPercentage { get; init; }
	}
}
