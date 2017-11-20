using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS0660, CS0661 // Type defines operator == or operator != but does not override Object.Equals(object o), GetHashCode

namespace Catkeys
{
	public unsafe partial class Acc
	{
		internal struct IAccessible :IDisposable
		{
			IntPtr _iptr;

			public IAccessible(IntPtr iacc)
			{
				_iptr = iacc;
				Debug.Assert(!Is0);
			}

			public void Dispose()
			{
				if(_iptr != default) {
					Release();
					_iptr = default;
				}
			}

			public bool Is0 => _iptr == default;

			public static bool operator ==(IAccessible a, IAccessible b) => a._iptr == b._iptr;
			public static bool operator !=(IAccessible a, IAccessible b) => a._iptr != b._iptr;

			public int AddRef()
			{
				Debug.Assert(!Is0);
				return Marshal.AddRef(_iptr);
				//tested: Marshal.AddRef/Release is same speeed as direct AddRef/Release.
			}

			public void AddRefIfSame(IAccessible other)
			{
				Debug.Assert(!Is0);
				if(_iptr == other._iptr) Marshal.AddRef(_iptr);
			}

			public int Release()
			{
				Debug.Assert(!Is0);
				return Marshal.Release(_iptr);
			}

			public static implicit operator IntPtr(IAccessible a) => a._iptr;

			public int GetWnd(out Wnd w)
			{
				w = default;
				Debug.Assert(!Is0);

				int hr = Api.WindowFromAccessibleObject(this, out w);

				return hr;
			}

			//TODO: test with new Chrome version where this bug fixed: https://bugs.chromium.org/p/chromium/issues/detail?id=773208
			public int get_accParent(out IAccessible iacc)
			{
				iacc = default;
				var hr = _F.get_accParent(_iptr, out var idisp);
				if(hr == 0) hr = FromIDispatch(idisp, out iacc);
				return hr;
			}

			public int get_accChildCount(out int pcountChildren)
			{
				int hr = _F.get_accChildCount(_iptr, out pcountChildren);
				if(hr != 0) pcountChildren = 0;
				return hr;
			}

			public int get_accChild(int elem, out IAccessible iacc)
			{
				iacc = default;
				var hr = _F.get_accChild(_iptr, elem, out var idisp);
				if(hr == 0) hr = FromIDispatch(idisp, out iacc);
				return hr;
			}

			/// <summary>
			/// Gets name or "".
			/// </summary>
			/// <param name="elem"></param>
			/// <param name="s"></param>
			/// <param name="role">If not 0, used by _WorkaroundGetToolbarButtonName.</param>
			public int get_accName(int elem, out string s, AccROLE role)
			{
				int hr = _F.get_accName(_iptr, elem, out BSTR b);
				if(hr == 1) {
					_WorkaroundGetToolbarButtonName(elem, role, out s);
					if(s != null) return 0;
				}
				return _BstrToString(hr, b, out s);
			}

			public int get_accValue(int elem, out string s)
			{
				int hr = _F.get_accValue(_iptr, elem, out BSTR b);
				return _BstrToString(hr, b, out s);
			}

			public int get_accDescription(int elem, out string s)
			{
				int hr = _F.get_accDescription(_iptr, elem, out BSTR b);
				return _BstrToString(hr, b, out s);
			}

			int get_accRole(int elem, out AccROLE roleEnum, out string roleString, bool dontNeedString)
			{
				roleEnum = 0; roleString = null;
				int hr = _F.get_accRole(_iptr, elem, out var v);
				if(hr != 0) return hr;
				switch(v.vt) {
				case Api.VARENUM.VT_I4:
					roleEnum = (AccROLE)v.ValueInt;
					return 0;
				case Api.VARENUM.VT_BSTR:
					if(dontNeedString) break;
					var bstr = (char*)v.value;
					roleString = (bstr != null) ? Util.StringCache.LibAdd(bstr) : "";
					break;
				default:
					roleString = "";
					Debug.Assert(false);
					break;
				}
				v.Dispose();
				return 0;

				//rejected: try to map string roles to enum roles.
				//	tested: Chrome standard roles are enum.
			}

			/// <summary>
			/// Gets numeric (standard) role.
			/// Returns HRESULT.
			/// </summary>
			/// <param name="elem"></param>
			/// <param name="role">Receives numeric role. Receives 0 if role is string or failed.</param>
			public int GetRole(int elem, out AccROLE role)
			{
				return get_accRole(elem, out role, out _, true);
			}

