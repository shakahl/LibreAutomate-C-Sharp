using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;
using static Program;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

partial class ThisIsNotAFormFile { }

//[DebuggerStepThrough]
partial class PanelFiles :Control
{
	//idea: when file clicked, open it and show CatMenu of its functions (if > 1).

	TreeViewAdv _c;
	FilesModel _model;

	public PanelFiles()
	{
		//var p = new Perf.Inst(true);
		_c = new TreeViewAdv();
		//_c.SuspendLayout();
		//p.Next();
		_c.BorderStyle = BorderStyle.None;
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = this.Name = "Files";
		//_c.Font = MainForm.Font;

		//p.Next();
		//_c.AutoRowHeight = false; _c.RowHeight = 25; //RowHeight ignored if AutoRowHeight true
		//p.Next();
		_c.BackgroundImageLayout = ImageLayout.Center;
		//_c.DefaultToolTipProvider = null;
		//_c.GridLineStyle = GridLineStyle.Vertical;
		//_c.LineColor = SystemColors.ControlDark; //default
		//_c.LoadOnDemand = false;
		_c.LoadOnDemand = true; //saves 2 ms for 1000 items
								//_c.SelectionMode = TreeSelectionMode.Multi; //creates problems
		_c.SelectionMode = TreeSelectionMode.MultiSameParent;
		//_c.SelectionMode = TreeSelectionMode.Single;
		_c.ShowNodeToolTips = true;
		_c.FullRowSelect = true;
		//_c.HideSelection = true;
		//_c.ShowPlusMinus = false; _c.ShowLines = false;
		//_c.ShiftFirstNode = true;
		//_c.Indent /= 2;
		//_c.ShowLines = false;
		//_c.BackColor = Color.DarkGreen;
		//_c.ForeColor = Color.Yellow;
		//_c.LineColor = Color.Red;
		_c.AllowDrop = true;
		_c.DisplayDraggingNodes = true;

		//p.Next();
#if TEST_MANY_COLUMNS
		_c.GridLineStyle = GridLineStyle.HorizontalAndVertical;
		_c.AllowColumnReorder = true;
#endif
		_AddColumns();
		//p.Next();

		//_c.ResumeLayout(false);
		this.Controls.Add(_c);
		//p.NW();
	}

	protected override void Dispose(bool disposing)
	{
		//PrintFunc();
		_model?.SaveCollectionNowIfDirty();
		_model?.Dispose();
		base.Dispose(disposing);
	}

	public TreeViewAdv TV { get => _c; }

	public FilesModel Model { get => _model; }

	#region columns and node controls

#if TEST_MANY_COLUMNS
			TreeColumn _columnName;
			TreeColumn _columnGUID;
			TreeColumn _columnCombo;
			TreeColumn _columnComboEnum;
			TreeColumn _columnDecimal;
			TreeColumn _columnInteger;
			TreeColumn _columnUpDown;
#endif

	//use prefix _cc for "column control"
	NodeIcon _ccIcon;
	NodeTextBox _ccName;
#if TEST_MANY_COLUMNS
			NodeCheckBox _ccCheck;
			NodeTextBox _ccGUID;
			NodeComboBox _ccCombo;
			NodeComboBox _ccComboEnum;
			NodeDecimalTextBox _ccDecimal;
			NodeIntegerTextBox _ccInteger;
			NodeNumericUpDown _ccUpDown;
#endif

