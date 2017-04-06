using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Aga.Controls.Properties;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Aga.Controls.Tree.NodeControls
{
	internal class NodePlusMinus :NodeControl
	{
		public static int ImageSize { get { return Catkeys.Util.Dpi.ScaleInt(9); } }
		public static int Width { get { return Catkeys.Util.Dpi.ScaleInt(16); } }
		private Bitmap _plus;
		private Bitmap _minus;

		private VisualStyleRenderer _openedRenderer;
		private VisualStyleRenderer OpenedRenderer
		{
			get
			{
				if(_openedRenderer == null)
					_openedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
				return _openedRenderer;

			}
		}

		private VisualStyleRenderer _closedRenderer;
		private VisualStyleRenderer ClosedRenderer
		{
			get
			{
				if(_closedRenderer == null)
					_closedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
				return _closedRenderer;
			}
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return new Size(Width, Width);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if(node.CanExpand) {
				Rectangle r = context.Bounds;
				int dy = (int)Math.Round((float)(r.Height - ImageSize) / 2);
				int wh = ImageSize;
				r = new Rectangle(r.X, r.Y + dy, wh, wh);
				if(Application.RenderWithVisualStyles) {
					VisualStyleRenderer renderer;
					if(node.IsExpanded)
						renderer = OpenedRenderer;
					else
						renderer = ClosedRenderer;
					renderer.DrawBackground(context.Graphics, r);
				} else {
					Image img;
					if(node.IsExpanded)
						img = _minus ?? (_minus = Resources.minus);
					else
						img = _plus ?? (_plus = Resources.plus);
					//context.Graphics.DrawImageUnscaled(img, new Point(r.X, r.Y));
					context.Graphics.DrawImage(img, r);
				}
			}
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if(args.Button == MouseButtons.Left) {
				args.Handled = true;
				if(args.Node.CanExpand)
					args.Node.IsExpanded = !args.Node.IsExpanded;
			}
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			args.Handled = true; // Supress expand/collapse when double click on plus/minus
		}
	}
}
