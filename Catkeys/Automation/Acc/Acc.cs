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

using UIA = UIAutomationClient;

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Accessible object.
	/// </summary>
	/// <remarks>
	/// Accessible objects are various user interface (UI) objects in windows and controls. For example buttons, links, list items. This class can find them, get properties, click, etc. Especially useful with web pages, because there are no controls. And many other windows don't use controls but support accessible objects. But not all UI objects are accessible objects.
	/// 
	/// An Acc instance holds an accessible object COM pointer (<msdn>IAccessible</msdn>) and a child element id (int). Most Acc functions wrap IAccessible interface functions or/and related API.
	/// 
	/// Acc functions that get properties don't throw exception when the wrapped IAccessible/etc function failed (returned an error code of HRESULT type). Then they return default value of that type (null, 0, false); string property getters return "". Various accessible objects in various applications are implemented differently, often with bugs, and their IAccessible interface functions return a variety of error codes. It's impossible to reliably detect whether the error code means a serious error or the property is merely unavailable. These Acc functions also set the last error code of this thread = the return value (HRESULT) of the IAccessible function, and callers can use <see cref="Native.GetError"/> to get it. If Native.GetError returns 1 (S_FALSE), in most cases it's not an error, just the property is unavailable. On error it will probably be a negative error code.
	/// 
	/// Disposing Acc variables (to release the COM object) is not necessary (GC will do it later), but in libraries it is recommended.
	/// 
	/// Supports accessible objects of Java windows if the Java Access Bridge (JAB) is enabled. You can enable/disable it in Control Panel -> Ease of Access Center -> Use the computer without a display. Or use jabswitch.exe.
	/// </remarks>
	public unsafe partial class Acc :IDisposable, ISupportOrThrow
	{
		//tested: Acc object memory size with overhead: 32 bytes. Note: we don't use RCW<IAccessible>, which would add another 32 bytes.

		internal IAccessible _iacc;
		internal int _elem;
		internal AccROLE _role; //store role because it is slow to get and used in several places. Does not make real object memory bigger (still 32 bytes).

		/// <summary>
		/// Used only by the AFAcc class which is derived from Acc.
		/// </summary>
		internal Acc()
		{
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Creates Acc from IAccessible and child id.
		/// By default does not AddRef.
		/// iacc must not be Is0.
		/// role can be 0, it possibly will be set later.
		/// </summary>
		internal Acc(IAccessible iacc, int elem = 0, AccROLE role = 0, bool addRef = false)
		{
			_Set(iacc, elem, role, addRef);
		}

		/// <summary>
		/// Clones Acc a.
		/// Calls _iacc.AddRef.
		/// </summary>
		internal Acc(Acc a)
		{
			_Set(a._iacc, a._elem, a._role, true);
		}

		/// <summary>
		/// Sets fields.
		/// _iacc must be Is0.
		/// </summary>
		void _Set(IAccessible iacc, int elem = 0, AccROLE role = 0, bool addRef = false)
		{
			Debug.Assert(_iacc.Is0);
			Debug.Assert(!iacc.Is0);
			if(addRef) iacc.AddRef();
			_iacc = iacc;
			_elem = elem;
			_role = role;
		}

		/// <summary>
		/// Clears all fields and optionally calls _iacc.Release().
		/// The same as _Dispose, but does not call GC.SuppressFinalize.
		/// </summary>
		void _Clear(bool doNotRelease = false)
		{
			if(!_iacc.Is0) {
				var t = _iacc; _iacc = default;
				if(!doNotRelease) t.Release();
			}
			_elem = 0;
			_role = 0;
		}

		void _Dispose(bool doNotRelease = false)
		{
			_Clear(doNotRelease);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases IAccessible COM object and clears this variable.
		/// </summary>
		public void Dispose()
		{
			_Dispose();
		}

		///
		~Acc()
		{
			//Debug_.Print("Acc not disposed: " + ToString()); //cannot get props in other thread
			Debug_.Print("Acc not disposed: " + (_role != 0 ? RoleString : ("_Disposed=" + _Disposed.ToString()))); //note: with debugger somehow can be called several times, then Is0 true
			_Dispose();
		}

#if DEBUG
		/// <summary>
		/// The IAccessible that is managed by this variable.
		/// This func does not increment ref count etc. It simply returns the field. Don't release it. It will be released by Dispose.
		/// </summary>
		internal IAccessible LibIAccessibleDebug { get => _iacc; }
#endif

		/// <summary>
		/// Gets or sets child element id.
		/// </summary>
		public int Elem { get => _elem; set { _role = 0; _elem = value; } }

		/// <summary>
		/// Returns true if this variable is disposed.
		/// </summary>
		bool _Disposed { get => _iacc.Is0; }
		//note: named not 'IsDisposed' because can be easily confused with IsDisabled.

		/// <summary>
		/// Gets accessible object of window or control or its standard part - client area, titlebar etc.
		/// Uses API <msdn>AccessibleObjectFromWindow</msdn>.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="objid">Window part id. Default WINDOW (object of w itself). Can be one of enum AccOBJID constants. Also can be a custom id supported by that window, cast int to AccOBJID.</param>
		/// <param name="noThrow">Don't throw exception when fails. Then returns null.</param>
		/// <exception cref="WndException">Invalid window.</exception>
		/// <exception cref="CatException">Failed. For example, window of a higher UAC integrity level process.</exception>
		public static Acc FromWindow(Wnd w, AccOBJID objid = AccOBJID.WINDOW, bool noThrow = false)
		{
			var iacc = _FromWindow(w, objid, noThrow); if(iacc.Is0) return null;
			return new Acc(iacc);
		}

		static IAccessible _FromWindow(Wnd w, AccOBJID objid = AccOBJID.WINDOW, bool noThrow = false)
		{
			using(new _TempSetScreenReader(false)) {
				var hr = Api.AccessibleObjectFromWindow(w, objid, ref Api.IID_IAccessible, out var iacc);
				if(hr == 0 && iacc.Is0) hr = Api.E_FAIL;
				if(hr != 0) {
					if(noThrow) return default;
					w.ThrowIfInvalid();
					_WndThrow(hr, w, "*get accessible object from window");
				}
				return iacc;
			}
		}

		static void _WndThrow(int hr, Wnd w, string es)
		{
			if(hr == 0) return;
			//note: hr is random when blocked by UAC. Can be E_ACCESSDENIED, E_FAIL, E_OUTOFMEMORY.
			if(!w.Is0 && w.IsUacAccessDenied) es += ". Window of admin process (UAC). Run this app as admin or uiAccess";
			throw new CatException(hr, es);
		}

		/// <summary>
		/// Gets accessible object from point.
		/// Uses API <msdn>AccessibleObjectFromPoint</msdn>.
		/// </summary>
		/// <param name="x">X coordinate in screen.</param>
		/// <param name="y">Y coordinate in screen.</param>
		/// <param name="workArea">x y are relative to the work area, not entire screen.</param>
		/// <param name="screen">Screen of x y. If null, primary screen. See <see cref="Screen_.FromObject"/>.</param>
		/// <param name="noThrow">Don't throw exception when fails. Then returns null.</param>
		/// <param name="preferLINK">
		/// Get the parent object if it's LINK or PUSHBUTTON and this object is TEXT, STATICTEXT or GRAPHIC.
		/// Usually links have one or more children of type TEXT, GRAPHIC or other, and they are rarely used for automation.
		/// Note: This does not work if the object from point is a LINK's grandchild.
		/// Note: Some Chrome versions have this bug: the parent object does not support <see cref="WndContainer"/>.
		/// </param>
		/// <exception cref="CatException">Failed. For example, window of a higher UAC integrity level process.</exception>
		public static Acc FromXY(Coord x, Coord y, bool workArea = false, object screen = null, bool noThrow = false, bool preferLINK = false)
		{
			if(x.IsEmpty || y.IsEmpty) throw new ArgumentNullException();
			var p = Coord.Normalize(x, y, workArea, screen);
			return _FromPoint(p, noThrow, preferLINK);
		}

		/// <summary>
		/// Gets accessible object from mouse cursor position.
		/// Uses API <msdn>AccessibleObjectFromPoint</msdn>.
		/// </summary>
		/// <param name="noThrow">Don't throw exception when fails. Then returns null.</param>
		/// <param name="preferLINK">
		/// Get the parent object if it's LINK or PUSHBUTTON and this object is TEXT, STATICTEXT or GRAPHIC.
		/// Usually links have one or more children of type TEXT, GRAPHIC or other, and they are rarely used for automation.
		/// Note: This does not work if the object from point is a LINK's grandchild.
		/// Note: Some Chrome versions have this bug: the parent object does not support <see cref="WndContainer"/>.
		/// </param>
		/// <exception cref="CatException">Failed. For example, window of a higher UAC integrity level process.</exception>
		public static Acc FromMouse(bool noThrow = false, bool preferLINK = false)
		{
			return _FromPoint(Mouse.XY, noThrow, preferLINK);
		}

		static Acc _FromPoint(Point p, bool noThrow, bool preferLINK)
		{
			using(new _TempSetScreenReader(false)) {
				bool retry = false; g1:
				var hr = Api.AccessibleObjectFromPoint(p, out var iacc, out var v);
				if(hr == 0 && iacc.Is0) hr = Api.E_FAIL;
				if(hr != 0) {
					if(noThrow) return null;
					_WndThrow(hr, Wnd.FromXY(p.X, p.Y, WXYFlags.Raw), "*get accessible object from point");
				}
				Debug.Assert(v.vt == Api.VARENUM.VT_I4 || v.vt == 0);
				int elem = v.vt == Api.VARENUM.VT_I4 ? v.ValueInt : 0;

				if(!retry && elem == 0 && 0 == iacc.GetWnd(out var w)) {
					//Chrome
					var wTL = w.WndWindow; //protection from possible Chrome bugs, eg AOFP may get AO of a child window
					int chrome = _EnableInChrome(wTL, iacc, false, p);
					if(chrome != 0) {
						if(chrome == 2) { iacc.Dispose(); retry = true; goto g1; }
					}
					//Java
					else if(w == wTL && 0 == iacc.GetRole(0, out var role) && role == AccROLE.CLIENT
						&& 0 == iacc.get_accChildCount(out int cc) && cc == 0) { //if CLIENT of top-level window has 0 children
						var ja = _Java.AccFromPoint(p, w);
						if(ja != null) { iacc.Dispose(); return ja; }
					}
				}

				if(preferLINK) _FromPoint_GetLink(ref iacc, ref elem);
				return new Acc(iacc, elem);
			}
		}

		/// <summary>
		/// If iacc is the client area object of Chrome, enables its web content accessible objects.
		/// Returns: 0 not Chrome, 1 Chrome was already enabled, 2 Chrome enabled now.
		/// </summary>
		/// <param name="w">iacc container window/control. Must be top-level (asserts).</param>
		/// <param name="iacc">object from point, or w CLIENT.</param>
		/// <param name="isCLIENT">iacc is w CLIENT. If false, p is used to detect whether it is CLIENT.</param>
		/// <param name="p">Used when iacc is object from point. Then isCLIENT is false.</param>
		static int _EnableInChrome(Wnd w, IAccessible iacc, bool isCLIENT, Point p = default)
		{
			Debug.Assert(!w.IsChildWindow);

			var f = Wnd.Lib.WinFlags.Get(w);
			if(f.Has_(Wnd.Lib.WFlags.ChromeYes)) return 1;
			if(f.Has_(Wnd.Lib.WFlags.ChromeNo)) return 0;

			bool yes = false;
			if(!w.ClassNameIs("Chrome*")) goto gNo;
			if(!isCLIENT) {
				if(0 != iacc.GetRole(0, out var role)) return 0;
				if(!(role == AccROLE.WINDOW || role == AccROLE.CLIENT || role == AccROLE.APPLICATION)) return 0; //in current Chrome it's WINDOW. Normally should be CLIENT or APPLICATION.
				var rc = w.ClientRectInScreen; if(!rc.Contains(p)) return 0;
				if(0 != iacc.accLocation(0, out var ra) || ra != rc) return 0;
				//after all the above checks, iacc still can be not the CLIENT we need
				iacc = _FromWindow(w, AccOBJID.CLIENT, noThrow: true); if(iacc.Is0) return 0;
			}
			var aDOCUMENT = Finder.LibGetChromeDOCUMENT(w, iacc);
			if(!isCLIENT) iacc.Dispose();
			if(aDOCUMENT.Is0) goto gNo;
			aDOCUMENT.Dispose();
			//Print("enabled");
			yes = true;
			gNo:
			Wnd.Lib.WinFlags.Set(w, f | (yes ? Wnd.Lib.WFlags.ChromeYes : Wnd.Lib.WFlags.ChromeNo));
			return yes ? 2 : 0;
		}

		static void _FromPoint_GetLink(ref IAccessible a, ref int elem)
		{
			if(0 != a.GetRole(elem, out var role)) return;
			switch(role) {
			case AccROLE.TEXT: //Firefox, IE
			case AccROLE.STATICTEXT: //Chrome
			case AccROLE.GRAPHIC:
			case AccROLE.CLIENT: //Chrome: the Bookmarks toolbar buttons have children of role CLIENT
			case 0: //string role, eg Firefox "span", "h2" etc
				IAccessible parent;
				if(elem != 0) parent = a; else if(0 != a.get_accParent(out parent)) return;
				if(_IsLink(parent)) {
					if(elem != 0) elem = 0; else { Math_.Swap(ref a, ref parent); parent.Dispose(); }
				}
				//rejected: support 2 levels, eg youtube right-side list. Actually there are 2-3 levels, and multiple children of LINK, some of them may be useful for automation.
				break;
			}

			bool _IsLink(IAccessible par)
			{
				if(0 != par.GetRole(0, out var role2) || !(role2 == AccROLE.LINK || role2 == AccROLE.PUSHBUTTON)) return false;
				//if(0 != _Api.WindowFromAccessibleObject(par, out var w) || w.Is0) return false; //rejected: Chrome bug workaround. For users in most cases LINK is more important than container window.
				return true;
			}
		}

		/// <summary>
		/// Gets accessible object from point in window.
		/// Returns null if the point is not in the window rectangle. Or if failed to get accessible object and noThrow is true.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the w client area.</param>
		/// <param name="y">Y coordinate relative to the w client area.</param>
		/// <param name="noThrow">Don't throw exception when fails to get accessible object from window. Then returns null.</param>
		/// <exception cref="WndException">Invalid window.</exception>
		/// <exception cref="CatException">Failed to get accessible object from window. For example, window of a higher UAC integrity level process.</exception>
		public static Acc FromXY(Wnd w, Coord x, Coord y, bool noThrow = false)
		{
			if(x.IsEmpty || y.IsEmpty) throw new ArgumentNullException();
			var p = Coord.NormalizeInWindow(x, y, w); //in client area
			if(!w.GetWindowAndClientRectInScreen(out RECT rw, out RECT rc)) {
				if(noThrow) return null;
				w.ThrowUseNative();
			}
			p.Offset(rc.left, rc.top);
			if(!rw.Contains(p)) return null;
			bool inClent = rc.Contains(p);
			var a = FromWindow(w, inClent ? AccOBJID.CLIENT : AccOBJID.WINDOW, noThrow: noThrow);
			if(a != null) {
				if(!inClent) {
				} else if(w.IsChildWindow) {
				} else if(0 != _EnableInChrome(w, a._iacc, true)) {
				} else if(0 == a._iacc.get_accChildCount(out int cc) && cc == 0) { //CLIENT of top-level window has 0 children
					var ja = _Java.AccFromPoint(p, w);
					if(ja != null) { a.Dispose(); return ja; }
				}
				if(0 == a._DescendantFromPoint(p, out var ac) && ac != a) {
					Math_.Swap(ref a, ref ac);
					ac.Dispose();
				}
			}
			return a;
		}

		/// <summary>
		/// Gets a child or descendant object from point.
		/// Returns self (this Acc variable, not a copy) if at that point is this object and not a child.
		/// Returns null if that point is not within boundaries of this object. For non-rectangular objects can return null even if the point is in the bounding rectangle. Also returns null if fails. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <param name="screenCoord">x y are screen coordinates. If false (default), x y are relative to the bounding rectangle of this object.</param>
		/// <param name="directChild">Get direct child. If false (default), gets the topmost descendant object, which often is not a direct child of this object.</param>
		/// <remarks>
		/// Uses API <msdn>IAccessible.accHitTest</msdn>.
		/// Does not work with Java windows.
		/// </remarks>
		public Acc ChildFromXY(Coord x, Coord y, bool screenCoord = false, bool directChild = false)
		{
			if(x.IsEmpty || y.IsEmpty) throw new ArgumentNullException();
			Point p;
			if(screenCoord) p = Coord.Normalize(x, y);
			else if(GetRect(out var r)) {
				p = Coord.NormalizeInRect(x, y, r);
				if(!r.Contains(p)) return null;
			} else return null;

			_Hresult(_FuncId.child_object, _DescendantFromPoint(p, out var ac, directChild));
			return ac;

			//rejected: parameter disposeThis.
			//never mind: does not work with Java windows.
		}

		int _DescendantFromPoint(Point p, out Acc ar, bool directChild = false)
		{
			ar = null;
			int hr = new _Acc(_iacc, _elem).DescendantFromPoint(p, out var ad, out bool isThis, directChild);
			if(hr == 0) ar = isThis ? this : new Acc(ad.a, ad.elem);
			return hr;
		}

		/// <summary>
		/// Gets the accessible object that has the keyboard focus.
		/// Returns null if fails.
		/// </summary>
		public static Acc Focused()
		{
			var w = Wnd.WndFocused;
			return w.Is0 ? null : _Focused(w);
		}

		/// <summary>
		/// Gets the accessible object that has the keyboard focus in the specified window.
		/// Returns null if the window does not contain the focused object or if fails.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <exception cref="WndException">w is default(Wnd).</exception>
		static Acc _Focused(Wnd w)
		{
			//info: At first this func was public. But it does not have sense. Inactive windows never return a focused child.
			//	Also does not have sense to get the focused object from an Acc.

			//w.ThrowIf0(); //don't need when this func is private
			var wTL = w.WndWindow;
			if(!wTL.IsActive) return null; //return quickly, anyway would return null
			var a = _FromWindow(w, AccOBJID.CLIENT, noThrow: true); //info: CLIENT, because WINDOW is never focused
			if(a.Is0) return null;
			bool isThis = false;
			try {
				if(w != wTL) {
				} else if(0 != _EnableInChrome(w, a, true)) {
				} else if(0 == a.get_accChildCount(out int cc) && cc == 0) { //CLIENT of top-level window has 0 children
					var ja = _Java.AccFromWindow(w, getFocused: true);
					if(ja != null) { a.Dispose(); return ja; }
				}
				int hr = new _Acc(a, 0).DescendantFocused(out var ad, out isThis);
				if(hr != 0) return null;
				if(isThis) return new Acc(a);
				return new Acc(ad.a, ad.elem);
			}
			finally { if(!isThis) a.Dispose(); }

			//SHOULDDO: can fail if recently focused. Noticed with FileZilla Settings dialog controls after IUIAutomationElement.SetFocus. Need to wait/retry.
		}

		/// <summary>
		/// Gets the accessible object that generated the event that is currently being processed by the callback function used with API <msdn>SetWinEventHook</msdn>.
		/// Returns null if failed. Suports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="idObject"></param>
		/// <param name="idChild"></param>
		/// <remarks>
		/// The parameters are of the callback function.
		/// Uses API <msdn>AccessibleObjectFromEvent</msdn>.
		/// Often fails because the object already does not exist, because the callback function is called asynchronously, especially when the event is OBJECT_DESTROY, OBJECT_HIDE, SYSTEM_*END.
		/// Returns null if failed. Always check the return value, to avoid NullReferenceException. An exception in a callback function used with API <msdn>SetWinEventHook</msdn> kills this process.
		/// </remarks>
		public static Acc FromEvent(Wnd w, int idObject, int idChild)
		{
			int hr = Api.AccessibleObjectFromEvent(w, idObject, idChild, out var iacc, out var v);
			if(hr == 0 && iacc.Is0) hr = Api.E_FAIL;
			if(hr != 0) { Native.SetError(hr); return null; }
			int elem = v.vt == Api.VARENUM.VT_I4 ? v.ValueInt : 0;
			return new Acc(iacc, elem);
		}

		/// <summary>
		/// Gets accessible object from a COM object of any type that supports it.
		/// Returns null if fails.
		/// </summary>
		/// <param name="x">Unmanaged COM object.</param>
		/// <remarks>
		/// The COM object type can be IAccessible, IAccessible2, IHTMLElement, ISimpleDOMNode or any other COM interface type that can give <msdn>IAccessible</msdn> interface pointer through API <msdn>IUnknown.QueryInterface</msdn> or <msdn>IServiceProvider.QueryService</msdn>.
		/// For IHTMLElement and ISimpleDOMNode returns null if the HTML element is not an accessible object. Then you can try to get accessible object of its parent HTML element, parent's parent and so on, until succeeds.
		/// </remarks>
		public static Acc FromComObject(IntPtr x)
		{
			if(x == default) return null;
			if(Util.Marshal_.QueryInterface(x, out IAccessible iacc, ref Api.IID_IAccessible)
				|| Util.Marshal_.QueryService(x, out iacc, ref Api.IID_IAccessible)
				) return new Acc(iacc);
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
		/// Also partially supports UIAutomationClient.IUIAutomationElement. Works with web page objects, but returns null for many other.
		/// </remarks>
		public static Acc FromComObject(object x)
		{
			if(x == null) return null;
			if(x is UIA.IUIAutomationElement e) { //info: IUIAutomationElement2-7 are IUIAutomationElement too
				var pat = e.GetCurrentPattern(10018) as UIA.IUIAutomationLegacyIAccessiblePattern; //UIA_LegacyIAccessiblePatternId
				x = pat?.GetIAccessible();
				if(x == null) return null;
			}

			var ip = Marshal.GetIUnknownForObject(x);
			if(ip == default) return null;
			try { return FromComObject(ip); }
			finally { Marshal.Release(ip); }
		}

		//public static Acc FromImage(SIResult foundImage, Coord x=default, Coord y=default, bool noThrow = false)
		//{
		//	//Acc FUTURE
		//	return null;
		//}

		/// <summary>
		/// Used only for debug.
		/// </summary>
		enum _FuncId { name, value, description, default_action, role, state, rectangle, parent_object, child_object, container_window, child_count, child_objects, help_text, keyboard_shortcut, navigate, html }

		/// <summary>
		/// Calls Native.SetError and returns hr.
		/// In Debug config also outputs error in red.
		/// If hr looks like not an error but just the property or action is unavailable, changes it to S_FALSE and does not show error. These are: S_FALSE, DISP_E_MEMBERNOTFOUND, E_NOTIMPL, for navigate also E_INVALIDARG, E_FAIL, E_NOINTERFACE.
		/// </summary>
		int _Hresult(_FuncId funcId, int hr)
		{
			if(hr != 0) {
				switch(hr) {
				case Api.DISP_E_MEMBERNOTFOUND: case Api.E_NOTIMPL: hr = Api.S_FALSE; break;
				case Api.E_INVALIDARG: case Api.E_FAIL: case Api.E_NOINTERFACE: if(funcId == _FuncId.navigate) hr = Api.S_FALSE; break;
				}
#if DEBUG
				if(hr != Api.S_FALSE) {
					_DebugPropGet(funcId, hr);
				}
#endif
			}
			Native.SetError(hr);
			return hr;
		}

#if DEBUG
		void _DebugPropGet(_FuncId funcId, int hr)
		{
			if(t_debugNoRecurse || _Disposed) return;
			if(hr == Api.E_FAIL && funcId == _FuncId.default_action) return; //many in old VS etc
			t_debugNoRecurse = true;
			try {
				var s = ToString();
				Print($"<><c 0xff>-{funcId}, 0x{hr:X} - {Native.GetErrorMessage(hr)}    {s}</c>");
			}
			finally { t_debugNoRecurse = false; }
		}
		[ThreadStatic] static bool t_debugNoRecurse;
#endif

		/// <summary>
		/// Formats string from main properties of this accessible object.
		/// </summary>
		/// <remarks>
		/// The string starts with role. If fails to get role, the string is "failed: error message".
		/// </remarks>
		public override string ToString()
		{
			return _ToString();
		}

		string _ToString()
		{
			if(_Disposed) return "<disposed>";
			StringBuilder s = null;
			bool isWindow = false;
			for(int i = 0; i < 8; i++) {
				string t = null, what = null; char quot1 = '\"', quot2 = '\"';
				try {
					switch(i) {
					case 0: t = RoleString; isWindow = (t == "WINDOW"); break;
					case 1: what = "Name"; t = Name; break;
					case 2: what = "State"; quot1 = '('; quot2 = ')'; var st = State; if(st != 0) t = st.ToString(); break;
					case 3: what = "Value"; t = Value; break;
					case 4: what = "Description"; t = Description; break;
					case 5: what = "DefaultAction"; t = DefaultAction; break;
					case 6: what = "Elem"; quot1 = quot2 = '\0'; if(_elem != 0) t = _elem.ToString(); break;
					case 7:
						if(isWindow) {
							what = "WndContainer"; quot1 = '{'; quot2 = '}';
							t = WndContainer.ToString();
						} else {
							what = "Rect"; quot1 = quot2 = '\0';
							if(GetRect(out var rect)) t = rect.ToString();
						}
						break;
					}
				}
				catch(Exception ex) {
					Debug_.Print("exception, " + what);
					if(i == 0) return "failed: " + ex.Message;
				}
				if(i == 0) {
					if(t == null) return "failed: " + Native.GetErrorMessage();
					s = Util.LibStringBuilderCache.Acquire();
					s.Append(t);
				} else if(!Empty(t)) {
					s.Append(", ");
					s.Append(what);
					s.Append('=');
					if(quot1 != 0) s.Append(quot1);
					if(quot1 == '\"') t = t.Limit_(250).Escape_();
					s.Append(t);
					if(quot2 != 0) s.Append(quot2);
				}
			}
			return s.ToStringCached_();
		}

		/// <summary>
		/// This <see cref="ToString()"/> overload indents the string depending on level.
		/// </summary>
		/// <param name="level">Object level in the tree. This function prepends level*2 spaces to the string.</param>
		public string ToString(int level)
		{
			var s = _ToString(); //note: not ToString because it causes overflow if ToString is of AFAcc
			if(level > 0) s = s.PadLeft(s.Length + level * 2);
			return s;
		}
	}
}
