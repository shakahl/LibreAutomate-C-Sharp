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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys.Types;
using static Catkeys.NoClass;

//info: in 64-bit process very slow with some 32-bit processes, eg QM2. 5 - 25 times. Half CPU is used by "COM Surrogate" process.

namespace Catkeys
{
	public unsafe partial class Acc
	{
		/// <summary>
		/// Contains accessible object properties and is used to find the object.
		/// Can be used instead of <see cref="Acc.Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>.
		/// Also can be used to find window that contains certain accessible object, like in the example.
		/// </summary>
		/// <example>
		/// Find window that contains certain accessible object, and get the object too.
		/// <code><![CDATA[
		/// var f = new Acc.Finder("PUSHBUTTON", "Apply"); //object properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => f.FindIn(t));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public class Finder
		{
			const AFFlags c_InFirefoxPage = (AFFlags)0x10000;
			const AFFlags c_InChromePage = (AFFlags)0x20000;
			const AFFlags c_InWebPage = (AFFlags)0x70000; //any browser
			const AFFlags c_Java = (AFFlags)0x80000;
			const AFFlags c_RoleHasPrefix = (AFFlags)0x100000;

			string _role;
			_PathPart[] _path;
			Wildex _name;
			object[] _props;
			AFFlags _flags;
			Func<AFAcc, bool> _also;
			int _skip, _skipCounter;
			List<_Acc.NavdirN> _navig;
			object _classOrId;
			AFAcc _alsoArgs;

			//rejected: nobody would use it. And it has no sense because Finder is not garbage-free anyway.
			///// <summary>
			///// Your callback function (<b>also</b>) will receive this value as <see cref="AFAcc.Param"/>.
			///// </summary>
			//public object Param { get; set; }

			/// <summary>
			/// Maximal level in the object tree at which the object can be found.
			/// The default value is 1000. It prevents stack overflow when an object has a bug and returns itself when queried a child.
			/// A smaller level can make faster, because objects at higher levels are not searched. An alternative is to use path.
			/// </summary>
			public int MaxLevel { get; set; } = 1000;
			//tested: stack overflow after this number of recursions:
			//	64-bit: debug 3161, release 6325
			//	32-bit: debug 5469, release 9524
			//Noticed this bug in the past with one web page in IE 11.

			/// <summary>
			/// Sets array of roles that are not in path to the object you want to find.
			/// When searching, objects that have one of these roles will be skipped together with descendants. It can make faster.
			/// </summary>
			public string[] SkipRoles { get; set; }

			//Acc FUTURE: an option to get path of found object.

			/// <summary>
			/// See <see cref="Acc.Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>.
			/// </summary>
			/// <param name="role"></param>
			/// <param name="name"></param>
			/// <param name="flags"></param>
			/// <param name="also"></param>
			/// <param name="skip"></param>
			/// <param name="navig"></param>
			/// <exception cref="ArgumentException">Invalid role, name or navig.</exception>
			public Finder(string role = null, string name = null, AFFlags flags = 0, Func<AFAcc, bool> also = null, int skip = 0, string navig = null)
			{
				_flags = flags;
				_ParseRole(role);
				_ParseName(name);
				_also = also;
				_skip = skip;
				_navig = Empty(navig) ? null : _Acc.NavdirN.Parse(navig); //ArgumentException

				if(_path != null || _flags.HasAny_(AFFlags.DirectChild | c_InWebPage | c_Java)) _flags |= AFFlags.MenuToo;
			}

			void _ParseRole(string role)
			{
				if(role == null) return;
				if(role.Length == 0) throw new ArgumentException("role cannot be \"\". To match any, use null.");

				//is prefix?
				int colon = role.IndexOf(':');
				if(colon > 0) {
					int prefix = role.StartsWith_(false, "class=", "id=", "web:", "firefox:", "chrome:", "java:");
					if(prefix > 0) {
						_flags |= c_RoleHasPrefix;
						switch(prefix) {
						case 1:
							if(colon == 6) goto ge;
							_classOrId = role.Substring(6, colon - 6);
							break;
						case 2:
							_classOrId = role.ToInt32_(3, out var end);
							if(end != colon) goto ge;
							break;
						case 3: _flags |= c_InWebPage; break;
						case 4: _flags |= c_InFirefoxPage; break;
						case 5: _flags |= c_InChromePage; break;
						case 6: _flags |= c_Java; break;
						}
						if(++colon == role.Length) return;
						role = role.Substring(colon);
					}
				}

				//is path?
				int i = role.IndexOf('/');
				if(i >= 0) {
					fixed (char* p = role) {
						//calculate n parts
						int nParts = 1, n = role.Length;
						for(; i < n; i++) if(p[i] == '/') nParts++;
						var a = new _PathPart[nParts];
						_path = a;
						//parse string
						int level = 0, iStartPart = 0;
						for(i = 0; i <= n; i++) {
							var c = p[i];
							if(c == '/' || c == '[' || c == '\0') {
								if(i > iStartPart) //else can be any role at this level
									a[level].role = Util.StringCache.LibAdd(p + iStartPart, i - iStartPart);
								if(c == '[') {
									a[level].startIndex = role.ToInt32_(i + 1, out i);
									if(i == 0) goto ge;
									if(p[i] == '!') { i++; a[level].exactIndex = true; }
									if(p[i++] != ']') goto ge;
									if(i < n && p[i] != '/') goto ge;
								}
								level++;
								iStartPart = i + 1;
							}
						}
					}

					//Acc FUTURE: "PART/PART/.../PART"
				} else {
					_role = role;
				}
				return;
				ge:
				throw new ArgumentException("Invalid role.");
			}

			void _ParseName(string name)
			{
				if(name == null) return;
				if(name.IndexOf('\0') >= 0) {
					var a = new List<object>();
					foreach(var s in name.Segments_("\0", SegFlags.NoEmpty)) {
						s.TrimStart();
						int i = s.IndexOf('='); if(i < 0) throw new ArgumentException("Missing =.", "name");
						string na = s.Substring(0, i, cached: true), va = s.Substring(i + 1);
						if(!na.StartsWith_("a:")) {
							i = na.Equals_(false, "name", "value", "description");
							if(i == 0) throw new ArgumentException("Unknown property name. Expected \"name\", \"value\", \"description\" or prefix \"a:\".", "name");
							if(i == 1) {
								_name = va;
								continue;
							}
						}
						//PrintList(s, na, va);
						a.Add(na); a.Add(new Wildex(va));
					}
					if(a.Count > 0) _props = a.ToArray();
				} else {
					_name = name;
				}
			}

			/// <summary>
			/// The found object (after calling FindIn).
			/// </summary>
			public Acc Result { get; private set; }

			/// <summary>
			/// Resets all result/temporary fields/properties to their default values.
			/// Optionally disposes Result.
			/// </summary>
			void _Reset(bool disposeResult)
			{
				if(Result != null) {
					if(disposeResult) Result.Dispose();
					Result = null;
				}
				_skipCounter = 0;
			}

			/// <summary>
			/// Resets like _Reset, but returns Result._iacc instead of releasing it.
			/// If Result is null, returns default.
			/// </summary>
			IAccessible _ResetAndDetachResultIAccessible()
			{
				IAccessible r;
				if(Result == null) r = default;
				else {
					r = Result._iacc;
					Result._Dispose(doNotRelease: true);
					Result = null;
				}
				_Reset(disposeResult: false);
				return r;
			}

			const string c_IES = "Internet Explorer_Server";

			/// <summary>
			/// Finds the specified accessible object in window w.
			/// Returns true if found. The <see cref="Result"/> property will be the found object (null if not found).
			/// Used by <see cref="Acc.Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>. More info there.
			/// </summary>
			/// <param name="w">Window or control that contains the object.</param>
			/// <param name="controls">Allows to specify all possible properties of the child window (of w) where the accessible object is. If used (not null), this functions searches for the accessible object in all matching controls until found.</param>
			/// <exception cref="WndException">Invalid window.</exception>
			public bool FindIn(Wnd w, Wnd.ChildFinder controls = null)
			{
				w.ThrowIfInvalid();
				try {
					if(controls == null) return _FindIn(w);

					var a = controls.FindAllIn(w);
					foreach(var c in a) {
						if(_FindIn(c)) {
							controls.Result = c;
							return true;
						}
					}
				}
				catch(CatException ex) when(!w.IsAlive) { Debug_.Print(ex.Message); } //don't throw WndException/CatException if the window or a control is destroyed while searching, but throw CatException if eg access denied
				return false;
				//never mind: can be optimized to avoid garbage of the Wnd[] and its IEnumerator. Maybe in the future.
			}

			bool _FindIn(Wnd w)
			{
				_Reset(disposeResult: false);
				bool controlsFound = false;

				var web = _flags & c_InWebPage;
				if(web != 0) {
					if(web == c_InWebPage) {
						//We cannot detect IE by window class name. It can be any, because IE-based web browser controls can be used anywhere.
						//	To detect it, we look for an "Internet Explorer_Server" control.
						//	We search for the accessible object in all found controls.
						//	If it is Firefox or Chrome, this makes slightly slower, maybe 1%.
						if(_FindInChildControls(c_IES, AccOBJID.CLIENT)) return true; //info: the hierarchy is WINDOW/CLIENT/PANE, therefore PANE will be at level 0
						if(controlsFound) return false; //IE, but object not found
					}
					using(var iacc = _FindDocument(w)) {
						if(iacc.Is0) return false;
						if(_Match(iacc, 0, out bool skipChildren, 0)) return true;
						if(skipChildren) return false;
						if(_alsoArgs?.stop ?? false) return false;
						return _FindIn(iacc, 1);
					}
				} else if(_flags.Has_(c_Java)) {
					using(var aw = _Java.AccFromWindow(w)) {
						if(aw == null) throw new CatException("*get Java objects of this window");
						return _FindIn(aw._iacc, 0);
					}
				} else if(_classOrId != null) {
					return _FindInChildControls(_classOrId);
				} else {
					using(var iacc = _FromWindow(w)) { //throws if failed
						return _FindIn(iacc, 0);
					}
				}

				bool _FindInChildControls(object classOrId, AccOBJID objid = AccOBJID.WINDOW)
				{
					using(var ab = Wnd.Lib.EnumWindows2(Wnd.Lib.EnumWindowsAPI.EnumChildWindows, !_flags.Has_(AFFlags.HiddenToo), true, w,
						predicate: (c, p) => (p is string cn) ? c.ClassNameIs(cn) : (c.ControlId == (int)p),
						param: classOrId)) {
						for(int i = 0; i < ab.Count; i++) {
							controlsFound = true;
							using(var iacc = _FromWindow(ab[i], objid)) { //throws if failed
								if(_FindIn(iacc, 0)) return true;
							}
						}
						return false;
					}
				}
			}

			/// <summary>
			/// Finds the specified accessible object in an accessible object, like <see cref="Acc.Find(string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the found object (null if not found).
			/// </summary>
			/// <param name="parent">Direct or indirect parent accessible object.</param>
			/// <exception cref="ArgumentException">role with prefix ("web:" etc).</exception>
			public bool FindIn(Acc parent)
			{
				if(_flags.Has_(c_RoleHasPrefix)) throw new ArgumentException("role cannot have a prefix when searching in Acc.");
				_Reset(disposeResult: false);
				if(parent.Elem != 0) return false;
				return _FindIn(parent._iacc, 0);
			}

			bool _FindIn(IAccessible iacc, int level)
			{
				Debug.Assert(Result == null);
				if(level == 0) _skipCounter = 0;

				int startIndex = 0; bool exactIndex = false;
				if(_path != null) { startIndex = _path[level].startIndex; exactIndex = _path[level].exactIndex; }

				using(var c = new _Children(iacc, startIndex, exactIndex: exactIndex, reverse: _flags.Has_(AFFlags.Reverse))) {
					while(c.GetNext(out var a)) {
						try {
							if(_Match(a.a, a.elem, out bool skipChildren, level)) return true;
							if(_alsoArgs?.stop ?? false) break;
							if(skipChildren) continue;
							if(level >= MaxLevel) continue;
							if(_FindIn(a.a, level + 1)) return true;
							if(_alsoArgs?.stop ?? false) break;
						}
						finally {
							c.FinallyInLoop(a);
							_alsoArgs?.LibResetIacc(); //not necessary, but let it throw objectdisposedexception if the callback assigned AFAcc to Acc and will try to use it (must use Args.ToAcc)
						}
					}
					return false;
				}

				//Acc FUTURE: maybe add flag to search the tree differently: at first compare all objects in level 0, then all in level 1 and so on. Maybe default.
			}

			bool _Match(IAccessible a, int e, out bool skipChildren, int level)
			{
				//Print(a.ToString(e, level));

				skipChildren = e != 0 || _flags.Has_(AFFlags.DirectChild);
				bool filtering = !skipChildren && (!_flags.Has_(AFFlags.MenuToo) || _flags.HasAny_(AFFlags.SkipLists | AFFlags.SkipWeb));
				bool hiddenToo = _flags.Has_(AFFlags.HiddenToo);
				string roleNeeded = _path != null ? _path[level].role : _role;
				//bool haveState = false; AccSTATE state = 0;

				AccROLE roleE = 0; string roleS = null; bool haveRole = false;
				if(roleNeeded != null || filtering || SkipRoles != null) { //else don't need role
					if(0 != a.GetRoleString(e, out roleS, ref roleE)) goto gSkipChildren; //invalid object; assume that all descendants are invalid too
					haveRole = true;

					var skipr = SkipRoles;
					if(skipr != null) {
						for(int i = 0; i < skipr.Length; i++) {
							if(roleS == skipr[i]) goto gSkipChildren;
						}
					}

					if(filtering) skipChildren = _IsRoleToSkipDescendants(roleE, level, roleNeeded, a);
				}

				if(roleNeeded != null && !roleNeeded.Equals_(roleS)) {
					if(_path != null) goto gSkipChildren;
					goto gSkipChildrenIfInvisible;
				}
				if(_path != null) {
					if(level < _path.Length - 1) goto gSkipChildrenIfInvisible;
					skipChildren = true;
				}

				if(_name != null) {
					a.get_accName(e, out string s, roleE);
					if(!_name.Match(s)) return false;
				}

				if(!hiddenToo && _IsInvisible()) {
					//skip some invisible ancestors (see comment below, under gSkipChildrenIfInvisible)
					if(!skipChildren) {
						if(!haveRole && 0 != a.GetRole(e, out roleE)) goto gSkipChildren;
						if(_IsRoleToSkipIfInvisible(roleE)) goto gSkipChildren;
					}
					return false;
					//note: gettig state often is slower than getting role, name and other properties.
				}

				if(_also != null || _props != null) {
					if(_alsoArgs == null) _alsoArgs = new AFAcc();
					_alsoArgs.LibSet(a, e, roleE, level);

					if(_props != null) {
						if(!_alsoArgs.Match(_props)) return false;
					}

					if(_also != null) {
						if(!_also(_alsoArgs)) {
							if(_alsoArgs.skipChildren) goto gSkipChildren;
							return false;
						}
					}
				}

				if(_skipCounter < _skip) {
					_skipCounter++;
					return false;
				}

				a.AddRef(); //caller will release

				if(_navig != null) {
					var an = new _Acc(a, e);
					if(0 != an.Navigate(_navig)) {
						_skipCounter = 0;
						return false;
					}
					a = an.a; e = an.elem; roleE = 0;
				}

				this.Result = new Acc(a, e, roleE);
				return true;

				gSkipChildren:
				skipChildren = true;
				return false;

				gSkipChildrenIfInvisible:
				//skip invisible ancestors that usually have large number of descendants (eg DOCUMENT, WINDOW)
				//	or are useless (MENUBAR/TITLEBAR/SCROLLBAR that are children of every standard WINDOW)
				if(!skipChildren && !hiddenToo) {
					if(_IsRoleToSkipIfInvisible(roleE) && _IsInvisible()) goto gSkipChildren;
				}
				return false;

				bool _IsInvisible()
				{
					//if(!haveState) {
					//	haveState = true;
					//	if(0 != a.get_accState(e, out state)) state = 0;
					//}
					//return state.Has_(AccSTATE.INVISIBLE);

					return 0 == a.get_accState(e, out var state) && state.Has_(AccSTATE.INVISIBLE);
				}
			}

			bool _IsRoleToSkipDescendants(AccROLE roleE, int level, string roleNeeded, IAccessible a)
			{
				switch(roleE) {
				case AccROLE.MENUITEM:
					if(!_flags.Has_(AFFlags.MenuToo))
						switch(roleNeeded) { case "MENUITEM": case "MENUPOPUP": break; default: return true; }
					break;
				case AccROLE.OUTLINE:
				case AccROLE.LIST:
				case AccROLE.TITLEBAR: //note: some apps use custom TITLEBAR full of everything, for example VS 2017
				case AccROLE.SCROLLBAR:
					if(_flags.Has_(AFFlags.SkipLists)) return true;
					break;
				case AccROLE.DOCUMENT:
					if(_flags.Has_(AFFlags.SkipWeb)) return true;
					break;
				case AccROLE.PANE:
					if(_flags.Has_(AFFlags.SkipWeb)) {
						if(0 == a.GetWnd(out var w) && w.ClassNameIs(c_IES)) return true;
					}
					break;
				}
				return false;
			}

			static bool _IsRoleToSkipIfInvisible(AccROLE roleE)
			{
				switch(roleE) {
				case AccROLE.MENUBAR: //nonclient
				case AccROLE.TITLEBAR: //nonclient
				case AccROLE.SCROLLBAR: //nonclient
				case AccROLE.WINDOW: //child control. Getting state is fast.
				case AccROLE.DOCUMENT: //web page in Firefox, Chrome
				case AccROLE.PANE: //web page in IE, Pane of WinForms, etc
				case AccROLE.PROPERTYPAGE: //page in multi-tab dialog or window
				case AccROLE.GROUPING: //eg some objects in Firefox
				case AccROLE.ALERT: //eg web browser message box. In Firefox can be some invisible alerts.
					return true;
				}
				return false;
				//note: don't add CLIENT. It is often used as default role. Although in some windows it can make faster.

				//problem: some frameworks mark visible offscreen objects as invisible. Eg IE, WPF. Not Firefox, Chrome.
				//	Can be evan parent marked as invisible even if child not. Then we'll not find child if parent's role is one of above.
				//	Never mind. This probably will be rare with these roles. Then user can add flag HiddenToo.
				//	But code tools should somehow detect it and add the flag.
			}

			IAccessible _FindDocument(Wnd w)
			{
				IAccessible ar = default, ap = default;
				Finder f = null;

				int browser;
				switch(_flags & c_InWebPage) {
				case c_InFirefoxPage: browser = 1; break;
				case c_InChromePage: browser = 2; break;
				default: browser = w.ClassNameIs("Mozilla*", "Chrome*"); break;
				}

				using(ap = _FromWindow(w, AccOBJID.CLIENT)) { //throws if failed
					if(browser == 1) { //Firefox

						//To get DOCUMENT, use Navigate(0x1009). It is documented and tested on FF>=2.
						//Sometimes also need to wait while state is BUSY.
						for(int i = 0; i < 100; i++, Thread.Sleep(10)) {
							int hr = ap.accNavigate((AccNAVDIR)0x1009, 0, out var t);
							if(hr == 0) {
								ar = t.a;
								if(_flags.Has_(AFFlags.WebBusy)) break;
								if(0 != ar.get_accState(0, out var state) || !state.Has_(AccSTATE.BUSY)) break;
								ar.Dispose(); continue;
							} else {
							//if(hr == Api.E_NOTIMPL) {
								Debug_.Print("Firefox 56 bug still not fixed");
								break;
								//Acc FUTURE: make more intelligent.
								//	I fails always or often, don't wait.
								//	If fails never or ocassionally, wait anyway.
								//	Not only when E_NOTIMPL.
								//	The same with Chrome.
							}
							//if(0 != ap.GetRole(0, out var roleE) || roleE != AccROLE.APPLICATION) break; //Detect is it really a Mozilla window. If role is APPLICATION, assume it is really a Mozilla window, because with most windows it is CLIENT or DIALOG.
						}

						//In older Firefox, Navigate() used to fail when calling first time after starting Firefox.
						//Also ocassionally fails in some pages, even if page is loaded, maybe when executing scripts.
						//Therefore we wait a while until it succeeds and the page is not busy (if busy, could fail later).
						//	Usually need 1-4 loops.
						//	But some pages always have BUSY state. Then waits ~1.5 s. Use AFFlags.WebBusy. The 'Find Acc' dialog detects busy FF document and adds this flag. Or uncheck 'in web page'.

					} else if(browser == 2) { //Chrome

						//Acc CONSIDER: auto-enable Chrome AO even if without prefix "web:" etc.
						//	For example, when DOCUMENT has 0 children, look what is it's class name. If "Chrome*", try to enable its AO, or show info.
						//	This also could be used for apps like OpenOffice that enable AO only if SPI_GETSCREENREADER is true. Set SPI_SETSCREENREADER only when main window's CLIENT has 0 children.

						return LibGetChromeDOCUMENT(w, ap);
					}

					//Unknown web browser? Or Firefox accNavigate(0x1009) stopped working?
					if(ar.Is0) {
						Debug_.Print("unknown browser");
						f = new Finder("DOCUMENT", null, AFFlags.SkipLists | AFFlags.Reverse); //info: Reverse is faster with Firefox
						f.SkipRoles = s_docNoRoles;
						f._FindIn(ap, 0);
						ar = f._ResetAndDetachResultIAccessible();
					}
				}

				return ar;
			}

			static string[] s_docNoRoles = new string[] {
				"MENUPOPUP", //many in Firefox
				"TOOLBAR", //in Firefox and Chrome
				"OUTLINE", //Firefox bookmarks
				"DIAGRAM", //in Firefox
				"iframe", //Firefox developer tools. It even can contain DOCUMENT.
				"ALERT", //message box
				"STATUSBAR",
				"LIST",
				"TABLE",
				"PAGETABLIST", //in Chrome
				"PUSHBUTTON", //in Chrome
			};

			/// <summary>
			/// Finds Chrome DOCUMENT and enables its accessible objects if need.
			/// </summary>
			/// <param name="w"></param>
			/// <param name="aCLENT">w CLIENT</param>
			internal static IAccessible LibGetChromeDOCUMENT(Wnd w, IAccessible aCLENT)
			{
				IAccessible ar = default, ap = aCLENT;
				Finder f = new Finder("DOCUMENT", null, AFFlags.SkipLists); //tested: with Reverse normally similar speed; faster when there is Developer Tools but slower when there is Search box.
				f.SkipRoles = s_docNoRoles;
				if(!_FindChromeDOCUMENT()) return default;

				//Does it have children? If not, probably AO in Chrome web pages is disabled.
				if(0 == ar.get_accChildCount(out int nChildren) && nChildren == 0) {
					_EnableChromeAO(ap); //for new Chrome versions
					_EnableChromeAO(ar); //for old Chrome versions. Does not harm. Same speed.

					//Wait until enabling AO finished.
					for(int i = 10; i < 150; Thread.Sleep(i++)) //max 11 s. The required time depends on page.
					{
						if(ar.Is0 || 0 != ar.get_accState(0, out var state)) //fails when enabling finished. Then need to find new DOCUMENT.
						{
							if(!ar.Is0) { ar.Release(); ar = default; }
							if(!w.IsAlive) break;
							//ar (DOCUMENT) becomes invalid. Need to find DOCUMENT again.
							//	To make faster, we could get direct parent before, but it makes even slower.
							_FindChromeDOCUMENT();
							continue;
						}
						if(!state.Has_(AccSTATE.BUSY)) break; //busy state until enabled AO
					}
				}
				return ar;

				bool _FindChromeDOCUMENT()
				{
					if(!f._FindIn(ap, 0)) {
						//var ff = new Finder(flags: f._flags, also: o => { Print(o); return false; }) { SkipRoles = f.SkipRoles }; ff._FindIn(ap, 0);
						return false;
					}
					//maybe it is Developer Tools
					var val = f.Result.Value;
					if(val != null && val.StartsWith_("chrome-devtools:")) {
						f._Reset(disposeResult: true);
						f._flags ^= AFFlags.Reverse;
						if(!f._FindIn(ap, 0)) return false;
					}
					ar = f._ResetAndDetachResultIAccessible();
					return true;
				}

				void _EnableChromeAO(IAccessible a)
				{
					//Query service IAccessible2. Then Chrome enables its web page AO.
					if(Util.Marshal_.QueryService(a, out IntPtr ia2, ref Api.IID_IAccessible, ref Api.IID_IAccessible2)) Marshal.Release(ia2);
					//This is undocumented, discovered from Chrome source code. There are no other ways to auto-enable on demand.
					//note: QS in most cases returns error, but Chrome enables AO anyway. We'll wait while STATE_SYSTEM_BUSY.
				}
			}

			/// <summary>
			/// Finds the specified accessible object in window w.
			/// The same as <see cref="FindIn(Wnd, Wnd.ChildFinder)"/>, but waits until the object is found or the given time expires.
			/// Returns true if found. If secondsTimeout is negative, on timeout returns false (else exception).
			/// </summary>
			/// <param name="secondsTimeout">
			/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
			/// </param>
			/// <param name="w">Window or control that contains the object.</param>
			/// <param name="controls">Allows to specify all possible properties of the child window (of w) where the accessible object is. If used (not null), this functions searches for the accessible object in all matching controls until found.</param>
			/// <exception cref="TimeoutException">secondsTimeout time has expired (if &gt; 0).</exception>
			/// <exception cref="WndException">Invalid window.</exception>
			public bool WaitIn(double secondsTimeout, Wnd w, Wnd.ChildFinder controls = null)
			{
				return Catkeys.WaitFor.Condition(secondsTimeout, o => FindIn(w, controls));
			}

			/// <summary>
			/// Finds the specified accessible object in an accessible object.
			/// The same as <see cref="FindIn(Acc)"/>, but waits until the object is found or the given time expires.
			/// Returns true if found. If secondsTimeout is negative, on timeout returns false (else exception).
			/// </summary>
			/// <param name="secondsTimeout">
			/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns false.
			/// </param>
			/// <param name="parent">Direct or indirect parent accessible object.</param>
			/// <exception cref="ArgumentException">role with prefix ("web:" etc).</exception>
			public bool WaitIn(double secondsTimeout, Acc parent)
			{
				return Catkeys.WaitFor.Condition(secondsTimeout, o => FindIn(parent));
			}

			//Acc FUTURE: FindAll
		}

