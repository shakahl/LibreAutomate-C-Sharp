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
//using System.Linq;

using Au.Types;

namespace Au
{
	/// <summary>
	/// Options for some functions of this library.
	/// </summary>
	/// <remarks>
	/// Some frequently used static functions of this library have some options (settings). For example <see cref="AKeys.Key"/> allows to change speed, text sending method, etc. Passing options as parameters in each call usually isn't what you want to do in automation scripts. Instead you can set options using static properties. This class contains several groups of options for functions of various classes. See examples.
	/// 
	/// There are two sets of identical or similar options - in class <b>AOpt</b> and in class <see cref="AOpt.Static"/>:
	/// - <b>AOpt</b> - thread-static options (each thread has its own instance). Functions of this library use them. You can change or change-restore them anywhere in script. Initial options are automatically copied from <b>AOpt.Static</b> when that group of options (<b>Key</b>, <b>Mouse</b>, etc) is used first time in that thread (explicitly or by library functions).
	/// - <b>AOpt.Static</b> - static options. Contains initial property values for <b>AOpt</b>. Normally you change them at the very start of script. Don't change later, it's not thread-safe.
	/// </remarks>
	public static class AOpt
	{
		/// <summary>
		/// Options for keyboard and clipboard functions (classes <see cref="AKeys"/>, <see cref="AClipboard"/> and functions that use them).
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>AOpt.Key</b> instance. It inherits options from <see cref="AOpt.Static.Key"/>.
		/// Also can be used when creating <see cref="AKeys"/> instances. See the second example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// AOpt.Key.KeySpeed = 100;
		/// AKeys.Key("Right*10 Ctrl+A");
		/// ]]></code>
		/// Use an AKeys instance.
		/// <code><![CDATA[
		/// var k = new AKeys(AOpt.Key); //create new AKeys instance and copy options from AOpt.Key to it
		/// k.Options.KeySpeed = 100; //changes option of k but not of AOpt.Key
		/// k.Add("Right*10 Ctrl+A").Send(); //uses options of k
		/// ]]></code>
		/// </example>
		public static OptKey Key => t_key ??= new OptKey(AOpt.Static.Key);
		[ThreadStatic] static OptKey t_key;

		/// <summary>
		/// Options for mouse functions (class <see cref="AMouse"/> and functions that use it).
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>AOpt.Mouse</b> instance. It inherits options from <see cref="AOpt.Static.Mouse"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// AOpt.Mouse.ClickSpeed = 100;
		/// AMouse.Click();
		/// ]]></code>
		/// </example>
		public static OptMouse Mouse => t_mouse ??= new OptMouse(AOpt.Static.Mouse);
		[ThreadStatic] static OptMouse t_mouse;

		/// <summary>
		/// Options for 'wait for' functions.
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>AOpt.WaitFor</b> instance. There is no <b>AOpt.Static.WaitFor</b>.
		/// Most 'wait for' functions of this library use these options. Functions of .NET classes don't.
		/// </remarks>
		public static OptWaitFor WaitFor => t_waitFor ??= new OptWaitFor();
		[ThreadStatic] static OptWaitFor t_waitFor;

		/// <summary>
		/// Options for showing run-time warnings and other info that can be useful to find problems in code at run time.
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>AOpt.Warnings</b> instance. It inherits options from <see cref="AOpt.Static.Warnings"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// AOpt.Warnings.Verbose = false;
		/// AWarning.Write("Example");
		/// AWarning.Write("Example");
		/// ]]></code>
		/// </example>
		public static OptWarnings Warnings => t_warnings ??= new OptWarnings(AOpt.Static.Warnings);
		[ThreadStatic] static OptWarnings t_warnings;

		//rejected. Use AOpt.Scope.
		///// <summary>
		///// Resets all options. Copies from <see cref="AOpt.Static"/>.
		///// </summary>
		//public static void Reset()
		//{
		//	t_key?.Reset();
		//	t_mouse?.Reset();
		//	t_waitFor?.Reset();
		//	t_warnings?.Reset();
		//}

		/// <summary>
		/// Default <see cref="AOpt"/> properties of each thread.
		/// </summary>
		/// <remarks>
		/// You can change these options at the start of your script/program. Don't change later.
		/// </remarks>
		public static class Static
		{
			/// <summary>
			/// Default option values for <see cref="AOpt.Key"/> of each thread.
			/// </summary>
			/// <remarks>
			/// Also can be used when creating <see cref="AKeys"/> instances. See the second example.
			/// </remarks>
			/// <example>
			/// <code><![CDATA[
			/// AOpt.Static.Key.KeySpeed = 10;
			/// ...
			/// AKeys.Key("Tab Ctrl+V"); //uses AOpt.Key, which is implicitly copied from AOpt.Static.Key
			/// ]]></code>
			/// Use an AKeys instance.
			/// <code><![CDATA[
			/// var k = new AKeys(AOpt.Static.Key); //create new AKeys instance and copy options from AOpt.Static.Key to it
			/// k.Options.KeySpeed = 100; //changes option of k but not of AOpt.Static.Key
			/// k.Add("Tab Ctrl+V").Send(); //uses options of k
			/// ]]></code>
			/// </example>
			public static OptKey Key { get; } = new OptKey();

