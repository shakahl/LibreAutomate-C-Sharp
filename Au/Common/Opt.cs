using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Options used by some functions of this library.
	/// </summary>
	/// <remarks>
	/// Some frequently used static functions of this library have some options (settings). For example <see cref="Keyb.Key"/> allows to change speed, text sending method, etc. Passing options as parameters in each call usually isn't what you want to do in automation scripts. Instead you can set options using static properties. This class contains several groups of options for functions of various classes. See examples.
	/// 
	/// There are two sets of identical or similar options - in class <b>Opt</b> and in its nested class <see cref="Opt.Static"/>:
	/// <list type="bullet">
	/// <item><b>Opt</b> - thread-static options (each thread has its own instance). Functions of this library use them. You can change or change-restore them anywhere in script. Initial options are automatically copied from <b>Opt.Static</b> when that group of options (<b>Key</b>, <b>Mouse</b>, etc) is used first time in that thread (explicitly or by library functions).</item>
	/// <item><b>Opt.Static</b> - static options. Contains initial property values for <b>Opt</b>. Normally you change them in your script template (in script initialization code) or at the very start of script. Don't change later, it's not thread-safe.</item>
	/// </list>
	/// </remarks>
	/// <example><inheritdoc cref="Static.Mouse"/></example>
	public static class Opt
	{
		/// <summary>
		/// Options for keyboard and clipboard functions (classes <see cref="Keyb"/>, <see cref="Clipb"/> and functions that use them).
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>Opt.Key</b> instance. It inherits options from <see cref="Opt.Static.Key"/>.
		/// Also can be used when creating <see cref="Keyb"/> instances. See the second example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Opt.Key.KeySpeed = 20;
		/// Key("Tab Ctrl+V");
		/// ]]></code>
		/// Use a Keyb instance.
		/// <code><![CDATA[
		/// var k = new Keyb(Opt.Key); //create new Keyb instance and copy options from Opt.Key to it
		/// k.Options.KeySpeed = 100; //changes option of k but not of Opt.Key
		/// k.Add("Tab Ctrl+V").Send(); //uses options of k
		/// ]]></code>
		/// </example>
		public static KOptions Key => t_key ?? (t_key = new KOptions(Opt.Static.Key));
		[ThreadStatic] internal static KOptions t_key;

		/// <summary>
		/// Options for mouse functions (class <see cref="Mouse"/> and functions that use it).
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>Opt.Mouse</b> instance. It inherits options from <see cref="Opt.Static.Mouse"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Opt.Mouse.ClickSpeed = 100;
		/// Mouse.Click();
		/// ]]></code>
		/// </example>
		public static MOptions Mouse => t_mouse ?? (t_mouse = new MOptions(Opt.Static.Mouse));
		[ThreadStatic] internal static MOptions t_mouse;

		/// <summary>
		/// Options for showing run-time warnings and other info that can be useful to find problems in code at run time.
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>Opt.Debug</b> instance. It inherits options from <see cref="Opt.Static.Debug"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Opt.Debug.Verbose = false;
		/// PrintWarning("Example");
		/// PrintWarning("Example");
		/// ]]></code>
		/// </example>
		public static DOptions Debug => t_debug ?? (t_debug = new DOptions());
		[ThreadStatic] static DOptions t_debug;

		/// <summary>
		/// Default <see cref="Opt"/> properties of each thread.
		/// </summary>
		/// <remarks>
		/// You can change these options at the start of your script or in the static constructor of script's class. Don't change later.
		/// </remarks>
		public static class Static
		{
			/// <summary>
			/// Default option values for <see cref="Opt.Key"/> of each thread.
			/// </summary>
			/// <remarks>
			/// Also can be used when creating <see cref="Keyb"/> instances. See the second example.
			/// </remarks>
			/// <example>
			/// <code><![CDATA[
			/// static MyScriptClass() { Opt.Static.Key.KeySpeed = 10; } //static constructor
			/// ...
			/// Key("Tab Ctrl+V"); //uses Opt.Key, which is implicitly copied from Opt.Static.Key
			/// ]]></code>
			/// Use a Keyb instance.
			/// <code><![CDATA[
			/// var k = new Keyb(Opt.Static.Key); //create new Keyb instance and copy options from Opt.Static.Key to it
			/// k.Options.KeySpeed = 100; //changes option of k but not of Opt.Static.Key
			/// k.Add("Tab Ctrl+V").Send(); //uses options of k
			/// ]]></code>
			/// </example>
			public static KOptions Key { get; } = new KOptions();

			/// <summary>
			/// Default option values for <see cref="Opt.Mouse"/> of each thread.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// static MyScriptClass() { Opt.Static.Mouse.ClickSpeed = 10; } //static constructor, for example in script template
			/// ...
			/// Mouse.Click(); //uses Opt.Mouse, which is implicitly copied from Opt.Static.Mouse
			/// ]]></code>
			/// </example>
			public static MOptions Mouse { get; } = new MOptions();

			/// <summary>
			/// Default option values for <see cref="Opt.Debug"/> of each thread.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// Opt.Static.Debug.Verbose = false;
			/// ]]></code>
			/// </example>
			public static DOptions Debug { get; } = new DOptions();

		}

		/// <summary>
		/// Makes easy to restore current options of this thread. See example.
		/// </summary>
		/// <example><inheritdoc cref="Temp.Mouse"/></example>
		public static class Temp
		{
			/// <summary>
			/// Makes easy to restore current <see cref="Opt.Mouse"/> of this thread. See example.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// Print(Opt.Mouse.ClickSpeed);
			/// using(Opt.Temp.Mouse) {
			/// 	Opt.Mouse.ClickSpeed = 100;
			/// 	Print(Opt.Mouse.ClickSpeed);
			/// } //here restored automatically
			/// Print(Opt.Mouse.ClickSpeed);
			/// ]]></code>
			/// </example>
			public static RestoreMouse Mouse => new RestoreMouse(0);

			/// <summary>
			/// Makes easy to restore current <see cref="Opt.Key"/> of this thread. See example.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// Print(Opt.Key.KeySpeed);
			/// using(Opt.Temp.Key) {
			/// 	Opt.Key.KeySpeed = 5;
			/// 	Print(Opt.Key.KeySpeed);
			/// } //here restored automatically
			/// Print(Opt.Key.KeySpeed);
			/// ]]></code>
			/// </example>
			public static RestoreKey Key => new RestoreKey(0);

			/// <summary>Infrastructure.</summary>
			/// <tocexclude />
			[EditorBrowsable(EditorBrowsableState.Never)]
			public struct RestoreMouse :IDisposable
			{
				MOptions _o;
				internal RestoreMouse(int unused) => Opt.t_mouse = new MOptions(_o = Opt.t_mouse);
				/// <summary>Restores options.</summary>
				public void Dispose() => Opt.t_mouse = _o;
			}

			/// <summary>Infrastructure.</summary>
			/// <tocexclude />
			[EditorBrowsable(EditorBrowsableState.Never)]
			public struct RestoreKey :IDisposable
			{
				KOptions _o;
				internal RestoreKey(int unused) => Opt.t_key = new KOptions(_o = Opt.t_key);
				/// <summary>Restores options.</summary>
				public void Dispose() => Opt.t_key = _o;
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Options for showing run-time warnings and other info that can be useful to find problems in code at run time.
	/// </summary>
	public class DOptions
	{
		struct _Options //makes easier to copy and reset fields
		{
			public byte Verbose; //0 non-init, 1 false, 2 true
		}
		_Options _o;
		List<string> _disabledWarnings;

		internal DOptions()
		{
			var o = Opt.Static.Debug;
			if(this == o) {
				_o = o._o;
				if(o._disabledWarnings != null) _disabledWarnings = new List<string>(o._disabledWarnings);
			} else {
			}
		}

		/// <summary>
		/// If true, some library functions may display more warnings and other info.
		/// If not explicitly set, the default value depends on the build configuration of the entry assymbly: true if Debug, false if Release.
		/// </summary>
		public bool Verbose
		{
			get
			{
				if(_o.Verbose == 0) Verbose = _IsAppDebugConfig();
				return _o.Verbose == 2;
			}
			set
			{
				_o.Verbose = (byte)(value ? 2 : 1);
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
		/// Disables one or more run-time warnings.
		/// </summary>
		/// <param name="warningsWild">One or more warnings as case-insensitive wildcard strings. See <see cref="String_.Like_(string, string, bool)"/>.</param>
		/// <remarks>
		/// Adds the strings to an internal list. When <see cref="PrintWarning"/> is called, it looks in the list. If it finds the warning in the list, it does not show the warning.
		/// It's easy to auto-restore warnings with 'using', like in the second example. Restoring is optional.
		/// </remarks>
		/// <example>
		/// This code in script template disables two warnings for all new scripts.
		/// <code><![CDATA[
		/// Opt.Static.Debug.DisableWarnings("*part of warning 1 text*", "*part of warning 2 text*");
		/// ]]></code>
		/// Temporarily disable all warnings for this thread.
		/// <code><![CDATA[
		/// Opt.Debug.Verbose = true;
		/// PrintWarning("one");
		/// using(Opt.Debug.DisableWarnings("*")) {
		/// 	PrintWarning("two");
		/// }
		/// PrintWarning("three");
		/// ]]></code>
		/// </example>
		public RestoreWarnings DisableWarnings(params string[] warningsWild)
		{
			if(_disabledWarnings == null) _disabledWarnings = new List<string>();
			int restoreCount = _disabledWarnings.Count;
			_disabledWarnings.AddRange(warningsWild);
			return new RestoreWarnings(this, restoreCount);
		}

		internal void LibRestoreWarnings(int restoreCount)
		{
			if(this == Opt.Static.Debug) throw new InvalidOperationException(); //would be not thread-safe, and useless
			_disabledWarnings.RemoveRange(restoreCount, _disabledWarnings.Count - restoreCount);
		}

		/// <summary>
		/// Returns true if the specified warning text matches a wildcard string added with <see cref="DisableWarnings"/>.
		/// </summary>
		/// <param name="text">Warning text. Case-insensitive.</param>
		public bool IsWarningDisabled(string text)
		{
			string s = text ?? "";
			var a = _disabledWarnings;
			if(a != null) foreach(var k in a) if(s.Like_(k, true)) return true;
			return false;
		}

		/// <summary>Infrastructure.</summary>
		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public struct RestoreWarnings :IDisposable
		{
			DOptions _o;
			int _restoreCount;

			internal RestoreWarnings(DOptions o, int restoreCount) { _o = o; _restoreCount = restoreCount; }

			/// <summary>Restores warnings.</summary>
			public void Dispose() => _o.LibRestoreWarnings(_restoreCount);
		}
	}

	/// <summary>
	/// Options for functions of class <see cref="Mouse"/>.
	/// </summary>
	/// <seealso cref="Opt.Mouse"/>
	/// <seealso cref="Opt.Static.Mouse"/>
	/// <remarks>
	/// Total <c>Click(x, y)</c> time is: mouse move + <see cref="MoveSleepFinally"/> + button down + <see cref="ClickSpeed"/> + button down + <see cref="ClickSpeed"/> + <see cref="ClickSleepFinally"/>.
	/// </remarks>
	public class MOptions
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
		internal MOptions(MOptions cloneOptions = null) //don't need public like KOptions
		{
			_LibReset(cloneOptions);
		}

		/// <summary>
		/// Copies options from o, or sets default if o==null. Like ctor does.
		/// </summary>
		void _LibReset(MOptions o) //could be used as internal, but currently don't need
		{
			if(o != null) {
				_o = o._o;
			} else {
				_o.ClickSpeed = 20;
				_o.MoveSpeed = 0;
				_o.ClickSleepFinally = 10;
				_o.MoveSleepFinally = 10;
			}
		}

		bool _IsStatic => this == Opt.Static.Mouse;

		int _SetValue(int value, int max, int maxStatic)
		{
			var m = _IsStatic ? maxStatic : max;
			if((uint)value > m) throw new ArgumentOutOfRangeException(null, "Max " + m.ToString());
			return value;
		}

		/// <summary>
		/// How long to wait (milliseconds) after sending each mouse button down or up event (2 events for click, 4 for double-click).
		/// Default: 20. Valid values: 0 - 1000 (1 s). Valid values for <see cref="Opt.Static.Mouse"/>: 0 - 100 (1 s).
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int ClickSpeed
		{
			get => _o.ClickSpeed;
			set => _o.ClickSpeed = _SetValue(value, 1000, 100);
		}

		/// <summary>
		/// If not 0, makes mouse movements slower, not instant.
		/// Default: 0. Valid values: 0 (instant) - 10000 (slowest). Valid values for <see cref="Opt.Static.Mouse"/>: 0 - 100.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Used by <see cref="Mouse.Move"/>, <see cref="Mouse.Click"/> and other functions that generate mouse movement events, except <see cref="Mouse.MoveRecorded"/>.
		/// It is not milliseconds or some other unit. It adds intermediate mouse movements and small delays when moving the mouse cursor to the specified point. The speed also depends on the distance.
		/// Value 0 (default) does not add intermediate mouse movements. Adds at least 1 if some mouse buttons are pressed. Value 1 adds at least 1 intermediate mouse movement. Values 10-50 are good for visually slow movements.
		/// </remarks>
		public int MoveSpeed
		{
			get => _o.MoveSpeed;
			set => _o.MoveSpeed = _SetValue(value, 10000, 100);
		}

		/// <summary>
		/// How long to wait (milliseconds) before a 'mouse click' or 'mouse wheel' function returns.
		/// Default: 10. Valid values: 0 - 10000 (10 s). Valid values for <see cref="Opt.Static.Mouse"/>: 0 - 100.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// The 'click' functions also sleep <see cref="ClickSpeed"/> ms after button down and up. Default <b>ClickSpeed</b> is 20, default <b>ClickSleepFinally</b> is 10, therefore default click time without mouse-move is 20+20+10=50.
		/// </remarks>
		public int ClickSleepFinally
		{
			get => _o.ClickSleepFinally;
			set => _o.ClickSleepFinally = _SetValue(value, 10000, 100);
		}

		/// <summary>
		/// How long to wait (milliseconds) after moving the mouse cursor. Used in 'move+click' functions too.
		/// Default: 10. Valid values: 0 - 1000 (1 s). Valid values for <see cref="Opt.Static.Mouse"/>: 0 - 100.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Used by <see cref="Mouse.Move"/> (finally), <see cref="Mouse.Click"/> (between moving and clicking) and other functions that generate mouse movement events.
		/// </remarks>
		public int MoveSleepFinally
		{
			get => _o.MoveSleepFinally;
			set => _o.MoveSleepFinally = _SetValue(value, 1000, 100);
		}

		/// <summary>
		/// Make some functions less strict (throw less exceptions etc).
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// This option is used by these functions:
		/// <list type="bullet">
		/// <item><see cref="Mouse.Move"/>, <see cref="Mouse.Click"/> and other functions that move the cursor (mouse pointer):
		/// false - throw exception if cannot move the cursor to the specified x y. For example it the x y is not in screen.
		/// true - try to move anyway. Don't throw exception, regardless of the final cursor position (which probably will be at a screen edge).
		/// </item>
		/// <item><see cref="Mouse.Move"/>, <see cref="Mouse.Click"/> and other functions that move the cursor (mouse pointer):
		/// false - before moving the cursor, wait while a mouse button is pressed by the user or another thread. It prevents an unintended drag-drop.
		/// true - do not wait.
		/// </item>
		/// <item><see cref="Mouse.Click"/> and other functions that click or press a mouse button using window coordinates:
		/// false - don't allow to click in another window. If need, activate the specified window (or its top-level parent). If that does not help, throw exception. However if the window is a control, allow x y anywhere in its top-level parent window.
		/// true - allow to click in another window. Don't activate the window and don't throw exception.
		/// </item>
		/// </list>
		/// </remarks>
		public bool Relaxed { get => _o.Relaxed; set => _o.Relaxed = value; }
	}

	/// <summary>
	/// Options for functions of class <see cref="Keyb"/>.
	/// Some options also are used with <see cref="Clipb"/> functions that send keys (Ctrl+V etc).
	/// </summary>
	/// <seealso cref="Opt.Key"/>
	/// <seealso cref="Opt.Static.Key"/>
	public class KOptions
	{
		/// <summary>
		/// Initializes this instance with default values or values copied from another instance.
		/// </summary>
		/// <param name="cloneOptions">If not null, copies its options into this variable.</param>
		public KOptions(KOptions cloneOptions = null)
		{
			LibReset(cloneOptions);
		}

		/// <summary>
		/// Copies options from o, or sets default if o==null. Like ctor does.
		/// </summary>
		internal void LibReset(KOptions o)
		{
			if(o != null) {
				_keySpeed = o._keySpeed;
				_textSpeed = o._textSpeed;
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
				_keySpeed = 1;
				_textSpeed = default;
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

		/// <summary>
		/// Returns this variable or KOptions cloned from this variable and possibly modified by Hook.
		/// </summary>
		/// <param name="wFocus">The focused or active window. Use Lib.GetWndFocusedOrActive().</param>
		internal KOptions LibGetHookOptionsOrThis(Wnd wFocus)
		{
			var call = this.Hook;
			if(call == null || wFocus.Is0) return this;
			var R = new KOptions(this);
			call(new KOHookData(R, wFocus));
			return R;
		}

		/// <summary>
		/// How long to wait (milliseconds) between pressing and releasing each key of 'keys' parameters of <see cref="Keyb.Key"/> and similar functions.
		/// Default: 1. Valid values: 0 - 1000 (1 second). Valid values for <see cref="Opt.Static.Key"/>: 0 - 10.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used for 'text' parameters; see <see cref="TextSpeed"/>.
		/// Clipboard functions that send keys Ctrl+V, Ctrl+C or Ctrl+X use this only after Ctrl (need it for some apps). The V/C/X key time depends on how fast the target app gets or sets clipboard data. 
		/// </remarks>
		public int KeySpeed
		{
			get => _keySpeed;
			set => _keySpeed = _SetValue(value, 1000, 10);
		}
		int _keySpeed;

		/// <summary>
		/// How long to wait (milliseconds) between pressing and releasing each character key of 'text' parameters of <see cref="Keyb.Text"/>, <see cref="Keyb.Key"/> and similar functions.
		/// Default: 0. Valid values: 0 - 1000 (1 second). Valid values for <see cref="Opt.Static.Key"/>: 0 - 10.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used for 'keys' parameters; see <see cref="KeySpeed"/>.
		/// </remarks>
		public int TextSpeed
		{
			get => _textSpeed;
			set => _textSpeed = _SetValue(value, 1000, 10);
		}
		int _textSpeed;

		/// <summary>
		/// How long to wait (milliseconds) before a 'send keys or text' function returns.
		/// Default: 10. Valid values: 0 - 10000 (10 seconds). Valid values for <see cref="Opt.Static.Key"/>: 0 - 100.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used by <see cref="Clipb.CopyText"/>.
		/// </remarks>
		public int SleepFinally
		{
			get => _sleepFinally;
			set => _sleepFinally = _SetValue(value, 10000, 100);
		}
		int _sleepFinally;

		bool _IsStatic => this == Opt.Static.Key;

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
		public int PasteLength
		{
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

		//rejected: rarely used. Eg can be useful for Python programmers. Let call Paste() explicitly or set the Paste option eg in hook.
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
		/// This property is static, not thread-static. It should be set (if need) at the very start of the script (eg in the script template) and not changed later.
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
		/// This property is static, not thread-static. It should be set (if need) at the very start of the script (eg in the script template) and not changed later.
		/// </remarks>
		/// <seealso cref="RestoreClipboard"/>
		/// <seealso cref="PrintClipboard"/>
		public static string[] RestoreClipboardExceptFormats { get; set; }

		/// <summary>
		/// Prints some info about current clipboard data.
		/// </summary>
		/// <remarks>
		/// Shows this info in the output, for each clipboard format: format name, time spent to get data (microseconds), data size (bytes), and whether this format would be restored (depends on <see cref="RestoreClipboardExceptFormats"/>).
		/// <note type="note">Copy something to the clipboard each time before calling this function. Don't use <see cref="Clipb.CopyText"/> and don't call this function in loop. Else it shows small times.</note>
		/// The time depends on app, etc. More info: <see cref="RestoreClipboardExceptFormats"/>.
		/// </remarks>
		public static void PrintClipboard() => Clipb.LibPrintClipboard();

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
		/// The callback function is called by <see cref="Key"/>, <see cref="Text"/>, <see cref="Keyb.Send"/>, <see cref="Paste"/> and similar functions. Not called by <see cref="Clipb.CopyText"/>.
		/// </remarks>
		/// <seealso cref="KOHookData"/>
		public Action<KOHookData> Hook { get; set; }
	}

	/// <summary>
	/// Parameter type of the <see cref="KOptions.Hook"/> callback function.
	/// </summary>
	public struct KOHookData
	{
		internal KOHookData(KOptions opt, Wnd w) { this.opt = opt; this.w = w; }

		/// <summary>
		/// Options used by the 'send keys or text' function. The callback function can modify them, except Hook, NoModOff, NoCapsOff, NoBlockInput.
		/// </summary>
		public readonly KOptions opt;

		/// <summary>
		/// The focused control. If there is no focused control - the active window. Use <c>w.WndWindow</c> to get top-level parent window; if <c>w.WndWindow == w</c>, <b>w</b> is the active window, else the focused control. The callback function is not called if there is no active window.
		/// </summary>
		public readonly Wnd w;
	}

	/// <summary>
	/// How functions send text.
	/// See <see cref="KOptions.TextOption"/>.
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
		/// For newlines is used key Enter (implicitly).
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
		/// Cannot send characters that cannot be typed using the keyboard. For example most Unicode characters. For these characters is used the <b>Characters</b> option (implicitly).
		/// </summary>
		Keys,

		/// <summary>
		/// Paste text using the clipboard and Ctrl+V.
		/// Few apps don't support it.
		/// This option is recommended for long text, because other ways then are too slow.
		/// Other options are unreliable when text length is more than 4000 or 5000 and the target app is too slow to process sent characters. Then <see cref="KOptions.TextSpeed"/> can help.
		/// Also, other options are unreliable when the target app modifies typed text, for example has such features as auto-complete or auto-indent. However some apps modify even pasted text, for example trim the last newline.
		/// When pasting text, previous clipboard data is lost.
		/// </summary>
		Paste,

		//rejected: WmPaste. Few windows support it.
	}

}
