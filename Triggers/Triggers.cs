using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
using System.Reflection;
using System.Diagnostics;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Triggers
{
public static class Trigger
{
	public static readonly HotkeyTriggers Hotkey=new HotkeyTriggers();

	[AttributeUsage(AttributeTargets.Method)]
	public class HotkeyAttribute : Attribute
	{
		public readonly string hotkey;

		public HotkeyAttribute(string hotkey)
		{
			this.hotkey=hotkey;
		}
	}

}
}
