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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Au.Types;
using Au.Util;

namespace Au
{
	public partial class AToolbar_old
	{
		class _Settings : ASettings
		{
			public static _Settings Load(string file, bool useDefault) => Load<_Settings>(file, useDefault);

			public TBBorder_old border { get => _border; set => Set(ref _border, value); }
			TBBorder_old _border = TBBorder_old.Width2;

			public int borderColor { get => _borderColor; set => Set(ref _borderColor, value); }
			int _borderColor; //not ColorInt because in JSON it is saved as struct

			public TBLayout_old layout { get => _layout; set => Set(ref _layout, value); }
			TBLayout_old _layout = TBLayout_old.Flow;

			public TBAnchor_old anchor { get => _anchor; set => Set(ref _anchor, value); }
			TBAnchor_old _anchor = TBAnchor_old.TopLeft;

			public TBOffsets_old offsets { get => _location; set => Set(ref _location, value); }
			TBOffsets_old _location; // = new TBOffsets(150, 5, 7, 7);

			public bool sizable { get => _sizable; set => Set(ref _sizable, value); }
			bool _sizable = true;

			public SIZE size { get => _size; set => Set(ref _size, value); }
			SIZE _size = (150, 26);

			public bool autoSize { get => _autoSize; set => Set(ref _autoSize, value); }
			bool _autoSize;

			public int wrapWidth { get => _wrapWidth; set => Set(ref _wrapWidth, value); }
			int _wrapWidth;

			public int screen { get => _screen; set => Set(ref _screen, value); }
			int _screen;

			public TBFlags_old miscFlags { get => _miscFlags; set => Set(ref _miscFlags, value); }
			TBFlags_old _miscFlags;
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Used with <see cref="AToolbar_old.MiscFlags"/>.
	/// </summary>
	[Flags]
	public enum TBFlags_old
	{
		/// <summary>
		/// Don't activate the owner window when the toolbar clicked.
		/// </summary>
		DontActivateOwner = 1,

		/// <summary>
		/// Hide the toolbar if a full-screen window is active.
		/// </summary>
		HideIfFullScreen = 2,
	}

	/// <summary>
	/// Used with <see cref="AToolbar_old.Border"/>.
	/// </summary>
	public enum TBBorder_old
	{
		//note: don't reorder.

		/// <summary>No border.</summary>
		None,

		/// <summary>1 pixel border.</summary>
		Width1,

		/// <summary>1 pixel border + 1 pixel padding.</summary>
		Width2,

		/// <summary>1 pixel border + 2 pixels padding.</summary>
		Width3,

		/// <summary>1 pixel border + 3 pixels padding.</summary>
		Width4,

		/// <summary>3D border.</summary>
		ThreeD,

		/// <summary>Standard window border.</summary>
		Thick,

		/// <summary>Title bar and standard window border.</summary>
		Caption,

		/// <summary>Title bar, [x] button and standard window border.</summary>
		CaptionX,
	}

	/// <summary>
	/// Used with <see cref="AToolbar_old.Anchor"/>.
	/// </summary>
	public enum TBAnchor_old
	{
		/// <summary>
		/// Don't move the toolbar together with its owner.
		/// </summary>
		None,

		//top 1, bottom 2, left 4, right 8

		/// <summary>
		/// Anchors are top and left edges. Default.
		/// </summary>
		TopLeft = 1 | 4,

		/// <summary>
		/// Anchors are top and right edges.
		/// </summary>
		TopRight = 1 | 8,

		/// <summary>
		/// Anchors are bottom and left edges.
		/// </summary>
		BottomLeft = 2 | 4,

		/// <summary>
		/// Anchors are bottom and right edges.
		/// </summary>
		BottomRight = 2 | 8,

		/// <summary>
		/// Anchors are top, left and right edges. The toolbar is resized horizontally when resizing its owner.
		/// </summary>
		TopLR = 1 | 4 | 8,

		/// <summary>
		/// Anchors are bottom, left and right edges. The toolbar is resized horizontally when resizing its owner.
		/// </summary>
		BottomLR = 2 | 4 | 8,

		/// <summary>
		/// Anchors are left, top and bottom edges. The toolbar is resized vertically when resizing its owner.
		/// </summary>
		LeftTB = 4 | 1 | 2,

		/// <summary>
		/// Anchors are right, top and bottom edges. The toolbar is resized vertically when resizing its owner.
		/// </summary>
		RightTB = 8 | 1 | 2,

		/// <summary>
		/// Anchors are all edges. The toolbar is resized when resizing its owner.
		/// </summary>
		All = 15,

