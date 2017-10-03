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
	/// </remarks>
	public unsafe partial class Acc :IDisposable, ISupportOrThrow
	{
		//tested: Acc object memory size with overhead: 32 bytes. Note: we don't use RCW<IAccessible>, which would add another 32 bytes.

		internal IAccessible _iacc;
		internal int _elem;
		internal AccROLE _role; //store role because it is slow to get and used in several places. Does not make real object memory bigger (still 32 bytes).
								//CONSIDER: cache state too. Add property StateCurrent. But then also need IsInvisibleCurrent etc. Or instead add property StateDoNotCache.

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
			Debug_.Print("Acc not disposed: " + (_role != 0 ? RoleString : ""));
			_Dispose();
		}

		///// <summary>
		///// The IAccessible that is managed by this variable.
		///// This func does not increment ref count etc. It simply returns the field. Don't release it. It will be released by Dispose.
		///// </summary>
		//internal IAccessible LibIAccessible { get => _iacc; }

		/// <summary>
		/// Gets or sets child element id.
		/// </summary>
		public int Elem { get => _elem; set { _role = 0; _elem = value; } }

		/// <summary>
		/// Returns true if this variable is disposed.
		/// </summary>
		public bool Is0 { get => _iacc.Is0; }
		//note: named not 'IsDisposed' because can be easily confused with IsDisabled.

		/// <summary>
		/// Gets accessible object from point.
		/// Uses API <msdn>AccessibleObjectFromPoint</msdn>.
		/// </summary>
		/// <param name="x">X coordinate in screen.</param>
		/// <param name="y">Y coordinate in screen.</param>
		/// <param name="workArea">x y are relative to the work area, not entire screen.</param>
		/// <param name="screen">Screen of x y. If null, primary screen. See <see cref="Screen_.FromObject"/>.</param>
		/// <param name="noThrow">Don't throw exception if failed. Then returns null.</param>
		/// <exception cref="CatException">Failed. For example, window of a higher UAC integrity level process.</exception>
		public static Acc FromPoint(Coord x, Coord y, bool workArea = false, object screen = null, bool noThrow = false)
		{
			if(x.IsNull || y.IsNull) throw new ArgumentNullException();
			return _FromPoint(Coord.Normalize(x, y, workArea, screen), noThrow);
		}

		/// <summary>
		/// Gets accessible object from mouse cursor position.
		/// Uses API <msdn>AccessibleObjectFromPoint</msdn>.
		/// </summary>
		/// <param name="noThrow">Don't throw exception if failed. Then returns null.</param>
		/// <exception cref="CatException">Failed. For example, window of a higher UAC integrity level process.</exception>
		public static Acc FromMouse(bool noThrow = false)
		{
			return _FromPoint(Mouse.XY, noThrow);
		}

		static Acc _FromPoint(Point p, bool noThrow)
		{
			var hr = _Api.AccessibleObjectFromPoint(p, out var iacc, out var elem);
			if(hr == 0 && iacc.Is0) hr = Api.E_FAIL;
			if(hr != 0) {
				if(noThrow) return null;
				_WndThrow(hr, Wnd.FromXY(p.X, p.Y, WXYFlags.Raw), "*get accessible object from point");
			}
			Debug.Assert(elem.vt == Api.VARENUM.VT_I4);
			int e = (elem.vt == Api.VARENUM.VT_I4 ? elem.ValueInt : 0);
			return new Acc(iacc, e);
		}

		//public static Acc FromPoint(Wnd w, Coord x, Coord y, bool nonClient = false, bool noThrow = false)
		//{
		//	if(x.IsNull || y.IsNull) throw new ArgumentNullException();
		//	//var p=Coord.NormalizeInWindow(x, y, w, nonClient);
		//	//TODO
		//	return null;
		//}

		//public static Acc FromImage(SIResult foundImage, Coord x=default, Coord y=default, bool noThrow = false)
		//{
		//	//TODO
		//	return null;
		//}

		//QI/QS from IAccessible, IUIAutomationElement, IHTMLElement, ISimpleDOMNode or anything that can give IAccessible.
		//public static Acc FromComObject(IntPtr x, int accChildId = 0)
		//{
		//	//TODO
		//	return null;
		//}
		//public static Acc FromComObject(object x, int accChildId = 0)
		//{
		//	//TODO
		//	return null;
		//}

		/// <summary>
		/// Gets accessible object of window or control or its standard part - client area, titlebar etc.
		/// Uses API <msdn>AccessibleObjectFromWindow</msdn>.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="objid">Window part id. Default WINDOW (object of w itself). Can be one of enum AccOBJID constants. Also can be a custom id supported by that window, cast int to AccOBJID.</param>
		/// <param name="noThrow">Don't throw exception if failed. Then returns null.</param>
		/// <exception cref="WndException">Invalid window.</exception>
		/// <exception cref="CatException">Failed. For example, window of a higher UAC integrity level process.</exception>
		public static Acc FromWindow(Wnd w, AccOBJID objid = AccOBJID.WINDOW, bool noThrow = false)
		{
			//TODO: review callers, do they are prepared for exceptions.

			//if(!LibFromWindow(out var iacc, w, objid, noThrow)) return null;
			//return new Acc(iacc);

			var iacc = _FromWindow(w, objid, noThrow); if(iacc.Is0) return null;
			return new Acc(iacc);
		}

		static IAccessible _FromWindow(Wnd w, AccOBJID objid = AccOBJID.WINDOW, bool noThrow = false)
		{
			var hr = _Api.AccessibleObjectFromWindow(w, objid, ref _Api.IID_IAccessible, out var iacc);
			if(hr == 0 && iacc.Is0) hr = Api.E_FAIL;
			if(hr != 0) {
				if(noThrow) return default;
				w.ThrowIfInvalid();
				_WndThrow(hr, w, "*get accessible object from window");
			}
			return iacc;
		}
		//TODO: AccessibleObjectFromEvent

		static void _WndThrow(int hr, Wnd w, string es)
		{
			if(hr == 0) return;
			//note: hr is random when blocked by UAC. Can be E_ACCESSDENIED, E_FAIL, E_OUTOFMEMORY.
			if(!w.Is0 && w.IsUacAccessDenied) es += ". Window of admin process (UAC). Run this app as admin or uiAccess";
			throw new CatException(hr, es);
		}

		//rejected: rarely used. Problem with libraries. Objects return variety of error codes. Some even after closing the window return S_FALSE and not "RPC server disconnected" etc.
		///// <summary>
		///// If set to true, Acc functions (properties and methods) that get object properties throw exception when failed.
		///// If false (default), when failed they return default value for that type (null if string, 0 if Enum, etc). Then you can use <see cref="Native.GetError"/>; it is HRESULT returned by the API (<msdn>IAccessible</msdn> member function or some other).
		///// This property is thread-specific.
		///// </summary>
		///// <remarks>
		///// Why the default behavior is to not throw when failed? Because many objects have bugs or unexpected rules and may throw exception when they shouldn't or where you don't expect it. In most cases it's better to just ignore empty properties than to handle exceptions. You'll probably want to set this property to true only when debugging your code, or in some tools that handle and display exceptions, or when working with a known window where you know there cannot be any exceptions in normal conditions.
		///// Regardless of this property, the functions don't throw exception if the property is unavailable for that object, ie the API returned HRESULT is S_FALSE, DISP_E_MEMBERNOTFOUND or E_NOTIMPL. Then string return value is "", not null.
		///// Regardless of this property, the functions throw ObjectDisposedException if this variable is disposed.
		///// The static functions (FromWindow etc) don't use this property. Some of them have a noThrow parameter instead.
		///// </remarks>
		//public static bool PropGetThrows { get => t_propGetThrows; set => t_propGetThrows = value; }
		//[ThreadStatic] static bool t_propGetThrows;

		///// <summary>
		///// If hr is 0, returns 0.
		///// Else if the property is unavailable (S_FALSE, DISP_E_MEMBERNOTFOUND, E_NOTIMPL), calls Native.SetError and returns S_FALSE (1). For navigate also if E_INVALIDARG, E_FAIL, E_NOINTERFACE.
		///// Else if PropGetThrows is true, throws CatException.
		///// Else calls Native.SetError and returns hr (usually negative).
		///// </summary>
		//int _Hresult(_FuncId funcId, int hr)
		//{
		//	if(hr == 0) return 0;
		//	switch(hr) {
		//	case Api.DISP_E_MEMBERNOTFOUND: case Api.E_NOTIMPL: hr = Api.S_FALSE; break;
		//	case Api.E_INVALIDARG: case Api.E_FAIL: case Api.E_NOINTERFACE: if(funcId == _FuncId.navigate) hr = Api.S_FALSE; break;
		//	}
		//	if(hr != Api.S_FALSE) {
		//		_DebugPropGet(funcId, hr);
		//		if(PropGetThrows) throw new CatException(hr, "*get " + funcId.ToString().Replace('_', ' '));
		//	}
		//	Native.SetError(hr);
		//	return hr;
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
			if(t_debugNoRecurse || Is0) return;
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
		/// Gets the container window or control of this accessible object.
		/// Uses API <msdn>WindowFromAccessibleObject</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns default(Wnd) if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property, but some have bugs and can return default(Wnd) or a wrong window.
		/// </remarks>
		public Wnd WndContainer
		{
			get
			{
				if(Is0) throw new ObjectDisposedException(nameof(Acc));
				_Hresult(_FuncId.container_window, _Api.WindowFromAccessibleObject(_iacc, out var w));
				return w;
			}
		}

		/// <summary>
		/// Gets the top-level window that contains this accessible object.
		/// Uses API <msdn>WindowFromAccessibleObject</msdn> and API <msdn>GetAncestor</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns default(Wnd) if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property, but some have bugs and can return default(Wnd).
		/// </remarks>
		public Wnd WndWindow { get => WndContainer.WndWindow; }

		/// <summary>
		/// Gets location of this accessible object in screen.
		/// Uses <msdn>IAccessible.accLocation</msdn>.
		/// </summary>
		/// <param name="r">Receives object rectangle in screen coordinates.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="Native.GetError"/>.
		/// Most but not all objects support this property.
		/// </remarks>
		public bool GetRect(out RECT r)
		{
			return 0 == _Hresult(_FuncId.rectangle, _iacc.accLocation(_elem, out r));
		}

		/// <summary>
		/// Gets location of this accessible object in the client area of window w.
		/// Uses <msdn>IAccessible.accLocation</msdn> and <see cref="Wnd.MapScreenToClient(ref RECT)"/>.
		/// </summary>
		/// <param name="r">Receives object rectangle in w client area coordinates.</param>
		/// <param name="w">Window or control.</param>
		/// <remarks>
		/// Returns false if failed or this property is unavailable. Supports <see cref="Native.GetError"/>.
		/// Most but not all objects support this property.
		/// </remarks>
		public bool GetRect(out RECT r, Wnd w)
		{
			return GetRect(out r) && w.MapScreenToClient(ref r);
		}

		/// <summary>
		/// Gets standard non-string role, as enum AccROLE.
		/// Uses <msdn>IAccessible.get_accRole</msdn>.
		/// </summary>
		/// <remarks>
		/// Most objects have a standard role, as enum AccROLE. Some objects have a custom role, normally as string.
		/// Returns 0 if object's role is string.
		/// Returns 0 if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property. If failed, probably the object is invalid, for example its window was closed.
		/// </remarks>
		public AccROLE RoleEnum
		{
			get
			{
				if(_role == 0) _Hresult(_FuncId.role, _iacc.GetRole(_elem, out _role));
				return _role;
			}
		}

		/// <summary>
		/// Gets standard or custom role, as string.
		/// Uses <msdn>IAccessible.get_accRole</msdn>.
		/// </summary>
		/// <remarks>
		/// Most objects have a standard role, as enum <see cref="AccROLE"/>. Some objects have a custom role, normally as string, for example in web pages in Firefox and Chrome.
		/// For standard roles this function returns enum <see cref="AccROLE"/> member name. For string roles - the string. For unknown non-string roles - the int value like "0" or "500".
		/// Returns "" if failed. Supports <see cref="Native.GetError"/>.
		/// All objects must support this property. If failed, probably the object is invalid, for example its window was closed.
		/// </remarks>
		public string RoleString
		{
			get
			{
				_Hresult(_FuncId.role, _iacc.GetRoleString(_elem, out string s, ref _role));
				return s ?? "";
			}
		}

		/// <summary>
		/// Gets object state (flags).
		/// Uses <msdn>IAccessible.get_accState</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns 0 if failed. Supports <see cref="Native.GetError"/>.
		/// Getting state often is slower than getting role, name or rectangle. But usually not much slower.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// if(a.State.Has_(AccSTATE.INVISIBLE)) Print("invisible");
		/// if(a.IsInvisible) Print("invisible"); //the same as above
		/// ]]></code>
		/// </example>
		public AccSTATE State
		{
			get
			{
				_Hresult(_FuncId.state, _iacc.get_accState(_elem, out var state));
				return state;
			}
		}

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.BUSY. Can be used with DOCUMENT object of web browsers. </summary>
		public bool IsBusy { get => State.Has_(AccSTATE.BUSY); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.CHECKED. </summary>
		public bool IsChecked { get => State.Has_(AccSTATE.CHECKED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.UNAVAILABLE. </summary>
		public bool IsDisabled { get => State.Has_(AccSTATE.UNAVAILABLE); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.FOCUSED. </summary>
		public bool IsFocused { get => State.Has_(AccSTATE.FOCUSED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.INVISIBLE. </summary>
		public bool IsInvisible { get => State.Has_(AccSTATE.INVISIBLE); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.OFFSCREEN. </summary>
		public bool IsOffscreen { get => State.Has_(AccSTATE.OFFSCREEN); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.PROTECTED. </summary>
		public bool IsPassword { get => State.Has_(AccSTATE.PROTECTED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.PRESSED. </summary>
		public bool IsPressed { get => State.Has_(AccSTATE.PRESSED); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.READONLY. </summary>
		public bool IsReadonly { get => State.Has_(AccSTATE.READONLY); }

		/// <summary> Calls <see cref="State"/> and returns true if has AccSTATE.SELECTED. See also <see cref="IsFocused"/>. </summary>
		public bool IsSelected { get => State.Has_(AccSTATE.SELECTED); }

		/// <summary>
		/// Gets or sets name.
		/// Uses <msdn>IAccessible.get_accName</msdn> or <msdn>IAccessible.put_accName</msdn>.
		/// </summary>
		/// <exception cref="CatException">Failed to set name.</exception>
		/// <remarks>
		/// Object name usually is its read-only text (eg button text or link text), or its adjacent read-only text (eg text label by this edit box). It usually does not change, therefore can be used to find or identify the object.
		/// Most objects don't support 'set'.
		/// The 'get' function returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Name
		{
			get
			{
				_Hresult(_FuncId.name, _iacc.get_accName(_elem, out var s, _role));
				return s;
			}
			set
			{
				CatException.ThrowIfFailed(_iacc.put_accName(_elem, value));
			}
		}

		/// <summary>
		/// Gets or sets value.
		/// Uses <msdn>IAccessible.get_accValue</msdn> or <msdn>IAccessible.put_accValue</msdn>.
		/// </summary>
		/// <exception cref="CatException">Failed to set value.</exception>
		/// <remarks>
		/// Object value usually is its editable text or some other value that can be changed at run time, therefore in most cases it cannot be used to find or identify the object reliably.
		/// Most objects don't support 'set'.
		/// The 'get' function returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Value
		{
			get
			{
				_Hresult(_FuncId.value, _iacc.get_accValue(_elem, out var s));
				return s;
			}
			set
			{
				CatException.ThrowIfFailed(_iacc.put_accValue(_elem, value));
			}
		}

		/// <summary>
		/// Gets description.
		/// Uses <msdn>IAccessible.get_accDescription</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Description
		{
			get
			{
				_Hresult(_FuncId.description, _iacc.get_accDescription(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets help text.
		/// Uses <msdn>IAccessible.get_accHelp</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string Help
		{
			get
			{
				_Hresult(_FuncId.help_text, _iacc.get_accHelp(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets keyboard shortcut.
		/// Uses <msdn>IAccessible.get_accKeyboardShortcut</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string KeyboardShortcut
		{
			get
			{
				_Hresult(_FuncId.keyboard_shortcut, _iacc.get_accKeyboardShortcut(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Gets default action.
		/// Uses <msdn>IAccessible.get_accDefaultAction</msdn>.
		/// </summary>
		/// <remarks>
		/// Returns "" if this property is unavailable or if failed. Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public string DefaultAction
		{
			get
			{
				_Hresult(_FuncId.default_action, _iacc.get_accDefaultAction(_elem, out var s));
				return s;
			}
		}

		/// <summary>
		/// Queries the accessible object to perform its default action (see <see cref="DefaultAction"/>), eg 'click'.
		/// Uses <msdn>IAccessible.accDoDefaultAction</msdn>.
		/// </summary>
		/// <exception cref="CatException">Failed.</exception>
		public void DoDefaultAction()
		{
			CatException.ThrowIfFailed(_iacc.accDoDefaultAction(_elem));
		}

		/// <summary>
		/// Gets another accessible object that is somehow related to this object (child, parent, next, etc). Allows to specify multiple relations.
		/// </summary>
		/// <param name="navig">
		/// String consisting of one or more navigation direction strings separated by space, like "parent next child4 first".
		/// Navigation string reference and more info: <see cref="AccNAVDIR"/>.
		/// A string can end with a number, like "child4" or "child,4". It is the n parameter of <see cref="Navigate(AccNAVDIR, int, bool)"/>.
		/// A custom direction can be specified like "#0x1009".
		/// </param>
		/// <param name="disposeThis">Dispose this Acc variable (release the COM object).</param>
		/// <exception cref="ArgumentException">Invalid navig string.</exception>
		public Acc Navigate(string navig, bool disposeThis = false)
		{
			if(Is0) throw new ObjectDisposedException(nameof(Acc));
			var na = _Acc.NavdirN.Parse(navig); //ArgumentException
			var a = new _Acc(_iacc, _elem);
			if(disposeThis) _Clear(doNotRelease: true); else _iacc.AddRef();
			if(0 != _Hresult(_FuncId.navigate, a.Navigate(na))) return null;
			return new Acc(a.a, a.elem);
		}

		/// <summary>
		/// Gets another accessible object that is somehow related to this object (child, parent, next, etc).
		/// Returns null if not found. You can use <see cref="ExtensionMethods.OrThrow{T}"/> (see example).
		/// </summary>
		/// <param name="navDir">
		/// Navigation direction (relation). For example <c>AccNAVDIR.PARENT</c>.
		/// Custom directions also are supported. For example <c>(AccNAVDIR)0x1009</c>.
		/// More info is in <see cref="AccNAVDIR"/> documentation.
		/// </param>
		/// <param name="n">
		/// With navDir AccNAVDIR.CHILD it is 1-based child index. Negative index means from end, for example -1 is the last child. Cannot be 0.
		/// With all other navDir - how many times to get object in the specified direction. For example PARENT 2 will get parent's parent. Cannot be less than 1.
		/// </param>
		/// <param name="disposeThis">Dispose this Acc variable (release the COM object).</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid n.</exception>
		public Acc Navigate(AccNAVDIR navDir, int n = 1, bool disposeThis = false)
		{
			if(Is0) throw new ObjectDisposedException(nameof(Acc));
			if(n == 0 || (n < 0 && navDir != AccNAVDIR.CHILD)) throw new ArgumentOutOfRangeException();
			var a = new _Acc(_iacc, _elem);
			if(disposeThis) _Clear(doNotRelease: true); else _iacc.AddRef();
			if(0 != _Hresult(_FuncId.navigate, a.Navigate(navDir, n))) return null;
			return new Acc(a.a, a.elem);
		}

		//rejected: public Acc Parent(bool disposeThis = false) (call get_accParent directly). Can use Navigate(), it's almost as fast. Useful mostly in programming, not in scripts.
		//rejected: public Acc Child(int childIndex, bool disposeThis = false) (call get_accChild). Many objects don't implement get_accChild. Can use Navigate, which calls AccessibleChildren. Rarely used, unlike Parent.

		/// <summary>
		/// Formats string from main properties of this accessible object.
		/// </summary>
		/// <remarks>
		/// The string starts with role. If fails to get role, the string is "failed: error message".
		/// </remarks>
		public override string ToString()
		{
			if(Is0) return "<disposed>";
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
			var s = ToString();
			if(level > 0) s = s.PadLeft(s.Length + level * 2);
			return s;
		}

		static class _Api
		{
			internal static Guid IID_IAccessible = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

			internal static Guid IID_IAccessible2 = new Guid(0xE89F726E, 0xC4F4, 0x4c19, 0xBB, 0x19, 0xB6, 0x47, 0xD7, 0xFA, 0x84, 0x78);

			[DllImport("oleacc.dll", PreserveSig = true)]
			internal static extern int AccessibleObjectFromWindow(Wnd hwnd, AccOBJID dwId, ref Guid riid, out IAccessible ppvObject);

			[DllImport("oleacc.dll", PreserveSig = true)]
			internal static extern int WindowFromAccessibleObject(IAccessible iacc, out Wnd phwnd);

			[DllImport("oleacc.dll", PreserveSig = true)]
			internal static extern int AccessibleObjectFromPoint(Point ptScreen, out IAccessible ppacc, out VARIANT pvarChild);

			[DllImport("oleacc.dll", PreserveSig = true)]
			internal static extern int AccessibleObjectFromEvent(Wnd hwnd, int dwId, int dwChildId, out IAccessible ppacc, out VARIANT pvarChild);

			[DllImport("oleacc.dll", PreserveSig = true)]
			internal static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, VARIANT* rgvarChildren, out int pcObtained);

			//these are not useful. They work only with standard roles/states, and return localized string. We instead use non-localized string[] or Enum.ToString().
			//[DllImport("oleacc.dll", EntryPoint = "GetRoleTextW")]
			//internal static extern int GetRoleText(int lRole, [Out] char[] lpszRole, int cchRoleMax);
			//[DllImport("oleacc.dll", EntryPoint = "GetStateTextW")]
			//internal static extern int GetStateText(int lStateBit, [Out] char[] lpszState, int cchState);
		}
	}
}
