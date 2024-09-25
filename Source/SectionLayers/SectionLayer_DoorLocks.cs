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
		internal static void RegenerateLayer()
		{
			foreach (Map map in Find.Maps)
			{
				if (map.IsPlayerHome)
				{
					foreach (Building_Door door in map.listerBuildings.AllBuildingsColonistOfClass<Building_Door>())
					{
						foreach (IntVec3 item in door.OccupiedRect())
							map.mapDrawer.MapMeshDirty(item, DefOf.LDMapMeshFlagDefOf.DoorLocks);
					}
				}
			}
		}

		private GraphicsDef _lockedDoorGraphics;
		private CellRect _bounds;

		public SectionLayer_DoorLocks(Section section) 
			: base(section)
		{
			relevantChangeTypes = (ulong)DefOf.LDMapMeshFlagDefOf.DoorLocks | (ulong)MapMeshFlagDefOf.Buildings;
			_lockedDoorGraphics = DefDatabase<GraphicsDef>.GetNamed("LockedDoorGraphics");
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
					if (thing is Building_Door door)
					{
						if (door.IsLocked())
						{
							_lockedDoorGraphics.graphicData?.GraphicColoredFor(door).Print(this, door, 0);
							_bounds.Encapsulate(thing.OccupiedDrawRect());
						}
					}
				}
			}
			FinalizeMesh(MeshParts.All);
		}
	}
}
