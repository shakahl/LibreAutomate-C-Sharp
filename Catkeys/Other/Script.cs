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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Stores script speed and other options common to multiple automation library methods and script statements.
	/// </summary>
	[DebuggerStepThrough]
	public class ScriptOptions
	{
		/// <summary>
		/// Auto-delay in milliseconds.
		/// Used by some automation functions.
		/// </summary>
		public int Speed
		{
			get => _speed;
			set { if(value < 0 || value > 60000) throw new ArgumentOutOfRangeException(); _speed = value; }
		}
		int _speed;
		//CONSIDER: use two speeds - function-specific and thread-specific.
		//To implement function-specific speed, can use thread-specific Dictionary. When a function sets speed, write to the dictionary: function id, its position in stack, its speed.
		//Then called functions can access caller's speed by walking the stack.
		//Alternative implementation: set function's speed in its attributes.

		/// <summary>
		/// TODO
		/// </summary>
		public bool SlowMouse { get; set; }
		/// <summary>
		/// TODO
		/// </summary>
		public bool SlowKeys { get; set; }
		/// <summary>
		/// TODO
		/// </summary>
		public bool WaitMsg { get; set; }

		/// <summary>
		/// Copies all options from ScriptOptions.Default.
		/// </summary>
		public ScriptOptions() : this(Default) { }

		/// <summary>
		/// Copies all options from another ScriptOptions object.
		/// </summary>
		/// <param name="o">If null, sets speed = 100 and other members = false/0/null.</param>
		public ScriptOptions(ScriptOptions o)
		{
			if(o == null) { Speed = 100; return; }
			Speed = o.Speed;
			SlowMouse = o.SlowMouse; SlowKeys = o.SlowKeys; WaitMsg = o.WaitMsg;
		}

		/// <summary>
		/// Creates new object with all the same option values.
		/// Code <c>var o2=o1.Copy();</c> does the same as <c>var o2=new ScriptOptions(o1);</c> .
		/// </summary>
		public ScriptOptions Copy() { return new ScriptOptions(this); }

		/// <summary>
		/// Default options used by:
		/// 	All threads of current script's appdomain;
		///		New ScriptOptions objects created like var o=new ScriptOptions();.
		/// Initially default speed is 100, other options false/0/null.
		/// You can modify them in scripts and script templates. Do it in ScriptClass static constructor.
		/// </summary>
		/// <example>
		/// <code>
		/// 	static ScriptClass() { ScriptOptions.Default.speed=50; } //constructor
		///		...
		///		Print(Option.speed); //speed of this thread
		///		Option.speed=10; //changes only for this thread
		/// </code>
		/// </example>
		public static ScriptOptions Default { get; set; } = new ScriptOptions(null);

		//Equals() - don't need.

		//CONSIDER:
		//public override string ToString()
		//{
		//	return $"(speed={speed}, SlowMouse={SlowMouse}, SlowKeys={SlowKeys}, WaitMsg={WaitMsg}, ...)";
		//}
	}

	/// <summary>
	/// Base class of user main script class. Manages script options, calling script methods on launch/trigger, etc.
	/// </summary>
	[DebuggerStepThrough]
	public class Script
	{
		[ThreadStatic]		static ScriptOptions _opt;

		/// <summary>
		/// Gets ScriptOptions object of this thread.
		/// </summary>
		public static ScriptOptions Option { get => _opt ?? (_opt = new ScriptOptions()); }

		/// <summary>
		/// Gets or sets Option.speed (auto-delay and speed for some automation functions, for this thread).
		/// </summary>
		public static int Speed
		{
			get { var t = _opt; return t != null ? t.Speed : ScriptOptions.Default.Speed; }
			set { Option.Speed = value; }
		}

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
