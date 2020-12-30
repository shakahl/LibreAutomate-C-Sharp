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
//using System.Linq;

namespace Au.Controls
{
	/// <summary>
	/// Scintilla-based control to show formatted information text.
	/// To set text use the <see cref="SciHost.Text"/> property. For formatting and links use tags: <see cref="SciTags"/>.
	/// </summary>
	public class InfoBox : SciHost
	{
		public InfoBox() {
			ZInitReadOnlyAlways = true;
			ZInitTagsStyle = ZTagsStyle.AutoAlways;
			ZInitImagesStyle = ZImagesStyle.ImageTag;
			ZInitUseDefaultContextMenu = true;
			ZInitWrapVisuals = false;
			ZWrapLines = true;
			//TabStop = false;
		}

		protected override void OnHandleCreated() {
			base.OnHandleCreated();

			Z.StyleBackColor(Sci.STYLE_DEFAULT, 0xf8fff0);
			if (ZInitUseControlFont) Z.StyleFont(Sci.STYLE_DEFAULT, App.Wmain); //Segoe UI 9 is narrower but taller than the default Verdana 8. Also tested Calibri 9 (used for HtmlRenderer), but Verdana looks better.
			Z.StyleClearAll();
			Z.MarginWidth(1, 0);
		}

		/// <summary>
		/// Use font of App.Wmain instead of the default font Verdana 8.
		/// </summary>
		[DefaultValue(false)]
		public bool ZInitUseControlFont { get; set; }

		//protected override bool IsInputKey(Keys keyData) {
		//	switch (keyData & Keys.KeyCode) { case Keys.Tab: case Keys.Escape: case Keys.Enter: return false; }
		//	return base.IsInputKey(keyData);
		//}
	}
}
