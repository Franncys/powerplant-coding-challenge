using PowerPlantCodingChallenge.Application.Interfaces;
using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Planning
{
	public sealed class MeritOrderProductionPlanner : IProductionPlanner
	{
		public IReadOnlyCollection<ProductionPlanOutput> Calculate(ProductionPlanInput input)
		{
			ArgumentNullException.ThrowIfNull(input);

			return input.PowerPlants
				.Select(powerPlant => new ProductionPlanOutput
				{
					Name = powerPlant.Name,
					Production = 0m
				})
				.ToList();
		}
	}
}