			/// <summary>
			/// Default option values for <see cref="AOpt.Mouse"/> of each thread.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// AOpt.Static.Mouse.ClickSpeed = 10;
			/// ...
			/// AMouse.Click(); //uses AOpt.Mouse, which is implicitly copied from AOpt.Static.Mouse
			/// ]]></code>
			/// </example>
			public static OptMouse Mouse { get; } = new OptMouse();

			/// <summary>
			/// Default option values for <see cref="AOpt.Warnings"/> of each thread.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// AOpt.Static.Warnings.Verbose = false;
			/// ]]></code>
			/// </example>
			public static OptWarnings Warnings { get; } = new OptWarnings();

		}

		/// <summary>
		/// Creates temporary scopes for options.
		/// Example: <c>using(AOpt.Scope.Key()) { AOpt.Key.KeySpeed=5; ... }</c>.
		/// </summary>
		public static class Scope
		{
			/// <summary>
			/// Creates temporary scope for <see cref="AOpt.Mouse"/> options. See example.
			/// </summary>
			/// <param name="inherit">If true (default), inherit current options. If false, inherit default options (<see cref="AOpt.Static"/>).</param>
			/// <example>
			/// <code><![CDATA[
			/// AOutput.Write(AOpt.Mouse.ClickSpeed);
			/// using(AOpt.Scope.Mouse()) {
			/// 	AOpt.Mouse.ClickSpeed = 100;
			/// 	AOutput.Write(AOpt.Mouse.ClickSpeed);
			/// } //here restored automatically
			/// AOutput.Write(AOpt.Mouse.ClickSpeed);
			/// ]]></code>
			/// </example>
			public static UsingEndAction Mouse(bool inherit = true)
			{
				var old = _Mouse(inherit);
				return new UsingEndAction(() => t_mouse = old);
			}

			static OptMouse _Mouse(bool inherit)
			{
				var old = t_mouse;
				//t_mouse = new OptMouse((old != null && inherit) ? old : Static.Mouse);
				t_mouse = (old != null && inherit) ? new OptMouse(old) : null; //lazy
				return old;
			}

			/// <summary>
			/// Creates temporary scope for <see cref="AOpt.Key"/> options. See example.
			/// </summary>
			/// <param name="inherit">If true (default), inherit current options. If false, inherit default options (<see cref="AOpt.Static"/>).</param>
			/// <example>
			/// <code><![CDATA[
			/// AOutput.Write(AOpt.Key.KeySpeed);
			/// using(AOpt.Scope.Key()) {
			/// 	AOpt.Key.KeySpeed = 5;
			/// 	AOutput.Write(AOpt.Key.KeySpeed);
			/// } //here restored automatically
			/// AOutput.Write(AOpt.Key.KeySpeed);
			/// ]]></code>
			/// </example>
			public static UsingEndAction Key(bool inherit = true)
			{
				var old = _Key(inherit);
				return new UsingEndAction(() => t_key = old);
			}

			static OptKey _Key(bool inherit)
			{
				var old = t_key;
				//t_key = new OptKey((old != null && inherit) ? old : Static.Key);
				t_key = (old != null && inherit) ? new OptKey(old) : null; //lazy
				return old;
			}

			/// <summary>
			/// Creates temporary scope for <see cref="AOpt.WaitFor"/> options. See example.
			/// </summary>
			/// <param name="inherit">If true (default), inherit current options. If false, inherit default options (<see cref="AOpt.Static"/>).</param>
			/// <example>
			/// <code><![CDATA[
			/// AOutput.Write(AOpt.WaitFor.Period);
			/// using(AOpt.Scope.WaitFor()) {
			/// 	AOpt.WaitFor.Period = 5;
			/// 	AOutput.Write(AOpt.WaitFor.Period);
			/// } //here restored automatically
			/// AOutput.Write(AOpt.WaitFor.Period);
			/// ]]></code>
			/// </example>
			public static UsingEndAction WaitFor(bool inherit = true)
			{
				var old = _WaitFor(inherit);
				return new UsingEndAction(() => t_waitFor = old);
			}

			static OptWaitFor _WaitFor(bool inherit)
			{
				var old = t_waitFor;
				//t_waitFor = (old != null && inherit) ? new OptWaitFor(old.Period, old.DoEvents) : new OptWaitFor();
				t_waitFor = (old != null && inherit) ? new OptWaitFor(old.Period, old.DoEvents) : null; //lazy
				return old;
			}

