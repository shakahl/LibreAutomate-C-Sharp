using Au;
using Au.Types;
using Au.More;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;


namespace Au
{
	public unsafe partial class elm
	{
		/// <summary>
		/// Finds a UI element in window.
		/// </summary>
		/// <returns>UI element, or null if not found. See also: <see cref="operator +(elm)"/>.</returns>
		/// <param name="w">Window or control that contains the UI element.</param>
		/// <param name="role">
		/// UI element role, like <c>"LINK"</c>. Or path, like <c>"ROLE/ROLE/ROLE"</c>.
		/// Can have prefix <c>"web:"</c>, <c>"firefox:"</c> or <c>"chrome:"</c> which means "search only in web page" and enables Chrome UI elements.
		/// Case-sensitive. Not wildcard. null means 'can be any'. Cannot be "".
		/// More info in Remarks.
		/// </param>
		/// <param name="name">
		/// UI element name (<see cref="Name"/>).
		/// String format: [](xref:wildcard_expression).
		/// null means 'any'. "" means 'empty or unavailable'.
		/// </param>
		/// <param name="prop">
		/// Other UI element properties and search settings.
		/// Example: <c>"description=xxx\0 @href=yyy"</c>.
		/// More info in Remarks.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Callback function. Called for each matching UI element. Let it return true if this is the wanted UI element.
		/// Example: the UI element must contain point x y: <c>o => o.GetRect(out var r, o.WndTopLevel) &amp;&amp; r.Contains(266, 33)</c>
		/// </param>
		/// <param name="skip">
		/// 0-based index of matching UI element.
		/// For example, if 1, the function skips the first matching UI element and returns the second.
		/// </param>
		/// <param name="navig">If not null, the specified UI element is an intermediate UI element. After finding it, call <see cref="Navigate"/> with this string and return its result.</param>
		/// <param name="controls">
		/// Defines child controls where to search.
		/// This is an alternative for class/id in <i>prop</i>. Allows to specify more control properties. Works better/faster when the control is of a different process or thread than the parent window; else slightly slower.
		/// Currently not supported with non-0 <i>waitS</i>.
		/// </param>
		/// 
		/// <exception cref="ArgumentException">
		/// - <i>role</i> is "" or invalid.
		/// - <i>name</i> is invalid wildcard expression (<c>"**options "</c> or regular expression).
		/// - <i>prop</i> has invalid format or contains unknown property names or invalid wildcard expressions.
		/// - <i>navig</i> string is invalid.
		/// - <i>flags</i> has <see cref="EFFlags.UIA"/> when searching in web page (role prefix <c>"web:"</c> etc).
		/// - non-null <i>controls</i> with non-0 <i>waitS</i>.
		/// </exception>
		/// <exception cref="AuWndException">Invalid window.</exception>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		/// 
		/// <remarks>
		/// To create code for this function, use dialog "Find UI element".
		/// 
		/// Walks the tree of UI elements, until finds a matching UI element.
		/// 
		/// Uses <see cref="elmFinder"/>. You can use it directly. See example.
		/// 
		/// In wildcard expressions supports PCRE regular expressions (prefix <c>"**r "</c>) but not .NET regular expressions (prefix <c>"**R "</c>). They are similar.
		/// 
		/// More info in <see cref="elm"/> topic.
		/// 
		/// ##### About the <i>role</i> parameter
		/// 
		/// Can be standard role (see <see cref="ERole"/>) like <c>"LINK"</c> or custom role like <c>"div"</c>. More info: <see cref="Role"/>.
		/// 
		/// Can have a prefix:
		/// - <c>"web:"</c> - search only in the visible web page, not in whole window. Examples: <c>"web:LINK"</c>, <c>"web:/LIST/LISTITEM/LINK"</c>.\
		///   Supports Firefox, Chrome, Internet Explorer (IE) and apps that use their code. With other windows, searches in the first found visible UI element that has DOCUMENT role.\
		///   Tip: To search only NOT in web pages, use <i>prop</i> <c>"notin=DOCUMENT"</c> (Chrome, Firefox) or <c>"notin=PANE"</c> (IE).
		/// - <c>"firefox:"</c> - search only in the visible web page of Firefox or Firefox-based web browser. If w window class name starts with "Mozilla", can be used <c>"web:"</c> instead.
		/// - <c>"chrome:"</c> - search only in the visible web page of Chrome or Chrome-based web browser. If w window class name starts with "Chrome", can be used <c>"web:"</c> instead.
		/// 
		/// <note>Chrome web page UI elements normally are disabled (don't exist). Use prefix <c>"web:"</c> or <c>"chrome:"</c> to enable.</note>
		/// 
		/// Prefix cannot be used:
		/// - if <i>prop</i> contains <c>"id"</c> or <c>"class"</c>;
		/// - with flag <see cref="EFFlags.UIA"/>;
		/// - with the non-static <b>Find</b> function (searching in a UI element).
		/// 
		/// Can be path consisting of roles separated by "/". Examples:
		/// - <c>"web:DOCUMENT/div/LIST/LISTITEM/LINK"</c> - find LINK using its full path in web page.
		/// - <c>"web:/div/LIST//LINK"</c> - the empty parts mean 'any role'. For example don't need to specify DOCUMENT because in web pages the first part is always DOCUMENT (Firefox, Chrome) or PANE (IE).
		/// - <c>"web:/div/LIST[4]/LISTITEM[-1]/LINK"</c> - the 4 is 1-based index of div child from which to start searching (4-th, then 3-th, 5-th and so on). It can make faster. Negative means 'index from end', for example use -1 to search in reverse order. Flag <see cref="EFFlags.Reverse"/> is not applied to path parts with an index. If index is invalid, will use the nearest valid index.
		/// - <c>"web:/div/LIST[4!]/LISTITEM[-1!]/LINK"</c> - like the above, but the LIST must be exactly 4-th child (don't search 3-th, 5-th etc) and the LISTITEM must be the last child. This can be useful when waiting (uses less CPU), however useless if UI element indices in the window or web page change often.
		/// - <c>"web://[4]/[-1!]/[2]"</c> - index without role.
		/// - <c>"CLIENT/WINDOW/TREE/TREEITEM[-1]"</c> - path in window or control. The first path part is a direct child UI element of the WINDOW UI element of the window/control. The WINDOW UI element itself is not included in the search; if you need it, instead use <see cref="fromWindow"/>.
		/// 
		/// ##### About the <i>prop</i> parameter
		/// 
		/// Format: one or more <c>"name=value"</c>, separated with <c>"\0"</c> or <c>"\0 "</c>, like <c>"description=xxx\0 @href=yyy"</c>. Names must match case. Values of string properties are wildcard expressions.
		/// 
		/// - <c>"class"</c> - search only in child controls that have this class name (see <see cref="wnd.ClassName"/>).
		/// Cannot be used when searching in a UI element.
		/// - <c>"id"</c> - search only in child controls that have this id (see <see cref="wnd.ControlId"/>). If the value is not a number - Windows Forms control name (see <see cref="wnd.NameWinforms"/>); case-sensitive, not wildcard.
		/// Cannot be used when searching in a UI element.
		/// - <c>"value"</c> - <see cref="Value"/>.
		/// - <c>"description"</c> - <see cref="Description"/>.
		/// - <c>"state"</c> - <see cref="State"/>. List of states the UI element must have and/or not have.
		/// Example: <c>"state=CHECKED, FOCUSABLE, !DISABLED"</c>.
		/// Example: <c>"state=0x100010, !0x1"</c>.
		/// Will find UI element that has all states without <c>"!"</c> prefix and does not have any of states with <c>"!"</c> prefix.
		/// - <c>"rect"</c> - <see cref="Rect"/>. Can be specified left, top, width and/or height, using <see cref="RECT.ToString"/> format.
		/// Example: <c>"rect={L=1155 T=1182 W=132 H=13}"</c>.
		/// Example: <c>"rect={W=132 T=1182}"</c>.
		/// The L T coordinates are relative to the primary screen.
		/// - <c>"level"</c> - level (see <see cref="Level"/>) at which the UI element can be found. Can be exact level, or minimal and maximal level separated by space.
		/// The default value is 0 1000.
		/// Alternatively you can use path in role, like <c>"////LINK"</c>.
		/// - <c>"elem"</c> - <see cref="SimpleElementId"/>.
		/// - <c>"action"</c> - <see cref="DefaultAction"/>.
		/// - <c>"key"</c> - <see cref="KeyboardShortcut"/>.
		/// - <c>"help"</c> - <see cref="Help"/>.
		/// - <c>"uiaid"</c> - <see cref="UiaId"/>.
		/// - <c>"maxcc"</c> - when searching, skip children of UI elements that have more than this number of direct children. It can make faster.
		/// The default value is 10000. It also prevents hanging or crashing when a UI element in the UI element tree has large number of children. For example OpenOffice Calc TABLE has one billion children.
		/// - <c>"notin"</c> - when searching, skip children of UI elements that have these roles. It can make faster.
		/// Example: <c>"notin=TREE,LIST,TOOLBAR"</c>.
		/// Roles in the list must be separated with <c>","</c> or <c>", "</c>. Case-sensitive, not wildcard. See also: <see cref="EFFlags.MenuToo"/>.
		/// - <c>"@attr"</c> - <see cref="HtmlAttribute"/>. Here "attr" is any attribute name. Example: <c>"@id=example"</c>.
		/// </remarks>
		/// <example>
		/// Find link "Example" in web page, and click. Wait max 5 s. Throw <b>NotFoundException</b> if not found.
		/// <code><![CDATA[
		/// var w = +wnd.find("* Chrome");
		/// elm.find(5, w, "web:LINK", "Example").Invoke();
		/// ]]></code>
		/// Try to find link "Example" in web page. Return if not found.
		/// <code><![CDATA[
		/// var w = +wnd.find("* Chrome");
		/// var e = elm.find(w, "web:LINK", "Example");
		/// //var e = elm.find(-5, w, "web:LINK", "Example"); //waits max 5 s
		/// if(e == null) { print.it("not found"); return; }
		/// e.Invoke();
		/// ]]></code>
		/// Use <see cref="elmFinder"/>.
		/// <code><![CDATA[
		/// var w = +wnd.find("* Chrome");
		/// var f = new elmFinder("BUTTON", "Example");
		/// if(!f.Find(w)) { print.it("not found"); return; }
		/// elm e = f.Result;
		/// e.Invoke();
		/// ]]></code>
		/// </example>
		public static elm find(
			wnd w, string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0,
			Func<elm, bool> also = null, int skip = 0, string navig = null, wndChildFinder controls = null
			) {
			var f = new elmFinder(role, name, prop, flags, also, skip, navig);
			bool found = controls != null ? f.Find(w, controls) : f.Find(w);
			if (!found) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds a UI element in window. Can wait and throw <b>NotFoundException</b>.
		/// </summary>
		/// <returns>UI element. If not found, throws exception or returns null (if <i>waitS</i> negative).</returns>
		/// <param name="waitS">The wait timeout, seconds. If 0, does not wait. If negative, does not throw exception when not found.</param>
		/// <param name="w"></param>
		/// <param name="role"></param>
		/// <param name="name"></param>
		/// <param name="prop"></param>
		/// <param name="flags"></param>
		/// <param name="also"></param>
		/// <param name="skip"></param>
		/// <param name="navig"></param>
		/// <param name="controls"></param>
		/// <exception cref="ArgumentException" />
		/// <exception cref="AuWndException">Invalid window.</exception>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		/// <exception cref="NotFoundException" />
		/// <exception cref="NotSupportedException">Used <i>controls</i> with non-0 <i>waitS</i>.</exception>
		public static elm find(
			double waitS,
			wnd w, string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0,
			Func<elm, bool> also = null, int skip = 0, string navig = null, wndChildFinder controls = null
			) {
			elm r;
			if (waitS == 0) r = find(w, role, name, prop, flags, also, skip, navig, controls);
			else if (controls == null) r = wait(waitS < 0 ? waitS : -waitS, w, role, name, prop, flags, also, skip, navig);
			else throw new NotSupportedException("non-null controls with non-0 waitS"); //FUTURE?

			return r != null || waitS < 0 ? r : throw new NotFoundException();
		}
		//rejected: same for Find(). Rarely used, has Wait(), not used in the tool dialog.

		/// <summary>
		/// Finds and returns a descendant UI element of this UI element.
		/// Returns null if not found.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <exception cref="ArgumentException">Exceptions of other overload, plus:
		/// - <i>flags</i> has <see cref="EFFlags.UIA"/>.
		/// - <see cref="SimpleElementId"/> is not 0.
		/// </exception>
		/// <exception cref="AuException">Failed.</exception>
		public elm Find(string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0,
			Func<elm, bool> also = null, int skip = 0, string navig = null) {
			//info: f.Find will throw if this elm is invalid etc.

			var f = new elmFinder(role, name, prop, flags, also, skip, navig);
			if (!f.Find(this)) return null;
			return f.Result;
		}

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
		/// <summary>
		/// Finds UI element (UI element) in window. Waits until the UI element is found or the given time expires.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns the found UI element. On timeout returns null if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="AuWndException"/>
		/// <exception cref="AuException"/>
		public static elm wait(double secondsTimeout, wnd w, string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0,
			Func<elm, bool> also = null, int skip = 0, string navig = null) {
			var f = new elmFinder(role, name, prop, flags, also, skip, navig);
			if (!f.Wait(secondsTimeout, w)) return null;
			return f.Result;
		}

		/// <summary>
		/// Finds a descendant UI element of this UI element. Waits until the UI element is found or the given time expires.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <returns>Returns the found UI element. On timeout returns null if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="AuException"/>
		public elm Wait(double secondsTimeout, string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0,
			Func<elm, bool> also = null, int skip = 0, string navig = null) {
			//info: f.Find will throw if this elm is invalid etc.

			var f = new elmFinder(role, name, prop, flags, also, skip, navig);
			if (!f.Wait(secondsTimeout, this)) return null;
			return f.Result;
		}
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

		/// <summary>
		/// Finds all matching UI elements in window.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <returns>Array of 0 or more elements.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="AuWndException"/>
		/// <exception cref="AuException"/>
		/// <example>
		/// Get all taskbar buttons (Windows 10).
		/// <code><![CDATA[
		/// var w = +wnd.find(null, "Shell_TrayWnd");
		/// foreach(var e in elm.findAll(w, "BUTTON", prop: "level=7")) print.it(e);
		/// ]]></code>
		/// </example>
		public static elm[] findAll(wnd w, string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0, Func<elm, bool> also = null) {
			var a = new List<elm>();
			find(w, role, name, prop, flags, o => {
				if (also == null || also(o)) a.Add(o);
				return false;
			});
			return a.ToArray();
		}

		/// <summary>
		/// Finds all matching descendant UI elements of this UI element.
		/// More info: <see cref="find"/>.
		/// </summary>
		/// <returns>Array of 0 or more elements.</returns>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="AuException"/>
		/// <example>
		/// Get all taskbar buttons (Windows 10).
		/// <code><![CDATA[
		/// var w = +wnd.find(null, "Shell_TrayWnd");
		/// var etb = +elm.find(w, "TOOLBAR", "Running applications");
		/// foreach(var e in etb.FindAll("BUTTON", prop: "level=0")) print.it(e);
		/// ]]></code>
		/// </example>
		public elm[] FindAll(string role = null,
			[ParamString(PSFormat.wildex)] string name = null,
			string prop = null, EFFlags flags = 0, Func<elm, bool> also = null) {
			var a = new List<elm>();
			Find(role, name, prop, flags, o => {
				if (also == null || also(o)) a.Add(o);
				return false;
			});
			return a.ToArray();
		}

		/// <summary>
		/// Returns the same value if it is not null. Else throws <see cref="NotFoundException"/>.
		/// </summary>
		/// <exception cref="NotFoundException"></exception>
		/// <example>
		/// <code><![CDATA[
		/// var w = +wnd.find("Example");
		/// var e1 = +elm.find(w, "web:LINK", "Example");
		/// var e2 = +elm.find(w, ...)?.Find(...);
		/// ]]></code>
		/// </example>
		public static elm operator +(elm e) => e ?? throw new NotFoundException("Not found (elm).");
	}
}
