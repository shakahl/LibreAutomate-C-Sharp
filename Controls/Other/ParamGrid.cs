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

using SG = SourceGrid;
using Editors = SourceGrid.Cells.Editors;
using DevAge.Drawing;

namespace Au.Controls
{
	/// <summary>
	/// 2-column grid control similar to a propertygrid but more flexible.
	/// Used in many code tools to edit function arguments etc. For example in "Find window or control".
	/// </summary>
	public class ParamGrid : SG.Grid
	{
		Editors.TextBox _editor;
		_CellController _controller;
		SG.Cells.Controllers.ToolTipText _controllerTooltip0, _controllerTooltip1;

		const SG.EditableMode c_editableMode = SG.EditableMode.F2Key | SG.EditableMode.AnyKey; //default is these + doubleclick. See also OnMouseDown.

		public ParamGrid()
		{
			this.ColumnsCount = 2; //let the programmer set = 1 if need only for flags
			this.MinimumHeight = 18; //height when font is SegoeUI,9. With most other fonts it's smaller.
			this.SpecialKeys = SG.GridSpecialKeys.Default & ~SG.GridSpecialKeys.Tab;

			//this.AutoStretchColumnsToFitWidth = true; //does not work well. Instead we resize in OnClientSizeChanged etc.

			_editor = new Editors.TextBox(typeof(string)) { EditableMode = c_editableMode };
			var c = _editor.Control;
			c.Multiline = true; //let all rows have the same multiline editor, even if the value cannot be multiline

			this.Controller.AddController(_controller = new _CellController(this));
			//this.Controller.AddController(SG.Cells.Controllers.Resizable.ResizeWidth); //we resize width and height automatically, but the user may want to resize width. This is like in VS. Rejected because the control does not resize normally.
			_controllerTooltip0 = new SG.Cells.Controllers.ToolTipText() /*{ IsBalloon = true }*/;
			_controllerTooltip1 = new SG.Cells.Controllers.ToolTipText();

			//this.Font = new Font("Verdana", 8);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			//somehow this does not work in ctor
			var sel = this.Selection as SG.Selection.SelectionBase;
			var border = sel.Border;
			border.SetWidth(1);
			border.SetColor(Color.Blue);
			border.SetDashStyle(System.Drawing.Drawing2D.DashStyle.Dot);
			sel.Border = border;
			sel.BackColor = sel.FocusBackColor;
			sel.EnableMultiSelection = false;

			//SourceGrid.Grid always shows a selection rect after entering first time.
			//	The properties only allow to change background color when focused/nonfocused.
			//	We use OnEnter/OnLeave to show a focus rect only when the control is focused.

			VScrollBar.LocationChanged += (unu, sed) => _AutoSizeLastColumn(); //when vscrollbar added/removed (SB width changed); when grid width changed.
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if(this._comboDD != null) {
				this._comboDD.PopupWindow.Dispose();
				this._comboDD = null;
			}
			base.OnHandleDestroyed(e);
		}

		//#if DEBUG
		//		public bool ZDebug { get; set; }
		//#endif

		void _ShowCellFocusRect(bool yes)
		{
			var sel = this.Selection as SG.Selection.SelectionBase;
			var border = sel.Border;
			border.SetWidth(yes ? 1 : 0);
			sel.Border = border;
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			_ShowCellFocusRect(true);
		}

		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			_ShowCellFocusRect(false);
		}

		void _AutoSizeLastColumn()
		{
			//Print(this.Name, this.RowsCount);
			if(this.RowsCount > 0) {
				int n = this.ClientSize.Width;
				if(this.VScrollBarVisible) n -= this.VScrollBar.Width;
				int col = 0;
				if(this.ColumnsCount > 1) {
					n -= this.Columns[0].Width;
					col = 1;
				}
				this.Columns[col].Width = Math.Max(n, 0);
			}

			//SHOULDDO: also call this when the user resizes column 0. Or don't allow to resize, why need it.
		}

		int _MeasureColumnWidth(int column)
		{
			//This code taken from MeasureColumnWidth and modified to skip col-spanned cells etc.

			int wid = 10;
			for(int r = 0, n = this.RowsCount; r < n; r++) {
				var cell = this[r, column];
				if(cell == null || cell.ColumnSpan != 1) continue;
				var cellContext = new SG.CellContext(this, new SG.Position(r, column), cell);
				Size cellSize = cellContext.Measure(default);
				if(cellSize.Width > wid) wid = cellSize.Width;
			}
			return wid;
		}

