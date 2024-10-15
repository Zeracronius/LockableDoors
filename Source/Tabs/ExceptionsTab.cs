using LockableDoors.Enums;
using LockableDoors.Extensions;
using LockableDoors.Mod;
using LockableDoors.UserInterface;
using LockableDoors.UserInterface.TreeBox;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LockableDoors.Tabs
{
	internal class ExceptionsTab : ITab
	{
		public static ExceptionsTab Instance = new ExceptionsTab();
		
		private FilterTreeBox _optionsTree;
		private Exceptions _copiedExceptions;

		public Gizmo[] CopyPasteButtons;

		public override bool IsVisible => Mod.LockableDoorsMod.Settings.AllowExceptions && (SelThing as Building_Door)?.IsLocked() == true;
		public override bool Hidden => false;


		private void DrawCheckbox(in Rect rect, Exceptions exception)
		{
			if (SelThing is Building_Door door)
			{
				ref Exceptions exceptions = ref door.LockExceptions();

				bool enabled = (exceptions & exception) == exception;
				bool wasEnabled = enabled;
				Widgets.Checkbox(rect.x, rect.y, ref enabled);

				if (wasEnabled != enabled)
				{
					exceptions ^= exception;

					// Invalidate lock print state
					door.Map.mapDrawer.MapMeshDirty(door.Position, DefOf.LDMapMeshFlagDefOf.DoorLocks);
				}
			}
		}

		public ExceptionsTab()
        {
			size = new Vector2(420f, 240f);
			labelKey = "LockableDoorsAllowButton";

			CopyPasteButtons = new Gizmo[]
			{
				new Verse.Command_Action()
				{
					defaultLabel = "Copy exceptions",
					icon = Textures.CopyIcon,
					action = CopyExceptions
				},
				new Verse.Command_Action()
				{
					defaultLabel = "Paste exceptions",
					icon = Textures.PasteIcon,
					action = PasteExceptions
				},
			};

			var nodes = new List<TreeNode_FilterBox>()
			{
				new TreeNode_FilterBox("LockableDoorsAllowColonists".Translate(), "LockableDoorsAllowColonistsTooltip".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Colonists)),
				new TreeNode_FilterBox("LockableDoorsAllowMechs".Translate(), "LockableDoorsAllowMechsTooltip".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.ColonyMechs)),
				new TreeNode_FilterBox("LockableDoorsAllowPets".Translate(), "LockableDoorsAllowPetsTooltip".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Pets)),
				new TreeNode_FilterBox("LockableDoorsAllowAllies".Translate(), "LockableDoorsAllowAlliesTooltip".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Allies)),
				new TreeNode_FilterBox("LockableDoorsAllowSlaves".Translate(), "LockableDoorsAllowSlavesTooltip".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Slaves)),
			};

			_optionsTree = new FilterTreeBox(nodes);
		}

		private void CopyExceptions()
		{
			if (SelThing is Building_Door door)
			{
				_copiedExceptions = door.LockExceptions();
			}
		}

		private void PasteExceptions()
		{
			foreach (Building_Door door in AllSelObjects.OfType<Building_Door>())
			{
				door.LockExceptions() = _copiedExceptions;
			}
		}

        protected override void FillTab()
		{
			Rect inRect = new Rect(0f, 0f, size.x, size.y).ContractedBy(10f);
			_optionsTree.Draw(inRect);
		}
	}
}
