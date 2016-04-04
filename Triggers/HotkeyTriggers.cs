using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Triggers
{
	public class HotkeyTriggers
	{
		///Table of triggers.
		static Dictionary<int, Target> _t = new Dictionary<int, Target>();

		///Trigger target (handler) function type.
		public delegate void Target(Message b);

		///Trigger event data sent to the target function.
		public class Message { }

		//Indexer that allows to assign triggers to target functions like this: Trigger.Hotkey["Ctrl+K"] =m=> { Out("target"); };
		public Target this[string hotkey]
		{
			set
			{
				//_t.Add(1, value);
				_t[1]=value;
			}
		}

		static HotkeyTriggers()
		{
			//System.Reflection.
			Assembly assembly = Assembly.GetEntryAssembly();

			var methods = assembly.GetTypes()
						.SelectMany(t => t.GetMethods())
						.Where(m => m.GetCustomAttributes(typeof(Trigger.HotkeyAttribute), false).Length > 0)
						.ToArray();
			foreach(MethodInfo m in methods) {
				Out($"\t{m.Name}");
				foreach(var a in (Trigger.HotkeyAttribute[])m.GetCustomAttributes(typeof(Trigger.HotkeyAttribute), false)) {
					Out($"\t\t{a.hotkey}");
					Trigger.Hotkey[a.hotkey]=(Target)m.CreateDelegate(typeof(Target));
					//Trigger.Hotkey[a.hotkey] = (Target)Delegate.CreateDelegate(typeof(Target), m);
				}
			}

			//foreach(Type t in k.GetTypes())
			//{
			//	Out(t.Name);
			//	foreach(MethodInfo m in t.GetMethods())
			//	{
			//	foreach(CustomAttributeData d in m.CustomAttributes)
			//	{
			//		Out(
			//		Attribute[] attrs = m.CustomAttributes Attribute
			//		Out($"\t{m.Name}");
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
