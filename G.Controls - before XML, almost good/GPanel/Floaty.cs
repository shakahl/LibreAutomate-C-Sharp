//This is taken from Code Project and modified.
//https://www.codeproject.com/articles/17729/add-docking-and-floating-support-easely-and-quickl

// Copyright 2007 Herre Kuijpers - <herre@xs4all.nl>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

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
using System.Drawing.Drawing2D;
using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	public partial class GPanel :Panel
	{
		/// <summary>
		/// Floating parent form of this GPanel.
		/// Manages drag/float/dock operations.
		/// </summary>
		protected class Floaty
		{
			//The original state of the panel. Used to reset a panel to its original state if it was floating.
			internal struct _DockState
			{
				internal Control parent;
				internal DockStyle dockStyle;
				internal Rectangle bounds;
				internal Wnd wInsertAfter;
			}

			GPanels _manager;
			GPanel _panel;
			FloatyForm _form;
			internal _DockState _dockState;
			Rectangle _floatingRect;
			bool _isFloating;
			bool _isMoving;

			internal Floaty(GPanel panel)
			{
				_manager = panel._manager;
				_panel = panel;
			}

			internal bool IsFloating { get { return _isFloating; } }

			internal void Show()
			{
				if(_isFloating && !_form.Visible) _form.Show(_manager.Host);
			}

			internal void Hide()
			{
				if(_isFloating && _form.Visible) _form.Hide();
			}

			class FloatyForm :Form
			{
				Floaty _floaty;

				internal FloatyForm(Floaty floaty)
				{
					_floaty = floaty;

					this.SuspendLayout();
					this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
					this.MaximizeBox = false;
					this.MinimizeBox = false;
					this.ShowIcon = false;
					this.ShowInTaskbar = false;
					this.StartPosition = FormStartPosition.Manual;
					this.ControlBox = false;
					this.Text = "";
					this.ResumeLayout(false);
				}

				protected override void OnResizeEnd(EventArgs e)
				{
					_floaty._OnFormResizeEnd();
					base.OnResizeEnd(e);
				}

				protected override void OnMove(EventArgs e)
				{
					if(IsDisposed) return;
					_floaty._OnMove();
					base.OnMove(e);
				}

				protected override void OnClosing(CancelEventArgs e)
				{
					e.Cancel = true;
					this.Hide(); // hide but don't close
					base.OnClosing(e);
				}
			}

			void _OnFormResizeEnd()
			{
				if(_isFloating && _isMoving) {
					if(_manager.Overlay.Visible == true) {
						_manager.Overlay.Hide();
						if(_manager.Overlay.DockHostControl != null) { //ok found new docking position
							_dockState.bounds = _panel.RectangleToClient(_manager.Overlay.Bounds);
							_dockState.dockStyle = _manager.Overlay.Dock;
							var parent = _manager.Overlay.DockHostControl;
							var target = parent as GPanel;
							if(target != null && target != _manager.Host) {
								MultiGPanel.DroppedPanelOnPanel(target, _panel);
							} else {
								_dockState.parent = parent;
								MakeDocked(true);
							}
						}
					}
					_manager.Overlay.DockHostControl = null;
				}
			}

			void _OnMove()
			{
				if(_isFloating && _isMoving) {
					if(ModifierKeys.HasFlag(Keys.Alt)) {
					//if(true) {
						Point pt = Cursor.Position;
						Control t = _FindDockHost(pt);
						if(t == null) {
							_manager.Overlay.Hide();
						} else {
							//Out(t);
							_SetOverlay(t, pt);
						}
					} else {
						_manager.Overlay.Hide();
					}
				}
			}

			// finds the potential dockhost control at the specified location
			Control _FindDockHost(Point pt)
			{
				//is GPanel?
				foreach(var p in _manager.Panels) { //info: Panels does not contain MultiGPanel
					if(p != _panel && p.Visible && !p.IsFloating && _FindDockHost_FormIsHit(p, pt)) return p;
				}
				//is the host?
				if(_FindDockHost_FormIsHit(_manager.Host, pt)) return _manager.Host;
				return null;
			}

			// finds the potential dockhost control at the specified location
			bool _FindDockHost_FormIsHit(Control c, Point pt)
			{
				return c.ClientRectangle.Contains(c.PointToClient(pt));
			}

			/// <summary>
			/// makes the docked control floatable in this Floaty form
			/// </summary>
			internal void MakeFloating(bool startMove)
			{
				if(!_isFloating) {
					RECT r;
					if(startMove || _floatingRect.IsEmpty) { //started to drag, or never was floating
						Wnd w = ((Wnd)_panel);
						r = w.Rect;
						Wnd.Misc.WindowRectFromClientRect(ref r, Api.WS_POPUP | Api.WS_THICKFRAME, Api.WS_EX_TOOLWINDOW);
					} else {
						r = _floatingRect;
						//TODO: ensure in screen
					}

					if(_form == null) {
						_form = new FloatyForm(this);
						_form.Font = _panel.Font; //GPanel.Font, which is used to draw caption, gets font of parent form, ie this
					}

					_dockState.parent = _panel.Parent;

					//this code works well with MultiGPanel child panels, which must be DockStyle.Fill when docked alone (siblings made floating); then we must remeber the original dock style.
					var dock = _panel.Dock;
					if(dock != DockStyle.Fill) {
						_dockState.dockStyle = dock;
						_dockState.bounds = _panel.Bounds;
					}

					if(_panel.DockOutside) _dockState.wInsertAfter = Wnd.Get.NextSibling((Wnd)_panel);
					else _dockState.wInsertAfter = Wnd.Get.PreviousSibling(_panel._splitter != null ? (Wnd)_panel._splitter : (Wnd)_panel);

					_panel.Visible = false;
					_panel.Parent = _form;
					_panel.Dock = DockStyle.Fill;
					_form.Bounds = r;
					_isFloating = true;
					_form.Show(_manager.Host);
					_panel.Visible = true; //not before Show, or will be many OnVisibleChanged

					//redraw, else 0.5 s delay
					//Application.DoEvents(); //redraws, but then cursor is in wrong place
					_form.Invalidate(); _form.Update();

					_panel.MadeFloating?.Invoke(this, new EventArgs());
				} else {
					_floatingRect = _form.Bounds;
				}

				if(startMove) {
					_isMoving = true;
					((Wnd)_form).Send(Api.WM_SYSCOMMAND, Api.SC_MOVE | 0x02, 0); //info: modal loop until window dropped
					_isMoving = false;
				}
			}

			/// <summary>
			/// this will dock the floaty control
			/// </summary>
			internal void MakeDocked(bool dropped)
			{
				if(!_isFloating) return;
				_isFloating = false;

				if(!dropped) _floatingRect = _form.Bounds; //to restore later on "Float" command. If dropped, must save when started to drag.

				var parent = _dockState.parent;
				var md = parent as MultiGPanel;

				_manager.Host.TopLevelControl.BringToFront(); //else something sends it to back
				_form.Hide();
				_panel.Visible = false; // hide it temporarely
				_panel.Parent = parent;

				//this code works well with MultiGPanel child panels, which must be DockStyle.Fill when docked alone (siblings made floating); then we must remeber the original dock style.
				var dock = _dockState.dockStyle == DockStyle.None ? DockStyle.Fill : _dockState.dockStyle;
				_panel.Dock = dock;
				if(dock != DockStyle.Fill) _panel.Bounds = _dockState.bounds;

				((Wnd)_panel).ZorderAfter(_FindToInsertAfter(dropped));
				if(md != null) md.OnChildPanelDocked(_panel);
				_panel.Visible = true;
				//OnX will set splitter

				_panel.MadeDocked?.Invoke(this, new EventArgs());
			}

			Wnd _FindToInsertAfter(bool dropped)
			{
				if(!_IsDockStyleLTRB(_panel.Dock)) return Wnd0; //top

				Wnd w = _dockState.wInsertAfter;
				if(!dropped && w.Visible && w.DirectParent == (Wnd)_dockState.parent) {
					if(_panel.DockOutside) w = Wnd.Get.PreviousSibling(w);
					return w;
				}

				if(_panel.DockOutside) return Wnd.Get.LastSibling((Wnd)_panel); //or Wnd.Misc.SpecHwnd.Bottom

				foreach(Control c in _dockState.parent.Controls) {
					if(c.Dock == DockStyle.Fill) return (Wnd)c;
				}
				return Wnd0;
			}

			/// <summary>
			/// determines the client area of the control. The area of docked controls are excluded
			/// </summary>
			/// <param name="c">the control to which to determine the client area</param>
			/// <returns>returns the docking area in screen coordinates</returns>
			Rectangle _GetDockingArea(Control c)
			{
				Rectangle rc = ((Wnd)c).ClientRectInScreen;

				if(!_panel.DockOutside) {
					foreach(Control cs in c.Controls) {
						if(!cs.Visible) continue;
						switch(cs.Dock) {
						case DockStyle.Left:
							rc.X += cs.Width;
							rc.Width -= cs.Width;
							break;
						case DockStyle.Right:
							rc.Width -= cs.Width;
							break;
						case DockStyle.Top:
							rc.Y += cs.Height;
							rc.Height -= cs.Height;
							break;
						case DockStyle.Bottom:
							rc.Height -= cs.Height;
							break;
						default:
							break;
						}
					}
				}

				return rc;
			}

			/// <summary>
			/// This method will check if the overlay needs to be displayed or not
			/// for display it will position the overlay
			/// </summary>
			/// <param name="c"></param>
			/// <param name="p">position of cursor in screen coordinates</param>
			void _SetOverlay(Control c, Point pc)
			{
				Rectangle r = _GetDockingArea(c);
				//Out((RECT)r);
				Rectangle rc = r;

				//determine relative coordinates
				float rx = (pc.X - r.Left) / (float)(r.Width);
				float ry = (pc.Y - r.Top) / (float)(r.Height);

				//Console.WriteLine("Moving over " + c.Name + " " +  rx.ToString() + "," + ry.ToString());

				_manager.Overlay.Dock = DockStyle.None; // keep floating

				// this section determines when the overlay is to be displayed.
				// it depends on the position of the mouse cursor on the client area.
				// the overlay is currently only shown if the mouse is moving over either the Northern, Western, 
				// Southern or Eastern parts of the client area.
				// when the mouse is in the center or in the NE, NW, SE or SW, no overlay preview is displayed, hence
				// allowing the user to dock the container.

				int formWidth = _form.Width, formHeight = _form.Height;

				// dock to left, checks the Western area
				if(rx > 0 && rx < ry && rx < 0.25 && ry < 0.75 && ry > 0.25) {
					r.Width = r.Width / 2;
					if(r.Width > formWidth)
						r.Width = formWidth;

					_manager.Overlay.Dock = DockStyle.Left; // dock to left
				} else

				// dock to the right, checks the Easter area
				if(rx < 1 && rx > ry && rx > 0.75 && ry < 0.75 && ry > 0.25) {
					r.Width = r.Width / 2;
					if(r.Width > formWidth)
						r.Width = formWidth;
					r.X = rc.X + rc.Width - r.Width;
					_manager.Overlay.Dock = DockStyle.Right;
				} else

				// dock to top, checks the Northern area
				if(ry > 0 && ry < rx && ry < 0.25 && rx < 0.75 && rx > 0.25) {
					r.Height = r.Height / 2;
					if(r.Height > formHeight)
						r.Height = formHeight;
					_manager.Overlay.Dock = DockStyle.Top;
				} else

				// dock to the bottom, checks the Southern area
				if(ry < 1 && ry > rx && ry > 0.75 && rx < 0.75 && rx > 0.25) {
					r.Height = r.Height / 2;
					if(r.Height > formHeight)
						r.Height = formHeight;
					r.Y = rc.Y + rc.Height - r.Height;
					_manager.Overlay.Dock = DockStyle.Bottom;
				}

				if(_manager.Overlay.Dock != DockStyle.None)
					_manager.Overlay.Bounds = r;
				else
					_manager.Overlay.Hide();

				if(!_manager.Overlay.Visible && _manager.Overlay.Dock != DockStyle.None) {
					_manager.Overlay.DockHostControl = c;
					_manager.Overlay.Show(_manager.Host);
					_form.BringToFront();
				}
			}
		}
	}

	public sealed partial class GPanels
	{
		/// <summary>
		/// Transparent-blue rectangle (form) that shows where the floating panel would be docked when dragging it.
		/// </summary>
		internal class DockOverlay :Form
		{
			internal DockOverlay()
			{
				//Out("_DockOverlay");
				//var p = new Perf.Inst(true);
				this.SuspendLayout();
				this.BackColor = Color.DodgerBlue;
				this.ControlBox = false;
				this.FormBorderStyle = FormBorderStyle.None;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Opacity = 0.3;
				this.ShowIcon = false;
				this.ShowInTaskbar = false;
				this.StartPosition = FormStartPosition.Manual;
				this.TopMost = true;
				this.ResumeLayout(false);
				//p.NW();
			}

			// override Dockstate.
			internal new DockStyle Dock;
			internal Control DockHostControl;
		}

		/// <summary>
		/// Mostly transparent rectangle (form) with indicators to show where the floating panel can be docked when dragging it.
		/// </summary>
		//class DocIndicator :Form
		//{
		//	GPanels _manager;

		//	internal DocIndicator(GPanels dockManager)
		//	{
		//		_manager = dockManager;

		//		this.SuspendLayout();
		//		this.ControlBox = false;
		//		this.FormBorderStyle = FormBorderStyle.None;
		//		this.MaximizeBox = false;
		//		this.MinimizeBox = false;
		//		this.ShowIcon = false;
		//		this.ShowInTaskbar = false;
		//		this.StartPosition = FormStartPosition.Manual;

		//		this.AllowTransparency = true;
		//		this.TransparencyKey = Color.Yellow;
		//		this.BackColor = Color.Yellow;

		//		this.ResumeLayout(false);
		//	}

		//	protected override bool ShowWithoutActivation { get { return true; } }

		//	protected override void OnPaint(PaintEventArgs e)
		//	{
		//		base.OnPaint(e);

		//		var g=e.Graphics;
		//		var host = _manager.Host;
		//		var a = new Rectangle[4];
		//		var pen = new Pen(Color.ForestGreen, 10);
		//		var brush = new SolidBrush(Color.ForestGreen);

		//		_DrawRectangles(true, host, g, pen, brush, a);
		//		brush.Color= pen.Color = Color.Blue; pen.Width = 2;
		//		foreach(Control c in host.Controls) {
		//			if(!c.Visible) continue;
		//			//if(c is Splitter) continue;
		//			if(!(c is GPanel || c.Dock==DockStyle.Fill)) continue;
		//			_DrawRectangles(false, c, g, pen, brush, a);
		//		}

		//		pen.Dispose();
		//		brush.Dispose();
		//	}

		//	void _DrawRectangles(bool isForm, Control c, Graphics g, Pen pen, Brush brush, Rectangle[] a)
		//	{
		//		var r=isForm? this.ClientRectangle: c.Bounds;
		//		r.Inflate(-1, -1);
		//		g.DrawRectangle(pen, r);
		//		int z= isForm?20:10, half=z/2, x = (r.Left + r.Right) / 2 - half, y = (r.Top + r.Bottom) / 2 - half;
		//		a[0] = new Rectangle(x, r.Top, z, z);
		//		a[1] = new Rectangle(x, r.Bottom-z, z, z);
		//		a[2] = new Rectangle(r.Left, y, z, z);
		//		a[3] = new Rectangle(r.Right-z, y, z, z);
		//		g.FillRectangles(brush, a);
		//	}

		//}

		//void _ShowDockIndicator()
		//{
		//	if(_dockIndicator==null) _dockIndicator = new DocIndicator(this);
		//	var di =_dockIndicator;
		//	di.Bounds = ((Wnd)Host).ClientRectInScreen;

		//	//var c=new Ell
		//	//di


		//	di.Show(Host);
		//}
	}
}
