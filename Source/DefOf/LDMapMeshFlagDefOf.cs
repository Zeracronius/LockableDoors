using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockableDoors.DefOf
{
	[RimWorld.DefOf]
	static internal class LDMapMeshFlagDefOf
	{
		static LDMapMeshFlagDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(LDMapMeshFlagDefOf));
		}

		public static MapMeshFlagDef? DoorLocks;
	}
}