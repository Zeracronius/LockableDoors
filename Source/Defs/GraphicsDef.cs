using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LockableDoors
{
	/// <summary>
	/// Def that allows defining a stand-alone <see cref="GraphicData"/>.
	/// </summary>
	/// <seealso cref="Verse.Def" />
	internal class GraphicsDef : Def
	{
		public GraphicData? graphicData;
	}
}