		/// <summary>
		/// Finds accessible object in window.
		/// Returns null if not found. You can use <see cref="ExtensionMethods.OrThrow{T}"/> (see example).
		/// </summary>
		/// <param name="w">Window or control that contains the object.</param>
		/// <param name="role">
		/// Accessible object role, like "LINK". See <see cref="RoleString"/>, <see cref="AccROLE"/>.
		/// Or path consisting of roles, like "ROLE/ROLE/ROLE". More info in Remarks.
		/// Case-sensitive, not wildcard. Use null to match any role. Cannot be "".
		/// </param>
		/// <param name="name">
		/// Accessible object name (<see cref="Name"/>).
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'can be any'. "" means 'must not have name'.
		/// Or can be a '\0'-separated list of properties and/or HTML attributes, like "name=xxx\0 a:href=yyy" or "\0 a:href=yyy". The list is passed to <see cref="Match"/>. The values are wildcard expressions.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Lambda etc callback function to call for each matching accessible object. Let it return true if this is the object you want.
		/// The parameter is of type <see cref="AFAcc"/> which has all Acc properties and several own.
		/// Example, the object must not be disabled: <c>also: o => !o.IsDisabled</c>
		/// Example, the object must have certain Value: <c>also: o => o.Value == "http://www.example.com/x.php"</c>
		/// Example, the object's Value must match a wildcard: <c>also: o => o.Value.Like_("start*end")</c>
		/// Example, the object must be at certain level in the object tree: <c>also: o => o.Level == 2</c>
		/// Example, the object must contain x y: <c>o => o.GetRect(out var r, o.WndWindow) &amp;&amp; r.Contains(266, 33)</c>
		/// </param>
		/// <param name="skip">
		/// 0-based index of matching object.
		/// For example, if 1, the function skips the first matching object and returns the second.
		/// </param>
		/// <param name="navig">
		/// A navigation string that can be used to get another object related to the found object, like with <see cref="Navigate(string, bool)"/>.
		/// This is applied when all other parameters match (including <paramref name="also"/> and <paramref name="skip"/>).
		/// </param>
		/// <exception cref="WndException">Invalid w.</exception>
		/// <exception cref="ArgumentException">
		/// role is "" or invalid.
		/// name is invalid wildcard expression ("**options|" or regular expression).
		/// Parts of '\0'-separated name string have invalid format or unknown property names.
		/// navig is invalid.
		/// </exception>
		/// <remarks>
		/// Standard role names are as in the <see cref="AccROLE"/> enum. Also can be used custom roles, such as "div" in Firefox.
		/// When searching in window, role or path can have one of these prefixes:
		/// <list type="bullet">
		/// <item>
		/// "class=X:" - search only in visible child controls whose class name is X (see <see cref="Wnd.ClassName"/>).
		/// Case-insensitive wildcard, see <see cref="String_.Like_(string, string, bool)"/>.
		/// </item>
		/// <item>
		/// "id=X:" - search only in visible child controls whose id is X (see <see cref="Wnd.ControlId"/>).
		/// </item>
		/// <item>
		/// "web:" - search only in the visible web page, not in whole window. Examples: "web:LINK", "web:DOCUMENT/LIST/LISTITEM/LINK". It usually makes faster.
		/// Supports Firefox, Chrome, Internet Explorer (IE) and apps based on their code. With other windows, searches in the first found visible DOCUMENT object.
		/// <note type="note">Chrome web page accessible objects normally are disabled (DOCUMENT does not have children). Use prefix "web:" or "chrome:" to enable them.</note>
		/// </item>
		/// <item>"firefox:" - search only in the visible web page of Firefox or Firefox-based web browser. If w window class name starts with "Mozilla", can be used "web:" instead.</item>
		/// <item>"chrome:" - search only in the visible web page of Chrome or Chrome-based web browser. If w window class name starts with "Chrome", can be used "web:" instead.</item>
		/// <item>"java:" - find Java accessible object.</item>
		/// </list>
		/// This function walks the tree of accessible objects of the window or control, until it finds a matching object. To make faster, you can use path to the object. Examples:
		/// <list type="bullet">
		/// <item>"web:DOCUMENT/div/LIST/LISTITEM/LINK" - find LINK using its full path in web page.</item>
		/// <item>"web:/div/LIST//LINK" - the empty parts mean 'any role'. For example don't need to specify DOCUMENT because in web pages the first part is always DOCUMENT (Firefox, Chrome) or PANE (IE).</item>
		/// <item>"web:/div/LIST[4]/LISTITEM[-1]/LINK" - the 4 is 1-based index of div child from which to start searching (4-th, then 3-th, 5-th and so on). It can make faster. Negative means 'index from end', for example use -1 to search in reverse order. Flag Reverse is not applied to path parts with indexes. If index is invalid, will use the nearest valid index.</item>
		/// <item>"web:/div/LIST[4!]/LISTITEM[-1!]/LINK" - like the above, but the LIST must be exactly 4-th child (don't search 3-th, 5-th etc) and the LISTITEM must be the last child. This can be useful when waiting (uses less CPU), however useless if object indices in the window or web page change often.</item>
		/// <item>"web://[4]/[-1!]/[2]" - index without role.</item>
		/// <item>"CLIENT/WINDOW/OUTLINE/OUTLINEITEM[-1]" - path in window or control. The first path part is a direct child object of the WINDOW object of the window/control. The WINDOW object itself is not included in the search; if you need it, instead use <see cref="FromWindow"/>.</item>
		/// </list>
		/// Tip: Instead of literal role string you can use nameof. Example: <c>nameof(AccROLE.CHECKBUTTON)</c>.
		/// 
		/// This function uses a <see cref="Finder"/>. You can use it directly, it has some more options (see example).
		/// 
		/// Finding accessible objects can be much slower if this process is 64-bit and that process is 32-bit or vice versa.
		/// Finding accessible objects of some applications works well only if this process is 32-bit or only if 64-bit. For example LibreOffice.
		/// </remarks>
		/// <example>
		/// Try to find link "Example" in web page. Return if not found.
		/// <code><![CDATA[
		/// var a = Acc.Find(w, "web:LINK", "Example");
		/// if(a == null) { Print("not found"); return; }
		/// ]]></code>
		/// Try to find link "Example" in web page. Throw NotFoundException if not found.
		/// <code><![CDATA[
		/// var a = Acc.Find(w, "web:LINK", "Example").OrThrow();
		/// ]]></code>
		/// Use a Finder.
		/// <code><![CDATA[
		/// var f = new Acc.Finder("PUSHBUTTON", "Example");
		/// f.SkipRoles = new string[] { "OUTLINE", "PANE" };
		/// if(!f.FindIn(w)) { Print("not found"); return; }
		/// Acc a = f.Result;
		/// ]]></code>
		/// </example>
		/// <seealso cref="Finder"/>
		public static Acc Find(Wnd w, string role = null, string name = null, AFFlags flags = 0, Func<AFAcc, bool> also = null, int skip = 0, string navig = null)
		{
			var f = new Finder(role, name, flags, also, skip, navig);
			f.FindIn(w);
			return f.Result;
		}

