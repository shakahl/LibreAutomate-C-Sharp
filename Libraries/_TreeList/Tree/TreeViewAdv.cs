using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Collections;

using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	/// <summary>
	/// Extensible advanced <see cref="TreeView"/> implemented in 100% managed C# code.
	/// Features: Model/View architecture. Multiple column per node. Ability to select
	/// multiple tree nodes. Different types of controls for each node column: 
	/// <see cref="CheckBox"/>, Icon, Label... Drag and Drop highlighting. Load on
	/// demand of nodes. Incremental search of nodes.
	/// </summary>
	public partial class TreeViewAdv : Au.Controls.AuScrollableControl
	{
		int LeftMargin => ShowPlusMinus ? 5 : 0; //au: was private const int LeftMargin = 7;
		internal const int ItemDragSensivity = 4;
		private const int DividerWidth = 9;
		private const int DividerCorrectionGap = -2;

		private Pen _linePen;
		private Pen _markPen;
		private bool _suspendUpdate;
		private bool _needFullUpdate;
		private bool _fireSelectionEvent;
		private NodePlusMinus _plusMinus;
		private ToolTip _toolTip;
		private DrawContext _measureContext;
		private TreeColumn _hotColumn;
		//private IncrementalSearch _search;

		#region Public Events

		[Category("Action")]
		public event ItemDragEventHandler ItemDrag;
		//Au: made protected virtual. Why these functions are private?
		protected virtual void OnItemDrag(MouseButtons buttons, object item)
		{
			if(ItemDrag != null)
				ItemDrag(this, new ItemDragEventArgs(buttons, item));
		}

		[Category("Behavior")]
		public event EventHandler<TreeNodeAdvMouseEventArgs> NodeMouseClick;
		private void OnNodeMouseClick(TreeNodeAdvMouseEventArgs args)
		{
			if(NodeMouseClick != null)
				NodeMouseClick(this, args);
		}

		[Category("Behavior")]
		public event EventHandler<TreeNodeAdvMouseEventArgs> NodeMouseDoubleClick;
		private void OnNodeMouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			if(NodeMouseDoubleClick != null)
				NodeMouseDoubleClick(this, args);
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnWidthChanged;
		internal void OnColumnWidthChanged(TreeColumn column)
		{
			if(ColumnWidthChanged != null)
				ColumnWidthChanged(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnReordered;
		internal void OnColumnReordered(TreeColumn column)
		{
			if(ColumnReordered != null)
				ColumnReordered(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnClicked;
		internal void OnColumnClicked(TreeColumn column)
		{
			if(ColumnClicked != null)
				ColumnClicked(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler<SelectionReason> SelectionChanged;
		internal void OnSelectionChanged(SelectionReason reason)
		{
			if(_suspendSelectionEvent) {
				_fireSelectionEvent = true;
			} else {
				_fireSelectionEvent = false;
				if(SelectionChanged != null) {
					int n = Selection.Count;
					if(n < 1) reason = SelectionReason.NoSelection; else if(n > 1) reason = SelectionReason.Multi;
					SelectionChanged.Invoke(this, reason);
				}
			}
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Collapsing;
		private void OnCollapsing(TreeNodeAdv node)
		{
			if(Collapsing != null)
				Collapsing(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Collapsed;
		private void OnCollapsed(TreeNodeAdv node)
		{
			if(Collapsed != null)
				Collapsed(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Expanding;
		private void OnExpanding(TreeNodeAdv node)
		{
			if(Expanding != null)
				Expanding(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Expanded;
		private void OnExpanded(TreeNodeAdv node)
		{
			if(Expanded != null)
				Expanded(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler GridLineStyleChanged;
		private void OnGridLineStyleChanged()
		{
			if(GridLineStyleChanged != null)
				GridLineStyleChanged(this, EventArgs.Empty);
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewRowDrawEventArgs> RowDraw;
		protected virtual void OnRowDraw(PaintEventArgs e, TreeNodeAdv node, ref DrawContext context, int row, Rectangle rowRect)
		{
			//au: made context ref. Then the override can make more changes. Also it is 48 bytes.
			if(RowDraw != null) {
				TreeViewRowDrawEventArgs args = new TreeViewRowDrawEventArgs(e.Graphics, e.ClipRectangle, node, context, row, rowRect);
				RowDraw(this, args);
			}
		}

		//au: removed because: 1. Duplicates the BaseTextControl.[On]DrawText. 2. OnDrawControl not called if DrawControl==null; it's confusing.
		///// <summary>
		///// Fires when control is going to draw. Can be used to change text or back color
		///// </summary>
		//[Category("Behavior")]
		//public event EventHandler<DrawEventArgs> DrawControl;

		//internal bool DrawControlMustBeFired()
		//{
		//	return DrawControl != null;
		//}

		//internal void FireDrawControl(DrawEventArgs args)
		//{
		//	OnDrawControl(args);
		//}

		//protected virtual void OnDrawControl(DrawEventArgs args)
		//{
		//	if(DrawControl != null)
		//		DrawControl(this, args);
		//}


		[Category("Drag Drop")]
		public event EventHandler<DropNodeValidatingEventArgs> DropNodeValidating;
		protected virtual void OnDropNodeValidating(Point point, ref TreeNodeAdv node)
		{
			if(DropNodeValidating != null) {
				DropNodeValidatingEventArgs args = new DropNodeValidatingEventArgs(point, node);
				DropNodeValidating(this, args);
				node = args.Node;
			}
		}
		#endregion

		public TreeViewAdv()
		{
			this.BackColor = SystemColors.Window;
			SetStyle(ControlStyles.AllPaintingInWmPaint
				| ControlStyles.UserPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.ResizeRedraw
				| ControlStyles.Selectable
				, true);

			_rowLayout = new AutoRowHeightLayout(this, 0);
			_rowMap = new List<TreeNodeAdv>();
			_selection = new List<TreeNodeAdv>();
			_readonlySelection = new ReadOnlyCollection<TreeNodeAdv>(_selection);
			_columns = new TreeColumnCollection(this);
			_toolTip = new ToolTip { ShowAlways = true };

			//_measureContext = new DrawContext();
			//_measureContext.Font = Font; //au: will set in OnFontChanged
			//_measureContext.Graphics = Graphics.FromImage(new Bitmap(1, 1));
			_measureContext.Graphics = Graphics.FromHwndInternal(default); //au

			Input = new NormalInputState(this);
			CreateNodes();
			CreateLinePen();

			_plusMinus = new NodePlusMinus();
			_controls = new NodeControlsCollection(this);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing) {
				_measureContext.Graphics.Dispose();
				_linePen.Dispose();
				_markPen?.Dispose();
				_dragBitmap?.Dispose();
				_dragTimer?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Public Methods

		public TreeNodeAdv GetNodeAt(Point point)
		{
			NodeControlInfo info = GetNodeControlInfoAt(point);
			return info.Node;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="point">Point in client area.</param>
		/// <remarks>
		/// Gets virtual bounds. They don't include scroll offsets and header height. To get real bounds in client area, subtract OffsetX and OffsetY.
		/// </remarks>
		public NodeControlInfo GetNodeControlInfoAt(Point point)
		{
			if(point.X < 0 || point.Y < 0)
				return NodeControlInfo.Empty;

			int row = _rowLayout.GetRowAt(point);
			if(row < RowCount && row >= 0)
				return GetNodeControlInfoAt(RowMap[row], point);
			else
				return NodeControlInfo.Empty;
		}

		private NodeControlInfo GetNodeControlInfoAt(TreeNodeAdv node, Point point)
		{
			Rectangle rect = _rowLayout.GetRowBounds(FirstVisibleRow);
			point.Y += (rect.Y - ColumnHeaderHeight);
			point.X += OffsetX;
			foreach(NodeControlInfo info in GetNodeControls(node))
				if(info.Bounds.Contains(point))
					return info;

			if(FullRowSelect)
				return new NodeControlInfo(null, Rectangle.Empty, node);
			else
				return NodeControlInfo.Empty;
		}

		public void BeginUpdate()
		{
			_suspendUpdate = true;
			SuspendSelectionEvent(SelectionReason.Other);
		}

		public void EndUpdate()
		{
			_suspendUpdate = false;
			if(_needFullUpdate)
				FullUpdate();
			else
				UpdateView();
			ResumeSelectionEvent();
		}

		public void ExpandAll()
		{
			_root.ExpandAll();
		}

		public void CollapseAll()
		{
			_root.CollapseAll();
		}

		/// <summary>
		/// Expand all parent nodes, andd scroll to the specified node
		/// </summary>
		public void EnsureVisible(TreeNodeAdv node)
		{
			if(node == null)
				throw new ArgumentNullException("node");

			if(!IsMyNode(node))
				throw new ArgumentException();

			TreeNodeAdv parent = node.Parent;
			while(parent != _root) {
				parent.IsExpanded = true;
				parent = parent.Parent;
			}
			ScrollTo(node);
		}

		/// <summary>
		/// Make node visible, scroll if needed. All parent nodes of the specified node must be expanded
		/// </summary>
		/// <param name="node"></param>
		public void ScrollTo(TreeNodeAdv node)
		{
			if(node == null)
				throw new ArgumentNullException("node");

			if(!IsMyNode(node))
				throw new ArgumentException();

			if(node.Row < 0)
				CreateRowMap();

			int row = -1;

			if(node.Row < FirstVisibleRow)
				row = node.Row;
			else {
				int pageStart = _rowLayout.GetRowBounds(FirstVisibleRow).Top;
				int rowBottom = _rowLayout.GetRowBounds(node.Row).Bottom;
				if(rowBottom > pageStart + DisplayRectangle.Height - ColumnHeaderHeight)
					row = _rowLayout.GetFirstRow(node.Row);
			}

			if(row >= 0) SetScrollPos(true, row, true);
		}

		public void ClearSelection()
		{
			BeginUpdate();
			try {
				ClearSelectionInternal();
				SelectionStart = null; //not in Internal
			}
			finally {
				EndUpdate();
			}
		}

		internal void ClearSelectionInternal()
		{
			while(Selection.Count > 0) {
				var t = Selection[0];
				t.IsSelected = false;
				Selection.Remove(t); //_hack
			}
		}

		#endregion

		protected override void OnSizeChanged(EventArgs e)
		{
			//if(IsHandleCreated) AOutput.Write("OnSizeChanged");
			UpdateScrollBars();
			base.OnSizeChanged(e);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if(RowCount > 0) UpdateScrollBars();
		}

		public void UpdateScrollBars()
		{
			if(!IsHandleCreated) return;
			//AOutput.Write("UpdateScrollBars", ItemCount);
			for(int i = 2; i >= 0; i--) {
				SetScrollInfo(true, Math.Max(RowCount - 1, 0), _rowLayout.PageRowCount, notify: true);
				SetScrollInfo(false, ContentWidth, Math.Max(DisplayRectangle.Width, 0), notify: true);
			}
		}

		protected override void OnScroll(ScrollInfo si)
		{
			if(si.IsVertical) {
				if(si.Pos != FirstVisibleRow) FirstVisibleRow = si.Pos;
			} else {
				if(si.Pos != OffsetX) OffsetX = si.Pos;
			}

			base.OnScroll(si);
		}

		protected override CreateParams CreateParams {
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				CreateParams res = base.CreateParams;
				switch(BorderStyle) {
				case BorderStyle.FixedSingle:
					res.Style |= 0x800000;
					break;
				case BorderStyle.Fixed3D:
					res.ExStyle |= 0x200;
					break;
				}
				return res;
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			UpdateView();
			ChangeInput();
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			UpdateView();
			base.OnLostFocus(e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			_measureContext.Font = Font;
			//Au.AOutput.Write(Font);
			_columnHeaderHeight = 0;
			FullUpdate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <remarks>
		/// Gets virtual bounds. They don't include scroll offsets and header height. To get real bounds in client area, subtract OffsetX and OffsetY.
		/// </remarks>
		public IEnumerable<NodeControlInfo> GetNodeControls(TreeNodeAdv node)
		{
			if(node == null)
				yield break;
			Rectangle rowRect = _rowLayout.GetRowBounds(node.Row);
			foreach(NodeControlInfo n in GetNodeControls(node, rowRect))
				yield return n;
		}

		internal IEnumerable<NodeControlInfo> GetNodeControls(TreeNodeAdv node, Rectangle rowRect)
		{
			if(node == null)
				yield break;

			int y = rowRect.Y;
			int x = (node.Level - 1) * _indent + LeftMargin;
			int width = 0;
			if(node.Row == 0 && ShiftFirstNode)
				x -= _indent;
			Rectangle rect = Rectangle.Empty;

			if(ShowPlusMinus) {
				width = _plusMinus.GetActualSize(node, _measureContext).Width;
				rect = new Rectangle(x, y, width, rowRect.Height);
				if(UseColumns && Columns.Count > 0 && Columns[0].Width < rect.Right)
					rect.Width = Columns[0].Width - x;

				yield return new NodeControlInfo(_plusMinus, rect, node);
				x += width;
			}

			if(!UseColumns) {
				foreach(NodeControl c in NodeControls) {
					Size s = c.GetActualSize(node, _measureContext);
					if(!s.IsEmpty) {
						width = s.Width;
						rect = new Rectangle(x, y, width, rowRect.Height);
						x += rect.Width;
						yield return new NodeControlInfo(c, rect, node);
					}
				}
			} else {
				int right = 0;
				foreach(TreeColumn col in Columns) {
					if(col.IsVisible && col.Width > 0) {
						right += col.Width;
						for(int i = 0; i < NodeControls.Count; i++) {
							NodeControl nc = NodeControls[i];
							if(nc.ParentColumn == col) {
								Size s = nc.GetActualSize(node, _measureContext);
								if(!s.IsEmpty) {
									bool isLastControl = true;
									for(int k = i + 1; k < NodeControls.Count; k++)
										if(NodeControls[k].ParentColumn == col) {
											isLastControl = false;
											break;
										}

									width = right - x;
									if(!isLastControl)
										width = s.Width;
									int maxWidth = Math.Max(0, right - x);
									rect = new Rectangle(x, y, Math.Min(maxWidth, width), rowRect.Height);
									x += width;
									yield return new NodeControlInfo(nc, rect, node);
								}
							}
						}
						x = right;
					}
				}
			}
		}

		internal static double Dist(Point p1, Point p2)
		{
			return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
		}

		public void FullUpdate()
		{
			HideEditor();
			if(InvokeRequired)
				BeginInvoke(new MethodInvoker(UnsafeFullUpdate));
			else
				UnsafeFullUpdate();
		}

		private void UnsafeFullUpdate()
		{
			_rowLayout.ClearCache();
			CreateRowMap();
			UpdateScrollBars();
			UpdateView();
			_needFullUpdate = false;
		}

		internal void UpdateView()
		{
			if(!_suspendUpdate)
				Invalidate(false);
		}

		/// <summary>
		/// Invalidates node's rectangle.
		/// </summary>
		/// <param name="node"></param>
		public void UpdateNode(TreeNodeAdv node)
		{
			if(!_suspendUpdate)
				Invalidate(GetNodeBoundsInClient(node, true));
		}

		internal void UpdateHeaders()
		{
			Invalidate(new Rectangle(0, 0, Width, ColumnHeaderHeight));
		}

		internal void UpdateColumns()
		{
			FullUpdate();
		}

		private void CreateNodes()
		{
			Selection.Clear();
			SelectionStart = null;
			_root = new TreeNodeAdv(this, null);
			_root.IsExpanded = true;
			_root.TryGetFirstChild(out var node);
			CurrentNode = node;
		}

		internal void ReadChilds(TreeNodeAdv parentNode)
		{
			ReadChilds(parentNode, false);
		}

		internal void ReadChilds(TreeNodeAdv parentNode, bool performFullUpdate)
		{
			if(!parentNode.IsLeaf) {
				parentNode.IsExpandedOnce = true;
				parentNode.Nodes.Clear();

				if(Model != null) {
					IEnumerable items = Model.GetChildren(parentNode.Tag);
					if(items != null)
						foreach(object obj in items) {
							AddNewNode(parentNode, obj, -1);
							if(performFullUpdate)
								FullUpdate();
						}
				}

				if(parentNode.AutoExpandOnStructureChanged)
					parentNode.ExpandAll();
			}
		}

		private void AddNewNode(TreeNodeAdv parent, object tag, int index)
		{
			TreeNodeAdv node = new TreeNodeAdv(this, tag);
			AddNode(parent, index, node);
		}

		private void AddNode(TreeNodeAdv parent, int index, TreeNodeAdv node)
		{
			var pn = parent.GetOrCreateNodes();
			if(index >= 0 && index < pn.Count)
				pn.Insert(index, node);
			else
				pn.Add(node);

			node.IsLeaf = Model.IsLeaf(node.Tag);
			if(node.IsLeaf)
				node.Nodes.Clear();
			if(!LoadOnDemand || node.IsExpandedOnce)
				ReadChilds(node);
		}

		private struct ExpandArgs
		{
			public TreeNodeAdv Node;
			public bool Value;
			public bool IgnoreChildren;
		}

		internal void SetIsExpanded(TreeNodeAdv node, bool value, bool ignoreChildren)
		{
			ExpandArgs eargs = new ExpandArgs();
			eargs.Node = node;
			eargs.Value = value;
			eargs.IgnoreChildren = ignoreChildren;
			SetIsExpanded(eargs);
		}

		private void SetIsExpanded(ExpandArgs eargs)
		{
			bool update = !eargs.IgnoreChildren /*&& !AsyncExpanding*/;
			if(update)
				BeginUpdate();
			try {
				if(IsMyNode(eargs.Node) && eargs.Node.IsExpanded != eargs.Value)
					SetIsExpanded(eargs.Node, eargs.Value);

				if(!eargs.IgnoreChildren)
					SetIsExpandedRecursive(eargs.Node, eargs.Value);
			}
			finally {
				if(update)
					EndUpdate();
			}
		}

		internal void SetIsExpanded(TreeNodeAdv node, bool value)
		{
			if(Root == node && !value)
				return; //Can't collapse root node

			if(value) {
				OnExpanding(node);
				node.OnExpanding();
			} else {
				OnCollapsing(node);
				node.OnCollapsing();
			}

			if(value && !node.IsExpandedOnce) {
				ReadChilds(node, false);
			}
			node.AssignIsExpanded(value);
			SmartFullUpdate();

			if(value) {
				OnExpanded(node);
				node.OnExpanded();
			} else {
				OnCollapsed(node);
				node.OnCollapsed();
			}
		}

		internal void SetIsExpandedRecursive(TreeNodeAdv root, bool value)
		{
			if(!root.HasChildren) return;
			foreach(var node in root.Nodes) {
				node.IsExpanded = value;
				SetIsExpandedRecursive(node, value);
			}
		}

		private void CreateRowMap()
		{
			RowMap.Clear();
			int row = 0;
			_contentWidth = 0;
			foreach(TreeNodeAdv node in VisibleNodes) {
				node.Row = row;
				RowMap.Add(node);
				if(!UseColumns) {
					_contentWidth = Math.Max(_contentWidth, GetNodeWidth(node));
				}
				row++;
			}
			if(UseColumns) {
				_contentWidth = 0;
				foreach(TreeColumn col in _columns)
					if(col.IsVisible)
						_contentWidth += col.Width;
			}
		}

		private int GetNodeWidth(TreeNodeAdv node)
		{
			if(node.RightBounds == null) {
				Rectangle res = GetNodeBounds(GetNodeControls(node, Rectangle.Empty));
				node.RightBounds = res.Right;
			}
			return node.RightBounds.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <remarks>
		/// Gets virtual bounds. They don't include scroll offsets and header height. To get real bounds in client area, subtract OffsetX and OffsetY.
		/// </remarks>
		public Rectangle GetNodeBounds(TreeNodeAdv node)
		{
			return GetNodeBounds(GetNodeControls(node));
		}

		private Rectangle GetNodeBounds(IEnumerable<NodeControlInfo> nodeControls)
		{
			Rectangle res = Rectangle.Empty;
			foreach(NodeControlInfo info in nodeControls) {
				if(res == Rectangle.Empty)
					res = info.Bounds;
				else
					res = Rectangle.Union(res, info.Bounds);
			}
			return res;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="fullRow">
		/// If true, X will be always 0 and Width will be client area width.
		/// If false, it will be node bounds; it is not full row if !UseColumns; it can be negative if scrolled horizontally.
		/// </param>
		public Rectangle GetNodeBoundsInClient(TreeNodeAdv node, bool fullRow = false)
		{
			if(fullRow) {
				var r = _rowLayout.GetRowBounds(node.Row);
				r.Y -= OffsetY;
				r.X = 0; r.Width = ClientSize.Width;
				return r;
			}
			return RectVirtualToClient(GetNodeBounds(node));
		}

		internal void SmartFullUpdate()
		{
			if(_suspendUpdate)
				_needFullUpdate = true;
			else
				FullUpdate();
		}

		internal bool IsMyNode(TreeNodeAdv node)
		{
			if(node == null)
				return false;

			if(node.Tree != this)
				return false;

			while(node.Parent != null)
				node = node.Parent;

			return node == _root;
		}

		internal void UpdateSelection()
		{
			bool flag = false;

			if(!IsMyNode(CurrentNode))
				CurrentNode = null;
			if(!IsMyNode(_selectionStart))
				_selectionStart = null;

			for(int i = Selection.Count - 1; i >= 0; i--)
				if(!IsMyNode(Selection[i])) {
					flag = true;
					Selection.RemoveAt(i);
				}

			if(flag)
				OnSelectionChanged(SelectionReason.Other);
		}

		internal void ChangeColumnWidth(TreeColumn column)
		{
			if(!(_input is ResizeColumnState)) {
				FullUpdate();
				OnColumnWidthChanged(column);
			}
		}

		public TreeNodeAdv FindNode(TreePath path)
		{
			return FindNode(path, false);
		}

		public TreeNodeAdv FindNode(TreePath path, bool readChilds)
		{
			if(path.IsEmpty())
				return _root;
			else
				return FindNode(_root, path, 0, readChilds);
		}

		private TreeNodeAdv FindNode(TreeNodeAdv root, TreePath path, int level, bool readChilds)
		{
			if(!root.IsExpandedOnce && readChilds)
				ReadChilds(root);

			foreach(var node in root.Nodes) {
				if(node.Tag == path.FullPath[level]) {
					if(level == path.FullPath.Length - 1)
						return node;
					else
						return FindNode(node, path, level + 1, readChilds);
				}
			}
			return null;
			//TODO: can be optimized to not enumerate childless nodes?
		}

		public TreeNodeAdv FindNodeByTag(object tag)
		{
			return FindNodeByTag(_root, tag);
		}

		private TreeNodeAdv FindNodeByTag(TreeNodeAdv root, object tag)
		{
			foreach(var node in root.Nodes) {
				if(node.Tag == tag)
					return node;
				TreeNodeAdv res = FindNodeByTag(node, tag);
				if(res != null)
					return res;
			}
			return null;
			//TODO: can be optimized to not enumerate childless nodes?
		}

		public void SelectAllNodes()
		{
			SuspendSelectionEvent(SelectionReason.Multi);
			try {
				if(SelectionMode == TreeSelectionMode.MultiSameParent) {
					if(CurrentNode != null) {
						foreach(var n in CurrentNode.Parent.Nodes)
							n.IsSelected = true;
					}
				} else if(SelectionMode == TreeSelectionMode.Multi) {
					SelectNodes(Root.Nodes);
				}
			}
			finally {
				ResumeSelectionEvent();
			}
		}

		private void SelectNodes(Collection<TreeNodeAdv> nodes)
		{
			foreach(TreeNodeAdv n in nodes) {
				n.IsSelected = true;
				if(n.IsExpanded)
					SelectNodes(n.Nodes);
			}
		}

		#region ModelEvents
		private void BindModelEvents()
		{
			_model.NodesChanged += _model_NodesChanged;
			_model.NodesInserted += _model_NodesInserted;
			_model.NodesRemoved += _model_NodesRemoved;
			_model.StructureChanged += _model_StructureChanged;
		}

		private void UnbindModelEvents()
		{
			_model.NodesChanged -= _model_NodesChanged;
			_model.NodesInserted -= _model_NodesInserted;
			_model.NodesRemoved -= _model_NodesRemoved;
			_model.StructureChanged -= _model_StructureChanged;
		}

		private void _model_StructureChanged(object sender, TreePathEventArgs e)
		{
			if(e.Path == null)
				throw new ArgumentNullException();

			TreeNodeAdv node = FindNode(e.Path);
			if(node != null) {
				if(node != Root)
					node.IsLeaf = Model.IsLeaf(node.Tag);

				var list = new Dictionary<object, object>();
				SaveExpandedNodes(node, list);
				ReadChilds(node);
				RestoreExpandedNodes(node, list);

				UpdateSelection();
				SmartFullUpdate();
			}
			//else 
			//	throw new ArgumentException("Path not found");
		}

		private void RestoreExpandedNodes(TreeNodeAdv node, Dictionary<object, object> list)
		{
			if(node.Tag != null && list.ContainsKey(node.Tag)) {
				node.IsExpanded = true;
				foreach(var child in node.Nodes)
					RestoreExpandedNodes(child, list);
			}
		}

		private void SaveExpandedNodes(TreeNodeAdv node, Dictionary<object, object> list)
		{
			if(node.IsExpanded && node.Tag != null) {
				list.Add(node.Tag, null);
				foreach(var child in node.Nodes)
					SaveExpandedNodes(child, list);
			}
		}

		private void _model_NodesRemoved(object sender, TreeModelEventArgs e)
		{
			TreeNodeAdv parent = FindNode(e.Path);
			if(parent != null) {
				if(e.Indices != null) {
					List<int> list = new List<int>(e.Indices);
					list.Sort();
					for(int n = list.Count - 1; n >= 0; n--) {
						int index = list[n];
						if(index >= 0 && index <= parent.ChildCount)
							parent.Nodes.RemoveAt(index);
						else
							throw new ArgumentOutOfRangeException("Index out of range");
					}
				} else {
					for(int i = parent.ChildCount - 1; i >= 0; i--) {
						for(int n = 0; n < e.Children.Length; n++)
							if(parent.Nodes[i].Tag == e.Children[n]) {
								parent.Nodes.RemoveAt(i);
								break;
							}
					}
				}
			}
			UpdateSelection();
			SmartFullUpdate();
		}

		private void _model_NodesInserted(object sender, TreeModelEventArgs e)
		{
			if(e.Indices == null)
				throw new ArgumentNullException("Indices");

			TreeNodeAdv parent = FindNode(e.Path);
			if(parent != null) {
				for(int i = 0; i < e.Children.Length; i++)
					AddNewNode(parent, e.Children[i], e.Indices[i]);
			}
			SmartFullUpdate();
		}

		private void _model_NodesChanged(object sender, TreeModelEventArgs e)
		{
			TreeNodeAdv parent = FindNode(e.Path);
			if(parent != null && parent.IsVisible && parent.IsExpanded) {
				if(InvokeRequired)
					BeginInvoke(new UpdateContentWidthDelegate(ClearNodesSize), e, parent);
				else
					ClearNodesSize(e, parent);
				SmartFullUpdate();
			}
		}

		private delegate void UpdateContentWidthDelegate(TreeModelEventArgs e, TreeNodeAdv parent);
		private void ClearNodesSize(TreeModelEventArgs e, TreeNodeAdv parent)
		{
			var cc = parent.ChildCount;
			if(e.Indices != null) {
				foreach(int index in e.Indices) {
					if(index >= 0 && index < cc) {
						TreeNodeAdv node = parent.Nodes[index];
						node.Height = node.RightBounds = null;
					} else
						throw new ArgumentOutOfRangeException("Index out of range");
				}
			} else if(cc > 0) {
				foreach(TreeNodeAdv node in parent.Nodes) {
					foreach(object obj in e.Children)
						if(node.Tag == obj) {
							node.Height = node.RightBounds = null;
						}
				}
			}
		}
		#endregion
	}
}