			/// <summary>
			/// Gets numeric (standard) or string (custom) role.
			/// Returns HRESULT.
			/// </summary>
			/// <param name="elem"></param>
			/// <param name="roleEnum">Receives numeric role. Receives 0 if role is string or failed.</param>
			/// <param name="roleString">Receives string role. Receives null if role is numeric or failed.</param>
			public int GetRole(int elem, out AccROLE roleEnum, out string roleString)
			{
				return get_accRole(elem, out roleEnum, out roleString, false);
			}

			/// <summary>
			/// Gets standard or custom role as string.
			/// Returns HRESULT.
			/// </summary>
			/// <param name="elem"></param>
			/// <param name="role">Receives standard role name or custom role string or non-standard role numeric value as string. Receives null if failed.</param>
			public int GetRoleString(int elem, out string role)
			{
				AccROLE roleEnum = 0;
				return GetRoleString(elem, out role, ref roleEnum);
			}

			/// <summary>
			/// Gets standard or custom role as string.
			/// Returns HRESULT.
			/// </summary>
			/// <param name="elem"></param>
			/// <param name="role">Receives standard role name or custom role string or non-standard role numeric value as string. Receives null if failed.</param>
			/// <param name="roleEnum">If 0, calls get_accRole and sets roleEnum = numeric role or 0. Else just maps roleEnum to string.</param>
			public int GetRoleString(int elem, out string role, ref AccROLE roleEnum)
			{
				if(roleEnum == 0) {
					var hr = GetRole(elem, out roleEnum, out role);
					if(hr != 0 || role != null) return hr;
				}
				var a = s_roles; uint u = (uint)roleEnum;
				role = (u < a.Length) ? a[u] : ((int)roleEnum).ToString();
				return 0;
			}

			static string[] s_roles = { "0", "TITLEBAR", "MENUBAR", "SCROLLBAR", "GRIP", "SOUND", "CURSOR", "CARET", "ALERT", "WINDOW", "CLIENT", "MENUPOPUP", "MENUITEM", "TOOLTIP", "APPLICATION", "DOCUMENT", "PANE", "CHART", "DIALOG", "BORDER", "GROUPING", "SEPARATOR", "TOOLBAR", "STATUSBAR", "TABLE", "COLUMNHEADER", "ROWHEADER", "COLUMN", "ROW", "CELL", "LINK", "HELPBALLOON", "CHARACTER", "LIST", "LISTITEM", "OUTLINE", "OUTLINEITEM", "PAGETAB", "PROPERTYPAGE", "INDICATOR", "GRAPHIC", "STATICTEXT", "TEXT", "PUSHBUTTON", "CHECKBUTTON", "RADIOBUTTON", "COMBOBOX", "DROPLIST", "PROGRESSBAR", "DIAL", "HOTKEYFIELD", "SLIDER", "SPINBUTTON", "DIAGRAM", "ANIMATION", "EQUATION", "BUTTONDROPDOWN", "BUTTONMENU", "BUTTONDROPDOWNGRID", "WHITESPACE", "PAGETABLIST", "CLOCK", "SPLITBUTTON", "IPADDRESS", "OUTLINEBUTTON" };

			public int get_accState(int elem, out AccSTATE state)
			{
				state = 0;
				var hr = _F.get_accState(_iptr, elem, out var v);
				if(hr == 0) {
					if(v.vt == Api.VARENUM.VT_I4) state = (AccSTATE)v.ValueInt;
					else {
						Debug_.Print(v.vt); //never saw this
						v.Dispose();
					}
				}
				return hr;
			}

			//rarely used, but some apps use this instead of get_accDescription
			public int get_accHelp(int elem, out string s)
			{
				int hr = _F.get_accHelp(_iptr, elem, out BSTR b);
				return _BstrToString(hr, b, out s);
			}

			public int get_accKeyboardShortcut(int elem, out string s)
			{
				int hr = _F.get_accKeyboardShortcut(_iptr, elem, out BSTR b);
				return _BstrToString(hr, b, out s);
			}