		/// <summary>
		/// Finds a descendant accessible object of this accessible object.
		/// Parameters and other info are as with <see cref="Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>.
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="Finder(string, string, AFFlags, Func{AFAcc, bool}, int, string)"/> and <see cref="Finder.FindIn(Acc)"/>.</exception>
		public Acc Find(string role = null, string name = null, AFFlags flags = 0, Func<AFAcc, bool> also = null, int skip = 0, string navig = null)
		{
			var f = new Finder(role, name, flags, also, skip, navig);
			f.FindIn(this);
			return f.Result;
		}

		//rejected: searching method: scan by calling accHitTest at every n-th x/y. Tested, slow.

		/// <summary>
		/// Finds an accessible object in window.
		/// The same as <see cref="Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>, but waits until the object is found or the given time expires.
		/// Returns the object. If secondsTimeout is negative, on timeout returns null (else exception).
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns null.
		/// </param>
		/// <param name="w"></param>
		/// <param name="role"></param>
		/// <param name="name"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="skip"></param>
		/// <param name="navig"></param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired (if &gt; 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>.</exception>
		public static Acc WaitFor(double secondsTimeout, Wnd w, string role = null, string name = null, AFFlags flags = 0, Func<AFAcc, bool> also = null, int skip = 0, string navig = null)
		{
			var f = new Finder(role, name, flags, also, skip, navig);
			Catkeys.WaitFor.Condition(secondsTimeout, o => f.FindIn(w));
			return f.Result;
		}