	void _AddColumns()
	{
#if TEST_MANY_COLUMNS
				_c.UseColumns = true;

				_columnName = new TreeColumn("Name", 200);
				_c.Columns.Add(_columnName);
#endif

		_ccIcon = new NodeIcon();
		_c.NodeControls.Add(_ccIcon);
		_ccIcon.LeftMargin = 0;
		_ccIcon.ScaleMode = ImageScaleMode.ScaleUp;
		_ccIcon.DpiStretch = true;

		_ccName = new NodeTextBox();
		_c.NodeControls.Add(_ccName);
		_ccName.EditEnabled = true;
		_ccName.LeftMargin = 0;

#if TEST_MANY_COLUMNS
				_ccIcon.ParentColumn = _columnName;
				_ccName.ParentColumn = _columnName;


				_columnGUID = new TreeColumn("GUID", 100);
				//_columnGUID.IsVisible = false;
				//_columnGUID.MinColumnWidth = 25; _columnGUID.MaxColumnWidth = 200;
				//_columnGUID.TextAlign = HorizontalAlignment.Center; //header text
				//_columnGUID.TooltipText = "column tooltip";
				//_columnGUID.Sortable = true;
				//_columnGUID.SortOrder = SortOrder.Ascending;
				_c.Columns.Add(_columnGUID);

				_ccCheck = new NodeCheckBox();
				_c.NodeControls.Add(_ccCheck);
				_ccCheck.DataPropertyName = "Checked";
				_ccCheck.EditEnabled = true;
				_ccCheck.LeftMargin = 2;
				_ccCheck.ParentColumn = _columnGUID;
				//_nodeCheckBox.ThreeState = true;
				//_nodeCheckBox.CheckStateChanged += _nodeCheckBox_CheckStateChanged;

				_ccGUID = new NodeTextBox();
				_c.NodeControls.Add(_ccGUID);
				_ccGUID.ParentColumn = _columnGUID;
				_ccGUID.DataPropertyName = "GUID";
				//_controlGUID.LeftMargin = 3; //default
				_ccGUID.EditEnabled = true; _ccGUID.EditOnClick = true;
				//_ccGUID.TrimMultiLine = true;
				_ccGUID.MultilineEdit = true;
				//_ccGUID.Font = new Font("Courier New", 8);
				//_c.Font = new Font("Courier New", 8);

				_columnCombo = new TreeColumn("Combo", 100);
				//_columnCombo.IsVisible = false;
				_c.Columns.Add(_columnCombo);
				_columnComboEnum = new TreeColumn("Combo Enum", 100);
				_c.Columns.Add(_columnComboEnum);
				_columnDecimal = new TreeColumn("Decimal", 60);
				_c.Columns.Add(_columnDecimal);
				_columnInteger = new TreeColumn("Integer", 60);
				_c.Columns.Add(_columnInteger);
				_columnUpDown = new TreeColumn("UpDown", 100);
				_c.Columns.Add(_columnUpDown);

				_ccCombo = new NodeComboBox();
				_c.NodeControls.Add(_ccCombo);
				_ccCombo.ParentColumn = _columnCombo;
				_ccCombo.DataPropertyName = "Combo";
				_ccCombo.EditEnabled = true; _ccCombo.EditOnClick = true;
				_ccCombo.EditableCombo = true;
				_ccCombo.TextAlign = HorizontalAlignment.Right;
				var dd = _ccCombo.DropDownItems;
				dd.Add("one"); dd.Add(2); dd.Add(3.5); dd.Add(4); dd.Add(5); dd.Add(6); dd.Add(7); dd.Add(8); dd.Add(9); dd.Add(10);
				dd.Add("one"); dd.Add(2); dd.Add(3.5); dd.Add(4); dd.Add(5); dd.Add(6); dd.Add(7); dd.Add(8); dd.Add(9); dd.Add(10);
				//_ccCombo.CreatingEditor += _ccCombo_CreatingEditor;

				_ccComboEnum = new NodeComboBox();
				_c.NodeControls.Add(_ccComboEnum);
				_ccComboEnum.ParentColumn = _columnComboEnum;
				_ccComboEnum.DataPropertyName = "ComboEnum";
				_ccComboEnum.EditEnabled = true; _ccComboEnum.EditOnClick = true;
				//_ccComboEnum.CreatingEditor += (unu, sed) => Print("ce");

				_ccDecimal = new NodeDecimalTextBox();
				_c.NodeControls.Add(_ccDecimal);
				_ccDecimal.ParentColumn = _columnDecimal;
				_ccDecimal.DataPropertyName = "Decimal";
				_ccDecimal.EditEnabled = true; _ccDecimal.EditOnClick = true;

				_ccInteger = new NodeIntegerTextBox();
				_c.NodeControls.Add(_ccInteger);
				_ccInteger.ParentColumn = _columnInteger;
				_ccInteger.DataPropertyName = "Integer";
				_ccInteger.EditEnabled = true; _ccInteger.EditOnClick = true;

				_ccUpDown = new NodeNumericUpDown();
				_c.NodeControls.Add(_ccUpDown);
				_ccUpDown.ParentColumn = _columnUpDown;
				_ccUpDown.DataPropertyName = "UpDown";
				_ccUpDown.EditEnabled = true; _ccUpDown.EditOnClick = true;
#endif

		foreach(var tc in _c.NodeControls.OfType<BaseTextControl>()) {
			tc.Trimming = StringTrimming.EllipsisCharacter; //add "..." if too long. But does not work if multiline.
		}
	}

#if TEST_MANY_COLUMNS
			private void _ccCombo_CreatingEditor(object sender, EditEventArgs e)
			{
				//Print(e.Control);
				var c = e.Control as ComboBox;
				var dd = c.Items; dd.Add("one"); dd.Add(2); dd.Add(3.5); dd.Add(4); dd.Add(5); dd.Add(6); dd.Add(7); dd.Add(8); dd.Add(9); dd.Add(10);
			}
			//private void _nodeCheckBox_CheckStateChanged(object sender, TreeViewAdvEventArgs e)
			//{
			//	Print(node);
			//}
#endif

