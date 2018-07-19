using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	/// <summary>
	/// Can be used as base class for forms when you want to use correct dialog font and correct auto-scaling when high DPI.
	/// </summary>
	/// <remarks>
	/// Sets these properties:
	/// Font = SystemFonts.MessageBoxFont; //usually Segoe UI, 9pt //default font of Form is MS Sans Serif, 8.25pt
	/// AutoScaleMode = AutoScaleMode.Font;
	/// 
	/// Form designer uses these properties as default. It also adds 'this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);', its OK.
	/// note:
	///		Never set font in designer if you want to support high DPI (AutoScaleMode = AutoScaleMode.Font).
	///		Because designer places the 'Font=...' line after the 'AutoScaleMode = ...' line, and then .NET does not scale the form.
	/// </remarks>
	public class Form_ :Form
	{
		public Form_()
		{
			this.Font = SystemFonts.MessageBoxFont; //must be before 'AutoScaleMode = ...'
			this.AutoScaleMode = AutoScaleMode.Font;
		}

		/// <summary>
		/// Adds WS_POPUP style. Also prevents activating an unrelated window when closing this active owned nonmodal form.
		/// Set it before creating; later does nothing.
		/// </summary>
		public bool IsPopup { get; set; }

		protected override CreateParams CreateParams
		{
			get
			{
				var p = base.CreateParams;
				if(IsPopup) p.Style |= unchecked((int)Native.WS.POPUP);
				return p;
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			//workaround for: when active owned nonmodal form closed, if previous window was not its owner, is activated that window and not the owner.
			//	This is default behavior for native windows without WS_POPUP. If WS_POPUP, then OS activates the owner.
			//	But adding WS_POPUP for .NET forms is not enough.
			//		When closing, before destroying the form window, .NET sets its owner window =0 (=TaskbarOwner if ShowInTaskbar==false).
			//	We now set Owner = null, and set native owner. .NET does not know about it. Then OS will activate the owner.
			if(IsPopup && !Modal) {
				var fo = Owner;
				if(fo != null) {
					var w = ((Wnd)this);
					if(w.IsActive) {
						Owner = null;
						w.Owner = (Wnd)fo;
					}
				}
			}
		}
	}
}