		/// <summary>
		/// Finds a descendant accessible object of this accessible object.
		/// The same as <see cref="Find(string, string, AFFlags, Func{AFAcc, bool}, int, string)"/>, but waits until the object is found or the given time expires.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after secondsTimeout time throws <b>TimeoutException</b>. If &lt;0, after -secondsTimeout time returns null.
		/// </param>
		/// <param name="role"></param>
		/// <param name="name"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="skip"></param>
		/// <param name="navig"></param>
		/// <exception cref="TimeoutException">secondsTimeout time has expired (if &gt; 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Finder(string, string, AFFlags, Func{AFAcc, bool}, int, string)"/> and <see cref="Finder.FindIn(Acc)"/>.</exception>
		public Acc WaitFor(double secondsTimeout, string role = null, string name = null, AFFlags flags = 0, Func<AFAcc, bool> also = null, int skip = 0, string navig = null)
		{
			var f = new Finder(role, name, flags, also, skip, navig);
			Catkeys.WaitFor.Condition(secondsTimeout, o => f.FindIn(this));
			return f.Result;
		}

		/// <summary>
		/// Gets the number of direct child objects.
		/// Uses <msdn>IAccessible.get_accChildCount</msdn>.
		/// </summary>
		/// <remarks>
		/// Some objects (maybe 0.1%) return an incorrect number. A more reliable alternative is <see cref="Children(bool)"/>, but usually it is much slower.
		/// </remarks>
		public int ChildCount
		{
			get
			{
				if(_Disposed) throw new ObjectDisposedException(nameof(Acc));
				Native.ClearError();
				if(_elem != 0) return 0;
				_Hresult(_FuncId.child_count, _iacc.get_accChildCount(out var v));
				return v;
			}
		}

