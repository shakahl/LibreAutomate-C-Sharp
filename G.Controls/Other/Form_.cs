using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
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
	}
}
