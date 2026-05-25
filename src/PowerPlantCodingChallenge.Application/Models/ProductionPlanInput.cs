using System;
using System.Collections.Generic;
using System.Text;

namespace PowerPlantCodingChallenge.Application.Models
{
	public sealed class ProductionPlanInput
	{
		public decimal Load { get; init; }

		public FuelInput Fuels { get; init; } = new();

		public IReadOnlyCollection<PowerPlantInput> PowerPlants { get; init; } = [];
	}
}
