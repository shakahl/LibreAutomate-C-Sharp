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
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	public abstract class Trigger
	{
		internal Trigger next; //linked list when eg same hotkey is used in multiple scopes
		internal readonly Delegate action;
		internal readonly AuTriggers triggers;
		internal readonly TOptions options; //Triggers.Options
		internal readonly TriggerScope scope; //Triggers.Of.WindowX. Used by hotkey, autotext and mouse triggers.
		readonly TriggerFunc[] _funcAfter, _funcBefore; //Triggers.FuncOf. _funcAfter used by all triggers; _funcBefore - like scope.

		internal Trigger(AuTriggers triggers, Delegate action, bool usesWindowScope)
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
			triggers.Last.trigger = this;

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

		internal void DictAdd<TKey>(Dictionary<TKey, Trigger> d, TKey key)
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

		public abstract string TypeString();
		public abstract string ShortString();

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
						var t1 = Time.PerfMilliseconds;
						bool ok = v.f(args);
						var td = Time.PerfMilliseconds - t1;
						if(td > 200) PrintWarning($"Too slow Triggers.FuncOf function of a window trigger. Should be < 10 ms, now {td} ms. Task name: {AuTask.Name}.", -1);
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
		///// The <see cref="AuTriggers"/> instance to which this trigger belongs.
		///// </summary>
		//public AuTriggers Triggers => triggers;

		/// <summary>
		/// Gets or sets whether this trigger is disabled.
		/// Does not depend on <see cref="AuTriggers.Disabled"/>, <see cref="AuTriggers.DisabledEverywhere"/>, <see cref="EnabledAlways"/>.
		/// </summary>
		public bool Disabled { get; set; }

		/// <summary>
		/// Returns true if <see cref="Disabled"/>; also if <see cref="AuTriggers.Disabled"/> or <see cref="AuTriggers.DisabledEverywhere"/>, unless <see cref="EnabledAlways"/>.
		/// </summary>
		public bool DisabledThisOrAll => Disabled || (!EnabledAlways && (triggers.Disabled | AuTriggers.DisabledEverywhere));

		/// <summary>
		/// Gets or sets whether this trigger ignores <see cref="AuTriggers.Disabled"/> and <see cref="AuTriggers.DisabledEverywhere"/>.
		/// </summary>
		/// <remarks>
		/// When adding the trigger, this property is set to the value of <see cref="TriggerOptions.EnabledAlways"/> at that time.
		/// </remarks>
		public bool EnabledAlways { get; set; }
	}

	/// <summary>
	/// Base of trigger action argument classes of all trigger types.
	/// </summary>
	public class TriggerArgs
	{
	}

	public class TriggerScopes
	{
		internal TriggerScope Current { get; private set; }

		public void AllWindows() => Current = null;

		public void Again(TriggerScope scope) => Current = scope;

		public TriggerScope Window(string name = null, string cn = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null)
			=> _Window(false, name, cn, program, also, contains);

		public TriggerScope NotWindow(string name = null, string cn = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null)
			=> _Window(true, name, cn, program, also, contains);

		TriggerScope _Window(bool not, string name, string cn, WF3 program, Func<Wnd, bool> also, object contains)
			=> _Add(not, new Wnd.Finder(name, cn, program, 0, also, contains));

		public TriggerScope Window(Wnd.Finder f)
			=> _Add(false, f);

		public TriggerScope NotWindow(Wnd.Finder f)
			=> _Add(true, f);

		public TriggerScope Window(Wnd w)
			=> _Add(false, w);

		public TriggerScope NotWindow(Wnd w)
			=> _Add(true, w);

		public TriggerScope Windows(params object[] any)
			=> _Add(false, any);

		public TriggerScope NotWindows(params object[] any)
			=> _Add(true, any);

		TriggerScope _Add(bool not, object o)
		{
			switch(o) {
			case null: throw new ArgumentNullException();
			case Wnd w: w.ThrowIf0(); break;
			case object[] a:
				if(a.Length > 1) a = (o = a.Clone()) as object[]; //don't overwrite elements of array passed as argument. In most cases it contains strings.
				for(int j = 0; j < a.Length; j++) {
					switch(a[j]) {
					case Wnd.Finder _: break;
					case Wnd w: w.ThrowIf0(); break;
					case string s:
						var f = (Wnd.Finder)s;
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

	public class TriggerScope
	{
		internal readonly object o; //Wnd.Finder, Wnd, object<Wnd.Finder|Wnd>[]
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
				case Wnd.Finder f:
					yes = f.IsMatch(w, thc);
					break;
				case Wnd hwnd:
					yes = w == hwnd;
					break;
				case object[] a:
					foreach(var v in a) {
						switch(v) {
						case Wnd.Finder f1: yes = f1.IsMatch(w, thc); break;
						case Wnd w1: yes = (w == w1); break;
						}
						if(yes) break;
					}
					break;
				}
			}
			return yes ^ not;
		}
	}

	public class TriggerFuncs
	{
		internal Dictionary<TFunc, TriggerFunc> perfDict = new Dictionary<TFunc, TriggerFunc>();

		internal bool Used { get; private set; }

		internal TFunc nextAfter, nextBefore, commonAfter, commonBefore;

		public TFunc NextTrigger {
			get => nextAfter;
			set => nextAfter = _Func(value);
		}

		public TFunc NextTriggerBeforeWindow {
			get => nextBefore;
			set => nextBefore = _Func(value);
		}

		public TFunc FollowingTriggers {
			get => commonAfter;
			set => commonAfter = _Func(value);
		}

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

	public delegate bool TFunc(TriggerArgs args);
}
