//Implements MSAA IAccessible.

//Problem: UIA "object from point" returns CLIENT, not item, although has items in tree. MSAA works well.
//	For UIA to work correctly, need to implement UIA (AutomationPeer for control and items).
//		But then problem: if not implemented MSAA, MSAA "object from point" does not return item, although has items in tree.
//			If implemented both, tree contains duplicate subtrees; example - SysListView32 control.
//		Also, cannot implement it directly for HwndHost, because WPF then throws exception. But can implement for its parent.
//	I did not find a way to make both UIA and MSAA work correctly with WPF HwndHost.
//	Good: NVDA now works anyway.

//Strange: QM2 does not find items and client unless checked +invisible, although no invisible state. Never mind.


using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Input;

namespace Au.Controls {
public unsafe partial class AuTreeView {
	
	partial class _HwndHost {
//		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() => null; //removes unused object from MSAA tree, but then no UIA
		
		LPARAM _WmGetobject(LPARAM wParam, AccOBJID oid) {
//			AOutput.Write(oid);
			if(oid!=AccOBJID.CLIENT) return default;
			_acc??=new _Accessible(_tv);
			var accIP=Marshal.GetIUnknownForObject(_acc);
//			var accIP=Marshal.GetIDispatchForObject(_acc);
			var r= LresultFromObject(typeof(IAccessible).GUID, wParam, accIP);
//			Marshal.AddRef(accIP); AOutput.Write(Marshal.Release(accIP));
			Marshal.Release(accIP);
			return r;
		}
		_Accessible _acc;
		
		[DllImport("oleacc.dll")]
		internal static extern LPARAM LresultFromObject(in Guid riid, LPARAM wParam, IntPtr punk);
	}
	
#pragma warning disable 1591 //no doc
	//public //if non-public, GetIDispatchForObject throws, and with GetIUnknownForObject does not work too.
	//	See https://docs.microsoft.com/en-us/dotnet/standard/native-interop/qualify-net-types-for-interoperation.
	//	But somehow works if we implement IReflect, even if all functions just return default. Winforms use it.
	unsafe class _Accessible : IAccessible, IReflect {
		AuTreeView _tv;
		IAccessible _sao;
		AWnd _w;
		
		internal _Accessible(AuTreeView tv) {
			_tv=tv;
			_w=_tv._hh.Wnd;
			CreateStdAccessibleObject(_w, AccOBJID.CLIENT, typeof(IAccessible).GUID, out _sao);
		}
			
		[DllImport("oleacc.dll", PreserveSig=true)]
		internal static extern int CreateStdAccessibleObject(AWnd hwnd, AccOBJID idObject, in Guid riid, out IAccessible ppvObject);
		
//		static void _PrintFunc([CallerFilePath] string f=null, [CallerMemberName] string m=null) { AOutput.Write($"{APath.GetNameNoExt(f)}:{m}"); }
		
		int _Index(in VarInt varChild) {
			int i=varChild;
			if(i==0) return -1;
			if((uint)(--i)<_tv.CountVisible) return i;
			throw new ArgumentException();
		}
		
		ITreeViewItem _Item(in VarInt varChild, out int index) {
			index=_Index(varChild);
			return index<0 ? null : _tv._avi[index].item;
		}
		
		ITreeViewItem _Item(in VarInt varChild) => _Item(varChild, out _);
		
#region IAccessible

		public IAccessible get_accParent() => _sao.get_accParent();

		public int get_accChildCount() => _tv.CountVisible;

		//[PreserveSig]
		public int get_accChild(VarInt varChild, [MarshalAs(UnmanagedType.IDispatch)] out object ppdispChild) {
			ppdispChild=null;
			return 1;
		}

		public string get_accName(VarInt varChild) {
			var k=_Item(varChild);
			//if(k==null) return _sao.get_accName(varChild);
			if(k==null) return _tv.Name;
			return k.DisplayText;
		}

		public string get_accValue(VarInt varChild) => null;

		public string get_accDescription(VarInt varChild) => null;

		public VarInt get_accRole(VarInt varChild) {
			int i=varChild;
			return (int)(i==0 ? AccROLE.TREE : AccROLE.TREEITEM);
		}

