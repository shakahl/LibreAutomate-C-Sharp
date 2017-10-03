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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Options for some often used automation functions.
	/// </summary>
	/// <remarks>
	/// Used like this:
	/// 
	/// 
	/// 
	/// This library has functions
	/// 
	/// 
	/// Usually function options are specified using either function parameters or class properties.
	/// As you know, many functions allow to adjust their behavior through some kind of options (aka settings, flags, parameters). There are two common ways to pass options to a function: 1. Function parameters, like <c>Class.Func(other parameters, option1, option2);</c> 2. Class properties, like <c>var x=new Class(); x.Option1=1; x.Option2=2; x.Func(parameters);</c>.
	/// 
	/// </remarks>
	//[DebuggerStepThrough]
	public class ScriptOptions
	{
		//infor: struct is used to copy easier, like _o = o._o;
		struct _Options
		{
			public ushort
				MouseClickSleep,
				MouseMoveSpeed,
				KeySleep,
				KeySpeed;
			public bool //info: could instead use enum flags. Then smaller object memory, but bigger source code and calling code.
				Relaxed
				//MouseBlockUserInput,
				//KeyBlockUserInput,
				//WaitMsg
				;
			public byte Debug; //0 non-init, 1 false, 2 true
		}
		_Options _o;

		/// <summary>
		/// Copies all options from another ScriptOptions object.
		/// </summary>
		/// <param name="o">If null, sets default options (unchanged ScriptOptions.Default).</param>
		public ScriptOptions(ScriptOptions o)
		{
			if(o == null) {
				_o.MouseClickSleep = 50;
				_o.KeySleep = 50;
			} else {
				_o = o._o;
				if(o.LibDisabledWarnings != null) LibDisabledWarnings = new List<string>(o.LibDisabledWarnings);
			}
		}

		/// <summary>
		/// Copies all options from <see cref="Default"/>.
		/// </summary>
		public ScriptOptions() : this(Default) { }

		/// <summary>
		/// Initial options used for: <see cref="Options"/> of all threads of this appdomain; new ScriptOptions objects created like <c>var o=new ScriptOptions();</c>.
		/// </summary>
		/// <remarks>
		/// Why there are two static ScriptOptions objects - Options and Default:
		/// Options - thread-scoped object. Functions of this library use it. You can change its properties in scripts where need. Initial property values are automatically copied from Default when the Options property is accessed first time in that thread.
		/// Default - appdomain-scoped object. Contains initial property values for Options and new ScriptOptions objects. Normally you change its properties in script template, at the start of script execution.
		/// </remarks>
		public static ScriptOptions Default { get; } = new ScriptOptions(null);

		/// <summary>
		/// Options of this thread.
		/// You can set and get its properties. Functions of this library use it.
		/// Initially it is a clone of <see cref="Default"/>.
		/// </summary>
		public static ScriptOptions Options
		{
			get => t_opt ?? (t_opt = new ScriptOptions());
			internal set => t_opt = value; //CONSIDER: public
		}
		[ThreadStatic] static ScriptOptions t_opt;

		/// <summary>
		/// Wait milliseconds at the end of Mouse.Click() and other functions that generate mouse button or wheel events.
		/// Default: 50. Valid values: 0 - 60000 (1 minute).
		/// </summary>
		/// <remarks>
		/// This option also is applied to:
		/// The sleep time after mouse move events is Math.Min(7, MouseClickSleep).
		/// The sleep time between mouse button down and up events is Math.Min(20, MouseClickSleep).
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int MouseClickSleep
		{
			get => _o.MouseClickSleep;
			set { if((uint)value > 60000) throw new ArgumentOutOfRangeException(null, "0-60000"); _o.MouseClickSleep = (ushort)value; }
		}

		/// <summary>
		/// If not 0, makes mouse movements slower, not instant.
		/// Used by Mouse.Move, Mouse.Click and other functions that generate mouse movement events.
		/// Default: 0. Valid values: 0 (instant) - 60000 (slowest).
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// It is not milliseconds or some other unit. It adds intermediate mouse movements and small delays when moving the mouse cursor to the specified point. The speed also depends on the distance.
		/// Value 0 (default) does not add intermediate mouse movements. Adds at least 1 if some mouse buttons are pressed.
		/// Value 1 adds at least 1 intermediate mouse movement.
		/// Values 10-50 are good for visually slow movements.
		/// </remarks>
		public int MouseMoveSpeed
		{
			get => _o.MouseMoveSpeed;
			set { if((uint)value > 60000) throw new ArgumentOutOfRangeException(null, "0-60000"); _o.MouseMoveSpeed = (ushort)value; }
		}


		/// <summary>
		/// Wait milliseconds at the end of Input.Key() and other functions that generate keyboard events.
		/// Default: 50. Valid values: 0 - 60000 (1 minute).
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int KeySleep
		{
			get => _o.KeySleep;
			set { if((uint)value > 60000) throw new ArgumentOutOfRangeException(null, "0-60000"); _o.KeySleep = (ushort)value; }
		}

		/// <summary>
		/// Wait milliseconds after each key down and up event in Input.Key() and other functions that generate keyboard events.
		/// Default: 0. Valid values: 0 - 60000.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// If 0 (default), just sleeps briefly after some keys.
		/// </remarks>
		public int KeySpeed
		{
			get => _o.KeySpeed;
			set { if((uint)value > 60000) throw new ArgumentOutOfRangeException(null, "0-60000"); _o.KeySpeed = (ushort)value; }
		}

		/// <summary>
		/// Make some functions less strict (less checking for possibly invalid conditions).
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// This option is used by these functions:
		/// <list type="bullet">
		/// <item>Mouse.Move, Mouse.Click and other functions that move the cursor (mouse pointer):
		/// false - throw exception if cannot move the cursor to the specified x y. For example it the x y is not in screen.
		/// true - try to move anyway. Don't throw exception, regardless of the final cursor position (which probably will be at a screen edge).
		/// </item>
		/// <item>Mouse.Move, Mouse.Click and other functions that move the cursor (mouse pointer):
		/// false - before moving the cursor, wait while a mouse button is pressed by the user or another thread. It prevents an unintended drag-drop.
		/// true - do not wait.
		/// </item>
		/// <item>Mouse.Click and other functions that click or press a mouse button using window coordinates:
		/// false - don't allow to click in another window. If need, activate the specified window (or its top-level parent). If that does not help, throw exception. However if the window is a control, allow x y anywhere in its top-level parent window.
		/// true - allow to click in another window. Don't activate the window and don't throw exception.
		/// </item>
		/// <item></item>
		/// <item></item>
		/// <item></item>
		/// </list>
		/// </remarks>
		public bool Relaxed { get => _o.Relaxed; set => _o.Relaxed = value; }

		//rejected, at least for now. Better let block input explicitly.
		///// <summary>
		///// Block user mouse and keyboard input in <see cref="Mouse"/> class functions (when sending mouse movements and clicks).
		///// </summary>
		///// <remarks>
		///// Uses API <msdn>BlockInput</msdn>. Unblocks when the called function ends or when the user presses Ctrl+Alt+Delete.
		///// </remarks>
		//public bool MouseBlockUserInput { get => _o.MouseBlockUserInput; set => _o.MouseBlockUserInput = value; }

		///// <summary>
		///// Block user mouse and keyboard input in <see cref="Input"/> class functions (when sending keys).
		///// </summary>
		///// <remarks>
		///// Uses API <msdn>BlockInput</msdn>. Unblocks when the called function ends or when the user presses Ctrl+Alt+Delete.
		///// </remarks>
		//public bool KeyBlockUserInput { get => _o.KeyBlockUserInput; set => _o.KeyBlockUserInput = value; }

		//public bool WaitMsg { get => _o.WaitMsg; set => _o.WaitMsg = value; }
		//TODO: implement or remove.

		/// <summary>
		/// If true, some library functions may display some debug info that is not displayed if this is false.
		/// If not explicitly set, the default value depends on the configuration of the entry assymbly: true if Debug, false if Release.
		/// </summary>
		public bool Debug
		{
			get
			{
				if(_o.Debug == 0) Debug = _IsAppDebugConfig();
				return _o.Debug == 2;
			}
			set
			{
				_o.Debug = (byte)(value ? 2 : 1);
			}
		}

		static bool _IsAppDebugConfig()
		{
			var a = Util.Assembly_.EntryAssembly.GetCustomAttribute<DebuggableAttribute>();
			if(a == null) return false;
			//return a.IsJITOptimizerDisabled; //depends on 'Optimize code' checkbox in project Properties, regardless of config
			return a.IsJITTrackingEnabled; //depends on config, but not 100% reliable, eg may be changed explicitly in source code (maybe the above too)
		}

		/// <summary>
		/// Disables one or more warnings.
		/// </summary>
		/// <param name="warningsWild">One or more warnings as case-insensitive wildcard strings. See <see cref="String_.Like_(string, string, bool)"/>.</param>
		/// <remarks>
		/// Adds the strings to an internal list. When <see cref="Output.Warning"/> is called, it looks in the list. If it finds the warning in the list, it does not show the warning.
		/// It's easy to auto-restore warnings with 'using', like in the second example. Restoring is optional.
		/// </remarks>
		/// <example>
		/// This code in script template disables two warnings for all new scripts.
		/// <code><![CDATA[
		/// ScriptOptions.Default.DisableWarnings("*part of warning 1 text*", "*part of warning 2 text*");
		/// ]]></code>
		/// Temporarily disable all warnings for this thread.
		/// <code><![CDATA[
		/// using(Options.DisableWarnings("*")) {
		/// 	...
		/// } //here warnings are automatically restored
		/// ]]></code>
		/// </example>
		public SORestoreWarnings DisableWarnings(params string[] warningsWild)
		{
			if(LibDisabledWarnings == null) LibDisabledWarnings = new List<string>();
			var r = new SORestoreWarnings(LibDisabledWarnings.Count);
			LibDisabledWarnings.AddRange(warningsWild);
			return r;
		}

		internal List<string> LibDisabledWarnings;

		/// <summary>
		/// Saves current Options so that it will be automatically restored like in the example.
		/// </summary>
		/// <remarks>
		/// Multiple <c>using(ScriptOptions.Temp)</c> can be nested.
		/// Note: Each thread has its own Options. This function saves/restores Options of this thread.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Print(Options.MouseClickSleep);
		/// using(ScriptOptions.Temp) {
		/// 	Options.MouseClickSleep = 1000;
		/// 	Print(Options.MouseClickSleep);
		/// } //here Options is automatically restored
		/// Print(Options.MouseClickSleep);
		/// ]]></code>
		/// </example>
		public static SORestore Temp { get => new SORestore(ref t_opt); }
	}
}

