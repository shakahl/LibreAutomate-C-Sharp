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

namespace Au.Types
{
	/// <summary>
	/// Used with <see cref="AToolbar.MiscFlags"/>.
	/// </summary>
	[Flags]
	public enum TBFlags
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
	/// Used with <see cref="AToolbar.Border"/>.
	/// </summary>
	public enum TBBorder
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
	/// Used with <see cref="AToolbar.Anchor"/>.
	/// </summary>
	public enum TBAnchor
	{
		/// <summary>
		/// Don't move the toolbar together with its owner.
		/// </summary>
		None,

		//top 1, bottom 2, left 4, right 8

		/// <summary>
		/// Anchors are owner's top and left edges. Default.
		/// </summary>
		TopLeft = 1 | 4,

		/// <summary>
		/// Anchors are owner's top and right edges.
		/// </summary>
		TopRight = 1 | 8,

		/// <summary>
		/// Anchors are owner's bottom and left edges.
		/// </summary>
		BottomLeft = 2 | 4,

		/// <summary>
		/// Anchors are owner's bottom and right edges.
		/// </summary>
		BottomRight = 2 | 8,

		/// <summary>
		/// Anchors are owner's top, left and right edges. The toolbar is resized vertically when resizing its owner.
		/// </summary>
		Top = 1 | 4 | 8,

		/// <summary>
		/// Anchors are owner's bottom, left and right edges. The toolbar is resized vertically when resizing its owner.
		/// </summary>
		Bottom = 2 | 4 | 8,

		/// <summary>
		/// Anchors are owner's left, top and bottom edges. The toolbar is resized horizontally when resizing its owner.
		/// </summary>
		Left = 4 | 1 | 2,

		/// <summary>
		/// Anchors are owner's right, top and bottom edges. The toolbar is resized horizontally when resizing its owner.
		/// </summary>
		Right = 8 | 1 | 2,

		/// <summary>
		/// Anchors are all owner's edges. The toolbar is resized when resizing its owner.
		/// </summary>
		All = 15,

		/// <summary>
		/// Use toolbar's opposite left/right edge than specified. In other words, attach toolbar's left edge to owner's right edge or vice versa.
		/// This flag can be used with TopLeft, TopRight, BottomLeft or BottomRight.
		/// </summary>
		OppositeToolbarEdgeX = 32,

		/// <summary>
		/// Use toolbar's opposite top/bottom edge than specified. In other words, attach toolbar's top edge to owner's bottom edge or vice versa.
		/// This flag can be used with TopLeft, TopRight, BottomLeft or BottomRight.
		/// </summary>
		OppositeToolbarEdgeY = 64,
	}

	static partial class TBExt_
	{
		internal static bool HasTop(this TBAnchor a) => 0 != ((int)a & 1);
		internal static bool HasBottom(this TBAnchor a) => 0 != ((int)a & 2);
		internal static bool HasLeft(this TBAnchor a) => 0 != ((int)a & 4);
		internal static bool HasRight(this TBAnchor a) => 0 != ((int)a & 8);
		internal static bool OppositeX(this TBAnchor a) => 0 != ((int)a & 32);
		internal static bool OppositeY(this TBAnchor a) => 0 != ((int)a & 64);
		internal static TBAnchor WithoutFlags(this TBAnchor a) => a & TBAnchor.All;
	}

	/// <summary>
	/// Reasons to hide a toolbar. Used with <see cref="AToolbar.Hide"/>.
	/// </summary>
	[Flags]
	public enum TBHide
	{
		/// <summary>Owner window is hidden, minimized, etc.</summary>
		Owner = 1,

		/// <summary>A full-screen window is active, and there is flag <see cref="TBFlags.HideIfFullScreen"/>.</summary>
		FullScreen = 2,

		//Satellite = 128, //no, _SetVisible and this enum aren't used with satellites

		/// <summary>This and bigger flag values can be used by callers for any purpose. Value 0x10000.</summary>
		User = 0x10000,
	}

	/// <summary>
	/// Used with <see cref="AToolbar.Offsets"/>.
	/// </summary>
	public struct TBOffsets : IEquatable<TBOffsets>
	{
		/// <summary>
		/// Horizontal distance from the owner's left edge to the toolbar's left edge (right if <see cref="TBAnchor.OppositeToolbarEdgeX"/>).
		/// </summary>
		public int Left { get; set; }

