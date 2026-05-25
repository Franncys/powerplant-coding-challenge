using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PowerPlantCodingChallenge.Api.Models.Requests;
using PowerPlantCodingChallenge.Api.Models.Responses;
using PowerPlantCodingChallenge.Application.Interfaces;
using PowerPlantCodingChallenge.Application.Models;

namespace PowerPlantCodingChallenge.Api.Controller
{
	[Route("productionplan")]
	[ApiController]
	public class ProductionPlanController : ControllerBase
	{
		private readonly IProductionPlanService _productionPlanService;

		public ProductionPlanController(IProductionPlanService productionPlanService)
		{
			_productionPlanService = productionPlanService;
		}

		[HttpPost]
		[ProducesResponseType(typeof(IReadOnlyCollection<ProductionPlanResponse>), StatusCodes.Status200OK)]
		public IActionResult CalculateProductionPlan(ProductionPlanRequest request)
		{
			try
			{
				var input = MapToApplicationInput(request);

				var result = _productionPlanService.CalculateProductionPlan(input);

				var response = result
					.Select(item => new ProductionPlanResponse
					{
						Name = item.Name,
						Production = item.Production
					})
					.ToList();

				return Ok(response);
			}
			catch (ArgumentException exception)
			{
				return BadRequest(new
				{
					error = exception.Message
				});
			}
			catch (InvalidOperationException exception)
			{
				return BadRequest(new
				{
					error = exception.Message
				});
			}
		}

		private static ProductionPlanInput MapToApplicationInput(ProductionPlanRequest request)
		{
			return new ProductionPlanInput
			{
				Load = request.Load,
				Fuels = new FuelInput
				{
					GasEuroPerMwh = request.Fuels.GasEuroPerMwh,
					KerosineEuroPerMwh = request.Fuels.KerosineEuroPerMwh,
					Co2EuroPerTon = request.Fuels.Co2EuroPerTon,
					WindPercentage = request.Fuels.WindPercentage
				},
				PowerPlants = request.PowerPlants
					.Select(powerPlant => new PowerPlantInput
					{
						Name = powerPlant.Name,
						Type = powerPlant.Type,
						Efficiency = powerPlant.Efficiency,
						Pmin = powerPlant.Pmin,
						Pmax = powerPlant.Pmax
					})
					.ToList()
			};
		}
	}
}
