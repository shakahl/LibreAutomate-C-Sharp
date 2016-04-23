using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
using System.Xml.Serialization;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public partial struct Wnd
	{
		//Base of WinProp and ChildProp.
		public class _WndProp
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
			/// The window must have this child window.
			/// The ChildDefinition object specifies Wnd.Child() arguments; more info there.
			/// Alternatively use childName, childClass and/or childId.
			/// </summary>
			public ChildDefinition child;
			/// <summary>
			/// The window must have a child window that has this name.
			/// This value will be passed to Wnd.Child(), together with childClass and childId.
			/// Wildcard, case-sensitive (read more in WildString class help).
			/// </summary>
			public WildString childName;
			/// <summary>
			/// The window must have a child window that has this class name.
			/// This value will be passed to Wnd.Child(), together with childName and childId.
			/// Wildcard, case-insensitive (read more in WildString class help).
			/// </summary>
			public WildStringI childClass;
			/// <summary>
			/// The window must have a child window that has this non-zero id.
			/// This value will be passed to Wnd.Child(), together with childName and childClass.
			/// </summary>
			public int childId;
			//info: C# does not allow this: new Wnd.WinProp() { child.Name="..", ... }
			//TODO: public ImageDefinition image;
			//TODO: public UIElemDefinition elem; or/and elemRole, elemName.
			//TODO: consider: maybe instead of WinProp/ChildProp add FindEx/ChildEx.

			ushort _propAtom;

			protected bool _Init(Wnd wParent)
			{
				_propAtom = 0;
				if(!Empty(propName)) {
					_propAtom = Api.GlobalFindAtom(propName);
					if(_propAtom == 0) return false;
				} else propName = null;

				if(childName != null || childClass != null || childId != 0) {
					child = new ChildDefinition(childName, childClass, childId);
					childName = null; childClass = null; childId = 0; //next time use the ChildDefinition object
				}

				return true;
			}

			internal bool MatchPropStyles(Wnd w)
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

				return true;
			}
		}

		#region find top-level

		/// <summary>
		/// Contains various window properties to pass to Wnd.Find 'prop' parameter.
		/// </summary>
		public class WinProp :_WndProp
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
				if(!_Init(Wnd0)) return false;
				return true;
			}
		}

		/// <summary>
		/// Wnd.Find flags. Documented there.
		/// </summary>
		[Flags]
		public enum WinFlag
		{
			HiddenToo = 1, SkipCloaked = 2, ProgramPath = 4,
		}

		/// <summary>
		/// Specifies Wnd.Find arguments. Documented there.
		/// </summary>
		public class WindowDefinition
		{
			readonly WildString _name; //default wildcard, match case
			readonly WildStringI _className; //default wildcard, ignore case
			readonly WildStringI _program; //default wildcard, ignore case
			readonly WinProp _prop;
			readonly Action<CallbackArgs> _f;
			readonly int _matchIndex;
			readonly WinFlag _flags;

			public WindowDefinition(
				WinFlag flags,
				WildString name, WildStringI className = null, WildStringI program = null,
				WinProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
				)
			{
				_name = name;
				_className = className;
				_program = program;
				_prop = prop;
				_f = f;
				_matchIndex = matchIndex;
				_flags = flags;
			}

			public WindowDefinition(
				WildString name, WildStringI className = null, WildStringI program = null, bool hiddenToo = false,
				WinProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
				) : this(hiddenToo ? WinFlag.HiddenToo : 0, name, className, program, prop, f, matchIndex)
			{
			}

			/// <summary>
			/// The found window.
			/// </summary>
			public Wnd Result { get; private set; }

			/// <summary>
			/// Finds window, like Wnd.Find.
			/// Returns true if found.
			/// The Result property will be the window.
			/// </summary>
			public bool Find()
			{
				return FindInList(All.Windows(null, !_flags.HasFlag(WinFlag.HiddenToo), true)) >= 0;
			}

			/// <summary>
			/// Finds matching window in list of windows (handles).
			/// Returns 0-based index, or -1 if not found.
			/// The Result property will be the window.
			/// Does not skip hidden windows, even if flag HiddenToo is not set.
			/// </summary>
			/// <param name="a">List of windows (handles), for example returned by Wnd.All.Windows.</param>
			public int FindInList(List<Wnd> a)
			{
				Result = Wnd0;
				if(a == null) return -1;

				var p = _prop;
				if(p == null) p = new WinProp(); //will not need if(p != null && ...)
				else if(!p.Init()) return -1;

				List<uint> pids = null; bool programNamePlanB = false; //variables for faster getting/matching program name
				CallbackArgs cbArgs = null;
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
						//The worst case is when only program is specified, and the very worst case is when hiddenToo is also true.
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
								if(pids == null) break;
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

					if(!p.MatchPropStyles(w)) continue;

					if(_flags.HasFlag(WinFlag.SkipCloaked)) {
						if(w.Cloaked) continue;
					}

					if(p.child != null) {
						if(!p.child.Find(w)) continue;
					}

					if(_f != null) {
						if(cbArgs == null) cbArgs = new CallbackArgs();
						cbArgs.w = w;
						_f(cbArgs);
						if(!cbArgs.stop) continue;
					}

					if(--matchInd > 0) continue;

					Result = cbArgs != null ? cbArgs.w : w;
					return index;
				}

				return -1;
			}
		}

		/// <summary>
		/// Finds window.
		/// Returns its handle as Wnd. Returns Wnd0 if not found. No exceptions.
		/// Example: <c>Wnd w=Wnd.Find("Name"); if(w.Is0) Out("not found");</c>
		/// </summary>
		/// <param name="name">Window name. Usually it is the title bar text. Supports wildcard etc, case-sensitive (more info in WildString help).</param>
		/// <param name="className">Window class name. Supports wildcard etc, case-insensitive (more info in WildStringI and WildString help).</param>
		/// <param name="program">
		/// Program name without ".exe". Other overload has a flag to use full path of program file.
		/// Supports wildcard etc, case-insensitive (more info in WildStringI and WildString help).
		/// Can fail to get program name and especially path when the process has higher UAC integrity level; then assumes that the program name/path does not match.
		/// Getting program name or path is much slower than getting window name or class name, therefore this parameter should not be used alone.
		/// </param>
		/// <param name="hiddenToo">Can find hidden windows. Other overload also has a flag to skip cloaked windows; this overload does not skip.</param>
		/// <param name="prop">More properties of the window. Example: <c>new Wnd.WinProp(){ childClass="Static", childText="Text*" }</c></param>
		/// <param name="f">Lambda etc callback function to call for each matching window. It can evaluate more properties of the window and call Stop() when they match. Example: <c>e =˃ { Out(e.w); if(e.w.Name=="Find") e.Stop(); }</c></param>
		/// <param name="matchIndex">1-based index of matching window. For example, if matchIndex is 2, the function skips the first matching window and returns the second.</param>
		/// <remarks>
		/// Uses all arguments that are not omitted/0/null/"".
		/// </remarks>
		public static Wnd Find(
			WildString name, WildStringI className = null, WildStringI program = null, bool hiddenToo = false,
			WinProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
			)
		{
			var d = new WindowDefinition(name, className, program, hiddenToo, prop, f, matchIndex);
			d.Find();
			return d.Result;
		}

		/// <summary>
		/// Finds window.
		/// You can use this overload to specify more options (flags).
		/// Other parameters and everything else are the same as with other overload.
		/// </summary>
		/// <param name="flags">
		/// HiddenToo - can find hidden windows (note: other overload has a bool parameter for this).
		/// SkipCloaked - skip cloaked windows. Cloaked windows are windows hidden not in the classic way (Wnd.Visible does not detect it, Wnd.Cloaked detects). For example, windows on inactive Windows 10 virtual desktops, hidden Windows store apps on Windows 8.
		/// ProgramPath - the 'program' argument is full path. Need this flag because the function cannot auto-detect it when using wildcard or regex.
		/// </param>
		public static Wnd Find(
			WinFlag flags,
			WildString name, WildStringI className = null, WildStringI program = null,
			WinProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
			)
		{
			var d = new WindowDefinition(flags, name, className, program, prop, f, matchIndex);
			d.Find();
			return d.Result;
		}

		public static Wnd FindAny(params WindowDefinition[] a)
		{
			return Wnd0;
		}

		public static Wnd[] FindAll(params WindowDefinition[] a)
		{
			return null;
		}

		public static Wnd[] FindAll(string name, string className = null) //...
		{
			return null;
		}

		/// <summary>
		/// Finds window by class name.
		/// Uses Api.FindWindow(). Much faster than Find(), which uses Api.EnumWindows().
		/// Does not skip hidden and cloaked windows.
		/// </summary>
		/// <param name="className">Class name. Full, case-insensitive. Wildcard etc not supported.</param>
		public static Wnd FindByClassName(string className)
		{
			return Api.FindWindow(className, null);
		}

		#endregion

		#region find control

		/// <summary>
		/// Contains various control properties to pass to Wnd.Child 'prop' parameter.
		/// </summary>
		public class ChildProp :_WndProp
		{
			/// <summary>
			/// The control must contain this x coordinate in parent window's client area.
			/// Can be int (pixels) or double (fraction of client area, eg 0.5 is middle).
			/// </summary>
			public Coord x;
			/// <summary>
			/// The control must contain this y coordinate in parent window's client area.
			/// Can be int (pixels) or double (fraction of client area, eg 0.5 is middle).
			/// </summary>
			public Coord y;
			/// <summary>
			/// The programming name of the Windows Forms control.
			/// Not used for other types of controls.
			/// </summary>
			public string wfName;
			/// <summary>
			/// TODO
			/// Wildcard, case-insensitive (read more in WildString class help).
			/// </summary>
			public WildString uiName;
			/// <summary>
			/// x is relative to the right of the client area, rigt-to-left.
			/// For example, if x is 1, the control must be at the very right.
			/// </summary>
			public bool xFromRight;
			/// <summary>
			/// y is relative to the bottom of the client area, bottom-to-top.
			/// For example, if y is 1, the control must be at the very bottom.
			/// </summary>
			public bool yFromBottom;

			Coord _x, _y; //x y copies
			WindowsFormsControlNames _wfControls;

			//public static implicit operator ChildProp(string s)
			//{
			//	return null;
			//}

			/// <summary>
			/// Call this before starting to search for the window.
			/// Returns false if the window cannot be found, eg prop atom not found.
			/// Can throw; should not handle.
			/// </summary>
			public bool Init(Wnd wParent)
			{
				if(!_Init(wParent)) return false;

				if(x != null) _x = new Coord(x); if(y != null) _y = new Coord(y); //copy, because we don't want to modify (normalize) x y
				Coord.NormalizeInWindowClientArea(_x, _y, wParent, xFromRight, yFromBottom);

				if(!Empty(wfName)) _wfControls = new WindowsFormsControlNames(wParent); //throws

				return true;
			}

			internal bool MatchControlProp(Wnd wParent, Wnd c)
			{
				if(_x != null || _y!=null) {
					if(!Coord.IsInRect(_x, _y, c.GetRectInClientOf(wParent))) return false;
				}

				if(!MatchPropStyles(c)) return false;

				if(_wfControls != null) {
					if(!wfName.Equals_(_wfControls.GetControlName(c))) return false;
				}

				if(uiName != null) {
					//TODO
				}

				return true;
			}
		}

		/// <summary>
		/// Wnd.Child flags. Documented there.
		/// </summary>
		[Flags]
		public enum ChildFlag
		{
			HiddenToo = 1, DirectChild = 2, ControlText = 4,
		}

		/// <summary>
		/// Specifies Wnd.Child arguments. Documented there.
		/// </summary>
		public class ChildDefinition
		{
			readonly WildString _name; //default wildcard, match case
			readonly WildStringI _className; //default wildcard, ignore case
			readonly ChildProp _prop;
			readonly Action<CallbackArgs> _f;
			readonly int _matchIndex;
			readonly int _id;
			readonly ChildFlag _flags;

			public ChildDefinition(
				ChildFlag flags,
				WildString name, WildStringI className = null, int id = 0,
				ChildProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
				)
			{
				_name = name;
				_className = className;
				_prop = prop;
				_f = f;
				_matchIndex = matchIndex;
				_id = id;
				_flags = flags;
			}

			public ChildDefinition(
				WildString name, WildStringI className = null, int id = 0, bool hiddenToo = false,
				ChildProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
				) : this(hiddenToo ? ChildFlag.HiddenToo : 0, name, className, id, prop, f, matchIndex)
			{
			}

			/// <summary>
			/// The found control.
			/// </summary>
			public Wnd Result { get; private set; }

			/// <summary>
			/// Finds control, like Wnd.Child.
			/// Returns true if found.
			/// The Result property will be the control.
			/// </summary>
			public bool Find(Wnd w)
			{
				var a = All.Controls(w, null, _flags.HasFlag(ChildFlag.DirectChild), !_flags.HasFlag(ChildFlag.HiddenToo), true);
				return _FindInList(w, a, false) >= 0;
			}

			/// <summary>
			/// Finds matching control in list of controls (handles).
			/// Returns 0-based index, or -1 if not found.
			/// The Result property will be the control.
			/// Does not skip hidden controls, even if flag HiddenToo is not set.
			/// </summary>
			/// <param name="w">Parent window.</param>
			/// <param name="a">List of controls (handles), for example returned by Wnd.All.Controls.</param>
			public int FindInList(Wnd w, List<Wnd> a)
			{
				return _FindInList(w, a, true);
			}

			int _FindInList(Wnd wParent, List<Wnd> a, bool publicCall)
			{
				Result = Wnd0;
				if(a == null) return -1;

				var p = _prop;
				if(p == null) p = new ChildProp(); //will not need if(p != null && ...)
				else if(!p.Init(wParent)) return -1;

				CallbackArgs cbArgs = null;
				int matchInd = _matchIndex;

				bool mustBeDirectChild = publicCall && _flags.HasFlag(ChildFlag.DirectChild);

				//List<ushort> badAtoms = null;

				//bool useName = false, useText = false;
				//if(_name != null) {
				//	if(_flags.HasFlag(ChildFlag.ControlText)) useText = true; else useName = true;
				//}

				int index = -1;
				foreach(Wnd c in a) {
					index++;

					if(mustBeDirectChild) {
						if(c.DirectParentOrOwner != wParent) continue;
					}

					if(_id != 0) {
						if(c.ControlId != _id) continue;
					}

					//if(useName) {
					//	//if(!_name.Match(c.Name)) continue; //note: if using only Name, call it after ClassName
					//	if(!_name.Match(c.GetControlName())) continue;
					//	//speed:
					//	//	Tested with QM2 window, finding control Find -> Folder (class "*Edit"), using only name (no class etc). Maybe 10-15-th control.
					//	//	With GetControlName 4-5 times faster than with Name.
					//	//	With GetControlText 12 times slower than with GetControlName.
					//	//	When also using class, with GetControlText 2 times slower than with GetControlName.
					//}

					if(_className != null) {
						if(!_className.Match(c.ClassName)) continue;
					}

					//This does not make faster. Even if only class used (no name). Why? Getting atom does not slow down much. If using id, 2 times faster.
					//if(_className != null) {
					//	ushort atom = c.GetClassLong(Api.GCW_ATOM); if(atom == 0) continue;
					//	if(badAtoms != null && badAtoms.Contains(atom)) continue;
					//	string s = c.ClassName; if(s == null) continue;
					//	if(!_className.Match(s)) {
					//		if(badAtoms == null) badAtoms = new List<ushort>();
					//		badAtoms.Add(atom);
					//		continue;
					//	}
					//}

					if(_name != null) {
						string s;
						if(_flags.HasFlag(ChildFlag.ControlText)) s = c.GetControlText(); //only text
						else if(_className == null && _id == 0) s = c.GetControlName(); //only internal name; getting editable text would be slow and unreliable
						else s = c.Name; //internal name, then text if name empty; it is quite fast and reliable when class or id specified

						if(!_name.Match(s)) {
							if(!Util.Misc.StringRemoveMnemonicUnderlineAmpersand(ref s)) continue;
                            if(!_name.Match(s)) continue;
						}
					}

					//if(useText) {
					//	if(!_name.Match(c.GetControlText())) continue;
					//}

					if(!p.MatchControlProp(wParent, c)) continue;

					if(p.child != null) {
						if(!p.child.Find(c)) continue;
					}

					if(_f != null) {
						if(cbArgs == null) cbArgs = new CallbackArgs();
						cbArgs.w = c;
						_f(cbArgs);
						if(!cbArgs.stop) continue;
					}

					if(--matchInd > 0) continue;

					Result = cbArgs != null ? cbArgs.w : c;
					return index;
				}

				return -1;
			}
		}

		/// <summary>
		/// Finds a child window (control) of this window.
		/// </summary>
		/// <param name="name">
		/// Control name or text.
		/// Supports wildcard etc, case-sensitive (more info in WildString help).
		/// This function in most cases ignores editable or slow-to-get text unless one of these is used: className, id, ChildFlag.ControlText (with other overload).
		/// Control text often contains an invisible '&' character to underline the next character when using the keyboard to select dialog controls. You can use control name with or without '&', this function supports both. 
		/// </param>
		/// <param name="className">Control class name. Supports wildcard etc, case-insensitive (more info in WildStringI and WildString help).</param>
		/// <param name="id">Control id.</param>
		/// <param name="hiddenToo">Can find hidden controls.</param>
		/// <param name="prop">More properties of the control. Example: <c>new Wnd.ChildProp(){ x=344, y=448}</c></param>
		/// <param name="f">Lambda etc callback function to call for each matching control. It can evaluate more properties of the control and call Stop() when they match. Example: <c>e =˃ { Out(e.w); if(e.w.Name=="Find") e.Stop(); }</c></param>
		/// <param name="matchIndex">1-based index of matching control. For example, if matchIndex is 2, the function skips the first matching control and returns the second.</param>
		/// <exception cref="CatkeysException">
		/// 1. When this window is invalid.
		/// 2. When prop.wfName used and cannot get form control names from this window, usually because of UAC.
		/// </exception>
		/// <remarks>
		/// Uses all arguments that are not omitted/0/null/"".
		/// By default also searches indirect children (children of children and so on).
		/// </remarks>
		public Wnd Child(
			WildString name, WildStringI className = null, int id = 0, bool hiddenToo = false,
			ChildProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
			)
		{
			Validate();
			var d = new ChildDefinition(name, className, id, hiddenToo, prop, f, matchIndex);
			d.Find(this);
			return d.Result;
		}

		/// <summary>
		/// Finds a child window (control) of this window.
		/// You can use this overload to specify more options (flags).
		/// Other parameters and everything else are the same as with other overload.
		/// </summary>
		/// <param name="flags">
		/// HiddenToo - can find hidden controls (note: other overload has a bool parameter for this).
		/// DirectChild - skip indirect child controls (children of children and so on).
		/// ControlText - always use only an alternative (and much slower) way to get text.
		/// </param>
		public Wnd Child(
			ChildFlag flags,
			WildString name, WildStringI className = null, int id = 0,
			ChildProp prop = null, Action<CallbackArgs> f = null, int matchIndex = 1
			)
		{
			Validate();
			var d = new ChildDefinition(flags, name, className, id, prop, f, matchIndex);
			d.Find(this);
			return d.Result;
		}

		/// <summary>
		/// Finds a child window (control) of this window by its id.
		/// Finds hidden controls too.
		/// </summary>
		/// <param name="id">Control id.</param>
		/// <param name="directChild">Must be direct child, not a child of a child and so on.</param>
		/// <remarks>
		/// You can also use other overload to find controls by id, but this one is faster and better when the control can be identified only by id. It works differently.
		/// At first calls Api.GetDlgItem. It is fast and searches only direct children. If it does not find, and !directChild, calls Api.EnumChildWindows like other oveload.
		/// </remarks>
		public Wnd Child(int id, bool directChild = false)
		{
			Wnd R = Api.GetDlgItem(this, id);
			if(R.Is0) {
				Validate();
				if(directChild == false) {
					//return Child(null, null, id, true);
					All.Controls(e =>
					{
						if(e.w.ControlId != id) return;
						R = e.w; e.Stop();
					}, this);
				}
			}
			return R;
		}

		/// <summary>
		/// Finds a child window (control) of this window by its class name.
		/// Finds hidden controls too.
		/// </summary>
		/// <param name="className">Class name. String by default is interpreted as wildcard, case-insensitive..</param>
		/// <param name="directChild">Must be direct child, not a grandchild.</param>
		/// <param name="matchIndex">1-based match index. For example, if 2, will get the second matching control.</param>
		public Wnd ChildByClassName(WildStringI className, bool directChild = false, int matchIndex = 1)
		{
			Debug.Assert(className != null);
			if(className == null) return Wnd0;
			Wnd R = Wnd0;
			All.Controls(e =>
			{
				if(--matchIndex > 0) return;
				R = e.w; e.Stop();
			}, this, className, directChild);
			return R;
		}

		#endregion

		#region from XY

		/// <summary>
		/// Gets top-level window or control from point.
		/// </summary>
		/// <param name="x">X coordinate. Can be int (pixels) or double (fraction of primary screen).</param>
		/// <param name="y">Y coordinate. Can be int (pixels) or double (fraction of primary screen).</param>
		/// <param name="control">
		/// If true, gets control; returns Wnd0 if there is no control at that point.
		/// If false, gets top-level window; if at that point is a control, gets its top-level parent.
		/// If omitted or null, gets exactly what is at that point (control or top-level window).
		/// </param>
		/// <param name="xFromRight">x is relative to the right edge of the primary screen, right-to-left.</param>
		/// <param name="yFromBottom">y is relative to the bottom edge of the primary screen, bottom-to-top.</param>
		public static Wnd FromXY(Coord x, Coord y, bool? control=null, bool xFromRight = false, bool yFromBottom = false)
		{
			Coord.NormalizeInScreen(x, y, xFromRight, yFromBottom);

			Wnd w = Api.WindowFromPoint(new POINT(x.coord, y.coord));
			if(control != null) {
				if(control.Value) {
					if(!w.IsControl) w = Wnd0;
				} else w = w.ToplevelParentOrThis;
			}
			return w;
		}

		/// <summary>
		/// Gets top-level window or control from mouse cursor position.
		/// </summary>
		/// <param name="control">
		/// If true, gets control; returns Wnd0 if there is no control at that point.
		/// If false, gets top-level window; if at that point is a control, gets its top-level parent.
		/// If omitted or null, gets exactly what is at that point (control or top-level window).
		/// </param>
		public static Wnd FromMouse(bool? control = null)
		{
			POINT p; Api.GetCursorPos(out p);
			return FromXY(p.x, p.y, control);
		}

		#endregion
	}

	/// <summary>
	/// Gets programming names of Windows Forms controls.
	/// Usually each control has a unique name. It is useful to identify controls without a classic name/text.
	/// Control id of these controls is not constant and cannot be used.
	/// </summary>
	public class WindowsFormsControlNames :IDisposable
	{
		ProcessMemory _pm;
		Wnd _w;

		#region IDisposable Support

		void _Dispose()
		{
			if(_pm != null) { _pm.Dispose(); _pm = null; }
		}

		~WindowsFormsControlNames() { _Dispose(); }

		public void Dispose()
		{
			_Dispose();
			GC.SuppressFinalize(this);
		}

		#endregion

		static readonly uint WM_GETCONTROLNAME = Api.RegisterWindowMessage("WM_GETCONTROLNAME");

		/// <summary>
		/// Prepares to get control names.
		/// </summary>
		/// <param name="w">Any top-level or child window of that process.</param>
		/// <exception cref="CatkeysException">Throws when cannot allocate process memory (see ProcessMemory) needed to get control names, usually because of UAC.</exception>
		public WindowsFormsControlNames(Wnd w)
		{
			_pm = new ProcessMemory(w, 4096); //throws
			_w = w;
		}

		/// <summary>
		/// Gets control name.
		/// Returns null if fails or the name is empty.
		/// </summary>
		/// <param name="c">The control. Can be a top-level window too. Must be of the same process as the window specified in the constructor.</param>
		public string GetControlName(Wnd c)
		{
			if(_pm == null) return null;
			if(!IsDotNetWindow(c)) return null;
			LPARAM R;
			if(!c.SendTimeout(10000, out R, WM_GETCONTROLNAME, 4096, _pm.Mem) || R < 1) return null;
			return _pm.ReadUnicodeString(R);
		}

		/// <summary>
		/// Returns true if window class name starts with "WindowsForms".
		/// Usually it means that we can get Windows Forms control name of the specified window and its child controls.
		/// </summary>
		/// <param name="w">The window. Can be top-level or control.</param>
		public static bool IsDotNetWindow(Wnd w)
		{
			return w.ClassNameIs(_cn);
		}
		static WildStringI _cn = new WildStringI("WindowsForms*");

		//TODO: implement
		public static string GetDotNetName(Wnd w, Wnd c, string cls)
		{
			return null;
		}

	}
}
