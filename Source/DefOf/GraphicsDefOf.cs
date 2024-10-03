using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockableDoors.DefOf
{
	[RimWorld.DefOf]
	static internal class GraphicsDefOf
	{
		static GraphicsDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(GraphicsDefOf));
		}

		/// <summary>
		/// Red icon used to indicate a completely blocked door.
		/// </summary>
		public static GraphicsDef? LockedDoorGraphics;

		/// <summary>
		/// Yellow icon used to indicate a locked door with exceptions.
		/// </summary>
		public static GraphicsDef? PartialLockedDoorGraphics;
	}
}