			/// <summary>
			/// Gets object from point (screen coordinates), which can be a child (topmost) or self.
			/// If neither a child or this is at that point, sets a.iacc=0 and a.elem=0. Returns 1 (S_FALSE). Note: the point can be in objects's bounding rectangle but the object is not rectangular.
			/// Else if at that point is this, sets a.iacc=this and a.elem=0. Does not AddRef.
			/// Else if at that point is a simple element, sets a.iacc=this and a.elem!=0. Does not AddRef.
			/// Else sets a.iacc=child and a.elem=0. Will need to release a.iacc.
			/// </summary>
			public int accHitTest(int xLeft, int yTop, out _Acc a)
			{
				a = default;
				int hr = _F.accHitTest(_iptr, xLeft, yTop, out var v);
				if(hr == 0) {
					if(v.vt == 0) hr = 1;
					else if(v.vt == Api.VARENUM.VT_I4 && v.value == 0) a.a = this;
					else hr = FromVARIANT(ref v, out a, true);
				}
				return hr;
			}

			/// <summary>
			/// Gets focused object, which can be a child or this.
			/// If neither a child or this is focused, sets a.iacc=0 and a.elem=0. Returns 1 (S_FALSE).
			/// Else if focused is this, sets a.iacc=this and a.elem=0. Does not AddRef.
			/// Else if focused is a simple element, sets a.iacc=this and a.elem!=0. Does not AddRef.
			/// Else sets a.iacc=child and a.elem=0. Will need to release a.iacc.
			/// </summary>
			public int get_accFocus(out _Acc a)
			{
				a = default;
				int hr = _F.get_accFocus(_iptr, out var v);
				if(hr == 0) {
					if(v.vt == 0) hr = 1;
					else if(v.vt == Api.VARENUM.VT_I4 && v.value == 0) a.a = this;
					else hr = FromVARIANT(ref v, out a, true);
				}
				return hr;
			}

			public Acc[] get_accSelection()
			{
				if(0 != _F.get_accSelection(_iptr, out var v)) v.Dispose();
				if(v.vt != 0) {
					if(v.vt == Api.VARENUM.VT_UNKNOWN) {
						var t = new List<Acc>();
						if(Api.IEnumVARIANT.From(v.value, out var e)) {
							while(0 == e.Next(1, out var vv, out int n) && n == 1) {
								if(0 == FromVARIANT(ref vv, out var a, true))
									t.Add(new Acc(a.a, a.elem, addRef: a.elem != 0));
							}
							e.Dispose();
						}
						v.Dispose();
						return t.ToArray();
					} else {
						if(0 == FromVARIANT(ref v, out var a, true))
							return new Acc[] { new Acc(a.a, a.elem, addRef: a.elem != 0) };
					}
				}
				return new Acc[0];
			}

			public int get_accDefaultAction(int elem, out string s)
			{
				int hr = _F.get_accDefaultAction(_iptr, elem, out BSTR b);
				return _BstrToString(hr, b, out s);
			}

			public int accSelect(AccSELFLAG flagsSelect, int elem)
			{
				return _F.accSelect(_iptr, (int)flagsSelect, elem);
			}

			//public int accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, int elem)
			//{
			//	return _F.accLocation(_iptr, out pxLeft, out pyTop, out pcxWidth, out pcyHeight, elem);
			//}

			public int accLocation(int elem, out RECT r)
			{
				int hr = _F.accLocation(_iptr, out var x, out var y, out var wid, out var hei, elem);
				r = hr == 0 ? new RECT(x, y, wid, hei, true) : default;
				return hr;
			}