		/// <summary>
		/// Calls a callback function for each child or descendant accessible object.
		/// </summary>
		/// <param name="allDescendants">If true, all descendants, else only direct children.</param>
		/// <param name="f">
		/// Lambda etc callback function to call for each accessible object.
		/// Its parameter is of type <see cref="AFAcc"/> which has all Acc properties and some other data and methods.
		/// </param>
		/// <param name="param">Something to pass to the function as <see cref="AFAcc.Param"/>.</param>
		public void EnumChildren(bool allDescendants, Action<AFAcc> f, object param = null)
		{
			if(_Disposed) throw new ObjectDisposedException(nameof(Acc));
			if(_elem != 0) return;
			_EnumChildren(_iacc, allDescendants, f, new AFAcc() { Param = param }, 0);
		}

		/// <summary>
		/// Calls f for all children or descendants.
		/// </summary>
		static void _EnumChildren(IAccessible parent, bool allDescendants, Action<AFAcc> f, AFAcc args, int level)
		{
			using(var c = new _Children(parent)) {
				while(c.GetNext(out var a)) {
					try {
						args.LibSet(a.a, a.elem, 0, level);
						f(args);
						if(args.stop) return;
						if(args.skipChildren) continue;
						if(!allDescendants) continue;
						if(a.elem != 0) continue;
						_EnumChildren(a.a, true, f, args, level + 1);
						if(args.stop) return;
					}
					finally {
						c.FinallyInLoop(a);
						args._iacc = default; //not necessary, but let it throw objectdisposedexception if the callback assigned AFAcc to Acc and will try to use it (must use Args.ToAcc)
					}
				}
			}
		}

