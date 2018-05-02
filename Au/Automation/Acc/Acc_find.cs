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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

#pragma warning disable CS0282 //VS bug: shows warning "There is no defined ordering between fields in multiple declarations of partial struct 'Acc'. To specify an ordering, all instance fields must be in the same declaration."

//TODO: some windows use eg Chrome control in non-chrome-classnamed window. Eg Spotify.
//	Now the tool somehow detects it and everything is OK, but need to review/test more.

namespace Au
{
	public unsafe partial class Acc
	{
		/// <summary>
		/// Contains accessible object (AO) properties and is used to find the AO.
		/// </summary>
		/// <remarks>
		/// Can be used instead of <see cref="Acc.Find(Wnd, string, string, String, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
		/// </remarks>
		/// <example>
		/// Find window that contains certain AO, and get the AO too.
		/// <code><![CDATA[
		/// var f = new Acc.Finder("BUTTON", "Apply"); //AO properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => f.Find(t));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public class Finder
		{
			string _role, _name, _prop;
			AFFlags _flags;
			int _skip;
			Cpp.AccCallbackT _callback;
			char _resultProp;

			/// <summary>
			/// The found accessible object.
			/// null if not found. null if used <see cref="ResultGetProperty"/>.
			/// </summary>
			public Acc Result { get; private set; }

			/// <summary>
			/// The requested propery of the found accessible object, depending on <see cref="ResultGetProperty"/>.
			/// null if: 1. Object not found. 2. ResultGetProperty not used or is '-'. 3. Failed to get the property.
			/// </summary>
			/// <remarks>
			/// The type depends on the property. Most properties are String. Others: <see cref="Rect"/>, <see cref="State"/>, <see cref="WndContainer"/>, <see cref="HtmlAttributes"/>.
			/// </remarks>
			public object ResultProperty { get; private set; }

			/// <summary>
			/// Set this when you need only some property of the accessible object (name, etc) and not the object itself.
			/// The value is a character, the same as with <see cref="GetProperties"/>, for example 'n' for Name. Use '-' if you don't need any property.
			/// </summary>
			/// <exception cref="ArgumentException">Parameter <i>also</i> was used.</exception>
			/// <remarks>
			/// ResultGetProperty cannot be used with parameter <i>also</i>.
			/// </remarks>
			public char ResultGetProperty
			{
				set
				{
					if(_callback != null) throw new ArgumentException("ResultGetProperty cannot be used with parameter 'also'.");
					_resultProp = value;
				}
			}

			void _ClearResult() { Result = null; ResultProperty = null; }

			/// <summary>
			/// Allows to specify accessible object properties, like with <see cref="Acc.Find(Wnd, string, string, String, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
			/// </summary>
			public Finder(string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<Acc, bool> also = null, int skip = 0)
			{
				_role = role;
				_name = name;
				_prop = prop;
				_flags = flags;
				_skip = skip;
				if(also != null) _callback = (Cpp.Cpp_Acc ca) => also(new Acc(ca)) ? 1 : 0;
			}

			/// <summary>
			/// Finds accessible object (AO) in the specified control of window w.
			/// Returns true if found. The <see cref="Result"/> property will be the found AO.
			/// </summary>
			/// <param name="w">Window that contains the control.</param>
			/// <param name="controls">Control properties. This functions searches in all matching controls.</param>
			/// <exception cref="Exception">Exceptions of <see cref="Find(Wnd)"/>.</exception>
			/// <remarks>
			/// Alternatively you can specify control class name or id in role. How this function is different: 1. Allows to specify more control properties. 2. Works better/faster when the control is of a different process or thread than the parent window; else slightly slower.
			/// </remarks>
			public bool Find(Wnd w, Wnd.ChildFinder controls)
			{
				w.ThrowIfInvalid();
				foreach(var c in controls.FindAll(w)) {
					try {
						if(_FindOrWait(c, 0, false)) {
							controls.Result = c;
							return true;
						}
					}
					catch(AuException ex) when(!c.IsAlive) { Debug_.Print(ex.Message); } //don't throw WndException/AuException if the window or a control is destroyed while searching, but throw AuException if eg access denied
				}
				return false;
			}

			/// <summary>
			/// Finds accessible object (AO) in window w, like <see cref="Acc.Find(Wnd, string, string, String, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
			/// Returns true if found. The <see cref="Result"/> property will be the found AO.
			/// </summary>
			/// <param name="w">Window or control that contains the AO.</param>
			/// <exception cref="ArgumentException">
			/// <i>role</i> is "" or invalid.
			/// <i>name</i> is invalid wildcard expression ("**options " or regular expression).
			/// <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
			/// Using flag <see cref="AFFlags.UIA"/> when searching in web page (role prefix "web:" etc).
			/// </exception>
			/// <exception cref="WndException">Invalid window.</exception>
			/// <exception cref="AuException">Failed. For example, window of a higher <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level process.</exception>
			public bool Find(Wnd w)
			{
				return _FindOrWait(w, 0, false);
			}

			/// <summary>
			/// Finds accessible object (AO) in another AO, like <see cref="Acc.Find(string, string, string, AFFlags, Func{Acc, bool}, int)"/>.
			/// Returns true if found. The <see cref="Result"/> property will be the found AO.
			/// </summary>
			/// <param name="a">Direct or indirect parent AO.</param>
			/// <exception cref="ArgumentNullException">a is null.</exception>
			/// <exception cref="ArgumentException">
			/// <i>role</i> is "" or invalid or has a prefix ("web:" etc).
			/// <i>name</i> is invalid wildcard expression ("**options " or regular expression).
			/// <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
			/// <see cref="SimpleElementId"/> is not 0.
			/// Using flag <see cref="AFFlags.UIA"/>.
			/// </exception>
			/// <exception cref="AuException">Failed. For example, window of a higher <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level process.</exception>
			public bool Find(Acc a)
			{
				return _FindOrWait(a, 0, false);
			}

			/// <summary>
			/// Finds accessible object (AO) in window w.
			/// The same as <see cref="Find(Wnd)"/>, but waits until the AO is found or the given time expires.
			/// Returns true if found. On timeout returns false if <paramref name="secondsTimeout"/> is negative; else exception.
			/// Returns true if found. Else if <paramref name="secondsTimeout"/> is negative, returns false. Else exception.
			/// </summary>
			/// <param name="secondsTimeout">
			/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns false.
			/// </param>
			/// <param name="w">Window or control that contains the AO.</param>
			/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
			/// <exception cref="Exception">Exceptions of <see cref="Find(Wnd)"/>.</exception>
			public bool Wait(double secondsTimeout, Wnd w)
			{
				return _FindOrWait(w, secondsTimeout, true);
			}

			/// <summary>
			/// Finds accessible object (AO) in another AO.
			/// The same as <see cref="Find(Acc)"/>, but waits until the AO is found or the given time expires.
			/// Returns true if found. On timeout returns false if <paramref name="secondsTimeout"/> is negative; else exception.
			/// </summary>
			/// <param name="secondsTimeout">
			/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns false.
			/// </param>
			/// <param name="a">Direct or indirect parent AO.</param>
			/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
			/// <exception cref="Exception">Exceptions of <see cref="Find(Acc)"/>.</exception>
			public bool Wait(double secondsTimeout, Acc a)
			{
				return _FindOrWait(a, secondsTimeout, true);
			}

			bool _FindOrWait(Wnd w, double secondsTimeout, bool isWaitFunc)
			{
				w.ThrowIfInvalid();
				return _Find(w, default, secondsTimeout, isWaitFunc);
			}

			bool _FindOrWait(Acc a, double secondsTimeout, bool isWaitFunc)
			{
				if(a == null) throw new ArgumentNullException();
				a.LibThrowIfDisposed();
				if(a.SimpleElementId != 0) throw new ArgumentException("SimpleElementId is not 0.");
				if(_flags.HasAny_(AFFlags.UIA | AFFlags.ClientArea)) throw new ArgumentException("Cannot use flags UIA and ClientArea when searching in Acc.");

				Cpp.Cpp_Acc aParent = a;
				var R = _Find(default, &aParent, secondsTimeout, isWaitFunc);
				GC.KeepAlive(a);
				return R;
			}

			bool _Find(Wnd w, Cpp.Cpp_Acc* aParent, double secondsTimeout, bool isWaitFunc)
			{
				if(_flags.Has_(AFFlags.UIA | AFFlags.ClientArea)) throw new ArgumentException("Cannot use flags UIA and ClientArea together.");

				_ClearResult();

				AFFlags flags = _flags;
				if(aParent != null) {
					if(!aParent->misc.flags.Has_(AccMiscFlags.InProc)) flags |= AFFlags.NotInProc;
				}

				bool inProc = !flags.Has_(AFFlags.NotInProc);

				if(!isWaitFunc) {
					Debug.Assert(secondsTimeout == 0.0);
					secondsTimeout = -1; //for WaitChromeDisabled
				}

				var ap = new Cpp.Cpp_AccParams(_role, _name, _prop, flags, _skip, _resultProp);

				var to = new WaitFor.Loop(secondsTimeout, inProc ? 20 : 100, inProc ? 200 : 1000);
				for(bool doneUAC = false, doneThread = false; ;) {
					var hr = Cpp.Cpp_AccFind(w, aParent, ref ap, _callback, out var ca, out var sResult);

					if(hr == 0) {
						switch(_resultProp) {
						case '\0':
							Result = new Acc(ca);
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
									case 'w': ResultProperty = (Wnd)(LPARAM)(*(int*)p); break;
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
						if(to.TimeRemaining < 3000) to.TimeRemaining += to.Period * 15 / 16;
						//normally waits ~10 times longer, eg 10 s instead of 1
						break;
					default:
						Debug.Assert(!Cpp.IsCppError((int)hr));
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
		/// Finds accessible object (AO) in window.
		/// Returns the found AO. Returns null if not found. See examples.
		/// </summary>
		/// <param name="w">Window or control that contains the AO.</param>
		/// <param name="role">
		/// AO role, like "LINK". Or path, like "ROLE/ROLE/ROLE".
		/// See <see cref="Role"/>. Can be used standard roles (see <see cref="AccROLE"/>) and custom roles (like "div" in Firefox).
		/// This parameter is string. If you want to use AccROLE: <c>nameof(AccROLE.CHECKBOX)</c>.
		/// Case-sensitive, not wildcard. Use null to match any role. Cannot be "".
		/// 
		/// Role or path can have a prefix:
		/// <list type="bullet">
		/// <item>
		/// "web:" - search only in the visible web page, not in whole window.
		/// Examples: "web:LINK", "web:/LIST/LISTITEM/LINK".
		/// Supports Firefox, Chrome, Internet Explorer (IE) and apps that use their code. With other windows, searches in the first found visible AO that has DOCUMENT role.
		/// <note type="note">Chrome web page accessible objects normally are disabled (don't exist). Use prefix "web:" or "chrome:" to enable.</note>
		/// Tip: To search only NOT in web pages, use <paramref name="prop"/> "notin=DOCUMENT" (Chrome, Firefox) or "notin=PANE" (IE).
		/// </item>
		/// <item>"firefox:" - search only in the visible web page of Firefox or Firefox-based web browser. If w window class name starts with "Mozilla", can be used "web:" instead.</item>
		/// <item>"chrome:" - search only in the visible web page of Chrome or Chrome-based web browser. If w window class name starts with "Chrome", can be used "web:" instead.</item>
		/// </list>
		/// Cannot use prefix when <paramref name="prop"/> contains "id" or "class". Also with flag <see cref="AFFlags.UIA"/>. Also when searching in Acc (<see cref="Find(string, string, string, AFFlags, Func{Acc, bool}, int)"/>).
		///
		/// Can be used path consisting of roles separated by "/". Examples:
		/// <list type="bullet">
		/// <item>"web:DOCUMENT/div/LIST/LISTITEM/LINK" - find LINK using its full path in web page.</item>
		/// <item>"web:/div/LIST//LINK" - the empty parts mean 'any role'. For example don't need to specify DOCUMENT because in web pages the first part is always DOCUMENT (Firefox, Chrome) or PANE (IE).</item>
		/// <item>"web:/div/LIST[4]/LISTITEM[-1]/LINK" - the 4 is 1-based index of div child from which to start searching (4-th, then 3-th, 5-th and so on). It can make faster. Negative means 'index from end', for example use -1 to search in reverse order. Flag <see cref="AFFlags.Reverse"/> is not applied to path parts with indexes. If index is invalid, will use the nearest valid index.</item>
		/// <item>"web:/div/LIST[4!]/LISTITEM[-1!]/LINK" - like the above, but the LIST must be exactly 4-th child (don't search 3-th, 5-th etc) and the LISTITEM must be the last child. This can be useful when waiting (uses less CPU), however useless if AO indices in the window or web page change often.</item>
		/// <item>"web://[4]/[-1!]/[2]" - index without role.</item>
		/// <item>"CLIENT/WINDOW/TREE/TREEITEM[-1]" - path in window or control. The first path part is a direct child AO of the WINDOW AO of the window/control. The WINDOW AO itself is not included in the search; if you need it, instead use <see cref="FromWindow"/>.</item>
		/// </list>
		/// </param>
		/// <param name="name">
		/// AO name (<see cref="Name"/>).
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'any'. "" means 'empty or unavailable'.
		/// </param>
		/// <param name="prop">
		/// Other AO properties and search settings.
		/// Format: one or more "name=value", separated with "\0" or "\0 ", like "description=xxx\0 @href=yyy". Names must match case. Values of string properties are wildcard expressions.
		/// 
		/// <list type="bullet">
		/// <item>
		/// "class" - search only in child controls that have this class name (see <see cref="Wnd.ClassName"/>).
		/// </item>
		/// <item>
		/// "id" - search only in child controls that have this id (see <see cref="Wnd.ControlId"/>).
		/// </item>
		/// <item>
		/// "value" - <see cref="Value"/>.
		/// </item>
		/// <item>
		/// "description" - <see cref="Description"/>.
		/// </item>
		/// <item>
		/// "state" - <see cref="State"/>. List of states that the AO must have and/or not have.
		/// Example: "state=CHECKED, FOCUSABLE, !DISABLED".
		/// Example: "state=0x100010, !0x1".
		/// Will find AO that has all states without "!" prefix and does not have any of states with "!" prefix.
		/// </item>
		/// <item>
		/// "rect" - <see cref="Rect"/>. Can be specified left, top, width and/or height, using <see cref="RECT.ToString"/> format.
		/// Example: "rect={L=1155 T=1182 W=132 H=13}".
		/// Example: "rect={W=132 T=1182}".
		/// The L T coordinates are relative to the primary screen.
		/// </item>
		/// <item>
		/// "level" - level (see <see cref="Level"/>) at which the AO can be found. Can be exact level, or minimal and maximal level separated by space.
		/// The default value is 0 1000.
		/// Alternatively you can use path in role, like "////LINK".
		/// </item>
		/// <item>
		/// "elem" - <see cref="SimpleElementId"/>.
		/// </item>
		/// <item>
		/// "action" - <see cref="DefaultAction"/>.
		/// </item>
		/// <item>
		/// "key" - <see cref="KeyboardShortcut"/>.
		/// </item>
		/// <item>
		/// "help" - <see cref="Help"/>.
		/// </item>
		/// <item>
		/// "uiaid" - <see cref="UiaId"/>.
		/// </item>
		/// <item>
		/// "maxcc" - when searching, skip children of AO that have more than this number of direct children. It can make faster.
		/// The default value is 10000. It also prevents hanging or crashing when an AO in the object tree has large number of children. For example OpenOffice Calc TABLE has one billion children.
		/// </item>
		/// <item>
		/// "notin" - when searching, skip children of AO that have these roles. It can make faster.
		/// Example: "notin=TREE,LIST,TOOLBAR".
		/// Roles in the list must be separated with "," or ", ". Case-sensitive, not wildcard.
		/// See also: <see cref="AFFlags.MenuToo"/>.
		/// </item>
		/// <item>
		/// "@attr" - <see cref="HtmlAttribute"/>. Here "attr" is any attribute name.
		/// Example: "@id=example".
		/// </item>
		/// </list>
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Lambda etc callback function to call for each matching AO. Let it return true if this is the wanted AO.
		/// Example, the AO must contain point x y: <c>o => o.GetRect(out var r, o.WndTopLevel) &amp;&amp; r.Contains(266, 33)</c>
		/// </param>
		/// <param name="skip">
		/// 0-based index of matching AO.
		/// For example, if 1, the function skips the first matching AO and returns the second.
		/// </param>
		/// <param name="controls">
		/// Properties of child controls where to search.
		/// This is an alternative for class/id in <paramref name="prop"/>. Allows to specify more control properties. Works better/faster when the control is of a different process or thread than the parent window; else slightly slower.
		/// </param>
		/// <exception cref="ArgumentException">
		/// role is "" or invalid.
		/// name is invalid wildcard expression ("**options " or regular expression).
		/// prop has invalid format or contains unknown property names or invalid wildcard expressions.
		/// Using flag <see cref="AFFlags.UIA"/> when searching in web page (role prefix "web:" etc).
		/// </exception>
		/// <exception cref="WndException">Invalid w.</exception>
		/// <exception cref="AuException">Failed. For example, window of a higher <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level process.</exception>
		/// <remarks>
		/// This function walks the tree of accessible objects of the window, control or web page, until it finds a matching AO.
		/// Uses <see cref="Finder.Find(Wnd)"/>. You can use it directly (see example).
		/// In wildcard expressions supports PCRE regular expressions (prefix "**r ") but not .NET regular expressions (prefix "**R "). They are similar.
		/// To find web page AOs usually it's better to use <see cref="Wait(double, Wnd, string, string, string, AFFlags, Func{Acc, bool}, int)"/> instead, it's more reliable.
		/// More info in <see cref="Acc"/> topic.
		/// </remarks>
		/// <example>
		/// Find link "Example" in web page, and click. Throw NotFoundException if not found.
		/// <code><![CDATA[
		/// var w = +Wnd.Find("* Chrome");
		/// var a = +Acc.Find(w, "web:LINK", "Example");
		/// a.DoAction();
		/// ]]></code>
		/// Try to find link "Example" in web page. Return if not found.
		/// <code><![CDATA[
		/// var w = +Wnd.Find("* Chrome");
		/// var a = Acc.Find(w, "web:LINK", "Example");
		/// if(a == null) { Print("not found"); return; }
		/// a.DoAction();
		/// ]]></code>
		/// Use a Finder.
		/// <code><![CDATA[
		/// var w = +Wnd.Find("* Chrome");
		/// var f = new Acc.Finder("BUTTON", "Example");
		/// if(!f.Find(w)) { Print("not found"); return; }
		/// Acc a = f.Result;
		/// a.DoAction();
		/// ]]></code>
		/// </example>
		public static Acc Find(Wnd w, string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<Acc, bool> also = null, int skip = 0, Wnd.ChildFinder controls = null)
		{
			var f = new Finder(role, name, prop, flags, also, skip);
			bool found = controls != null ? f.Find(w, controls) : f.Find(w);
			if(!found) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds a descendant accessible object (AO) of this AO.
		/// Returns the found AO. Returns null if not found.
		/// Parameters etc are as with <see cref="Find(Wnd, string, string, string, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// <paramref name="role"/> is "" or invalid or has a prefix ("web:" etc).
		/// <paramref name="name"/> is invalid wildcard expression ("**options " or regular expression).
		/// <paramref name="prop"/> has invalid format or contains unknown property names or invalid wildcard expressions or "class", "id".
		/// <see cref="SimpleElementId"/> is not 0.
		/// Using flag <see cref="AFFlags.UIA"/>.
		/// </exception>
		/// <exception cref="AuException">Failed. For example, window of a higher <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level process.</exception>
		public Acc Find(string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<Acc, bool> also = null, int skip = 0)
		{
			//info: f.Find will throw if this Acc is invalid etc.

			var f = new Finder(role, name, prop, flags, also, skip);
			if(!f.Find(this)) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds accessible object (AO) in window. Waits until the AO is found or the given time expires.
		/// Returns the found AO. On timeout returns null if <paramref name="secondsTimeout"/> is negative; else exception.
		/// Parameters etc are as with <see cref="Find(Wnd, string, string, string, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns null.
		/// </param>
		/// <param name="w"></param>
		/// <param name="role"></param>
		/// <param name="name"></param>
		/// <param name="prop"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="skip"></param>
		/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find(Wnd, string, string, string, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.</exception>
		public static Acc Wait(double secondsTimeout, Wnd w, string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<Acc, bool> also = null, int skip = 0)
		{
			var f = new Finder(role, name, prop, flags, also, skip);
			if(!f.Wait(secondsTimeout, w)) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds a descendant accessible object (AO) of this AO. Waits until the AO is found or the given time expires.
		/// Returns the found AO. On timeout returns null if <paramref name="secondsTimeout"/> is negative; else exception.
		/// Parameters etc are as with <see cref="Find(Wnd, string, string, string, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns null.
		/// </param>
		/// <param name="role"></param>
		/// <param name="name"></param>
		/// <param name="prop"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="skip"></param>
		/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Find(string, string, string, AFFlags, Func{Acc, bool}, int)"/>.</exception>
		public Acc Wait(double secondsTimeout, string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<Acc, bool> also = null, int skip = 0)
		{
			//info: f.Find will throw if this Acc is invalid etc.

			var f = new Finder(role, name, prop, flags, also, skip);
			if(!f.Wait(secondsTimeout, this)) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds all matching accessible objects in window.
		/// Returns array of 0 or more elements.
		/// Parameters etc are as with <see cref="Find(Wnd, string, string, string, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="Find(Wnd, string, string, string, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.</exception>
		/// <example>
		/// Get all taskbar buttons (Windows 10).
		/// <code><![CDATA[
		/// var w = +Wnd.Find(null, "Shell_TrayWnd");
		/// foreach(var a in Acc.FindAll(w, "BUTTON", prop: "level=7")) Print(a);
		/// ]]></code>
		/// </example>
		public static Acc[] FindAll(Wnd w, string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<Acc, bool> also = null)
		{
			var a = new List<Acc>();
			Find(w, role, name, prop, flags, o =>
			{
				if(also == null || also(o)) a.Add(o);
				return false;
			});
			return a.ToArray();
		}

		/// <summary>
		/// Finds all matching descendant accessible objects (AO) of this AO.
		/// Returns array of 0 or more elements.
		/// Parameters etc are as with <see cref="Find(Wnd, string, string, string, AFFlags, Func{Acc, bool}, int, Wnd.ChildFinder)"/>.
		/// </summary>
		/// <exception cref="Exception">Exceptions of <see cref="Find(string, string, string, AFFlags, Func{Acc, bool}, int)"/>.</exception>
		/// <example>
		/// Get all taskbar buttons (Windows 10).
		/// <code><![CDATA[
		/// var w = +Wnd.Find(null, "Shell_TrayWnd");
		/// var atb = +Acc.Find(w, "TOOLBAR", "Running applications");
		/// foreach(var a in atb.FindAll("BUTTON", prop: "level=0")) Print(a);
		/// ]]></code>
		/// </example>
		public Acc[] FindAll(string role = null, string name = null, string prop = null, AFFlags flags = 0, Func<Acc, bool> also = null)
		{
			var a = new List<Acc>();
			Find(role, name, prop, flags, o =>
			{
				if(also == null || also(o)) a.Add(o);
				return false;
			});
			return a.ToArray();
		}

		/// <summary>
		/// Gets the number of direct child objects.
		/// Uses <msdn>IAccessible.get_accChildCount</msdn>.
		/// </summary>
		public int ChildCount
		{
			get
			{
				LibThrowIfDisposed();
				if(_elem != 0) { Native.ClearError(); return 0; }
				_Hresult(_FuncId.child_count, Cpp.Cpp_AccGetInt(this, 'c', out int cc));
				GC.KeepAlive(this);
				return cc;
			}
		}
	}

}
