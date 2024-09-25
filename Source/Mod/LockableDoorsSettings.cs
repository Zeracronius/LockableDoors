﻿using LockableDoors.UserInterface.TreeBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LockableDoors.Mod
{
	internal class LockableDoorsSettings : ModSettings
	{
		public bool PrintLockSymbol = false;
		public List<TreeNode_FilterBox> Nodes;

        public LockableDoorsSettings()
        {
			Nodes = new List<TreeNode_FilterBox>
			{
				new TreeNode_FilterBox("LockableDoorsSettingsShowLocks".Translate(), callback: (in Rect x) =>
					Widgets.Checkbox(x.position, ref PrintLockSymbol, x.height))


			};

		}

        public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Values.Look(ref PrintLockSymbol, nameof(PrintLockSymbol));
		}
	}
}