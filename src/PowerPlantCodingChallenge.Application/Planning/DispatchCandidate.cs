namespace PowerPlantCodingChallenge.Application.Planning;

/// <summary>
/// Internal model used by the planning algorithm.
/// 
/// It enriches the input power plant data with calculated values such as available capacity,
/// production cost and original position in the request.
/// </summary>
internal sealed class DispatchCandidate
{
	public string Name { get; init; } = string.Empty;

	public decimal Pmin { get; init; }

	public decimal AvailablePmax { get; init; }

	public decimal CostPerMwh { get; init; }

	public int OriginalIndex { get; init; }
}