		/// <summary>
		/// Use owner's opposite left/right edge than specified. In other words, attach toolbar's left edge to owner's right edge or vice versa.
		/// This flag is for toolbars that normally are outside of the owner rectangle (at the left or right).
		/// This flag cannot be used with <b>TopLR</b>, <b>BottomLR</b>, <b>All</b>.
		/// </summary>
		OppositeEdgeX = 32,

		/// <summary>
		/// Use owner's opposite top/bottom edge than specified. In other words, attach toolbar's top edge to owner's bottom edge or vice versa.
		/// This flag is for toolbars that normally are outside of the owner rectangle (above or below).
		/// This flag cannot be used with <b>LeftTB</b>, <b>RightTB</b>, <b>All</b>.
		/// </summary>
		OppositeEdgeY = 64,
	}

	static partial class TBExt_old_
	{
		internal static bool HasTop(this TBAnchor_old a) => 0 != ((int)a & 1);
		internal static bool HasBottom(this TBAnchor_old a) => 0 != ((int)a & 2);
		internal static bool HasLeft(this TBAnchor_old a) => 0 != ((int)a & 4);
		internal static bool HasRight(this TBAnchor_old a) => 0 != ((int)a & 8);
		internal static bool OppositeX(this TBAnchor_old a) => 0 != ((int)a & 32);
		internal static bool OppositeY(this TBAnchor_old a) => 0 != ((int)a & 64);
		internal static TBAnchor_old WithoutFlags(this TBAnchor_old a) => a & TBAnchor_old.All;
	}

	/// <summary>
	/// Used with <see cref="AToolbar_old.Offsets"/>.
	/// </summary>
	public struct TBOffsets_old : IEquatable<TBOffsets_old>
	{
		/// <summary>
		/// Horizontal distance from the owner's left edge (right if <see cref="TBAnchor_old.OppositeEdgeX"/>) to the toolbar's left edge.
		/// </summary>
		public int Left { get; set; }

		/// <summary>
		/// Vertical distance from the owner's top edge (bottom if <see cref="TBAnchor_old.OppositeEdgeY"/>) to the toolbar's top edge.
		/// </summary>
		public int Top { get; set; }

		/// <summary>
		/// Horizontal distance from the toolbar's right edge to the owner's right edge (left if <see cref="TBAnchor_old.OppositeEdgeX"/>).
		/// </summary>
		public int Right { get; set; }

		/// <summary>
		/// Vertical distance from the toolbar's bottom edge to the owner's bottom edge (top if <see cref="TBAnchor_old.OppositeEdgeY"/>).
		/// </summary>
		public int Bottom { get; set; }

		/// <summary>
		/// Sets all properties.
		/// </summary>
		public TBOffsets_old(int left, int top, int right, int bottom)
		{
			Left = left; Top = top; Right = right; Bottom = bottom;
		}

		///
		public bool Equals(TBOffsets_old other)
			=> other.Left == this.Left && other.Top == this.Top && other.Right == this.Right && other.Bottom == this.Bottom;

		///
		public override string ToString() => $"L={Left} T={Top} R={Right} B={Bottom}";
	}

	/// <summary>
	/// See <see cref="AToolbar_old.Layout"/>.
	/// </summary>
	public enum TBLayout_old
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		Flow = (int)ToolStripLayoutStyle.Flow,

		Horizontal = (int)ToolStripLayoutStyle.HorizontalStackWithOverflow,

		Vertical = (int)ToolStripLayoutStyle.VerticalStackWithOverflow,

		Table = (int)ToolStripLayoutStyle.Table,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}

	/// <summary>
	/// Reasons to hide a toolbar. Used with <see cref="AToolbar_old.Hide"/>.
	/// </summary>
	[Flags]
	public enum TBHide_old
	{
		/// <summary>Owner window is hidden, minimized, etc.</summary>
		Owner = 1,

		/// <summary>A full-screen window is active, and there is flag <see cref="TBFlags_old.HideIfFullScreen"/>.</summary>
		FullScreen = 2,

		//Satellite = 128, //no, _SetVisible and this enum aren't used with satellites

		/// <summary>This and bigger flag values can be used by callers for any purpose. Value 0x10000.</summary>
		User = 0x10000,
	}

	/// <summary>
	/// See <see cref="AToolbar_old.NoMenu"/>.
	/// </summary>
	[Flags]
	public enum TBNoMenu_old
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		Menu = 1,
		Edit = 1 << 1,
		Anchor = 1 << 2,
		Layout = 1 << 3,
		Border = 1 << 4,
		Sizable = 1 << 5,
		AutoSize = 1 << 6,
		More = 1 << 7,
		Toolbars = 1 << 8,
		Close = 1 << 9,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}

