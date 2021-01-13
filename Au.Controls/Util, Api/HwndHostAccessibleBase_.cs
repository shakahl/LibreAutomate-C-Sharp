using Au.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Windows;

namespace Au.Controls
{
	//public //if non-public, GetIDispatchForObject throws, and with GetIUnknownForObject does not work too.
	//	See https://docs.microsoft.com/en-us/dotnet/standard/native-interop/qualify-net-types-for-interoperation.
	//	But somehow works if implements IReflect, even if all functions just return default. Winforms use it.
	abstract class HwndHostAccessibleBase_ : IAccessible, IReflect
	{
		FrameworkElement _e;
		IAccessible _sao;
		AWnd _w;

		/// <param name="e">HwndHost or its container control (if the HwndHost is part of the control).</param>
		/// <param name="w">Native control hosted by the HwndHost.</param>
		public HwndHostAccessibleBase_(FrameworkElement e, AWnd w) {
			_e = e;
			_w = w;
			CreateStdAccessibleObject(_w, AccOBJID.CLIENT, typeof(IAccessible).GUID, out _sao);
		}

		[DllImport("oleacc.dll", PreserveSig = true)]
		internal static extern int CreateStdAccessibleObject(AWnd hwnd, AccOBJID idObject, in Guid riid, out IAccessible ppvObject);

		#region IAccessible

		IAccessible IAccessible.get_accParent() => _sao.get_accParent();

		/// <summary>
		/// Returns 0.
		/// </summary>
		public virtual int ChildCount => 0;
		int IAccessible.get_accChildCount() => ChildCount;

		//[PreserveSig]
		int IAccessible.get_accChild(VarInt varChild, [MarshalAs(UnmanagedType.IDispatch)] out object ppdispChild) {
			ppdispChild = null;
			return 1;
			//currently this class supports only simple element children.
		}

		/// <summary>
		/// Returns FrameworkElement.Name.
		/// </summary>
		public virtual string Name(int child) => _e.Name;
		string IAccessible.get_accName(VarInt varChild) => Name(varChild);

		/// <summary>
		/// Returns null.
		/// </summary>
		public virtual string Value(int child) => null;
		string IAccessible.get_accValue(VarInt varChild) => Value(varChild);

		/// <summary>
		/// Returns null.
		/// </summary>
		public virtual string Description(int child) => null;
		string IAccessible.get_accDescription(VarInt varChild) => Description(varChild);

		public abstract AccROLE Role(int child);
		VarInt IAccessible.get_accRole(VarInt varChild) => (int)Role(varChild) - 1;

		/// <summary>
		/// If self (child -1), returns combination of FOCUSABLE, FOCUSED, DISABLED, INVISIBLE. Else returns 0.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public virtual AccSTATE State(int child) {
			if (child != -1) return 0;
			AccSTATE r = 0;
			if (_e.Focusable) {
				r |= AccSTATE.FOCUSABLE;
				if (AWnd.ThisThread.IsFocused(_w) || _e.IsKeyboardFocused) r |= AccSTATE.FOCUSED;
			}
			if (!_e.IsEnabled) r |= AccSTATE.DISABLED;
			if (!_e.IsVisible) r |= AccSTATE.INVISIBLE;
			return r;
		}
		VarInt IAccessible.get_accState(VarInt varChild) => (int)State(varChild) - 1;

		/// <summary>
		/// Returns null.
		/// </summary>
		public virtual string Help(int child) => null;
		string IAccessible.get_accHelp(VarInt varChild) => Help(varChild);

		int IAccessible.get_accHelpTopic(out string pszHelpFile, VarInt varChild) => throw new NotImplementedException();

		/// <summary>
		/// Returns null.
		/// </summary>
		public virtual string KeyboardShortcut(int child) => null;
		string IAccessible.get_accKeyboardShortcut(VarInt varChild) => KeyboardShortcut(varChild);