			/// <summary>
			/// Creates temporary scope for <see cref="AOpt.Warnings"/> options. See example.
			/// </summary>
			/// <param name="inherit">If true (default), inherit current options. If false, inherit default options (<see cref="AOpt.Static"/>).</param>
			/// <example>
			/// <code><![CDATA[
			/// AOpt.Warnings.Verbose = false;
			/// AOutput.Write(AOpt.Warnings.Verbose, AOpt.Warnings.IsDisabled("Test*"));
			/// using(AOpt.Scope.Warnings()) {
			/// 	AOpt.Warnings.Verbose = true;
			/// 	AOpt.Warnings.Disable("Test*");
			/// 	AOutput.Write(AOpt.Warnings.Verbose, AOpt.Warnings.IsDisabled("Test*"));
			/// } //here restored automatically
			/// AOutput.Write(AOpt.Warnings.Verbose, AOpt.Warnings.IsDisabled("Test*"));
			/// ]]></code>
			/// </example>
			public static UsingEndAction Warnings(bool inherit = true)
			{
				var old = _Warnings(inherit);
				return new UsingEndAction(() => t_warnings = old);
			}

			static OptWarnings _Warnings(bool inherit)
			{
				var old = t_warnings;
				//t_warnings = new OptWarnings((old != null && inherit) ? old : Static.Warnings);
				t_warnings = (old != null && inherit) ? new OptWarnings(old) : null; //lazy
				return old;
			}