	/// <summary>
	/// Flags for <see cref="AToolbar_old"/> constructor.
	/// </summary>
	[Flags]
	public enum TBCtor_old
	{
		/// <summary>
		/// Don't load saved settings. Delete the settings file of the toolbar, if exists.
		/// </summary>
		ResetSettings = 1,

		/// <summary>
		/// Don't load and save settings. No file will be created or opened.
		/// </summary>
		DontSaveSettings = 2,
	}

	//rejected
	//Instead of TBCtor etc could be used this class.
	//Example: AToolbar.Next.ResetSettings = true; var t = new AToolbar("name");
	//public class TBNext
	//{
	//	public bool ResetSettings { get; set; }
	//	public bool DontSaveSettings { get; set; }
	//	public string Name { get; set; }

	//	internal void Clear_()
	//	{
	//		ResetSettings = false;
	//		DontSaveSettings = false;
	//		Name = null;
	//	}
	//}
	//in AToolbar class:
	//public static TBNext Next => t_next ??= new TBNext();
	//[ThreadStatic] static TBNext t_next;
	//And remove all flags/ctorFlags parameters.

	/// <summary>
	/// Used with <see cref="AToolbar.Show(AWnd, ITBOwnerObject_old)"/>.
	/// </summary>
	/// <remarks>
	/// Allows a toolbar to follow an object in the owner window, for example an accessible object or image. Or to hide in certain conditions.
	/// Define a class that implements this interface. Create a variable of that class and pass it to <see cref="AToolbar.Show(AWnd, ITBOwnerObject_old)"/>.
	/// The interface functions are called every 250 ms, sometimes more frequently. Not called when the owner window is invisible or cloaked or minimized.
	/// </remarks>
	public interface ITBOwnerObject_old
	{
		/// <summary>
		/// Returns false to close the toolbar.
		/// </summary>
		/// <remarks>
		/// Not called if the owner window is invisible or cloaked or minimized.
		/// The default implementation returns true.
		/// </remarks>
		bool IsAlive => true;

		/// <summary>
		/// Returns false to hide the toolbar temporarily.
		/// </summary>
		/// <remarks>
		/// Not called if the owner window is invisible or cloaked or minimized.
		/// The default implementation returns true.
		/// </remarks>
		bool IsVisible => true;

		/// <summary>
		/// Gets object rectangle.
		/// Returns false if failed.
		/// </summary>
		/// <param name="r">Rectangle in screen coordinates.</param>
		/// <remarks>
		/// Not called if the owner window is invisible or cloaked or minimized or if <see cref="IsVisible"/> returned false.
		/// </remarks>
		bool GetRect(out RECT r);
	}

	/// <summary>
	/// Horizontal separator for <b>ToolStrip</b> controls. With or without a text label. See <see cref="AToolbar_old.Group"/>.
	/// </summary>
	public class TBGroupSeparator_old : ToolStripSeparator
	{
		/// <summary>
		/// Adds horizontal separator to the toolstrip.
		/// </summary>
		/// <param name="ts">Parent control.</param>
		/// <param name="name">Optional label.</param>
		public TBGroupSeparator_old(ToolStrip ts, string name = null)
		{
			switch(ts.LayoutStyle) {
			case ToolStripLayoutStyle.HorizontalStackWithOverflow:
			case ToolStripLayoutStyle.Table:
				AWarning.Write("This layout cannot be used with groups.");
				ts.LayoutStyle = ToolStripLayoutStyle.Flow;
				break;
			}
			if(ts.LayoutSettings is FlowLayoutSettings f1) {
				switch(f1.FlowDirection) {
				case FlowDirection.TopDown:
				case FlowDirection.BottomUp:
					AWarning.Write("This FlowDirection cannot be used with groups.");
					f1.FlowDirection = FlowDirection.LeftToRight;
					break;
				}
			}

			this.AutoSize = false;
			Size z;
			if(name.NE()) z = new Size(3, 3);
			else {
				z = TextRenderer.MeasureText(name, ts.Font);
				z.Width += 20; z.Height += 3;
				this.AccessibleName = this.Name = name;
			}
			this.Size = z;

			var a = ts.Items;
			a.Add(this);
			if(ts.LayoutSettings is FlowLayoutSettings f2) {
				int prev = a.Count - 2;
				if(prev >= 0) f2.SetFlowBreak(a[prev], true);
				f2.SetFlowBreak(this, true);
				//workaround for .NET bug: draws items as if min height of this item = height of next item (+ margin etc)
				a.Add(new ToolStripLabel { AutoSize = false, Size = default }); //and no Visible = false
			}
			//Could instead override OnParentChanged, but it is called too late, on ResumeLayout, and may be bad for parformance etc.
			//Cannot use OnOwnerChanged, it's too early. It seems there are no better overridables.
		}

