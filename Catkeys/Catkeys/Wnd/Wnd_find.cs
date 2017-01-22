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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public partial struct Wnd
	{
		/// <summary>
		/// Base of Wnd.WinProp and Wnd.ChildProp.
		/// All strings are case-insensitive.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public abstract class WinProp_ChildProp_Base
		{
			/// <summary>
			/// The window must have all these styles (Api.WS_x). Reference: MSDN.
			/// </summary>
			public uint styleHas;
			/// <summary>
			/// The window must not have any of these styles (Api.WS_x). Reference: MSDN.
			/// </summary>
			public uint styleNot;
			/// <summary>
			/// The window must have all these extended styles (Api.WS_EX_x). Reference: MSDN.
			/// </summary>
			public uint exStyleHas;
			/// <summary>
			/// The window must not have any of these extended styles (Api.WS_EX_x). Reference: MSDN.
			/// </summary>
			public uint exStyleNot;
			/// <summary>
			/// The window must have this window property. Reference: MSDN -> GetProp, SetProp, RemoveProp, EnumProps.
			/// </summary>
			public string propName;
			/// <summary>
			/// When propName used, the window property must have this value. If 0, can have any non-zero value.
			/// </summary>
			public LPARAM propValue;
			/// <summary>
			/// The control must contain this x coordinate in parent window's client area. If top-level window - relative to the primary screen.
			/// Can be int (pixels) or float (fraction of client area, eg 0.5 is middle).
			/// </summary>
			public Coord x;
			/// <summary>
			/// The control must contain this y coordinate in parent window's client area. If top-level window - relative to the primary screen.
			/// Can be int (pixels) or float (fraction of client area, eg 0.5 is middle).
			/// Example of getting control that is at the very right of the parent window w: <c>Wnd c=w.Child(... prop:new Wnd.ChildProp() { x=w.ClientWidth-1 });</c>
			/// </summary>
			public Coord y;
			/// <summary>
			/// The window must have a child control that has this name/text.
			/// This value will be passed to <see cref="Child"/>, together with childClass and childId.
			/// <see cref="Wildex">Wildcard expression</see>.
			/// </summary>
			public string childName;
			/// <summary>
			/// The window must have a child control that has this class name.
			/// This value will be passed to <see cref="Child"/>, together with childName and childId.
			/// <see cref="Wildex">Wildcard expression</see>.
			/// </summary>
			public string childClass;
			/// <summary>
			/// The window must have a child control that has this non-zero id.
			/// This value will be passed to <see cref="Child"/>, together with childName and childClass.
			/// </summary>
			public int childId;

			internal POINT _xy;
			ushort _propAtom;
			internal Wnd.ChildDefinition _child;

			//TODO: Maybe remove childName,childClass, childId etc, because easier to use lambda for it. Maybe don't need entire prop.
			//TODO, when code or experience will be available:
			//1. public UIElemDefinition elem; or/and elemRole, elemName. Also could add public ImageDefinition image, but better use lambda for it.


			internal bool _Init()
			{
				_propAtom = 0;
				if(!Empty(propName)) {
					_propAtom = Api.GlobalFindAtom(propName);
					if(_propAtom == 0) return false;
				} else propName = null;

				if(childName != null || childClass != null || childId != 0) {
					_child = new Wnd.ChildDefinition(childName, childClass, childId);
				}

				return true;
			}

			internal bool MatchPropStylesXY(Wnd w, Wnd wParent = default(Wnd))
			{
				if(_propAtom != 0) {
					LPARAM prop = w.GetProp(_propAtom);
					if(propValue != 0) { if(prop != propValue) return false; } //must match value
					else { if(prop == 0) return false; } //can be any non-zero value //note: cannot use 'can be any value including 0' because GetLastError returns 0 when using atom that exists somewhere else
				}

				if((styleHas | styleNot) != 0) {
					uint u = w.Style;
					if(styleHas != 0 && (u & styleHas) != styleHas) return false;
					if(styleNot != 0 && (u & styleNot) != 0) return false;
				}

				if((exStyleHas | exStyleNot) != 0) {
					uint u = w.ExStyle;
					if(exStyleHas != 0 && (u & exStyleHas) != exStyleHas) return false;
					if(exStyleNot != 0 && (u & exStyleNot) != 0) return false;
				}

				if(x != null || y != null) {
					RECT r; if(!w.GetRectInClientOf(wParent, out r)) return false;
					if(!r.Contains(x == null ? r.left : _xy.x, y == null ? r.top : _xy.y)) return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Contains various window properties to pass to Wnd.Find() 'prop' parameter.
		/// </summary>
		public class WinProp :WinProp_ChildProp_Base
		{
			/// <summary>
			/// Owner window.
			/// </summary>
			public Wnd owner;
			/// <summary>
			/// Process id.
			/// </summary>
			public uint processId;
			/// <summary>
			/// Thread id.
			/// </summary>
			public uint threadId;

			//public static implicit operator WinProp(string s)
			//{
			//	return null;
			//}

			/// <summary>
			/// Call this before starting to search for the window.
			/// Returns false if the window cannot be found, eg prop atom not found.
			/// </summary>
			internal bool Init()
			{
				if(!_Init()) return false;

				_xy = Coord.GetNormalizedInScreen(x, y);

				return true;
			}
		}

		/// <summary>
		/// 'flags' parameter of <see cref="Wnd.Find"/>.
		/// </summary>
		[Flags]
		public enum WinFlag
		{
			/// <summary>Can find hidden windows. Use this carefully, always use className, not just name, because there are many hidden tooltip windows etc that could match the name.</summary>
			HiddenToo = 1,
			/// <summary>Skip cloaked windows. These are windows hidden not in the classic way (Wnd.IsVisible does not detect it, Wnd.Cloaked detects). For example, windows on inactive Windows 10 virtual desktops, hidden Windows Store apps on Windows 8.</summary>
			SkipCloaked = 2,
			/// <summary>The 'program' argument is full path. Need this flag because the function cannot auto-detect it when using wildcard, regex etc.</summary>
			ProgramPath = 4,
		}

		/// <summary>
		/// Arguments of <see cref="Wnd.Find"/>.
		/// </summary>
		public class WindowDefinition
		{
			readonly Wildex _name;
			readonly Wildex _className;
			readonly Wildex _program;
			readonly WinProp _prop;
			readonly Func<Wnd, bool> _f;
			readonly int _matchIndex;
			readonly WinFlag _flags;

			///
			public WindowDefinition(
				string name = null, string className = null, string program = null,
				WinFlag flags = 0, WinProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
				)
			{
				_ThrowIfStringEmptyNotNull(className, nameof(className));
				_ThrowIfStringEmptyNotNull(program, nameof(program));

				_name = name;
				_className = className;
				_program = program;
				_flags = flags;
				_prop = prop;
				_f = f;
				_matchIndex = matchIndex;
			}

			/// <summary>
			/// The found window.
			/// </summary>
			public Wnd Result { get; private set; }

			/// <summary>
			/// Finds window, like Wnd.Find().
			/// Returns true if found.
			/// The Result property will be the window.
			/// </summary>
			public bool Find()
			{
				return FindInList(AllWindows(!_flags.HasFlag(WinFlag.HiddenToo), true)) >= 0;
			}

			static WinProp _propEmpty;

			/// <summary>
			/// Finds matching window in list of windows (handles).
			/// Returns 0-based index, or -1 if not found.
			/// The Result property will be the window.
			/// Does not skip hidden windows, even if flag HiddenToo is not set.
			/// </summary>
			/// <param name="a">List of windows (handles), for example returned by Wnd.AllWindows.</param>
			public int FindInList(List<Wnd> a)
			{
				Result = Wnd0;
				if(a == null || a.Count == 0) return -1;

				var p = _prop;
				if(p == null) p = _propEmpty ?? (_propEmpty = new WinProp()); //will not need if(p != null && ...)
				else if(!p.Init()) return -1;

				List<uint> pids = null; bool programNamePlanB = false; //variables for faster getting/matching program name
				int matchInd = _matchIndex;

				int index = -1;
				foreach(Wnd w in a) {
					index++;

					//speed of 1000 times getting:
					//name 400, class 400, foreign pid/tid 400,
					//owner 55, rect 55, style 50, exstyle 50, cloaked 280,
					//GetProp(string) 1700, GetProp(atom) 300, GlobalFindAtom 650,
					//program >=2500

					if(!p.owner.Is0) {
						if(p.owner != w.Owner) continue;
					}

					if(_name != null) {
						if(!_name.Match(w.Name)) continue;
					}

					if(_className != null) {
						if(!_className.Match(w.ClassName)) continue;
					}

					uint pid = 0, tid = 0;
					if(_program != null || p.processId != 0 || p.threadId != 0) {
						tid = w.GetThreadAndProcessId(out pid);
						if(tid == 0) continue;
						//speed: with foreign processes the same speed as getting name or class name. Much faster if same process.
					}

					if(p.threadId != 0) {
						if(p.threadId != tid) continue;
					}

					if(p.processId != 0) {
						if(p.processId != pid) continue;
					}

					if(_program != null) {
						//Getting program name is one of slowest parts.
						//Usually it does not slow down much because need to do it only 1 or several times, only when window name, class etc match.
						//The worst case is when only program is specified, and the very worst case is when also using flag HiddenToo.
						//We are prepared for the worst case.
						//Normally we call Process_.GetProcessName. In most cases it is quite fast.
						//Anyway, we use this optimization:
						//	Add pid of processes that don't match the specified name in the pids list (bad pids).
						//	Next time, if pid is in the bad pids list, just continue, don't need to get program name again.
						//However in the worst case we would encounter some processes that Process_.GetProcessName cannot get name using the fast API.
						//For each such process it would then use the much slower 'get all processes' API, which is almost as slow as Process.GetProcessById(pid).ProcessName.
						//To solve this:
						//We tell Process_.GetProcessName to not use the slow API, but just return null when the fast API fails.
						//When it happens (Process_.GetProcessName returns null):
						//	If need full path: continue, we cannot do anything more.
						//	Switch to plan B and no longer use all the above. Plan B:
						//	Get list of pids of all processes that match _program. For it we call Process_.GetProcessesByName, which uses the same slow API, but we call it just one time.
						//	If it returns null (it means there are no matching processes), break (window not found).
						//	From now, in each loop will need just to find pid in the returned list, and continue if not found.

						g1:
						if(programNamePlanB) {
							if(!pids.Contains(pid)) continue;
						} else {
							if(pids != null && pids.Contains(pid)) continue; //is known bad pid?

							string pname = Process_.GetProcessName(pid, _flags.HasFlag(WinFlag.ProgramPath), true);

							if(pname == null) {
								if(_flags.HasFlag(WinFlag.ProgramPath)) continue;
								//switch to plan B
								pids = Process_.GetProcessesByName(_program);
								if(pids.Count == 0) break;
								programNamePlanB = true;
								goto g1;
							}

							if(!_program.Match(pname)) {
								if(pids == null) pids = new List<uint>();
								pids.Add(pid); //add bad pid
								continue;
							}
						}
					}

					if(!p.MatchPropStylesXY(w)) continue;

					if(_flags.HasFlag(WinFlag.SkipCloaked)) {
						if(w.IsCloaked) continue;
					}

					if(p._child != null) {
						if(!p._child.Find(w)) continue;
					}

					if(_f != null && !_f(w)) continue;

					if(--matchInd > 0) continue;

					Result = w;
					return index;
				}

				return -1;
			}
		}

		/// <summary>
		/// Finds window.
		/// Returns its handle as Wnd. Returns Wnd0 if not found.
		/// Example: <c>Wnd w=Wnd.Find("Name"); if(w.Is0) Print("not found");</c>
		/// </summary>
		/// <param name="name">Window name. Usually it is the title bar text. <see cref="Wildex">Wildcard expression</see> or null (null means 'any', "" means 'empty name').</param>
		/// <param name="className">Window class name. <see cref="Wildex">Wildcard expression</see>.</param>
		/// <param name="program">
		/// Program name without ".exe", or full path if using flag ProgramPath. <see cref="Wildex">Wildcard expression</see>.
		/// Can fail to get program name and especially path when the process has higher UAC integrity level; then assumes that the program name/path does not match.
		/// Getting program name or path is much slower than getting window name or class name, therefore this parameter should not be used alone.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="prop">More properties, child control, etc. Example: <c>new Wnd.WinProp(){ childClass="Static", childText="Text*" }</c></param>
		/// <param name="f">
		/// Lambda etc callback function to call for each matching window.
		/// It can evaluate more properties of the window and return true when they match.
		/// Example: <c>e =&gt; { Print(e); return e.Name=="Find"; }</c>
		/// </param>
		/// <param name="matchIndex">1-based index of matching window. For example, if matchIndex is 2, the function skips the first matching window and returns the second.</param>
		/// <remarks>
		/// If there are multiple matching windows, gets the first in the Z order matching window, preferring visible windows.
		/// On Windows 8 and later finds only desktop windows, not Windows Store app Metro-style windows (on Windows 10 only few such windows exist), unless this process has uiAccess; to find such windows you can use Wnd.FindRaw().
		/// </remarks>
		/// <exception cref="ArgumentException">className, program or prop.childClass is "". To match any, use null.</exception>
		public static Wnd Find(
			string name = null, string className = null, string program = null,
			WinFlag flags = 0, WinProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
			)
		{
			var d = new WindowDefinition(name, className, program, flags, prop, f, matchIndex);
			d.Find();
			return d.Result;
		}

		/// <summary>
		/// Calls <see cref="Find"/> with flag WinFlag.HiddenToo.
		/// </summary>
		/// <seealso cref="Find"/>
		public static Wnd FindH(
			string name = null, string className = null, string program = null,
			WinProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
			)
		{
			return Find(name, className, program, WinFlag.HiddenToo, prop, f, matchIndex);
		}

		/// <summary>
		/// Finds window by class name and optionally name.
		/// Calls Api.FindWindowEx().
		/// Faster than Find(), which uses Api.EnumWindows(), but slower when used in a loop to enumerate all windows.
		/// Does not skip hidden and cloaked windows, and does not prefer visible windows like Find().
		/// </summary>
		/// <param name="className">Class name. Full, case-insensitive. Wildcard etc not supported. Can be null to match any.</param>
		/// <param name="fullName">Name. Full, case-insensitive. Wildcard etc not supported. Can be omitted or null to match any.</param>
		/// <param name="wAfter">If used, starts searching from the next window in the Z order.</param>
		public static Wnd FindRaw(string className, string fullName = null, Wnd wAfter = default(Wnd))
		{
			return Api.FindWindowEx(Wnd0, wAfter, className, fullName);
		}

		/// <summary>
		/// Finds all matching windows.
		/// Returns list containing 0 or more window handles as Wnd.
		/// Everything except the return type is the same as with <see cref="Find"/>.
		/// The list is sorted to match the Z order, however hidden windows (when using WinFlag.HiddenToo) are always placed after visible windows.
		/// </summary>
		public static List<Wnd> FindAll(
			string name = null, string className = null, string program = null,
			WinFlag flags = 0, WinProp prop = null, Func<Wnd, bool> f = null
			)
		{
			var a = new List<Wnd>();
			Find(name, className, program, flags, prop, e =>
			{
				if(f == null || f(e)) a.Add(e);
				return false;
			});
			return a;
		}

		//public static List<Wnd> FindAll(params WindowDefinition[] a)
		//{
		//	return null;
		//}

		//public static Wnd FindAny(params WindowDefinition[] a)
		//{
		//	return Wnd0;
		//}

		/// <summary>
		/// Gets list of top-level windows.
		/// Returns list containing 0 or more window handles as Wnd.
		/// Uses Api.EnumWindows().
		/// By default the list elements are sorted to match the Z order.
		/// </summary>
		/// <param name="onlyVisible">Need only visible windows.</param>
		/// <param name="sortFirstVisible">Place all list elements of hidden windows at the end of the returned list, even if the hidden windows are before some visible windows in the Z order.</param>
		/// <remarks>
		/// On Windows 8 and later gets only desktop windows, not Windows Store app Metro-style windows (on Windows 10 only few such windows exist), unless this process has uiAccess; to get such windows you can use Wnd.FindRaw().
		/// </remarks>
		public static List<Wnd> AllWindows(bool onlyVisible = false, bool sortFirstVisible = false)
		{
			List<Wnd> a = new List<Wnd>(), aHidden = null;
			if(onlyVisible) sortFirstVisible = false;

			AllWindows(e =>
			{
				if(sortFirstVisible && !e.IsVisible) {
					if(aHidden == null) aHidden = new List<Wnd>(256);
					aHidden.Add(e);
				} else {
					a.Add(e);
				}
				return false;
			}, onlyVisible);

			if(aHidden != null) a.AddRange(aHidden);
			return a;

			//info: tried to add a flag to skip tooltips, IME, MSCTFIME UI. But for it need to get class. It is slow. Other ways are unreliable and also make slower. Only the onlyVisible flag is really effective.
		}

		/// <summary>
		/// Calls callback function for each window.
		/// Uses Api.EnumWindows().
		/// </summary>
		/// <param name="f">Lambda etc callback function to call for each matching window. Can return true to stop.</param>
		/// <param name="onlyVisible">Need only visible windows.</param>
		/// <remarks>
		/// On Windows 8 and later gets only desktop windows, not Windows Store app Metro-style windows (on Windows 10 only few such windows exist), unless this process has uiAccess; to get such windows you can use Wnd.FindRaw().
		/// </remarks>
		public static void AllWindows(Func<Wnd, bool> f, bool onlyVisible = false)
		{
			Api.EnumWindows((w, param) =>
			{
				if(onlyVisible && !w.IsVisible) return 1;
				return f(w) ? 0 : 1;
			}, Zero);
		}

		//Better don't use this.
		//Why tried to use this? Because once one IME window was visible. IME windows are zero-size, disabled. Tested only on Win10.
		//bool _HiddenOrZeroSize
		//{
		//	get
		//	{
		//		if(!IsVisible) return true;
		//		RECT r; if(!Api.GetWindowRect(this, out r)) return true;
		//		return r.left == 0 && r.top == 0 && r.right == 0 && r.bottom == 0;
		//	}
		//}

		/// <summary>
		/// Gets list of top-level windows of current thread or another thread.
		/// Returns list containing 0 or more window handles as Wnd.
		/// Uses Api.EnumThreadWindows().
		/// </summary>
		/// <param name="threadId">Unmanaged thread id. If 0, gets windows of current thread.</param>
		/// <param name="className">Need only windows of this class. <see cref="Wildex">Wildcard expression</see> or null (null means 'any').</param>
		/// <param name="onlyVisible">Need only visible windows.</param>
		public static List<Wnd> ThreadWindows(uint threadId = 0, string className = null, bool onlyVisible = false)
		{
			if(threadId == 0) threadId = Api.GetCurrentThreadId();
			var a = new List<Wnd>();
			_ThrowIfStringEmptyNotNull(className, nameof(className));
			Wildex wildCN = className;

			Api.EnumThreadWindows(threadId, (w, param) =>
			{
				if(onlyVisible && !w.IsVisible) return 1;
				if(wildCN != null && !wildCN.Match(w.ClassName)) return 1;
				a.Add(w);
				return 1;
			}, 0);

			return a;

			//speed: ~40% of EnumWindows time, tested with a foreign thread with 30 windows.
		}

		static void _ThrowIfStringEmptyNotNull(string s, string paramName)
		{
			if(s != null && s.Length == 0) throw new ArgumentException("Cannot be \"\". Do you mean null?", paramName);
		}
	}
}