			/// <summary>
			/// Creates temporary scope for all options. See example.
			/// </summary>
			/// <param name="inherit">If true (default), inherit current options. If false, inherit default options (<see cref="AOpt.Static"/>).</param>
			/// <example>
			/// <code><![CDATA[
			/// AOutput.Write(AOpt.Key.KeySpeed, AOpt.Mouse.ClickSpeed);
			/// using(AOpt.Scope.All()) {
			/// 	AOpt.Key.KeySpeed = 5;
			/// 	AOpt.Mouse.ClickSpeed = 50;
			/// 	AOutput.Write(AOpt.Key.KeySpeed, AOpt.Mouse.ClickSpeed);
			/// } //here restored automatically
			/// AOutput.Write(AOpt.Key.KeySpeed, AOpt.Mouse.ClickSpeed);
			/// ]]></code>
			/// </example>
			public static UsingEndAction All(bool inherit = true)
			{
				var o1 = _Mouse(inherit);
				var o2 = _Key(inherit);
				var o3 = _WaitFor(inherit);
				var o4 = _Warnings(inherit);
				return new UsingEndAction(() => {
					t_mouse = o1;
					t_key = o2;
					t_waitFor = o3;
					t_warnings = o4;
				});
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Options for run-time warnings (<see cref="AWarning.Write"/>).
	/// </summary>
	public class OptWarnings
	{
		bool? _verbose;
		List<string> _disabledWarnings;

		/// <summary>
		/// Initializes this instance with default values or values copied from another instance.
		/// </summary>
		/// <param name="cloneOptions">If not null, copies its options into this variable.</param>
		internal OptWarnings(OptWarnings cloneOptions = null)
		{
			if(cloneOptions != null) {
				_Copy(cloneOptions);
			}
		}

		void _Copy(OptWarnings o)
		{
			_verbose = o._verbose;
			_disabledWarnings = o._disabledWarnings == null ? null : new List<string>(o._disabledWarnings);
		}

		//rejected. Use AOpt.Scope.
		///// <summary>
		///// Resets all options. Copies from <see cref="AOpt.Static.Warnings"/>.
		///// </summary>
		//public void Reset() => _Copy(AOpt.Static.Warnings);

		/// <summary>
		/// If true, some library functions may display more warnings and other info.
		/// If not explicitly set, the default value depends on the build configuration of the entry assymbly: true if Debug, false if Release.
		/// </summary>
		public bool Verbose {
			get => (_verbose ??= _IsAppDebugConfig()) == true;
			set => _verbose = value;
		}

		static bool _IsAppDebugConfig()
		{
			var a = Assembly.GetEntryAssembly().GetCustomAttribute<DebuggableAttribute>();
			if(a == null) return false;
			//return a.IsJITOptimizerDisabled; //depends on 'Optimize code' checkbox in project Properties, regardless of config
			return a.IsJITTrackingEnabled; //depends on config, but not 100% reliable, eg may be changed explicitly in source code (maybe the above too)
		}

		/// <summary>
		/// Disables one or more run-time warnings.
		/// </summary>
		/// <param name="warningsWild">One or more warnings as case-insensitive wildcard strings. See <see cref="AExtString.Like(string, string, bool)"/>.</param>
		/// <remarks>
		/// Adds the strings to an internal list. When <see cref="AWarning.Write"/> is called, it looks in the list. If finds the warning in the list, does not show the warning.
		/// It's easy to auto-restore warnings with 'using', like in the second example. Restoring is optional.
		/// </remarks>
		/// <example>
		/// This code at the very start of script disables two warnings in all threads.
		/// <code><![CDATA[
		/// AOpt.Static.Warnings.Disable("*part of warning 1 text*", "*part of warning 2 text*");
		/// ]]></code>
		/// Temporarily disable all warnings in this thread.
		/// <code><![CDATA[
		/// AOpt.Warnings.Verbose = true;
		/// AWarning.Write("one");
		/// using(AOpt.Warnings.Disable("*")) {
		/// 	AWarning.Write("two");
		/// }
		/// AWarning.Write("three");
		/// ]]></code>
		/// Don't use code <c>using(AOpt.Static.Warnings.Disable...</c>, it's not thread-safe.
		/// </example>
		public UsingEndAction Disable(params string[] warningsWild)
		{
			_disabledWarnings ??= new List<string>();
			int restoreCount = _disabledWarnings.Count;
			_disabledWarnings.AddRange(warningsWild);
			return new UsingEndAction(() => _disabledWarnings.RemoveRange(restoreCount, _disabledWarnings.Count - restoreCount));
		}

		/// <summary>
		/// Returns true if the specified warning text matches a wildcard string added with <see cref="Disable"/>.
		/// </summary>
		/// <param name="text">Warning text. Case-insensitive.</param>
		public bool IsDisabled(string text)
		{
			string s = text ?? "";
			var a = _disabledWarnings;
			if(a != null) foreach(var k in a) if(s.Like(k, true)) return true;
			return false;
		}
	}

	/// <summary>
	/// Options for functions of class <see cref="AMouse"/>.
	/// </summary>
	/// <remarks>
	/// Total <c>Click(x, y)</c> time is: mouse move + <see cref="MoveSleepFinally"/> + button down + <see cref="ClickSpeed"/> + button down + <see cref="ClickSpeed"/> + <see cref="ClickSleepFinally"/>.
	/// </remarks>
	/// <seealso cref="AOpt.Mouse"/>
	/// <seealso cref="AOpt.Static.Mouse"/>
	public class OptMouse
	{
		struct _Options //makes easier to copy and reset fields
		{
			public int ClickSpeed, MoveSpeed, ClickSleepFinally, MoveSleepFinally;
			public bool Relaxed;
		}
		_Options _o;

		/// <summary>
		/// Initializes this instance with default values or values copied from another instance.
		/// </summary>
		/// <param name="cloneOptions">If not null, copies its options into this variable.</param>
		internal OptMouse(OptMouse cloneOptions = null) //don't need public like OptKey
		{
			if(cloneOptions != null) {
				_o = cloneOptions._o;
			} else {
				_o.ClickSpeed = 20;
				//_o.MoveSpeed = 0;
				_o.ClickSleepFinally = 10;
				_o.MoveSleepFinally = 10;
				//_o.Relaxed = false;
			}
		}

		//rejected. Use AOpt.Scope.
		///// <summary>
		///// Resets all options. Copies from <see cref="AOpt.Static.Mouse"/>.
		///// </summary>
		//public void Reset() => _o = AOpt.Static.Mouse._o;

		bool _IsStatic => this == AOpt.Static.Mouse;

		int _SetValue(int value, int max, int maxStatic)
		{
			var m = _IsStatic ? maxStatic : max;
			if((uint)value > m) throw new ArgumentOutOfRangeException(null, "Max " + m.ToString());
			return value;
		}

		/// <summary>
		/// How long to wait (milliseconds) after sending each mouse button down or up event (2 events for click, 4 for double-click).
		/// Default: 20.
		/// </summary>
		/// <value>Valid values: 0 - 1000 (1 s). Valid values for <see cref="AOpt.Static.Mouse"/>: 0 - 100 (1 s).</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int ClickSpeed {
			get => _o.ClickSpeed;
			set => _o.ClickSpeed = _SetValue(value, 1000, 100);
		}

		/// <summary>
		/// If not 0, makes mouse movements slower, not instant.
		/// Default: 0.
		/// </summary>
		/// <value>Valid values: 0 (instant) - 10000 (slowest). Valid values for <see cref="AOpt.Static.Mouse"/>: 0 - 100.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Used by <see cref="AMouse.Move"/>, <see cref="AMouse.Click"/> and other functions that generate mouse movement events, except <see cref="AMouse.MoveRecorded"/>.
		/// It is not milliseconds or some other unit. It adds intermediate mouse movements and small delays when moving the mouse cursor to the specified point. The speed also depends on the distance.
		/// Value 0 (default) does not add intermediate mouse movements. Adds at least 1 if some mouse buttons are pressed. Value 1 adds at least 1 intermediate mouse movement. Values 10-50 are good for visually slow movements.
		/// </remarks>
		public int MoveSpeed {
			get => _o.MoveSpeed;
			set => _o.MoveSpeed = _SetValue(value, 10000, 100);
		}

		/// <summary>
		/// How long to wait (milliseconds) before a 'mouse click' or 'mouse wheel' function returns.
		/// Default: 10.
		/// </summary>
		/// <value>Valid values: 0 - 10000 (10 s). Valid values for <see cref="AOpt.Static.Mouse"/>: 0 - 100.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// The 'click' functions also sleep <see cref="ClickSpeed"/> ms after button down and up. Default <b>ClickSpeed</b> is 20, default <b>ClickSleepFinally</b> is 10, therefore default click time without mouse-move is 20+20+10=50.
		/// </remarks>
		public int ClickSleepFinally {
			get => _o.ClickSleepFinally;
			set => _o.ClickSleepFinally = _SetValue(value, 10000, 100);
		}

		/// <summary>
		/// How long to wait (milliseconds) after moving the mouse cursor. Used in 'move+click' functions too.
		/// Default: 10.
		/// </summary>
		/// <value>Valid values: 0 - 1000 (1 s). Valid values for <see cref="AOpt.Static.Mouse"/>: 0 - 100.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Used by <see cref="AMouse.Move"/> (finally), <see cref="AMouse.Click"/> (between moving and clicking) and other functions that generate mouse movement events.
		/// </remarks>
		public int MoveSleepFinally {
			get => _o.MoveSleepFinally;
			set => _o.MoveSleepFinally = _SetValue(value, 1000, 100);
		}

		/// <summary>
		/// Make some functions less strict (throw less exceptions etc).
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// This option is used by these functions:
		/// - <see cref="AMouse.Move"/>, <see cref="AMouse.Click"/> and other functions that move the cursor (mouse pointer):
		/// <br/>false - throw exception if cannot move the cursor to the specified x y. For example it the x y is not in screen.
		/// <br/>true - try to move anyway. Don't throw exception, regardless of the final cursor position (which probably will be at a screen edge).
		/// - <see cref="AMouse.Move"/>, <see cref="AMouse.Click"/> and other functions that move the cursor (mouse pointer):
		/// <br/>false - before moving the cursor, wait while a mouse button is pressed by the user or another thread. It prevents an unintended drag-drop.
		/// <br/>true - do not wait.
		/// - <see cref="AMouse.Click"/> and other functions that click or press a mouse button using window coordinates:
		/// <br/>false - don't allow to click in another window. If need, activate the specified window (or its top-level parent). If that does not help, throw exception. However if the window is a control, allow x y anywhere in its top-level parent window.
		/// <br/>true - allow to click in another window. Don't activate the window and don't throw exception.
		/// </remarks>
		public bool Relaxed { get => _o.Relaxed; set => _o.Relaxed = value; }
	}

