using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using IAccessible = Au.Types.Api.IAccessible;
using VarInt = Au.Types.Api.VarInt;
using AccNAVDIR = Au.Types.Api.AccNAVDIR;

namespace Au
{
public partial class AToolbar
{
	bool _WmGetobject(LPARAM wParam, LPARAM lParam, out LPARAM result) {
		result = default;
		var oid = (AccOBJID)(uint)lParam;
		if (oid != AccOBJID.CLIENT) return false;
		_acc ??= new(this);
		var accIP = Marshal.GetIUnknownForObject(_acc);
		result = Api.LresultFromObject(typeof(IAccessible).GUID, wParam, accIP);
		Marshal.Release(accIP);
		return true;
	}
	_Accessible _acc;
	
	class _Accessible : IAccessible, IReflect {
		AToolbar _tb;
		IAccessible _sao;
		AWnd _w;
		
		public _Accessible(AToolbar tb) {
			_tb = tb;
			_w = tb._w;
			Api.CreateStdAccessibleObject(_w, AccOBJID.CLIENT, typeof(IAccessible).GUID, out _sao);
		}
	
		#region IAccessible

		IAccessible IAccessible.get_accParent() => _sao.get_accParent();

		int IAccessible.get_accChildCount() => _tb._a.Count;

		int IAccessible.get_accChild(VarInt varChild, out object ppdispChild) { ppdispChild = null; return 1; }

		string IAccessible.get_accName(VarInt varChild) => _B(varChild, out var b) ? b.Text : _tb.Name;
		
		string IAccessible.get_accValue(VarInt varChild) => null;

		string IAccessible.get_accDescription(VarInt varChild) => _B(varChild, out var b) ? null : "Floating toolbar";

		VarInt IAccessible.get_accRole(VarInt varChild) {
			var r = !_B(varChild, out var b) ? AccROLE.TOOLBAR : b.ItemType switch { TBItemType.Separator => AccROLE.SEPARATOR, TBItemType.Group => AccROLE.GROUPING, TBItemType.Menu => AccROLE.BUTTONMENU, _ => AccROLE.BUTTON };
			return (int)r - 1;
		}

		VarInt IAccessible.get_accState(VarInt varChild) {
			AccSTATE r = 0;
			if(!_w.IsEnabled()) r |= AccSTATE.DISABLED;
			if (!_B(varChild, out var b)) {
				if(!_w.IsVisible) r |= AccSTATE.INVISIBLE;
			} else {
				if(b.IsSeparatorOrGroup_) r |= AccSTATE.DISABLED;
				//SHOULDDO: if offscreen, r |= AccSTATE.INVISIBLE | AccSTATE.OFFSCREEN;
				//SHOULDDO: AccSTATE.HOTTRACKED;
			}
			return (int)r - 1;
		}

		string IAccessible.get_accHelp(VarInt varChild) => _B(varChild, out var b) ? b.Tooltip ?? b.File : null;

		int IAccessible.get_accHelpTopic(out string pszHelpFile, VarInt varChild) => throw new NotImplementedException();

		string IAccessible.get_accKeyboardShortcut(VarInt varChild) => null;

		object IAccessible.get_accFocus() => null;

		object IAccessible.get_accSelection() => null;

		string IAccessible.get_accDefaultAction(VarInt varChild) => _B(varChild, out var b) && b.clicked!=null ? "Click" : null;

		void IAccessible.accSelect(AccSELFLAG flagsSelect, VarInt varChild) => throw new NotImplementedException();

		void IAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, VarInt varChild) {
			if (!_B(varChild, out var b)) {
				_sao.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, varChild);
			} else {
				var r = b.rect; _w.MapClientToScreen(ref r);
				pxLeft = r.left; pyTop = r.top; pcxWidth = r.Width; pcyHeight = r.Height;
			}
		}

		object IAccessible.accNavigate(AccNAVDIR navDir, VarInt varStart) {
			int i=varStart;
			var a=_tb._a;
			if (navDir == AccNAVDIR.FIRSTCHILD || navDir == AccNAVDIR.LASTCHILD) {
				if (i == -1) return navDir == AccNAVDIR.FIRSTCHILD ? 1 : a.Count;
			} else {
				if (i == -1) return _sao.accNavigate(navDir, varStart);
				switch (navDir) {
				case AccNAVDIR.PREVIOUS: 
					if (i > 0) return i;
					break;
				case AccNAVDIR.NEXT:
					if (++i < a.Count) return i+1;
					break;
				}
			}
			return null;
		}

		VarInt IAccessible.accHitTest(int xLeft, int yTop) {
			POINT p=new(xLeft, yTop); _w.MapScreenToClient(ref p);
			if (!_w.ClientRect.Contains(p)) return _sao.accHitTest(xLeft, yTop);
			return _tb._HitTest(p);
		}

		void IAccessible.accDoDefaultAction(VarInt varChild) {
			if(!_B(varChild, out var b) || b.clicked==null) return;
			_w.Post(Api.WM_USER+50, (int)varChild);
		}

		void IAccessible.put_accName(VarInt varChild, string szName) => throw new NotImplementedException();

		void IAccessible.put_accValue(VarInt varChild, string szValue) => throw new NotImplementedException();
		
		bool _B(VarInt varChild, out AToolbar.ToolbarItem b) {
			int i=varChild;
			if (i == -1) { b=null; return false; }
			b=_tb._a[i]; return true;
		}

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
}