			/// <summary>
			/// Calls accNavigate(out VARIANT) and gets IAccessible/elem from VARIANT using standard pattern which may involve get_accParent/get_accChild.
			/// Unlike the higher-level function Acc.Navigate: Does not support PARENT and CHILD. Does not apply a workaround when fails.
			/// </summary>
			public int accNavigate(AccNAVDIR navDir, int varStart, out _Acc a)
			{
				a = default;

				Debug.Assert(!(navDir == AccNAVDIR.PARENT || navDir == AccNAVDIR.CHILD)); //our special navdirs, not supported by accNavigate
				Debug.Assert(!(varStart != 0 && (navDir == AccNAVDIR.FIRSTCHILD || navDir == AccNAVDIR.LASTCHILD)));

				int hr = _F.accNavigate(_iptr, (int)navDir, varStart, out var v);
				if(hr == 0 && v.vt != 0) {
					if(varStart == 0 && v.vt == Api.VARENUM.VT_I4 && v.value != 0 && navDir >= AccNAVDIR.UP && navDir <= AccNAVDIR.PREVIOUS) {
						hr = get_accParent(out var iaccParent);
						if(hr == 0) {
							hr = iaccParent.get_accChild(v.value, out a.a);
							if(hr == 0) iaccParent.Dispose();
							else { hr = 0; a.a = iaccParent; a.elem = v.value; } //note: some AO return wrong childid. Then a will be invalid. Noticed only 1 such object. hr are various. Never mind.
						}
					} else {
						if(v.vt == Api.VARENUM.VT_I4 && v.value == 0) Debug_.Print("AO bug: VT_I4 value 0"); //eg .NET submenu-item
						else hr = FromVARIANT(ref v, out a, true);
					}
				}
				if(hr == 0 && a.a.Is0) hr = Api.S_FALSE;
				return hr;
			}

			public int accDoDefaultAction(VARIANT elem) //info: VT_BSTR used by DoJavaAction
			{
				return _F.accDoDefaultAction(_iptr, elem);
			}

			public int put_accName(int elem, string szName)
			{
				return _F.put_accName(_iptr, elem, szName);
			}

			public int put_accValue(int elem, string szValue)
			{
				return _F.put_accValue(_iptr, elem, szValue);
			}

			static ConcurrentDictionary<LPARAM, _Vtbl> s_vtbls = new ConcurrentDictionary<LPARAM, _Vtbl>();
			//note: for key use long or LPARAM, not IntPtr. GetOrAdd would generate IntPtr boxing garbage because IntPtr does not implement IEquatable<T>.

			_Vtbl _F
			{
				get
				{
					if(_iptr == default) throw new ObjectDisposedException(nameof(Acc));
					return s_vtbls.GetOrAdd(*(IntPtr*)_iptr, vtbl => new _Vtbl(vtbl));
				}
			}

			class _Vtbl
			{
				public _Vtbl(long vtbl)
				{
#if DEBUG
					//tested: 2 IAccessible VTBLs are used for all AO. Tested on Win 10 and 7. Enumerated all AO in all windows.
					//	Plus one of our IJAccessible.
					//	If in the future will notice more but not too many, just edit 'if(n>x)'.
					//	But note, it can be because of invalid object, eg used after fully releasing.
					int n = s_vtbls.Count; if(n > 2) Debug_.Print("many VTBLs: " + (n + 1));
#endif
					var a = (IntPtr*)vtbl;
					Util.Marshal_.GetDelegate(a[7], out get_accParent);
					Util.Marshal_.GetDelegate(a[8], out get_accChildCount);
					Util.Marshal_.GetDelegate(a[9], out get_accChild);
					Util.Marshal_.GetDelegate(a[10], out get_accName);
					Util.Marshal_.GetDelegate(a[11], out get_accValue);
					Util.Marshal_.GetDelegate(a[12], out get_accDescription);
					Util.Marshal_.GetDelegate(a[13], out get_accRole);
					Util.Marshal_.GetDelegate(a[14], out get_accState);
					Util.Marshal_.GetDelegate(a[15], out get_accHelp);
					//get_accHelpTopicT skipped
					Util.Marshal_.GetDelegate(a[17], out get_accKeyboardShortcut);
					Util.Marshal_.GetDelegate(a[18], out get_accFocus);
					Util.Marshal_.GetDelegate(a[19], out get_accSelection);
					Util.Marshal_.GetDelegate(a[20], out get_accDefaultAction);
					Util.Marshal_.GetDelegate(a[21], out accSelect);
					Util.Marshal_.GetDelegate(a[22], out accLocation);
					Util.Marshal_.GetDelegate(a[23], out accNavigate);
					Util.Marshal_.GetDelegate(a[24], out accHitTest);
					Util.Marshal_.GetDelegate(a[25], out accDoDefaultAction);
					Util.Marshal_.GetDelegate(a[26], out put_accName);
					Util.Marshal_.GetDelegate(a[27], out put_accValue);
				}

