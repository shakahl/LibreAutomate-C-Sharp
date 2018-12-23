using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	public partial class TreeViewAdv
	{
		private Cursor _innerCursor = null;

		public override Cursor Cursor
		{
			get
			{
				if(_innerCursor != null)
					return _innerCursor;
				else
					return base.Cursor;
			}
			set
			{
				base.Cursor = value;
			}
		}

		#region Internal Properties

		private IRowLayout _rowLayout;
		internal IRowLayout RowLayout { get => _rowLayout; }

		private bool _dragMode;
		private bool DragMode
		{
			get { return _dragMode; }
			set
			{
				_dragMode = value;
				if(!value) {
					StopDragTimer();
					if(_dragBitmap != null)
						_dragBitmap.Dispose();
					_dragBitmap = null;
				} else
					StartDragTimer();
			}
		}

		internal int ColumnHeaderHeight
		{
			get
			{
				if(!UseColumns) return 0;
				//au: was fixed height, did not depend on DPI and even on font.
				if(_columnHeaderHeight == 0) {
					//_columnHeaderHeight = this.FontHeight + 4;
					Size z = TextRenderer.MeasureText("A", Font);
					//Size z = TextRenderer.MeasureText(_measureContext.Graphics, "A", Font); //unexpected: much slower
					_columnHeaderHeight = z.Height * 6 / 5 + 2;
					//Au.Output.WriteList(z.Height, _columnHeaderHeight);
				}
				return _columnHeaderHeight;
			}
		}

		int _columnHeaderHeight;
		//if(Application.RenderWithVisualStyles)
		//	_columnHeaderHeight = 20;
		//else
		//	_columnHeaderHeight = 17;


		/// <summary>
		/// returns all nodes, which parent is expanded
		/// </summary>
		private IEnumerable<TreeNodeAdv> VisibleNodes
		{
			get
			{
				TreeNodeAdv node = Root;
				while(node != null) {
					node = node.NextVisibleNode;
					if(node != null)
						yield return node;
				}
			}
		}

		private bool _suspendSelectionEvent;
		internal void _SuspendSelectionEvent(bool on)
		{
			if(on != _suspendSelectionEvent) {
				_suspendSelectionEvent = on;
				if(!_suspendSelectionEvent && _fireSelectionEvent)
					OnSelectionChanged(_selectionReason);
			}
		}
		SelectionReason _selectionReason;
		internal void SuspendSelectionEvent(SelectionReason reason)
		{
			_SuspendSelectionEvent(true);
			_selectionReason = reason;
		}
		internal void ResumeSelectionEvent()
		{
			_SuspendSelectionEvent(false);
		}

		private List<TreeNodeAdv> _rowMap;
		internal List<TreeNodeAdv> RowMap
		{
			get { return _rowMap; }
		}

		private TreeNodeAdv _selectionStart;
		internal TreeNodeAdv SelectionStart
		{
			get { return _selectionStart; }
			set { _selectionStart = value; }
		}

		private InputState _input;
		internal InputState Input
		{
			get { return _input; }
			set
			{
				_input = value;
			}
		}

		private bool _itemDragMode;
		internal bool ItemDragMode
		{
			get { return _itemDragMode; }
			set { _itemDragMode = value; }
		}

		private Point _itemDragStart;
		internal Point ItemDragStart
		{
			get { return _itemDragStart; }
			set { _itemDragStart = value; }
		}


		/// <summary>
		/// Number of rows fits to the current page
		/// </summary>
		internal int CurrentPageSize
		{
			get
			{
				return _rowLayout.CurrentPageSize;
			}
		}

		/// <summary>
		/// Number of all visible nodes (which parent is expanded)
		/// </summary>
		[Browsable(false)]
		public int RowCount
		{
			get
			{
				return RowMap.Count;
			}
		}

		private int _contentWidth = 0;
		private int ContentWidth
		{
			get
			{
				return _contentWidth;
			}
		}

		private int _firstVisibleRow;
		internal int FirstVisibleRow
		{
			get { return _firstVisibleRow; }
			set
			{
				HideEditor();
				_firstVisibleRow = value;
				UpdateView();
			}
		}

		private int _offsetX;
		/// <summary>
		/// How many pixels scrolled horizontally.
		/// </summary>
		/// <remarks>
		/// Some functions and events give virtual bounds of node controls. They don't include scroll offsets and header height. To get real bounds in client area, subtract OffsetX and OffsetY.
		/// </remarks>
		public int OffsetX
		{
			get { return _offsetX; }
			private set
			{
				HideEditor();
				_offsetX = value;
				UpdateView();
			}
		}

		/// <summary>
		/// How many pixels scrolled vertically, minus ColumnHeaderHeight.
		/// </summary>
		/// <remarks>
		/// Some functions and events give virtual bounds of node controls. They don't include scroll offsets and header height. To get real bounds in client area, subtract OffsetX and OffsetY.
		/// </remarks>
		public int OffsetY
		{
			get { return _rowLayout.GetRowBounds(FirstVisibleRow).Y - ColumnHeaderHeight; }
		}

		public Rectangle RectVirtualToClient(Rectangle r)
		{
			r.X -= OffsetX;
			r.Y -= OffsetY;
			return r;
		}

		public int XVirtualToClient(int x)
		{
			return x - OffsetX;
		}

		public int YVirtualToClient(int y)
		{
			return y - OffsetY;
		}

		private List<TreeNodeAdv> _selection;
		internal List<TreeNodeAdv> Selection
		{
			get { return _selection; }
		}

		#endregion

		#region Public Properties

		#region DesignTime

		private bool _shiftFirstNode;
		[DefaultValue(false), Category("Behavior")]
		public bool ShiftFirstNode
		{
			get { return _shiftFirstNode; }
			set { _shiftFirstNode = value; }
		}

		private bool _displayDraggingNodes;
		[DefaultValue(false), Category("Behavior")]
		public bool DisplayDraggingNodes
		{
			get { return _displayDraggingNodes; }
			set { _displayDraggingNodes = value; }
		}

		private bool _fullRowSelect;
		[DefaultValue(false), Category("Behavior")]
		public bool FullRowSelect
		{
			get { return _fullRowSelect; }
			set
			{
				_fullRowSelect = value;
				UpdateView();
			}
		}

		private bool _useColumns;
		[DefaultValue(false), Category("Behavior")]
		public bool UseColumns
		{
			get { return _useColumns; }
			set
			{
				_useColumns = value;
				FullUpdate();
			}
		}

		private bool _allowColumnReorder;
		[DefaultValue(false), Category("Behavior")]
		public bool AllowColumnReorder
		{
			get { return _allowColumnReorder; }
			set { _allowColumnReorder = value; }
		}

		private bool _showLines = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowLines
		{
			get { return _showLines; }
			set
			{
				_showLines = value;
				UpdateView();
			}
		}

		private bool _showPlusMinus = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowPlusMinus
		{
			get { return _showPlusMinus; }
			set
			{
				_showPlusMinus = value;
				FullUpdate();
			}
		}

		private bool _showNodeToolTips = false;
		[DefaultValue(false), Category("Behavior")]
		public bool ShowNodeToolTips
		{
			get { return _showNodeToolTips; }
			set { _showNodeToolTips = value; }
		}

		private ITreeModel _model;
		/// <Summary>
		/// The model associated with this <see cref="TreeViewAdv"/>.
		/// </Summary>
		/// <seealso cref="ITreeModel"/>
		/// <seealso cref="TreeModel"/>
		[Browsable(false)]
		public ITreeModel Model
		{
			get { return _model; }
			set
			{
				if(_model != value) {
					//AbortBackgroundExpandingThreads();
					if(_model != null)
						UnbindModelEvents();
					_model = value;
					CreateNodes();
					FullUpdate();
					if(_model != null)
						BindModelEvents();
				}
			}
		}

		private BorderStyle _borderStyle = BorderStyle.Fixed3D;
		[DefaultValue(BorderStyle.Fixed3D), Category("Appearance")]
		public BorderStyle BorderStyle
		{
			get
			{
				return this._borderStyle;
			}
			set
			{
				if(_borderStyle != value) {
					_borderStyle = value;
					base.UpdateStyles();
				}
			}
		}

		private bool _autoRowHeight = true;
		/// <summary>
		/// Set to true to expand each row's height to fit the text of it's largest column.
		/// </summary>
		[DefaultValue(true), Category("Appearance"), Description("Expand each row's height to fit the text of it's tallest column.")]
		public bool AutoRowHeight
		{
			get
			{
				return _autoRowHeight;
			}
			set
			{
				_autoRowHeight = value;
				if(value)
					_rowLayout = new AutoRowHeightLayout(this, RowHeight);
				else
					_rowLayout = new FixedRowHeightLayout(this, RowHeight);
				FullUpdate();
			}
		}

		private GridLineStyle _gridLineStyle = GridLineStyle.None;
		[DefaultValue(GridLineStyle.None), Category("Appearance")]
		public GridLineStyle GridLineStyle
		{
			get
			{
				return _gridLineStyle;
			}
			set
			{
				if(value != _gridLineStyle) {
					_gridLineStyle = value;
					UpdateView();
					OnGridLineStyleChanged();
				}
			}
		}

		private int _rowHeight = 16;
		[DefaultValue(16), Category("Appearance")]
		public int RowHeight
		{
			get
			{
				return _rowHeight;
			}
			set
			{
				if(value <= 0)
					throw new ArgumentOutOfRangeException("value");

				_rowHeight = value;
				_rowLayout.PreferredRowHeight = value;
				FullUpdate();
			}
		}

		private TreeSelectionMode _selectionMode = TreeSelectionMode.Single;
		[DefaultValue(TreeSelectionMode.Single), Category("Behavior")]
		public TreeSelectionMode SelectionMode
		{
			get { return _selectionMode; }
			set { _selectionMode = value; }
		}

		private bool _hideSelection;
		[DefaultValue(false), Category("Behavior")]
		public bool HideSelection
		{
			get { return _hideSelection; }
			set
			{
				_hideSelection = value;
				UpdateView();
			}
		}

		private float _topEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float DragDropTopEdgeSensivity
		{
			get { return _topEdgeSensivity; }
			set
			{
				if(value < 0 || value > 1)
					throw new ArgumentOutOfRangeException();
				_topEdgeSensivity = value;
			}
		}

		private float _bottomEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float DragDropBottomEdgeSensivity
		{
			get { return _bottomEdgeSensivity; }
			set
			{
				if(value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("value should be from 0 to 1");
				_bottomEdgeSensivity = value;
			}
		}

		private bool _loadOnDemand;
		[DefaultValue(false), Category("Behavior")]
		public bool LoadOnDemand
		{
			get { return _loadOnDemand; }
			set { _loadOnDemand = value; }
		}

		private bool _unloadCollapsedOnReload = false;
		[DefaultValue(false), Category("Behavior")]
		public bool UnloadCollapsedOnReload
		{
			get { return _unloadCollapsedOnReload; }
			set { _unloadCollapsedOnReload = value; }
		}

		private int _indent = 16;
		[DefaultValue(16), Category("Behavior")]
		public int Indent
		{
			get { return _indent; }
			set
			{
				_indent = value;
				UpdateView();
			}
		}

		private Color _lineColor = SystemColors.ControlDark;
		[Category("Behavior")]
		public Color LineColor
		{
			get { return _lineColor; }
			set
			{
				_lineColor = value;
				CreateLinePen();
				UpdateView();
			}
		}

		private Color _dragDropMarkColor = Color.MidnightBlue;
		[Category("Behavior")]
		public Color DragDropMarkColor
		{
			get { return _dragDropMarkColor; }
			set
			{
				_dragDropMarkColor = value;
				_markPen?.Dispose(); _markPen = null;
			}
		}

		private float _dragDropMarkWidth = 3.0f;
		[DefaultValue(3.0f), Category("Behavior")]
		public float DragDropMarkWidth
		{
			get { return _dragDropMarkWidth; }
			set
			{
				_dragDropMarkWidth = value;
				_markPen?.Dispose(); _markPen = null;
			}
		}

		private bool _highlightDropPosition = true;
		[DefaultValue(true), Category("Behavior")]
		public bool HighlightDropPosition
		{
			get { return _highlightDropPosition; }
			set { _highlightDropPosition = value; }
		}

		private TreeColumnCollection _columns;
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Collection<TreeColumn> Columns
		{
			get { return _columns; }
		}

		private NodeControlsCollection _controls;
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(NodeControlCollectionEditor), typeof(UITypeEditor))]
		public Collection<NodeControl> NodeControls
		{
			get
			{
				return _controls;
			}
		}

		#endregion

		#region RunTime

		private IToolTipProvider _defaultToolTipProvider = null;
		[Browsable(false)]
		public IToolTipProvider DefaultToolTipProvider
		{
			get { return _defaultToolTipProvider; }
			set { _defaultToolTipProvider = value; }
		}

		[Browsable(false)]
		public IEnumerable<TreeNodeAdv> AllNodes
		{
			get
			{
				if(_root.TryGetFirstChild(out var node)) {
					while(node != null) {
						yield return node;
						if(node.TryGetFirstChild(out var n))
							node = n;
						else if(node.NextNode != null)
							node = node.NextNode;
						else
							node = node.BottomNode;
					}
				}
			}
		}

		private DropPosition _dropPosition;
		[Browsable(false)]
		public DropPosition DropPosition
		{
			get { return _dropPosition; }
			set { _dropPosition = value; }
		}

		private TreeNodeAdv _root;
		[Browsable(false)]
		public TreeNodeAdv Root
		{
			get { return _root; }
		}

		private ReadOnlyCollection<TreeNodeAdv> _readonlySelection;
		[Browsable(false)]
		public ReadOnlyCollection<TreeNodeAdv> SelectedNodes
		{
			get
			{
				return _readonlySelection;
			}
		}

		[Browsable(false)]
		public TreeNodeAdv SelectedNode
		{
			get
			{
				if(Selection.Count > 0) {
					if(CurrentNode != null && CurrentNode.IsSelected)
						return CurrentNode;
					else
						return Selection[0];
				} else
					return null;
			}
			set
			{
				if(SelectedNode == value)
					return;

				BeginUpdate();
				try {
					if(value == null) {
						ClearSelectionInternal();
					} else {
						if(!IsMyNode(value))
							throw new ArgumentException();

						ClearSelectionInternal();
						value.IsSelected = true;
						CurrentNode = value;
						EnsureVisible(value);
					}
				}
				finally {
					EndUpdate();
				}
			}
		}

		private TreeNodeAdv _currentNode;
		[Browsable(false)]
		public TreeNodeAdv CurrentNode
		{
			get { return _currentNode; }
			internal set { _currentNode = value; }
		}

		#endregion

		#endregion

	}
}
