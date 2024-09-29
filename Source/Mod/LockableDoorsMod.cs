using HarmonyLib;
using LockableDoors.Tabs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LockableDoors.Mod
{
	internal class LockableDoorsMod : Verse.Mod
	{

#pragma warning disable CS8618 // Will always be initialized by constructor by rimworld.
		internal static Harmony Harmony;
		public static LockableDoorsSettings Settings;
		public static LockableDoorsMod Mod;
#pragma warning restore CS8618


		public override string SettingsCategory() => Mod.Content?.Name ?? "Lockable Doors";


		public LockableDoorsMod(ModContentPack content) : base(content)
		{
			Mod = this;
		}

		public static void Initialize()
		{
			Settings = Mod.GetSettings<LockableDoorsSettings>();

			var harmony = new Harmony(Mod.Content.PackageId);
			harmony.PatchAll();
			Harmony = harmony;


			List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
			int count = thingDefs.Count;
			for (int i = 0; i < count; i++)
			{
				// Find all door defs
				ThingDef doorDef = thingDefs[i];
				if (doorDef.thingClass == typeof(Building_Door))
				{
					doorDef.inspectorTabsResolved ??= new List<InspectTabBase>();
					doorDef.inspectorTabsResolved.Add(ExceptionsTab.Instance);
				}
			}
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			bool before = Settings.PrintLockSymbol;

			Settings.Menu.Draw(inRect);
			
			if (before != Settings.PrintLockSymbol)
				SectionLayers.SectionLayer_DoorLocks.InvalidateDoors();
		}
	}
}
