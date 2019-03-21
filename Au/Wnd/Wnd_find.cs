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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	public unsafe partial struct Wnd
	{
		/// <summary>
		/// Contains top-level window properties and can be used to find the window.
		/// </summary>
		/// <remarks>
		/// Can be used instead of <see cref="Wnd.Find"/> or <see cref="Wnd.FindAll"/>.
		/// These codes are equivalent:
		/// <code>Wnd w = Wnd.Find(a, b, c, d, e); if(!w.Is0) Print(w);</code>
		/// <code>var p = new Wnd.Finder(a, b, c, d, e); if(p.Find()) Print(p.Result);</code>
		/// Also can find in a list of windows.
		/// </remarks>
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
			object _contains;

			/// <summary>
			/// See <see cref="Wnd.Find"/>.
			/// </summary>
			/// <exception cref="ArgumentException">
			/// <paramref name="cn"/> is "". To match any, use null.
			/// <paramref name="program"/> is "" or 0 or contains \ or /. To match any, use null.
			/// Invalid wildcard expression ("**options " or regular expression).
			/// Invalid image string in <paramref name="contains"/>.
			/// </exception>
			public Finder(
				string name = null, string cn = null, WF3 program = default,
				WFFlags flags = 0, Func<Wnd, bool> also = null, object contains = null)
			{
				_name = name;
				if(cn != null) _className = cn.Length != 0 ? cn : throw new ArgumentException("Class name cannot be \"\". Use null to match any.");
				program.GetValue(out _program, out _processId, out _threadId, out _owner);
				_flags = flags;
				_also = also;
				if(contains != null) _ParseContains(contains);
			}

			//TODO: use uniform and more readable format:
			//	acc:ROLE, name
			//	child:class, name
			//	image:xxxxxxxxxxxxxxx
			void _ParseContains(object contains)
			{
				if(contains is string s) { //accessible object or control or image. Acc format: "'role' name" or "name". Control: "'^class' text". Image: "image:...".
					if(s.Length == 0) return;
					string role = null, name = s;
					switch(s[0]) {
					case '\'':
						if(s.RegexMatch_(@"^'(.+?)?' ((?s).+)?$", out var m)) { role = m[1].Value; name = m[2].Value; }
						break;
					case 'i' when s.StartsWith_("image:"):
						_contains = WinImage.LoadImage(s);
						return;
					}
					if(role != null && role[0] == '!') { //control
						_contains = new ChildFinder(name, role.Length == 1 ? null : role.Substring(1));
					} else { //acc
						_contains = new Acc.Finder(role, name, flags: AFFlags.ClientArea) { ResultGetProperty = '-' };
					}
				} else if(contains is Acc.Finder || contains is ChildFinder || contains is System.Drawing.Image) _contains = contains;
				else throw new ArgumentException("Unsupported object type.", nameof(contains));
			}

			/// <summary>
			/// Implicit conversion from string that can contain window name, class name, program and/or an object.
			/// Examples: "name,cn,program", "name", ",cn", ",,program", "name,cn", "name,,program", ",cn,program", "name,,,object".
			/// </summary>
			/// <param name="s">
			/// One or more comma-separated window properties: name, class, program and/or an object. Empty parts are considered null.
			/// The same as parameters of <see cref="Wnd.Find"/>. The first 3 parts are <i>name</i>, <i>cn</i> and <i>program</i>. The last part is <i>contains</i> as string; can specify an accessible object, control or image.
			/// The first 3 comma-separated parts cannot contain commas. Alternatively, parts can be separated by '\0' characters, like "name\0"+"cn\0"+"program\0"+"object". Then parts can contain commas. Example: "*one, two, three*\0" (name with commas).
			/// </param>
			/// <exception cref="Exception">Exceptions of the constructor.</exception>
			public static implicit operator Finder(string s)
			{
				string name = null, cn = null, prog = null, contains = null;
				char[] sep = null; if(s.IndexOf('\0') >= 0) sep = s_sepZero; else if(s.IndexOf(',') >= 0) sep = s_sepComma;
				if(sep == null) name = s;
				else {
					var ra = s.Split(sep, 4);
					if(ra[0].Length > 0) name = ra[0];
					if(ra[1].Length > 0) cn = ra[1];
					if(ra.Length > 2 && ra[2].Length > 0) prog = ra[2];
					if(ra.Length > 3 && ra[3].Length > 0) contains = ra[3];
				}
				return new Finder(name, cn, prog, contains: contains);
			}
			static readonly char[] s_sepComma = { ',' }, s_sepZero = { '\0' };

			/// <summary>
			/// The found window.
			/// </summary>
			public Wnd Result { get; internal set; }

			/// <summary>
			/// Finds the specified window, like <see cref="Wnd.Find"/>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
			public bool Find()
			{
				using(var k = new _WndList(_AllWindows()))
					return _FindInList(k) >= 0;
			}

			Util.LibArrayBuilder<Wnd> _AllWindows()
			{
				//FUTURE: optimization: if cn not wildcard etc, at first find atom.
				//	If not found, don't search. If found, compare atom, not class name string.

				var f = _threadId != 0 ? Lib.EnumAPI.EnumThreadWindows : Lib.EnumAPI.EnumWindows;
				return Lib.EnumWindows2(f, 0 == (_flags & WFFlags.HiddenToo), true, wParent: _owner, threadId: _threadId);
			}

			/// <summary>
			/// Finds the specified window in a list of windows.
			/// Returns 0-based index, or -1 if not found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
			/// <param name="a">Array or list of windows, for example returned by <see cref="GetWnd.AllWindows"/>.</param>
			public int FindInList(IEnumerable<Wnd> a)
			{
				using(var k = new _WndList(a))
					return _FindInList(k);
			}

			/// <summary>
			/// Finds all matching windows, like <see cref="Wnd.FindAll"/>.
			/// Returns array containing 0 or more window handles as Wnd.
			/// </summary>
			public Wnd[] FindAll()
			{
				return _FindAll(new _WndList(_AllWindows()));
			}

			/// <summary>
			/// Finds all matching windows in a list of windows.
			/// Returns array containing 0 or more window handles as Wnd.
			/// </summary>
			/// <param name="a">Array or list of windows, for example returned by <see cref="GetWnd.AllWindows"/>.</param>
			public Wnd[] FindAllInList(IEnumerable<Wnd> a)
			{
				return _FindAll(new _WndList(a));
			}

			Wnd[] _FindAll(_WndList k)
			{
				using(k) {
					using(var ab = new Util.LibArrayBuilder<Wnd>()) {
						_FindInList(k, w => ab.Add(w)); //CONSIDER: ab could be part of _WndList. Now the delegate creates garbage.
						return ab.ToArray();
					}
				}
			}

			/// <summary>
			/// Returns index of matching element or -1.
			/// Returns -1 if using getAll.
			/// </summary>
			/// <param name="a">List of Wnd. Does not dispose it.</param>
			/// <param name="getAll">If not null, calls it for all matching and returns -1.</param>
			/// <param name="cache"></param>
			int _FindInList(_WndList a, Action<Wnd> getAll = null, WFCache cache = null)
			{
				Result = default;
				if(a.Type == _WndList.ListType.None) return -1;
				bool inList = a.Type != _WndList.ListType.ArrayBuilder;
				bool ignoreVisibility = cache?.IgnoreVisibility ?? false;
				bool mustBeVisible = inList && (_flags & WFFlags.HiddenToo) == 0 && !ignoreVisibility;
				bool isOwner = inList && !_owner.Is0;
				bool isTid = inList ? _threadId != 0 : false;
				List<int> pids = null; bool programNamePlanB = false; //variables for faster getting/matching program name

				for(int index = 0; a.Next(out Wnd w); index++) {
					if(w.Is0) continue;

					//With warm CPU, speed of 1000 times getting:
					//name 400, class 400, foreign pid/tid 400,
					//owner 55, rect 55, style 50, exstyle 50, cloaked 280,
					//GetProp(string) 1700, GetProp(atom) 300, GlobalFindAtom 650,
					//program >=2500

					if(mustBeVisible) {
						if(!w.IsVisible) continue;
					}

					if(isOwner) {
						if(_owner != w.Owner) continue;
					}

					cache?.Begin(w);

					if(_name != null) {
						var s = cache != null && cache.CacheName ? (cache.Name ?? (cache.Name = w.LibNameTL)) : w.LibNameTL;
						if(!_name.Match(s)) continue;
						//note: name is before classname. It makes faster in slowest cases (HiddenToo), because most windows are nameless.
					}

					if(_className != null) {
						var s = cache != null ? (cache.Class ?? (cache.Class = w.ClassName)) : w.ClassName;
						if(!_className.Match(s)) continue;
					}

					if(0 == (_flags & WFFlags.CloakedToo) && !ignoreVisibility) {
						if(w.IsCloaked) continue;
					}

					int pid = 0, tid = 0;
					if(_program != null || _processId != 0 || isTid) {
						if(cache != null) {
							if(cache.Tid == 0) cache.Tid = w.GetThreadProcessId(out cache.Pid);
							tid = cache.Tid; pid = cache.Pid;
						} else tid = w.GetThreadProcessId(out pid);
						if(tid == 0) continue;
						//speed: with foreign processes the same speed as getting name or class name. Much faster if same process.
					}

					if(isTid) {
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
						//Normally we call Process_.GetName. In most cases it is quite fast.
						//Anyway, we use this optimization:
						//	Add pid of processes that don't match the specified name in the pids list (bad pids).
						//	Next time, if pid is in the bad pids list, just continue, don't need to get program name again.
						//However in the worst case we would encounter some processes that Process_.GetName cannot get name using the fast API.
						//For each such process it would then use the much slower 'get all processes' API, which is almost as slow as Process.GetProcessById(pid).ProgramName.
						//To solve this:
						//We tell Process_.GetName to not use the slow API, but just return null when the fast API fails.
						//When it happens (Process_.GetName returns null):
						//	If need full path: continue, we cannot do anything more.
						//	Switch to plan B and no longer use all the above. Plan B:
						//	Get list of pids of all processes that match _program. For it we call Process_.LibGetProcessesByName, which uses the same slow API, but we call it just one time.
						//	If it returns null (it means there are no matching processes), break (window not found).
						//	From now, in each loop will need just to find pid in the returned list, and continue if not found.

						g1:
						if(programNamePlanB) {
							if(!pids.Contains(pid)) continue;
						} else {
							if(pids != null && pids.Contains(pid)) continue; //is known bad pid?

							string pname = cache != null ? (cache.Program ?? (cache.Program = _Program())) : _Program();
							string _Program() => Process_.GetName(pid, false, true);
							//string _Program() => Process_.GetName(pid, 0!=(_flags&WFFlags.ProgramPath), true);

							if(pname == null) {
								//if(0!=(_flags&WFFlags.ProgramPath)) continue;

								//switch to plan B
								Process_.LibGetProcessesByName(ref pids, _program);
								if(pids == null || pids.Count == 0) break;
								programNamePlanB = true;
								goto g1;
							}

							if(!_program.Match(pname)) {
								if(a.Type == _WndList.ListType.SingleWnd) break;
								if(pids == null) pids = new List<int>(16);
								pids.Add(pid); //add bad pid
								continue;
							}
						}
					}

					if(_also != null && !_also(w)) continue;

					if(_contains != null) {
						bool found = false;
						switch(_contains) {
						case Acc.Finder f: found = f.Find(w); break;
						case ChildFinder f: found = f.Find(w); break;
						//case System.Drawing.Image f: found = null != WinImage.Find(w, f, WIFlags.WindowDC); break;
						default: found = _FIL_Image(w, _contains); break; //avoid loading System.Drawing.dll
						}
						if(!found) continue;
					}

					if(getAll != null) {
						getAll(w);
						continue;
					}

					Result = w;
					return index;
				}

				return -1;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			static bool _FIL_Image(Wnd w, object contains)
			{
				var im = contains as System.Drawing.Image;
				return null != WinImage.Find(w, im, WIFlags.WindowDC); //FUTURE: optimize
			}

			/// <summary>
			/// Returns true if window w properties match the specified properties.
			/// </summary>
			/// <param name="w">A top-level window. If 0 or invalid, returns false.</param>
			/// <param name="cache">Can be used to make faster when multiple <b>Finder</b> variables are used with same window. The function gets window name/class/program once, and stores in <paramref name="cache"/>; next time it gets these strings from <paramref name="cache"/>.</param>
			public bool IsMatch(Wnd w, WFCache cache = null)
			{
				return 0 == _FindInList(new _WndList(w), cache: cache);
			}
		}

		/// <summary>
		/// Finds a top-level window and returns its handle as Wnd.
		/// Returns default(Wnd) if not found. See examples.
		/// </summary>
		/// <param name="name">
		/// Window name. Usually it is the title bar text.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'can be any'. "" means 'must not have name'.
		/// </param>
		/// <param name="cn">
		/// Window class name.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'can be any'. Cannot be "".
		/// You can see window name, class name and program in editor's status bar and dialog "Find window or control".
		/// </param>
		/// <param name="program">
		/// Program file name, like "notepad.exe".
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'can be any'. Cannot be "". Cannot be path.
		/// Or <see cref="WF3.Process"/>(process id), <see cref="WF3.Thread"/>(thread id), <see cref="WF3.Owner"/>(owner window).
		/// See <see cref="ProcessId"/>, <see cref="Process_.CurrentProcessId"/>, <see cref="ThreadId"/>, <see cref="Thread_.NativeId"/>, <see cref="Owner"/>.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Callback function. Called for each matching window.
		/// It can evaluate more properties of the window and return true when they match.
		/// Example: <c>also: t =&gt; !t.IsPopupWindow</c>.
		///
		/// Called after evaluating all other parameters except <paramref name="contains"/>.
		/// </param>
		/// <param name="contains">
		/// Text, image or other object in the client area of the window. Depends on type:
		/// <see cref="Acc.Finder"/> - arguments for <see cref="Acc.Find"/>. Defines an accessible object that must be in the window.
		/// <see cref="ChildFinder"/> - arguments for <see cref="Child"/>. Defines a child control that must be in the window.
		/// <see cref="System.Drawing.Bitmap"/> - image that must be visible in the window. This function calls <see cref="WinImage.Find"/> with flag <see cref="WIFlags.WindowDC"/>. See also <see cref="WinImage.LoadImage"/>.
		/// string - an object that must be in the window. Depends on string format:
		/// <list type="bullet">
		/// <item>"'role' name" or "name" - accessible object. See <see cref="Acc.Find"/>.</item>
		/// <item>"'!cn' name" or "'!' name" - child control. See <see cref="Child"/>.</item>
		/// <item>"image:..." - image. See <see cref="WinImage.Find"/>, <see cref="WinImage.LoadImage"/>.</item>
		/// </list>
		///
		/// This parameter is evaluated after <paramref name="also"/>.
		/// </param>
		/// <remarks>
		/// To create code for this function, use dialog "Find window or control". It is form <b>Au.Tools.Form_Wnd</b> in Au.Tools.dll.
		/// 
		/// If there are multiple matching windows, gets the first in the Z order matching window, preferring visible windows.
		/// On Windows 8 and later finds only desktop windows, not Windows Store app Metro-style windows (on Windows 10 few such windows exist), unless this process has uiAccess or High+uiAccess or has disableWindowFiltering in manifest; to find such windows you can use <see cref="FindFast"/>.
		/// To find message-only windows use <see cref="Misc.FindMessageOnlyWindow"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="cn"/> is "". To match any, use null.
		/// <paramref name="program"/> is "" or 0 or contains \ or /. To match any, use null.
		/// Invalid wildcard expression ("**options " or regular expression).
		/// Invalid image string in <paramref name="contains"/>.
		/// </exception>
		/// <example>
		/// Try to find Notepad window. Return if not found.
		/// <code>
		/// Wnd w = Wnd.Find("* Notepad");
		/// if(w.Is0) { Print("not found"); return; }
		/// </code>
		/// Try to find Notepad window. Throw NotFoundException if not found.
		/// <code>
		/// Wnd w1 = Wnd.Find("* Notepad").OrThrow();
		/// </code>
		/// </example>
		[MethodImpl(MethodImplOptions.NoInlining)] //inlined code makes harder to debug using disassembly
		public static Wnd Find(
			string name = null, string cn = null, WF3 program = default,
			WFFlags flags = 0, Func<Wnd, bool> also = null, object contains = null)
		{
			var f = new Finder(name, cn, program, flags, also, contains);
			f.Find();
			//LastFind = f;
			return f.Result;
		}

		//rejected: probably most users will not understand what is it, and will not use. It's easy and more clear to create and use Finder instances.
		///// <summary>
		///// Gets arguments and result of this thread's last call to <see cref="Find"/> or <see cref="FindAll"/>.
		///// </summary>
		///// <remarks>
		///// <b>Wnd.Wait</b> and similar functions don't change this property. <see cref="FindOrRun"/> and some other functions of this library change this property because they call <see cref="Find"/> internally.
		///// </remarks>
		///// <example>
		///// This example is similar to what <see cref="FindOrRun"/> does.
		///// <code><![CDATA[
		///// Wnd w = Wnd.Find("*- Notepad", "Notepad");
		///// if(w.Is0) { Shell.Run("notepad.exe"); w = Wnd.WaitAny(60, true, Wnd.LastFind); }
		///// ]]></code>
		///// </example>
		//[field: ThreadStatic]
		//public static Finder LastFind { get; set; }

		/// <inheritdoc cref="Find"/>
		/// <summary>
		/// Finds all matching windows.
		/// Returns array containing 0 or more window handles as Wnd.
		/// Parameters etc are the same as <see cref="Find"/>.
		/// </summary>
		/// <remarks>
		/// The list is sorted to match the Z order, however hidden windows (when using <see cref="WFFlags.HiddenToo"/>) are always after visible windows.
		/// </remarks>
		/// <seealso cref="GetWnd.AllWindows"/>
		/// <seealso cref="GetWnd.MainWindows"/>
		/// <seealso cref="GetWnd.ThreadWindows"/>
		public static Wnd[] FindAll(
			string name = null, string cn = null, WF3 program = default,
			WFFlags flags = 0, Func<Wnd, bool> also = null, object contains = null)
		{
			var f = new Finder(name, cn, program, flags, also, contains);
			var a = f.FindAll();
			//LastFind = f;
			return a;
		}

		/// <summary>
		/// Finds a top-level window and returns its handle as Wnd.
		/// Returns default(Wnd) if not found. See also: <see cref="Is0"/>, <see cref="ExtensionMethods.OrThrow(Wnd)" r=""/>.
		/// </summary>
		/// <param name="name">
		/// Name.
		/// Use null to match any.
		/// Full, case-insensitive. Wildcard etc not supported.
		/// </param>
		/// <param name="cn">
		/// Class name.
		/// Use null to match any. Cannot be "".
		/// Full, case-insensitive. Wildcard etc not supported.
		/// </param>
		/// <param name="wAfter">If used, starts searching from the next window in the Z order.</param>
		/// <remarks>
		/// Calls API <msdn>FindWindowEx</msdn>.
		/// Faster than <see cref="Find"/>, which uses API <msdn>EnumWindows</msdn>.
		/// Finds hidden windows too.
		/// To find message-only windows use <see cref="Misc.FindMessageOnlyWindow"/> instead.
		/// Supports <see cref="WinError.Code"/>.
		/// It is not recommended to use this function in a loop to enumerate windows. It would be unreliable because window positions in the Z order can be changed while enumerating. Also then it would be slower than <b>Find</b> and <b>FindAll</b>.
		/// </remarks>
		public static Wnd FindFast(string name, string cn, Wnd wAfter = default)
		{
			return Api.FindWindowEx(default, wAfter, cn, name);
		}

		public static partial class Misc
		{
			/// <summary>
			/// Finds a message-only window and returns its handle as Wnd.
			/// Returns default(Wnd) if not found.
			/// </summary>
			/// <param name="name">
			/// Name.
			/// Use null to match any.
			/// Full, case-insensitive. Wildcard etc not supported.
			/// </param>
			/// <param name="cn">
			/// Class name.
			/// Use null to match any. Cannot be "".
			/// Full, case-insensitive. Wildcard etc not supported.
			/// </param>
			/// <param name="wAfter">If used, starts searching from the next window in the Z order.</param>
			/// <remarks>
			/// Calls API <msdn>FindWindowEx</msdn>.
			/// Faster than <see cref="Find"/>, which does not find message-only windows.
			/// Finds hidden windows too.
			/// Supports <see cref="WinError.Code"/>.
			/// </remarks>
			public static Wnd FindMessageOnlyWindow(string name, string cn, Wnd wAfter = default)
			{
				return Api.FindWindowEx(Native.HWND.MESSAGE, wAfter, cn, name);
			}
		}

		/// <inheritdoc cref="Find"/>
		/// <summary>
		/// Finds a top-level window (<see cref="Find"/>). If found, activates (optionally), else calls callback function and waits for the window. The callback should open the window, for example call <see cref="Shell.Run"/>.
		/// Returns window handle as Wnd. Returns default(Wnd) if not found (if <paramref name="runWaitS"/> is negative; else exception).
		/// </summary>
		/// <param name="run">Callback function. See example.</param>
		/// <param name="runWaitS">How long to wait for the window after calling the callback function. Seconds. Default 60. See <see cref="Wait"/>.</param>
		/// <param name="needActiveWindow">Finally the window must be active. Default: true.</param>
		/// <exception cref="TimeoutException"><paramref name="runWaitS"/> time has expired. Not thrown if <paramref name="runWaitS"/> &lt;= 0.</exception>
		/// <remarks>
		/// The algorithm is:
		/// <code>
		/// var w=Wnd.Find(...);
		/// if(w.Is0) { run(); w=Wnd.Wait(runWaitS, needActiveWindow, ...); }
		/// else if(needActiveWindow) w.Activate();
		/// return w;
		/// </code>
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.FindOrRun("* Notepad", run: () => Shell.Run("notepad.exe"));
		/// Print(w);
		/// ]]></code>
		/// </example>
		public static Wnd FindOrRun(
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
			string name = null, string cn = null, WF3 program = default,
			WFFlags flags = 0, Func<Wnd, bool> also = null, object contains = null,
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
			Action run = null, double runWaitS = 60.0, bool needActiveWindow = true)
		{
			Wnd w = default;
			var f = new Finder(name, cn, program, flags, also, contains);
			if(f.Find()) {
				w = f.Result;
				if(needActiveWindow) w.Activate();
			} else if(run != null) {
				run();
				w = WaitAny(runWaitS, needActiveWindow, f);
			}
			return w;
		}

		public partial struct GetWnd
		{
			/// <summary>
			/// Gets top-level windows.
			/// Returns array containing window handles as Wnd.
			/// </summary>
			/// <param name="onlyVisible">
			/// Need only visible windows.
			/// Note: this function does not check whether windows are cloaked, as it is rather slow. Use <see cref="IsCloaked"/> if need.
			/// </param>
			/// <param name="sortFirstVisible">
			/// Place hidden windows at the end of the array. If false, the order of array elements matches the Z order.
			/// Not used when <paramref name="onlyVisible"/> is true.</param>
			/// <remarks>
			/// Calls API <msdn>EnumWindows</msdn>.
			/// <note>The array can be bigger than you expect, because there are many invisible windows, tooltips, etc. See also <see cref="MainWindows"/>.</note>
			/// Does not get message-only windows. Use <see cref="Misc.FindMessageOnlyWindow"/> if need.
			/// On Windows 8 and later does not get Windows Store app Metro-style windows (on Windows 10 few such windows exist), unless this process has <see cref="Uac">UAC</see> integrity level uiAccess or High+uiAccess or its manifest contains disableWindowFiltering; to get such windows you can use <see cref="FindFast"/>.
			/// Tip: To get top-level and child windows in single array: <c>var a = Wnd.GetWnd.Root.Get.Children();</c>.
			/// </remarks>
			/// <seealso cref="Children"/>
			/// <seealso cref="FindAll"/>
			public static Wnd[] AllWindows(bool onlyVisible = false, bool sortFirstVisible = false)
			{
				return Lib.EnumWindows(Lib.EnumAPI.EnumWindows, onlyVisible, sortFirstVisible);
			}

			/// <inheritdoc cref="AllWindows(bool, bool)"/>
			/// <summary>
			/// Gets top-level windows.
			/// </summary>
			/// <param name="a">Receives window handles as Wnd. If null, this function creates new List, else clears before adding items.</param>
			/// <param name="onlyVisible"><inheritdoc cref="AllWindows(bool, bool)"/></param>
			/// <param name="sortFirstVisible"><inheritdoc cref="AllWindows(bool, bool)"/></param>
			/// <remarks>
			/// Use this overload to avoid much garbage when calling frequently with the same List variable. Other overload always allocates new array. This overload in most cases reuses memory allocated for the list variable.
			/// </remarks>
			public static void AllWindows(ref List<Wnd> a, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				Lib.EnumWindows2(Lib.EnumAPI.EnumWindows, onlyVisible, sortFirstVisible, list: a ?? (a = new List<Wnd>()));
			}

			/// <summary>
			/// Gets top-level windows of a thread.
			/// Returns array containing 0 or more window handles as Wnd.
			/// </summary>
			/// <param name="threadId">
			/// Unmanaged thread id.
			/// See <see cref="Thread_.NativeId"/>, <see cref="ThreadId"/>.
			/// If 0, throws exception. If other invalid value (ended thread?), returns empty list. Supports <see cref="WinError.Code"/>.
			/// </param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <param name="sortFirstVisible">Place all array elements of hidden windows at the end of the array, even if the hidden windows are before some visible windows in the Z order.</param>
			/// <exception cref="ArgumentException">0 threadId.</exception>
			/// <remarks>
			/// Calls API <msdn>EnumThreadWindows</msdn>.
			/// </remarks>
			/// <seealso cref="Thread_.HasMessageLoop"/>
			public static Wnd[] ThreadWindows(int threadId, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				if(threadId == 0) throw new ArgumentException("0 threadId.");
				return Lib.EnumWindows(Lib.EnumAPI.EnumThreadWindows, onlyVisible, sortFirstVisible, threadId: threadId);
			}

			/// <inheritdoc cref="ThreadWindows(int, bool, bool)"/>
			/// <summary>
			/// Gets top-level windows of a thread.
			/// </summary>
			/// <remarks><inheritdoc cref="AllWindows(ref List{Wnd}, bool, bool)"/></remarks>
			public static void ThreadWindows(ref List<Wnd> a, int threadId, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				if(threadId == 0) throw new ArgumentException("0 threadId.");
				Lib.EnumWindows2(Lib.EnumAPI.EnumThreadWindows, onlyVisible, sortFirstVisible, threadId: threadId, list: a ?? (a = new List<Wnd>()));
			}
		}

		/// <summary>
		/// Internal static functions.
		/// </summary>
		internal static partial class Lib
		{
			internal enum EnumAPI { EnumWindows, EnumThreadWindows, EnumChildWindows, }

			internal static Wnd[] EnumWindows(EnumAPI api,
				bool onlyVisible, bool sortFirstVisible, Wnd wParent = default, bool directChild = false, int threadId = 0)
			{
				using(var a = EnumWindows2(api, onlyVisible, sortFirstVisible, wParent, directChild, threadId)) {
					return a.ToArray();
				}
			}

			/// <summary>
			/// This version creates much less garbage.
			/// The caller must dispose the returned LibArrayBuilder, unless list is not null.
			/// If list is not null, adds windows there (clears at first) and returns default(LibArrayBuilder).
			/// </summary>
			internal static Util.LibArrayBuilder<Wnd> EnumWindows2(EnumAPI api,
				bool onlyVisible, bool sortFirstVisible = false, Wnd wParent = default, bool directChild = false, int threadId = 0,
				Func<Wnd, object, bool> predicate = null, object predParam = default, List<Wnd> list = null)
			{
				if(directChild && wParent == GetWnd.Root) { api = EnumAPI.EnumWindows; wParent = default; }

				Util.LibArrayBuilder<Wnd> ab = default;
				bool disposeArray = true;
				var d = new _EnumData { api = api, onlyVisible = onlyVisible, directChild = directChild, wParent = wParent };
				try {
					switch(api) {
					case EnumAPI.EnumWindows:
						Api.EnumWindows(_wndEnumProc, &d);
						break;
					case EnumAPI.EnumThreadWindows:
						Api.EnumThreadWindows(threadId, _wndEnumProc, &d);
						break;
					case EnumAPI.EnumChildWindows:
						Api.EnumChildWindows(wParent, _wndEnumProc, &d);
						break;
					}

					int n = d.len;
					if(n > 0) {
						if(predicate != null) {
							n = 0;
							for(int i = 0; i < d.len; i++) {
								if(predicate((Wnd)d.a[i], predParam)) d.a[n++] = d.a[i];
							}
						}

						if(list != null) {
							list.Clear();
							if(list.Capacity < n) list.Capacity = n + n / 2;
						} else {
							ab.Alloc(n, zeroInit: false, noExtra: true);
						}
						if(sortFirstVisible && !onlyVisible) {
							int j = 0;
							for(int i = 0; i < n; i++) {
								var w = (Wnd)d.a[i];
								if(!_EnumIsVisible(w, api, wParent)) continue;
								if(list != null) list.Add(w); else ab[j++] = w;
								d.a[i] = 0;
							}
							for(int i = 0; i < n; i++) {
								int wi = d.a[i];
								if(wi == 0) continue;
								var w = (Wnd)wi;
								if(list != null) list.Add(w); else ab[j++] = w;
							}
						} else if(list != null) {
							for(int i = 0; i < n; i++) list.Add((Wnd)d.a[i]);
						} else {
							for(int i = 0; i < n; i++) ab[i] = (Wnd)d.a[i];
						}
					}
					disposeArray = false;
					return ab;
				}
				finally {
					Util.NativeHeap.Free(d.a);
					if(disposeArray) ab.Dispose();
				}
			}
			static Api.WNDENUMPROC _wndEnumProc = (w, p) => ((_EnumData*)p)->Proc(w);

			struct _EnumData
			{
				public int* a;
				public int len;
				int _cap;
				public EnumAPI api;
				public bool onlyVisible, directChild;
				public Wnd wParent;

				public int Proc(Wnd w)
				{
					if(onlyVisible && !_EnumIsVisible(w, api, wParent)) return 1;
					if(api == EnumAPI.EnumChildWindows) {
						if(directChild && w.LibParentGWL != wParent) return 1;
					} else {
						if(!wParent.Is0 && w.Owner != wParent) return 1;
					}
					if(a == null) a = (int*)Util.NativeHeap.Alloc((_cap = onlyVisible ? 200 : 1000) * 4);
					else if(len == _cap) a = (int*)Util.NativeHeap.ReAlloc(a, (_cap *= 2) * 4);
					a[len++] = (int)w;
					return 1;
				}

				//note: need this in exe manifest. Else EnumWindows skips "immersive" windows if this process is not admin/uiAccess.
				/*
<asmv3:application>
...
<asmv3:windowsSettings xmlns="http://schemas.microsoft.com/SMI/2011/WindowsSettings">
  <disableWindowFiltering>true</disableWindowFiltering>
</asmv3:windowsSettings>
</asmv3:application>
				*/
			}

			static bool _EnumIsVisible(Wnd w, EnumAPI api, Wnd wParent)
				=> api == EnumAPI.EnumChildWindows ? w.LibIsVisibleIn(wParent) : w.IsVisible;
		}

		/// <summary>
		/// An enumerable list of Wnd for <see cref="Finder._FindInList"/> and <see cref="ChildFinder._FindInList"/>.
		/// Holds Util.LibArrayBuilder or IEnumerator or single Wnd or none.
		/// Must be disposed if it is Util.LibArrayBuilder or IEnumerator, else disposing is optional.
		/// </summary>
		struct _WndList : IDisposable
		{
			internal enum ListType { None, ArrayBuilder, Enumerator, SingleWnd }

			ListType _t;
			int _i;
			Wnd _w;
			IEnumerator<Wnd> _en;
			Util.LibArrayBuilder<Wnd> _ab;

			internal _WndList(Util.LibArrayBuilder<Wnd> ab) : this()
			{
				_ab = ab;
				_t = ListType.ArrayBuilder;
			}

			internal _WndList(IEnumerable<Wnd> en) : this()
			{
				var e = en?.GetEnumerator();
				if(e != null) {
					_en = e;
					_t = ListType.Enumerator;
				}
			}

			internal _WndList(Wnd w) : this()
			{
				if(!w.Is0) {
					_w = w;
					_t = ListType.SingleWnd;
				}
			}

			internal ListType Type => _t;

			internal bool Next(out Wnd w)
			{
				w = default;
				switch(_t) {
				case ListType.ArrayBuilder:
					if(_i == _ab.Count) return false;
					w = _ab[_i++];
					break;
				case ListType.Enumerator:
					if(!_en.MoveNext()) return false;
					w = _en.Current;
					break;
				case ListType.SingleWnd:
					if(_i > 0) return false;
					_i = 1; w = _w;
					break;
				default:
					return false;
				}
				return true;
			}

			public void Dispose()
			{
				switch(_t) {
				case ListType.ArrayBuilder: _ab.Dispose(); break;
				case ListType.Enumerator: _en.Dispose(); break;
				}
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags of <see cref="Wnd.Find"/>.
	/// </summary>
	[Flags]
	public enum WFFlags
	{
		/// <summary>
		/// Can find invisible windows. See <see cref="Wnd.IsVisible"/>.
		/// Use this carefully. Always use <i>cn</i> (class name), not just <i>name</i>, to avoid finding a wrong window with the same name.
		/// </summary>
		HiddenToo = 1,

		/// <summary>
		/// Can find cloaked windows. See <see cref="Wnd.IsCloaked"/>.
		/// Cloaked are windows hidden not in the classic way, therefore <see cref="Wnd.IsVisible"/> does not detect it, but <see cref="Wnd.IsCloaked"/> detects. For example, windows on inactive Windows 10 virtual desktops, ghost windows of inactive Windows Store apps, various hidden system windows.
		/// Use this carefully. Always use <i>cn</i> (class name), not just <i>name</i>, to avoid finding a wrong window with the same name.
		/// </summary>
		CloakedToo = 2,
	}

	/// <summary>
	/// <i>program</i> of <see cref="Wnd.Find"/>.
	/// Program name, process id, thread id or owner window handle.
	/// </summary>
	public struct WF3
	{
		object _o;
		WF3(object o) => _o = o;

		/// <summary>Program name like "notepad.exe", or null.</summary>
		public static implicit operator WF3(string program) => new WF3(program);

		/// <summary>Process id.</summary>
		public static WF3 Process(int processId) => new WF3(processId);

		/// <summary>Thread id.</summary>
		public static WF3 Thread(int threadId) => new WF3((uint)threadId);

		/// <summary>Owner window.</summary>
		public static WF3 Owner(AnyWnd ownerWindow) => new WF3(ownerWindow);

		/// <summary>
		/// Gets program name or process id or thread id or owner window.
		/// Other variables will be null/0.
		/// </summary>
		/// <exception cref="ArgumentException">The value is "" or 0 or contains \ or /.</exception>
		public void GetValue(out Wildex program, out int pid, out int tid, out Wnd owner)
		{
			program = null; pid = 0; tid = 0; owner = default;
			switch(_o) {
			case string s:
				//LibThrowIfInvalid(s);
				if(s.Length == 0) throw new ArgumentException("Program name cannot be \"\". Use null to match any.");
				if(!s.StartsWith_("**")) { //can be regex
					if(s.IndexOfAny(String_.Lib.pathSep) >= 0) throw new ArgumentException("Program name contains \\ or /.");
					if(Path_.FindExtension(s) < 0 && !Wildex.HasWildcards(s)) PrintWarning("Program name without .exe.");
				}
				program = s;
				break;
			case int i:
				if(i == 0) throw new ArgumentException("0 process id");
				pid = i;
				break;
			case uint i:
				if(i == 0) throw new ArgumentException("0 thread id");
				tid = (int)i;
				break;
			case AnyWnd aw:
				var w = aw.Wnd;
				if(w.Is0) throw new ArgumentException("0 window handle");
				owner = w;
				break;
			}
		}

		///// <exception cref="ArgumentException">The value is "" or 0 or contains \ or /.</exception>
		//internal static void LibThrowIfInvalid(string s)
		//{
		//	if(s.Length == 0) throw new ArgumentException("Program name cannot be \"\". Use null to match any.");
		//	if(s.IndexOfAny(String_.Lib.pathSep) >= 0) throw new ArgumentException("Program name contains \\ or /.");
		//	if(Path_.FindExtension(s) < 0) PrintWarning("Program name without .exe.");
		//}

		/// <summary>
		/// Returns true if nothing was assigned to this variable.
		/// </summary>
		public bool IsEmpty => _o == null;
	}

	/// <summary>
	/// Can be used with <see cref="Wnd.Finder.IsMatch"/>.
	/// </summary>
	public class WFCache
	{
		Wnd _w;
		long _time;
		internal string Name, Class, Program;
		internal int Tid, Pid;

		/// <summary>
		/// Cache window name.
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// Window name is not cached by default because can be changed. Window class name and program name are always cached because cannot be changed.
		/// </remarks>
		public bool CacheName { get; set; }

		/// <summary>
		/// Don't auto-clear cached properties on timeout.
		/// </summary>
		public bool NoTimeout { get; set; }

		internal void Begin(Wnd w)
		{
			if(NoTimeout) {
				if(w != _w) {
					Clear();
					_w = w;
				}
			} else {
				var t = Api.GetTickCount64();
				if(w != _w || t - _time > 2500) {
					Clear();
					if(w.IsAlive) { _w = w; _time = t; }
				}
				//else if(CacheName && t - _time > 100) Name = null; //no, instead let call Clear if need
			}
		}

		/// <summary>
		/// Clears all cached properties, or only name.
		/// </summary>
		/// <remarks>
		/// Usually don't need to call this function. It is implicitly called when the variable is used with a new window.
		/// </remarks>
		/// <param name="onlyName">Clear only name (because it may change, unlike other cached properties).</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear(bool onlyName = false)
		{
			if(onlyName) Name = null;
			else { _w = default; _time = 0; Name = Class = Program = null; Tid = Pid = 0; }
		}

		/// <summary>
		/// Match invisible and cloaked windows too, even if the flags are not set (see <see cref="WFFlags"/>).
		/// </summary>
		public bool IgnoreVisibility { get; set; }
	}
}