namespace Catkeys.Types
{
	/// <summary>Infrastructure.</summary>
	/// <tocexclude />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct SORestore :IDisposable
	{
		bool _disposed;
		[ThreadStatic] static Stack<ScriptOptions> t_savedOptions;

		internal SORestore(ref ScriptOptions t_opt)
		{
			_disposed = false;
			if(t_savedOptions == null) t_savedOptions = new Stack<ScriptOptions>();
			t_savedOptions.Push(t_opt);
			if(t_opt != null) t_opt = new ScriptOptions(t_opt);
		}

		/// <summary>
		/// Restores Options saved by <see cref="ScriptOptions.Temp"/>.
		/// </summary>
		public void Dispose()
		{
			if(!_disposed) {
				_disposed = true;
				ScriptOptions.Options = t_savedOptions.Pop();
			}
		}
	}

	/// <summary>Infrastructure.</summary>
	/// <tocexclude />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct SORestoreWarnings :IDisposable
	{
		int _listCount;

		internal SORestoreWarnings(int listCount) { _listCount = listCount; }

		/// <summary>
		/// Restores warnings disabled by <see cref="ScriptOptions.DisableWarnings"/>.
		/// </summary>
		public void Dispose()
		{
			var a = Options.LibDisabledWarnings;
			int n = a.Count - _listCount;
			if(n > 0) a.RemoveRange(_listCount, n);
		}
	}
}
