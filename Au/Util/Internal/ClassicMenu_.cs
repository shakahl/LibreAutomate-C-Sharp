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
using System.Linq;

using Au.Types;

namespace Au.Util
{
	class ClassicMenu_
	{
		class _Item
		{
			public string text;
			public int id;
			public ushort state;
			public short submenu; //1 start, <0 end
		}

		List<_Item> _a = new();
		List<Action<string>> _aa;

		//public ClassicMenu_() {
		//}

		public void Add(int id, string text, bool disable = false, bool check = false, bool bold = false) {
			if (id < 0) throw new ArgumentException();
			_Add(id, text, disable, check, bold);
		}

		void _Add(int id, string text, bool disable = false, bool check = false, bool bold = false) {
			uint v = 0;
			if (disable) v |= Api.MFS_DISABLED;
			if (check) v |= Api.MFS_CHECKED;
			//if (hilite) v |= Api.MFS_HILITE; //does not work as needed. Hilites but does not make focused.
			if (bold) v |= Api.MFS_DEFAULT;
			_a.Add(new() { id = id, text = text, state = (ushort)v });
		}

		public Action<string> this[string text, bool disable = false, bool check = false, bool bold = false] {
			set {
				(_aa ??= new()).Add(value);
				_Add(-_aa.Count, text, disable, check, bold);
			}
		}

		public void Separator() => _a.Add(new() { state = 0x8000 });

		public UsingEndAction Submenu(string text, bool disable = false) {
			_Add(0, text ?? "", disable);
			_a[^1].submenu = 1;
			return new UsingEndAction(() => _a[^1].submenu--);
		}

		public unsafe int Show(AnyWnd owner, bool byCaret = false) {
			if (Api.GetFocus() == default) { //prevent activating the menu window on click
				ATimer.After(1, _ => {
					var w = AWnd.Find("", "#32768", WOwner.ThisThread); //find visible classic menu window of this thread
					if (!w.Is0) w.SetExStyle(WS2.NOACTIVATE, WSFlags.Add);
				});
				//} else {
				//rejected. Rare.
				//if (focusFirst) Api.PostMessage(default, Api.WM_KEYDOWN, (int)KKey.Down, 0);
				//never mind: may not work if a mouse button is pressed, for example on double click.
			}

			POINT p;
			Api.TPMPARAMS tp = default;
			Api.TPMPARAMS* tpp = null;
			if (byCaret && AKeys.More.GetTextCursorRect(out RECT cr, out _)) {
				p = new POINT(cr.left, cr.bottom);
				cr.Inflate(4, 0);
				tp = new Api.TPMPARAMS { cbSize = sizeof(Api.TPMPARAMS), rcExclude = cr };
				tpp = &tp;
			} else p = AMouse.XY;

			var h = Api.CreatePopupMenu();
			Stack<IntPtr> sub = null;
			foreach (var v in _a) {
				if (v.state == 0x8000) { //separator
					Api.AppendMenu(h);
				} else {
					IntPtr hs = default;
					if (v.submenu > 0) {
						hs = Api.CreatePopupMenu();
						Api.AppendMenu(h, Api.MF_POPUP, hs, v.text);
					} else {
						Api.AppendMenu(h, 0, v.id, v.text);
					}

					if (v.state != 0) Api.SetMenuItemInfo(h, v.id, false, new Api.MENUITEMINFO(Api.MIIM_STATE) { fState = v.state });

					if (v.submenu > 0) { (sub ??= new()).Push(h); h = hs; }
				}
				for (int i = v.submenu; i < 0; i++) h = sub.Pop();
			}

			try {
				int id = Api.TrackPopupMenuEx(h, Api.TPM_RETURNCMD, p.x, p.y, owner.Hwnd, tpp);
				if (id < 0) _aa[-id - 1](_a.Find(o => o.id == id).text);
				return id;
			}
			finally {
				Api.DestroyMenu(h);
			}
		}

		/// <summary>
		/// Creates and shows popup menu.
		/// Returns selected item id, or 0 if cancelled.
		/// </summary>
		/// <param name="items">
		/// Menu items. Can be string[], List&lt;string&gt; or string like "One|Two|Three".
		/// Item id can be optionally specified like "1 One|2 Two|3 Three". If missing, uses id of previous non-separator item + 1. Example: "One|Two|100 Three Four" //1|2|100|101.
		/// For separators use null or empty strings: "One|Two||Three|Four".
		/// </param>
		/// <param name="owner">Owner window.</param>
		/// <param name="byCaret">Show by caret (text cursor) position if possible.</param>
		/// <remarks>
		/// The menu is modal; the function returns when closed.
		/// </remarks>
		/// <seealso cref="ADialog.ShowList"/>
		public static int ShowSimple(DStringList items, AnyWnd owner, bool byCaret = false) {
			var a = items.ToArray();
			var m = new ClassicMenu_();
			int autoId = 0;
			foreach (var v in a) {
				var s = v;
				if (s.NE()) {
					m.Separator();
				} else {
					if (s.ToInt(out int id, 0, out int end)) {
						if (s.Eq(end, ' ')) end++;
						s = s[end..];
						autoId = id;
					} else {
						id = ++autoId;
					}
					m.Add(id, s);
				}
			}
			return m.Show(owner, byCaret);
		}
	}
}