		//protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
		//{
		//	base.OnParentChanged(oldParent, newParent);
		//	if(newParent != null && !vertical) {
		//		this.Height = this.Name.NE() ? 3 : TextRenderer.MeasureText("A", newParent.Font).Height + 3;
		//		if(newParent.LayoutSettings is FlowLayoutSettings flow) {
		//			var a = newParent.Items;
		//			int i = a.IndexOf(this);
		//			if(i > 0) flow.SetFlowBreak(a[i - 1], true);
		//			flow.SetFlowBreak(this, true);
		//			//workaround for .NET bug: draws items as if min height of this item = height of next item (+ margin etc)
		//			a.Insert(i + 1, new ToolStripLabel { AutoSize = false, Size = default }); //and no Visible = false
		//		}
		//	}
		//}
	}

	/// <summary>
	/// Renderer used by <see cref="AToolbar_old"/>. Based on <see cref="ToolStripSystemRenderer"/>. Draws groups etc.
	/// If you want to replace a toolbar's renderer with a custom renderer, use this class as the base class of your renderer class.
	/// </summary>
#if SYSTEM_RENDERER
	public class TBRenderer : ToolStripSystemRenderer
	{
#else
	public class TBRenderer_old : ToolStripProfessionalRenderer
	{
		///
		public TBRenderer_old() : base(new _ColorTable())
		{
			RoundedEdges = false;
			//if(!Debugger.IsAttached) Debugger.Launch();

			//var c = new ProfessionalColorTable();
			//AOutput.Write((uint)c.ButtonSelectedGradientBegin.ToArgb(), (uint)c.ButtonSelectedGradientMiddle.ToArgb(), (uint)c.ButtonSelectedGradientEnd.ToArgb());
		}

		//Makes selected buttons less vivid and more like of native toolbars.
		class _ColorTable : ProfessionalColorTable
		{
			public override Color ButtonSelectedBorder => Color.FromArgb(unchecked((int)0xFF90C0F0));
			static readonly Color s_c1 = Color.FromArgb(unchecked((int)0xFFD8E6F2)), s_c2 = Color.FromArgb(unchecked((int)0xFFC0DCF3));
			public override Color ButtonSelectedGradientBegin => s_c1;
			public override Color ButtonSelectedGradientEnd => s_c1;
			public override Color ButtonPressedGradientBegin => s_c2;
			public override Color ButtonPressedGradientEnd => s_c2;
		}
#endif
		///
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			//base.OnRenderToolStripBorder(e);
			//System renderer draws 2 white lines at the bottom.
			//Prof renderer draws 1 gray line at the bottom if no native border.
		}

		///
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			//base.OnRenderToolStripBackground(e); //don't need to render background. Let it use BackColor.

			//draw group separators
			//Cannot draw in OnRenderSeparator. The graphics bitmap is too small there. Or need to set item width, which complicates various calculations etc.
			var ts = e.ToolStrip;
			if(ts.LayoutStyle == ToolStripLayoutStyle.HorizontalStackWithOverflow) return;
			var dr = ts.DisplayRectangle;
			if(dr.Width <= 2) return;
			dr.X += 2;
			var g = e.Graphics;
			var crf = g.ClipBounds; //RectangleF
			var clipRect = new Rectangle((int)crf.X, (int)crf.Y, (int)crf.Width, (int)crf.Height);
			foreach(var k in ts.Items.OfType<TBGroupSeparator_old>()) {
				if(!k.Bounds.IntersectsWith(clipRect)) continue;
				//draw line
				Color foreColor = k.ForeColor, backColor = k.BackColor;
				bool customForeColor = foreColor != SystemColors.ControlDark, customBackColor = backColor != ts.BackColor;
				int y = k.Bounds.Y, y2 = k.Height; y2 = y2 > 3 ? y2 / 2 : 1; y2 += y;
				var pen = customForeColor | customBackColor ? new Pen(customBackColor ? backColor : foreColor) : null;
				g.DrawLine(pen ?? SystemPens.ControlDark, dr.X, y2, dr.Right - 5, y2);
				pen?.Dispose();
				//draw text
				var s = k.Name;
				if(!s.NE()) {
					var r = new Rectangle(dr.X - 1, y + 1, dr.Width, k.Height - 1);
					var f = TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix;
					TextRenderer.DrawText(g, " " + s + " ", ts.Font, r, customForeColor ? foreColor : SystemColors.GrayText, ts.BackColor, f);
				}
			}
		}

		///
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			if(e.Item is TBGroupSeparator_old) return;
			base.OnRenderSeparator(e);
		}
	}
}