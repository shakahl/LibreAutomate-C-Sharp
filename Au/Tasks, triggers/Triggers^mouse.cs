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

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	//MouseClick, MouseWheel, MouseEdge, MouseSwipe


	class MouseHook
	{
		Util.WinHook _hook;
		public static MouseHook Instance;

		public MouseHook()
		{
			//Output.LibWriteQM2("ctor");
			_hook = Util.WinHook.Mouse(_Hook);
		}

		public void Dispose()
		{
			//Output.LibWriteQM2("disp");
			_hook.Dispose(); _hook = null;
			Instance = null;
		}

		bool _Hook(HookData.Mouse k)
		{
			if(k.Event== HookData.MouseEvent.Move) {
				if(Keyb.IsScrollLock) {
					var ti = TriggersServer.Instance;
					//ti.SendBegin();
					//ti.SendAdd(0, 100);
					//ti.Send(ETriggerType.Hotkey);
					ti.SendHook(-100);
				}
			}
			return false;
		}
	}
}
