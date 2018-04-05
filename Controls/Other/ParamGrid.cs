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
using static Au.NoClass;

using SG = SourceGrid;

namespace Au.Controls
{
	//[DebuggerStepThrough]
	public class ParamGrid :SG.Grid
	{
		SG.Cells.Editors.TextBox _editor;
		SG.Cells.Views.Cell _viewReadonly;
		SG.Cells.Views.Cell _viewHeaderRow;
		SG.Cells.Controllers.ToolTipText _controllerTooltip0, _controllerTooltip1;

		/// <summary>
		/// The editor of values.
		/// For example, can be used to subscribe to its Control.Validating event.
		/// </summary>
		public SG.Cells.Editors.TextBox ValueEditor => _editor;

		static ParamGrid()
		{
			var tr = new DevAge.Drawing.VisualElements.TextRenderer();
			//tr.TextFormatFlags |= TextFormatFlags.ExpandTabs; //ignored. It seems it can be used only in SourceGrid.Cells.Views.Cell.PrepareVisualElementText override. Never mind.
			SG.Cells.Views.Cell.Default.ElementText = tr; //default GDI+

			//workaround for too big cell height, especially with font SegoeUI, which is the default Windows UI font
			var p1 = new DevAge.Drawing.Padding(0, 2, 0, 0);
			var p2 = new DevAge.Drawing.Padding(2, 6, 0, -1);
			SG.Cells.Views.Cell.DefaultPadding = p1; //default all 2 2 2 2
			SG.Cells.Views.Cell.Default.Padding = p1; //need both
			SG.Cells.Views.CheckBox.DefaultPadding = p2;
			//SG.Cells.Views.CheckBox.DefaultAlignment = DevAge.Drawing.ContentAlignment.TopLeft; //somehow makes big cell height

			//SG.Cells.Views.Cell.Default.WordWrap = true; //does not change row height automatically. We could do it in OnClientSizeChanged.
			SG.Cells.Views.CheckBox.DefaultBackColor = Form.DefaultBackColor; //default white

			//_viewOfValueCells = new SG.Cells.Views.Cell();
			//_viewOfValueCells.WordWrap = true; //does not work correctly
			//_viewOfValueCells.Padding = //uses SG.Cells.Views.Cell.DefaultPadding;
		}

		public ParamGrid()
		{
			this.ColumnsCount = 2; //let the programmer set = 1 if need only for flags
			this.MinimumHeight = 18; //height when font is SegoeUI,9. With most other fonts it's smaller.
			this.SpecialKeys = SG.GridSpecialKeys.Default & ~SG.GridSpecialKeys.Tab;

			//this.AutoStretchColumnsToFitWidth = true; //does not work well. Instead we resize in OnClientSizeChanged etc.

			_editor = new SG.Cells.Editors.TextBox(typeof(string));
			_editor.EditableMode = SG.EditableMode.SingleClick | SG.EditableMode.F2Key | SG.EditableMode.AnyKey; //double click -> single click. See also OnMouseDown.
			var c = _editor.Control;
			c.Multiline = true; //let all rows have the same multiline editor, even if the value cannot be multiline

			this.Controller.AddController(new _CellController());
			this.Controller.AddController(SG.Cells.Controllers.Resizable.ResizeWidth); //we resize width and height automatically, but the user may want to resize width. This is like in VS.
			_controllerTooltip0 = new SG.Cells.Controllers.ToolTipText() /*{ IsBalloon = true }*/;
			_controllerTooltip1 = new SG.Cells.Controllers.ToolTipText();
		}

		protected override void CreateHandle()
		{
			base.CreateHandle();

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
		}

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

		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			_AutoSizeLastColumn(); //not ZAutoSizeColumns
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

			//TODO: also call this when the user resizes column 0. Or don't allow to resize, why need it.
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
		public void ZAutoSizeColumns()
		{
			if(this.ColumnsCount > 1 && this.RowsCount > 0) {
				//this.Columns.AutoSizeColumn(0); //no, may be too wide. There is MinimumWidth but no MaximumWidth.
				//int wid = this.Columns.MeasureColumnWidth(0, false, 0, this.RowsCount - 1); //no, it does not work well with col-spanned cells
				int wid = _MeasureColumnWidth(0);
				this.Columns.SetWidth(0, Math.Min(wid, this.ClientSize.Width / 2));
			}
			_AutoSizeLastColumn();
		}

