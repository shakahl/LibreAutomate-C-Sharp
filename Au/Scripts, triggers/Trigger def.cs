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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	/// <summary>
	/// Triggers.
	/// </summary>
	[CompilerGenerated()]
	class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	public static class Trigger
	{
		//public static readonly HotkeyTriggers Hotkey = new HotkeyTriggers();

		[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
		public class HotkeyAttribute :Attribute
		{
			public string Hotkey { get; }
			public bool PassToApp { get; set; }
			public bool WhenReleased { get; set; }

			public HotkeyAttribute(string hotkey)
			{
				Hotkey = hotkey;
			}
		}

		public class HotkeyData { }

		[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
		public class MouseClickAttribute :Attribute
		{
			public string Moo { get; }

			public MouseClickAttribute(string moo)
			{
				Moo = moo;
			}
		}

		[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
		public class MouseWheelAttribute :Attribute
		{
			public string Moo { get; }

			public MouseWheelAttribute(string moo)
			{
				Moo = moo;
			}
		}

		[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
		public class MouseScreenEdgeAttribute :Attribute
		{
			public string Moo { get; }

			public MouseScreenEdgeAttribute(string moo)
			{
				Moo = moo;
			}
		}

		[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
		public class MouseMoveAttribute :Attribute
		{
			public string Moo { get; }

			public MouseMoveAttribute(string moo)
			{
				Moo = moo;
			}
		}

		static List<MethodInfo> _methods;

		/// <summary>
		/// Waits for trigger events specified in function attributes like <c>[Trigger.Hotkey("Ctrl+K")]</c>. On events calls that functions and continues to wait.
		/// </summary>
		/// <param name="app">Object instance for calling non-static functions. Can be typeof(Class) if all functions are static. Only functions of that class will be called.</param>
		public static void Run(object app)
		{
			bool hasTriggers = false;
			if(app is Type ty) app = null; else ty = app?.GetType() ?? throw new ArgumentNullException();
			_methods = new List<MethodInfo>();
			foreach(var m in ty.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
				//var a = m.GetCustomAttributes(typeof(HotkeyAttribute), false);
				//if(a.Length == 0) continue;
				foreach(var a in m.GetCustomAttributes(false)) {
					switch(a) {
					case HotkeyAttribute hk:
						if(_IsGood(typeof(HotkeyData))) Print(a.GetType().Name, hk.Hotkey, m, m.ReturnType==typeof(bool));
						break;
					}
				}

				bool _IsGood(Type pt)
				{
					var fn = m.Name; if(fn == "Main" || fn == "_Main") {
						PrintWarning($"Function {fn} cannot have a trigger. Create new function below it and add the trigger attribute to it.", -1);
						return false;
					}
					bool good = false;
					var pi = m.GetParameters();
					switch(pi.Length) {
					case 0: good = true; break;
					case 1: good = pi[0].ParameterType == pt; break;
					}
					if(!good) {
						PrintWarning($"Function {fn}: with this trigger need 0 parameters or 1 parameter of type {pt.DeclaringType.Name}.{pt.Name}.", -1);
						return false;
					}
					if(app == null && !m.IsStatic) {
						PrintWarning($"Non-static function {fn} cannot have a trigger because Trigger.Run was called without an object.", -1);
						return false;
					}
					hasTriggers = true;
					_methods.Add(m);
					return true;
				}
			}
			if(!hasTriggers) return;

			AuDialog.Show();
		}

		//struct _Method
		//{
		//	public MethodInfo mi;

		//}
	}
}