		/// <summary>
		/// Gets all child or descendant accessible objects.
		/// </summary>
		internal void LibGetChildren(bool allDescendants, List<Acc> a, bool clearList = true)
		{
			if(clearList) a.Clear();
			EnumChildren(allDescendants, p => (p.Param as List<Acc>).Add(p.ToAcc()), a);
		}

		/// <summary>
		/// Gets all child or descendant accessible objects.
		/// </summary>
		/// <param name="allDescendants">If true, all descendants, else only direct children.</param>
		public Acc[] Children(bool allDescendants)
		{
			var a = new List<Acc>();
			LibGetChildren(allDescendants, a);
			return a == null ? new Acc[0] : a.ToArray();
		}

		/// <summary>
		/// Gets direct children of an accessible object.
		/// Uses API <msdn>AccessibleChildren</msdn>.
		/// </summary>
		struct _Children :IDisposable
		{
			IAccessible _parent;
			VARIANT* _v;
			int _count, _i, _startAtIndex;
			bool _exactIndex, _reverse;

			/// <param name="parent"></param>
			/// <param name="startAtIndex">
			/// Can be 1-based child index from which to start searching (in both directions). If negative, it is index from the end.
			/// If 0, starts from the start or end, depending on the reverse parameter. If not 0, the reverse parameter is ignored.
			/// </param>
			/// <param name="exactIndex">Get object at startAtIndex and not at any other index. Not used when startIndex is 0.</param>
			/// <param name="reverse">When startIndex is 0, get objects in reverse order (starting from the end).</param>
			public _Children(IAccessible parent, int startAtIndex = 0, bool exactIndex = false, bool reverse = false) : this()
			{
				_parent = parent;
				_count = -1;
				_startAtIndex = startAtIndex;
				_exactIndex = exactIndex;
				_reverse = reverse;
			}