		/// <summary>
		/// Call this after adding all rows.
		/// </summary>
		public void ZAutoSizeRows()
		{
			this.Rows.AutoSize(false);
		}

		class _CellController :SG.Cells.Controllers.ControllerBase
		{
			public override void OnValueChanged(SG.CellContext sender, EventArgs e)
			{
				base.OnValueChanged(sender, e);

				var grid = sender.Grid as ParamGrid;

				var pos = sender.Position;
				if(pos.Column == 1) {
					grid.Rows.AutoSizeRow(pos.Row);
					if(sender.IsEditing()) grid.ZCheck(pos.Row, true); //note: this alone would interfere with the user clicking the checkbox of this row. OnMouseDown prevents it.
				}

				grid.ZOnValueChanged(sender);
			}

			//rejected: start editing on mouse enter. Probably more bad than good.
			//public override void OnMouseEnter(SG.CellContext sender, EventArgs e)
			//{
			//	base.OnMouseEnter(sender, e);

			//	Print(sender.Position, sender.IsEditing());
			//	var pos = sender.Position;
			//	if(pos.Column == 1) {
			//		var grid = sender.Grid as ParamGrid;
			//		//if(!grid.ValueEditor.IsEditing) { //bad
			//			sender.StartEdit();
			//			//var con = (sender.Cell.Editor as SG.Cells.Editors.TextBox).Control;
			//			//con.SelectionLength = 0;
			//		//}
			//	}
			//}

			public override void OnEditStarted(SG.CellContext sender, EventArgs e)
			{
				_ShowEditInfo(sender, true);
				base.OnEditStarted(sender, e);
			}

			public override void OnEditEnded(SG.CellContext sender, EventArgs e)
			{
				_ShowEditInfo(sender, false);
				base.OnEditEnded(sender, e);
			}

			void _ShowEditInfo(SG.CellContext sender, bool show)
			{
				var t = sender.Cell as EditCell;
				if(t?.Info != null) {
					var grid = sender.Grid as ParamGrid;
					grid.ZOnShowEditInfo(sender, show?t.Info:null);
				}
			}

			//public override void OnFocusEntered(SG.CellContext sender, EventArgs e)
			//{
			//	Debug_.PrintFunc();
			//	base.OnFocusEntered(sender, e);
			//}

			//public override void OnFocusLeft(SG.CellContext sender, EventArgs e)
			//{
			//	Debug_.PrintFunc();
			//	base.OnFocusLeft(sender, e);
			//}

			//public override void OnMouseEnter(SG.CellContext sender, EventArgs e)
			//{
			//	Debug_.PrintFunc();
			//	base.OnMouseEnter(sender, e);
			//}

			//public override void OnMouseLeave(SG.CellContext sender, EventArgs e)
			//{
			//	Debug_.PrintFunc();
			//	base.OnMouseLeave(sender, e);
			//}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(_editor.IsEditing) {
				_editor.EditCellContext.EndEdit(cancel: false);
				return;
			}
			base.OnMouseDown(e);
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

		public class EditCell :SG.Cells.Cell
		{
			public EditCell(string value) : base(value, typeof(string)) { }

			public string Info { get; set; }
		}

		/// <summary>
		/// Row type.
		/// </summary>
		public enum RowType
		{
			/// <summary>Checkbox and editable cell.</summary>
			Optional,
			/// <summary>Label and editable cell.</summary>
			Required,
			/// <summary>Only checkbox.</summary>
			Flag,
			/// <summary>Only label.</summary>
			Header,
		}

