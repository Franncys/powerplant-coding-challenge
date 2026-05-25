using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PowerPlantCodingChallenge.Api.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductionPlanController : ControllerBase
	{
		[HttpPost]
		public IActionResult CalculateProductionPlan()
		{
			return Ok("");
		}

	}
}