				internal readonly get_accParentT get_accParent;
				internal readonly get_accChildCountT get_accChildCount;
				internal readonly get_accChildT get_accChild;
				internal readonly get_accNameT get_accName;
				internal readonly get_accNameT get_accValue;
				internal readonly get_accNameT get_accDescription;
				internal readonly get_accRoleT get_accRole;
				internal readonly get_accRoleT get_accState;
				internal readonly get_accNameT get_accHelp;
				//get_accHelpTopicT skipped
				internal readonly get_accNameT get_accKeyboardShortcut;
				internal readonly get_accFocusT get_accFocus;
				internal readonly get_accFocusT get_accSelection;
				internal readonly get_accNameT get_accDefaultAction;
				internal readonly accSelectT accSelect;
				internal readonly accLocationT accLocation;
				internal readonly accNavigateT accNavigate;
				internal readonly accHitTestT accHitTest;
				internal readonly accDoDefaultActionT accDoDefaultAction;
				internal readonly put_accNameT put_accName;
				internal readonly put_accNameT put_accValue;

				internal delegate int get_accParentT(IntPtr obj, out IntPtr ppdispParent);
				internal delegate int get_accChildCountT(IntPtr obj, out int pcountChildren);
				internal delegate int get_accChildT(IntPtr obj, VARIANT varChild, out IntPtr ppdispChild);
				internal delegate int get_accNameT(IntPtr obj, VARIANT varChild, out BSTR pszName);
				internal delegate int get_accRoleT(IntPtr obj, VARIANT varChild, out VARIANT pvarRole);
				internal delegate int get_accFocusT(IntPtr obj, out VARIANT pvarChild);
				internal delegate int accSelectT(IntPtr obj, int flagsSelect, VARIANT varChild);
				internal delegate int accLocationT(IntPtr obj, out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, VARIANT varChild);
				internal delegate int accNavigateT(IntPtr obj, int navDir, VARIANT varStart, out VARIANT pvarEndUpAt);
				internal delegate int accHitTestT(IntPtr obj, int xLeft, int yTop, out VARIANT pvarChild);
				internal delegate int accDoDefaultActionT(IntPtr obj, VARIANT varChild);
				internal delegate int put_accNameT(IntPtr obj, VARIANT varChild, [MarshalAs(UnmanagedType.BStr)] string szName);
			}

			/// <summary>
			/// Creates child IAccessible/elem from VARIANT.
			/// If VT_DISPATCH, gets IAccessible and releases the IDispatch. Sets a.elem=0.
			/// If VT_I4, sets non-zero a.elem and sets a.a=this. Does not AddRef.
			/// Clears the VARIANT (releases IDispatch etc, sets vt=0).
			/// Returns HRESULT.
			/// If vt is not VT_DISPATCH/VT_I4 or value is 0, asserts and returns E_FAIL.
			/// </summary>
			/// <param name="v">The input VARIANT. This function disposes it.</param>
			/// <param name="a">
			/// a.a - result IAccessible. If VT_DISPATCH, it is a child of this. If VT_I4, it is this.
			/// a.elem - if VT_I4, receives non-zero child id, else 0.
			/// </param>
			/// <param name="tryGetObjectFromId">If VT_I4, try to call get_accChild. If it succeeds, sets a.elem = 0.</param>
			internal int FromVARIANT(ref VARIANT v, out _Acc a, bool tryGetObjectFromId = false)
			{
				a = default;
				int hr = 0;
				Debug.Assert(v.value != 0); //bug in our code or AO
				if(v.value == 0) hr = Api.E_FAIL;
				else {
					switch(v.vt) {
					case Api.VARENUM.VT_DISPATCH:
						hr = FromIDispatch(v.value, out a.a);
						break;
					case Api.VARENUM.VT_I4: //info: AccessibleChildren does not AddRef
						a.elem = v.value;
						if(a.elem == 0) hr = Api.E_FAIL;
						else if(tryGetObjectFromId && 0 == get_accChild(a.elem, out a.a)) a.elem = 0;
						else a.a = this;
						break;
					default:
						Debug.Assert(false);
						v.Dispose();
						hr = Api.E_FAIL;
						break;
					}
				}
				v.vt = 0;
				Debug_.PrintIf(hr != 0, $"0x{hr:X}");
				return hr;
			}

