using LockableDoors.DefOf;
using LockableDoors.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LockableDoors.SectionLayers
{
	internal class SectionLayer_DoorLocks : SectionLayer
	{
		/// <summary>
		/// Invalidates the printed lock of all player-owned doors.
		/// </summary>
		internal static void InvalidateDoors()
		{
			foreach (Map map in Find.Maps)
			{
				if (map.IsPlayerHome)
				{
					foreach (Building_Door door in map.listerBuildings.AllBuildingsColonistOfClass<Building_Door>())
					{
						map.mapDrawer.MapMeshDirty(door.Position, DefOf.LDMapMeshFlagDefOf.DoorLocks);
					}
				}
			}
		}

		private CellRect _bounds;

		public SectionLayer_DoorLocks(Section section) 
			: base(section)
		{
			// Trigger printing of this layer when DoorLocks or Buildings change.
			relevantChangeTypes = (ulong)DefOf.LDMapMeshFlagDefOf.DoorLocks | (ulong)MapMeshFlagDefOf.Buildings;
		}

		public override CellRect GetBoundaryRect()
		{
			return _bounds;
		}

		public override void DrawLayer()
		{
			if (DebugViewSettings.drawThingsPrinted)
			{
				base.DrawLayer();
			}
		}

		public override void Regenerate()
		{
			ClearSubMeshes(MeshParts.All);
			_bounds = section.CellRect;

			if (Mod.LockableDoorsMod.Settings.PrintLockSymbol == false)
				return;

			foreach (IntVec3 item in section.CellRect)
			{
				List<Thing> list = base.Map.thingGrid.ThingsListAt(item);
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					Thing thing = list[i];
					if (thing is Building_Door door && thing.Position == item)
					{
						if (door.IsLocked())
						{
							GraphicsDef? lockGraphic;
							if (door.LockExceptions() == Enums.Exceptions.None)
								lockGraphic = GraphicsDefOf.LockedDoorGraphics;
							else
								lockGraphic = GraphicsDefOf.PartialLockedDoorGraphics;

							lockGraphic?.graphicData?.GraphicColoredFor(door).Print(this, door, 0);
							_bounds.Encapsulate(thing.OccupiedDrawRect());
						}
					}
				}
			}
			FinalizeMesh(MeshParts.All);
		}
	}
}
