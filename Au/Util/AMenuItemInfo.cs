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

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Gets item id, text and other info of a classic menu.
	/// </summary>
	public class AMenuItemInfo
	{
		IntPtr _hm;
		int _id;
		bool _isSystem;
		AWnd _ow;

		private AMenuItemInfo() { }

		/// <summary>
		/// Gets info of a menu item from point.
		/// Returns null if fails, eg the point is not in the menu or the window is hung.
		/// </summary>
		/// <param name="pScreen">Point in screen coordinates.</param>
		/// <param name="w">Popup menu window, class name "#32768".</param>
		/// <param name="msTimeout">Timeout (ms) to use when the window is busy or hung.</param>
		public static AMenuItemInfo FromXY(POINT pScreen, AWnd w, int msTimeout = 5000)
		{
			if(!w.SendTimeout(msTimeout, out var hm, Api.MN_GETHMENU)) return null;
			int i = Api.MenuItemFromPoint(default, hm, pScreen); if(i == -1) return null;
			i = Api.GetMenuItemID(hm, i); if(i == -1 || i == 0) return null;
			return new AMenuItemInfo { _hm=hm, _id=i};
		}

		/// <summary>
		/// Gets info of a menu item from mouse.
		/// Returns null if fails, eg the point is not in a menu or the window is hung.
		/// </summary>
		/// <param name="msTimeout">Timeout (ms) to use when the window is busy or hung.</param>
		public static AMenuItemInfo FromXY(int msTimeout = 5000)
		{
			var p = AMouse.XY;
			var w = AWnd.FromXY(p, WXYFlags.Raw);
			if(!w.ClassNameIs("#32768")) return null;
			return FromXY(p, w, msTimeout);
		}

		/// <summary>
		/// Gets the popup menu handle.
		/// </summary>
		public IntPtr MenuHandle => _hm;

		/// <summary>
		/// Gets menu item id.
		/// </summary>
		public int ItemId => _id;

		/// <summary>
		/// Gets the owner window of the popup menu.
		/// </summary>
		public AWnd OwnerWindow => _OwnerSystem().ow;

		/// <summary>
		/// true if it is a system menu, eg when right-clicked the title bar of a window.
		/// </summary>
		public bool IsSystem => _OwnerSystem().sys;

		(AWnd ow, bool sys) _OwnerSystem()
		{
			if(_ow.Is0 && AWnd.More.GetGUIThreadInfo(out var g)) {
				_ow = g.hwndMenuOwner;
				_isSystem = g.flags.Has(Native.GUI.SYSTEMMENUMODE);
			}
			return (_ow, _isSystem);
		}

		/// <summary>
		/// Gets menu item text.
		/// Returns null if failed.
		/// </summary>
		/// <param name="removeHotkey">If contains '\t' character, get substring before it.</param>
		/// <param name="removeAmp">Call <see cref="AStringUtil.RemoveUnderlineChar"/>.</param>
		public string GetText(bool removeHotkey, bool removeAmp) => GetText(_hm, _id, false, removeHotkey, removeAmp);

		/// <summary>
		/// Gets menu item text.
		/// Returns null if failed.
		/// </summary>
		/// <param name="menuHandle"></param>
		/// <param name="id"></param>
		/// <param name="byIndex">id is 0-based index. For example you can use it to get text of a submenu-item, because such items usually don't have id.</param>
		/// <param name="removeHotkey">If contains '\t' character, get substring before it.</param>
		/// <param name="removeAmp">Call <see cref="AStringUtil.RemoveUnderlineChar"/>.</param>
		public static unsafe string GetText(IntPtr menuHandle, int id, bool byIndex, bool removeHotkey, bool removeAmp)
		{
			Api.MENUITEMINFO mi = default;
			mi.cbSize = sizeof(Api.MENUITEMINFO);
			mi.fMask = Api.MIIM_TYPE;
			if(!Api.GetMenuItemInfo(menuHandle, id, byIndex, ref mi)) return null; //get required buffer size
			if(mi.cch == 0) return "";
			mi.cch++; //string length -> buffer length
			var b = AMemoryArray.Char_(ref mi.cch);
			fixed (char* p = b.A) {
				mi.dwTypeData = p;
				if(!Api.GetMenuItemInfo(menuHandle, id, byIndex, ref mi)) return null;
			}
			var s = b.ToString();
			if(removeHotkey) { int i = s.IndexOf('\t'); if(i >= 0) s = s.Remove(i); }
			if(removeAmp) s = AStringUtil.RemoveUnderlineChar(s);
			return s;
		}
	}
}