		/// <summary>
		/// Call this after adding all rows.
		/// </summary>
		public void ZAutoSize(bool rows = true, bool columns = true)
		{
			if(rows) {
				this.Rows.AutoSize(false);
			}
			if(columns) {
				if(this.ColumnsCount > 1 && this.RowsCount > 0) {
					//this.Columns.AutoSizeColumn(0); //no, can be too wide. There is MinimumWidth but no MaximumWidth.
					//int wid = this.Columns.MeasureColumnWidth(0, false, 0, this.RowsCount - 1); //no, it does not work well with col-spanned cells
					int wid = _MeasureColumnWidth(0);
					this.Columns.SetWidth(0, Math.Min(wid, this.ClientSize.Width / 2));
				}
				_AutoSizeLastColumn();
			}
		}

		class _CellController : SG.Cells.Controllers.ControllerBase
		{
			ParamGrid _grid;

			public _CellController(ParamGrid grid)
			{
				_grid = grid;
			}

			public override void OnValueChanged(SG.CellContext c, EventArgs e)
			{
				base.OnValueChanged(c, e);

				var pos = c.Position;
				if(pos.Column == 1) {
					_grid.Rows.AutoSizeRow(pos.Row);
					if(c.IsEditing()) _grid.ZCheck(pos.Row, true); //note: this alone would interfere with the user clicking the checkbox of this row. OnMouseDown prevents it.
				}

				_grid.ZOnValueChanged(c);
			}

			//public override void OnEditStarting(SG.CellContext sender, CancelEventArgs e)
			//{
			//	base.OnEditStarting(sender, e);
			//	ATime.SleepDoEvents(500);
			//}

			public override void OnEditStarted(SG.CellContext c, EventArgs e)
			{
				//ADebug.PrintFunc();

				if(c.Cell.Editor is Editors.ComboBox cb) { //read-only combo. The grid shows a ComboBox control.
					cb.Control.DroppedDown = true;
				} else if(c.Cell is ComboCell cc) { //editable combo. We show PopupList _comboDD. With ComboBox too ugly and limited, eg cannot edit multiline.
					var tb = (cc.Editor as Editors.TextBox).Control;
					tb.Width -= cc.MeasuredButtonWidth; //don't hide the drop-down button
				}

				base.OnEditStarted(c, e);

				_ShowEditInfo(c, true);
			}

			public override void OnEditEnded(SG.CellContext c, EventArgs e)
			{
				_ShowEditInfo(c, false);

				base.OnEditEnded(c, e);
			}

			void _ShowEditInfo(SG.CellContext c, bool show)
			{
				var t = c.Cell as EditCell;
				if(t?.Info != null) _grid.ZOnShowEditInfo(c, show ? t.Info : null);
			}

