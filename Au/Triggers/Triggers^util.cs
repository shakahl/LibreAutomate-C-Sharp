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
using static Au.NoClass;

namespace Au.Triggers
{
	static class TrigUtil
	{
		/// <summary>
		/// To specify 'any modifier(s)' in hotkey and mouse triggers, can be used ?, like "Shift?+K" (any Shift) or "Ctrl+Shift?+K" (Ctrl+any Shift) or "?+K" (any mods).
		/// To implement it simply, we add all modifier combinations to the dictionary. For example, for "Shift?+K" we add "K" and "Shift+K".
		/// This function returns a 16 bit array containing 1 bits for modifier combinations to add: 0, Shift, Ctrl, Ctrl+Shift, Alt, ....
		/// </summary>
		public static int ModBitArray(KMod mod, KMod modAny)
		{
			int b = 0b1111111111111111;
			if(0 != (mod & KMod.Shift)) b &= 0b1010101010101010; else if(0 == (modAny & KMod.Shift)) b &= 0b0101010101010101; //if must be Shift, erase all without Shift; else if cannot be Shift, erase all with Shift
			if(0 != (mod & KMod.Ctrl)) b &= 0b1100110011001100; else if(0 == (modAny & KMod.Ctrl)) b &= 0b0011001100110011;
			if(0 != (mod & KMod.Alt)) b &= 0b1111000011110000; else if(0 == (modAny & KMod.Alt)) b &= 0b0000111100001111;
			if(0 != (mod & KMod.Win)) b &= 0b1111111100000000; else if(0 == (modAny & KMod.Win)) b &= 0b0000000011111111;
			//for(int i = 0; i < 16; i++) if(0 != (b & (1 << i))) Print((KMod)i);
			return b;
		}

		/// <summary>
		/// Gets left and right modifiers. Uses Keyb.IsPressed.
		/// Returns modL | modR.
		/// </summary>
		public static KMod GetModLR(out KMod modL, out KMod modR)
		{
			KMod L = 0, R = 0;
			if(Keyb.IsPressed(KKey.LCtrl)) L |= KMod.Ctrl;
			if(Keyb.IsPressed(KKey.LShift)) L |= KMod.Shift;
			if(Keyb.IsPressed(KKey.LAlt)) L |= KMod.Alt;
			if(Keyb.IsPressed(KKey.Win)) L |= KMod.Win;
			if(Keyb.IsPressed(KKey.RCtrl)) R |= KMod.Ctrl;
			if(Keyb.IsPressed(KKey.RShift)) R |= KMod.Shift;
			if(Keyb.IsPressed(KKey.RAlt)) R |= KMod.Alt;
			if(Keyb.IsPressed(KKey.RWin)) R |= KMod.Win;
			modL = L; modR = R;
			return L | R;
		}
	}
}
