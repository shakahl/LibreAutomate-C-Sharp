using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	/// <summary>
	/// Scintilla-based control to show formatted information text.
	/// To set text use the <see cref="Text"/> property. For formatting and links use tags: <see cref="SciTags"/>.
	/// </summary>
	public class InfoBox :AuScintilla
	{
		public InfoBox()
		{
			ZInitReadOnlyAlways = true;
			ZInitTagsStyle = ZTagsStyle.AutoAlways;
			ZInitImagesStyle = ZImagesStyle.ImageTag;
			ZInitUseDefaultContextMenu = true;
			ZInitWrapVisuals = false;
			ZWrapLines = true;
			TabStop = false;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e); //note: must be first

			Z.StyleBackColor(Sci.STYLE_DEFAULT, 0xf8fff0);
			if(ZInitUseControlFont) Z.StyleFont(Sci.STYLE_DEFAULT, Font); //Segoe UI 9 is narrower but taller than the default Verdana 8
			Z.StyleClearAll();
			Z.MarginWidth(1, 0);
		}

		#region default property values for VS form designer

		[DefaultValue(true)]
		public override bool ZInitReadOnlyAlways { get => base.ZInitReadOnlyAlways; set => base.ZInitReadOnlyAlways = value; }

		[DefaultValue(ZTagsStyle.AutoAlways)]
		public override ZTagsStyle ZInitTagsStyle { get => base.ZInitTagsStyle; set => base.ZInitTagsStyle = value; }

		[DefaultValue(ZImagesStyle.ImageTag)]
		public override ZImagesStyle ZInitImagesStyle { get => base.ZInitImagesStyle; set => base.ZInitImagesStyle = value; }

		[DefaultValue(true)]
		public override bool ZInitUseDefaultContextMenu { get => base.ZInitUseDefaultContextMenu; set => base.ZInitUseDefaultContextMenu = value; }

		[DefaultValue(false)]
		public override bool ZInitWrapVisuals { get => base.ZInitWrapVisuals; set => base.ZInitWrapVisuals = value; }

		[DefaultValue(true)]
		public override bool ZWrapLines { get => base.ZWrapLines; set => base.ZWrapLines = value; }

		[DefaultValue(false)]
		public new bool TabStop { get => base.TabStop; set => base.TabStop = value; }

		#endregion

		/// <summary>
		/// Use control's <b>Font</b> instead of the default font Verdana 8.
		/// </summary>
		[DefaultValue(false)]
		public bool ZInitUseControlFont { get; set; }

		protected override bool IsInputKey(Keys keyData)
		{
			switch(keyData & Keys.KeyCode) { case Keys.Tab: case Keys.Escape: case Keys.Enter: return false; }
			return base.IsInputKey(keyData);
		}
	}
}
