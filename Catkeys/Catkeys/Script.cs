using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

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
		public int speed
		{
			get { return _speed; }
			set { if(value < 0 || value > 60000) throw new ArgumentOutOfRangeException(); _speed = value; }
		}
		int _speed;
		//CONSIDER: use two speeds - function-specific and thread-specific.
		//To implement function-specific speed, can use thread-specific Dictionary. When a function sets speed, write to the dictionary: function id, its position in stack, its speed.
		//Then called functions can access caller's speed by walking the stack.
		//Alternative implementation: set function's speed in its attributes.

		/// <summary>
		/// If dialog owner window not specified, let the dialog be always on top of most other windows.
		/// Used by Show.TaskDialog and similar functions.
		/// </summary>
		public bool dialogTopmostIfNoOwner { get; set; }

		/// <summary>
		/// Dialog right-to-left layout.
		/// Used by Show.TaskDialog and similar functions.
		/// </summary>
		public bool dialogRtlLayout { get; set; }

		/// <summary>
		/// Dialog screen.
		/// Used by Show.TaskDialog and similar functions.
		/// </summary>
		public object dialogScreenIfNoOwner { get; set; }

		public bool slowMouse { get; set; }
		public bool slowKeys { get; set; }
		public bool waitMsg { get; set; }

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
			if(o == null) { speed = 100; return; }
			speed = o.speed;
			slowMouse = o.slowMouse; slowKeys = o.slowKeys; waitMsg = o.waitMsg;
			dialogTopmostIfNoOwner = o.dialogTopmostIfNoOwner;
			dialogRtlLayout = o.dialogRtlLayout;
			dialogScreenIfNoOwner = o.dialogScreenIfNoOwner;
		}

		/// <summary>
		/// Creates new object with all the same option values.
		/// Code <c>var o2=o1.Copy();</c> does the same as <c>var o2=new ScriptOptions(o1);</c>.
		/// </summary>
		public ScriptOptions Copy() { return new ScriptOptions(this); }

		/// <summary>
		/// Default options used by:
		/// 	All threads of current script's appdomain;
		///		New ScriptOptions objects created like var o=new ScriptOptions();.
		/// Initially default speed is 100, other options false/0/null.
		/// You can modify them in scripts and script templates. Do it in ScriptClass static constructor.
		/// <example>
		///	<code>
		/// 	static ScriptClass() { ScriptOptions.Default.speed=50; } //constructor
		///		...
		///		Out(Option.speed); //speed of this thread
		///		Option.speed=10; //changes only for this thread
		/// </code>
		/// </example>
		/// </summary>
		public static ScriptOptions Default { get; set; } = new ScriptOptions(null);

		//Equals() - don't need.

		//CONSIDER:
		//public override string ToString()
		//{
		//	return $"(speed={speed}, slowMouse={slowMouse}, slowKeys={slowKeys}, waitMsg={waitMsg}, ...)";
		//}

		/// <summary>
		/// Default title text for standard dialogs (function Show.MessageDialog() etc). Also can be displayed in other places.
		/// Default value - current appdomain name. In exe it is exe file name like "example.exe", else it is script name.
		/// </summary>
		public static string DisplayName
		{
			get { return _DisplayName ?? AppDomain.CurrentDomain.FriendlyName; }
			set { _DisplayName = value; }
		}
		static string _DisplayName;
		//consider: use [assembly: AssemblyTitle("...")]. var a=Assembly.GetEntryAssembly(); But exception if appdomain runs with DoCallBack().
	}

	/// <summary>
	/// Base class of user main script class. Manages script options, calling script methods on launch/trigger, etc.
	/// </summary>
	[DebuggerStepThrough]
	public class Script
	{
		[ThreadStatic]
		static ScriptOptions _opt;

		/// <summary>
		/// Gets ScriptOptions object of this thread.
		/// </summary>
		public static ScriptOptions Option { get { return _opt ?? (_opt = new ScriptOptions()); } }

		/// <summary>
		/// Gets or sets Option.speed (auto-delay and speed for some automation functions, for this thread).
		/// </summary>
		public static int Speed
		{
			get { var t = _opt; return t != null ? t.speed : ScriptOptions.Default.speed; }
			set { Option.speed = value; }
		}

		public void CallFirstMethod()
		{
			//Out(this.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance).Length);
			//foreach(var m in this.GetType().GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly)) {
			//	Out(m.Name);
			//}

			var a = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			if(a.Length == 0) { Out("Info: Script code should be in a non-static method. Example:\nvoid Script() { Out(\"test\"); }"); return; }

			_CallMethod(a[0], null);

			//From MSDN: The GetMethods method does not return methods in a particular order, such as alphabetical or declaration order. Your code must not depend on the order in which methods are returned, because that order varies.
			//But in my experience it returns methods in declaration order. At least when che class is in single file.
		}

		/// <summary>
		/// Calls a non-static method of the derived class by string name.
		/// </summary>
		/// <param name="name">Method name. The method must have 0 or 1 parameter.</param>
		public void CallTriggerMethod(string name, object eventData)
		{
			var m = GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			//Out(m);
			if(m == null) { Out($"Error: Method {name} not found."); return; }
			_CallMethod(m, eventData);
			//object[] a=null; if(parameter!=null) a=new object[1] { parameter };
			//m.Invoke(this, a);
		}

		void _CallMethod(MethodInfo m, object arg)
		{
			int n = m.GetParameters().Length;
			if(n != 0 && n != 1) { Out($"Error: Method {m.Name} must have 0 or 1 parameter."); return; }
			try {
				m.Invoke(this, n == 0 ? null : new object[1] { arg });
			} catch(Exception e) {
				Out($"Error: Failed to call method {m.Name}. {e.Message}");
			}
		}
	}
}
