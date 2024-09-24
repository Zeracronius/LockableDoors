using Prepatcher;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LockableDoors.Patches
{
	[HarmonyLib.HarmonyPatch(typeof(Building_Door))]
	internal static class DoorsPatch
	{
		private static Action<Building_Door, Verse.Map> ClearReachabilityCache;

		static DoorsPatch()
		{
			ClearReachabilityCache = HarmonyLib.AccessTools.MethodDelegate<Action<Building_Door, Verse.Map>>("RimWorld.Building_Door:ClearReachabilityCache");
		}

		static string UnlockedLabel = "LockableDoorsUnlocked".Translate();
		static string LockedLabel = "LockableDoorsLocked".Translate();


		/// <summary>
		/// Injected prepatcher locked field on Building_Door object.
		/// </summary>
		[PrepatcherField]
		[Prepatcher.DefaultValue(false)]
		public static extern ref bool IsLocked(this Building_Door target);

		/// <summary>
		/// Injected prepatcher Gizmo field on Building_Door object.
		/// </summary>
		[PrepatcherField]
		[Prepatcher.DefaultValue(null)]
		public static extern ref Verse.Command_Action ToggleLockGizmo(this Building_Door target);

		// Where the actual magic happens! Prevent doors from being opened by anyone if locked.
		[HarmonyLib.HarmonyPatch(nameof(Building_Door.PawnCanOpen)), HarmonyLib.HarmonyPrefix]
		internal static bool PawnCanOpenPatch(ref bool __result, Building_Door __instance)
		{
			if (__instance.IsLocked())
			{
				__result = false;
				return false;
			}
			return true;
		}

		// Patch GetGizmos to include toggle button for doors owned by player.
		[HarmonyLib.HarmonyPatch(nameof(Building_Door.GetGizmos)), HarmonyLib.HarmonyPostfix]
		internal static IEnumerable<Verse.Gizmo> GetGizmosPatch(IEnumerable<Verse.Gizmo> values, Building_Door __instance, Faction ___factionInt)
		{
			// Show any existing buttons
			foreach (Verse.Gizmo gizmo in values)
			{
				yield return gizmo;
			}

			if (___factionInt?.def?.isPlayer == true)
			{
				// Get cached button
				Verse.Command_Action togglebutton = __instance.ToggleLockGizmo();

				// If no button is cached on this door, generate one.
				if (togglebutton == null)
				{
					togglebutton = new Verse.Command_Action()
					{
						defaultLabel = UnlockedLabel,
						icon = Mod.Textures.UnlockedIcon,
						action = () => ToggleDoor(__instance, togglebutton!)
					};
					__instance.ToggleLockGizmo() = togglebutton;
				}


				yield return togglebutton;
			}
		}

		/// <summary>
		/// Toggles the door's lock and updates gizmo.
		/// </summary>
		/// <param name="door">The toggled door.</param>
		/// <param name="action">The gizmo itself.</param>
		private static void ToggleDoor(Building_Door door, Command_Action action)
		{
			ref bool locked = ref door.IsLocked();
			locked = !locked;
			action!.defaultLabel = locked ? LockedLabel : UnlockedLabel;
			action!.icon = locked ? Mod.Textures.LockedIcon : Mod.Textures.UnlockedIcon;
			ClearReachabilityCache(door, door.Map);
		}
	}
}