	#endregion

	/// <summary>
	/// Loads existing or new collection.
	/// If fails, shows a task dialog with several choices - retry, load another, create new, cancel.
	/// Sets Model and Text properties of the main form. Updates recent files.
	/// </summary>
	/// <param name="file">
	/// Collection's XML file.
	/// If null, loads the last used file from settings.
	/// If the file or the setting does not exist, copies from Folders.ThisAppDocuments + @"Main\Main.xml".
	/// </param>
	public FilesModel LoadCollection(string file = null)
	{
		if(file == null) file = Settings.Get("collection");
		if(Empty(file)) file = Folders.ThisAppDocuments + @"Main\Main.xml";
		g1:
		try {
			var collDir = Path_.GetDirectoryPath(file);
			if(!Files.ExistsAsFile(file)) {
				Files.Copy(Folders.ThisApp + @"Default\Main.xml", file);
				Files.CopyTo(Folders.ThisApp + @"Default\files", collDir);
			}

			if(_model != null) {
				_model.SaveCollectionNowIfDirty();
				_model.SetCurrentFile(null);
			}

			var oldModel = _model;
			var m = new FilesModel(_c, file);
			m.InitNodeControls(_ccIcon, _ccName);
			_c.Model = _model = m;
			//_c.SelectedNode = null;
			oldModel?.Dispose();

			//CONSIDER: unexpand path
			if(Settings.Set("collection", file)) {
				//add to recent
				var x1 = Settings.XmlOf("recent", true);
				var x2 = x1.Element_("f", "n", file, true);
				if(x2 != null && x2 != x1.FirstNode) { x2.Remove(); x2 = null; }
				if(x2 == null) x1.AddFirst(new XElement("f", new XAttribute("n", file)));
			}

			MainForm.Model = _model;
			MainForm.Text = "Catkeys - " + collDir;
		}
		catch(Exception ex) {
			FilesModel m = null;
			//Print($"Failed to load '{file}'. {ex.Message}");
			switch(TaskDialog.ShowError("Failed to load collection", file,
				"1 Retry|2 Load another|3 Create new|0 Cancel",
				owner: this, expandedText: ex.Message)) {
			case 1: goto g1;
			case 2: m = LoadAnotherCollection(); break;
			case 3: m = LoadNewCollection(); break;
			}
			if(m != null) return m;
			if(_model == null) Environment.Exit(1);
		}
		return _model;
	}

	/// <summary>
	/// Shows "Open" dialog to select an existing collection.
	/// On OK loads the selected collection and returns FilesModel. On Cancel return null.
	/// </summary>
	public FilesModel LoadAnotherCollection()
	{
		var d = new OpenFileDialog() { Title = "Open collection" };
		d.DefaultExt = ".xml";
		d.Filter = "Collection files (*.xml)|*.xml";
		if(d.ShowDialog(this) != DialogResult.OK) return null;
		return LoadCollection(d.FileName);
	}

	/// <summary>
	/// Shows dialog to create new collection.
	/// On OK creates new collection and returns FilesModel. On Cancel return null.
	/// </summary>
	public FilesModel LoadNewCollection()
	{
		var file = _model.GetFilePathForNewCollection();
		if(file == null) return null;
		return LoadCollection(file);
	}

	void _AddToRecentCollections(string file)
	{
		var x1 = Settings.XmlOf("recent", true);
		var x2 = x1.Element_("f", "n", file, true);
		if(x2 != null && x2 != x1.FirstNode) { x2.Remove(); x2 = null; }
		if(x2 == null) x1.AddFirst(new XElement("f", new XAttribute("n", file)));
	}

	/// <summary>
	/// Fills submenu File -> Collection -> Recent.
	/// </summary>
	public void FillMenuRecentCollections(ToolStripDropDownMenu dd)
	{
		var x1 = Settings.XmlOf("recent");
		if(x1 == null) return;
		var current = Settings.Get("collection");
		dd.SuspendLayout();
		dd.Items.Clear();
		bool currentOK = false;
		var aRem = new List<XElement>();
		foreach(var x2 in x1.Elements("f")) {
			var path = x2.Attribute_("n");
			if(dd.Items.Count == 20 || !Files.ExistsAsFile(path)) {
				aRem.Add(x2);
				continue;
			}
			var mi = dd.Items.Add(path, null, (o, u) => LoadCollection(o.ToString()));
			if(!currentOK && (path == current)) {
				currentOK = true;
				mi.Font = Stock.FontBold;
			}
		}
		dd.ResumeLayout();
		if(aRem.Count > 0) {
			foreach(var v in aRem) v.Remove();
		}
	}
}
