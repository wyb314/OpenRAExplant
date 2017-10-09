using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	public class PlannerActionPool : FlyweightPool<PlannerAction>
	{
		/// <summary>
		/// Constructor. Initializes flyweight pool with an instance of each PlannerAction available
		/// in currently-loaded assemblies
		/// </summary>
		public PlannerActionPool()
			:this(true)
		{
		}

		/// <summary>
		/// Constructor. Allows the user to specify whether or not to preload PlannerAction instances.
		/// </summary>
		/// <param name="preload"></param>
		public PlannerActionPool(bool preload)
			:base(preload)
		{
		}

		/// <summary>
		/// Gets a new PlannerActionSet containing all currently loaded PlannerActions.
		/// </summary>
		/// <returns></returns>
		public PlannerActionSet ToSet()
		{
			return new PlannerActionSet(_instances.Values);
		}
	}
}
