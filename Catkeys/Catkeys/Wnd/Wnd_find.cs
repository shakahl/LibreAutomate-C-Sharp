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
	[Flags]
	public enum WFFlags
	{
		/// <summary>Can find hidden windows. Use this carefully, always use className, not just name, because there are many hidden tooltip windows etc that could match the name.</summary>
		HiddenToo = 1,
		/// <summary>Skip cloaked windows. These are windows hidden not in the classic way (Wnd.IsVisible does not detect it, Wnd.Cloaked detects). For example, windows on inactive Windows 10 virtual desktops, hidden Windows Store apps on Windows 8.</summary>
		SkipCloaked = 2,
		/// <summary>The 'programEtc' argument is thread id, not process id.</summary>
		ThreadId = 4,

		//Don't need this. Not very useful, 3 times slower, and not always can get full path.
		///// <summary>The 'programEtc' argument is full path. Need this flag because the function cannot auto-detect it when using wildcard, regex etc.</summary>
		//ProgramPath = 8,
	}

	//[DebuggerStepThrough]
	public partial struct Wnd
	{
		/// <summary>
		/// Contains the same fields as <see cref="Wnd.Find"/> parameters, and can be used instead of it.
		/// These codes are equivalent:
		/// <code>Wnd w = Wnd.Find(a, b, c, d, e); if(!w.Is0) Print(w);</code>
		/// <code>var p = new Wnd.FindParams(a, b, c, d, e); if(p.Find()) Print(p.Result);</code>
		/// Also can find in a custom list of windows.
		/// </summary>
		public class FindParams
		{
			readonly Wildex _name;
			readonly Wildex _className;
			readonly Wildex _program;
			readonly Func<Wnd, bool> _also;
			readonly WFFlags _flags;
			readonly uint _processId;
			readonly uint _threadId;
			readonly Wnd _owner;

			/// <summary>
			/// See <see cref="Wnd.Find">Wnd.Find</see>.
			/// </summary>
			/// <exception cref="ArgumentException">
			/// className is "". To match any, use null.
			/// programEtc is "" or 0. To match any, use null.
			/// programEtc argument type is not string/Wnd/int/uint/null.
			/// Invalid wildcard expression ("**options|" or regular expression).
			/// </exception>
			public FindParams(
				string name = null, string className = null, object programEtc = null,
				WFFlags flags = 0, Func<Wnd, bool> also = null)
			{
				_ThrowIfStringEmptyNotNull(className, nameof(className));

				if(programEtc != null) {
					uint pidTid = 0; bool isPidTid = false;
					switch(programEtc) {
					case string program:
						_ThrowIfStringEmptyNotNull(program, nameof(programEtc));
						_program = program;
						break;
					case uint i:
						pidTid = i;
						isPidTid = true;
						break;
					case int i:
						pidTid = (uint)i;
						isPidTid = true;
						break;
					case Wnd owner:
						_owner = owner;
						break;
					default:
						throw new ArgumentException("Bad type.", nameof(programEtc));
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
			/// The found window.
			/// </summary>
			public Wnd Result { get; private set; }

			/// <summary>
			/// Finds the specified window, like <see cref="Wnd.Find">Wnd.Find</see>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
			public bool Find()
			{
				var a = Misc.AllWindows(0 == (_flags & WFFlags.HiddenToo), true);
				return _FindInList(a, false) >= 0;
			}

			/// <summary>
			/// Finds the specified window in a list of windows.
			/// Returns 0-based index, or -1 if not found.
			/// The <see cref="Result"/> property will be the window.
			/// </summary>
			/// <param name="a">List of windows, for example returned by <see cref="Misc.AllWindows"/>.</param>
			public int FindInList(List<Wnd> a)
			{
				return _FindInList(a, true);
			}

			int _FindInList(List<Wnd> a, bool inList)
			{
				Result = Wnd0;
				if(a == null || a.Count == 0) return -1;

				bool mustBeVisible = inList && (_flags & WFFlags.HiddenToo) == 0;
				List<uint> pids = null; bool programNamePlanB = false; //variables for faster getting/matching program name

				for(int index = 0; index < a.Count; index++) {
					Wnd w = a[index];

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

					uint pid = 0, tid = 0;
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
								if(pids == null) pids = new List<uint>();
								pids.Add(pid); //add bad pid
								continue;
							}
						}
					}

					if(0 != (_flags & WFFlags.SkipCloaked)) {
						if(w.IsCloaked) continue;
					}

					if(_also != null && !_also(w)) continue;

					Result = w;
					return index;
				}

				return -1;
			}

			//This function can be useful, but then we cannot apply optimizations for program etc.
			//public bool Match(Wnd w)
			//{
			//	return _Match(w, 0==(_flags&WCFlags.HiddenToo));
			//}

			//bool _Match(Wnd w, bool mustBeVisible)
			//{

			//}
		}

		/// <summary>
		/// Finds window.
		/// Returns its handle as Wnd. Returns Wnd0 if not found.
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
		/// int - process id. If using flag ThreadId - native thread id (not Thread.ManagedThreadId).
		/// See <see cref="ProcessId"/>, <see cref="ThreadId"/>, <see cref="GetThreadProcessId"/>, <see cref="Process_.CurrentProcessId"/>, <see cref="Process_.CurrentThreadId"/>, <msdn>GetProcessId</msdn> or <msdn>GetThreadId</msdn>.
		/// </item>
		/// <item>uint - the same as int.</item>
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
		public static Wnd Find(string name = null, string className = null, object programEtc = null, WFFlags flags = 0, Func<Wnd, bool> also = null)
		{
			var d = new FindParams(name, className, programEtc, flags, also);
			d.Find();
			return d.Result;
		}

		/// <summary>
		/// Finds all matching windows.
		/// Returns list containing 0 or more window handles as Wnd.
		/// Everything except the return type is the same as with <see cref="Find"/>.
		/// The list is sorted to match the Z order, however hidden windows (when using WFFlags.HiddenToo) are always placed after visible windows.
		/// </summary>
		/// <exception cref="ArgumentException"/>
		public static List<Wnd> FindAll(string name = null, string className = null, object programEtc = null, WFFlags flags = 0, Func<Wnd, bool> also = null)
		{
			var a = new List<Wnd>();
			Find(name, className, programEtc, flags, e =>
			{
				if(also == null || also(e)) a.Add(e);
				return false;
			});
			return a;
		}

		/// <summary>
		/// Finds window.
		/// Returns its handle as Wnd. Returns Wnd0 if not found.
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
		/// It is not recommended to use this function in a loop to enumerate windows. It would be unreliable because window positions in the Z order can be changed while enumerating. Also then it would be slower than <b>Find</b> and <b>FindAll</b>.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public static Wnd FindFast(string name, string className, Wnd wAfter = default(Wnd))
		{
			return Api.FindWindowEx(Wnd0, wAfter, className, name);
		}

		//public static List<Wnd> FindAll(params FindParams[] a)
		//{
		//	return null;
		//}

		//public static Wnd FindAny(params FindParams[] a)
		//{
		//	return Wnd0;
		//}

		public static partial class Misc
		{
			/// <summary>
			/// Gets list of top-level windows.
			/// Returns list containing 0 or more window handles as Wnd.
			/// Calls API <msdn>EnumWindows</msdn>.
			/// <note>The list can be bigger than you expect. See also <see cref="MainWindows">MainWindows</see>.</note>
			/// </summary>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <param name="sortFirstVisible">Place all list elements of hidden windows at the end of the returned list, even if the hidden windows are before some visible windows in the Z order.</param>
			/// <remarks>
			/// By default the list elements are sorted to match the Z order.
			/// On Windows 8 and later gets only desktop windows, not Windows Store app Metro-style windows (on Windows 10 only few such windows exist), unless this process has UAC integrity level uiAccess; to get such windows you can use <see cref="FindFast">FindFast</see>.
			/// </remarks>
			public static List<Wnd> AllWindows(bool onlyVisible = false, bool sortFirstVisible = false)
			{
				List<Wnd> a = new List<Wnd>(), aHidden = null;
				if(onlyVisible) sortFirstVisible = false;

				LibAllWindows(e =>
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
			/// Calls API <msdn>EnumWindows</msdn>.
			/// </summary>
			/// <param name="f">Lambda etc callback function to call for each matching window. Can return true to stop.</param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <remarks>
			/// On Windows 8 and later gets only desktop windows, not Windows Store app Metro-style windows (on Windows 10 only few such windows exist), unless this process has uiAccess; to get such windows you can use <see cref="FindFast">FindFast</see>.
			/// </remarks>
			internal static void LibAllWindows(Func<Wnd, bool> f, bool onlyVisible = false)
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
			/// Calls API <msdn>EnumThreadWindows</msdn>.
			/// </summary>
			/// <param name="threadId">Unmanaged thread id. If 0, gets windows of current thread.</param>
			/// <param name="onlyVisible">Need only visible windows.</param>
			/// <param name="also">
			/// Lambda etc callback function to call for each matching window.
			/// It can evaluate more properties of the window and return true when they match.
			/// Example: <c>also: t =&gt; t.ClassNameIs("#32770")</c>.
			/// </param>
			public static List<Wnd> ThreadWindows(uint threadId = 0, bool onlyVisible = false, Func<Wnd, bool> also = null)
			{
				if(threadId == 0) threadId = Api.GetCurrentThreadId();
				var a = new List<Wnd>();

				Api.EnumThreadWindows(threadId, (w, param) =>
				{
					if(onlyVisible && !w.IsVisible) return 1;
					if(also != null && !also(w)) return 1;
					a.Add(w);
					return 1;
				}, 0);

				return a;

				//speed: ~40% of EnumWindows time, tested with a foreign thread with 30 windows.
			}
		}
	}
}
