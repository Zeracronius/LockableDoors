using HarmonyLib;
using LockableDoors.Enums;
using LockableDoors.Extensions;
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
	internal static class DoorsPatches
	{
		private static string _unlockedLabel = "LockableDoorsUnlocked".Translate();
		private static string _lockedLabel = "LockableDoorsLocked".Translate();
		private static Action<Building_Door, Verse.Map> _clearReachabilityCache;

		static DoorsPatches()
		{
			_clearReachabilityCache = HarmonyLib.AccessTools.MethodDelegate<Action<Building_Door, Verse.Map>>("RimWorld.Building_Door:ClearReachabilityCache");
		}

		// Extend door's expose data with additional lock value.
		[HarmonyLib.HarmonyPatch(nameof(Building_Door.ExposeData)), HarmonyLib.HarmonyPostfix]
		internal static void ExposeDataPostfix(Building_Door __instance)
		{
			Scribe_Values.Look(ref __instance.IsLocked(), nameof(DoorExtensions.IsLocked));
			Scribe_Values.Look(ref __instance.LockExceptions(), nameof(DoorExtensions.LockExceptions));
		}

		// Where the actual magic happens! Prevent doors from being opened by anyone if locked.
		[HarmonyLib.HarmonyPatch(nameof(Building_Door.PawnCanOpen)), HarmonyLib.HarmonyPrefix]
		[HarmonyPriority(Priority.First)]
		internal static bool PawnCanOpenPrefix(Pawn p, ref bool __result, Building_Door __instance)
		{
			// If the door is not locked, continue as normal.
			if (__instance.IsLocked() == false)
				return true;

			// Otherwise check for exceptions
			Exceptions exceptions = __instance.LockExceptions();
			if (exceptions != Exceptions.None && Mod.LockableDoorsMod.Settings.AllowExceptions)
			{
				// If exceptions are defined, then check if pawn is player-owned.
				if (p.Faction?.def.isPlayer == true)
				{
					if ((exceptions & Exceptions.Pets) == Exceptions.Pets)
					{
						if (p.def.race.Animal)
							return true;
					}

					if ((exceptions & Exceptions.Colonists) == Exceptions.Colonists)
					{
						// If colonists are exempt and pawn is colonist, continue as normal.
						if (p.IsColonist || p.IsColonyMech)
							return true;
					}

					if ((exceptions & Exceptions.Slaves) == Exceptions.Slaves)
					{
						if (p.IsSlaveOfColony)
							return true;
					}
				}
				else
				{
					if ((exceptions & Exceptions.Allies) == Exceptions.Allies)
					{
						if (p.Faction.HostileTo(Faction.OfPlayer) == false)
							return true;
					}
				}
			}

			__result = false;
			return false;
		}

		// Patch GetGizmos to include toggle button for doors owned by player.
		[HarmonyLib.HarmonyPatch(nameof(Building_Door.GetGizmos)), HarmonyLib.HarmonyPostfix]
		internal static IEnumerable<Verse.Gizmo> GetGizmosPostfix(IEnumerable<Verse.Gizmo> values, Building_Door __instance, Faction ___factionInt)
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
				bool locked = __instance.IsLocked();
				if (togglebutton == null)
				{
					togglebutton = new Verse.Command_Action()
					{
						defaultLabel = locked ? _lockedLabel : _unlockedLabel,
						icon = locked ? Mod.Textures.LockedIcon : Mod.Textures.UnlockedIcon,
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
			action!.defaultLabel = locked ? _lockedLabel : _unlockedLabel;
			action!.icon = locked ? Mod.Textures.LockedIcon : Mod.Textures.UnlockedIcon;
			_clearReachabilityCache(door, door.Map);

			// Invalidate lock print state
			door.Map.mapDrawer.MapMeshDirty(door.Position, DefOf.LDMapMeshFlagDefOf.DoorLocks);
		}
	}
}