		/// <summary>
		/// Returns -1.
		/// Called only if the control is focused.
		/// </summary>
		public virtual int FocusedChild => -1;
		object IAccessible.get_accFocus() => AWnd.ThisThread.IsFocused(_w) ? FocusedChild + 1 : null;
		//object IAccessible.get_accFocus() {
		//	AOutput.Write("get_accFocus", AWnd.ThisThread.IsFocused(_w));//SHOULDDO. Now this not called for KTreeView. The native control normally is never focused. IAccessible.get_accFocus called by QM2 returns unknown error 0x80131509.
		//	return AWnd.ThisThread.IsFocused(_w) ? FocusedChild + 1 : null;
		//}

		/// <summary>
		/// Returns null.
		/// </summary>
		public virtual List<int> SelectedChildren => null;
		object IAccessible.get_accSelection() {
			var a = SelectedChildren;
			if (a.NE_()) return null;
			if (a.Count == 1) return a[0] + 1;
			return new _EnumSelected(a);
		}

		unsafe class _EnumSelected : IEnumVarInt
		{
			List<int> _a;
			int _i;
			public _EnumSelected(List<int> a) { _a = a; }

			public int Next(int celt, VarInt* rgVar, int* pCeltFetched) {
				int n = Math.Min(celt, _a.Count - _i);
				if (pCeltFetched != null) *pCeltFetched = n;
				for (int i = 0; i < n;) rgVar[i++] = _a[_i++];
				return n == celt ? 0 : 1;
			}

			public int Skip(int celt) {
				long i = (long)_i + (uint)celt;
				if (i > _a.Count) return 1;
				_i = (int)i;
				return 0;
			}

			public void Reset() { _i = 0; }

			public IEnumVarInt Clone() => throw new NotImplementedException();
		}

		[ComImport, Guid("00020404-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal unsafe interface IEnumVarInt
		{
			[PreserveSig] int Next(int celt, VarInt* rgVar, int* pCeltFetched);
			[PreserveSig] int Skip(int celt);
			void Reset();
			IEnumVarInt Clone();
		}

		/// <summary>
		/// Returns null.
		/// </summary>
		public virtual string DefaultAction(int child) => null;
		string IAccessible.get_accDefaultAction(VarInt varChild) => DefaultAction(varChild);

		/// <summary>
		/// Does nothing.
		/// Not called for self.
		/// </summary>
		public virtual void SelectChild(AccSELFLAG flagsSelect, int child) { }
		void IAccessible.accSelect(AccSELFLAG flagsSelect, VarInt varChild) {
			int child = varChild;
			if (flagsSelect.Has(AccSELFLAG.TAKEFOCUS)) Api.SetFocus(_w); // _e.Focus();//SHOULDDO: now Api.SetFocus makes KTreeView item nonfocused (works like focused but displayed like not)
			if (child == -1) {
				if (flagsSelect is not (AccSELFLAG.TAKEFOCUS or 0)) throw new ArgumentException();
			} else {
				SelectChild(flagsSelect, child);
			}
		}

		/// <summary>
		/// Returns default.
		/// Return child rect in client area.
		/// Not called for self.
		/// </summary>
		public virtual RECT ChildRect(int child) => default;
		void IAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, VarInt varChild) {
			int child = varChild;
			if (child == -1) {
				_sao.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, varChild);
			} else {
				var r = ChildRect(child);
				_w.MapClientToScreen(ref r);
				pxLeft = r.left; pyTop = r.top; pcxWidth = r.Width; pcyHeight = r.Height;
			}
		}

		/// <summary>
		/// Returns null.
		/// If self (childStart==-1), navDir is FIRSTCHILD or LASTCHILD, else navDir is any except these.
		/// </summary>
		public virtual int? Navigate(AccNAVDIR navDir, int childStart) => null;
		object IAccessible.accNavigate(AccNAVDIR navDir, VarInt varStart) {
			int i = varStart;
			if (navDir == AccNAVDIR.FIRSTCHILD || navDir == AccNAVDIR.LASTCHILD) {
				if (i != -1) return null;
			} else {
				if (i == -1) return _sao.accNavigate(navDir, varStart);//never mind: gets some not adjacent AO
			}
			var v = Navigate(navDir, i);
			if (v == null) return null;
			return v.Value + 1;
		}