			public override void OnMouseDown(SG.CellContext c, MouseEventArgs e)
			{
				base.OnMouseDown(c, e);

				//Start cell editing on mouse button down, prevent selecting all text, and set caret correctly.

				//Print(sender.Cell.Editor);
				c.StartEdit();

				TextBox tb = null;
				switch(c.Cell.Editor) {
				case Editors.TextBox ce: tb = ce.Control; break;
				case Editors.TextBoxButton ce: tb = ce.Control.TextBox; break;
				}
				if(tb != null) {
					var wt = (AWnd)tb;
					//tb.SelectionLength = 0; tb.SelectionStart = 0; tb.ScrollToCaret();
					wt.Send(Api.WM_HSCROLL, Api.SB_TOP);
					if(e.X < tb.Right) {
						POINT p = (e.X, e.Y); ((AWnd)_grid).MapClientToClientOf(wt, ref p);
						wt.Post(Api.WM_LBUTTONDOWN, Api.MK_LBUTTON, AMath.MakeUint(p.x, p.y));
					} else if(c.Cell is ComboCell cc) {
						cc.ShowDropDown(); //clicked the drop-down button
					}
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(ZGetEditCell(out var c)) { //if in edit mode
				if(c.Position != PositionAtPoint(e.Location)) {
					c.EndEdit(cancel: false);
				} else if(c.Cell is ComboCell cc && !_comboNoDD) {
					cc.ShowDropDown();
				}
				return;
			}

			base.OnMouseDown(e);
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_USER + 10:
				_comboNoDD = false;
				break;
			}
			base.WndProc(ref m);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch(keyData) {
			case Keys.Down:
			case Keys.Down | Keys.Alt:
				if(ZGetEditCell(out var c) && c.Cell is ComboCell cc) {
					cc.ShowDropDown();
					return true;
				}
				break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		protected virtual void ZOnValueChanged(SG.CellContext sender)
		{
			ZValueChanged?.Invoke(sender);
		}

		/// <summary>
		/// When changed text of a value cell or state of a checkbox cell.
		/// </summary>
		public event Action<SG.CellContext> ZValueChanged;

		protected virtual void ZOnShowEditInfo(SG.CellContext sender, string info)
		{
			ZShowEditInfo?.Invoke(sender, info);
		}

		/// <summary>
		/// When started and ended editing a cell that has info.
		/// When ended, the string is null.
		/// </summary>
		public event Action<SG.CellContext, string> ZShowEditInfo;

		/// <summary>
		/// Simple editable text cell.
		/// Adds to SG.Cells.Cell: property Info.
		/// </summary>
		public class EditCell : SG.Cells.Cell
		{
			public EditCell(string value) : base(value, typeof(string)) { }

			public string Info { get; set; }
		}

		/// <summary>
		/// Editable text cell with drop-down button that shows drop-down list similar to combo box.
		/// </summary>
		public class ComboCell : EditCell
		{
			string[] _items;
			Func<string[]> _callback;

			internal ComboCell(string[] items, int iSelect) : base(iSelect >= 0 ? items[iSelect] : "")
			{
				_items = items;
			}

			internal ComboCell(Func<string[]> callback) : base("")
			{
				_callback = callback;
			}

			internal int MeasuredButtonWidth => ((this.View as SG.Cells.Views.ComboBox).ElementDropDown as _ComboButton).MeasuredWidth;

			internal void ShowDropDown()
			{
				var g = Grid as ParamGrid;
				var p = g._comboDD;
				var items = _items;
				if(items == null) items = _callback?.Invoke();
				if(items == null || items.Length == 0) return;

				var t = (this.Editor as Editors.TextBox).Control;
				t.Update(); g.Update(); //paint controls before the popup animation, to avoid flickering
				if(p == null) {
					g._comboDD = p = new PopupList { MultiShow = true, ComboBoxAnimation = true };

					//prevent showing drop-down again when the user clicks the drop-down button to close it
					p.PopupWindow.VisibleChanged += (se1, sed) => {
						if((se1 as Control).Visible) g._comboNoDD = true;
						else ((AWnd)g).Post(Api.WM_USER + 10); //WndProc will set _comboNoDD = false
					};
				}
				p.Items = items;
				p.SelectedAction = pp => {
					t.Value = pp.ResultItem as string;
					if(!pp.ResultWasKey) g.ZEndEdit(cancel: false);
				};
				var r = g.PositionToRectangle(new SG.Position(Row.Index, Column.Index));
				p.Show(g, r);
			}
		}
		PopupList _comboDD;
		bool _comboNoDD;

		enum _ViewType
		{
			//column 0
			Check, Readonly, HeaderRow, HeaderRowCheck,
			//column 1
			Edit, Combo, Button
		}
		SG.Cells.Views.Cell[] _views = new SG.Cells.Views.Cell[7];
		static DevAge.Drawing.VisualElements.TextRenderer s_textRenderer = new DevAge.Drawing.VisualElements.TextRenderer(); //default GDI+ //ExpandTabs ignored. It seems it can be used only in SourceGrid.Cells.Views.Cell.PrepareVisualElementText override. Never mind.

		SG.Cells.Views.Cell _GetView(_ViewType type)
		{
			ref SG.Cells.Views.Cell view = ref _views[(int)type];
			switch(type) {
			case _ViewType.Edit:
				if(view == null) view = new SG.Cells.Views.Cell {
					ElementText = s_textRenderer,
					Padding = new DevAge.Drawing.Padding(0, 2, 0, 0) //default all 2 2 2 2
				};
				break;
			case _ViewType.Combo:
				if(view == null) view = new SG.Cells.Views.ComboBox {
					ElementText = s_textRenderer,
					Padding = new DevAge.Drawing.Padding(0, 0, 0, 0),
					ElementDropDown = new _ComboButton()
				};
				break;
			case _ViewType.Button:
				if(view == null) view = new SG.Cells.Views.Button {
					ElementText = s_textRenderer,
					Padding = new DevAge.Drawing.Padding(0, 0, 0, 0)
				};
				break;
			case _ViewType.Check:
				if(view == null) view = new SG.Cells.Views.CheckBox {
					ElementText = s_textRenderer,
					BackColor = Form.DefaultBackColor,
					CheckBoxAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft,
					Padding = new DevAge.Drawing.Padding(2, 2, 0, 0)
				};
				break;
			case _ViewType.Readonly:
				if(view == null) view = new SG.Cells.Views.Cell {
					ElementText = s_textRenderer,
					BackColor = Form.DefaultBackColor,
					Padding = new DevAge.Drawing.Padding(0, 2, 0, 0)
				};
				break;
			case _ViewType.HeaderRow:
				if(view == null) view = new SG.Cells.Views.Cell {
					ElementText = s_textRenderer,
					TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter,
					Padding = new DevAge.Drawing.Padding(2.1f),
					Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Silver, Color.WhiteSmoke, 90)
				};
				break;
			case _ViewType.HeaderRowCheck:
				if(view == null) view = new SG.Cells.Views.CheckBox {
					ElementText = s_textRenderer,
					TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter,
					CheckBoxAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft, //default MiddleCenter draws on text
					Padding = new DevAge.Drawing.Padding(2.1f),
					Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Silver, Color.WhiteSmoke, 90)
				};
				break;
			}
			return view;
		}