		int _AddRow(string key, string name, string value, bool check, RowType type, string tt, string info, int insertAt)
		{
			int r = insertAt < 0 ? this.RowsCount : insertAt;
			this.Rows.Insert(r);
			SG.Cells.Cell c;
			if(type == RowType.Required) {
				c = new SG.Cells.Cell(name);
				if(_viewReadonly == null) {
					_viewReadonly = new SG.Cells.Views.Cell() { BackColor = Form.DefaultBackColor }; //default white
				}
				c.View = _viewReadonly;
			} else if(type == RowType.Header) {
				c = new SG.Cells.Cell(name);
				if(_viewHeaderRow == null) {
					_viewHeaderRow = new SG.Cells.Views.Cell() { TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter };
					_viewHeaderRow.Padding = new DevAge.Drawing.Padding(1);
					_viewHeaderRow.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.Silver, Color.WhiteSmoke, 90);
				}
				c.View = _viewHeaderRow;
				c.AddController(SG.Cells.Controllers.Unselectable.Default);
			} else {
				c = new SG.Cells.CheckBox(name, check);

				//TODO: try to add correct acc name, maybe it is possible through model
				//SG.Cells.Models.
				//Print(c.Model.ValueModel);
			}
			//c.AddController(SG.Cells.Controllers.Unselectable.Default); //no, then cannot check/uncheck with keyboard
			if(tt != null) {
				c.AddController(_controllerTooltip0);
				c.ToolTipText=tt;
			}
			this[r, 0] = c;
			int nc = this.ColumnsCount;
			if(nc > 1) {
				if(type== RowType.Flag || type==RowType.Header) {
					c.ColumnSpan = nc;
				} else {
					EditCell t = new EditCell(value) { Editor = _editor, Info=info };
					t.AddController(_controllerTooltip1);
					this[r, 1] = t;
				}
			}

			this.Rows[r].Tag = key ?? name;
			return r;
		}

		#region public add/get/clear functions

		/// <summary>
		/// Adds required parameter.
		/// </summary>
		/// <param name="key">Row Tag property. Used by <see cref="ZGetValue(string, out string, bool)"/>.</param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="tt">Tooltip text.</param>
		/// <param name="info"><see cref="ZShowEditInfo"/> text.</param>
		/// <param name="insertAt"></param>
		public int ZAddRequired(string key, string name, string value = null, string tt = null, string info = null, int insertAt = -1)
		{
			return _AddRow(key, name, value, check: true, type: RowType.Required, tt, info, insertAt);
		}

		/// <summary>
		/// Adds required parameter.
		/// </summary>
		/// <param name="key">Row Tag property. Used by <see cref="ZGetValue(string, out string, bool)"/>.</param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="check"></param>
		/// <param name="tt">Tooltip text.</param>
		/// <param name="info"><see cref="ZShowEditInfo"/> text.</param>
		/// <param name="insertAt"></param>
		public int ZAddOptional(string key, string name, string value = null, bool check = false, string tt = null, string info = null, int insertAt = -1)
		{
			return _AddRow(key, name, value, check, type: RowType.Optional, tt, info, insertAt);
		}

		/// <summary>
		/// Adds required parameter.
		/// </summary>
		/// <param name="key">Row Tag property. Used by <see cref="ZGetValue(string, out string, bool)"/>.</param>
		/// <param name="name"></param>
		/// <param name="check"></param>
		/// <param name="tt">Tooltip text.</param>
		/// <param name="insertAt"></param>
		public int ZAddFlag(string key, string name, bool check = false, string tt = null, int insertAt = -1)
		{
			return _AddRow(key, name, null, check, type: RowType.Flag, tt, null, insertAt);
		}

		/// <summary>
		/// Adds a header row. It is readonly and spans all columns.
		/// </summary>
		public int ZAddHeaderRow(string text, int insertAt = -1)
		{
			return _AddRow(null, text, null, false, type: RowType.Header, null, null, insertAt);
		}

		/// <summary>
		/// Returns true if the row is checked or required.
		/// </summary>
		/// <param name="row">Row index. If negative, asserts and returns false.</param>
		public bool ZIsChecked(int row)
		{
			Debug.Assert(row >= 0); if(row < 0) return false;
			var cb = this[row, 0] as SG.Cells.CheckBox;
			if(cb == null) return this[row, 0].View != _viewHeaderRow; //required
			return cb.Checked.GetValueOrDefault();
		}