			/// <summary>
			/// Calls QueryInterface to get IAccessible from IDispatch idisp. Releases idisp.
			/// Returns HRESULT.
			/// </summary>
			/// <param name="idisp">IDispatch as IntPtr. If == default, returns Api.E_FAIL.</param>
			/// <param name="iacc">Result.</param>
			internal static int FromIDispatch(IntPtr idisp, out IAccessible iacc)
			{
				iacc = default;
				if(idisp == default) return Api.E_FAIL;
				int hr = Marshal.QueryInterface(idisp, ref Api.IID_IAccessible, out IntPtr ip);
				Marshal.Release(idisp);
				if(hr == 0 && ip == default) hr = Api.E_FAIL;
				if(hr == 0) iacc = new IAccessible(ip);
				return hr;
			}

			public string ToString(int elem, int level = 0)
			{
				using(var a = new Acc(this, elem, addRef: true)) return a.ToString(level);
			}

			public override string ToString()
			{
				return ToString(0);
			}

			//TODO: no bug if the AO retrieved by our in-proc dll. Probably standard out-proc AO are not proxy.
			[MethodImpl(MethodImplOptions.NoInlining)]
			void _WorkaroundGetToolbarButtonName(int elem, AccROLE role, out string R)
			{
				//get_accName bug: 64-bit process cannot get standard toolbar button name from 32-bit process if it is tooltip text. Tested on Win 10 and 7.
				//Normally the toolbar control receives these messages. No WM_NOTIFY when bug.
				//TB_GETBUTTON
				//TB_GETBUTTONTEXTW (lParam is NULL, to get text length; returns -1)
				//TB_GETTOOLTIPS
				//TTM_GETTEXTW (received by the tooltips window)
				//WM_NOTIFY (missing when bug)

				//tested: UI Automation does not have this bug.

				//rejected: try workaround even when role is unknown. Now returns immediately.
				//	Usually we get role before name, so it should be known in most cases.
				//	Some comments below are not updated after this rejection.

				R = null;
				if(!Ver.Is64BitProcess) return;

				//return fast if we know role
				switch(role) {
				//case 0: break; //still unknown
				case AccROLE.PUSHBUTTON: if(elem == 0) return; break;
				case AccROLE.SPLITBUTTON: case AccROLE.CHECKBUTTON: case AccROLE.RADIOBUTTON: break;
				default: return;
				}

				//is it a standard toolbar control in a 32-bit process?
				if(0 != GetWnd(out var w)) return; //usually fast for these roles
				if(!w.ClassNameIs("*ToolbarWindow32*")) return; //fast
				if(w.Is64Bit) return; //slower
				if(w.Send(Api.WM_GETOBJECT, 0, (int)AccOBJID.QUERYCLASSNAMEIDX) != 65536 + 12) return; //slow but not too much. //note: RealGetWindowClass does not work

				//avoid getting role if possible. Makes this func 2 times slower. Slower even for SEPARATOR etc.
				//if(elem <= 0) {
				//	if(role == 0) {
				//		if(0 != GetRole(elem, out role)) return; //fast for WINDOW, TITLEBAR, GRIP. Slower for TOOLBAR. Slow for SPLITBUTTON.
				//	}
				//	if(role != AccROLE.SPLITBUTTON) return; //WINDOW, TOOLBAR, TITLEBAR, GRIP
				//}

				//info: role can be:
				//	PUSHBUTTON, CHECKBUTTON, maybe RADIOBUTTON (not tested). Has Elem.
				//	SPLITBUTTON. No Elem.
				//	SEPARATOR. Has Elem. Usually idCommand is -1 or 0, therefore does not pass the if(idCommand <= 0) return;.
				//	MENUITEM, RADIOBUTTON. Has Elem. These are hidden buttons. Usually idCommand is -1 or 0.
				//	Any standard window part that has no name - WINDOW, TOOLBAR, TITLEBAR, GRIP. No Elem.
				//	This func not called for other standard window parts because they have name, eg PUSHBUTTON of TITLEBAR.

				Process_.Memory pm = null;
				try {
					const int pmSize = 0x1000; //4 KB
					pm = new Process_.Memory(w, pmSize);
					//get button index
					int buttonIndex;
					if(elem > 0) { //PUSHBUTTON, SEPARATOR, etc
						buttonIndex = elem - 1;
					} else { //SPLITBUTTON
						if(0 != accLocation(elem, out var rect)) return; //quite slow, but >2 times faster tan getting SPLITBUTTON role
						rect.left += rect.Width / 2; rect.top += rect.Height / 2; //we need POINT in r center
						if(!pm.Write(&rect, 8)) return;
						buttonIndex = w.Send(TB_HITTEST, 0, pm.Mem);
						if(buttonIndex < 0) return;
					}

					//get button command id from index
					int idCommand;
					if(0 == w.Send(TB_GETBUTTON, buttonIndex, pm.Mem)) return; //pm.Mem is &TBBUTTON_32, and we'll read only its idCommand, offset 4
					if(!pm.Read(&idCommand, 4, 4)) return;
					if(idCommand <= 0) return;
					//never mind: PUSHBUTTON can have 0 id, but it is rare. More often 0 id is used for SEPARATOR, although in most cases -1.
					//if(idCommand < 0) return; if(idCommand == 0 && elem > 0 && role != AccROLE.PUSHBUTTON) return;

					//try to get non-tooltip text length. Returns -1 if no such text.
					if(w.Send(TB_GETBUTTONTEXTW, idCommand) >= 0) return;

					//try to get toolbar's tooltips window
					Wnd tt = (Wnd)w.Send(TB_GETTOOLTIPS);
					if(tt.Is0) return; //rare. Probably some old-looking window, eg HTML Help Workshop. Then get_accName in 32-bit process usually/randomly cannot get name too.

					//get text from tooltip control using TTM_GETTEXTW
					if(tt.ProcessId != w.ProcessId) return;
					var ti = new TOOLINFO_32() { cbSize = sizeof(TOOLINFO_32) };
					ti.hwnd = (LPARAM)w;
					ti.uId = idCommand;
					ti.lpszText = (int)pm.Mem + ti.cbSize;
					if(!pm.Write(&ti, ti.cbSize)) return;
					int nCharsBuffer = (pmSize - ti.cbSize) / 2;
					tt.Send(TTM_GETTEXTW, nCharsBuffer, pm.Mem);
					R = pm.LibReadUnicodeStringCached(nCharsBuffer, ti.cbSize, true);

					//speed: get_accName 110 mcs (when fails), workaround PUSHBUTTON 110 mcs, workaround SPLITBUTTON 160-240 mcs.
				}
				catch(Exception ex) { Debug_.Print(ex.Message); }
				finally { pm?.Dispose(); }
			}