		//Combo button that does not make the cell taller.
		class _ComboButton : DevAge.Drawing.VisualElements.DropDownButtonThemed
		{
			public _ComboButton()
			{
				AnchorArea = new AnchorArea(float.NaN, 0, 0, 0, false, false);
			}

			protected override SizeF OnMeasureContent(MeasureHelper measure, SizeF maxSize)
			{
				var z = base.OnMeasureContent(measure, maxSize);
				z.Height = 16; //info: can be even 0; grid does not draw it smaller than cell height.
				MeasuredWidth = (int)z.Width;
				return z;
			}

			public int MeasuredWidth { get; private set; }
		}

		enum _RowType
		{
			/// <summary>Checkbox and editable cell. If check is null, adds lebel instead of checkbox.</summary>
			Editable,

			/// <summary>Only checkbox.</summary>
			Check,

			/// <summary>Only label. If check is not null, adds checkbox instead of label.</summary>
			Header,
		}

		/// <summary>
		/// Types of the editable cell.
		/// </summary>
		public enum EditType
		{
			/// <summary>Simple editable text.</summary>
			Text,

			/// <summary>Editable text with combobox-like drop-down.</summary>
			ComboText,

			/// <summary>Read-only combobox.</summary>
			ComboList,

			/// <summary>Editable text with button.</summary>
			TextButton,

			/// <summary>Button.</summary>
			Button,
		}

		int _AddRow(string key, string name, object value, bool? check, _RowType type, string tt, string info, int insertAt,
			EditType etype = EditType.Text, EventHandler buttonAction = null, int comboIndex = -1)
		{
			int r = insertAt < 0 ? this.RowsCount : insertAt;

			this.Rows.Insert(r);
			if(ZAddHidden) this.Rows[r].Visible = false;

			SG.Cells.Cell c; SG.Cells.Views.Cell view;
			if(check == null) {
				c = new SG.Cells.Cell(name);
				if(type == _RowType.Header) {
					view = _GetView(_ViewType.HeaderRow);
					c.AddController(SG.Cells.Controllers.Unselectable.Default);
				} else {
					view = _GetView(_ViewType.Readonly);
				}
			} else {
				c = new SG.Cells.CheckBox(name, check.GetValueOrDefault());
				if(type == _RowType.Header) {
					view = _GetView(_ViewType.HeaderRowCheck);
				} else {
					view = _GetView(_ViewType.Check);
				}
			}
			c.View = view;

			if(tt != null) {
				c.AddController(_controllerTooltip0);
				c.ToolTipText = tt;
			}

			this[r, 0] = c;

			int nc = this.ColumnsCount;
			if(nc > 1) {
				if(type == _RowType.Check || type == _RowType.Header) {
					c.ColumnSpan = nc;
				} else {
					SG.Cells.Cell t; _ViewType viewType = _ViewType.Edit;
					switch(etype) {
					case EditType.Text:
						t = new EditCell(value?.ToString()) { Editor = _editor, Info = info };
						break;
					case EditType.TextButton: {
						var ed = new Editors.TextBoxButton(typeof(string)) { EditableMode = c_editableMode };
						ed.Control.TextBox.Multiline = true;
						t = new EditCell(value?.ToString()) { Editor = ed, Info = info };
						ed.Control.DialogOpen += buttonAction;
					}
					break;
					case EditType.Button: {
						t = new SG.Cells.Button(value?.ToString());
						var ev = new SG.Cells.Controllers.Button();
						ev.Executed += buttonAction;
						t.Controller.AddController(ev);
						viewType = _ViewType.Button;
					}
					break;
					default: { //combo
						string[] a = null; Func<string[]> callback = null;
						switch(value) {
						case string s: a = s.SegSplit("|"); break;
						case string[] sa: a = sa; break;
						case List<string> sl: a = sl.ToArray(); break;
						case Func<string[]> callb: callback = callb; break;
						}
						if(etype == EditType.ComboList) { //read-only
							var ed = new Editors.ComboBox(typeof(string), a, false) { EditableMode = c_editableMode };
							var cb = ed.Control;
							cb.DropDownStyle = ComboBoxStyle.DropDownList;
							cb.SelectionChangeCommitted += (unu, sed) => ZEndEdit(false);
							if(buttonAction != null) cb.DropDown += buttonAction;
							t = new EditCell(comboIndex >= 0 ? a[comboIndex] : "") { Editor = ed, Info = info };
						} else { //editable
							var ed = _editor;
							var cc = (callback != null) ? new ComboCell(callback) : new ComboCell(a, comboIndex);
							cc.Editor = ed;
							cc.Info = info;
							t = cc;
							viewType = _ViewType.Combo;
						}
					}
					break;
					}
					t.AddController(_controllerTooltip1);
					t.View = _GetView(viewType);
					this[r, 1] = t;
				}
			}

			if(key == null) key = name;
#if DEBUG
			Debug.Assert(ZFindRow(key) < 0, "Duplicate grid row key:", key);
#endif
			this.Rows[r].Tag = key;
			return r;
		}