			void _Init()
			{
				//note: don't call get_accChildCount here.
				//	With Firefox and some other apps it makes almost 2 times slower. With others same speed.

				const int nStack = 100; //info: fast even with 10 or 7, but 5 makes slower. Just slightly faster with 100. Not faster with 30 etc.
				var v = stackalloc VARIANT[nStack];
				int hr = Api.AccessibleChildren(_parent, 0, nStack, v, out int n);
				if(hr < 0) { Debug_.PrintHex(hr); n = 0; } //never noticed

				if(n == nStack) { //probably there are more children
					_parent.get_accChildCount(out n); //note: some objects return 0 or 1, ie less than AccessibleChildren, and HRESULT is usually 0. Noticed this only in IE, when c_acBufferSize<10.
					if(n != nStack) { //yes, more children
						for(int i = nStack; i > 0;) v[--i].Dispose();
						if(n > Misc.MaxChildren) { //protection from AO such as LibreOffice Calc TABLE that has 1073741824 children. Default 10000.
							n = 0;
						} else {
							if(n < nStack) n = 1000; //get_accChildCount returned error or incorrect value
							_v = (VARIANT*)Util.NativeHeap.Alloc(n * sizeof(VARIANT), true);
							hr = Api.AccessibleChildren(_parent, 0, n, _v, out n); //note: iChildStart must be 0, else not always gets all children
							if(hr < 0) { Debug_.PrintHex(hr); n = 0; }
						}
					}
				}

				if(n > 0 && _v == null) {
					int memSize = n * sizeof(VARIANT);
					_v = (VARIANT*)Util.NativeHeap.Alloc(memSize);
					Api.memcpy(_v, v, memSize);
				}

				_count = n;
				if(n > 0 && _startAtIndex != 0) {
					if(_startAtIndex < 0) _startAtIndex = n + _startAtIndex; else _startAtIndex--; //if < 0, it is index from end
					int i = _startAtIndex; if(i < 0) i = 0; else if(i >= n) i = n - 1;
					if(_exactIndex && i != _startAtIndex) _startAtIndex = -1; else _startAtIndex = i;
				} else _startAtIndex = -1; //not used

				//speed: AccessibleChildren same as IEnumVARIANT with array. IEnumVARIANT.Next(1, ...) much slower.
				//	get_accChild is tried only when IEnumVARIANT not supported. Else it often fails or is slower.

				//50% AO passed to this function have 0 children. Then we don't allocate _v.
				//	20% have 1 child. Few have more than 7.

				//Acc CONSIDER: if we know it's Firefox, try to just IEnumVARIANT, because it's documented that if no IEnumVARIANT then there are no children.
				//	Now AccessibleChildren also calls get_accChildCount, which probably makes slower.
			}

