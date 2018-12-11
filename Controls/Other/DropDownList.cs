using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

//SHOULDDO: now something activates _popup when mouse entered (in some forms) or eg pressed an arrow key.

namespace Au.Controls
{
	/// <summary>
	/// Drop-down list window, similar to a menu.
	/// </summary>
	public class DropDownList :IDisposable
	{
		PopupControlHost _popup;
		ListView _lv;
		int _itemHeight;

		public void Dispose()
		{
			_popup.Dispose();
		}

		/// <summary>
		/// Gets the popup control.
		/// </summary>
		public PopupControlHost Popup => _popup;

		/// <summary>
		/// Sets list items.
		/// Call this before <b>Show</b>.
		/// </summary>
		public string[] Items { get => _items; set { _items = value; _itemsReplaced = true; } }
		string[] _items;
		bool _itemsReplaced;

		/// <summary>
		/// Sets callback function called when an item selected.
		/// The callback can use the <b>ResultX</b> properties to get selection info.
		/// Then the popup is already hidden, but not disposed.
		/// Call this before <b>Show</b>.
		/// </summary>
		public Action<DropDownList> OnSelected { get; set; }

		/// <summary>
		/// Gets the selected index.
		/// Call this from <b>OnSelected</b> callback function.
		/// </summary>
		public int ResultIndex { get; private set; }

		/// <summary>
		/// Gets the selected string.
		/// Call this from <b>OnSelected</b> callback function.
		/// </summary>
		public string ResultString { get; private set; }

		/// <summary>
		/// True if the selection was made with the keyboard (Enter), false if clicked.
		/// Call this from <b>OnSelected</b> callback function.
		/// </summary>
		public bool ResultWasKey { get; private set; }

		///
		public DropDownList()
		{
			_lv = new _ListView(this) {
				View = View.Details,
				HeaderStyle = ColumnHeaderStyle.None,
				VirtualMode = true,
				Size = new Size(1000, 600),
				MultiSelect = false,
				FullRowSelect = true,
				AutoArrange = false,
				HotTracking = true,
				ShowItemToolTips = true,
			};
			_lv.Columns.Add(null, 0);

			_popup = new PopupControlHost(_lv) {
				FocusOnOpen = false,
				ShowingAnimation = PopupControlHost.PopupAnimations.TopToBottom | PopupControlHost.PopupAnimations.Roll,
				AnimationDuration = 0, //system default, same as standard combobox
			};
		}

		/// <summary>
		/// Shows the popup list.
		/// To add items, set <see cref="Items"/> before. To get results, set <see cref="OnSelected"/> before.
		/// </summary>
		/// <param name="owner">Owner control.</param>
		/// <param name="r">Rectangle in owner control's client area. The popup will be below or above it.</param>
		public void Show(Control owner, Rectangle r)
		{
			if(!_AddItemsAndSetSize(r.Width)) return;
			_popup.Show(owner, r);
		}

		/// <summary>
		/// Shows the popup list.
		/// To add items, set <see cref="Items"/> before. To get results, set <see cref="OnSelected"/> before.
		/// </summary>
		/// <param name="owner">Owner control. The popup will be below or above it.</param>
		public void Show(Control owner)
		{
			if(!_AddItemsAndSetSize(owner.Width)) return;
			_popup.Show(owner);
		}

		/// <summary>
		/// Shows the popup list.
		/// To add items, set <see cref="Items"/> before. To get results, set <see cref="OnSelected"/> before.
		/// </summary>
		/// <param name="r">Rectangle in screen. The popup will be below or above it.</param>
		public void Show(Rectangle r)
		{
			if(!_AddItemsAndSetSize(r.Width)) return;
			_popup.Show(r);
		}