		#region public add/get/clear functions

		/// <summary>
		/// Adds row with checkbox (or label) and editable cell.
		/// Returns row index.
		/// </summary>
		/// <param name="key">Row's Tag property. If null, uses <paramref name="name"/>. Used with functions that have <i>rowKey</i> parameter.</param>
		/// <param name="name">Readonly text in column 0 (checkbox or label).</param>
		/// <param name="value">
		/// string.
		/// For combo can be string like "one|two|three" or string[] or List of string.
		/// For editable combo also can be Func&lt;string[]&gt; callback that returns items. Called before each dropdown.
		/// </param>
		/// <param name="check">Checked or not. If null, adds label instead of checkbox.</param>
		/// <param name="tt">Tooltip text.</param>
		/// <param name="info"><see cref="ZShowEditInfo"/> text.</param>
		/// <param name="insertAt"></param>
		/// <param name="etype">Edit cell control type.</param>
		/// <param name="buttonAction">Button click action when etype is Button, TextButton or ComboList; required if Button/TextButton.</param>
		/// <param name="comboIndex">If not -1, selects this combo box item.</param>
		public int ZAdd(string key, string name, object value = null, bool? check = false, string tt = null, string info = null, int insertAt = -1,
			EditType etype = EditType.Text, EventHandler buttonAction = null, int comboIndex = -1)
		{
			return _AddRow(key, name, value, check, _RowType.Editable, tt, info, insertAt, etype, buttonAction, comboIndex);
		}

		/// <summary>
		/// Adds row with only checkbox (without an editable cell).
		/// Returns row index.
		/// </summary>
		/// <param name="key">Row's Tag property. If null, uses <paramref name="name"/>. Used with functions that have <i>rowKey</i> parameter.</param>
		/// <param name="name">Checkbox text.</param>
		/// <param name="check"></param>
		/// <param name="tt">Tooltip text.</param>
		/// <param name="insertAt"></param>
		public int ZAddCheck(string key, string name, bool check = false, string tt = null, int insertAt = -1)
		{
			return _AddRow(key, name, null, check, _RowType.Check, tt, null, insertAt);
		}

		/// <summary>
		/// Adds a header row that can be anywhere (and multiple). It is readonly and spans all columns. Optionally with checkbox.
		/// Returns row index.
		/// </summary>
		/// <param name="name">Read-only text.</param>
		/// <param name="check">Checked or not. If null, adds label instead of checkbox.</param>
		/// <param name="tt">Tooltip text.</param>
		/// <param name="insertAt"></param>
		/// <param name="key">Row's Tag property. If null, uses <paramref name="name"/>. Used with functions that have <i>rowKey</i> parameter.</param>
		public int ZAddHeaderRow(string name, bool? check = null, string tt = null, int insertAt = -1, string key = null)
		{
			return _AddRow(key, name, null, check, _RowType.Header, tt, null, insertAt);
		}

		/// <summary>
		/// If true, ZAdd and similar functions will add hidden rows.
		/// </summary>
		public bool ZAddHidden { get; set; }