			/// <summary>
			/// Gets next child.
			/// Returns false if there are no more children.
			/// First time also gets all children into an internal array. The constructor does not do it.
			/// </summary>
			public bool GetNext(out _Acc a)
			{
				a = default;
				if(_count < 0) _Init();
				if(_count == 0) return false;
				if(_exactIndex) {
					int i = _startAtIndex; _startAtIndex = -1;
					return i >= 0 && 0 == _parent.FromVARIANT(ref _v[i], out a);
				}
				g1:
				if(_startAtIndex < 0) { //_startAtIndex is -1 if not used
					if(_i >= _count) return false;
					int i = _i++; if(_reverse) i = _count - i - 1;
					if(0 != _parent.FromVARIANT(ref _v[i], out a)) goto g1;
				} else { //_startAtIndex is in _count range
					int i = _startAtIndex + _i;
					if(i < 0 || i >= _count) return false; //no more
														   //calculate next i
					if(_i >= 0) {
						_i = -(_i + 1);
						if(_startAtIndex + _i < 0) _i = -_i;
					} else {
						_i = -_i;
						if(_startAtIndex + _i >= _count) _i = -(_i + 1);
					}
					if(0 != _parent.FromVARIANT(ref _v[i], out a)) goto g1;
				}
				return true;
			}

			/// <summary>
			/// Finally call this after GetNext, to release a.a if a.elem == 0.
			/// </summary>
			public void FinallyInLoop(_Acc a)
			{
				if(a.elem == 0) a.a.Dispose();
			}

			/// <summary>
			/// Clears and frees the internal array.
			/// </summary>
			public void Dispose()
			{
				if(_v != null) {
					while(_count > 0) _v[--_count].Dispose(); //info: it's OK to dispose variants for which FromVARIANT was called because then vt is 0 and Dispose does nothing
					var t = _v; _v = null; Util.NativeHeap.Free(t);
				}
			}
		}

		/// <summary>
		/// When role is path, stores a path part as role string and optional start index.
		/// For example, if role is "A/B[4]", the array is _PathPart[2] { { "A", 0 }, { "B", 4 } }. 
		/// </summary>
		struct _PathPart
		{
			public string role;
			public int startIndex;
			public bool exactIndex;
		}
	}

}
