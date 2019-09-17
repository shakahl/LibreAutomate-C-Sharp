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
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	public unsafe partial class AAcc
	{
		/// <summary>
		/// Contains accessible object (AO) properties and is used to find the AO.
		/// </summary>
		/// <remarks>
		/// Can be used instead of <see cref="AAcc.Find"/>.
		/// </remarks>
		/// <example>
		/// Find window that contains certain AO, and get the AO too.
		/// <code><![CDATA[
		/// var f = new AAcc.Finder("BUTTON", "Apply"); //AO properties
		/// AWnd w = AWnd.Find(cn: "#32770", also: t => f.Find(t));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public class Finder
		{
			string _role, _name, _prop, _navig;
			AFFlags _flags;
			int _skip;
			Cpp.AccCallbackT _also;
			char _resultProp;

			/// <summary>
			/// The found accessible object.
			/// null if not found. null if used <see cref="ResultGetProperty"/>.
			/// </summary>
			public AAcc Result { get; private set; }

			/// <summary>
			/// The requested propery of the found accessible object, depending on <see cref="ResultGetProperty"/>.
			/// null if: 1. Object not found. 2. <b>ResultGetProperty</b> not used or is '-'. 3. Failed to get the property.
			/// </summary>
			/// <remarks>
			/// The type depends on the property. Most properties are String. Others: <see cref="Rect"/>, <see cref="State"/>, <see cref="WndContainer"/>, <see cref="HtmlAttributes"/>.
			/// </remarks>
			public object ResultProperty { get; private set; }

			/// <summary>
			/// Set this when you need only some property of the accessible object (name, etc) and not the object itself.
			/// The value is a character, the same as with <see cref="GetProperties"/>, for example 'n' for Name. Use '-' if you don't need any property.
			/// </summary>
			/// <exception cref="ArgumentException">Used parameter <i>also</i> or <i>navig</i>.</exception>
			public char ResultGetProperty
			{
				set
				{
					if(_also != null) throw new ArgumentException("ResultGetProperty cannot be used with parameter 'also'.");
					if(_navig != null) throw new ArgumentException("ResultGetProperty cannot be used with parameter 'navig'.");
					_resultProp = value;
				}
			}

			/// <summary>
			/// true if used parameter <i>navig</i> and the intermediate object was found but the navigation did not find the final object.
			/// </summary>
			public bool NavigFailed { get; private set; }

			void _ClearResult() { Result = null; ResultProperty = null; NavigFailed = false; }

			/// <summary>
			/// Stores the specified accessible object properties in this object. Reference: <see cref="AAcc.Find"/>.
			/// Does not search now. For it call <b>Find</b> or <b>Wait</b>.
			/// </summary>
			public Finder(string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<AAcc, bool> also = null, int skip = 0, string navig = null)
			{
				_role = role;
				_name = name;
				_prop = prop;
				_flags = flags;
				_skip = skip;
				_navig = navig;
				if(also != null) _also = (Cpp.Cpp_Acc ca) => also(new AAcc(ca)) ? 1 : 0;
			}

			/// <summary>
			/// Finds accessible object (AO) in the specified control of window w.
			/// Returns true if found. The <see cref="Result"/> property will be the found AO.
			/// </summary>
			/// <param name="w">Window that contains the control.</param>
			/// <param name="controls">Control properties. This functions searches in all matching controls.</param>
			/// <exception cref="Exception">Exceptions of <see cref="Find(AWnd)"/>.</exception>
			/// <remarks>
			/// Alternatively you can specify control class name or id in role. How this function is different: 1. Allows to specify more control properties. 2. Works better/faster when the control is of a different process or thread than the parent window; else slightly slower.
			/// </remarks>
			public bool Find(AWnd w, AWnd.ChildFinder controls)
			{
				w.ThrowIfInvalid();
				foreach(var c in controls.FindAll(w)) {
					try {
						if(_FindOrWait(c, 0, false)) {
							controls.Result = c;
							return true;
						}
					}
					catch(AuException ex) when(!c.IsAlive) { ADebug.Print(ex.Message); } //don't throw WndException/AuException if the window or a control is destroyed while searching, but throw AuException if eg access denied
				}
				return false;
			}

			/// <summary>
			/// Finds accessible object (AO) in window w, like <see cref="AAcc.Find"/>.
			/// Returns true if found. The <see cref="Result"/> property will be the found AO.
			/// </summary>
			/// <param name="w">Window or control that contains the AO.</param>
			/// <exception cref="ArgumentException">
			/// - <i>role</i> is "" or invalid.
			/// - <i>name</i> is invalid wildcard expression (<c>"**options "</c> or regular expression).
			/// - <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
			/// - flag <see cref="AFFlags.UIA"/> when searching in web page (role prefix "web:" etc).
			/// </exception>
			/// <exception cref="WndException">Invalid window.</exception>
			/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
			public bool Find(AWnd w)
			{
				return _FindOrWait(w, 0, false);
			}

			/// <summary>
			/// Finds accessible object (AO) in another AO, like <see cref="AAcc.Find(string, string, string, AFFlags, Func{AAcc, bool}, int, string)"/>.
			/// Returns true if found. The <see cref="Result"/> property will be the found AO.
			/// </summary>
			/// <param name="a">Direct or indirect parent AO.</param>
			/// <exception cref="ArgumentNullException">a is null.</exception>
			/// <exception cref="ArgumentException">
			/// - <i>role</i> is "" or invalid or has a prefix ("web:" etc).
			/// - <i>name</i> is invalid wildcard expression (<c>"**options "</c> or regular expression).
			/// - <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
			/// - flag <see cref="AFFlags.UIA"/>.
			/// - <see cref="SimpleElementId"/> is not 0.
			/// </exception>
			/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
			public bool Find(AAcc a)
			{
				return _FindOrWait(a, 0, false);
			}

			/// <summary>
			/// Finds accessible object (AO) in window w.
			/// The same as <see cref="Find(AWnd)"/>, but waits until the AO is found or the given time expires.
			/// </summary>
			/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
			/// <param name="w">Window or control that contains the AO.</param>
			/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
			/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
			/// <exception cref="Exception">Exceptions of <see cref="Find(AWnd)"/>.</exception>
			public bool Wait(double secondsTimeout, AWnd w)
			{
				return _FindOrWait(w, secondsTimeout, true);
			}

			/// <summary>
			/// Finds accessible object (AO) in another AO.
			/// The same as <see cref="Find(AAcc)"/>, but waits until the AO is found or the given time expires.
			/// </summary>
			/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
			/// <param name="a">Direct or indirect parent AO.</param>
			/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
			/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
			/// <exception cref="Exception">Exceptions of <see cref="Find(AAcc)"/>.</exception>
			public bool Wait(double secondsTimeout, AAcc a)
			{
				return _FindOrWait(a, secondsTimeout, true);
			}

			bool _FindOrWait(AWnd w, double secondsTimeout, bool isWaitFunc)
			{
				w.ThrowIfInvalid();
				return _Find(w, default, secondsTimeout, isWaitFunc);
			}

			bool _FindOrWait(AAcc a, double secondsTimeout, bool isWaitFunc)
			{
				if(a == null) throw new ArgumentNullException();
				a.LibThrowIfDisposed();
				if(a.SimpleElementId != 0) throw new ArgumentException("SimpleElementId is not 0.");
				if(_flags.HasAny(AFFlags.UIA | AFFlags.ClientArea)) throw new ArgumentException("Cannot use flags UIA and ClientArea when searching in AAcc.");

				Cpp.Cpp_Acc aParent = a;
				var R = _Find(default, &aParent, secondsTimeout, isWaitFunc);
				GC.KeepAlive(a);
				return R;
			}

			bool _Find(AWnd w, Cpp.Cpp_Acc* aParent, double secondsTimeout, bool isWaitFunc)
			{
				if(_flags.Has(AFFlags.UIA | AFFlags.ClientArea)) throw new ArgumentException("Cannot use flags UIA and ClientArea together.");

				_ClearResult();

				AFFlags flags = _flags;
				if(aParent != null) {
					if(!aParent->misc.flags.Has(AccMiscFlags.InProc)) flags |= AFFlags.NotInProc;
				}

				bool inProc = !flags.Has(AFFlags.NotInProc);

				if(!isWaitFunc) {
					Debug.Assert(secondsTimeout == 0.0);
					secondsTimeout = -1; //for WaitChromeDisabled
				}

				var ap = new Cpp.Cpp_AccParams(_role, _name, _prop, flags, _skip, _resultProp);

				var to = new AWaitFor.Loop(secondsTimeout, inProc ? 10 : 40);
				for(bool doneUAC = false, doneThread = false; ;) {
					var hr = Cpp.Cpp_AccFind(w, aParent, in ap, _also, out var ca, out var sResult);

					if(hr == 0) {
						switch(_resultProp) {
						case '\0':
							var res = new AAcc(ca);
							if(_navig != null) {
								res = res.Navigate(_navig);
								if(NavigFailed = (res == null)) {
									if(isWaitFunc && to.Sleep()) continue;
									return false;
								}
							}
							Result = res;
							break;
						case 'r':
						case 's':
						case 'w':
						case '@':
							if(sResult == null) break;
							unsafe {
								fixed (char* p = sResult) {
									switch(_resultProp) {
									case 'r': ResultProperty = *(RECT*)p; break;
									case 's': ResultProperty = *(AccSTATE*)p; break;
									case 'w': ResultProperty = (AWnd)(*(int*)p); break;
									case '@': ResultProperty = _AttributesToDictionary(p, sResult.Length); break;
									}
								}
							}
							break;
						default:
							ResultProperty = sResult;
							break;
						}
						return true;
					}

					if(hr == Cpp.EError.InvalidParameter) throw new ArgumentException(sResult);
					if((hr == Cpp.EError.WindowClosed) || (!w.Is0 && !w.IsAlive)) return false; //FUTURE: check if a is disconnected etc. Or then never wait.

					if(!doneUAC) {
						doneUAC = true;
						w.LibUacCheckAndThrow(); //CONSIDER: don't throw. Maybe show warning.
					}

					//Print(hr > 0 ? $"hr={hr}" : $"hr={(int)hr:X}");
					switch(hr) {
					case Cpp.EError.NotFound:
						if(!isWaitFunc) return false;
						break;
					case Cpp.EError.WaitChromeDisabled:
						//Print("WaitChromeDisabled");
						if(to.TimeRemaining < 3000) to.TimeRemaining += (long)(to.Period * 15 / 16);
						//normally waits ~10 times longer, eg 10 s instead of 1
						break;
					default:
						Debug.Assert(!Cpp.IsCppError((int)hr));
						if(hr == (Cpp.EError)Api.RPC_E_SERVER_CANTMARSHAL_DATA && !_flags.Has(AFFlags.NotInProc))
							throw new AuException((int)hr, "For this object need flag NotInProc");
						throw new AuException((int)hr);
					}

					if(!doneThread) {
						doneThread = true;
						if(!w.Is0 && w.IsOfThisThread) return false;
					}

					if(!to.Sleep()) return false;
				}
			}
		}

		/// <summary>
		/// Finds an accessible object (AO) in window.
		/// Returns the found AO. Returns null if not found.
		/// </summary>
		/// <param name="w">Window or control that contains the AO.</param>
		/// <param name="role">
		/// AO role, like <c>"LINK"</c> or <c>nameof(AccROLE.LINK)</c>. Or path, like <c>"ROLE/ROLE/ROLE"</c>.
		/// Can be used standard roles (see <see cref="AccROLE"/>) and custom roles (like <c>"div"</c> in Firefox). More info: <see cref="Role"/>.
		/// 
		/// Case-sensitive. Not wildcard.
		/// null means 'can be any'. Cannot be "".
		/// 
		/// Can have a prefix:
		/// - <c>"web:"</c> - search only in the visible web page, not in whole window. Examples: <c>"web:LINK"</c>, <c>"web:/LIST/LISTITEM/LINK"</c>.\
		///   Supports Firefox, Chrome, Internet Explorer (IE) and apps that use their code. With other windows, searches in the first found visible AO that has DOCUMENT role.\
		///   Tip: To search only NOT in web pages, use <i>prop</i> <c>"notin=DOCUMENT"</c> (Chrome, Firefox) or <c>"notin=PANE"</c> (IE).
		/// - <c>"firefox:"</c> - search only in the visible web page of Firefox or Firefox-based web browser. If w window class name starts with "Mozilla", can be used <c>"web:"</c> instead.
		/// - <c>"chrome:"</c> - search only in the visible web page of Chrome or Chrome-based web browser. If w window class name starts with "Chrome", can be used <c>"web:"</c> instead.
		/// 
		/// <note>Chrome web page accessible objects normally are disabled (don't exist). Use prefix <c>"web:"</c> or <c>"chrome:"</c> to enable.</note>
		/// 
		/// Prefix cannot be used:
		/// - if <i>prop</i> contains <c>"id"</c> or <c>"class"</c>;
		/// - with flag <see cref="AFFlags.UIA"/>;
		/// - with the non-static <b>Find</b> overload (searching in an AO).
		/// 
		/// Can be path consisting of roles separated by "/". Examples:
		/// - <c>"web:DOCUMENT/div/LIST/LISTITEM/LINK"</c> - find LINK using its full path in web page.
		/// - <c>"web:/div/LIST//LINK"</c> - the empty parts mean 'any role'. For example don't need to specify DOCUMENT because in web pages the first part is always DOCUMENT (Firefox, Chrome) or PANE (IE).
		/// - <c>"web:/div/LIST[4]/LISTITEM[-1]/LINK"</c> - the 4 is 1-based index of div child from which to start searching (4-th, then 3-th, 5-th and so on). It can make faster. Negative means 'index from end', for example use -1 to search in reverse order. Flag <see cref="AFFlags.Reverse"/> is not applied to path parts with indexes. If index is invalid, will use the nearest valid index.
		/// - <c>"web:/div/LIST[4!]/LISTITEM[-1!]/LINK"</c> - like the above, but the LIST must be exactly 4-th child (don't search 3-th, 5-th etc) and the LISTITEM must be the last child. This can be useful when waiting (uses less CPU), however useless if AO indices in the window or web page change often.
		/// - <c>"web://[4]/[-1!]/[2]"</c> - index without role.
		/// - <c>"CLIENT/WINDOW/TREE/TREEITEM[-1]"</c> - path in window or control. The first path part is a direct child AO of the WINDOW AO of the window/control. The WINDOW AO itself is not included in the search; if you need it, instead use <see cref="FromWindow"/>.
		/// </param>
		/// <param name="name">
		/// AO name (<see cref="Name"/>).
		/// String format: [](xref:wildcard_expression).
		/// null means 'any'. "" means 'empty or unavailable'.
		/// </param>
		/// <param name="prop">
		/// Other AO properties and search settings.
		/// Format: one or more <c>"name=value"</c>, separated with <c>"\0"</c> or <c>"\0 "</c>, like <c>"description=xxx\0 @href=yyy"</c>. Names must match case. Values of string properties are wildcard expressions.
		/// 
		/// - <c>"class"</c> - search only in child controls that have this class name (see <see cref="AWnd.ClassName"/>).
		/// Cannot be used when searching in an AO.
		/// - <c>"id"</c> - search only in child controls that have this id (see <see cref="AWnd.ControlId"/>). If the value is not a number - Windows Forms control name (see <see cref="AWnd.NameWinForms"/>); case-sensitive, not wildcard.
		/// Cannot be used when searching in an AO.
		/// - <c>"value"</c> - <see cref="Value"/>.
		/// - <c>"description"</c> - <see cref="Description"/>.
		/// - <c>"state"</c> - <see cref="State"/>. List of states that the AO must have and/or not have.
		/// Example: <c>"state=CHECKED, FOCUSABLE, !DISABLED"</c>.
		/// Example: <c>"state=0x100010, !0x1"</c>.
		/// Will find AO that has all states without <c>"!"</c> prefix and does not have any of states with <c>"!"</c> prefix.
		/// - <c>"rect"</c> - <see cref="Rect"/>. Can be specified left, top, width and/or height, using <see cref="RECT.ToString"/> format.
		/// Example: <c>"rect={L=1155 T=1182 W=132 H=13}"</c>.
		/// Example: <c>"rect={W=132 T=1182}"</c>.
		/// The L T coordinates are relative to the primary screen.
		/// - <c>"level"</c> - level (see <see cref="Level"/>) at which the AO can be found. Can be exact level, or minimal and maximal level separated by space.
		/// The default value is 0 1000.
		/// Alternatively you can use path in role, like <c>"////LINK"</c>.
		/// - <c>"elem"</c> - <see cref="SimpleElementId"/>.
		/// - <c>"action"</c> - <see cref="DefaultAction"/>.
		/// - <c>"key"</c> - <see cref="KeyboardShortcut"/>.
		/// - <c>"help"</c> - <see cref="Help"/>.
		/// - <c>"uiaid"</c> - <see cref="UiaId"/>.
		/// - <c>"maxcc"</c> - when searching, skip children of AO that have more than this number of direct children. It can make faster.
		/// The default value is 10000. It also prevents hanging or crashing when an AO in the object tree has large number of children. For example OpenOffice Calc TABLE has one billion children.
		/// - <c>"notin"</c> - when searching, skip children of AO that have these roles. It can make faster.
		/// Example: <c>"notin=TREE,LIST,TOOLBAR"</c>.
		/// Roles in the list must be separated with <c>","</c> or <c>", "</c>. Case-sensitive, not wildcard.
		/// See also: <see cref="AFFlags.MenuToo"/>.
		/// - <c>"@attr"</c> - <see cref="HtmlAttribute"/>. Here "attr" is any attribute name.
		/// 
		/// Example: <c>"@id=example"</c>.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Callback function. Called for each matching AO. Let it return true if this is the wanted AO.
		/// Example: the AO must contain point x y: <c>o => o.GetRect(out var r, o.WndTopLevel) &amp;&amp; r.Contains(266, 33)</c>
		/// </param>
		/// <param name="skip">
		/// 0-based index of matching AO.
		/// For example, if 1, the function skips the first matching AO and returns the second.
		/// </param>
		/// <param name="navig">If not null, the specified object is an intermediate object. After finding it, call <see cref="Navigate"/> with this string and return its result.</param>
		/// <param name="controls">
		/// Properties of child controls where to search.
		/// This is an alternative for class/id in <i>prop</i>. Allows to specify more control properties. Works better/faster when the control is of a different process or thread than the parent window; else slightly slower.
		/// </param>
		/// 
		/// <exception cref="ArgumentException">
		/// - <i>role</i> is "" or invalid.
		/// - <i>name</i> is invalid wildcard expression (<c>"**options "</c> or regular expression).
		/// - <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
		/// - <i>navig</i> string is invalid.
		/// - <i>flags</i> has <see cref="AFFlags.UIA"/> when searching in web page (role prefix <c>"web:"</c> etc).
		/// </exception>
		/// <exception cref="WndException">Invalid window.</exception>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		/// 
		/// <remarks>
		/// To create code for this function, use dialog "Find accessible object". It is form <b>Au.Tools.FormAAcc</b> in Au.Tools.dll.
		/// 
		/// Walks the tree of accessible objects, until finds a matching AO.
		/// 
		/// Uses <see cref="Finder"/>. You can use it directly. See example.
		/// 
		/// In wildcard expressions supports PCRE regular expressions (prefix <c>"**r "</c>) but not .NET regular expressions (prefix <c>"**R "</c>). They are similar.
		/// 
		/// To find web page AOs usually it's better to use <see cref="Wait"/> instead, it's more reliable.
		/// 
		/// More info in <see cref="AAcc"/> topic.
		/// </remarks>
		/// <example>
		/// Find link "Example" in web page, and click. Throw <b>NotFoundException</b> if not found.
		/// <code><![CDATA[
		/// var w = AWnd.Find("* Chrome").OrThrow();
		/// var a = AAcc.Find(w, "web:LINK", "Example").OrThrow();
		/// a.DoAction();
		/// ]]></code>
		/// Try to find link "Example" in web page. Return if not found.
		/// <code><![CDATA[
		/// var w = AWnd.Find("* Chrome").OrThrow();
		/// var a = AAcc.Find(w, "web:LINK", "Example");
		/// if(a == null) { Print("not found"); return; }
		/// a.DoAction();
		/// ]]></code>
		/// Use <see cref="Finder"/>.
		/// <code><![CDATA[
		/// var w = AWnd.Find("* Chrome").OrThrow();
		/// var f = new AAcc.Finder("BUTTON", "Example");
		/// if(!f.Find(w)) { Print("not found"); return; }
		/// AAcc a = f.Result;
		/// a.DoAction();
		/// ]]></code>
		/// </example>
		public static AAcc Find(AWnd w, string role = null, string name = null, string prop = null, AFFlags flags = 0,
			Func<AAcc, bool> also = null, int skip = 0, string navig = null, AWnd.ChildFinder controls = null)
		{
			var f = new Finder(role, name, prop, flags, also, skip, navig);
			bool found = controls != null ? f.Find(w, controls) : f.Find(w);
			if(!found) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds a descendant accessible object (AO) of this AO.
		/// Returns the found AO. Returns null if not found.
		/// </summary>
		/// <exception cref="ArgumentException">Exceptions of other overload, plus:
		/// - <i>flags</i> has <see cref="AFFlags.UIA"/>.
		/// - <see cref="SimpleElementId"/> is not 0.
		/// </exception>
		/// <exception cref="AuException">Failed.</exception>
		public AAcc Find(string role = null, string name = null, string prop = null, AFFlags flags = 0,
			Func<AAcc, bool> also = null, int skip = 0, string navig = null)
		{
			//info: f.Find will throw if this AAcc is invalid etc.

			var f = new Finder(role, name, prop, flags, also, skip, navig);
			if(!f.Find(this)) return null;
			return f.Result;
		}

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		/// <summary>
		/// Finds accessible object (AO) in window. Waits until the AO is found or the given time expires.
		/// More info: <see cref="Find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns the found AO. On timeout returns null if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="WndException"/>
		/// <exception cref="AuException"/>
		public static AAcc Wait(double secondsTimeout, AWnd w, string role = null, string name = null, string prop = null, AFFlags flags = 0,
			Func<AAcc, bool> also = null, int skip = 0, string navig = null)
		{
			var f = new Finder(role, name, prop, flags, also, skip, navig);
			if(!f.Wait(secondsTimeout, w)) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds a descendant accessible object (AO) of this AO. Waits until the AO is found or the given time expires.
		/// More info: <see cref="Find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns the found AO. On timeout returns null if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="AuException"/>
		public AAcc Wait(double secondsTimeout, string role = null, string name = null, string prop = null, AFFlags flags = 0,
			Func<AAcc, bool> also = null, int skip = 0, string navig = null)
		{
			//info: f.Find will throw if this AAcc is invalid etc.

			var f = new Finder(role, name, prop, flags, also, skip, navig);
			if(!f.Wait(secondsTimeout, this)) return null;
			return f.Result;
		}
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

		/// <summary>
		/// Finds all matching accessible objects in window.
		/// More info: <see cref="Find"/>.
		/// </summary>
		/// <returns>Array of 0 or more elements.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="WndException"/>
		/// <exception cref="AuException"/>
		/// <example>
		/// Get all taskbar buttons (Windows 10).
		/// <code><![CDATA[
		/// var w = AWnd.Find(null, "Shell_TrayWnd").OrThrow();
		/// foreach(var a in AAcc.FindAll(w, "BUTTON", prop: "level=7")) Print(a);
		/// ]]></code>
		/// </example>
		public static AAcc[] FindAll(AWnd w, string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<AAcc, bool> also = null)
		{
			var a = new List<AAcc>();
			Find(w, role, name, prop, flags, o =>
			{
				if(also == null || also(o)) a.Add(o);
				return false;
			});
			return a.ToArray();
		}

		/// <summary>
		/// Finds all matching descendant accessible objects (AO) of this AO.
		/// More info: <see cref="Find"/>.
		/// </summary>
		/// <returns>Array of 0 or more elements.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="AuException"/>
		/// <example>
		/// Get all taskbar buttons (Windows 10).
		/// <code><![CDATA[
		/// var w = AWnd.Find(null, "Shell_TrayWnd").OrThrow();
		/// var atb = AAcc.Find(w, "TOOLBAR", "Running applications").OrThrow();
		/// foreach(var a in atb.FindAll("BUTTON", prop: "level=0")) Print(a);
		/// ]]></code>
		/// </example>
		public AAcc[] FindAll(string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<AAcc, bool> also = null)
		{
			var a = new List<AAcc>();
			Find(role, name, prop, flags, o =>
			{
				if(also == null || also(o)) a.Add(o);
				return false;
			});
			return a.ToArray();
		}
	}

}