	/// <summary>
	/// Options for functions of class <see cref="AKeys"/>.
	/// Some options also are used with <see cref="AClipboard"/> functions that send keys (Ctrl+V etc).
	/// </summary>
	/// <seealso cref="AOpt.Key"/>
	/// <seealso cref="AOpt.Static.Key"/>
	public class OptKey
	{
		/// <summary>
		/// Initializes this instance with default values or values copied from another instance.
		/// </summary>
		/// <param name="cloneOptions">If not null, copies its options into this variable.</param>
		public OptKey(OptKey cloneOptions = null)
		{
			CopyOrDefault_(cloneOptions);
		}

		/// <summary>
		/// Copies options from o, or sets default if o==null. Like ctor does.
		/// </summary>
		internal void CopyOrDefault_(OptKey o)
		{
			if(o != null) {
				_textSpeed = o._textSpeed;
				_keySpeed = o._keySpeed;
				_clipboardKeySpeed = o._clipboardKeySpeed;
				_sleepFinally = o._sleepFinally;
				_pasteLength = o._pasteLength;
				TextOption = o.TextOption;
				PasteEnter = o.PasteEnter;
				RestoreClipboard = o.RestoreClipboard;
				NoModOff = o.NoModOff;
				NoCapsOff = o.NoCapsOff;
				NoBlockInput = o.NoBlockInput;
				Hook = o.Hook;
			} else {
				_textSpeed = default;
				_keySpeed = 1;
				_clipboardKeySpeed = 5;
				_sleepFinally = 10;
				_pasteLength = 300;
				TextOption = KTextOption.Characters;
				PasteEnter = default;
				RestoreClipboard = true;
				NoModOff = default;
				NoCapsOff = default;
				NoBlockInput = default;
				Hook = default;
			}
		}

		//rejected. Use AOpt.Scope.
		///// <summary>
		///// Resets all options. Copies from <see cref="AOpt.Static.Key"/>.
		///// </summary>
		//public void Reset() => CopyOrDefault_(AOpt.Static.Key);

		/// <summary>
		/// Returns this variable, or OptKey cloned from this variable and possibly modified by Hook.
		/// </summary>
		/// <param name="wFocus">The focused or active window. Use Lib.GetWndFocusedOrActive().</param>
		internal OptKey GetHookOptionsOrThis_(AWnd wFocus)
		{
			var call = this.Hook;
			if(call == null || wFocus.Is0) return this;
			var R = new OptKey(this);
			call(new KOHookData(R, wFocus));
			return R;
		}