		public VarInt get_accState(VarInt varChild) {
			var k=_Item(varChild, out int i);
			AccSTATE r=0;
			bool focusable=_tv.Focusable;
			if(focusable) r|=AccSTATE.FOCUSABLE;
			if(k==null) {
				if(focusable && _tv.IsKeyboardFocused) r|=AccSTATE.FOCUSED;
				if(!_tv.IsEnabled) r|=AccSTATE.DISABLED;
				if(!_tv.IsVisible) r|=AccSTATE.INVISIBLE;
			} else {
				if(focusable && i==_tv._focusedIndex && _tv.IsKeyboardFocused) r|=AccSTATE.FOCUSED;
				if(k.IsSelectable) {
					r|=AccSTATE.SELECTABLE;
					if(_tv.MultiSelect) r|=AccSTATE.MULTISELECTABLE;
					if(_tv.IsSelected(i)) r|=AccSTATE.SELECTED;
				}
				if(k.IsFolder) r|=k.IsExpanded?AccSTATE.EXPANDED:AccSTATE.COLLAPSED;
				var (from, to)=_tv._GetViewRange(); if(i<from||i>=to) r|=AccSTATE.INVISIBLE|AccSTATE.OFFSCREEN;
				if(k.IsDisabled) r|=AccSTATE.DISABLED;
				switch(k.CheckState) { case TVCheck.Checked: case TVCheck.RadioChecked: r|=AccSTATE.CHECKED; break; case TVCheck.Mixed: r|=AccSTATE.MIXED; break; }
				//if(i==_tv._hotIndex) r|=AccSTATE.HOTTRACKED;
				//if(!k.IsEditable) r|=AccSTATE.READONLY;
			}
			return (int)r;
		}

		public string get_accHelp(VarInt varChild) => null;

		public int get_accHelpTopic(out string pszHelpFile, VarInt varChild) => throw new NotImplementedException();

		public string get_accKeyboardShortcut(VarInt varChild) {
			int i=varChild;
			if(i!=0) return null;
			return _sao.get_accKeyboardShortcut(varChild);
		}

		public object get_accFocus() {
			if(!_tv.IsKeyboardFocused) return null;
			return _tv._focusedIndex+1;
		}

		public object get_accSelection() {
			var a=_tv.SelectedIndices;
			if(a.Count==0) return null;
			if(a.Count==1) return a[0]+1;
			return new _EnumSelected(a);
		}
		
		class _EnumSelected : IEnumVarInt {
			List<int> _a;
			int _i;
			public _EnumSelected(List<int> a) { _a=a; }
			
			public int Next(int celt, VarInt* rgVar, int* pCeltFetched) {
				int n=Math.Min(celt, _a.Count-_i);
				if(pCeltFetched!=null) *pCeltFetched=n;
				for(int i=0; i<n; ) rgVar[i++]=_a[_i++]+1;
				return n==celt?0:1;
			}

			public int Skip(int celt) {
				long i=(long)_i+(uint)celt;
				if(i>_a.Count) return 1;
				_i=(int)i;
				return 0;
			}

			public void Reset() { _i=0; }

			public IEnumVarInt Clone() => throw new NotImplementedException();
		}

		public string get_accDefaultAction(VarInt varChild) {
			var k=_Item(varChild);
			if(k==null) return null;
			if(k.IsFolder) return k.IsExpanded ? "Collapse" : "Expand";
			return "Activate";
		}

