using Prepatcher;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockableDoors.Patches
{
	internal static class DoorsPatch
	{
		[PrepatcherField]
		[DefaultValue(false)]
		public static extern ref bool IsLocked(this Building_Door target);

		[PrepatcherField]
		[DefaultValue(null)]
		public static extern ref Verse.Gizmo ToggleLockGizmo(this Building_Door target);

		[HarmonyLib.HarmonyPatch(nameof(Building_Door.PawnCanOpen), typeof(Building_Door)), HarmonyLib.HarmonyPrefix]
		internal static bool PawnCanOpenPatch(Verse.Pawn pawn, ref bool __result, Building_Door ___instance)
		{
			if (IsLocked(___instance))
			{
				__result = false;
				return false;
			}
			return true;
		}


		[HarmonyLib.HarmonyPatch(nameof(Building_Door.GetGizmos), typeof(Building_Door)), HarmonyLib.HarmonyPostfix]
		internal static IEnumerable<Verse.Gizmo> GetGizmosPatch(IEnumerable<Verse.Gizmo> gizmos, Building_Door ___instance)
		{
			// Show any existing buttons
			foreach (Verse.Gizmo gizmo in gizmos)
			{
				yield return gizmo;
			}

			// Get cached button
			Verse.Gizmo togglebutton = ToggleLockGizmo(___instance);

			// If no button is cached on this door, generate one.
			if (togglebutton == null)
			{
				togglebutton = new Verse.Command_Action()
				{
					action = () =>
					{
						ref bool locked = ref IsLocked(___instance);
						locked = !locked;
					},
				};
				ToggleLockGizmo(___instance) = togglebutton;
			}

			yield return togglebutton;
		}
	}
}
