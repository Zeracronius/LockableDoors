using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LockableDoors.Mod
{
	[StaticConstructorOnStartup]
	internal static class Initalizer
	{
        static Initalizer()
        {

			Harmony Harmony = new Harmony("DynamicTradeInterfaceMod");
		}
    }
}