		/// <summary>
		/// How long to wait (milliseconds) between pressing and releasing each character key. Used by <see cref="AKeys.Text"/>. Also by <see cref="AKeys.Key"/> and similar functions for <c>(KText)"text"</c> arguments.
		/// Default: 0.
		/// </summary>
		/// <value>Valid values: 0 - 1000 (1 second). Valid values for <see cref="AOpt.Static.Key"/>: 0 - 10.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used for 'keys' arguments. See <see cref="KeySpeed"/>.
		/// </remarks>
		public int TextSpeed {
			get => _textSpeed;
			set => _textSpeed = _SetValue(value, 1000, 10);
		}
		int _textSpeed;

		/// <summary>
		/// How long to wait (milliseconds) between pressing and releasing each key. Used by <see cref="AKeys.Key"/> and similar functions, except for <c>(KText)"text"</c> arguments.
		/// Default: 1.
		/// </summary>
		/// <value>Valid values: 0 - 1000 (1 second). Valid values for <see cref="AOpt.Static.Key"/>: 0 - 10.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used for 'text' arguments. See <see cref="TextSpeed"/>.
		/// </remarks>
		public int KeySpeed {
			get => _keySpeed;
			set => _keySpeed = _SetValue(value, 1000, 10);
		}
		int _keySpeed;

		/// <summary>
		/// How long to wait (milliseconds) between sending Ctrl+V and Ctrl+C keys of clipboard functions (paste, copy).
		/// Default: 5.
		/// </summary>
		/// <value>Valid values: 0 - 1000 (1 second). Valid values for <see cref="AOpt.Static.Key"/>: 0 - 50.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// In most apps copy/paste works without this delay. Known apps that need it: Internet Explorer's address bar, BlueStacks.
		/// </remarks>
		public int KeySpeedClipboard {
			get => _clipboardKeySpeed;
			set => _clipboardKeySpeed = _SetValue(value, 1000, 50);
		}
		int _clipboardKeySpeed;

		/// <summary>
		/// How long to wait (milliseconds) before a 'send keys or text' function returns.
		/// Default: 10.
		/// </summary>
		/// <value>Valid values: 0 - 10000 (10 seconds). Valid values for <see cref="AOpt.Static.Key"/>: 0 - 100.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used by <see cref="AClipboard.Copy"/>.
		/// </remarks>
		public int SleepFinally {
			get => _sleepFinally;
			set => _sleepFinally = _SetValue(value, 10000, 100);
		}
		int _sleepFinally;

		bool _IsStatic => this == AOpt.Static.Key;

		int _SetValue(int value, int max, int maxStatic)
		{
			var m = _IsStatic ? maxStatic : max;
			if((uint)value > m) throw new ArgumentOutOfRangeException(null, "Max " + m.ToString());
			return value;
		}
		//T _SetValue<T>(T value, T max, T maxStatic) where T : IComparable<T>
		//{
		//	var m = _IsStatic ? maxStatic : max;
		//	if(value.CompareTo(m) > 0 || value.CompareTo(default) < 0) throw new ArgumentOutOfRangeException(null, "Max " + m.ToString());
		//	return value;
		//}

		/// <summary>
		/// How functions send text to the active window (keys, clipboard, etc).
		/// Default: <see cref="KTextOption.Characters"/>.
		/// </summary>
		public KTextOption TextOption { get; set; }

		/// <summary>
		/// To send text use clipboard (like with option <see cref="KTextOption.Paste"/>) if text length is &gt;= this value.
		/// Default: 300.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int PasteLength {
			get => _pasteLength;
			set => _pasteLength = _SetValue(value, int.MaxValue, int.MaxValue);
		}
		int _pasteLength;

		/// <summary>
		/// When pasting text that ends with newline characters, remove the last newline and after pasting send the Enter key.
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// Some apps remove the last newline when pasting. For example Word, WordPad, OpenOffice, LibreOffice, standard rich text controls. This option is a workaround.
		/// </remarks>
		public bool PasteEnter { get; set; }

		//rejected: rarely used. Eg can be useful for Python programmers. Let call AClipboard.Paste() explicitly or set the Paste option eg in hook.
		///// <summary>
		///// To send text use <see cref="KTextOption.Paste"/> if text contains characters '\n' followed by '\t' (tab) or spaces.
		///// </summary>
		///// <remarks>
		///// Some apps auto-indent. This option is a workaround.
		///// </remarks>
		//public bool PasteMultilineIndented { get; set; }

		/// <summary>
		/// Whether to restore clipboard data when copying or pasting text.
		/// Default: true.
		/// By default restores only text. See also <see cref="RestoreClipboardAllFormats"/>, <see cref="RestoreClipboardExceptFormats"/>.
		/// </summary>
		public bool RestoreClipboard { get; set; }

		#region static RestoreClipboard options

