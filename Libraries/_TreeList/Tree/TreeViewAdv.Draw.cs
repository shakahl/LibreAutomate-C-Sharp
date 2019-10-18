using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Aga.Controls.Tree.NodeControls;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Aga.Controls.Tree
{
	public partial class TreeViewAdv
	{
		public void AutoSizeColumn(TreeColumn column)
		{
			if(!Columns.Contains(column)) return;

			int res = 0;
			foreach(var node in RowMap) {
				int w = 0;
				foreach(NodeControl nc in NodeControls) {
					if(nc.ParentColumn == column)
						w += nc.GetActualSize(node, _measureContext).Width;
				}
				res = Math.Max(res, w);
			}

			if(res > 0) {
				if(column.Index == 0) res += 6;
				column.Width = res + 4;
			}

			//au: does not work well:
			//	Does not add the width of the lines/boxes area.
			//	Does not add the indent of items in expanded folders.
		}

		private void CreateLinePen()
		{
			_linePen = new Pen(_lineColor);
			_linePen.DashStyle = DashStyle.Dot;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawContext context = new DrawContext {
				Graphics = e.Graphics,
				Font = this.Font,
				Enabled = Enabled
			};

			int y = 0;
			var rClip = e.ClipRectangle;

			if(UseColumns) {
				DrawColumnHeaders(e.Graphics);
				y += ColumnHeaderHeight;
				if(Columns.Count == 0 || rClip.Bottom <= y)
					return;
			}

			int firstRowY = _rowLayout.GetRowBounds(FirstVisibleRow).Y;
			y -= firstRowY;

			Pen linePen = null; int lineRight = 0, lineBottom = 0;
			if(GridLineStyle != 0) {
				linePen = SystemPens.InactiveBorder;
				var cb = e.Graphics.ClipBounds; lineRight = (int)cb.Right; lineBottom = (int)cb.Bottom;
			}
			bool focused = Focused;

			e.Graphics.ResetTransform();
			e.Graphics.TranslateTransform(-OffsetX, y);
			for(int row = FirstVisibleRow; row < RowCount; row++) {
				Rectangle rowRect = _rowLayout.GetRowBounds(row);
				if(rowRect.Bottom + y <= rClip.Top) continue;
				if(rowRect.Y + y >= rClip.Bottom) break;
				DrawRow(e, ref context, row, rowRect, linePen, focused, lineRight);
			}

			if((GridLineStyle & GridLineStyle.Vertical) == GridLineStyle.Vertical && UseColumns)
				DrawVerticalGridLines(e.Graphics, firstRowY, linePen, lineBottom);

			if(_dropPosition.Node != null && DragMode && HighlightDropPosition)
				DrawDropMark(e.Graphics);

			e.Graphics.ResetTransform();

			if(DragMode && _dragBitmap != null)
				e.Graphics.DrawImage(_dragBitmap, PointToClient(MousePosition));

			base.OnPaint(e);
		}

		private void DrawRow(PaintEventArgs e, ref DrawContext context, int row, Rectangle rowRect, Pen linePen, bool focused, int lineRight)
		{
			TreeNodeAdv node = RowMap[row];
			context.DrawSelection = DrawSelectionMode.None;
			context.CurrentEditorOwner = CurrentEditorOwner;
			if(DragMode) {
				if((_dropPosition.Node == node) && _dropPosition.Position == NodePosition.Inside && HighlightDropPosition)
					context.DrawSelection = DrawSelectionMode.Active;
			} else {
				if(node.IsSelected && focused)
					context.DrawSelection = DrawSelectionMode.Active;
				else if(node.IsSelected && !focused && !HideSelection)
					context.DrawSelection = DrawSelectionMode.Inactive;
			}
			context.DrawFocus = focused && CurrentNode == node;

			OnRowDraw(e, node, ref context, row, rowRect);

			if(FullRowSelect) {
				context.DrawFocus = false;
				if(context.DrawSelection == DrawSelectionMode.Active || context.DrawSelection == DrawSelectionMode.Inactive) {
					Rectangle focusRect = new Rectangle(OffsetX, rowRect.Y, ClientRectangle.Width, rowRect.Height);
					if(context.DrawSelection == DrawSelectionMode.Active) {
						e.Graphics.FillRectangle(SystemBrushes.Highlight, focusRect);
						context.DrawSelection = DrawSelectionMode.FullRowSelect;
					} else {
						e.Graphics.FillRectangle(SystemBrushes.Control, focusRect);
						context.DrawSelection = DrawSelectionMode.None;
					}
				}
			}

			if((GridLineStyle & GridLineStyle.Horizontal) == GridLineStyle.Horizontal)
				e.Graphics.DrawLine(linePen, 0, rowRect.Bottom - 1, lineRight, rowRect.Bottom - 1);

			if(ShowLines)
				DrawLines(e.Graphics, node, rowRect);

			DrawNode(node, context);
		}

		private void DrawVerticalGridLines(Graphics gr, int y, Pen linePen, int lineBottom)
		{
			int x = 0;
			foreach(TreeColumn c in Columns) {
				if(c.IsVisible) {
					x += c.Width;
					gr.DrawLine(linePen, x - 1, y, x - 1, lineBottom);
				}
			}
		}

		private void DrawColumnHeaders(Graphics gr)
		{
			ReorderColumnState reorder = Input as ReorderColumnState;
			int x = 0;
			TreeColumn.DrawBackground(gr, new Rectangle(0, 0, ClientRectangle.Width + 2, ColumnHeaderHeight - 1), false, false);
			gr.TranslateTransform(-OffsetX, 0);
			foreach(TreeColumn c in Columns) {
				if(c.IsVisible) {
					if(x >= OffsetX && x - OffsetX < this.Bounds.Width)// skip invisible columns
					{
						Rectangle rect = new Rectangle(x, 0, c.Width, ColumnHeaderHeight - 1);
						gr.SetClip(rect);
						bool pressed = ((Input is ClickColumnState || reorder != null) && ((Input as ColumnState).Column == c));
						c.Draw(gr, rect, Font, pressed, _hotColumn == c);
						gr.ResetClip();

						if(reorder != null && reorder.DropColumn == c)
							TreeColumn.DrawDropMark(gr, rect);
					}
					x += c.Width;
				}
			}

			if(reorder != null) {
				if(reorder.DropColumn == null)
					TreeColumn.DrawDropMark(gr, new Rectangle(x, 0, 0, ColumnHeaderHeight));
				gr.DrawImage(reorder.GhostImage, new Point(reorder.Location.X + +reorder.DragOffset, reorder.Location.Y));
			}
		}

		public void DrawNode(TreeNodeAdv node, DrawContext context)
		{
			foreach(NodeControlInfo item in GetNodeControls(node)) {
				if(item.Bounds.Right >= OffsetX && item.Bounds.X - OffsetX < this.Bounds.Width)// skip invisible nodes
				{
					context.Bounds = item.Bounds;
					context.Graphics.SetClip(context.Bounds);
					item.Control.Draw(node, context);
					context.Graphics.ResetClip();
				}
			}
		}

		private void DrawDropMark(Graphics gr)
		{
			if(_dropPosition.Position == NodePosition.Inside)
				return;

			if(_markPen == null) {
				GraphicsPath path = new GraphicsPath();
				path.AddLines(new Point[] { new Point(0, 0), new Point(1, 1), new Point(-1, 1), new Point(0, 0) });
				CustomLineCap cap = new CustomLineCap(null, path);
				cap.WidthScale = 0.5f;

				_markPen = new Pen(_dragDropMarkColor, _dragDropMarkWidth) {
					CustomStartCap = cap,
					CustomEndCap = cap
				};
			}

			Rectangle rect = GetNodeBounds(_dropPosition.Node);
			int right = DisplayRectangle.Right - LeftMargin + OffsetX;
			int y = rect.Y;
			if(_dropPosition.Position == NodePosition.After)
				y = rect.Bottom;
			gr.DrawLine(_markPen, rect.X, y, right, y);
		}

		private void DrawLines(Graphics gr, TreeNodeAdv node, Rectangle rowRect)
		{
			if(UseColumns && Columns.Count > 0)
				gr.SetClip(new Rectangle(0, rowRect.Y, Columns[0].Width, rowRect.Bottom));

			TreeNodeAdv curNode = node;
			while(curNode != _root && curNode != null) {
				int level = curNode.Level;
				int x = (level - 1) * _indent + NodePlusMinus.ImageSize / 2 + LeftMargin;
				int width = NodePlusMinus.Width - NodePlusMinus.ImageSize / 2;
				int y = rowRect.Y;
				int y2 = y + rowRect.Height;

				if(curNode == node) {
					int midy = y + rowRect.Height / 2;
					gr.DrawLine(_linePen, x, midy, x + width, midy);
					if(curNode.NextNode == null)
						y2 = y + rowRect.Height / 2;
				}

				if(node.Row == 0)
					y = rowRect.Height / 2;
				if(curNode.NextNode != null || curNode == node)
					gr.DrawLine(_linePen, x, y, x, y2);

				curNode = curNode.Parent;
			}

			gr.ResetClip();
		}

	}
}
