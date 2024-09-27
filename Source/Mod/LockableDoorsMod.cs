using HarmonyLib;
using LockableDoors.Patches;
using LockableDoors.UserInterface;
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
#pragma warning restore CS8618

		public LockableDoorsMod(ModContentPack content) : base(content)
		{
		}

		public override string SettingsCategory()
		{
			return "Lockable doors";
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