		/// <summary>
		/// Returns true if the row is checked or required.
		/// </summary>
		/// <param name="row">Row index.</param>
		/// <exception cref="ArgumentException"></exception>
		public bool ZIsChecked(int row)
		{
			_ThrowIfRowInvalid(row);
			if(this[row, 0] is SG.Cells.CheckBox cb) return cb.Checked.GetValueOrDefault();
			return this[row, 0].View == _views[(int)_ViewType.Readonly]; //required; else header row
		}

		/// <summary>
		/// Returns true if the row is checked or required.
		/// </summary>
		/// <param name="rowKey">Row key.</param>
		/// <exception cref="ArgumentException"></exception>
		public bool ZIsChecked(string rowKey) => ZIsChecked(ZFindRow(rowKey));

		/// <summary>
		/// Checks or unchecks.
		/// Does nothing if no checkbox.
		/// </summary>
		/// <param name="row">Row index.</param>
		/// <param name="check"></param>
		/// <exception cref="ArgumentException"></exception>
		public void ZCheck(int row, bool check)
		{
			_ThrowIfRowInvalid(row);
			if(this[row, 0] is SG.Cells.CheckBox cb) cb.Checked = check;
		}

		/// <summary>
		/// Checks or unchecks.
		/// Use only for flags and optionals, not for required.
		/// </summary>
		/// <param name="rowKey">Row key.</param>
		/// <param name="check"></param>
		/// <exception cref="ArgumentException"></exception>
		public void ZCheck(string rowKey, bool check) => ZCheck(ZFindRow(rowKey), check);

		/// <summary>
		/// Checks or unchecks if rowKey exists.
		/// Use only for flags and optionals, not for required.
		/// </summary>
		/// <param name="rowKey">Row key.</param>
		/// <param name="check"></param>
		public void ZCheckIfExists(string rowKey, bool check) { int i = ZFindRow(rowKey); if(i >= 0) ZCheck(i, check); }

		/// <summary>
		/// If the row is checked or required, gets its value and returns true.
		/// </summary>
		/// <param name="row">Row index.</param>
		/// <param name="value"></param>
		/// <param name="falseIfEmpty">Return false if the value is empty (null).</param>
		/// <param name="falseIfHidden">Return false if the row is hidden.</param>
		/// <exception cref="ArgumentException"></exception>
		public bool ZGetValue(int row, out string value, bool falseIfEmpty, bool falseIfHidden = false)
		{
			value = null;
			_ThrowIfRowInvalid(row);
			if(!ZIsChecked(row)) return false;
			if(falseIfHidden && !Rows[row].Visible) return false;
			value = ZGetCellText(row, 1);
			if(falseIfEmpty && value == null) return false;
			return true;
		}

		/// <summary>
		/// If the row is checked or required, gets its value and returns true.
		/// </summary>
		/// <param name="rowKey">Row key.</param>
		/// <param name="value"></param>
		/// <param name="falseIfEmpty">Return false if the value is empty (null).</param>
		/// <param name="falseIfHidden">Return false if the row is hidden.</param>
		/// <exception cref="ArgumentException"></exception>
		public bool ZGetValue(string rowKey, out string value, bool falseIfEmpty, bool falseIfHidden = false)
			=> ZGetValue(ZFindRow(rowKey), out value, falseIfEmpty, falseIfHidden);

		/// <summary>
		/// If the row exists and is checked or required, gets its value and returns true.
		/// </summary>
		/// <param name="rowKey">Row key. If not found, returns false.</param>
		/// <param name="value"></param>
		/// <param name="falseIfEmpty">Return false if the value is empty (null).</param>
		/// <param name="falseIfHidden">Return false if the row is hidden.</param>
		public bool ZGetValueIfExists(string rowKey, out string value, bool falseIfEmpty, bool falseIfHidden = false)
		{
			int row = ZFindRow(rowKey);
			if(row < 0) { value = null; return false; }
			return ZGetValue(row, out value, falseIfEmpty, falseIfHidden);
		}

		/// <summary>
		/// Gets cell value or checkbox label.
		/// </summary>
		/// <param name="row">Row index. If negative, asserts and returns null.</param>
		/// <param name="column">Column index.</param>
		/// <exception cref="ArgumentException"></exception>
		public string ZGetCellText(int row, int column)
		{
			_ThrowIfRowInvalid(row);
			var c = this[row, column];
			if(column == 0 && c is SG.Cells.CheckBox cb) return cb.Caption;
			return c.Value as string;
		}

