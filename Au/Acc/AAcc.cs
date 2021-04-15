using Au.Types;
using Au.Util;
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

namespace Au
{
	/// <summary>
	/// Accessible object.
	/// Finds controls and smaller objects in a window or web page. Clicks, gets properties, etc.
	/// </summary>
	/// <remarks>
	/// Accessible objects (AO) are various user interface (UI) objects in windows and controls. For example buttons, links, list items. This class can find them, get properties, click, etc. Especially useful with web pages, because there are no controls. And many other windows don't use controls but support AO. But not all UI objects are AO.
	/// 
	/// An <b>AAcc</b> instance holds an AO COM pointer (<msdn>IAccessible</msdn>) and a simple element id (int). Most <b>AAcc</b> functions wrap <b>IAccessible</b> interface functions or/and related API.
	/// 
	/// <b>AAcc</b> functions that get properties don't throw exception when the wrapped <b>IAccessible</b>/etc function failed (returned an error code of <b>HRESULT</b> type). Then they return "" (string properties), 0, false, null or empty collection, dependin on return type. Applications implement AOs differently, often with bugs, and their <b>IAccessible</b> interface functions return a variety of error codes. It's impossible to reliably detect whether the error code means a serious error or the property is merely unavailable. These <b>AAcc</b> functions also set the last error code of this thread = the return value (<b>HRESULT</b>) of the <b>IAccessible</b> function, and callers can use <see cref="ALastError"/> to get it. If <b>ALastError.Code</b> returns 1 (<b>S_FALSE</b>), in most cases it's not an error, just the property is unavailable. On error it will probably be a negative error code.
	/// 
	/// You can dispose <b>AAcc</b> variables to release the COM object, but it is not necessary (GC will do it later).
	/// 
	/// An <b>AAcc</b> variable cannot be used in multiple threads. Only <b>Dispose</b> can be called in any thread.
	/// 
	/// AOs are implemented and live in their applications. This class just communicates with them.
	/// 
	/// Many applications have various problems with their AOs: bugs, incorrect/nonstandard/partial implementation, or initially disabled. This class implements workarounds for known problems, where possible.
	/// 
	/// <a data-toggle="collapse" data-target="#collapse1" aria-expanded="false" aria-controls="collapse1">Known problematic applications</a>
	/// <div class="collapse" id="collapse1">
	/// <table>
	/// <tr>
	/// <th>Application</th>
	/// <th>Problems</th>
	/// </tr>
	/// <tr>
	///  <td>Chrome web browser. Also Opera and other apps that use Chrome code. Window class name is like "Chrome_WidgetWin_1".</td>
	///  <td>
	///   <ol>
	///    <li>Web page AOs initially are disabled(missing). Workarounds:
	///     <ul>
	///      <li>Functions Find, Wait and FindAll enable it if used role prefix "web:" or "chrome:". Functions FromXY, FromMouse and Focused enable it if window class name starts with "Chrome". However Chrome does it lazily, therefore first time the functions often get wrong AO. Note: this auto-enabing may fail with future Chrome versions.</li>
	///      <li>Start Chrome with command line --force-renderer-accessibility.</li>
	///      <li>In the future the script editor will have an option to enable Chrome AOs when it starts.</li>
	///     </ul>
	///    </li>
	///    <li>Some new web browser versions add new features or bugs that break something.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>Firefox web browser.</td>
	///  <td>
	///   <ol>
	///    <li>By default, the Find function is about 50 times slower than it could be. Also for this reason the Wait function consumes much CPU. And HTML attributes may be unavailable. See <see cref="AFFlags.NotInProc"/>. Workaround: disable the Firefox multiprocess feature: set system environment variable MOZ_FORCE_DISABLE_E10S=1 and restart Firefox. Note: Firefox may remove this option in the future. If this does not work, google how to disable Firefox multiprocess. Or use Chrome instead.</li>
	///    <li>When Firefox starts, its web page AOs are unavailable. It creates them only when somebody asks (eg function Find), but does it lazily, and Find at first fails. Workaround: use Wait, not Find.</li>
	///    <li>Ocassionally Firefox briefly turns off its web page AOs. Workaround: use Wait, not Find. With other web browsers also it's better to use Wait.</li>
	///    <li>Some new web browser versions add new features or bugs that break something.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	/// <td>JavaFX and other applications that don't have true accessible objects but have UI Automation elements.</td>
	///  <td>
	///   <ol>
	///    <li>To find AOs in these applications, need flag <see cref="AFFlags.UIA"/>.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>Java applications that use AWT/Swing. Window class name starts with "SunAwt".</td>
	///  <td>
	///   <ol>
	///    <li>Not supported on 32-bit OS.</li>
	///    <li>Must be enabled Java Access Bridge (JAB).<br/>If JAB is disabled or does not work, the "Find accessible object" tool shows an "enable" link when you try to capture something in a Java window. Or you can enable JAB in Control Panel -> Ease of Access Center -> Use the computer without a display. Or use jabswitch.exe. Then restart Java apps. Also may need to restart apps that tried to use Java AOs.</li>
	///    <li>Your process must have the same 32/64 bitness as the installed Java. To remove this limitation, install Java 32-bit and 64-bit (they coexist).</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>OpenOffice.</td>
	///  <td>
	///   <ol>
	///    <li>Often crashes after using AOs, usually when closing. Noticed in OpenOffice 4.1.4; may be fixed in newer versions.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>LibreOffice.</td>
	///  <td>
	///   <ol>
	///    <li>AOs are unavailable unless this process is 32-bit (when LibreOffice is 64-bit). Also need flag <see cref="AFFlags.NotInProc"/>.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>Some controls.</td>
	///  <td>
	///   <ol>
	///    <li>AOs of some controls are not connected to the AO of the parent control. Then Find cannot find them if searches in whole window.<br/>Workaround: search only in that control. For example, use <i>prop</i> <c>"class"</c> or <c>"id"</c>. If it's a web browser control, use role prefix <c>"web:"</c>. Or find the control with <see cref="AWnd.Child"/> and search in it. Or use <see cref="AAcc.Finder.Find(AWnd, AWnd.ChildFinder)"/>.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>Some controls with flag <see cref="AFFlags.NotInProc"/>.</td>
	///  <td>
	///   AOs of many standard Windows controls have bugs when they are retrieved without loading dll into the target process (see <see cref="AFFlags.NotInProc"/>). Known bugs:
	///   <ol>
	///    <li>Toolbar buttons don't have Name in some cases.</li>
	///    <li><see cref="Focus"/> and <see cref="Select"/> often don't work properly.</li>
	///   </ol>
	///   Workarounds: Don't use <see cref="AFFlags.NotInProc"/>. Or use <see cref="AFFlags.UIA"/>.
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>When cannot load dll into the target process. For example Windows Store apps.</td>
	///  <td>
	///   <ol>
	///    <li>Function Find is much slower. Function Wait then consumes much more CPU. More info: <see cref="AFFlags.NotInProc"/>.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>Processes of a different 32/64 bitness than this process.</td>
	///  <td>
	///   <ol>
	///    <li>To load the dll is used rundll32.exe, which makes slower by about 50 ms first time.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// <tr>
	///  <td>DPI-scaled windows.</td>
	///  <td>
	///   <ol>
	///    <li>Currently this library does not support auto-scaled windows when using high DPI (text size 125%, 150% or more). If the target process is auto-scaled and this process isn't (or vice versa, or they have a different scaling factor), most coordinate-related functions don't work properly. For example, they get wrong AO rectangles.</li>
	///   </ol>
	///  </td>
	/// </tr>
	/// </table>
	/// </div>
	/// </remarks>
	/// <example>
	/// Click link "Example" in Chrome.
	/// <code><![CDATA[
	/// var w = +AWnd.Find("* Chrome");
	/// var a = +AAcc.Find(w, "web:LINK", "Example");
	/// a.DoAction();
	/// ]]></code>
	/// Click a link, wait for new web page, click a link in it.
	/// <code><![CDATA[
	/// var w = +AWnd.Find("* Chrome");
	/// var a = AAcc.Wait(1, w, "web:LINK", "Link 1");
	/// a.DoActionAndWaitForNewWebPage();
	/// a = AAcc.Wait(10, w, "web:LINK", "Link 2");
	/// a.DoActionAndWaitForNewWebPage();
	/// ]]></code>
	/// </example>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe sealed partial class AAcc : IDisposable
	{
		//FUTURE: AAcc.More.EnableAccInChromeWebPagesWhenItStarts
		//FUTURE: AAcc.More.EnableAccInJavaWindows (see JavaEnableJAB in QM2)
		//FUTURE: add functions to marshal AO to another thread.

		internal struct Misc_
		{
			public AccMiscFlags flags;
			public byte role; //for optimization. 0 if not set or failed to get or VT_BSTR or does not fit in BYTE.
			public ushort level; //for ToString. 0 if not set.

			public void SetRole(AccROLE role) { this.role = (byte)((uint)role <= 0xff ? role : 0); }
			public void SetLevel(int level) { this.level = (ushort)Math.Clamp(level, 0, 0xffff); }
		}

		internal IntPtr _iacc;
		internal int _elem;
		internal Misc_ _misc;
		//Real AAcc object memory size with header: 32 bytes on 64-bit.
		//We don't use RCW<IAccessible>, which would add another 32 bytes.

		/// <summary>
		/// Creates AAcc from IAccessible and child id.
		/// By default does not AddRef.
		/// iacc must not be Is0.
		/// </summary>
		internal AAcc(IntPtr iacc, int elem = 0, bool addRef = false) {
			_Set(iacc, elem, default, addRef);
		}

		/// <summary>
		/// Creates AAcc from Cpp_Acc.
		/// By default does not AddRef.
		/// x.acc must not be Is0.
		/// </summary>
		internal AAcc(Cpp.Cpp_Acc x, bool addRef = false) {
			_Set(x.acc, x.elem, x.misc, addRef);
		}

		/// <summary>
		/// Sets fields.
		/// _iacc must be 0, iacc not 0.
		/// </summary>
		void _Set(IntPtr iacc, int elem = 0, Misc_ misc = default, bool addRef = false) {
			Debug.Assert(_iacc == default);
			Debug.Assert(iacc != default);
			if (addRef) Marshal.AddRef(iacc);
			_iacc = iacc;
			_elem = elem;
			_misc = misc;

			int mp = _MemoryPressure;
			GC.AddMemoryPressure(mp);
			//s_dmp += mp; if(s_dmp > DebugMaxMemoryPressure) DebugMaxMemoryPressure = s_dmp;
			//DebugMemorySum += mp;
		}

		int _MemoryPressure => _elem == 0 ? c_memoryPressure : c_memoryPressure / 10;
		const int c_memoryPressure = 500; //Ideally this should be the average AO memory size, if counting both processes.

		//internal static int DebugMaxMemoryPressure;
		//static int s_dmp;
		//internal static int DebugMemorySum;

		/// <summary>
		/// Releases IAccessible COM object and clears this variable.
		/// </summary>
		public void Dispose() {
			if (_iacc != default) {
				var t = _iacc; _iacc = default;
				//APerf.First();
				Marshal.Release(t);
				//APerf.NW();
				//AOutput.Write($"rel: {Marshal.Release(t)}");

				int mp = _MemoryPressure;
				GC.RemoveMemoryPressure(mp);
				//s_dmp -= mp;
			}
			_elem = 0;
			_misc = default;
			GC.SuppressFinalize(this);
		}

		///
		~AAcc() {
			Dispose();
		}

		/// <summary>
		/// Gets or changes simple element id, also known as child id.
		/// </summary>
		/// <remarks>
		/// Most accessible objects (AO) are not simple elements. Then this property is 0.
		/// Often (but not always) this property is the 1-based item index in parent AO. For example LISTITEM in LIST.
		/// The 'set' function sometimes can be used as a fast alternative to <see cref="Navigate"/>. It modifies only this variable. It does not check whether the value is valid.
		/// Simple elements cannot have child AOs.
		/// </remarks>
		public int SimpleElementId { get => _elem; set { _misc.role = 0; _elem = value; } }

		/// <summary>
		/// Returns some additional info about this variable, such as how the accessible object was retrieved (inproc, UIA, Java).
		/// </summary>
		public AccMiscFlags MiscFlags => _misc.flags;

		/// <summary>
		/// Gets or sets indentation level for <see cref="ToString"/>.
		/// </summary>
		/// <remarks>
		/// When Find or similar function finds an accessible object, it sets this property of the AAcc variable. If FromXY etc, it is 0 (unknown).
		/// When searching in a window, at level 0 are direct children of the WINDOW object. When searching in controls (specified class or id), at level 0 is the object of the control; however if used path, at level 0 are direct children. When searching in AAcc, at level 0 are direct children of the AAcc. When searching in web page (role prefix <c>"web:"</c> etc), at level 0 is the web page object (role DOCUMENT or PANE).
		/// </remarks>
		public int Level { get => _misc.level; set => _misc.SetLevel(value); }

		/// <summary>
		/// Returns true if this variable is disposed.
		/// </summary>
		bool _Disposed => _iacc == default;

		internal void ThrowIfDisposed_() {
			if (_Disposed) throw new ObjectDisposedException(nameof(AAcc));
		}

		/// <summary>
		/// Gets accessible object of window or control or its standard part - client area, titlebar etc.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="objid">Window part id. Default AccOBJID.WINDOW. Also can be a custom id supported by that window, cast int to AccOBJID.</param>
		/// <param name="flags">Flags.</param>
		/// <exception cref="AuWndException">Invalid window.</exception>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		/// <exception cref="ArgumentException"><i>objid</i> is QUERYCLASSNAMEIDX or NATIVEOM.</exception>
		/// <remarks>
		/// Uses API <msdn>AccessibleObjectFromWindow</msdn>.
		/// </remarks>
		public static AAcc FromWindow(AWnd w, AccOBJID objid = AccOBJID.WINDOW, AWFlags flags = 0) {
			bool spec = false;
			switch (objid) {
			case AccOBJID.QUERYCLASSNAMEIDX: //use WM_GETOBJECT
			case AccOBJID.NATIVEOM: //use API AccessibleObjectFromWindow
				throw new ArgumentException();
			case AccOBJID.CARET: //w should be 0
			case AccOBJID.CURSOR: //w should be 0
			case AccOBJID.ALERT: //only with AccessibleObjectFromEvent?
			case AccOBJID.SOUND: //only with AccessibleObjectFromEvent?
				spec = true; flags |= AWFlags.NotInProc;
				break;
			}

			var hr = Cpp.Cpp_AccFromWindow(flags.Has(AWFlags.NotInProc) ? 1 : 0, w, objid, out var a, out _);
			if (hr != 0) {
				if (flags.Has(AWFlags.NoThrow)) return null;
				if (spec && w.Is0) throw new AuException();
				w.ThrowIfInvalid();
				_WndThrow(hr, w, "*get accessible object from window.");
			}
			return new AAcc(a);
		}

		static void _WndThrow(int hr, AWnd w, string es) {
			w.UacCheckAndThrow_(es);
			throw new AuException(hr, es);
		}

		/// <summary>
		/// Gets accessible object from point.
		/// </summary>
		/// <param name="p">
		/// Coordinates.
		/// Tip: To specify coordinates relative to the right, bottom, work area or a non-primary screen, use <see cref="Coord.Normalize"/>, like in the example.
		/// </param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		/// <remarks>
		/// Uses API <msdn>AccessibleObjectFromPoint</msdn>.
		/// </remarks>
		/// <example>
		/// Find object at 100 200.
		/// <code><![CDATA[
		/// var a = AAcc.FromXY((100, 200));
		/// AOutput.Write(a);
		/// ]]></code>
		/// 
		/// Find object at 50 from left and 100 from bottom of the work area.
		/// <code><![CDATA[
		/// var a = AAcc.FromXY(Coord.Normalize(50, Coord.Reverse(100), true));
		/// AOutput.Write(a);
		/// ]]></code>
		/// </example>
		public static AAcc FromXY(POINT p, AXYFlags flags = 0) {
			for (int i = 0; ; i++) {
				var hr = Cpp.Cpp_AccFromPoint(p, flags, out var a);
				if (hr == 0) return new AAcc(a);
				if (i < 2) continue;
				if (flags.Has(AXYFlags.NoThrow)) return null;
				_WndThrow(hr, AWnd.FromXY(p, WXYFlags.Raw), "*get accessible object from point.");
			}
		}
		//rejected: FromXY(Coord, Coord, ...). Coord makes no sense; could be int int, but it's easy to create POINT from it.

		/// <summary>
		/// Gets accessible object from mouse cursor (pointer) position.
		/// </summary>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed. For example, window of a higher [](xref:uac) integrity level process.</exception>
		/// <remarks>
		/// Uses API <msdn>AccessibleObjectFromPoint</msdn>.
		/// </remarks>
		public static AAcc FromMouse(AXYFlags flags = 0) {
			return FromXY(AMouse.XY, flags);
		}

		/// <summary>
		/// Gets the accessible object that has the keyboard focus.
		/// Returns null if fails.
		/// </summary>
		/// <param name="useUIAutomation">
		/// Use UI Automation API.
		/// Need this with windows that don't support accessible objects but support UI Automation elements. Can be used with most other windows too.
		/// More info: <see cref="AFFlags.UIA"/>.
		/// </param>
		public static AAcc Focused(bool useUIAutomation = false) {
			var w = AWnd.Focused;
			g1:
			if (w.Is0) return null;
			int hr = Cpp.Cpp_AccGetFocused(w, useUIAutomation ? 1 : 0, out var a);
			if (hr != 0) {
				var w2 = AWnd.Focused;
				if (w2 != w) { w = w2; goto g1; }
				return null;
			}
			return new AAcc(a);
			//FUTURE: wait, like FromXY.
		}

		/// <summary>
		/// Gets the accessible object that generated the event that is currently being processed by the callback function used with API <msdn>SetWinEventHook</msdn> or <see cref="AHookAcc"/>.
		/// Returns null if failed. Suports <see cref="ALastError"/>.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="idObject"></param>
		/// <param name="idChild"></param>
		/// <remarks>
		/// The parameters are of the callback function.
		/// Uses API <msdn>AccessibleObjectFromEvent</msdn>.
		/// Often fails because the object already does not exist, because the callback function is called asynchronously, especially when the event is OBJECT_DESTROY, OBJECT_HIDE, SYSTEM_*END.
		/// Returns null if failed. Always check the return value, to avoid NullReferenceException. An exception in the callback function kills this process.
		/// </remarks>
		public static AAcc FromEvent(AWnd w, AccOBJID idObject, int idChild) {
			int hr = Api.AccessibleObjectFromEvent(w, idObject, idChild, out var iacc, out var v);
			if (hr == 0 && iacc == default) hr = Api.E_FAIL;
			if (hr != 0) { ALastError.Code = hr; return null; }
			int elem = v.vt == Api.VARENUM.VT_I4 ? v.ValueInt : 0;
			return new AAcc(iacc, elem);
		}

#if false //rejected: not useful. Maybe in the future.
		/// <summary>
		/// Gets accessible object from a COM object of any type that supports it.
		/// Returns null if fails.
		/// </summary>
		/// <param name="x">Unmanaged COM object.</param>
		/// <remarks>
		/// The COM object type can be IAccessible, IAccessible2, IHTMLElement, ISimpleDOMNode or any other COM interface type that can give <msdn>IAccessible</msdn> interface pointer through API <msdn>IUnknown.QueryInterface</msdn> or <msdn>IServiceProvider.QueryService</msdn>.
		/// For IHTMLElement and ISimpleDOMNode returns null if the HTML element is not an accessible object. Then you can try to get accessible object of its parent HTML element, parent's parent and so on, until succeeds.
		/// </remarks>
		public static AAcc FromComObject(IntPtr x)
		{
			if(x == default) return null;
			if(AMarshal.QueryInterface(x, out IntPtr iacc, Api.IID_IAccessible)
				|| AMarshal.QueryService(x, out iacc, Api.IID_IAccessible)
				) return new AAcc(iacc);
			return null;
		}

		/// <summary>
		/// Gets accessible object from a COM object of any type that supports it.
		/// Returns null if fails.
		/// </summary>
		/// <param name="x">Managed COM object.</param>
		/// <remarks>
		/// The COM object type can be IAccessible, IAccessible2, IHTMLElement, ISimpleDOMNode or any other COM interface type that can give <msdn>IAccessible</msdn> interface pointer through API <msdn>IUnknown.QueryInterface</msdn> or <msdn>IServiceProvider.QueryService</msdn>.
		/// For IHTMLElement and ISimpleDOMNode returns null if the HTML element is not an accessible object. Then you can try to get accessible object of its parent HTML element, parent's parent and so on, until succeeds.
		/// </remarks>
		public static AAcc FromComObject(object x)
		{
			if(x == null) return null;

			//FUTURE: support UIA. Don't use LegacyIAccessible, it work not with all windows. Instead wrap in UIAccessible.
			//if(x is UIA.IElement e) { //info: IElement2-7 are IElement too
			//	var pat = e.GetCurrentPattern(UIA.PatternId.LegacyIAccessible) as UIA.ILegacyIAccessiblePattern;
			//	x = pat?.GetIAccessible();
			//	if(x == null) return null;
			//}

			var ip = Marshal.GetIUnknownForObject(x);
			if(ip == default) return null;
			try { return FromComObject(ip); }
			finally { Marshal.Release(ip); }
		}
#endif

		/// <summary>
		/// Used only for debug.
		/// </summary>
		enum _FuncId { name = 1, value, description, default_action, role, state, rectangle, parent_object, child_object, container_window, child_count, child_objects, help_text, keyboard_shortcut, html, selection, uiaid }

		/// <summary>
		/// Calls Native.SetError and returns hr.
		/// In Debug config also outputs error in red.
		/// If hr looks like not an error but just the property or action is unavailable, changes it to S_FALSE and does not show error. These are: S_FALSE, DISP_E_MEMBERNOTFOUND, E_NOTIMPL.
		/// _FuncId also can be char, like (_FuncId)'n' for name.
		/// </summary>
		int _Hresult(_FuncId funcId, int hr) {
			if (hr != 0) {
				switch (hr) {
				case Api.DISP_E_MEMBERNOTFOUND: case Api.E_NOTIMPL: hr = Api.S_FALSE; break;
				case (int)Cpp.EError.InvalidParameter: throw new ArgumentException("Invalid argument value.");
				default: Debug.Assert(!Cpp.IsCppError(hr)); break;
				}
#if DEBUG
				if (hr != Api.S_FALSE) {
					_DebugPropGet(funcId, hr);
				}
#endif
			}
			ALastError.Code = hr;
			return hr;
		}

#if DEBUG
		void _DebugPropGet(_FuncId funcId, int hr) {
			if (t_debugNoRecurse || _Disposed) return;

			if (funcId >= (_FuncId)'A') {
				switch ((char)funcId) {
				case 'R': funcId = _FuncId.role; break;
				case 'n': funcId = _FuncId.name; break;
				case 'v': funcId = _FuncId.value; break;
				case 'd': funcId = _FuncId.description; break;
				case 'h': funcId = _FuncId.help_text; break;
				case 'a': funcId = _FuncId.default_action; break;
				case 'k': funcId = _FuncId.keyboard_shortcut; break;
				case 's': funcId = _FuncId.state; break;
				case 'r': funcId = _FuncId.rectangle; break;
				case 'u': funcId = _FuncId.uiaid; break;
				}
			}

			if (hr == Api.E_FAIL && funcId == _FuncId.default_action) return; //many in old VS etc
			t_debugNoRecurse = true;
			try {
				var s = ToString();
				AOutput.Write($"<><c 0xff>-{funcId}, 0x{hr:X} - {ALastError.MessageFor(hr)}    {s}</c>");
			}
			finally { t_debugNoRecurse = false; }
		}
		[ThreadStatic] static bool t_debugNoRecurse;
#endif

		/// <summary>
		/// Formats string from main properties of this accessible object.
		/// </summary>
		/// <remarks>
		/// The string starts with role. Other properties have format like <c>x="value"</c>, where x is a property character like with <see cref="GetProperties"/>; character e is <see cref="SimpleElementId"/>. HTML attributes have format <c>@name="value"</c>. In string values are used C# escape sequences, for example \r\n for new line.
		/// Indentation depends on <see cref="Level"/>.
		/// </remarks>
		/// <seealso cref="PrintAll"/>
		public override string ToString() {
			if (_Disposed) return "<disposed>";
			if (!GetProperties("Rnsvdarw@", out var k)) return "<failed>";

			using (new StringBuilder_(out var b)) {
				if (Level > 0) b.Append(' ', Level);
				b.Append(k.Role);
				_Add('n', k.Name);
				if (k.State != 0) _Add('s', k.State.ToString(), '(', ')');
				_Add('v', k.Value);
				_Add('d', k.Description);
				_Add('a', k.DefaultAction);
				if (!k.Rect.Is0) _Add('r', k.Rect.ToString(), '\0', '\0');
				if (SimpleElementId != 0) b.Append(",  e=").Append(SimpleElementId);
				foreach (var kv in k.HtmlAttributes) {
					b.Append(",  @").Append(kv.Key).Append('=').Append('\"');
					b.Append(kv.Value.Escape(limit: 250)).Append('\"');
				}
				_Add('w', k.WndContainer.ClassName ?? "");

				void _Add(char name, string value, char q1 = '\"', char q2 = '\"') {
					if (value.Length == 0) return;
					var t = value; if (q1 == '\"') t = t.Escape(limit: 250);
					b.Append(",  ").Append(name).Append('=');
					if (q1 != '\0') b.Append(q1);
					b.Append(t);
					if (q1 != '\0') b.Append(q2);
				}

				return b.ToString();
			}
		}

		/// <summary>
		/// Displays properties of all found accessible objects of window w.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="ToString"/>.
		/// Catches exceptions. On exception writes to the output: <c>$"!exception! exceptionType exceptionMessage"</c>.
		/// Parameters are of <see cref="Find"/>.
		/// By default skips invisible objects and objects in menus. Use flags to include them.
		/// Chrome web page accessible objects normally are disabled (missing) when it starts. Use role prefix <c>"web:"</c> or <c>"chrome:"</c> to enable. See example.
		/// </remarks>
		/// <example>
		/// Displays visible accessible objects in Chrome web page.
		/// <code><![CDATA[
		/// AOutput.Clear();
		/// var w = +AWnd.Find("* Chrome");
		/// AOutput.Write("---- all ----");
		/// AAcc.PrintAll(w, "web:");
		/// AOutput.Write("---- links ----");
		/// AAcc.PrintAll(w, "web:LINK");
		/// ]]></code>
		/// </example>
		public static void PrintAll(AWnd w, string role = null, AFFlags flags = 0, string prop = null) {
			try {
				Find(w, role, null, prop, flags, also: o => { AOutput.Write(o); return false; });
			}
			catch (Exception ex) { AOutput.Write($"!exception! {ex.ToStringWithoutStack()}"); }
		}
	}
}
