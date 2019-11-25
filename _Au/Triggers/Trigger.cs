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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au.Triggers
{
	/// <summary>
	/// Base of classes of all action trigger types.
	/// </summary>
	public abstract class ActionTrigger
	{
		internal ActionTrigger next; //linked list when eg same hotkey is used in multiple scopes
		internal readonly Delegate action;
		internal readonly ActionTriggers triggers;
		internal readonly TOptions options; //Triggers.Options
		internal readonly TriggerScope scope; //Triggers.Of.WindowX. Used by hotkey, autotext and mouse triggers.
		readonly TriggerFunc[] _funcAfter, _funcBefore; //Triggers.FuncOf. _funcAfter used by all triggers; _funcBefore - like scope.

		internal ActionTrigger(ActionTriggers triggers, Delegate action, bool usesWindowScope)
		{
			this.action = action;
			this.triggers = triggers;
			var to = triggers.options;
			options = to.Current;
			EnabledAlways = to.EnabledAlways;
			if(usesWindowScope) scope = triggers.scopes.Current;
			var tf = triggers.funcs;
			_funcBefore = _Func(tf.commonBefore, tf.nextBefore); tf.nextBefore = null;
			_funcAfter = _Func(tf.nextAfter, tf.commonAfter); tf.nextAfter = null;

			TriggerFunc[] _Func(TFunc f1, TFunc f2)
			{
				var f3 = f1 + f2; if(f3 == null) return null;
				var a1 = f3.GetInvocationList();
				var r1 = new TriggerFunc[a1.Length];
				for(int i = 0; i < a1.Length; i++) {
					var f4 = a1[i] as TFunc;
					if(!tf.perfDict.TryGetValue(f4, out var fs)) tf.perfDict[f4] = fs = new TriggerFunc { f = f4 };
					r1[i] = fs;
				}
				return r1;
			}
		}

		internal void DictAdd<TKey>(Dictionary<TKey, ActionTrigger> d, TKey key)
		{
			if(!d.TryGetValue(key, out var o)) d.Add(key, this);
			else { //append to the linked list
				while(o.next != null) o = o.next;
				o.next = this;
			}
		}

		/// <summary>
		/// Called through <see cref="TriggerActionThreads.Run"/> in action thread.
		/// Possibly runs later.
		/// </summary>
		internal abstract void Run(TriggerArgs args);

		/// <summary>
		/// Makes simpler to implement <see cref="Run"/>.
		/// </summary>
		protected private void RunT<T>(T args) => (action as Action<T>)(args);

		/// <summary>
		/// Returns a trigger type string, like "Hotkey", "Mouse", "Window.ActiveNew".
		/// </summary>
		public abstract string TypeString { get; }

		/// <summary>
		/// Returns a string containing trigger parameters.
		/// </summary>
		public abstract string ParamsString { get; }

		/// <summary>
		/// Returns TypeString + " " + ParamsString.
		/// </summary>
		public override string ToString() => TypeString + " " + ParamsString;

		internal bool MatchScopeWindowAndFunc(TriggerHookContext thc)
		{
			try {
				for(int i = 0; i < 3; i++) {
					if(i == 1) {
						if(scope != null) {
							thc.PerfStart();
							bool ok = scope.Match(thc);
							thc.PerfEnd(false, ref scope.perfTime);
							if(!ok) return false;
						}
					} else {
						var af = i == 0 ? _funcBefore : _funcAfter;
						if(af != null) {
							foreach(var v in af) {
								thc.PerfStart();
								bool ok = v.f(thc.args);
								thc.PerfEnd(true, ref v.perfTime);
								if(!ok) return false;
							}
						}
					}
				}
			}
			catch(Exception ex) when(!(ex is ThreadAbortException)) {
				Print(ex);
				return false;
			}
			return true;

			//never mind: when same scope used several times (probably with different functions),
			//	should compare it once, and don't call 'before' functions again if did not match. Rare.
		}

		internal bool CallFunc(TriggerArgs args)
		{
#if true
			if(_funcAfter != null) {
				try {
					foreach(var v in _funcAfter) {
						var t1 = ATime.PerfMilliseconds;
						bool ok = v.f(args);
						var td = ATime.PerfMilliseconds - t1;
						if(td > 200) PrintWarning($"Too slow Triggers.FuncOf function of a window trigger. Should be < 10 ms, now {td} ms. Task name: {ATask.Name}.", -1);
						if(!ok) return false;
					}
				}
				catch(Exception ex) when(!(ex is ThreadAbortException)) {
					Print(ex);
					return false;
				}
			}
#else
			for(int i = 0; i < 2; i++) {
				var af = i == 0 ? _funcBefore : _funcAfter;
				if(af != null) {
					foreach(var v in af) {
						bool ok = v.f(args);
						if(!ok) return false;
					}
				}
			}
#endif
			return true;
			//SHOULDDO: measure time more intelligently, like in MatchScope, but maybe give more time.
		}

		internal bool HasFunc => _funcBefore != null || _funcAfter != null;

		//probably not useful. Or also need a property for eg HotkeyTriggers in inherited classes.
		///// <summary>
		///// The <see cref="ActionTriggers"/> instance to which this trigger belongs.
		///// </summary>
		//public ActionTriggers Triggers => triggers;

		/// <summary>
		/// Gets or sets whether this trigger is disabled.
		/// Does not depend on <see cref="ActionTriggers.Disabled"/>, <see cref="ActionTriggers.DisabledEverywhere"/>, <see cref="EnabledAlways"/>.
		/// </summary>
		public bool Disabled { get; set; }

		/// <summary>
		/// Returns true if <see cref="Disabled"/>; also if <see cref="ActionTriggers.Disabled"/> or <see cref="ActionTriggers.DisabledEverywhere"/>, unless <see cref="EnabledAlways"/>.
		/// </summary>
		public bool DisabledThisOrAll => Disabled || (!EnabledAlways && (triggers.Disabled | ActionTriggers.DisabledEverywhere));

		/// <summary>
		/// Gets or sets whether this trigger ignores <see cref="ActionTriggers.Disabled"/> and <see cref="ActionTriggers.DisabledEverywhere"/>.
		/// </summary>
		/// <remarks>
		/// When adding the trigger, this property is set to the value of <see cref="TriggerOptions.EnabledAlways"/> at that time.
		/// </remarks>
		public bool EnabledAlways { get; set; }

		/// <summary>
		/// Starts the action like when its trigger is activated.
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="InvalidOperationException">Called in a wrong place or from a wrong thread. More info in Ramarks.</exception>
		/// <remarks>
		/// Must be called while <see cref="ActionTriggers.Run"/> is running, from the same thread that called it.
		/// </remarks>
		public void RunAction(TriggerArgs args)
		{
			triggers.LibThrowIfNotRunning();
			if(AThread.NativeId != triggers.LibThreadId) throw new InvalidOperationException("Wrong thread. Call from a FuncOf function.");
			triggers.LibRunAction(this, args);
		}
	}

	/// <summary>
	/// Base of trigger action argument classes of all trigger types.
	/// </summary>
	public abstract class TriggerArgs
	{
	}

	/// <summary>
	/// Allows to specify working windows for multiple triggers of these types: hotkey, autotext, mouse.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// Triggers.Hotkey["Ctrl+K"] = o => Print("this trigger works with all windows");
	/// Triggers.Of.Window("* Notepad"); //specifies a working window for triggers added afterwards
	/// Triggers.Hotkey["Ctrl+F11"] = o => Print("this trigger works only when a Notepad window is active");
	/// Triggers.Hotkey["Ctrl+F12"] = o => Print("this trigger works only when a Notepad window is active");
	/// var wordpad = Triggers.Of.Window("* WordPad"); //specifies another working window for triggers added afterwards
	/// Triggers.Hotkey["Ctrl+F11"] = o => Print("this trigger works only when a WordPad window is active");
	/// Triggers.Hotkey["Ctrl+F12"] = o => Print("this trigger works only when a WordPad window is active");
	/// Triggers.Of.AllWindows(); //let triggers added afterwards work with all windows
	/// Triggers.Mouse[TMEdge.RightInTop25] = o => Print("this trigger works with all windows");
	/// Triggers.Of.Again(wordpad); //sets a previously specified working window for triggers added afterwards
	/// Triggers.Mouse[TMEdge.RightInBottom25] = o => Print("this trigger works only when a WordPad window is active");
	/// Triggers.Mouse[TMMove.DownUp] = o => Print("this trigger works only when a WordPad window is active");
	/// Triggers.Mouse[TMClick.Middle] = o => Print("this trigger works only when the mouse is in a WordPad window");
	/// Triggers.Mouse[TMWheel.Forward] = o => Print("this trigger works only when the mouse is in a WordPad window");
	/// Triggers.Run();
	/// ]]></code>
	/// </example>
	public class TriggerScopes
	{
		internal TriggerScopes() { }

		internal TriggerScope Current { get; private set; }

		/// <summary>
		/// Sets scope "all windows" again. Hotkey, autotext and mouse triggers added afterwards will work with all windows.
		/// </summary>
		/// <remarks>
		/// Example in class help.
		/// </remarks>
		public void AllWindows() => Current = null;

		/// <summary>
		/// Sets (reuses) a previously specified scope.
		/// </summary>
		/// <remarks>
		/// Example in class help.
		/// </remarks>
		/// <param name="scope">The return value of function <b>Window</b>, <b>NotWindow</b>, <b>Windows</b> or <b>NotWindows</b>.</param>
		public void Again(TriggerScope scope) => Current = scope;

		/// <summary>
		/// Sets scope "only this window". Hotkey, autotext and mouse triggers added afterwards will work only when the specified window is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		/// <remarks>
		/// Parameters are like with <see cref="AWnd.Find"/>.
		/// Example in class help.
		/// </remarks>
		/// <exception cref="ArgumentException">Exceptions of <see cref="AWnd.Finder"/> constructor.</exception>
		public TriggerScope Window(
			[ParamString(PSFormat.AWildex)] string name = null,
			[ParamString(PSFormat.AWildex)] string cn = null,
			[ParamString(PSFormat.AWildex)] WF3 program = default,
			Func<AWnd, bool> also = null, object contains = null)
			=> _Window(false, name, cn, program, also, contains);

		/// <summary>
		/// Sets scope "not this window". Hotkey, autotext and mouse triggers added afterwards will not work when the specified window is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		/// <remarks>
		/// Parameters are like with <see cref="AWnd.Find"/>.
		/// Example in class help.
		/// </remarks>
		/// <exception cref="ArgumentException">Exceptions of <see cref="AWnd.Finder"/> constructor.</exception>
		public TriggerScope NotWindow(
			[ParamString(PSFormat.AWildex)] string name = null,
			[ParamString(PSFormat.AWildex)] string cn = null,
			[ParamString(PSFormat.AWildex)] WF3 program = default,
			Func<AWnd, bool> also = null, object contains = null)
			=> _Window(true, name, cn, program, also, contains);

		TriggerScope _Window(bool not, string name, string cn, WF3 program, Func<AWnd, bool> also, object contains)
			=> _Add(not, new AWnd.Finder(name, cn, program, 0, also, contains));

		/// <summary>
		/// Sets scope "only this window". Hotkey, autotext and mouse triggers added afterwards will work only when the specified window is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		public TriggerScope Window(AWnd.Finder f)
			=> _Add(false, f);

		/// <summary>
		/// Sets scope "not this window". Hotkey, autotext and mouse triggers added afterwards will not work when the specified window is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		public TriggerScope NotWindow(AWnd.Finder f)
			=> _Add(true, f);

		/// <summary>
		/// Sets scope "only this window". Hotkey, autotext and mouse triggers added afterwards will work only when the specified window is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public TriggerScope Window(AWnd w)
			=> _Add(false, w);

		/// <summary>
		/// Sets scope "not this window". Hotkey, autotext and mouse triggers added afterwards will not work when the specified window is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		/// <exception cref="AuWndException">Invalid window handle.</exception>
		public TriggerScope NotWindow(AWnd w)
			=> _Add(true, w);

		/// <summary>
		/// Sets scope "only these windows". Hotkey, autotext and mouse triggers added afterwards will work only when one of the specified windows is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		/// <param name="any">
		/// Specifies one or more windows.
		/// Supported object types: <see cref="AWnd.Finder"/>, <see cref="AWnd"/>, string.
		/// If string, its format must be like with <see cref="AWnd.Finder.op_Implicit(string)"/>. Examples: "Name", "Name,Class", ",,Program.exe".
		/// The easiest way to specify "all windows of program X.exe": <c>Triggers.Of.Windows(",,X.exe")</c>.
		/// </param>
		/// <exception cref="ArgumentException">Unsupported object type.</exception>
		/// <exception cref="AuWndException">Invalid window handle (when object type is <b>AWnd</b>).</exception>
		public TriggerScope Windows(params object[] any)
			=> _Add(false, any);

		/// <summary>
		/// Sets scope "not these windows". Hotkey, autotext and mouse triggers added afterwards will not work when one of the specified windows is active.
		/// </summary>
		/// <returns>Returns an object that can be later passed to <see cref="Again"/> to reuse this scope.</returns>
		/// <param name="any">See <see cref="Windows"/>.</param>
		/// <exception cref="ArgumentException">Unsupported object type.</exception>
		/// <exception cref="AuWndException">Invalid window handle (when object type is <b>AWnd</b>).</exception>
		public TriggerScope NotWindows(params object[] any)
			=> _Add(true, any);

		TriggerScope _Add(bool not, object o)
		{
			switch(o) {
			case null: throw new ArgumentNullException();
			case AWnd w: w.ThrowIf0(); break;
			case object[] a:
				if(a.Length > 1) a = (o = a.Clone()) as object[]; //don't overwrite elements of array passed as argument. In most cases it contains strings.
				for(int j = 0; j < a.Length; j++) {
					switch(a[j]) {
					case AWnd.Finder _: break;
					case AWnd w: w.ThrowIf0(); break;
					case string s:
						var f = (AWnd.Finder)s;
						if(a.Length > 1) a[j] = f; else o = f;
						break;
					case null: throw new ArgumentNullException();
					default: throw new ArgumentException("Unsupported object type.");
					}
				}
				break;
			}

			Used = true;
			return Current = new TriggerScope(o, not);
		}

		internal bool Used { get; private set; }
	}

	/// <summary>
	/// A trigger scope returned by functions like <see cref="TriggerScopes.Window"/> and used with <see cref="TriggerScopes.Again"/>.
	/// </summary>
	/// <example>See <see cref="TriggerScopes"/>.</example>
	public class TriggerScope
	{
		internal readonly object o; //AWnd.Finder, AWnd, object<AWnd.Finder|AWnd>[]
		internal readonly bool not;
		internal int perfTime;

		internal TriggerScope(object o, bool not)
		{
			this.o = o;
			this.not = not;
		}

		/// <summary>
		/// Returns true if window matches.
		/// </summary>
		/// <param name="thc">This func uses the window handle (gets on demand) and WFCache.</param>
		internal bool Match(TriggerHookContext thc)
		{
			bool yes = false;
			var w = thc.Window;
			if(!w.Is0) {
				switch(o) {
				case AWnd.Finder f:
					yes = f.IsMatch(w, thc);
					break;
				case AWnd hwnd:
					yes = w == hwnd;
					break;
				case object[] a:
					foreach(var v in a) {
						switch(v) {
						case AWnd.Finder f1: yes = f1.IsMatch(w, thc); break;
						case AWnd w1: yes = (w == w1); break;
						}
						if(yes) break;
					}
					break;
				}
			}
			return yes ^ not;
		}
	}

	/// <summary>
	/// Allows to define custom scopes/contexts/conditions for triggers.
	/// </summary>
	/// <remarks>
	/// Similar to <see cref="TriggerScopes"/> (code like <c>Triggers.Of.Window(...);</c>), but allows to define any scope/condition/etc, not just the active window.
	/// 
	/// To define a scope, you create a callback function (CF) that checks some conditions and returns true to allow the trigger action to run or false to not allow. Assign the CF to some property of this class and then add the trigger, like in the examples below. The CF will be assigned to the trigger and called when need.
	/// 
	/// You may ask: why to use CF when the trigger action (TA) can do the same?
	/// 1. CF runs synchronously; if it returns false, the trigger key or mouse button message is passed to other triggers, hooks and apps. TA cannot do it reliably; it runs asynchronously, and the message is already stealed from other apps/triggers/hooks.
	/// 2. CF is faster to call. It is simply called in the same thread that processes trigger messages. TA runs in another thread.
	/// 3. A CF can be assigned to multiple triggers with a single line of code. Don't need to add the same code in all trigger actions.
	/// 
	/// A trigger can have up to 4 CF delegates and a window scope (<c>Triggers.Of...</c>). They are called in this order: CF assigned through <see cref="FollowingTriggersBeforeWindow"/>, <see cref="NextTriggerBeforeWindow"/>, window scope, <see cref="NextTrigger"/>, <see cref="FollowingTriggers"/>. The <b>NextX</b> properties assign the CF to the next single trigger. The <b>FollowingX</b> properties assign the CF to all following triggers until you assign another CF or null. If several are assigned, the trigger action runs only if all CF return true and the window scope matches. The <b>XBeforeWindow</b> properties are used only with hotkey, autotext and mouse triggers.
	/// 
	/// All CF must be as fast as possible. Slow CF can make triggers slower (or even all keyboard/mouse input); also may cause warnings and trigger failures. A big problem is the low-level hooks timeout that Windows applies to trigger hooks; see <see cref="AHookWin.LowLevelHooksTimeout"/>. A related problem - slow JIT and loading of assemblies, which can make the CF too slow the first time; in some rare cases may even need to preload assemblies or pre-JIT functions to avoid the timeout warning.
	///
	/// In CF never use functions that generate keyboard or mouse events or activate windows.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// //examples of assigning a CF to a single trigger
	/// Triggers.FuncOf.NextTrigger = o => AKeys.IsCapsLock; //o => AKeys.IsCapsLock is the callback function (lambda)
	/// Triggers.Hotkey["Ctrl+K"] = o => Print("action: Ctrl+K while CapsLock is on");
	/// Triggers.FuncOf.NextTrigger = o => { var v = o as HotkeyTriggerArgs; Print($"func: mod={v.Mod}"); return AMouse.IsPressed(MButtons.Left); };
	/// Triggers.Hotkey["Ctrl+Shift?+B"] = o => Print("action: mouse left button + Ctrl+B or Ctrl+Shift+B");
	/// 
	/// //examples of assigning a CF to multiple triggers
	/// Triggers.FuncOf.FollowingTriggers = o => { var v = o as HotkeyTriggerArgs; Print("func", v.Trigger); return true; };
	/// Triggers.Hotkey["Ctrl+F8"] = o => Print("action: " + o.Trigger);
	/// Triggers.Hotkey["Ctrl+F9"] = o => Print("action: " + o.Trigger);
	/// Triggers.FuncOf.FollowingTriggers = null; //stop assigning the CF to triggers added afterwards
	/// 
	/// //sometimes all work can be done in CF and you don't need the trigger action
	/// Triggers.FuncOf.NextTrigger = o => { var v = o as HotkeyTriggerArgs; Print("func: " + v.Trigger); return true; };
	/// Triggers.Hotkey["Ctrl+F12"] = null;
	/// 
	/// Triggers.Run();
	/// ]]></code>
	/// </example>
	public class TriggerFuncs
	{
		internal TriggerFuncs() { }

		internal Dictionary<TFunc, TriggerFunc> perfDict = new Dictionary<TFunc, TriggerFunc>();

		internal bool Used { get; private set; }

		internal TFunc nextAfter, nextBefore, commonAfter, commonBefore;

		/// <summary>
		/// Sets callback function for the next added trigger.
		/// If the trigger has a window scope, the callback function is called after evaluating the window.
		/// This function is used with triggers of all types.
		/// </summary>
		public TFunc NextTrigger {
			get => nextAfter;
			set => nextAfter = _Func(value);
		}

		/// <summary>
		/// Sets callback function for the next added trigger.
		/// If the trigger has a window scope, the callback function is called before evaluating the window.
		/// This function is used with triggers of these types: hotkey, autotext, mouse.
		/// </summary>
		public TFunc NextTriggerBeforeWindow {
			get => nextBefore;
			set => nextBefore = _Func(value);
		}

		/// <summary>
		/// Sets callback function for multiple triggers added afterwards.
		/// If the trigger has a window scope, the callback function is called after evaluating the window.
		/// This function is used with triggers of all types.
		/// The value can be null.
		/// </summary>
		public TFunc FollowingTriggers {
			get => commonAfter;
			set => commonAfter = _Func(value);
		}

		/// <summary>
		/// Sets callback function for multiple triggers added afterwards.
		/// If the trigger has a window scope, the callback function is called before evaluating the window.
		/// This function is used with triggers of these types: hotkey, autotext, mouse.
		/// The value can be null.
		/// </summary>
		public TFunc FollowingTriggersBeforeWindow {
			get => commonBefore;
			set => commonBefore = _Func(value);
		}

		TFunc _Func(TFunc f)
		{
			if(f != null) Used = true;
			return f;
		}
	}

	class TriggerFunc
	{
		internal TFunc f;
		internal int perfTime;
	}

	/// <summary>
	/// Type of functions used with class <see cref="TriggerFuncs"/> to define custom scope for triggers.
	/// </summary>
	/// <param name="args">Trigger action arguments. Example: <see cref="TriggerFuncs"/>.</param>
	/// <returns>Return true to run the trigger action, or false to not run.</returns>
	public delegate bool TFunc(TriggerArgs args);
}
