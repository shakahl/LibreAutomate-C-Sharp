//ToolStripTextBox and ToolStripComboBox that auto-stretches itself horizontally to fill all unused space of container ToolStrip.
//Most code from MSDN.
//tested: it is impossible to achieve this using control properties etc.

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
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
	//Contains static function used by ToolStripSpringTextBox, ToolStripSpringComboBox and possibly with other classes derived from ToolStripControlHost.
	internal static class ToolStripSpringBox
	{
		internal static Size CalcPreferredWidth(ToolStripControlHost c, Size basePreferredSize)
		{
			//basePreferredSize.Height -= 2; //then does not make toolstrip higher, but clips the control

			// Use the default size if the text box is on the overflow menu
			// or is on a vertical ToolStrip.
			if(c.IsOnOverflow || c.Owner.Orientation == Orientation.Vertical) {
				return basePreferredSize;
			}

			// Declare a variable to store the total available width as 
			// it is calculated, starting with the display width of the 
			// owning ToolStrip.
			Int32 width = c.Owner.DisplayRectangle.Width;

			// Subtract the width of the overflow button if it is displayed. 
			if(c.Owner.OverflowButton.Visible) {
				width = width - c.Owner.OverflowButton.Width -
					c.Owner.OverflowButton.Margin.Horizontal;
			}

			// Declare a variable to maintain a count of ToolStripSpringTextBox 
			// items currently displayed in the owning ToolStrip. 
			Int32 springBoxCount = 0;

			foreach(ToolStripItem item in c.Owner.Items) {
				// Ignore items on the overflow menu.
				if(item.IsOnOverflow) continue;

				if(item is ToolStripControlHost) {
					// For ToolStripSpringTextBox items, increment the count and 
					// subtract the margin width from the total available width.
					springBoxCount++;
					width -= item.Margin.Horizontal;
				} else {
					// For all other items, subtract the full width from the total
					// available width.
					width = width - item.Width - item.Margin.Horizontal;
				}
			}

			// If there are multiple ToolStripControlHost, divide the total available width between them. 
			if(springBoxCount > 1) width /= springBoxCount;

			// If the available width is less than the default width, use the
			// default width, forcing one or more items onto the overflow menu.
			if(width < 100) width = 100;

			// Retrieve the preferred size from the base class, but change the width to the calculated width.
			basePreferredSize.Width = width;
			return basePreferredSize;
		}

		static Font _font = new Font("Verdana", 8);

		internal static void SetDefaultProperties(ToolStripControlHost c)
		{
			c.ControlAlign = ContentAlignment.TopLeft; //prevents jumping TextBox up-down when resizing
			c.Margin = new Padding(0, 2, 0, 0);

			//The default heigth is too big. When high DPI, makes toolstrip higher.
			//Easiest is to change font, because the default Segoe UI 9 font has too many space above.
			//Another way - set AutoSize = false for c and c.Control, and then resize c when resizing its toolstrip. Difficult, because we receive the Resize event too late. Tried unsuccessfully.
			c.Font = _font;
		}
	}

	public class ToolStripSpringTextBox :ToolStripTextBox
	{
		public ToolStripSpringTextBox()
		{
			ToolStripSpringBox.SetDefaultProperties(this);
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			return ToolStripSpringBox.CalcPreferredWidth(this, base.GetPreferredSize(constrainingSize));
		}

		public void SetCueBanner(string text)
		{
			var c = this.Control as TextBox;
			if(c.IsHandleCreated) {
				c.SetCueBanner_(text);
			} else if(!Empty(text)) {
				c.HandleCreated += (unu, sed) => c.SetCueBanner_(text);
			}
		}
	}

	public class ToolStripSpringComboBox :ToolStripComboBox
	{
		public ToolStripSpringComboBox()
		{
			ToolStripSpringBox.SetDefaultProperties(this);
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			return ToolStripSpringBox.CalcPreferredWidth(this, base.GetPreferredSize(constrainingSize));
		}

		public void SetCueBanner(string text)
		{
			var c = this.Control as ComboBox;
			if(c.IsHandleCreated) {
				c.SetCueBanner_(text);
			} else if(!Empty(text)) {
				c.HandleCreated += (unu, sed) => c.SetCueBanner_(text);
			}
		}
	}
}
