using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Linq;

using Au;
using static Au.NoClass;

namespace Au.Triggers
{
	public class HotkeyTriggers
	{
		///Table of triggers.
		static Dictionary<int, Target> _t = new Dictionary<int, Target>();

		///Trigger target (handler) function type.
		public delegate void Target(Message b);

		///Trigger event data sent to the target function.
		public class Message { }

		//Indexer that allows to assign triggers to target functions like this: Trigger.Hotkey["Ctrl+K"] =m=> { Print("target"); };
		public Target this[string hotkey]
		{
			set
			{
				//_t.Add(1, value);
				_t[1] = value;
			}
		}

		static HotkeyTriggers()
		{
			//System.Reflection.
			Assembly assembly = Util.Assembly_.EntryAssembly;

			var methods = assembly.GetTypes()
						.SelectMany(t => t.GetMethods())
						.Where(m => m.GetCustomAttributes(typeof(Trigger.HotkeyAttribute), false).Length > 0)
						.ToArray();
			foreach(MethodInfo m in methods) {
				Print($"\t{m.Name}");
				foreach(var a in (Trigger.HotkeyAttribute[])m.GetCustomAttributes(typeof(Trigger.HotkeyAttribute), false)) {
					Print($"\t\t{a.hotkey}");
					Trigger.Hotkey[a.hotkey] = (Target)m.CreateDelegate(typeof(Target));
					//Trigger.Hotkey[a.hotkey] = (TargetPath)Delegate.CreateDelegate(typeof(TargetPath), m);
				}
			}

			//foreach(Type t in k.GetTypes())
			//{
			//	Print(t.Name);
			//	foreach(MethodInfo m in t.GetMethods())
			//	{
			//	foreach(CustomAttributeData d in m.CustomAttributes)
			//	{
			//		Print(
			//		Attribute[] attrs = m.CustomAttributes Attribute
			//		Print($"\t{m.Name}");
			//             }
			//	//
			//	//if(
			//}
		}

		public static void TestFireTrigger()
		{
			Target h;
			if(_t.TryGetValue(1, out h)) {
				h(null);
			}
		}
	}

}