		public void accSelect(AccSELFLAG flagsSelect, VarInt varChild) {
			int i=_Index(varChild);
			if(i<0) {
				if(flagsSelect!=0 && flagsSelect!=AccSELFLAG.TAKEFOCUS) throw new InvalidOperationException();
				if(flagsSelect!=0) Keyboard.Focus(_tv);
			} else {
				//if(flagsSelect.HasAny(AccSELFLAG.ADDSELECTION|AccSELFLAG.EXTENDSELECTION) && !_tv.MultiSelect) throw new InvalidOperationException();
				//int anchor=Math.Max(_tv.FocusedIndex, 0);
				switch(flagsSelect&(AccSELFLAG.TAKESELECTION|AccSELFLAG.ADDSELECTION|AccSELFLAG.EXTENDSELECTION|AccSELFLAG.REMOVESELECTION)) {
				case 0: break;
				case AccSELFLAG.TAKESELECTION:
					_tv.Select(i, true, unselectOther: true);
					break;
				case AccSELFLAG.REMOVESELECTION:
					_tv.Select(i, false);
					break;
				case AccSELFLAG.ADDSELECTION when _tv.MultiSelect:
					_tv.Select(i, true);
					break;
//				case AccSELFLAG.EXTENDSELECTION: //rarely used
//				case AccSELFLAG.ADDSELECTION|AccSELFLAG.EXTENDSELECTION:
//					break;
//				case AccSELFLAG.REMOVESELECTION|AccSELFLAG.EXTENDSELECTION:
//					break;
				default:
					throw new ArgumentException();
				}
				if(flagsSelect.Has(AccSELFLAG.TAKEFOCUS)) {
					Keyboard.Focus(_tv);
					_tv.FocusedIndex=i;
				}
			}
		}

		public void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, VarInt varChild) {
			int i=_Index(varChild);
			var r=i<0 ? _w.ClientRectInScreen : _tv.GetRectPhysical(i, 0, inScreen: true);
			pxLeft=r.left; pyTop=r.top; pcxWidth=r.Width; pcyHeight=r.Height;
		}

		public object accNavigate(NAVDIR navDir, VarInt varStart) {
			int i=_Index(varStart);
			if(i<0) {
				if(navDir==NAVDIR.FIRSTCHILD || navDir==NAVDIR.LASTCHILD) {
					int n=_tv.CountVisible; if(n==0) return null;
					return navDir==NAVDIR.FIRSTCHILD ? 1 : n;
				}
				return _sao.accNavigate(navDir, varStart);
			}
			switch (navDir) {
			case NAVDIR.PREVIOUS:
			case NAVDIR.UP:
				if(i>0) return i;
				break;
			case NAVDIR.NEXT:
			case NAVDIR.DOWN:
				if(i<_tv.CountVisible-1) return i+2;
				break;
			}
			return null;
		}

		public VarInt accHitTest(int xLeft, int yTop) {
			RECT rc=_w.ClientRect, rs=rc;
			_w.MapClientToScreen(ref rs);
			if(!rs.Contains(xLeft, yTop)) return default;
			return _tv._ItemFromY(yTop-rs.top)+1;
		}

		public void accDoDefaultAction(VarInt varChild) {
			var k=_Item(varChild, out int i);
			if(k==null) return;
			if(k.IsFolder) _tv.Expand(i, null);
			else {
				_tv.Select(i, true, unselectOther: false, focus: true);
				_tv.ItemActivated?.Invoke(_tv, new TVItemEventArgs(k, i));
			}
		}

		public void put_accName(VarInt varChild, string szName) {}

		public void put_accValue(VarInt varChild, string szValue) {}

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
}
}

[ComImport, Guid("618736e0-3c3d-11cf-810c-00aa00389b71"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
interface IAccessible {
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
	object accNavigate(NAVDIR navDir, VarInt varStart);
	VarInt accHitTest(int xLeft, int yTop);
	void accDoDefaultAction(VarInt varChild);
	void put_accName(VarInt varChild, string szName);
	void put_accValue(VarInt varChild, string szValue);
}

#pragma warning disable 169
struct VarInt {
	ushort _vt, _1, _2, _3;
	LPARAM _int, _4;
	public static implicit operator VarInt(int i) => new VarInt { _vt=3, _int=i };
	public static implicit operator int(VarInt v) {
		if(v._vt==3) return (int)v._int;
		ADebug.Print($"VarInt vt={v._vt}, value={v._int}, stack={new StackTrace(true)}");
		throw new ArgumentException();
	}
}
#pragma warning restore 169

[ComImport, Guid("00020404-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal unsafe interface IEnumVarInt {
	[PreserveSig] int Next(int celt, VarInt* rgVar, int* pCeltFetched);
	[PreserveSig] int Skip(int celt);
	void Reset();
	IEnumVarInt Clone();
}

internal enum NAVDIR { UP=1, DOWN, LEFT, RIGHT, NEXT, PREVIOUS, FIRSTCHILD, LASTCHILD }

}
