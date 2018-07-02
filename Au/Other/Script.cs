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

namespace Au
{
	/// <summary>
	/// Base class of user main script class. Manages script options, calling script methods on launch/trigger, etc.
	/// </summary>
	[DebuggerStepThrough]
	public class Script
	{
		/// <summary>
		/// Calls the first non-static method of the derived class.
		/// The method must have 0 parameters.
		/// </summary>
		public void CallFirstMethod()
		{
			//Print(this.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Length);
			//foreach(var m in this.GetType().GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly)) {
			//	Print(m.Name);
			//}

			var a = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			if(a.Length == 0) { Print("Info: Script code should be in a non-static method. Example:\nvoid Script() { Print(\"test\"); }"); return; }

			_CallMethod(a[0], null);

			//From MSDN: The GetMethods method does not return methods in a particular order, such as alphabetical or declaration order. Your code must not depend on the order in which methods are returned, because that order varies.
			//But in my experience it returns methods in declaration order. At least when che class is in single file.
		}

		/// <summary>
		/// Calls a non-static method of the derived class by name.
		/// </summary>
		/// <param name="name">Method name. The method must have 0 or 1 parameter.</param>
		/// <param name="eventData">An argument.</param>
		public void CallTriggerMethod(string name, object eventData)
		{
			var m = GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			//Print(m);
			if(m == null) { Print($"Error: Method {name} not found."); return; }
			_CallMethod(m, eventData);
			//object[] a=null; if(parameter!=null) a=new object[1] { parameter };
			//m.Invoke(this, a);
		}

		void _CallMethod(MethodInfo m, object arg)
		{
			int n = m.GetParameters().Length;
			if(n != 0 && n != 1) { Print($"Error: Method {m.Name} must have 0 or 1 parameter."); return; }
			try {
				m.Invoke(this, n == 0 ? null : new object[1] { arg });
			}
			catch(Exception e) {
				Print($"Error: Failed to call method {m.Name}. {e.Message}");
			}
		}
	}
}
