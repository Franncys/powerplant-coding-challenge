namespace PowerPlantCodingChallenge.Application.Models
{
	public sealed class PowerPlantInput
	{
		public string Name { get; init; } = string.Empty;

		public string Type { get; init; } = string.Empty;

		public decimal Efficiency { get; init; }

		public decimal Pmin { get; init; }

		public decimal Pmax { get; init; }
	}
}