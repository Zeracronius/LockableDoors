using LockableDoors.Enums;
using LockableDoors.Extensions;
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
					exceptions ^= exception;
			}
		}

		public ExceptionsTab()
        {
			size = new Vector2(420f, 240f);
			labelKey = "LockableDoorsAllowButton";

			var nodes = new List<TreeNode_FilterBox>()
			{
				new TreeNode_FilterBox("LockableDoorsAllowColonists".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Colonists)),
				new TreeNode_FilterBox("LockableDoorsAllowPets".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Pets)),
				new TreeNode_FilterBox("LockableDoorsAllowAllies".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Allies)),
				new TreeNode_FilterBox("LockableDoorsAllowSlaves".Translate(), callback: (in Rect rect) => DrawCheckbox(rect, Exceptions.Slaves)),
			};

			_optionsTree = new FilterTreeBox(nodes);
		}

        protected override void FillTab()
		{
			Rect inRect = new Rect(0f, 0f, size.x, size.y).ContractedBy(10f);
			_optionsTree.Draw(inRect);
		}
	}
}