		/// <summary>
		/// Returns -1.
		/// x y are in client area.
		/// Not called if not in client area.
		/// </summary>
		public virtual int HitTest(int x, int y) => -1;
		VarInt IAccessible.accHitTest(int xLeft, int yTop) {
			POINT p = (xLeft, yTop); _w.MapScreenToClient(ref p);
			if (!_w.ClientRect.Contains(p)) return _sao.accHitTest(xLeft, yTop);
			return HitTest(p.x, p.y);
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		public virtual void DoDefaultAction(int child) { }
		void IAccessible.accDoDefaultAction(VarInt varChild) => DoDefaultAction(varChild);

		void IAccessible.put_accName(VarInt varChild, string szName) => throw new NotImplementedException();

		void IAccessible.put_accValue(VarInt varChild, string szValue) => throw new NotImplementedException();

		#endregion


		#region IReflect

		FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr) => null;
		FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) => null;
		MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr) => null;
		MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr) => null;
		MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr) => null;
		MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers) => null;
		MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) => null;
		PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr) => null;
		PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr) => null;
		PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => null;
		object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) => null;
		Type IReflect.UnderlyingSystemType => null;

		#endregion

		[DllImport("oleacc.dll")]
		internal static extern LPARAM LresultFromObject(in Guid riid, LPARAM wParam, IntPtr punk);

		/// <summary>
		/// Call in hook wndproc on WM_GETOBJECT like this: <c>handled = true; return (_acc ??= new _Accessible(this)).WmGetobject(wParam, lParam);</c>.
		/// If lParam is AccOBJID.CLIENT, calls API LresultFromObject(this), else calls API DefWindowProc.
		/// </summary>
		public LPARAM WmGetobject(LPARAM wParam, LPARAM lParam) {
			var oid = (AccOBJID)(uint)lParam;
			//AOutput.Write(oid);
			if (oid != AccOBJID.CLIENT) return Api.DefWindowProc(_w, Api.WM_GETOBJECT, wParam, lParam);
			var accIP = Marshal.GetIUnknownForObject(this);
			//var accIP=Marshal.GetIDispatchForObject(this);
			var r = LresultFromObject(typeof(IAccessible).GUID, wParam, accIP);
			//Marshal.AddRef(accIP); AOutput.Write(Marshal.Release(accIP));
			Marshal.Release(accIP);
			return r;
		}

	}

#pragma warning disable 169
	struct VarInt
	{
		ushort _vt, _1, _2, _3;
		LPARAM _int, _4;
		public static implicit operator VarInt(int i) => new VarInt { _vt = 3, _int = i + 1 };
		public static implicit operator int(VarInt v) {
			if (v._vt == 3) return (int)v._int - 1;
			ADebug.Print($"VarInt vt={v._vt}, value={v._int}, stack={new StackTrace(true)}");
			throw new ArgumentException();
		}
	}
#pragma warning restore 169

	enum AccNAVDIR { UP = 1, DOWN, LEFT, RIGHT, NEXT, PREVIOUS, FIRSTCHILD, LASTCHILD }

	[ComImport, Guid("618736e0-3c3d-11cf-810c-00aa00389b71"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
	interface IAccessible
	{
		IAccessible get_accParent();
		int get_accChildCount();
		[PreserveSig] int get_accChild(VarInt varChild, [MarshalAs(UnmanagedType.IDispatch)] out object ppdispChild);
		string get_accName(VarInt varChild);
		string get_accValue(VarInt varChild);
		string get_accDescription(VarInt varChild);
		VarInt get_accRole(VarInt varChild);
		VarInt get_accState(VarInt varChild);
		string get_accHelp(VarInt varChild);
		int get_accHelpTopic(out string pszHelpFile, VarInt varChild);
		string get_accKeyboardShortcut(VarInt varChild);
		object get_accFocus();
		object get_accSelection();
		string get_accDefaultAction(VarInt varChild);
		void accSelect(AccSELFLAG flagsSelect, VarInt varChild);
		void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, VarInt varChild);
		object accNavigate(AccNAVDIR navDir, VarInt varStart);
		VarInt accHitTest(int xLeft, int yTop);
		void accDoDefaultAction(VarInt varChild);
		void put_accName(VarInt varChild, string szName);
		void put_accValue(VarInt varChild, string szValue);

		//NOTE: although some members are obsolete or useless, don't use default implementation, because then exception at run time.
	}
}
