//Simplified version of popup control from https://www.codeproject.com/Articles/17502/Simple-Popup-Control
#region License LGPL 3
// Copyright © Łukasz Świątkowski 2007–2010.
// http://www.lukesw.net/
//
// This library is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Security;

using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	/// <summary>
	/// Represents a pop-up window.
	/// </summary>
	[ToolboxItem(false)]
	internal partial class LibPopup :ToolStripDropDown
	{
		#region " Fields & Properties "

		/// <summary>
		/// Gets the content of the pop-up.
		/// </summary>
		public Control Content { get; private set; }

		/// <summary>
		/// Determines which animation to use while showing the pop-up window.
		/// </summary>
		public PopupAnimations ShowingAnimation { get; set; }

		/// <summary>
		/// Determines which animation to use while hiding the pop-up window.
		/// </summary>
		public PopupAnimations HidingAnimation { get; set; }

		/// <summary>
		/// Determines the duration of the animation.
		/// </summary>
		public int AnimationDuration { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the content should receive the focus after the pop-up has been opened.
		/// </summary>
		/// <value><c>true</c> if the content should be focused after the pop-up has been opened; otherwise, <c>false</c>.</value>
		/// <remarks>If the FocusOnOpen property is set to <c>false</c>, then pop-up cannot use the fade effect.</remarks>
		public bool FocusOnOpen { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether pressing the alt key should close the pop-up.
		/// </summary>
		/// <value><c>true</c> if pressing the alt key does not close the pop-up; otherwise, <c>false</c>.</value>
		public bool AcceptAlt { get; set; }

		private ToolStripControlHost _host;
		private Control _opener;
		private LibPopup _ownerPopup;
		private LibPopup _childPopup;
		private bool _resizableTop;
		private bool _resizableLeft;

		/// <summary>
		/// Gets or sets a minimum size of the pop-up.
		/// </summary>
		/// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
		public new Size MinimumSize { get; set; }

		/// <summary>
		/// Gets or sets a maximum size of the pop-up.
		/// </summary>
		/// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
		public new Size MaximumSize { get; set; }

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= (int)Native.WS_EX.NOACTIVATE;
				return cp;
			}
		}

		#endregion

		#region " Constructors "

		/// <param name="content">The content of the pop-up.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="content" /> is <code>null</code>.</exception>
		public LibPopup(Control content)
		{
			if(content == null) {
				throw new ArgumentNullException("content");
			}
			Content = content;
			FocusOnOpen = true;
			AcceptAlt = true;
			ShowingAnimation = PopupAnimations.SystemDefault;
			HidingAnimation = PopupAnimations.None;
			AnimationDuration = 100;
			InitializeComponent();
			AutoSize = false;
			DoubleBuffered = true;
			ResizeRedraw = true;
			_host = new ToolStripControlHost(content);
			Padding = Margin = _host.Padding = _host.Margin = Padding.Empty;
			MinimumSize = content.MinimumSize;
			content.MinimumSize = content.Size;
			MaximumSize = content.MaximumSize;
			content.MaximumSize = content.Size;
			Size = content.Size;
			TabStop = content.TabStop = true;
			content.Location = Point.Empty;
			Items.Add(_host);
			//content.Disposed += (sender, e) =>
			//{
			//	content = null;
			//	Dispose(true);
			//};
			content.RegionChanged += (sender, e) => UpdateRegion();
			UpdateRegion();
		}

		#endregion

		#region " Methods "

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.ToolStripItem.VisibleChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if((Visible && ShowingAnimation == PopupAnimations.None) || (!Visible && HidingAnimation == PopupAnimations.None)) {
				return;
			}
			NativeMethods.AnimationFlags flags = Visible ? NativeMethods.AnimationFlags.Roll : NativeMethods.AnimationFlags.Hide;
			PopupAnimations _flags = Visible ? ShowingAnimation : HidingAnimation;
			if(_flags == PopupAnimations.SystemDefault) {
				if(SystemInformation.IsMenuAnimationEnabled) {
					if(SystemInformation.IsMenuFadeEnabled) {
						_flags = PopupAnimations.Blend;
					} else {
						_flags = PopupAnimations.Slide | (Visible ? PopupAnimations.TopToBottom : PopupAnimations.BottomToTop);
					}
				} else {
					_flags = PopupAnimations.None;
				}
			}
			if((_flags & (PopupAnimations.Blend | PopupAnimations.Center | PopupAnimations.Roll | PopupAnimations.Slide)) == PopupAnimations.None) {
				return;
			}
			if(_resizableTop) // popup is “inverted”, so the animation must be
			{
				if((_flags & PopupAnimations.BottomToTop) != PopupAnimations.None) {
					_flags = (_flags & ~PopupAnimations.BottomToTop) | PopupAnimations.TopToBottom;
				} else if((_flags & PopupAnimations.TopToBottom) != PopupAnimations.None) {
					_flags = (_flags & ~PopupAnimations.TopToBottom) | PopupAnimations.BottomToTop;
				}
			}
			if(_resizableLeft) // popup is “inverted”, so the animation must be
			{
				if((_flags & PopupAnimations.RightToLeft) != PopupAnimations.None) {
					_flags = (_flags & ~PopupAnimations.RightToLeft) | PopupAnimations.LeftToRight;
				} else if((_flags & PopupAnimations.LeftToRight) != PopupAnimations.None) {
					_flags = (_flags & ~PopupAnimations.LeftToRight) | PopupAnimations.RightToLeft;
				}
			}
			flags = flags | (NativeMethods.AnimationFlags.Mask & (NativeMethods.AnimationFlags)(int)_flags);
			NativeMethods.SetTopMost(this);
			NativeMethods.AnimateWindow(this, AnimationDuration, flags);
		}

		/// <summary>
		/// Processes a dialog box key.
		/// </summary>
		/// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process.</param>
		/// <returns>
		/// true if the key was processed by the control; otherwise, false.
		/// </returns>
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(AcceptAlt && ((keyData & Keys.Alt) == Keys.Alt)) {
				if((keyData & Keys.F4) != Keys.F4) {
					return false;
				} else {
					Close();
				}
			}
			bool processed = base.ProcessDialogKey(keyData);
			if(!processed && (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift))) {
				bool backward = (keyData & Keys.Shift) == Keys.Shift;
				Content.SelectNextControl(null, !backward, true, true, true);
			}
			return processed;
		}

		/// <summary>
		/// Updates the pop-up region.
		/// </summary>
		protected void UpdateRegion()
		{
			if(Region != null) {
				Region.Dispose();
				Region = null;
			}
			if(Content.Region != null) {
				Region = Content.Region.Clone();
			}
		}

		/// <summary>
		/// Shows the pop-up window below the specified control.
		/// </summary>
		/// <param name="control">The control below which the pop-up will be shown.</param>
		/// <remarks>
		/// When there is no space below the specified control, the pop-up control is shown above it.
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="control"/> is <code>null</code>.</exception>
		public void Show(Control control)
		{
			if(control == null) {
				throw new ArgumentNullException("control");
			}
			Show(control, control.ClientRectangle);
		}

		/// <summary>
		/// Shows the pop-up window below the specified area.
		/// </summary>
		/// <param name="area">The area of desktop below which the pop-up will be shown.</param>
		/// <remarks>
		/// When there is no space below specified area, the pop-up control is shown above it.
		/// </remarks>
		public void Show(Rectangle area)
		{
			_resizableTop = _resizableLeft = false;
			Point location = new Point(area.Left, area.Top + area.Height);
			Rectangle screen = Screen.FromControl(this).WorkingArea;
			if(location.X + Size.Width > (screen.Left + screen.Width)) {
				_resizableLeft = true;
				location.X = (screen.Left + screen.Width) - Size.Width;
			}
			if(location.Y + Size.Height > (screen.Top + screen.Height)) {
				_resizableTop = true;
				location.Y -= Size.Height + area.Height;
			}
			//location = control.PointToClient(location);
			Show(location, ToolStripDropDownDirection.BelowRight);
		}

		/// <summary>
		/// Shows the pop-up window below the specified area of the specified control.
		/// </summary>
		/// <param name="control">The control used to compute screen location of specified area.</param>
		/// <param name="area">The area of control below which the pop-up will be shown.</param>
		/// <remarks>
		/// When there is no space below specified area, the pop-up control is shown above it.
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="control"/> is <code>null</code>.</exception>
		public void Show(Control control, Rectangle area)
		{
			if(control == null) {
				throw new ArgumentNullException("control");
			}
			SetOwnerItem(control);

			_resizableTop = _resizableLeft = false;
			Point location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
			Rectangle screen = Screen.FromControl(control).WorkingArea;
			if(location.X + Size.Width > (screen.Left + screen.Width)) {
				_resizableLeft = true;
				location.X = (screen.Left + screen.Width) - Size.Width;
			}
			if(location.Y + Size.Height > (screen.Top + screen.Height)) {
				_resizableTop = true;
				location.Y -= Size.Height + area.Height;
			}
			location = control.PointToClient(location);
			Show(control, location, ToolStripDropDownDirection.BelowRight);
		}

		private void SetOwnerItem(Control control)
		{
			if(control == null) {
				return;
			}
			if(control is LibPopup) {
				LibPopup popupControl = control as LibPopup;
				_ownerPopup = popupControl;
				_ownerPopup._childPopup = this;
				OwnerItem = popupControl.Items[0];
				return;
			} else if(_opener == null) {
				_opener = control;
			}
			if(control.Parent != null) {
				SetOwnerItem(control.Parent);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if(Content != null) {
				Content.MinimumSize = Size;
				Content.MaximumSize = Size;
				Content.Size = Size;
				Content.Location = Point.Empty;
			}
			base.OnSizeChanged(e);
		}

		protected override void OnOpening(CancelEventArgs e)
		{
			if(Content.IsDisposed || Content.Disposing) {
				e.Cancel = true;
				return;
			}
			UpdateRegion();
			base.OnOpening(e);
		}

		protected override void OnOpened(EventArgs e)
		{
			if(FocusOnOpen) {
				Content.Focus();
			}
			base.OnOpened(e);
		}

		protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
		{
			_opener = null;
			base.OnClosed(e);
		}

		//protected override void WndProc(ref Message m)
		//{
		//	//if((uint)m.Msg == Api.WM_MOUSEACTIVATE) {
		//	//	Print("WM_MOUSEACTIVATE 2");
		//	//	m.Result = (IntPtr)Api.MA_NOACTIVATE; //toolstrip sets focus anyway
		//	//	return;
		//	//}
		//	base.WndProc(ref m);
		//}

		#endregion

		/// <summary>
		/// Types of animation of the pop-up window.
		/// </summary>
		[Flags]
		internal enum PopupAnimations
		{
			/// <summary>
			/// Uses no animation.
			/// </summary>
			None = 0,
			/// <summary>
			/// Animates the window from left to right. This flag can be used with roll or slide animation.
			/// </summary>
			LeftToRight = 0x00001,
			/// <summary>
			/// Animates the window from right to left. This flag can be used with roll or slide animation.
			/// </summary>
			RightToLeft = 0x00002,
			/// <summary>
			/// Animates the window from top to bottom. This flag can be used with roll or slide animation.
			/// </summary>
			TopToBottom = 0x00004,
			/// <summary>
			/// Animates the window from bottom to top. This flag can be used with roll or slide animation.
			/// </summary>
			BottomToTop = 0x00008,
			/// <summary>
			/// Makes the window appear to collapse inward if it is hiding or expand outward if the window is showing.
			/// </summary>
			Center = 0x00010,
			/// <summary>
			/// Uses a slide animation.
			/// </summary>
			Slide = 0x40000,
			/// <summary>
			/// Uses a fade effect.
			/// </summary>
			Blend = 0x80000,
			/// <summary>
			/// Uses a roll animation.
			/// </summary>
			Roll = 0x100000,
			/// <summary>
			/// Uses a default animation.
			/// </summary>
			SystemDefault = 0x200000,
		}

		static class NativeMethods
		{
			private static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));

			[Flags]
			internal enum AnimationFlags :int
			{
				Roll = 0x0000, // Uses a roll animation.
				HorizontalPositive = 0x00001, // Animates the window from left to right. This flag can be used with roll or slide animation.
				HorizontalNegative = 0x00002, // Animates the window from right to left. This flag can be used with roll or slide animation.
				VerticalPositive = 0x00004, // Animates the window from top to bottom. This flag can be used with roll or slide animation.
				VerticalNegative = 0x00008, // Animates the window from bottom to top. This flag can be used with roll or slide animation.
				Center = 0x00010, // Makes the window appear to collapse inward if Hide is used or expand outward if the Hide is not used.
				Hide = 0x10000, // Hides the window. By default, the window is shown.
				Activate = 0x20000, // Activates the window.
				Slide = 0x40000, // Uses a slide animation. By default, roll animation is used.
				Blend = 0x80000, // Uses a fade effect. This flag can be used only with a top-level window.
				Mask = 0xfffff,
			}

			[SuppressUnmanagedCodeSecurity]
			[DllImport("user32.dll")]
			private static extern int AnimateWindow(HandleRef windowHandle, int time, AnimationFlags flags);

			internal static void AnimateWindow(Control control, int time, AnimationFlags flags)
			{
				AnimateWindow(new HandleRef(control, control.Handle), time, flags);
			}

			[SuppressUnmanagedCodeSecurity]
			[DllImport("user32.dll")]
			private static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);

			internal static void SetTopMost(Control control)
			{
				SetWindowPos(new HandleRef(control, control.Handle), HWND_TOPMOST, 0, 0, 0, 0, 0x13);
			}
		}
	}
}