		/// <summary>
		/// Gets cell value or checkbox label.
		/// </summary>
		/// <param name="rowKey">Row key. If not found, asserts and returns null.</param>
		/// <param name="column">Column index.</param>
		/// <exception cref="ArgumentException"></exception>
		public string ZGetCellText(string rowKey, int column) => ZGetCellText(ZFindRow(rowKey), column);

		/// <summary>
		/// Changes cell text or checkbox label. Ends editing.
		/// </summary>
		/// <param name="row">Row index.</param>
		/// <param name="column">Column index.</param>
		/// <param name="text"></param>
		/// <exception cref="ArgumentException"></exception>
		public void ZSetCellText(int row, int column, string text)
		{
			_ThrowIfRowInvalid(row);
			ZEndEdit(row, column, true);
			var c = this[row, column];
			if(c is SG.Cells.CheckBox cb) cb.Caption = text;
			else c.Value = text;
			InvalidateCell(new SG.Position(row, column));
		}

		/// <summary>
		/// Changes cell value or checkbox label. Ends editing.
		/// </summary>
		/// <param name="rowKey">Row key.</param>
		/// <param name="column">Column index.</param>
		/// <param name="text"></param>
		/// <exception cref="ArgumentException"></exception>
		public void ZSetCellText(string rowKey, int column, string text) => ZSetCellText(ZFindRow(rowKey), column, text);

		/// <summary>
		/// Finds row by row key and returns row index.
		/// Returns -1 if not found.
		/// </summary>
		public int ZFindRow(string rowKey)
		{
			for(int r = 0, n = this.RowsCount; r < n; r++) {
				if(this.Rows[r].Tag as string == rowKey) return r;
			}
			return -1;
		}

		/// <summary>
		/// Finds row by row key and returns row index.
		/// Returns -1 if not found.
		/// </summary>
		public int ZFindRow(in AStringSegment rowKey)
		{
			for(int r = 0, n = this.RowsCount; r < n; r++) {
				if(this.Rows[r].Tag as string == rowKey) return r;
			}
			return -1;
		}

		void _ThrowIfRowInvalid(int row)
		{
			if((uint)row >= this.RowsCount) throw new ArgumentException("invalid row");
		}

		/// <summary>
		/// Gets row key.
		/// </summary>
		public string ZGetRowKey(int row) => this.Rows[row].Tag as string;

		/// <summary>
		/// Removes all rows.
		/// </summary>
		public void ZClear()
		{
			//this.Rows.Clear(); //makes editors invalid
			ZEndEdit(true);
			this.RowsCount = 0;
		}

		#endregion public add/get/clear functions

		#region other public functions

		/// <summary>
		/// If editing any cell, gets the cell context and returns true.
		/// </summary>
		public bool ZGetEditCell(out SG.CellContext c)
		{
			//Somehow SG does not have a method to get the edit cell. This code is from SG source, eg in GridVirtual.OnMouseDown.
			if(!Selection.ActivePosition.IsEmpty()) {
				c = new SG.CellContext(this, Selection.ActivePosition);
				if(c.Cell != null && c.IsEditing()) return true;
			}
			c = default;
			return false;
		}

		/// <summary>
		/// If editing the specified cell, gets cell context and returns true.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public bool ZIsEditing(int row, int col, out SG.CellContext c)
		{
			_ThrowIfRowInvalid(row);
			c = new SG.CellContext(this, new SG.Position(row, col));
			if(c.Cell != null && c.IsEditing()) return true;
			c = default;
			return false;
		}

		/// <summary>
		/// If editing any cell, ends editing and returns true.
		/// </summary>
		/// <param name="cancel">Undo changes.</param>
		public bool ZEndEdit(bool cancel)
		{
			if(!ZGetEditCell(out var c)) return false;
			c.EndEdit(cancel: false);
			return true;
		}

		/// <summary>
		/// If editing the specified cell, ends editing and returns true.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public bool ZEndEdit(int row, int col, bool cancel)
		{
			if(!ZIsEditing(row, col, out var c)) return false;
			c.EndEdit(cancel);
			return true;
		}

