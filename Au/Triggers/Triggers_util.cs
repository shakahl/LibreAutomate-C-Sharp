using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Triggers
{
	static class TrigUtil
	{
		/// <summary>
		/// Gets left and right modifiers. Uses AKeyboard.IsPressed.
		/// Returns modL | modR.
		/// </summary>
		public static KMod GetModLR(out KMod modL, out KMod modR)
		{
			KMod L = 0, R = 0;
			if(AKeyboard.IsPressed(KKey.LCtrl)) L |= KMod.Ctrl;
			if(AKeyboard.IsPressed(KKey.LShift)) L |= KMod.Shift;
			if(AKeyboard.IsPressed(KKey.LAlt)) L |= KMod.Alt;
			if(AKeyboard.IsPressed(KKey.Win)) L |= KMod.Win;
			if(AKeyboard.IsPressed(KKey.RCtrl)) R |= KMod.Ctrl;
			if(AKeyboard.IsPressed(KKey.RShift)) R |= KMod.Shift;
			if(AKeyboard.IsPressed(KKey.RAlt)) R |= KMod.Alt;
			if(AKeyboard.IsPressed(KKey.RWin)) R |= KMod.Win;
			modL = L; modR = R;
			return L | R;
		}
	}
}