		bool _AddItemsAndSetSize(int width)
		{
			int n = Items?.Length ?? 0;

			if(!_itemsReplaced) return n>0;
			_itemsReplaced = false;

			//Perf.First();
			_lv.VirtualListSize = 0; //deselect
			if(n == 0) return false;
			_lv.VirtualListSize = n;
			//Perf.Next();

			if(_itemHeight == 0) {
				//get item height, to calculate popup height.
				//	The only reliable way is _lv.GetItemRect (LVM_GETITEMRECT).
				//	Problem: it creates _lv handle. Then on Popup.Size toolstrip recreates handle. Dirty and slow.
				//	Workaround: Popup.PerformLayout. Then creates handle once.
				_popup.PerformLayout();
				_itemHeight = _lv.GetItemRect(0).Height;
				//Perf.Next();

				//Problem: listview controls shows focus cues when on toolstrip.
				//	Tried to override ShowFocusCues, bit it is not called.
				//	Tested treeview, it doesn't.
				//	Workaround:
				((Wnd)_lv).Send(Api.WM_UPDATEUISTATE, Math_.MakeUint(1, 1));
			}

			int nVisible = Math.Min(n, 30);
			_popup.Size = new Size(width, _itemHeight * nVisible + 4);
			_lv.Columns[0].Width = _lv.ClientSize.Width;

			//rejected: set initial selected item from property SelectIndex or from ComboTextBox.Text. Problems.
			//var sel = SelectIndex; SelectIndex = 0; //default 0. If -1, selecting with arrow keys does not work well.
			//sel = 2; //arrow keys will start from 0 anyway
			////Popup.FocusOnOpen = true;
			//if(ComboTextBox!=null) {
			//	//sel=find ComboTextBox.Text in Items
			//}
			//if(sel >= 0) _lv.Items[sel].Selected = true;
			_lv.Items[0].Selected = true; //let arrow keys work well

			//Perf.NW();
			return true;
		}
		//public int SelectIndex { get; set; }

		class _ListView :ListView
		{
			DropDownList _p;
			ListViewItem _lvi;

			public _ListView(DropDownList p)
			{
				_p = p;
				_lvi = new ListViewItem();
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				//workaround: tooltips are below topmost parent window
				var tt = (Wnd)((Wnd)this).Send(0x104E); //LVM_GETTOOLTIPS
				tt.ZorderTopmost();

				base.OnHandleCreated(e);
			}

			protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
			{
				_lvi.Text = _p.Items[e.ItemIndex];
				e.Item = _lvi;
				base.OnRetrieveVirtualItem(e);
			}

			protected override void OnMouseClick(MouseEventArgs e)
			{
				base.OnMouseClick(e);
				if(e.Button == MouseButtons.Left) _Selected(false);
			}

			protected override void OnKeyDown(KeyEventArgs e)
			{
				base.OnKeyDown(e);
				if(e.KeyData == Keys.Enter) _Selected(true);
			}

			void _Selected(bool key)
			{
				_p._popup.Visible = false;
				var si = SelectedIndices;
				if(si.Count == 1) {
					int i = si[0];
					_p.ResultWasKey = key;
					_p.ResultIndex = i;
					_p.ResultString = Items[i].Text;
					_p.OnSelected?.Invoke(_p);
				}
			}

			//protected override void WndProc(ref Message m)
			//{
			//	//Wnd.Misc.PrintMsg(m, Api.WM_REFLECT|Api.WM_NOTIFY);
			//	//if(m.Msg < Api.WM_USER) Wnd.Misc.PrintMsg(m);
			//	//if(m.Msg == Api.WM_CREATE || m.Msg == Api.WM_DESTROY) Wnd.Misc.PrintMsg(m);
			//	Wnd.Misc.PrintMsg(m);

			//	//if(m.Msg == Api.WM_MOUSEACTIVATE) {
			//	//	Print("WM_MOUSEACTIVATE 1");
			//	//	m.Result = (IntPtr)Api.MA_NOACTIVATE; //toolstrip sets focus anyway
			//	//	return;
			//	//}

			//	base.WndProc(ref m);
			//}
		}
	}
}
