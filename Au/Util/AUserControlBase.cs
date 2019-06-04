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
using System.Xml.Linq;
using System.Windows.Forms;
using System.Drawing;

using Au.Types;
using static Au.AStatic;

namespace Au.Types
{
	/// <summary>
	/// Can be used as base class for user controls instead of UserControl when you want to use standard Windows font and correct auto-scaling when high DPI.
	/// </summary>
	/// <remarks>
	/// More info: <see cref="AFormBase"/>.
	/// </remarks>
	public class AUserControlBase : UserControl
	{
		Font _font;

		///
		public AUserControlBase()
		{
			_font = Util.AFonts.Regular;
			this.Font = _font; //must be before 'AutoScaleMode = ...'
			this.AutoScaleMode = AutoScaleMode.Font;

			//this.TabStop = false; //no, breaks tabstopping
			//this.SetStyle(ControlStyles.Selectable, false); //the same
		}

		///
		protected override void Dispose(bool disposing)
		{
			if(disposing) {
				//Print("Dispose");
				_font.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
