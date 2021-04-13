using Au.Types;
using Au.Util;
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
using System.Windows.Controls;
using System.Windows;
//using System.Linq;

namespace Au.Controls
{
	/// <summary>
	/// Scintilla-based control to show formatted information text.
	/// To set text use the <see cref="KScintilla.zText"/> property. For formatting and links use tags: <see cref="SciTags"/>.
	/// </summary>
	public class KSciInfoBox : KScintilla
	{
		public KSciInfoBox() {
			ZInitReadOnlyAlways = true;
			ZInitTagsStyle = ZTagsStyle.AutoAlways;
			ZInitImagesStyle = ZImagesStyle.ImageTag;
			ZInitUseDefaultContextMenu = true;
			ZInitWrapVisuals = false;
			ZWrapLines = true;
			//TabStop = false;
			Name = "info";
		}

		protected override void ZOnHandleCreated() {
			base.ZOnHandleCreated();

			zStyleBackColor(Sci.STYLE_DEFAULT, 0xf8fff0);
			if (ZInitUseControlFont != null) zStyleFont(Sci.STYLE_DEFAULT, ZInitUseControlFont); //Segoe UI 9 is narrower but taller than the default Verdana 8. Also tested Calibri 9 (used for HtmlRenderer), but Verdana looks better.
			zStyleClearAll();

			zMarginWidth(1, 0);

			SIZE z = ZInitBlankMargins;
			z = ADpi.Scale(z, this.Hwnd);
			Call(Sci.SCI_SETMARGINLEFT, 0, z.width);
			Call(Sci.SCI_SETMARGINRIGHT, 0, z.height);
		}

		/// <summary>
		/// Use font of <i>value</i> instead of the default font Verdana 8.
		/// </summary>
		public Control ZInitUseControlFont { get; set; }

		/// <summary>
		/// The width of the blank margin on both sides of the text. Logical pixels.
		/// </summary>
		public (int left, int right) ZInitBlankMargins { get; set; } = (1, 1);

		//protected override bool IsInputKey(Keys keyData) {
		//	switch (keyData & Keys.KeyCode) { case Keys.Tab: case Keys.Escape: case Keys.Enter: return false; }
		//	return base.IsInputKey(keyData);
		//}

		/// <summary>
		/// Sets element's tooltip text to show in this control instead of standard tooltip popup.
		/// Uses <b>ToolTip</b> property; don't overwrite it.
		/// </summary>
		public void AddElem(FrameworkElement c, string text) {
			c.ToolTip = text;
			c.ToolTipOpening += (o, e) => {
				e.Handled = true;
				this.zText = (o as FrameworkElement).ToolTip as string;
			};
		}
	}
}