		/// <summary>
		/// Returns true if the row is checked or required.
		/// </summary>
		/// <param name="rowKey">Row key. If not found, asserts and returns false.</param>
		public bool ZIsChecked(string rowKey)
		{
			return ZIsChecked(ZFindRow(rowKey));
		}

		/// <summary>
		/// Checks or unchecks.
		/// Use only for flags and optionals, not for required.
		/// </summary>
		/// <param name="row">Row index. If negative, asserts and returns.</param>
		/// <param name="check"></param>
		public void ZCheck(int row, bool check)
		{
			Debug.Assert(row >= 0); if(row < 0) return;
			var cb = this[row, 0] as SG.Cells.CheckBox;
			Debug.Assert(cb != null);
			cb.Checked = check;
		}

		/// <summary>
		/// Checks or unchecks.
		/// Use only for flags and optionals, not for required.
		/// </summary>
		/// <param name="rowKey">Row key. If not found, asserts and returns.</param>
		/// <param name="check"></param>
		public void ZCheck(string rowKey, bool check)
		{
			ZCheck(ZFindRow(rowKey), check);
		}

		/// <summary>
		/// If the row is checked or required, gets its value and returns true.
		/// </summary>
		/// <param name="row">Row index. If negative, asserts and returns false.</param>
		/// <param name="value"></param>
		/// <param name="falseIfEmpty">Return false if the value is empty (null).</param>
		public bool ZGetValue(int row, out string value, bool falseIfEmpty)
		{
			value = null;
			Debug.Assert(row >= 0); if(row < 0) return false;
			if(!ZIsChecked(row)) return false;
			value = ZGetCellText(row, 1);
			if(falseIfEmpty && value == null) return false;
			return true;
		}

		/// <summary>
		/// If the row is checked or required, gets its value and returns true.
		/// </summary>
		/// <param name="rowKey">Row key. If not found, asserts returns false.</param>
		/// <param name="value"></param>
		/// <param name="falseIfEmpty">Return false if the value is empty (null).</param>
		public bool ZGetValue(string rowKey, out string value, bool falseIfEmpty)
		{
			return ZGetValue(ZFindRow(rowKey), out value, falseIfEmpty);
		}

		/// <summary>
		/// If the row exists and is checked or required, gets its value and returns true.
		/// </summary>
		/// <param name="rowKey">Row key. If not found, returns false.</param>
		/// <param name="value"></param>
		/// <param name="falseIfEmpty">Return false if the value is empty (null).</param>
		public bool ZGetValueIfExists(string rowKey, out string value, bool falseIfEmpty)
		{
			int row = ZFindRow(rowKey);
			if(row < 0) { value = null; return false; }
			return ZGetValue(row, out value, falseIfEmpty);
		}

		/// <summary>
		/// Gets cell value or checkbox label.
		/// </summary>
		/// <param name="row">Row index. If negative, asserts and returns null.</param>
		/// <param name="column">Column index.</param>
		public string ZGetCellText(int row, int column)
		{
			Debug.Assert(row >= 0); if(row < 0) return null;
			var c = this[row, column];
			if(column == 0 && c is SG.Cells.CheckBox cb) return cb.Caption;
			return c.Value as string;
		}

		/// <summary>
		/// Gets cell value or checkbox label.
		/// </summary>
		/// <param name="rowKey">Row key. If not found, asserts and returns null.</param>
		/// <param name="column">Column index.</param>
		public string ZGetCellText(string rowKey, int column)
		{
			return ZGetCellText(ZFindRow(rowKey), column);
		}

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
		/// Gets row key.
		/// </summary>
		public string ZGetRowKey(int row) => this.Rows[row].Tag as string;

		/// <summary>
		/// Removes all rows.
		/// </summary>
		public void Clear()
		{
			//this.Rows.Clear(); //makes editors invalid
			if(_editor.IsEditing) _editor.EditCellContext.EndEdit(true);
			this.RowsCount = 0;
		}

		#endregion public add/get/clear functions
	}
}
