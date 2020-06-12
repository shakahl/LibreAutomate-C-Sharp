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
	public unsafe partial struct AWnd
	{
		/// <summary>
		/// Contains top-level window properties and can be used to find the window.
		/// </summary>
		/// <remarks>
		/// Can be used instead of <see cref="AWnd.Find"/> or <see cref="AWnd.FindAll"/>.
		/// These codes are equivalent:
		/// <code>AWnd w = AWnd.Find(a, b, c, d, e); if(!w.Is0) AOutput.Write(w);</code>
		/// <code>var p = new AWnd.Finder(a, b, c, d, e); if(p.Find()) AOutput.Write(p.Result);</code>
		/// Also can find in a list of windows.
		/// </remarks>
		public class Finder
		{
			readonly AWildex _name;
			readonly AWildex _cn;
			readonly AWildex _program;
			readonly Func<AWnd, bool> _also;
			readonly WFlags _flags;
			readonly int _processId;
			readonly int _threadId;
			readonly AWnd _owner;
			readonly WContains _contains;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
			/// <summary>
			/// Parsed parameter values. All read-only.
			/// </summary>
			public TProps Props => new TProps(this);

			[NoDoc]
			public struct TProps
			{
				readonly Finder _f;
				internal TProps(Finder f) { _f = f; }

				public AWildex name => _f._name;
				public AWildex cn => _f._cn;
				public AWildex program => _f._program;
				public Func<AWnd, bool> also => _f._also;
				public WFlags flags => _f._flags;
				public int processId => _f._processId;
				public int threadId => _f._threadId;
				public AWnd owner => _f._owner;
				public WContains contains => _f._contains;

				/// <summary>
				/// After unsuccesful <see cref="IsMatch"/> indicates the parameter that does not match.
				/// </summary>
				public EProps DoesNotMatch => _f._stopProp;
			}

			EProps _stopProp;

			[NoDoc]
			public enum EProps { name = 1, cn, of, also, contains, visible, cloaked, }

			public override string ToString()
			{
				using(new Util.StringBuilder_(out var b)) {
					_Append("name", _name);
					_Append("cn", _cn);
					if(_program != null) _Append("program", _program); else if(_processId != 0) _Append("processId", _processId); else if(_threadId != 0) _Append("threadId", _threadId);
					if(_also != null) _Append("also", "");
					_Append("contains", _contains.Value);
					return b.ToString();

					void _Append(string k, object v)
					{
						if(v == null) return;
						if(b.Length != 0) b.Append(", ");
						b.Append(k).Append('=').Append(v);
					}
				}
			}
#pragma warning restore CS1591

			/// <summary>
			/// See <see cref="AWnd.Find"/>.
			/// </summary>
			/// <exception cref="ArgumentException">See <see cref="AWnd.Find"/>.</exception>
			public Finder(
				[ParamString(PSFormat.AWildex)] string name = null,
				[ParamString(PSFormat.AWildex)] string cn = null,
				[ParamString(PSFormat.AWildex)] WOwner of = default,
				WFlags flags = 0, Func<AWnd, bool> also = null, WContains contains = default)
			{
				_name = name;
				if(cn != null) _cn = cn.Length != 0 ? cn : throw new ArgumentException("Class name cannot be \"\". Use null.");
				of.GetValue(out _program, out _processId, out _threadId, out _owner);
				_flags = flags;
				_also = also;
				_contains = contains;
			}

			/// <summary>
			/// Implicit conversion from string that can contain window name, class name, program and/or a <i>contains</i> object.
			/// Examples: <c>"name,cn,program"</c>, <c>"name"</c>, <c>",cn"</c>, <c>",,program"</c>, <c>"name,cn"</c>, <c>"name,,program"</c>, <c>",cn,program"</c>, <c>"name,,,object"</c>.
			/// </summary>
			/// <param name="s">
			/// One or more comma-separated window properties: name, class, program and/or a <i>contains</i> object. Empty parts are considered null.
			/// The same as parameters of <see cref="AWnd.Find"/>. The first 3 parts are <i>name</i>, <i>cn</i> and <i>of</i>. The last part is <i>contains</i> as string; can specify an accessible object, control or image.
			/// The first 3 comma-separated parts cannot contain commas. Alternatively, parts can be separated by '\0' characters, like <c>"name\0"+"cn\0"+"program\0"+"object"</c>. Then parts can contain commas. Example: <c>"*one, two, three*\0"</c> (name with commas).
			/// </param>
			/// <exception cref="ArgumentException">See <see cref="AWnd.Find"/>.</exception>
			/// <exception cref="Exception">If specifies a <i>contains</i> object: exceptions of constructor of <see cref="ChildFinder"/> or <see cref="AAcc.Finder"/> or <see cref="AWinImage.Finder"/>.</exception>
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
			public AWnd Result { get; internal set; }

			/// <summary>
			/// Finds the specified window, like <see cref="AWnd.Find"/>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
			public bool Find()
			{
				using var k = new _WndList(_AllWindows());
				return _FindOrMatch(k) >= 0;
			}

			Util.ArrayBuilder_<AWnd> _AllWindows()
			{
				//FUTURE: optimization: if cn not wildcard etc, at first find atom.
				//	If not found, don't search. If found, compare atom, not class name string.

				var f = _threadId != 0 ? Internal_.EnumAPI.EnumThreadWindows : Internal_.EnumAPI.EnumWindows;
				return Internal_.EnumWindows2(f, 0 == (_flags & WFlags.HiddenToo), true, wParent: _owner, threadId: _threadId);
			}

			/// <summary>
			/// Finds the specified window in a list of windows.
			/// Returns 0-based index, or -1 if not found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
			/// <param name="a">Array or list of windows, for example returned by <see cref="GetWnd.AllWindows"/>.</param>
			public int FindInList(IEnumerable<AWnd> a)
			{
				using var k = new _WndList(a);
				return _FindOrMatch(k);
			}

			/// <summary>
			/// Finds all matching windows, like <see cref="AWnd.FindAll"/>.
			/// Returns array containing 0 or more window handles as <b>AWnd</b>.
			/// </summary>
			public AWnd[] FindAll()
			{
				return _FindAll(new _WndList(_AllWindows()));
			}

			/// <summary>
			/// Finds all matching windows in a list of windows.
			/// Returns array containing 0 or more window handles as <b>AWnd</b>.
			/// </summary>
			/// <param name="a">Array or list of windows, for example returned by <see cref="GetWnd.AllWindows"/>.</param>
			public AWnd[] FindAllInList(IEnumerable<AWnd> a)
			{
				return _FindAll(new _WndList(a));
			}

			AWnd[] _FindAll(_WndList k)
			{
				using(k) {
					using var ab = new Util.ArrayBuilder_<AWnd>();
					_FindOrMatch(k, w => ab.Add(w)); //CONSIDER: ab could be part of _WndList. Now the delegate creates garbage.
					return ab.ToArray();
				}
			}

			/// <summary>
			/// Returns index of matching element or -1.
			/// Returns -1 if using getAll.
			/// </summary>
			/// <param name="a">List of AWnd. Does not dispose it.</param>
			/// <param name="getAll">If not null, calls it for all matching and returns -1.</param>
			/// <param name="cache"></param>
			int _FindOrMatch(_WndList a, Action<AWnd> getAll = null, WFCache cache = null)
			{
				Result = default;
				_stopProp = 0;
				if(a.Type == _WndList.ListType.None) return -1;
				bool inList = a.Type != _WndList.ListType.ArrayBuilder;
				bool ignoreVisibility = cache?.IgnoreVisibility ?? false;
				bool mustBeVisible = inList && (_flags & WFlags.HiddenToo) == 0 && !ignoreVisibility;
				bool isOwner = inList && !_owner.Is0;
				bool isTid = inList ? _threadId != 0 : false;
				List<int> pids = null; bool programNamePlanB = false; //variables for faster getting/matching program name

				for(int index = 0; a.Next(out AWnd w); index++) {
					if(w.Is0) continue;

					//With warm CPU, speed of 1000 times getting:
					//name 400, class 400, foreign pid/tid 400,
					//owner 55, rect 55, style 50, exstyle 50, cloaked 280,
					//GetProp(string) 1700, GetProp(atom) 300, GlobalFindAtom 650,
					//program >=2500

					if(mustBeVisible) {
						if(!w.IsVisible) { _stopProp = EProps.visible; continue; }
					}

					if(isOwner) {
						if(_owner != w.OwnerWindow) { _stopProp = EProps.of; continue; }
					}

					cache?.Begin(w);

					if(_name != null) {
						var s = cache != null && cache.CacheName ? (cache.Name ?? (cache.Name = w.NameTL_)) : w.NameTL_;
						if(!_name.Match(s)) { _stopProp = EProps.name; continue; }
						//note: name is before classname. It makes faster in slowest cases (HiddenToo), because most windows are nameless.
					}

					if(_cn != null) {
						var s = cache != null ? (cache.Class ?? (cache.Class = w.ClassName)) : w.ClassName;
						if(!_cn.Match(s)) { _stopProp = EProps.cn; continue; }
					}

					if(0 == (_flags & WFlags.CloakedToo) && !ignoreVisibility) {
						if(w.IsCloaked) { _stopProp = EProps.cloaked; continue; }
					}

					int pid = 0, tid = 0;
					if(_program != null || _processId != 0 || isTid) {
						if(cache != null) {
							if(cache.Tid == 0) cache.Tid = w.GetThreadProcessId(out cache.Pid);
							tid = cache.Tid; pid = cache.Pid;
						} else tid = w.GetThreadProcessId(out pid);
						if(tid == 0) { _stopProp = EProps.of; continue; }
						//speed: with foreign processes the same speed as getting name or class name. Much faster if same process.
					}

					if(isTid) {
						if(_threadId != tid) { _stopProp = EProps.of; continue; }
					}

					if(_processId != 0) {
						if(_processId != pid) { _stopProp = EProps.of; continue; }
					}

					if(_program != null) {
						//Getting program name is one of slowest parts.
						//Usually it does not slow down much because need to do it only 1 or several times, only when window name, class etc match.
						//The worst case is when only program is specified, and the very worst case is when also using flag HiddenToo.
						//We are prepared for the worst case.
						//Normally we call AProcess.GetName. In most cases it is quite fast.
						//Anyway, we use this optimization:
						//	Add pid of processes that don't match the specified name in the pids list (bad pids).
						//	Next time, if pid is in the bad pids list, just continue, don't need to get program name again.
						//However in the worst case we would encounter some processes that AProcess.GetName cannot get name using the fast API.
						//For each such process it would then use the much slower 'get all processes' API, which is almost as slow as Process.GetProcessById(pid).ProgramName.
						//To solve this:
						//We tell AProcess.GetName to not use the slow API, but just return null when the fast API fails.
						//When it happens (AProcess.GetName returns null):
						//	If need full path: continue, we cannot do anything more.
						//	Switch to plan B and no longer use all the above. Plan B:
						//	Get list of pids of all processes that match _program. For it we call AProcess.GetProcessesByName_, which uses the same slow API, but we call it just one time.
						//	If it returns null (it means there are no matching processes), break (window not found).
						//	From now, in each loop will need just to find pid in the returned list, and continue if not found.

						_stopProp = EProps.of;
						g1:
						if(programNamePlanB) {
							if(!pids.Contains(pid)) continue;
						} else {
							if(pids != null && pids.Contains(pid)) continue; //is known bad pid?

							string pname = cache != null ? (cache.Program ?? (cache.Program = _Program())) : _Program();
							string _Program() => AProcess.GetName(pid, false, true);
							//string _Program() => AProcess.GetName(pid, 0!=(_flags&WFlags.ProgramPath), true);

							if(pname == null) {
								//if(0!=(_flags&WFlags.ProgramPath)) continue;

								//switch to plan B
								AProcess.GetProcessesByName_(ref pids, _program);
								if(pids.NE_()) break;
								programNamePlanB = true;
								goto g1;
							}

							if(!_program.Match(pname)) {
								if(a.Type == _WndList.ListType.SingleWnd) break;
								pids ??= new List<int>(16);
								pids.Add(pid); //add bad pid
								continue;
							}
						}
						_stopProp = 0;
					}

					if(_also != null) {
						bool ok = false;
						try { ok = _also(w); }
						catch(AuWndException) { } //don't throw if w destroyed
						if(!ok) { _stopProp = EProps.also; continue; }
					}

					if(_contains.Value != null) {
						bool found = false;
						try {
							switch(_contains.Value) {
							case AAcc.Finder f: found = f.Find(w); break;
							case ChildFinder f: found = f.Find(w); break;
							case AWinImage.Finder f: found = f.Find(w); break;
							}
						}
						catch(Exception ex) {
							if(!(ex is AuWndException)) AWarning.Write("Exception when tried to find the 'contains' object. " + ex.ToStringWithoutStack());
						}
						if(!found) { _stopProp = EProps.contains; continue; }
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

			/// <summary>
			/// Returns true if window w properties match the specified properties.
			/// </summary>
			/// <param name="w">A top-level window. If 0 or invalid, returns false.</param>
			/// <param name="cache">Can be used to make faster when multiple <b>Finder</b> variables are used with same window. The function gets window name/class/program once, and stores in <i>cache</i>; next time it gets these strings from <i>cache</i>.</param>
			/// <seealso cref="AWnd.IsMatch"/>
			public bool IsMatch(AWnd w, WFCache cache = null)
			{
				return 0 == _FindOrMatch(new _WndList(w), cache: cache);
			}
		}

		/// <summary>
		/// Finds a top-level window and returns its handle as <b>AWnd</b>.
		/// </summary>
		/// <returns>Returns <c>default(AWnd)</c> if not found. See also: <see cref="Is0"/>, <see cref="operator +(AWnd)"/>.</returns>
		/// <param name="name">
		/// Window name. Usually it is the title bar text.
		/// String format: [](xref:wildcard_expression).
		/// null means 'can be any'. "" means 'no name'.
		/// </param>
		/// <param name="cn">
		/// Window class name.
		/// String format: [](xref:wildcard_expression).
		/// null means 'can be any'. Cannot be "".
		/// </param>
		/// <param name="of">
		/// Program file name, like <c>"notepad.exe"</c>.
		/// String format: [](xref:wildcard_expression).
		/// null means 'can be any'. Cannot be "". Cannot be path.
		/// 
		/// Or <see cref="WOwner.Process"/>(process id), <see cref="WOwner.Thread"/>(thread id), <see cref="WOwner.Window"/>(owner window).
		/// See <see cref="ProcessId"/>, <see cref="AProcess.ProcessId"/>, <see cref="ThreadId"/>, <see cref="AThread.NativeId"/>, <see cref="OwnerWindow"/>.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Callback function. Called for each matching window.
		/// It can evaluate more properties of the window and return true when they match.
		/// Example: <c>also: t =&gt; !t.IsPopupWindow</c>.
		/// Called after evaluating all other parameters except <i>contains</i>.
		/// </param>
		/// <param name="contains">
		/// Defines an object that must be in the client area of the window:
		/// - Accessible object: <see cref="AAcc.Finder"/> or string like <c>"name"</c> or <c>"a 'role' name"</c> or <c>"a 'role'"</c>.
		/// - Child control: <see cref="ChildFinder"/> or string like <c>"c 'cn' name"</c> or <c>"c '' name"</c> or <c>"c 'cn'"</c>.
		/// - Image(s) or color(s): <see cref="AWinImage.Finder"/> or string <c>"image:..."</c> (uses <b>AWinImage.Find</b> with flag <see cref="WIFlags.WindowDC"/>).
		/// </param>
		/// <remarks>
		/// To create code for this function, use dialog "Find window or control". It is form <b>Au.Tools.FormAWnd</b> in Au.Tools.dll.
		/// 
		/// If there are multiple matching windows, gets the first in the Z order matching window, preferring visible windows.
		/// 
		/// On Windows 8 and later may skip Windows Store app Metro-style windows (on Windows 10 few such windows exist). It happens if this program does not have disableWindowFiltering true in its manifest and is not uiAccess; to find such windows you can use <see cref="FindFast"/>.
		/// 
		/// To find message-only windows use <see cref="FindFast"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// - <i>cn</i> is "". To match any, use null.
		/// - <i>of</i> is "" or 0 or contains character \ or /. To match any, use null.
		/// - Invalid wildcard expression (<c>"**options "</c> or regular expression).
		/// </exception>
		/// <example>
		/// Try to find Notepad window. Return if not found.
		/// <code>
		/// AWnd w = AWnd.Find("* Notepad");
		/// if(w.Is0) { AOutput.Write("not found"); return; }
		/// </code>
		/// Try to find Notepad window. Throw NotFoundException if not found.
		/// <code>
		/// AWnd w1 = +AWnd.Find("* Notepad");
		/// </code>
		/// </example>
		[MethodImpl(MethodImplOptions.NoInlining)] //inlined code makes harder to debug using disassembly
		public static AWnd Find(
			[ParamString(PSFormat.AWildex)] string name = null,
			[ParamString(PSFormat.AWildex)] string cn = null,
			[ParamString(PSFormat.AWildex)] WOwner of = default,
			WFlags flags = 0, Func<AWnd, bool> also = null, WContains contains = default)
		{
			var f = new Finder(name, cn, of, flags, also, contains);
			f.Find();
			//LastFind = f;
			return f.Result;
		}

		//rejected: probably most users will not understand what is it, and will not use. It's easy and more clear to create and use Finder instances.
		///// <summary>
		///// Gets arguments and result of this thread's last call to <see cref="Find"/> or <see cref="FindAll"/>.
		///// </summary>
		///// <remarks>
		///// <b>AWnd.Wait</b> and similar functions don't change this property. <see cref="FindOrRun"/> and some other functions of this library change this property because they call <see cref="Find"/> internally.
		///// </remarks>
		///// <example>
		///// This example is similar to what <see cref="FindOrRun"/> does.
		///// <code><![CDATA[
		///// AWnd w = AWnd.Find("*- Notepad", "Notepad");
		///// if(w.Is0) { AFile.Run("notepad.exe"); w = AWnd.WaitAny(60, true, AWnd.LastFind).wnd; }
		///// ]]></code>
		///// </example>
		//[field: ThreadStatic]
		//public static Finder LastFind { get; set; }

		/// <summary>
		/// Finds all matching windows.
		/// Returns array containing 0 or more window handles as AWnd.
		/// Parameters etc are the same as <see cref="Find"/>.
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		/// <remarks>
		/// The list is sorted to match the Z order, however hidden windows (when using <see cref="WFlags.HiddenToo"/>) are always after visible windows.
		/// </remarks>
		/// <seealso cref="GetWnd.AllWindows"/>
		/// <seealso cref="GetWnd.MainWindows"/>
		/// <seealso cref="GetWnd.ThreadWindows"/>
		public static AWnd[] FindAll(
			[ParamString(PSFormat.AWildex)] string name = null,
			[ParamString(PSFormat.AWildex)] string cn = null,
			[ParamString(PSFormat.AWildex)] WOwner of = default,
			WFlags flags = 0, Func<AWnd, bool> also = null, WContains contains = default)
		{
			var f = new Finder(name, cn, of, flags, also, contains);
			var a = f.FindAll();
			//LastFind = f;
			return a;
		}

		/// <summary>
		/// Finds a top-level window and returns its handle as <b>AWnd</b>.
		/// </summary>
		/// <returns>Returns <c>default(AWnd)</c> if not found. See also: <see cref="Is0"/>, <see cref="operator +(AWnd)"/>.</returns>
		/// <param name="name">
		/// Name.
		/// Full, case-insensitive. Wildcard etc not supported.
		/// null means 'can be any'. "" means 'no name'.
		/// </param>
		/// <param name="cn">
		/// Class name.
		/// Full, case-insensitive. Wildcard etc not supported.
		/// null means 'can be any'. Cannot be "".
		/// </param>
		/// <param name="messageOnly">Search only message-only windows.</param>
		/// <param name="wAfter">If used, starts searching from the next window in the Z order.</param>
		/// <remarks>
		/// Calls API <msdn>FindWindowEx</msdn>.
		/// Faster than <see cref="Find"/>, which uses API <msdn>EnumWindows</msdn>.
		/// Finds hidden windows too.
		/// Supports <see cref="ALastError"/>.
		/// It is not recommended to use this function in a loop to enumerate windows. It would be unreliable because window positions in the Z order can be changed while enumerating. Also then it would be slower than <b>Find</b> and <b>FindAll</b>.
		/// </remarks>
		public static AWnd FindFast(string name, string cn, bool messageOnly = false, AWnd wAfter = default)
		{
			return Api.FindWindowEx(messageOnly ? Native.HWND.MESSAGE : default, wAfter, cn, name);
		}

		/// <summary>
		/// Calls <see cref="FindFast"/>, sets this and time, but only if !IsAlive or more than 1 s elapsed since that.
		/// Returns this.
		/// </summary>
		internal AWnd FindFastCached_(ref long time, string name, string cn, bool messageOnly)
		{
			long t = ATime.WinMilliseconds;
			if(t - time > 1000 || !IsAlive) {
				time = t;
				this = FindFast(name, cn, messageOnly);
			}
			return this;
		}

		/// <summary>
		/// Finds a top-level window (calls <see cref="Find"/>). If found, activates (optionally), else calls callback function and waits for the window. The callback should open the window, for example call <see cref="AFile.Run"/>.
		/// Returns window handle as <b>AWnd</b>. Returns <c>default(AWnd)</c> if not found (if <i>runWaitS</i> is negative; else exception).
		/// </summary>
		/// <param name="name">See <see cref="Find"/>.</param>
		/// <param name="cn">See <see cref="Find"/>.</param>
		/// <param name="of">See <see cref="Find"/>.</param>
		/// <param name="flags">See <see cref="Find"/>.</param>
		/// <param name="also">See <see cref="Find"/>.</param>
		/// <param name="contains">See <see cref="Find"/>.</param>
		/// <param name="run">Callback function. See example.</param>
		/// <param name="runWaitS">How long to wait for the window after calling the callback function. Seconds. Default 60. See <see cref="Wait"/>.</param>
		/// <param name="needActiveWindow">Finally the window must be active. Default: true.</param>
		/// <exception cref="TimeoutException"><i>runWaitS</i> time has expired. Not thrown if <i>runWaitS</i> &lt;= 0.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		/// <remarks>
		/// The algorithm is:
		/// <code>
		/// var w=AWnd.Find(...);
		/// if(w.Is0) { run(); w=AWnd.Wait(runWaitS, needActiveWindow, ...); }
		/// else if(needActiveWindow) w.Activate();
		/// return w;
		/// </code>
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// AWnd w = AWnd.FindOrRun("* Notepad", run: () => AFile.Run("notepad.exe"));
		/// AOutput.Write(w);
		/// ]]></code>
		/// </example>
		public static AWnd FindOrRun(
			[ParamString(PSFormat.AWildex)] string name = null,
			[ParamString(PSFormat.AWildex)] string cn = null,
			[ParamString(PSFormat.AWildex)] WOwner of = default,
			WFlags flags = 0, Func<AWnd, bool> also = null, WContains contains = default,
			Action run = null, double runWaitS = 60.0, bool needActiveWindow = true)
		{
			AWnd w = default;
			var f = new Finder(name, cn, of, flags, also, contains);
			if(f.Find()) {
				w = f.Result;
				if(needActiveWindow) w.Activate();
			} else if(run != null) {
				run();
				w = WaitAny(runWaitS, needActiveWindow, f).wnd;
			}
			return w;
		}

		/// <summary>
		/// Compares window name and other properties like <see cref="Find"/> does.
		/// Returns true if all specified (non-null/default) properties match.
		/// </summary>
		/// <param name="name">See <see cref="Find"/>.</param>
		/// <param name="cn">See <see cref="Find"/>.</param>
		/// <param name="of">See <see cref="Find"/>.</param>
		/// <param name="flags">See <see cref="Find"/>.</param>
		/// <param name="also">See <see cref="Find"/>.</param>
		/// <param name="contains">See <see cref="Find"/>.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Find"/>.</exception>
		/// <remarks>
		/// Creates new <see cref="Finder"/> and calls <see cref="Finder.IsMatch"/>.
		/// To compare single parameter, use more lightweight code. Examples: <c>if (w.Name.Like("* Notepad"))</c>, <c>if (w.ClassNameIs("CabinetWClass"))</c>.
		/// </remarks>
		/// <seealso cref="Name"/>
		/// <seealso cref="ClassName"/>
		/// <seealso cref="ClassNameIs"/>
		/// <seealso cref="ProgramName"/>
		public bool IsMatch([ParamString(PSFormat.AWildex)] string name = null,
			[ParamString(PSFormat.AWildex)] string cn = null,
			[ParamString(PSFormat.AWildex)] WOwner of = default,
			WFlags flags = 0, Func<AWnd, bool> also = null, WContains contains = default)
		{
			var f = new Finder(name, cn, of, flags, also, contains);
			return f.IsMatch(this);
		}


		public partial struct GetWnd
		{
			/// <summary>
			/// Gets top-level windows.
			/// Returns array containing window handles as <b>AWnd</b>.
			/// </summary>
			/// <param name="onlyVisible">
			/// Need only visible windows.
			/// Note: this function does not check whether windows are cloaked, as it is rather slow. Use <see cref="IsCloaked"/> if need.
			/// </param>
			/// <param name="sortFirstVisible">
			/// Place hidden windows at the end of the array. If false, the order of array elements matches the Z order.
			/// Not used when <i>onlyVisible</i> is true.</param>
			/// <remarks>
			/// Calls API <msdn>EnumWindows</msdn>.
			/// <note>The array can be bigger than you expect, because there are many invisible windows, tooltips, etc. See also <see cref="MainWindows"/>.</note>
			/// Skips message-only windows; use <see cref="FindFast"/> if need.
			/// On Windows 8 and later may skip Windows Store app Metro-style windows (on Windows 10 few such windows exist). It happens if this program does not have disableWindowFiltering true in its manifest and is not uiAccess; to find such windows you can use <see cref="FindFast"/>.
			/// Tip: To get top-level and child windows in single array: <c>var a = AWnd.GetWnd.Root.Get.Children();</c>.
			/// </remarks>
			/// <seealso cref="Children"/>
			/// <seealso cref="FindAll"/>
			public static AWnd[] AllWindows(bool onlyVisible = false, bool sortFirstVisible = false)
			{
				return Internal_.EnumWindows(Internal_.EnumAPI.EnumWindows, onlyVisible, sortFirstVisible);
			}

			/// <summary>
			/// Gets top-level windows.
			/// </summary>
			/// <param name="a">Receives window handles as <b>AWnd</b>. If null, this function creates new List, else clears before adding items.</param>
			/// <param name="onlyVisible"></param>
			/// <param name="sortFirstVisible"></param>
			/// <remarks>
			/// Use this overload to avoid much garbage when calling frequently with the same List variable. Other overload always allocates new array. This overload in most cases reuses memory allocated for the list variable.
			/// </remarks>
			public static void AllWindows(ref List<AWnd> a, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				Internal_.EnumWindows2(Internal_.EnumAPI.EnumWindows, onlyVisible, sortFirstVisible, list: a ??= new List<AWnd>());
			}

			/// <summary>
			/// Gets top-level windows of a thread.
			/// Returns array containing 0 or more window handles as <b>AWnd</b>.
			/// </summary>
			/// <param name="threadId">
			/// Unmanaged thread id.
			/// See <see cref="AThread.NativeId"/>, <see cref="ThreadId"/>.
			/// If 0, throws exception. If other invalid value (ended thread?), returns empty list. Supports <see cref="ALastError"/>.
			/// </param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <param name="sortFirstVisible">Place all array elements of hidden windows at the end of the array, even if the hidden windows are before some visible windows in the Z order.</param>
			/// <exception cref="ArgumentException">0 threadId.</exception>
			/// <remarks>
			/// Calls API <msdn>EnumThreadWindows</msdn>.
			/// </remarks>
			/// <seealso cref="AThread.HasMessageLoop"/>
			public static AWnd[] ThreadWindows(int threadId, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				if(threadId == 0) throw new ArgumentException("0 threadId.");
				return Internal_.EnumWindows(Internal_.EnumAPI.EnumThreadWindows, onlyVisible, sortFirstVisible, threadId: threadId);
			}

			/// <summary>
			/// Gets top-level windows of a thread.
			/// </summary>
			/// <remarks>This overload can be used to avoid much garbage when caling frequently.</remarks>
			public static void ThreadWindows(ref List<AWnd> a, int threadId, bool onlyVisible = false, bool sortFirstVisible = false)
			{
				if(threadId == 0) throw new ArgumentException("0 threadId.");
				Internal_.EnumWindows2(Internal_.EnumAPI.EnumThreadWindows, onlyVisible, sortFirstVisible, threadId: threadId, list: a ??= new List<AWnd>());
			}
		}

		/// <summary>
		/// Internal static functions.
		/// </summary>
		internal static partial class Internal_
		{
			internal enum EnumAPI { EnumWindows, EnumThreadWindows, EnumChildWindows, }

			internal static AWnd[] EnumWindows(EnumAPI api,
				bool onlyVisible, bool sortFirstVisible, AWnd wParent = default, bool directChild = false, int threadId = 0)
			{
				using var a = EnumWindows2(api, onlyVisible, sortFirstVisible, wParent, directChild, threadId);
				return a.ToArray();
			}

			/// <summary>
			/// This version creates much less garbage.
			/// The caller must dispose the returned ArrayBuilder_, unless list is not null.
			/// If list is not null, adds windows there (clears at first) and returns default(ArrayBuilder_).
			/// </summary>
			internal static Util.ArrayBuilder_<AWnd> EnumWindows2(EnumAPI api,
				bool onlyVisible, bool sortFirstVisible = false, AWnd wParent = default, bool directChild = false, int threadId = 0,
				Func<AWnd, object, bool> predicate = null, object predParam = default, List<AWnd> list = null)
			{
				if(directChild && wParent == GetWnd.Root) { api = EnumAPI.EnumWindows; wParent = default; }

				Util.ArrayBuilder_<AWnd> ab = default;
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
								if(predicate((AWnd)d.a[i], predParam)) d.a[n++] = d.a[i];
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
								var w = (AWnd)d.a[i];
								if(!_EnumIsVisible(w, api, wParent)) continue;
								if(list != null) list.Add(w); else ab[j++] = w;
								d.a[i] = 0;
							}
							for(int i = 0; i < n; i++) {
								int wi = d.a[i];
								if(wi == 0) continue;
								var w = (AWnd)wi;
								if(list != null) list.Add(w); else ab[j++] = w;
							}
						} else if(list != null) {
							for(int i = 0; i < n; i++) list.Add((AWnd)d.a[i]);
						} else {
							for(int i = 0; i < n; i++) ab[i] = (AWnd)d.a[i];
						}
					}
					disposeArray = false;
					return ab;
				}
				finally {
					Util.AMemory.Free(d.a);
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
				public AWnd wParent;

				public int Proc(AWnd w)
				{
					if(onlyVisible && !_EnumIsVisible(w, api, wParent)) return 1;
					if(api == EnumAPI.EnumChildWindows) {
						if(directChild && w.ParentGWL_ != wParent) return 1;
					} else {
						if(!wParent.Is0 && w.OwnerWindow != wParent) return 1;
					}
					if(a == null) a = (int*)Util.AMemory.Alloc((_cap = onlyVisible ? 200 : 1000) * 4);
					else if(len == _cap) a = (int*)Util.AMemory.ReAlloc(a, (_cap *= 2) * 4);
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

			static bool _EnumIsVisible(AWnd w, EnumAPI api, AWnd wParent)
				=> api == EnumAPI.EnumChildWindows ? w.IsVisibleIn_(wParent) : w.IsVisible;
		}

		/// <summary>
		/// An enumerable list of AWnd for <see cref="Finder._FindOrMatch"/> and <see cref="ChildFinder._FindInList"/>.
		/// Holds Util.ArrayBuilder_ or IEnumerator or single AWnd or none.
		/// Must be disposed if it is Util.ArrayBuilder_ or IEnumerator, else disposing is optional.
		/// </summary>
		struct _WndList : IDisposable
		{
			internal enum ListType { None, ArrayBuilder, Enumerator, SingleWnd }

			ListType _t;
			int _i;
			AWnd _w;
			IEnumerator<AWnd> _en;
			Util.ArrayBuilder_<AWnd> _ab;

			internal _WndList(Util.ArrayBuilder_<AWnd> ab) : this()
			{
				_ab = ab;
				_t = ListType.ArrayBuilder;
			}

			internal _WndList(IEnumerable<AWnd> en) : this()
			{
				var e = en?.GetEnumerator();
				if(e != null) {
					_en = e;
					_t = ListType.Enumerator;
				}
			}

			internal _WndList(AWnd w) : this()
			{
				if(!w.Is0) {
					_w = w;
					_t = ListType.SingleWnd;
				}
			}

			internal ListType Type => _t;

			internal bool Next(out AWnd w)
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
	/// Flags of <see cref="AWnd.Find"/> and similar functions.
	/// </summary>
	[Flags]
	public enum WFlags
	{
		/// <summary>
		/// Can find invisible windows. See <see cref="AWnd.IsVisible"/>.
		/// Use this carefully. Always use <i>cn</i> (class name), not just <i>name</i>, to avoid finding a wrong window with the same name.
		/// </summary>
		HiddenToo = 1,

		/// <summary>
		/// Can find cloaked windows. See <see cref="AWnd.IsCloaked"/>.
		/// Cloaked are windows hidden not in the classic way, therefore <see cref="AWnd.IsVisible"/> does not detect it, but <see cref="AWnd.IsCloaked"/> detects. For example, windows on inactive Windows 10 virtual desktops, ghost windows of inactive Windows Store apps, various hidden system windows.
		/// Use this carefully. Always use <i>cn</i> (class name), not just <i>name</i>, to avoid finding a wrong window with the same name.
		/// </summary>
		CloakedToo = 2,
	}

	/// <summary>
	/// Used with <see cref="AWnd.Find"/> and similar functions to specify an owner object of the window.
	/// Can be program name (like <c>"notepad.exe"</c>), process id (<see cref="Process"/>), thread id (<see cref="Thread"/> or <see cref="ThisThread"/>) or owner window (<see cref="Window"/>).
	/// </summary>
	public struct WOwner
	{
		readonly object _o;
		WOwner(object o) => _o = o;

		/// <summary>Program name like "notepad.exe", or null. See <see cref="AWnd.ProgramName"/>.</summary>
		public static implicit operator WOwner([ParamString(PSFormat.AWildex)] string program) => new WOwner(program);

		//rejected. Cannot check whether it is default(WOwner) or 0 window handle. And less readable code.
		///// <summary>Owner window. See <see cref="AWnd.OwnerWindow"/>.</summary>
		//public static implicit operator WOwner(AWnd ownerWindow) => new WOwner(ownerWindow);

		/// <summary>Process id. See <see cref="AWnd.ProcessId"/>.</summary>
		public static WOwner Process(int processId) => new WOwner(processId);

		/// <summary>Thread id. See <see cref="AWnd.ThreadId"/>.</summary>
		public static WOwner Thread(int threadId) => new WOwner((uint)threadId);

		/// <summary>Thread id of this thread.</summary>
		public static WOwner ThisThread => new WOwner((uint)AThread.NativeId);

		/// <summary>Owner window. See <see cref="AWnd.OwnerWindow"/>.</summary>
		public static WOwner Window(AnyWnd ownerWindow) => new WOwner(ownerWindow);

		/// <summary>
		/// Gets program name or process id or thread id or owner window.
		/// Other variables will be null/0.
		/// </summary>
		/// <exception cref="ArgumentException">The value is "" or 0 or contains characters \ or /.</exception>
		public void GetValue(out AWildex program, out int pid, out int tid, out AWnd owner)
		{
			program = null; pid = 0; tid = 0; owner = default;
			switch(_o) {
			case string s:
				if(s.Length == 0) throw new ArgumentException("Program name cannot be \"\". Use null.");
				if(!s.Starts("**")) { //can be regex
					if(s.FindAny(@"\/") >= 0) throw new ArgumentException("Program name contains \\ or /.");
					if(APath.FindExtension(s) < 0 && !AWildex.HasWildcardChars(s)) AWarning.Write("Program name without .exe.");
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

		/// <summary>
		/// Returns true if nothing was assigned to this variable.
		/// </summary>
		public bool IsEmpty => _o == null;
	}

	/// <summary>
	/// The <i>contains</i> parameter of <see cref="AWnd.Find"/> and similar functions.
	/// Specifies text, image or other object that must be in the window.
	/// </summary>
	public struct WContains
	{
		readonly object _o;
		WContains(object o) => _o = o;

		///
		public static implicit operator WContains(AWnd.ChildFinder f) => new WContains(f);

		///
		public static implicit operator WContains(AAcc.Finder f) => new WContains(f);

		///
		public static implicit operator WContains(AWinImage.Finder f) => new WContains(f);

		/// <summary>
		/// Converts from string to <see cref="AWnd.ChildFinder"/>, <see cref="AAcc.Finder"/> or <see cref="AWinImage.Finder"/>.
		/// See <see cref="AWnd.Find"/>.
		/// </summary>
		/// <exception cref="Exception">Exceptions of constructor of <see cref="AWnd.ChildFinder"/>, <see cref="AAcc.Finder"/> or <see cref="AWinImage.Finder"/>.</exception>
		public static implicit operator WContains(string s) => new WContains(_ParseString(s));

		static object _ParseString(string s)
		{
			if(s.NE()) return null;
			string role = null, name = s;
			switch(s[0]) {
			case 'a': //"a'role' name" or just "name"
			case 'c': //"c'class' text"
				if(s.RegexMatch(@"^. ?'(.+?)?' ?((?s).+)?$", out var m)) {
					role = m[1].Value; name = m[2].Value;
					if(s[0] == 'c') return new AWnd.ChildFinder(name, role);
				}
				break;
			case 'i' when s.Starts("image:"):
				return new AWinImage.Finder(s, WIFlags.WindowDC);
			}
			return new AAcc.Finder(role, name, flags: AFFlags.ClientArea) { ResultGetProperty = '-' };
		}

		/// <summary>
		/// Gets object stored in this variable. Can be null, <see cref="AWnd.ChildFinder"/>, <see cref="AAcc.Finder"/> or <see cref="AWinImage.Finder"/>.
		/// </summary>
		public object Value => _o;
	}

	/// <summary>
	/// Can be used with <see cref="AWnd.Finder.IsMatch"/>.
	/// </summary>
	public class WFCache
	{
		AWnd _w;
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

		internal void Begin(AWnd w)
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
		/// Match invisible and cloaked windows too, even if the flags are not set (see <see cref="WFlags"/>).
		/// </summary>
		public bool IgnoreVisibility { get; set; }
	}
}