		/// <summary>
		/// Hides or unhides one or more rows.
		/// </summary>
		/// <param name="visible"></param>
		/// <param name="from">Index of the first row in the range.</param>
		/// <param name="count">Count of rows in the range. If -1, until the end.</param>
		/// <param name="fromOpposite"></param>
		/// <param name="countOpposite">With fromOpposite specifies range to hide if visible==true or show if visible==false. Faster than calling this function 2 times.</param>
		/// <exception cref="ArgumentException"></exception>
		public void ZShowRows(bool visible, int from, int count, int fromOpposite = 0, int countOpposite = 0)
		{
			_ThrowIfRowInvalid(from);
			_ThrowIfRowInvalid(fromOpposite);
			int to = count < 0 ? RowsCount : from + count;
			int toOpposite = countOpposite < 0 ? RowsCount : fromOpposite + countOpposite;
			for(int i = from; i < to; i++) Rows.ShowRow(i, visible);
			for(int i = fromOpposite; i < toOpposite; i++) Rows.ShowRow(i, !visible);
			if(visible || toOpposite > fromOpposite) this.Rows.AutoSize(false);
			RecalcCustomScrollBars();
		}

		/// <summary>
		/// Hides or/and unhides one or more rows and row ranges.
		/// </summary>
		/// <param name="visible"></param>
		/// <param name="rows">Row keys separated by space. Can include ranges separated by -. Example: "one four-nine". If starts or ends with -, the range starts with row 0 or ends with the last row. Example: "three-".</param>
		/// <param name="rowsOpposite">The same as <paramref name="rows"/>, but the <paramref name="visible"/> parameter has opposite meaning (hide if true, show if false). Faster than calling this function 2 times.</param>
		/// <exception cref="ArgumentException"></exception>
		public void ZShowRows(bool visible, string rows, string rowsOpposite = null)
		{
			bool madeVisible = false;
			g1:
			int prevRow = -1;
			foreach(var s in rows.Segments(" -", SegFlags.NoEmpty)) {
				int row = ZFindRow(s); if(row < 0) throw new ArgumentException("invalid row " + s.ToString());
				if(s.Offset > 0 && rows[s.Offset - 1] == '-') _ShowRange(row);
				Rows.ShowRow(row, visible);
				prevRow = row;
				madeVisible |= visible;
			}
			if(rows.Ends('-')) _ShowRange(RowsCount);

			void _ShowRange(int toRow) { while(++prevRow < toRow) Rows.ShowRow(prevRow, visible); }

			if(rowsOpposite != null) {
				rows = rowsOpposite; rowsOpposite = null; visible ^= true; goto g1;
			}

			if(madeVisible) this.Rows.AutoSize(false);
			RecalcCustomScrollBars();
		}

		/// <summary>
		/// Disables or enables cell (can be checkbox).
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public void ZEnableCell(int row, int col, bool enable)
		{
			_ThrowIfRowInvalid(row);
			if(!enable) ZEndEdit(row, col, true);
			var c = this[row, col];
			var e = c.Editor; if(e == null) return;
			if(e.EnableEdit == enable) return;
			e.EnableEdit = enable;
			InvalidateCell(c);
		}

		/// <summary>
		/// Disables or enables cell (can be checkbox).
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public void ZEnableCell(string rowKey, int col, bool enable)
			=> ZEnableCell(ZFindRow(rowKey), col, enable);

		/// <summary>
		/// Disables or enables row cells.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public void ZEnableRow(int row, bool enable, bool uncheck = false)
		{
			if(uncheck) ZCheck(row, false);
			ZEnableCell(row, 0, enable);
			ZEnableCell(row, 1, enable);
		}

		/// <summary>
		/// Disables or enables row cells.
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public void ZEnableRow(string rowKey, bool enable, bool uncheck = false)
			=> ZEnableRow(ZFindRow(rowKey), enable, uncheck);

		#endregion

		//rejected: if disabled, draw gray. Difficult to make all parts gray.
		//protected override void OnEnabledChanged(EventArgs e)
		//{
		//	//ForeColor = c; //does nothing

		//	var enabled = Enabled;
		//	s_textRenderer.Enabled = enabled;
		//	if(_views[(int)_ViewType.Combo] is SG.Cells.Views.ComboBox cb) cb.ElementDropDown.Style = enabled ? ButtonStyle.Normal : ButtonStyle.Disabled;
		//	//these don't work, because grid overrides (draws normal/hot)
		//	//if(_views[(int)_ViewType.Check] is SG.Cells.Views.CheckBox c1) c1.ElementCheckBox.Style = enabled ? ControlDrawStyle.Normal : ControlDrawStyle.Disabled;
		//	//if(_views[(int)_ViewType.HeaderRowCheck] is SG.Cells.Views.CheckBox c2) c2.ElementCheckBox.Style = enabled ? ControlDrawStyle.Normal : ControlDrawStyle.Disabled;

		//	base.OnEnabledChanged(e);
		//}
	}
}