		/// <summary>
		/// When copying or pasting text, restore clipboard data of all formats that are possible to restore.
		/// Default: false - restore only text.
		/// </summary>
		/// <remarks>
		/// Restoring data of all formats set by some apps can be slow or cause problems. More info: <see cref="RestoreClipboardExceptFormats"/>.
		/// 
		/// This property is static, not thread-static. It should be set (if need) at the very start of script and not changed later.
		/// </remarks>
		/// <seealso cref="RestoreClipboard"/>
		/// <seealso cref="RestoreClipboardExceptFormats"/>
		public static bool RestoreClipboardAllFormats { get; set; }

		/// <summary>
		/// When copying or pasting text, and <see cref="RestoreClipboardAllFormats"/> is true, do not restore clipboard data of these formats.
		/// Default: null.
		/// </summary>
		/// <remarks>
		/// To restore clipboard data, the copy/paste functions at first get clipboard data. Getting data of some formats set by some apps can be slow (100 ms or more) or cause problems (the app can change something in its window or even show a dialog).
		/// It also depends on whether this is the first time the data is being retrieved. The app can render data on demand, when some app is retrieving it from the clipboard first time; then can be slow etc.
		/// 
		/// You can use function <see cref="PrintClipboard"/> to see format names and get-data times.
		/// 
		/// There are several kinds of clipboard formats - registered, standard, private and display. Only registered formats have string names. For standard formats use API contant names, like "CF_WAVE". Private, display and metafile formats are never restored.
		/// These formats are never restored: CF_METAFILEPICT, CF_ENHMETAFILE, CF_PALETTE, CF_OWNERDISPLAY, CF_DSPx formats, CF_GDIOBJx formats, CF_PRIVATEx formats. Some other formats too, but they are automatically synthesized from other formats if need. Also does not restore if data size is 0 or &gt; 10 MB.
		/// 
		/// This property is static, not thread-static. It should be set (if need) at the very start of script and not changed later.
		/// </remarks>
		/// <seealso cref="RestoreClipboard"/>
		/// <seealso cref="PrintClipboard"/>
		public static string[] RestoreClipboardExceptFormats { get; set; }

		/// <summary>
		/// Writes to the output some info about current clipboard data.
		/// </summary>
		/// <remarks>
		/// Shows this info for each clipboard format: format name, time spent to get data (microseconds), data size (bytes), and whether this format would be restored (depends on <see cref="RestoreClipboardExceptFormats"/>).
		/// <note>Copy something to the clipboard each time before calling this function. Don't use <see cref="AClipboard.Copy"/> and don't call this function in loop. Else it shows small times.</note>
		/// The time depends on app, etc. More info: <see cref="RestoreClipboardExceptFormats"/>.
		/// </remarks>
		public static void PrintClipboard() => AClipboard.PrintClipboard_();

		#endregion

		/// <summary>
		/// When starting to send keys or text, don't release modifier keys.
		/// Default: false.
		/// </summary>
		public bool NoModOff { get; set; }

		/// <summary>
		/// When starting to send keys or text, don't turn off CapsLock.
		/// Default: false.
		/// </summary>
		public bool NoCapsOff { get; set; }

		/// <summary>
		/// While sending or pasting keys or text, don't block user-pressed keys.
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// If false (default), user-pressed keys are sent afterwards. If true, user-pressed keys can be mixed with script-pressed keys, which is particularly dangerous when modifier keys are mixed (and combined) with non-modifier keys.
		/// </remarks>
		public bool NoBlockInput { get; set; }

		/// <summary>
		/// Callback function that can modify options of 'send keys or text' functions depending on active window etc.
		/// Default: null.
		/// </summary>
		/// <remarks>
		/// The callback function is called by <see cref="AKeys.Key"/>, <see cref="AKeys.Text"/>, <see cref="AKeys.Send"/>, <see cref="AClipboard.Paste"/> and similar functions. Not called by <see cref="AClipboard.Copy"/>.
		/// </remarks>
		/// <seealso cref="KOHookData"/>
		public Action<KOHookData> Hook { get; set; }
	}

	/// <summary>
	/// Parameter type of the <see cref="OptKey.Hook"/> callback function.
	/// </summary>
	public struct KOHookData
	{
		internal KOHookData(OptKey opt, AWnd w) { this.opt = opt; this.w = w; }

		/// <summary>
		/// Options used by the 'send keys or text' function. The callback function can modify them, except Hook, NoModOff, NoCapsOff, NoBlockInput.
		/// </summary>
		public readonly OptKey opt;

		/// <summary>
		/// The focused control. If there is no focused control - the active window. Use <c>w.Window</c> to get top-level window; if <c>w.Window == w</c>, <b>w</b> is the active window, else the focused control. The callback function is not called if there is no active window.
		/// </summary>
		public readonly AWnd w;
	}

