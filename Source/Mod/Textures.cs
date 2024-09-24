﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LockableDoors.Mod
{
	[StaticConstructorOnStartup]
	public static class Textures
	{
		public static readonly Texture2D LockedIcon = ContentFinder<Texture2D>.Get("Icons/Locked");
		public static readonly Texture2D UnlockedIcon = ContentFinder<Texture2D>.Get("Icons/Unlocked");
	}
}