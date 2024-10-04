using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockableDoors.Enums
{
	[Flags]
	public enum Exceptions : byte
	{
		None = 0,
		Colonists = 1,
		Slaves = 2,
		Pets = 4,
		Allies = 8,
		ColonyMechs = 16,
	}
}