	/// <summary>
	/// How functions send text.
	/// See <see cref="OptKey.TextOption"/>.
	/// </summary>
	/// <remarks>
	/// There are three ways to send text to the active app using keys: 1. Characters (default) - use special key code VK_PACKET. 2. Keys - press keybord keys. 3. Paste - use the clipboard and Ctrl+V.
	/// Most but not all apps support all three ways. Most Unicode characters cannot be sent with <b>Keys</b>.
	/// Depending on text, the 'send text' functions may use other method than specified. For some characters or for whole text. More info below.
	/// Many apps don't support Unicode surrogate pairs sent as keys. If the text contains such characters, is used <b>Paste</b> instead of other options (implicitly). These characters are rarely used.
	/// </remarks>
	public enum KTextOption
	{
		/// <summary>
		/// Send text characters using special key code VK_PACKET.
		/// Few apps don't support it.
		/// This option is default.
		/// Supports most Unicode characters.
		/// For newlines sends key Enter, because VK_PACKET often does not work well.
		/// </summary>
		Characters,
		//info:
		//	Tested many apps/controls/frameworks. Works almost everywhere.
		//	Does not work with Pidgin (thanks PhraseExpress for this info) and probably in all apps that use the same framework (GTK?).
		//	I guess does not work with many games.
		//	In PhraseExpress this is default. Its alternative methods are SendKeys (does not send Unicode chars) and clipboard. It uses clipboard if text is long, default 100. Allows to choose different for specified apps. Does not add any delays between chars; for some apps too fast, eg VirtualBox edit fields when text contains Unicode surrogates.

		/// <summary>
		/// Send text keys, with Shift or other modifiers where need, depending on the keyboard layout of the active window. The numpad keys are not used.
		/// All apps support it.
		/// Some characters cannot be easily typed using the keyboard. For example most non-ASCII characters. Sends these characters like with the <b>Characters</b> option.
		/// </summary>
		Keys,

		/// <summary>
		/// Paste text using the clipboard and Ctrl+V.
		/// Few apps don't support it.
		/// This option is recommended for long text, because other ways then are too slow.
		/// Other options are unreliable when text length is more than 4000 or 5000 and the target app is too slow to process sent characters. Then <see cref="OptKey.TextSpeed"/> can help.
		/// Also, other options are unreliable when the target app modifies typed text, for example has such features as auto-complete or auto-indent. However some apps modify even pasted text, for example trim the last newline.
		/// When pasting text, previous clipboard data is lost. Only text is restored.
		/// </summary>
		Paste,

		//rejected: WmPaste. Few windows support it.
	}

	/// <summary>
	/// Options for 'wait for' functions.
	/// </summary>
	/// <seealso cref="AOpt.WaitFor"/>
	/// <seealso cref="AWaitFor.Condition"/>
	/// <seealso cref="AWaitFor.Loop"/>
	public class OptWaitFor
	{
		/// <summary>
		/// The sleep time between checking the wait condition. Milliseconds.
		/// Default: 10.
		/// </summary>
		/// <value>Valid values: 1-1000.</value>
		/// <remarks>
		/// Most 'wait for' functions of this library use <see cref="AWaitFor.Loop"/>, which repeatedly checks the wait condition and sleeps (waits) several ms. This property sets the initial sleep time, which then is incremented by <b>Period</b>/10 ms (default 1 ms) in each loop until reaches <b>Period</b>*50 (default 500 ms).
		/// This property makes the response time shorter or longer. If &lt;10, makes it shorter (faster response), but increases CPU usage; if &gt;10, makes it longer (slower response).
		/// </remarks>
		/// <seealso cref="AWaitFor.Loop.Period"/>
		/// <example>
		/// <code><![CDATA[
		/// AOpt.WaitFor.Period = 100;
		/// ]]></code>
		/// </example>
		public int Period { get => _period; set => _period = Math.Clamp(value, 1, 1000); }
		int _period;

		/// <summary>
		/// Use <see cref="ATime.SleepDoEvents"/> instead of <see cref="ATime.Sleep"/>.
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// Use this property when need to process Windows messages, events, hooks, timers, etc while waiting. More info: <see cref="ATime.SleepDoEvents"/>.
		/// </remarks>
		/// <seealso cref="AWaitFor.MessagesAndCondition"/>
		/// <example>
		/// <code><![CDATA[
		/// AOpt.WaitFor.DoEvents = true;
		/// ]]></code>
		/// </example>
		public bool DoEvents { get; set; }

		///
		public OptWaitFor(int period = 10, bool doEvents = false)
		{
			Period = period; DoEvents = doEvents;
		}

		//rejected. Use AOpt.Scope.
		///// <summary>
		///// Resets all options.
		///// </summary>
		//public void Reset()
		//{
		//	Period = 10; DoEvents = false;
		//}

		//no
		///// <summary>
		///// Implicit conversion from int. Sets <see cref="Period"/>.
		///// </summary>
		//public static implicit operator OptWaitFor(int period) => new OptWaitFor(period, false);

		///// <summary>
		///// Implicit conversion from bool. Sets <see cref="DoEvents"/>.
		///// </summary>
		//public static implicit operator OptWaitFor(bool doEvents) => new OptWaitFor(10, doEvents);
	}
}
