//#define ENUMWINDOWS_LESS_GARBAGE

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
	/// 'flags' parameter of <see cref="Wnd.Find"/>.
	/// </summary>
	/// <tocexclude />
	[Flags]
	public enum WFFlags
	{
		/// <summary>Can find hidden windows. Use this carefully, always use className, not just name, because there are many hidden tooltip windows etc that could match the name.</summary>
		HiddenToo = 1,
		/// <summary>Skip cloaked windows. These are windows hidden not in the classic way (Wnd.IsVisible does not detect it, Wnd.Cloaked detects). For example, windows on inactive Windows 10 virtual desktops, hidden Windows Store apps on Windows 8.</summary>
		SkipCloaked = 2,
		/// <summary>
		/// The 'programEtc' argument is thread id, not process id.
		/// Alternatively use <see cref="Wnd.Misc.FindThreadWindow"/>, it's faster.
		/// </summary>
		ThreadId = 4,

		//Don't need this. Not very useful, 3 times slower, and not always can get full path.
		///// <summary>The 'programEtc' argument is full path. Need this flag because the function cannot auto-detect it when using wildcard, regex etc.</summary>
		//ProgramPath = 8,
	}

	public partial struct Wnd
	{
		/// <summary>
		/// Contains top-level window properties and can be used to find the window.
		/// Can be used instead of <see cref="Wnd.Find"/> or <see cref="Wnd.FindAll"/>.
		/// These codes are equivalent:
		/// <code>Wnd w = Wnd.Find(a, b, c, d, e); if(!w.Is0) Print(w);</code>
		/// <code>var p = new Wnd.Finder(a, b, c, d, e); if(p.Find()) Print(p.Result);</code>
		/// Also can find in a custom list of windows.
		/// </summary>
		public class Finder
		{
			Wildex _name;
			Wildex _className;
			Wildex _program;
			Func<Wnd, bool> _also;
			WFFlags _flags;
			int _processId;
			int _threadId;
			Wnd _owner;

			Wnd[] _aIsMatch;

			/// <summary>
			/// See <see cref="Wnd.Find">Wnd.Find</see>.
			/// </summary>
			/// <exception cref="ArgumentException">
			/// className is "". To match any, use null.
			/// programEtc is "" or 0. To match any, use null.
			/// programEtc argument type is not string/Wnd/int/uint/null.
			/// Invalid wildcard expression ("**options|" or regular expression).
			/// </exception>
			public Finder(
				string name = null, string className = null, object programEtc = null,
				WFFlags flags = 0, Func<Wnd, bool> also = null)
			{
				_ThrowIfStringEmptyNotNull(className, nameof(className));

				if(programEtc != null) {
					int pidTid = 0; bool isPidTid = false;
					switch(programEtc) {
					case string program:
						_ThrowIfStringEmptyNotNull(program, nameof(programEtc));
						_program = program;
						break;
					case int i:
						pidTid = i;
						isPidTid = true;
						break;
					case uint i:
						pidTid = (int)i;
						isPidTid = true;
						break;
					case Wnd owner:
						_owner = owner;
						break;
					default: throw new ArgumentException("Bad type.", nameof(programEtc));
					}
					if(isPidTid) {
						if(pidTid == 0) throw new ArgumentException("Cannot be 0. Can be null.", nameof(programEtc));
						if(0 != (flags & WFFlags.ThreadId)) _threadId = pidTid; else _processId = pidTid;
					}
				}

				_name = name;
				_className = className;
				_flags = flags;
				_also = also;
			}

			/// <summary>
			/// The found window (after calling <see cref="Find"/> or <see cref="FindInList"/>).
			/// </summary>
			public Wnd Result { get; private set; }

			/// <summary>
			/// Finds the specified window, like <see cref="Wnd.Find">Wnd.Find</see>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
#if ENUMWINDOWS_LESS_GARBAGE
			public bool Find()
			{
				using(var a = Misc.LibEnumWindows2(Misc.LibEnumWindowsAPI.EnumWindows, 0 == (_flags & WFFlags.HiddenToo), true)) {
					//Print(a.ToArray());
					return _FindInList(a, false) >= 0;
				}
			}
			//public bool Find()
			//{
			//	return _FindInList(Misc.LibEnumWindows3(Misc.LibEnumWindowsAPI.EnumWindows, 0 == (_flags & WFFlags.HiddenToo), true), false) >= 0;
			//}
#else
			public bool Find()
			{
				var a = Misc.AllWindows(0 == (_flags & WFFlags.HiddenToo), true);
				//Print(a);
				return _FindInList(a, false) >= 0;
			}
#endif

			/// <summary>
			/// Finds the specified window in a list of windows.
			/// Returns 0-based index, or -1 if not found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
			/// <param name="a">Array or list of windows, for example returned by <see cref="Misc.AllWindows"/>.</param>
			public int FindInList(IEnumerable<Wnd> a)
			{
				return _FindInList(a, true);
			}

			/// <summary>
			/// Finds all matching windows, like <see cref="Wnd.FindAll">Wnd.FindAll</see>.
			/// Returns array containing 0 or more window handles as Wnd.
			/// </summary>
			public Wnd[] FindAll()
			{
				var a = Misc.AllWindows(0 == (_flags & WFFlags.HiddenToo), true);
				using(var ab = new Util.LibArrayBuilder<Wnd>()) {
					_FindInList(a, false, ab);
					return ab.ToArray();
				}
			}

			/// <summary>
			/// Finds all matching windows in a list of windows.
			/// Returns array containing 0 or more window handles as Wnd.
			/// </summary>
			/// <param name="a">Array or list of windows, for example returned by <see cref="Misc.AllWindows"/>.</param>
			public Wnd[] FindAllInList(IEnumerable<Wnd> a)
			{
				using(var ab = new Util.LibArrayBuilder<Wnd>()) {
					_FindInList(a, true, ab);
					return ab.ToArray();
				}
			}

			/// <summary>
			/// If a is not null, returns index of matching element or -1.
			/// Else returns -2 if wSingle matches, else -1.
			/// Returns -1 if using aFindAll.
			/// </summary>
			/// <param name="a">Array etc of Wnd.</param>
			/// <param name="inList">Called by FindInList or FindAllInList.</param>
			/// <param name="aFindAll">If not null, adds all matching to it and returns -1.</param>
			/// <param name="wSingle">Can be used instead of a. Then a must be null.</param>
			int _FindInList(IEnumerable<Wnd> a, bool inList, Util.LibArrayBuilder<Wnd> aFindAll = null, Wnd wSingle = default(Wnd))
			{
				Result = default(Wnd);
				bool mustBeVisible = inList && (_flags & WFFlags.HiddenToo) == 0;
				List<int> pids = null; bool programNamePlanB = false; //variables for faster getting/matching program name

				//this thing is a foreach that supports either IEnumerable (a) or single Wnd (wSingle)
				using(var en = a?.GetEnumerator()) {
					for(int index = 0; index >= 0; index++) {
						Wnd w;
						if(en != null) {
							if(!en.MoveNext()) break;
							w = en.Current;
						} else {
							w = wSingle;
							index = -2; //if matches, returns -2, else breaks at -1
						}
						if(w.Is0) continue;

						//speed of 1000 times getting:
						//name 400, class 400, foreign pid/tid 400,
						//owner 55, rect 55, style 50, exstyle 50, cloaked 280,
						//GetProp(string) 1700, GetProp(atom) 300, GlobalFindAtom 650,
						//program >=2500

						if(mustBeVisible) {
							if(!w.IsVisible) continue;
						}

						if(!_owner.Is0) {
							if(_owner != w.WndOwner) continue;
						}

						if(_name != null) {
							if(!_name.Match(w.GetText(false, false))) continue;
						}

						if(_className != null) {
							if(!_className.Match(w.ClassName)) continue;
						}
						//CONSIDER: to make less garbage when waiting:
						//	Use a list of windows that will not match. As a field of this class.
						//	Turned on through an internal get/set property of this class. The wait functions would set it.
						//	If class or pid etc didn't match, next time we can continue without getting it.
						//	Maybe even pass it to Misc.AllWindows, let it add these windows to the array.
						//	Then getting class etc should be before getting name. Not good.
						//	For name cannot do such optimization because name can change.
						//		But still can avoid creating garbage:
						//			Add non-matching window names to a dictionary.
						//			Then the get-name method can be optimized to return null (don't create new string) if current name is the same as in the dictionary.

						int pid = 0, tid = 0;
						if(_program != null || _processId != 0 || _threadId != 0) {
							tid = w.GetThreadProcessId(out pid);
							if(tid == 0) continue;
							//speed: with foreign processes the same speed as getting name or class name. Much faster if same process.
						}

						if(_threadId != 0) {
							if(_threadId != tid) continue;
						}

						if(_processId != 0) {
							if(_processId != pid) continue;
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

								//string pname = Process_.GetProcessName(pid, 0!=(_flags&WFFlags.ProgramPath), true);
								string pname = Process_.GetProcessName(pid, false, true);

								if(pname == null) {
									//if(0!=(_flags&WFFlags.ProgramPath)) continue;

									//switch to plan B
									pids = Process_.GetProcessesByName(_program);
									if(pids.Count == 0) break;
									programNamePlanB = true;
									goto g1;
								}

								if(!_program.Match(pname)) {
									if(a == null) break;
									if(pids == null) pids = new List<int>();
									pids.Add(pid); //add bad pid
									continue;
								}
							}
						}

						if(0 != (_flags & WFFlags.SkipCloaked)) {
							if(w.IsCloaked) continue;
						}

						if(_also != null && !_also(w)) continue;

						if(aFindAll != null) {
							aFindAll.Add(w);
							continue;
						}

						Result = w;
						return index;
					}
				}

				return -1;
			}

			/// <summary>
			/// Returns true if window w properties match the specified properties.
			/// </summary>
			/// <param name="w">A top-level window. Can be 0/invalid, then returns false.</param>
			public bool IsMatch(Wnd w)
			{
				return -2 == _FindInList(null, true, null, w);
			}

			/// <summary>
			/// Sets process id.
			/// If programEtc was used, clears it. If value is 0, does not clear program name.
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// Wnd w = Wnd.Find("*- Notepad", "Notepad");
			/// if(w.Is0) { Wnd.LastFind.ProcessId = Shell.Run("notepad.exe"); w = WaitFor.WindowActive(Wnd.LastFind); }
			/// ]]></code>
			/// </example>
			public int ProcessId
			{
				set
				{
					_processId = value;
					_threadId = 0;
					_owner = default(Wnd);
					if(value != 0) _program = null;
				}
			}
		}

		/// <summary>
		/// Finds window.
		/// Returns its handle as Wnd. Returns default(Wnd) if not found.
		/// </summary>
		/// <param name="name">
		/// Window name. Usually it is the title bar text.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'any', "" means 'no name'.
		/// </param>
		/// <param name="className">Window class name.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'any'.
		/// </param>
		/// <param name="programEtc">
		/// Depends on type and flags:
		/// <list type="bullet">
		/// <item>null - any (this parameter is not used).</item>
		/// <item>
		/// string - program file name without ".exe".
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// </item>
		/// <item>Wnd - owner window. See <see cref="WndOwner"/>.</item>
		/// <item>
		/// int, uint - process id. If using flag ThreadId - native thread id (not Thread.ManagedThreadId).
		/// See <see cref="ProcessId"/>, <see cref="ThreadId"/>, <see cref="GetThreadProcessId"/>, <see cref="Process_.CurrentProcessId"/>, <see cref="Process_.CurrentThreadId"/>, <msdn>GetProcessId</msdn> or <msdn>GetThreadId</msdn>.
		/// </item>
		/// </list>
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Lambda etc callback function to call for each matching window.
		/// It can evaluate more properties of the window and return true when they match.
		/// Example: <c>also: t =&gt; t.HasStyle(Native.WS_CAPTION)</c>.
		/// </param>
		/// <remarks>
		/// If there are multiple matching windows, gets the first in the Z order matching window, preferring visible windows.
		/// On Windows 8 and later finds only desktop windows, not Windows Store app Metro-style windows (on Windows 10 only few such windows exist), unless this process has uiAccess; to find such windows you can use <see cref="FindFast">FindFast</see>.
		/// To find message-only windows use <see cref="Misc.FindMessageWindow"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// className is "". To match any, use null.
		/// programEtc is "" or 0. To match any, use null.
		/// programEtc argument type is not string/Wnd/int/uint/null.
		/// Invalid wildcard expression ("**options|" or regular expression).
		/// </exception>
		/// <example>
		/// <code>
		/// Wnd w=Wnd.Find("Name");
		/// if(w.Is0) Print("not found");
		/// </code>
		/// </example>
		[MethodImpl(MethodImplOptions.NoInlining)] //inlined code makes harder to debug using disassembly
		public static Wnd Find(string name = null, string className = null, object programEtc = null, WFFlags flags = 0, Func<Wnd, bool> also = null)
		{
			var f = new Finder(name, className, programEtc, flags, also);
			f.Find();
			LastFind = f;
			return f.Result;
		}

		/// <summary>
		/// Gets arguments of this thread's last call to <see cref="Find"/> or <see cref="FindAll"/>, which set it before returning, regardless of results.
		/// </summary>
		/// <remarks>
		/// <b>WaitFor.WindowActive</b> and similar functions don't change this property. But this property is often used with them, like in the example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("*- Notepad", "Notepad");
		/// if(w.Is0) { Shell.Run("notepad.exe"); w = WaitFor.WindowActive(Wnd.LastFind); }
		/// ]]></code>
		/// </example>
		public static Finder LastFind { get => t_lastFindParams; set { t_lastFindParams = value; } }
		[ThreadStatic] static Finder t_lastFindParams;

		/// <summary>
		/// Finds all matching windows.
		/// Returns array containing 0 or more window handles as Wnd.
		/// Everything except the return type is the same as with <see cref="Find"/>.
		/// </summary>
		/// <exception cref="ArgumentException"/>
		/// <remarks>
		/// The list is sorted to match the Z order, however hidden windows (when using WFFlags.HiddenToo) are always after visible windows.
		/// </remarks>
		public static Wnd[] FindAll(string name = null, string className = null, object programEtc = null, WFFlags flags = 0, Func<Wnd, bool> also = null)
		{
			var f = new Finder(name, className, programEtc, flags, also);
			var a = f.FindAll();
			LastFind = f;
			return a;
		}

		/// <summary>
		/// Finds window.
		/// Returns its handle as Wnd. Returns default(Wnd) if not found.
		/// Calls API <msdn>FindWindowEx</msdn>.
		/// Faster than <see cref="Find">Find</see>, which uses API <msdn>EnumWindows</msdn>.
		/// Can be used only when you know full name and/or class name.
		/// Finds hidden windows too.
		/// </summary>
		/// <param name="name">
		/// Name. Can be null to match any.
		/// Full, case-insensitive. Wildcard etc not supported.
		/// </param>
		/// <param name="className">
		/// Class name. Can be null to match any. Cannot be "".
		/// Full, case-insensitive. Wildcard etc not supported.
		/// </param>
		/// <param name="wAfter">If used, starts searching from the next window in the Z order.</param>
		/// <remarks>
		/// To find message-only windows use <see cref="Misc.FindMessageWindow"/> instead.
		/// Supports <see cref="Native.GetError"/>.
		/// It is not recommended to use this function in a loop to enumerate windows. It would be unreliable because window positions in the Z order can be changed while enumerating. Also then it would be slower than <b>Find</b> and <b>FindAll</b>.
		/// </remarks>
		public static Wnd FindFast(string name, string className, Wnd wAfter = default(Wnd))
		{
			return Api.FindWindowEx(default(Wnd), wAfter, className, name);
		}

		//TODO: test more.
		/// <summary>
		/// Finds window. If found, activates (optionally), else calls callback function that should open the window (eg call <see cref="Shell.Run"/>) and waits for the window.
		/// Returns window handle as Wnd. Returns default(Wnd) if not found (if runWaitTimeoutS is negative; else exception).
		/// The first 5 parameters are the same as <see cref="Find"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="className"></param>
		/// <param name="programEtc"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="run">Callback function's delegate. See example.</param>
		/// <param name="runWaitS">How long to wait for the window after calling the callback function. Seconds. Default 60. See <see cref="WaitFor.WindowActive(double, string, string, object, WFFlags, Func{Wnd, bool}, bool)"/>.</param>
		/// <param name="needActiveWindow">Finally the window must be active. For more info see the algorithm in Remarks.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		/// <exception cref="TimeoutException">runWaitTimeoutS time has expired.</exception>
		/// <remarks>
		/// The algorithm is:
		/// <code>
		/// var w=Wnd.Find(...);
		/// if(!w.Is0) { if(needActiveWindow) w.Activate(); }
		/// else if(run!=null) { run(); if(needActiveWindow) w = WaitFor.WindowActive(..., runWaitTimeoutS); else WaitFor.WindowExists(..., runWaitTimeoutS); }
		/// return w;
		/// </code>
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.FindOrRun("* Notepad", run: () => Shell.Run("notepad.exe"));
		/// Print(w);
		/// ]]></code>
		/// </example>
		public static Wnd FindOrRun(string name = null, string className = null, object programEtc = null, WFFlags flags = 0, Func<Wnd, bool> also = null,
			Func<int> run = null, double runWaitS = 60.0, bool needActiveWindow = true)
		{
			var w = Find(name, className, programEtc, flags, also);
			if(!w.Is0) {
				if(needActiveWindow) w.Activate();
			} else if(run != null) {
				var finder = LastFind;
				var r = run();
				//TODO: set finder.programEtc = r (when we have Shell.RunResult). Allow null.
				if(needActiveWindow) w = WaitFor.WindowActive(finder, runWaitS);
				else w = WaitFor.WindowExists(finder, runWaitS);
			}
			return w;
		}

		public static partial class Misc
		{
			/// <summary>
			/// Gets top-level windows.
			/// Returns array containing 0 or more window handles as Wnd.
			/// </summary>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <param name="sortFirstVisible">Place all array elements of hidden windows at the end of the array, even if the hidden windows are before some visible windows in the Z order.</param>
			/// <param name="also">
			/// Lambda etc callback function to call for each matching window.
			/// It can evaluate more properties of the window and return true when they match.
			/// Example: <c>also: t =&gt; t.ClassNameIs("#32770")</c>.
			/// </param>
			/// <remarks>
			/// Calls API <msdn>EnumWindows</msdn>.
			/// <note>The list can be bigger than you expect. See also <see cref="MainWindows">MainWindows</see>.</note>
			/// By default array elements are sorted to match the Z order.
			/// On Windows 8 and later gets only desktop windows, not Windows Store app Metro-style windows (on Windows 10 only few such windows exist), unless this process has UAC integrity level uiAccess; to get such windows you can use <see cref="FindFast">FindFast</see>.
			/// </remarks>
			public static Wnd[] AllWindows(bool onlyVisible = false, bool sortFirstVisible = false, Func<Wnd, bool> also = null)
			{
				return LibEnumWindows(LibEnumWindowsAPI.EnumWindows, onlyVisible, sortFirstVisible, also);

				//info: tried to add a flag to skip tooltips, IME, MSCTFIME UI. But for it need to get class. It is slow. Other ways are unreliable and also make slower. Only the onlyVisible flag is really effective.
			}

			internal enum LibEnumWindowsAPI { EnumWindows, EnumThreadWindows, EnumChildWindows, }

			internal static Wnd[] LibEnumWindows(LibEnumWindowsAPI api,
				bool onlyVisible, bool sortFirstVisible, Func<Wnd, bool> also = null,
				Wnd wParent = default(Wnd), bool directChild = false, int threadId = 0)
			{
				if(onlyVisible) sortFirstVisible = false;
				var a = new Util.LibArrayBuilder<Wnd>(onlyVisible ? 250 : 1020); //tested: normally there are 200-400 windows on my PC, rarely exceeds 500
				try {
					Api.WNDENUMPROC proc = (w, param) =>
					  {
						  if(onlyVisible && !w.IsVisible) return 1;
						  if(directChild && Api.GetParent(w) != wParent) return 1;
						  if(also != null && !also(w)) return 1;
						  a.Add(w);
						  return 1;
					  };

					switch(api) {
					case LibEnumWindowsAPI.EnumWindows:
						Api.EnumWindows(proc, Zero);
						break;
					case LibEnumWindowsAPI.EnumThreadWindows:
						Api.EnumThreadWindows(threadId, proc, Zero);
						break;
					case LibEnumWindowsAPI.EnumChildWindows:
						Api.EnumChildWindows(wParent, proc, Zero);
						break;
					}

					if(!sortFirstVisible) return a.ToArray();
					int j = 0, n = a.Count;
					var r = new Wnd[n];
					for(int i = 0; i < n; i++) {
						var w = a[i]; if(!w.IsVisible) continue;
						r[j++] = w;
						a[i] = default(Wnd);
					}
					for(int i = 0; i < n; i++) {
						var w = a[i];
						if(!w.Is0) r[j++] = w;
					}
					return r;
				}
				finally { a.Dispose(); }
			}

#if ENUMWINDOWS_LESS_GARBAGE
			//This version creates 12 times less garbage (the garbage is the returned array). But very slightly slower when need to sort.
			//But it is not so easy to use (the caller must dispose it).
			//CONSIDER: Finder can have own LibArrayBuilder. Then even don't need to allocate/free memory many times when waiting.
			internal static Util.LibArrayBuilder<Wnd> LibEnumWindows2(LibEnumWindowsAPI api,
				bool onlyVisible, bool sortFirstVisible, Func<Wnd, bool> also = null,
				Wnd wParent = default(Wnd), bool directChild = false, int threadId = 0)
			{
				if(onlyVisible) sortFirstVisible = false;
				var a = new Util.LibArrayBuilder<Wnd>(onlyVisible ? 250 : 1020); //tested: normally there are 200-400 windows on my PC, rarely exceeds 500
				try {
					Api.WNDENUMPROC proc = (w, param) =>
					  {
						  if(onlyVisible && !w.IsVisible) return 1;
						  if(directChild && Api.GetParent(w) != wParent) return 1;
						  if(also != null && !also(w)) return 1;
						  a.Add(w);
						  return 1;
					  };

					switch(api) {
					case LibEnumWindowsAPI.EnumWindows:
						Api.EnumWindows(proc, Zero);
						break;
					case LibEnumWindowsAPI.EnumThreadWindows:
						Api.EnumThreadWindows(threadId, proc, Zero);
						break;
					case LibEnumWindowsAPI.EnumChildWindows:
						Api.EnumChildWindows(wParent, proc, Zero);
						break;
					}

					if(sortFirstVisible) {
						if(t_sort == null) t_sort = new WeakReference<List<Wnd>>(null);
						if(!t_sort.TryGetTarget(out var aVisible)) t_sort.SetTarget(aVisible = new List<Wnd>(250));
						else aVisible.Clear();
						int n = a.Count;
						for(int i = 0; i < n; i++) {
							var w = a[i]; if(!w.IsVisible) continue;
							aVisible.Add(w);
							a[i] = default(Wnd);
						}
						for(int i = n - 1, j = i; i >= 0; i--) {
							var w = a[i];
							if(!w.Is0) a[j--] = w;
						}
						for(int i = 0; i < aVisible.Count; i++) a[i] = aVisible[i];
					}

					var R = a; a = null; //don't dispose
					return R;
				}
				finally { a?.Dispose(); } //dispose if exception
			}
			[ThreadStatic] static WeakReference<List<Wnd>> t_sort;

			////This version is easier to use than if returning LibArrayBuilder (the caller must dispose it).
			////But slightly slower and creates ~50% more garbage.
			//internal static IEnumerable<Wnd> LibEnumWindows3(LibEnumWindowsAPI api,
			//	bool onlyVisible, bool sortFirstVisible, Func<Wnd, bool> also = null,
			//	Wnd wParent = default(Wnd), bool directChild = false, int threadId = 0)
			//{
			//	if(onlyVisible) sortFirstVisible = false;
			//	var a = new Util.LibArrayBuilder<Wnd>(onlyVisible ? 250 : 1020); //tested: normally there are 200-400 windows on my PC, rarely exceeds 500
			//	try {
			//		Api.WNDENUMPROC proc = (w, param) =>
			//		  {
			//			  if(onlyVisible && !w.IsVisible) return 1;
			//			  if(directChild && Api.GetParent(w) != wParent) return 1;
			//			  if(also != null && !also(w)) return 1;
			//			  a.Add(w);
			//			  return 1;
			//		  };

			//		switch(api) {
			//		case LibEnumWindowsAPI.EnumWindows:
			//			Api.EnumWindows(proc, Zero);
			//			break;
			//		case LibEnumWindowsAPI.EnumThreadWindows:
			//			Api.EnumThreadWindows(threadId, proc, Zero);
			//			break;
			//		case LibEnumWindowsAPI.EnumChildWindows:
			//			Api.EnumChildWindows(wParent, proc, Zero);
			//			break;
			//		}

			//		if(sortFirstVisible) {
			//			if(t_sort == null) t_sort = new WeakReference<List<Wnd>>(null);
			//			if(!t_sort.TryGetTarget(out var aVisible)) t_sort.SetTarget(aVisible = new List<Wnd>(250));
			//			else aVisible.Clear();
			//			int n = a.Count;
			//			for(int i = 0; i < n; i++) {
			//				var w = a[i]; if(!w.IsVisible) continue;
			//				aVisible.Add(w);
			//				a[i] = default(Wnd);
			//			}
			//			for(int i = n - 1, j = i; i >= 0; i--) {
			//				var w = a[i];
			//				if(!w.Is0) a[j--] = w;
			//			}
			//			for(int i = 0; i < aVisible.Count; i++) a[i] = aVisible[i];
			//		}

			//		for(int i = 0, n = a.Count; i < n; i++) {
			//			yield return a[i];
			//		}
			//	}
			//	finally { a.Dispose(); }
			//}
#endif

			/// <summary>
			/// Gets top-level windows of a thread.
			/// Returns array containing 0 or more window handles as Wnd.
			/// </summary>
			/// <param name="threadId">
			/// Unmanaged thread id.
			/// See <see cref="Process_.CurrentThreadId"/>, <see cref="ThreadId"/>.
			/// If 0, throws exception. If other invalid value (ended thread?), returns empty list. Supports <see cref="Native.GetError"/>.
			/// </param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <param name="sortFirstVisible">Place all array elements of hidden windows at the end of the array, even if the hidden windows are before some visible windows in the Z order.</param>
			/// <param name="also">
			/// Lambda etc callback function to call for each matching window.
			/// It can evaluate more properties of the window and return true when they match.
			/// Example: <c>also: t =&gt; t.ClassNameIs("#32770")</c>.
			/// </param>
			/// <exception cref="ArgumentException">0 threadId.</exception>
			/// <remarks>
			/// Calls API <msdn>EnumThreadWindows</msdn>.
			/// </remarks>
			public static Wnd[] ThreadWindows(int threadId, bool onlyVisible = false, bool sortFirstVisible = false, Func<Wnd, bool> also = null)
			{
				if(threadId == 0) throw new ArgumentException("0 threadId");
				return LibEnumWindows(LibEnumWindowsAPI.EnumThreadWindows, onlyVisible, sortFirstVisible, also, threadId: threadId);

				//speed: 2.5 times faster than EnumWindows. Tested with a foreign thread with 30 windows.
			}

			/// <summary>
			/// Finds window of the specified thread.
			/// Returns its handle as Wnd. Returns default(Wnd) if not found.
			/// Parameters are the same as of <see cref="Find"/>.
			/// </summary>
			/// <param name="threadId">
			/// Unmanaged thread id.
			/// See <see cref="Process_.CurrentThreadId"/>, <see cref="ThreadId"/>.
			/// If 0, throws exception. If other invalid value (ended thread?), returns default(Wnd).
			/// </param>
			/// <param name="name"></param>
			/// <param name="className"></param>
			/// <param name="flags"></param>
			/// <param name="also"></param>
			public static Wnd FindThreadWindow(int threadId,
				string name = null, string className = null, WFFlags flags = 0, Func<Wnd, bool> also = null)
			{
				var a = ThreadWindows(threadId, 0 == (flags & WFFlags.HiddenToo), true);
				var f = new Finder(name, className, null, flags | WFFlags.HiddenToo, also);
				return f.FindInList(a) >= 0 ? f.Result : default(Wnd);
			}

			/// <summary>
			/// Finds a message-only window.
			/// Returns its handle as Wnd. Returns default(Wnd) if not found.
			/// Calls API <msdn>FindWindowEx</msdn>.
			/// Faster than <see cref="Find">Find</see>, which does not find message-only windows.
			/// Can be used only when you know full name and/or class name.
			/// Finds hidden windows too.
			/// </summary>
			/// <param name="name">
			/// Name. Can be null to match any.
			/// Full, case-insensitive. Wildcard etc not supported.
			/// </param>
			/// <param name="className">
			/// Class name. Can be null to match any. Cannot be "".
			/// Full, case-insensitive. Wildcard etc not supported.
			/// </param>
			/// <param name="wAfter">If used, starts searching from the next window in the Z order.</param>
			/// <remarks>
			/// Supports <see cref="Native.GetError"/>.
			/// </remarks>
			public static Wnd FindMessageWindow(string name, string className, Wnd wAfter = default(Wnd))
			{
				return Api.FindWindowEx(SpecHwnd.HWND_MESSAGE, wAfter, className, name);
			}
		}
	}
}