		/// <summary>
		/// Vertical distance from the owner's top edge to the toolbar's top edge (bottom if <see cref="TBAnchor.OppositeToolbarEdgeY"/>).
		/// </summary>
		public int Top { get; set; }

		/// <summary>
		/// Horizontal distance from the toolbar's right edge (left if <see cref="TBAnchor.OppositeToolbarEdgeX"/>) to the owner's right edge.
		/// </summary>
		public int Right { get; set; }

		/// <summary>
		/// Vertical distance from the toolbar's bottom edge (top if <see cref="TBAnchor.OppositeToolbarEdgeY"/>) to the owner's bottom edge.
		/// </summary>
		public int Bottom { get; set; }

		/// <summary>
		/// Sets all properties.
		/// </summary>
		public TBOffsets(int left, int top, int right, int bottom)
		{
			Left = left; Top = top; Right = right; Bottom = bottom;
		}

		///
		public bool Equals(TBOffsets other)
			=> other.Left == this.Left && other.Top == this.Top && other.Right == this.Right && other.Bottom == this.Bottom;

		///
		public override string ToString() => $"{Left} {Top} {Right} {Bottom}";
	}

	/// <summary>
	/// Horizontal separator for <b>ToolStrip</b> controls. With or without a text label. See <see cref="AToolbar.Group"/>.
	/// </summary>
	public class TBGroupSeparator : ToolStripSeparator
	{
		/// <summary>
		/// Adds horizontal separator to the toolstrip.
		/// </summary>
		/// <param name="ts">Parent control.</param>
		/// <param name="name">Optional label.</param>
		public TBGroupSeparator(ToolStrip ts, string name = null)
		{
			switch(ts.LayoutStyle) {
			case ToolStripLayoutStyle.HorizontalStackWithOverflow:
			case ToolStripLayoutStyle.Table:
				throw new InvalidOperationException("Cannot use groups with this LayoutStyle");
			}
			if(ts.LayoutSettings is FlowLayoutSettings f1) {
				switch(f1.FlowDirection) {
				case FlowDirection.TopDown:
				case FlowDirection.BottomUp:
					throw new InvalidOperationException("Cannot use groups with this FlowDirection");
				}
			}

			this.AutoSize = false;
			this.Size = new Size(3, name.NE() ? 3 : TextRenderer.MeasureText("A", ts.Font).Height + 3);
			if(name != null) this.AccessibleName = this.Name = name;
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
	/// Used with <see cref="AToolbar.Show(AWnd, ITBOwnerObject)"/>.
	/// </summary>
	/// <remarks>
	/// Allows a toolbar to follow an object in the owner window, for example an accessible object or image. Or to hide in certain conditions.
	/// Define a class that implements this interface. Create a variable of that class and pass it to <see cref="AToolbar.Show(AWnd, ITBOwnerObject)"/>.
	/// The interface functions are called every 250 ms, sometimes more frequently. Not called when the owner window is invisible or cloaked or minimized.
	/// </remarks>
	public interface ITBOwnerObject
	{
		/// <summary>
		/// Returns false to close the toolbar.
		/// </summary>
		/// <remarks>
		/// Not called if the owner window is invisible or cloaked or minimized.
		/// </remarks>
		bool IsAlive { get; }

		/// <summary>
		/// Returns false to hide the toolbar temporarily.
		/// </summary>
		/// <remarks>
		/// Not called if the owner window is invisible or cloaked or minimized.
		/// </remarks>
		bool IsVisible { get; }

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
	/// Renderer used by <see cref="AToolbar"/>. Based on <see cref="ToolStripSystemRenderer"/>. Draws groups etc.
	/// If you want to replace a toolbar's renderer with a custom renderer, use this class as the base class of your renderer class.
	/// </summary>
#if SYSTEM_RENDERER
	public class TBRenderer : ToolStripSystemRenderer
	{
#else
	public class TBRenderer : ToolStripProfessionalRenderer
	{
		///
		public TBRenderer() : base(new _ColorTable())
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
			foreach(var k in ts.Items.OfType<TBGroupSeparator>()) {
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
					var r = new Rectangle(dr.X, y + 1, dr.Width, k.Height - 1);
					var f = TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix;
					TextRenderer.DrawText(g, " " + s + " ", ts.Font, r, customForeColor ? foreColor : SystemColors.GrayText, ts.BackColor, f);
				}
			}
		}

		///
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			if(e.Item is TBGroupSeparator) return;
			base.OnRenderSeparator(e);
		}
	}
}