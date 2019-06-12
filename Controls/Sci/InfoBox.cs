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
			InitReadOnlyAlways = true;
			InitTagsStyle = TagsStyle.AutoAlways;
			InitImagesStyle = ImagesStyle.ImageTag;
			InitUseDefaultContextMenu = true;
			InitWrapVisuals = false;
			WrapLines = true;
			TabStop = false;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e); //note: must be first

			ST.StyleBackColor(Sci.STYLE_DEFAULT, 0xf8fff0);
			if(InitUseControlFont) ST.StyleFont(Sci.STYLE_DEFAULT, Font); //Segoe UI 9 is narrower but taller than the default Verdana 8
			ST.StyleClearAll();
			ST.MarginWidth(1, 0);
		}

		#region default property values for VS form designer

		[DefaultValue(true)]
		public override bool InitReadOnlyAlways { get => base.InitReadOnlyAlways; set => base.InitReadOnlyAlways = value; }

		[DefaultValue(TagsStyle.AutoAlways)]
		public override TagsStyle InitTagsStyle { get => base.InitTagsStyle; set => base.InitTagsStyle = value; }

		[DefaultValue(ImagesStyle.ImageTag)]
		public override ImagesStyle InitImagesStyle { get => base.InitImagesStyle; set => base.InitImagesStyle = value; }

		[DefaultValue(true)]
		public override bool InitUseDefaultContextMenu { get => base.InitUseDefaultContextMenu; set => base.InitUseDefaultContextMenu = value; }

		[DefaultValue(false)]
		public override bool InitWrapVisuals { get => base.InitWrapVisuals; set => base.InitWrapVisuals = value; }

		[DefaultValue(true)]
		public override bool WrapLines { get => base.WrapLines; set => base.WrapLines = value; }

		[DefaultValue(false)]
		public new bool TabStop { get => base.TabStop; set => base.TabStop = value; }

		#endregion

		/// <summary>
		/// Use control's <b>Font</b> instead of the default font Verdana 8.
		/// </summary>
		public bool InitUseControlFont { get; set; }

		protected override bool IsInputKey(Keys keyData)
		{
			switch(keyData & Keys.KeyCode) { case Keys.Tab: case Keys.Escape: case Keys.Enter: return false; }
			return base.IsInputKey(keyData);
		}
	}
}
