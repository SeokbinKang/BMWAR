using ContainerPacking.Entities;
using System.Collections.Generic;

namespace ContainerPacking.Algorithms
{
	public abstract class AlgorithmBase
	{
		public abstract ContainerPackingResult Run(Container container, List<Item> items);
	}
}