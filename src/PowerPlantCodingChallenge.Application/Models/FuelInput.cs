namespace PowerPlantCodingChallenge.Application.Models
{
	public sealed class FuelInput
	{
		public decimal GasEuroPerMwh { get; init; }

		public decimal KerosineEuroPerMwh { get; init; }

		public decimal Co2EuroPerTon { get; init; }

		public decimal WindPercentage { get; init; }
	}
}