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
using System.Runtime.ExceptionServices;
//using System.Linq;
using System.Windows.Forms;
using System.Drawing;

using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	/// <summary>
	/// Can be used as base class for forms used as dialogs. Adds WS_POPUP style and correct auto-scaling when high DPI.
	/// </summary>
	/// <remarks>
	/// Sets these properties:
	/// ZIsPopup = true;
	/// AutoScaleMode = AutoScaleMode.Font;
	/// 
	/// The Visual Studio form designer uses these properties as default. It also adds 'this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);', it's OK.
	/// <note>Never set font in designer if you want to support high DPI (AutoScaleMode = AutoScaleMode.Font). Because designer places the 'Font=...' line after the 'AutoScaleMode = ...' line, and then .NET does not scale the form at run time.</note>
	/// </remarks>
	public class DialogForm : Form
	{
		///
		public DialogForm()
		{
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ZIsPopup = true;
		}

		/// <summary>
		/// Adds WS_POPUP style. Also prevents activating an unrelated window when closing this active owned nonmodal form.
		/// Set it before creating; later does nothing.
		/// </summary>
		[DefaultValue(true)]
		public bool ZIsPopup { get; set; }

		///
		protected override CreateParams CreateParams {
			get {
				var p = base.CreateParams;
				if(ZIsPopup) {
					if(((WS)p.Style).Has(WS.CHILD)) p.Style &= ~unchecked((int)WS.POPUP); //probably in designer
					else p.Style |= unchecked((int)WS.POPUP);
				}
				return p;
			}
		}

		///
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			//workaround for: when active owned nonmodal form closed, if previous window was not its owner, is activated that window and not the owner.
			//	This is default behavior for native windows without WS_POPUP. If WS_POPUP, then OS activates the owner.
			//	But adding WS_POPUP for .NET forms is not enough.
			//		When closing, before destroying the form window, .NET sets its owner window =0 (=TaskbarOwner if ShowInTaskbar==false).
			//	We now set Owner = null, and set native owner. .NET does not know about it. Then OS will activate the owner.
			if(ZIsPopup && !Modal) {
				var fo = Owner;
				if(fo != null) {
					var w = (AWnd)this;
					if(w.IsActive) {
						Owner = null;
						w.Owner = (AWnd)fo;
					}
				}
			}
		}
	}
}
