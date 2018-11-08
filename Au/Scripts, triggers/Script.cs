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
//using System.Linq;

using Au.Types;
using static Au.NoClass;

//CONSIDER: add an option to inject and execute the script in any process/thread.
//	[assembly: Inject("firefox.exe", windowName="* - Firefox")]

//TODO: remove what is not used. Edit class xmldoc.
namespace Au
{
	/// <summary>
	/// 
	/// </summary>
	//[DebuggerStepThrough]
	public static class Script //TODO: AutomationTask
	{
		/// <summary>
		/// Gets current automation script/app name or <see cref="AppDomain.FriendlyName"/>.
		/// </summary>
		public static string Name {
			get {
				if(s_name != null) return s_name;
				if(t_name != null) return t_name;
				var s = Thread.CurrentThread.Name;
				if(s != null && s.StartsWith_("[script] ")) return t_name = s.Substring(9);
				return s_name = AppDomain.CurrentDomain.FriendlyName;
			}
			internal set {
				s_name = value;
			}
		}
		static string s_name;
		[ThreadStatic] static string t_name;

		///// <summary>
		///// Calls the first non-static method of the derived class.
		///// The method must have 0 parameters.
		///// </summary>
		//public void CallFirstMethod()
		//{
		//	//Print(this.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Length);
		//	//foreach(var m in this.GetType().GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly)) {
		//	//	Print(m.Name);
		//	//}

		//	var a = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		//	if(a.Length == 0) { Print("Info: Script code should be in a non-static method. Example:\nvoid Script() { Print(\"test\"); }"); return; }

		//	_CallMethod(a[0], null);

		//	//From MSDN: The GetMethods method does not return methods in a particular order, such as alphabetical or declaration order. Your code must not depend on the order in which methods are returned, because that order varies.
		//	//But in my experience it returns methods in declaration order. At least when che class is in single file.
		//}

		///// <summary>
		///// Calls a non-static method of the derived class by name.
		///// </summary>
		///// <param name="name">Method name. The method must have 0 or 1 parameter.</param>
		///// <param name="eventData">An argument.</param>
		//public void CallTriggerMethod(string name, object eventData)
		//{
		//	var m = GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		//	//Print(m);
		//	if(m == null) { Print($"Error: Method {name} not found."); return; }
		//	_CallMethod(m, eventData);
		//	//object[] a=null; if(parameter!=null) a=new object[1] { parameter };
		//	//m.Invoke(this, a);
		//}

		//void _CallMethod(MethodInfo m, object arg)
		//{
		//	int n = m.GetParameters().Length;
		//	if(n != 0 && n != 1) { Print($"Error: Method {m.Name} must have 0 or 1 parameter."); return; }
		//	try {
		//		m.Invoke(this, n == 0 ? null : new object[1] { arg });
		//	}
		//	catch(Exception e) {
		//		Print($"Error: Failed to call method {m.Name}. {e.Message}");
		//	}
		//}


		public static void Run(string script, params string[] args)
		{
			var w = WndMsg; if(w.Is0) throw new AuException("Au editor not found."); //FUTURE: run program, if installed
			var data = Util.LibSerializer.Serialize(script, args);
			switch((long)Wnd.Misc.InterProcessSendData(w, 100, data)) {
			case 1: return;
			case 2: throw new FileNotFoundException($"Script '{script}' not found.");
			default: throw new AuException("Failed to start script.");
			}
		}

		internal static Wnd WndMsg {
			get {
				if(!s_wndMsg.IsAlive) {
					s_wndMsg = Wnd.FindFast(null, "Au.Editor.Msg");
				}
				return s_wndMsg;
			}
		}
		static Wnd s_wndMsg;
	}
}
