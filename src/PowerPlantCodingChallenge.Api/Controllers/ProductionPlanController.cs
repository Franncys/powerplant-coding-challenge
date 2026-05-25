using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PowerPlantCodingChallenge.Api.Models.Requests;
using PowerPlantCodingChallenge.Api.Models.Responses;

namespace PowerPlantCodingChallenge.Api.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductionPlanController : ControllerBase
	{
		[HttpPost]
		[ProducesResponseType(typeof(IReadOnlyCollection<ProductionPlanResponse>), StatusCodes.Status200OK)]
		public IActionResult CalculateProductionPlan(ProductionPlanRequest request)
		{
			var response = request.PowerPlants
			.Select(powerPlant => new ProductionPlanResponse
			{
				Name = powerPlant.Name,
				Production = 0m
			})
			.ToList();

			return Ok(response);
		}
	}
}
