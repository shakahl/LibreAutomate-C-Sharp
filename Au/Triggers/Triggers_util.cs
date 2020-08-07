using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
//using System.Linq;

using Au.Types;

namespace Au.Triggers
{
	static class TrigUtil
	{
		/// <summary>
		/// Gets left and right modifiers. Uses AKeys.IsPressed.
		/// Returns modL | modR.
		/// </summary>
		public static KMod GetModLR(out KMod modL, out KMod modR)
		{
			KMod L = 0, R = 0;
			if(AKeys.IsPressed(KKey.LCtrl)) L |= KMod.Ctrl;
			if(AKeys.IsPressed(KKey.LShift)) L |= KMod.Shift;
			if(AKeys.IsPressed(KKey.LAlt)) L |= KMod.Alt;
			if(AKeys.IsPressed(KKey.Win)) L |= KMod.Win;
			if(AKeys.IsPressed(KKey.RCtrl)) R |= KMod.Ctrl;
			if(AKeys.IsPressed(KKey.RShift)) R |= KMod.Shift;
			if(AKeys.IsPressed(KKey.RAlt)) R |= KMod.Alt;
			if(AKeys.IsPressed(KKey.RWin)) R |= KMod.Win;
			modL = L; modR = R;
			return L | R;
		}
	}
}