			#region api

			const uint TB_HITTEST = 0x445;
			const uint TB_GETBUTTON = 0x417;
			const uint TB_GETBUTTONTEXTW = 0x44B;
			const uint TB_GETTOOLTIPS = 0x423;
			const uint TTM_GETTEXTW = 0x438;

			struct TOOLINFO_32
			{
				public int cbSize;
				public uint uFlags;
				public int hwnd;
				public int uId;
				public RECT rect;
				public int hinst;
				public int lpszText;
				public int lParam;
				public int lpReserved;
			}

			//struct TBBUTTON_32
			//{
			//	public int iBitmap;
			//	public int idCommand;
			//	public byte fsState;
			//	public byte fsStyle;
			//	byte bReserved_0, bReserved_1;
			//	public int dwData;
			//	public int iString;
			//}

			//tested: TB_GETBUTTONINFO does not get string.

			#endregion
		}

		/// <summary>
		/// Converts BSTR to string and disposes the BSTR.
		/// Normalizes HRESULT: if DISP_E_MEMBERNOTFOUND or E_NOTIMPL, makes S_FALSE.
		/// If hr is not 0, sets s = "" (never null).
		/// </summary>
		static int _BstrToString(int hr, BSTR b, out string s)
		{
			if(hr == 0) s = b.ToStringAndDispose() ?? "";
			else {
				if(!b.Is0) b.Dispose(); //rare, but noticed few cases
				s = "";
				switch(hr) {
				case Api.S_FALSE: case Api.DISP_E_MEMBERNOTFOUND: case Api.E_NOTIMPL: hr = 1; break;
					//default: s = null; break; //rejected: it makes 'also: o => o.Value.StartsWith(...)' etc unsafe (need 'also: o => o.Value?.StartsWith(...) ?? false'). Anyway, even S_FALSE can mean that the object is invalid etc.
				}
				//DISP_E_MEMBERNOTFOUND: many many. E_NOTIMPL: many.
				//note: 0x80070005 (access denied) when trying to get value of a password field.
			}
			return hr;
		}


	}
}
