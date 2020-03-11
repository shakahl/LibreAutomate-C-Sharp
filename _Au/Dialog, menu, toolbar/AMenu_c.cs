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
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;

namespace Au
{
	public partial class AMenu
	{
		class _ContextMenuStrip : ContextMenuStrip, _IAuToolStrip
		{
			AMenu _m;
			bool _isMain;
			bool _cancelClosing;
			//these are used only by submenus
			bool _submenu_openedOnce;
			internal Action<AMenu> _submenu_lazyDelegate;

			internal _ContextMenuStrip(AMenu am, bool isMain)
			{
				_m = am;
				_isMain = isMain;
				this.Name = _m._name;
				this.Text = _m._name;
			}

			protected override CreateParams CreateParams {
				get {
					var p = base.CreateParams;
					if(_m != null) p.ExStyle |= (int)_m._ExStyle;
					return p;
				}
			}

			protected override unsafe void WndProc(ref Message m)
			{
				//AWnd.More.PrintMsg(m, new PrintMsgOptions(Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH, Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_ERASEBKGND, Api.WM_CTLCOLOREDIT));
				//if(_cancelClosing) AWnd.More.PrintMsg(m);

				switch(m.Msg) {
				case Api.WM_CLOSE:
					//AOutput.Write("WM_CLOSE", dd.Visible);
					if((int)m.WParam != c_wmCloseWparam && Visible) { Close(); return; }
					break;
				case Api.WM_SHOWWINDOW when m.WParam != default:
					if(_isMain) _Workaround1();
					//workaround for .NET bug: makes a random window of this thread the owner window of the context menu.
					//	Related .NET bug: AutoClose changes the topmost style. If the owner is topmost, it becomes nontopmost.
					//AWnd w = this.Hwnd(), ow = w.OwnerWindow; if(!ow.Is0) w.OwnerWindow = default; //disabled. Currently it does not harm.
					break;
				case Api.WM_RBUTTONUP:
					_ContextMenu();
					return;
				case Api.WM_ACTIVATE when AMath.LoUshort(m.WParam) != 0:
					//Always activate when clicked a child control. Else cannot enter text in Edit control etc.
					if(!this.Hwnd().IsActive) Api.SetForegroundWindow(this.Hwnd());
					break;
				}

				base.WndProc(ref m);

				switch(m.Msg) {
				case Api.WM_CREATE:
					_m._closing_allMenus.Add(this);
					if(_isMain) Util.ACursor.SetArrowCursor_();
					break;
				case Api.WM_DESTROY:
					_m._closing_allMenus.Remove(this);
					//AOutput.Write("WM_DESTROY", _isMain, m.HWnd);
					break;
				}
			}

			protected override void OnOpening(CancelEventArgs e)
			{
				if(_isMain) {
					if(!e.Cancel) _m._OnOpeningMain();
				} else {
					if(!_submenu_openedOnce && !e.Cancel) {
						_submenu_openedOnce = true;

						//call the caller-provided callback function that should add submenu items on demand
						if(_submenu_lazyDelegate != null) {
							Items.Clear(); //remove the placeholder separator
							_m._submenuStack.Push(this);
							_submenu_lazyDelegate(_m);
							_m._submenuStack.Pop();
							_submenu_lazyDelegate = null;
						}

						PerformLayout();
					}

					if(_asyncIcons != null) {
						_m.GetIconsAsync_(this, _asyncIcons);
						_asyncIcons = null;
					}
				}
				base.OnOpening(e);
			}

			List<Util.IconsAsync_.Item> _IAuToolStrip.SubmenuAsyncIcons => _isMain ? null : (_asyncIcons ??= new List<Util.IconsAsync_.Item>());
			List<Util.IconsAsync_.Item> _asyncIcons;

			protected override void OnOpened(EventArgs e)
			{
				_m._OnOpened(this, _isMain);
				base.OnOpened(e);
			}

			protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
			{
				_m._OnClosed(this, _isMain);
				base.OnClosed(e);
			}

			protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
			{
				//if(e.CloseReason== ToolStripDropDownCloseReason.AppClicked) {
				//	Debugger.Launch();
				//}
				//AOutput.Write(e.Cancel, e.CloseReason, _isMain, AWnd.Active);
				if(_m._closing) e.Cancel = false;
				else if(_cancelClosing) e.Cancel = true;
				else if(!e.Cancel) {
					switch(e.CloseReason) {
					case ToolStripDropDownCloseReason.AppFocusChange:
					case ToolStripDropDownCloseReason.Keyboard:
						if(!_isMain && this.Hwnd().IsActive) Api.SetForegroundWindow((AWnd)this.OwnerItem.Owner.Handle); //prevent closing the parent menu
						break;
					}
				}
				base.OnClosing(e);
			}

			protected override void Dispose(bool disposing)
			{
				if(_isMain) {
					if(disposing && Visible) Close(); //else OnClosed not called etc
				}
				base.Dispose(disposing);
			}

			//~_ContextMenuStrip() => AOutput.Write("dtor");

			protected override void OnPaint(PaintEventArgs e)
			{
				base.OnPaint(e);
				_paintedOnce = true;
			}

			bool _paintedOnce;
			bool _IAuToolStrip.PaintedOnce => _paintedOnce;

			protected override void OnBackColorChanged(EventArgs e)
			{
				_changedBackColor = true;
				base.OnBackColorChanged(e);
			}

			internal bool _changedBackColor, _changedForeColor;

			protected override void OnForeColorChanged(EventArgs e)
			{
				_changedForeColor = true;
				base.OnBackColorChanged(e);
			}

			void _ContextMenu()
			{
				if(_m._name == null) return;
				if(!CanGoToEdit_) return;
				var p = this.MouseClientXY();
				var contextItem = this.GetItemAt(p);
				if(contextItem == null) { //maybe p is at the left or rigt of a textbox, label etc
					foreach(ToolStripItem v in this.Items) {
						var k = v.Bounds;
						if(p.y >= k.Y && p.y < k.Bottom) { contextItem = v; break; }
					}
				}
#if true
				using var m = new Util.ClassicPopupMenu_();
				m.Add(100, "Edit");
				_cancelClosing = true; //in some cases .NET installs a message hook to catch clicks
				int r = m.Show(this.Hwnd());
				_cancelClosing = false;
				switch(r) {
				case 100:
					_m.Close();
					_m.GoToEdit_(contextItem);
					break;
				}
#else //.NET does not like it. Tries to close the main menu, and if we cancel it, menu partially stops working.
				var m = new AMenu();
				m["Edit"] = o => _m.GoToEdit_(contextItem);

				m.Control.RenderMode = ToolStripRenderMode.System;
				m.Show(this);
#endif
			}

			//Workaround for .NET bug: When showing context menu in inactive thread, immediately closes it. Not always, but eg after showing in active.
			//	In some cases vice versa: when showing in active thread after showing in inactive.
			//	It happens when the .NET message loop receives any posted message and it is handled by ToolStripManager.ModalMenuFilter nested class.
			//	If then ModalMenuFilter._lastActiveWindow != GetActiveWindow(), closes menu. But _lastActiveWindow is not updated in inactive thread.
			void _Workaround1()
			{
				try {
					var ty = typeof(ToolStripManager).GetNestedType("ModalMenuFilter", BindingFlags.NonPublic);
					var pi = ty.GetProperty("Instance", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Static);
					var fi = ty.GetField("_lastActiveWindow", BindingFlags.GetField | BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance);
					var mmf = pi.GetValue(null);
					fi.SetValue(mmf, new HandleRef(null, Api.GetActiveWindow().Handle));
				}
				catch(Exception ex) {
					ADebug.Print(ex);
					//A simpler workaround. But then stops working .NET menu autoclosing, submenu on mouse move, menu keyboard navigation (after activating a window of this thread), etc.
					_cancelClosing = true;
					ATimer.After(1, _ => _cancelClosing = false);
				}
			}
		}
	}
}
