using PowerPlantCodingChallenge.Application.Interfaces;
using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Application.Services
{
	public sealed class ProductionPlanService : IProductionPlanService
	{
		private readonly IProductionPlanner _productionPlanner;

		public ProductionPlanService(IProductionPlanner productionPlanner)
		{
			_productionPlanner = productionPlanner;
		}

		public IReadOnlyCollection<ProductionPlanOutput> CalculateProductionPlan(ProductionPlanInput input)
		{
			ArgumentNullException.ThrowIfNull(input, nameof(input));

			return _productionPlanner.Calculate(input);
		}
	}
}
