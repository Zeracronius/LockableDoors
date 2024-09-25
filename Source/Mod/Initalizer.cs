using HarmonyLib;
using LockableDoors.Patches;
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
			LockableDoorsMod.Settings = LoadedModManager.GetMod<LockableDoorsMod>().GetSettings<LockableDoorsSettings>();


			var harmony = new Harmony("Zeracronius.LockableDoors");
			harmony.PatchCategory("Required");

			if (LockableDoorsMod.Settings.PrintLockSymbol)
				harmony.PatchCategory("Printing");

			LockableDoorsMod.Harmony = harmony;
		}
    }
}
