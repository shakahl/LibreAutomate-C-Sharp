using System.Windows.Forms;

namespace Au.Tools
{
	partial class Form_Acc
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this._grid = new Au.Controls.ParamGrid();
			this._grid2 = new Au.Controls.ParamGrid();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this._cCapture = new System.Windows.Forms.CheckBox();
			this._bOK = new Au.Controls.ButtonOK();
			this._bCancel = new Au.Controls.ButtonCancel();
			this._bTest = new System.Windows.Forms.Button();
			this._lSpeed = new System.Windows.Forms.Label();
			this._info = new Au.Controls.AuInfoBox();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._bEtc = new System.Windows.Forms.Button();
			this._code = new Au.Tools.CodeBox();
			this._tree = new Aga.Controls.Tree.TreeViewAdv();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(8, 104);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer1.Size = new System.Drawing.Size(600, 440);
			this.splitContainer1.SplitterDistance = 170;
			this.splitContainer1.SplitterWidth = 8;
			this.splitContainer1.TabIndex = 5;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this._grid);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this._grid2);
			this.splitContainer2.Size = new System.Drawing.Size(600, 170);
			this.splitContainer2.SplitterDistance = 398;
			this.splitContainer2.SplitterWidth = 8;
			this.splitContainer2.TabIndex = 0;
			// 
			// _grid
			// 
			this._grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._grid.ClipboardMode = SourceGrid.ClipboardMode.Copy;
			this._grid.ColumnsCount = 2;
			this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._grid.EnableSort = false;
			this._grid.Location = new System.Drawing.Point(0, 0);
			this._grid.MinimumHeight = 18;
			this._grid.Name = "_grid";
			this._grid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
			this._grid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
			this._grid.Size = new System.Drawing.Size(398, 170);
			this._grid.SpecialKeys = ((SourceGrid.GridSpecialKeys)((((((SourceGrid.GridSpecialKeys.Arrows | SourceGrid.GridSpecialKeys.PageDownUp) 
            | SourceGrid.GridSpecialKeys.Enter) 
            | SourceGrid.GridSpecialKeys.Escape) 
            | SourceGrid.GridSpecialKeys.Control) 
            | SourceGrid.GridSpecialKeys.Shift)));
			this._grid.TabIndex = 0;
			this._grid.TabStop = true;
			this._grid.ToolTipText = "";
			this._grid.ZAddHidden = false;
			// 
			// _grid2
			// 
			this._grid2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._grid2.ClipboardMode = SourceGrid.ClipboardMode.Copy;
			this._grid2.ColumnsCount = 2;
			this._grid2.Dock = System.Windows.Forms.DockStyle.Fill;
			this._grid2.EnableSort = false;
			this._grid2.Location = new System.Drawing.Point(0, 0);
			this._grid2.MinimumHeight = 18;
			this._grid2.Name = "_grid2";
			this._grid2.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
			this._grid2.SelectionMode = SourceGrid.GridSelectionMode.Cell;
			this._grid2.Size = new System.Drawing.Size(194, 170);
			this._grid2.SpecialKeys = ((SourceGrid.GridSpecialKeys)((((((SourceGrid.GridSpecialKeys.Arrows | SourceGrid.GridSpecialKeys.PageDownUp) 
            | SourceGrid.GridSpecialKeys.Enter) 
            | SourceGrid.GridSpecialKeys.Escape) 
            | SourceGrid.GridSpecialKeys.Control) 
            | SourceGrid.GridSpecialKeys.Shift)));
			this._grid2.TabIndex = 0;
			this._grid2.TabStop = true;
			this._grid2.ToolTipText = "";
			this._grid2.ZAddHidden = false;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this._code);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this._tree);
			this.splitContainer3.Size = new System.Drawing.Size(600, 262);
			this.splitContainer3.SplitterDistance = 59;
			this.splitContainer3.SplitterWidth = 8;
			this.splitContainer3.TabIndex = 0;
			// 
			// _cCapture
			// 
			this._cCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cCapture.Location = new System.Drawing.Point(536, 75);
			this._cCapture.Name = "_cCapture";
			this._cCapture.Size = new System.Drawing.Size(80, 19);
			this._cCapture.TabIndex = 0;
			this._cCapture.Text = "&Capture";
			this._toolTip.SetToolTip(this._cCapture, "Enables key F3. Shows AO rectangles when moving the mouse.");
			this._cCapture.UseVisualStyleBackColor = true;
			this._cCapture.CheckedChanged += new System.EventHandler(this._cCapture_CheckedChanged);
			// 
			// _bOK
			// 
			this._bOK.Enabled = false;
			this._bOK.Location = new System.Drawing.Point(184, 72);
			this._bOK.Name = "_bOK";
			this._bOK.Size = new System.Drawing.Size(72, 24);
			this._bOK.TabIndex = 2;
			this._bOK.UseVisualStyleBackColor = true;
			this._bOK.Click += new System.EventHandler(this._bOK_Click);
			// 
			// _bCancel
			// 
			this._bCancel.Location = new System.Drawing.Point(264, 72);
			this._bCancel.Name = "_bCancel";
			this._bCancel.Size = new System.Drawing.Size(72, 24);
			this._bCancel.TabIndex = 3;
			this._bCancel.UseVisualStyleBackColor = true;
			// 
			// _bTest
			// 
			this._bTest.Enabled = false;
			this._bTest.Location = new System.Drawing.Point(8, 72);
			this._bTest.Name = "_bTest";
			this._bTest.Size = new System.Drawing.Size(72, 24);
			this._bTest.TabIndex = 1;
			this._bTest.Text = "&Test";
			this._toolTip.SetToolTip(this._bTest, "Executes the \'find\' code (without wait, etc). If AO found, shows its rectangle.");
			this._bTest.UseVisualStyleBackColor = true;
			this._bTest.Click += new System.EventHandler(this._bTest_Click);
			// 
			// _lSpeed
			// 
			this._lSpeed.Location = new System.Drawing.Point(87, 77);
			this._lSpeed.Name = "_lSpeed";
			this._lSpeed.Size = new System.Drawing.Size(97, 15);
			this._lSpeed.TabIndex = 0;
			this._toolTip.SetToolTip(this._lSpeed, "Shows the Test execution time. Red if not found.");
			// 
			// _info
			// 
			this._info.AccessibleName = "_info";
			this._info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._info.DisableModifiedNotifications = false;
			this._info.Location = new System.Drawing.Point(8, 8);
			this._info.Name = "_info";
			this._info.Size = new System.Drawing.Size(600, 56);
			this._info.TabIndex = 0;
			// 
			// _bEtc
			// 
			this._bEtc.Enabled = false;
			this._bEtc.Location = new System.Drawing.Point(344, 72);
			this._bEtc.Name = "_bEtc";
			this._bEtc.Size = new System.Drawing.Size(32, 24);
			this._bEtc.TabIndex = 4;
			this._bEtc.Text = "&...";
			this._bEtc.UseVisualStyleBackColor = true;
			this._bEtc.Click += new System.EventHandler(this._bEtc_Click);
			// 
			// _code
			// 
			this._code.AccessibleName = "_code";
			this._code.DisableModifiedNotifications = false;
			this._code.Dock = System.Windows.Forms.DockStyle.Fill;
			this._code.Location = new System.Drawing.Point(0, 0);
			this._code.Name = "_code";
			this._code.Size = new System.Drawing.Size(600, 59);
			this._code.TabIndex = 4;
			// 
			// _tree
			// 
			this._tree.BackColor = System.Drawing.SystemColors.Window;
			this._tree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._tree.DefaultToolTipProvider = null;
			this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tree.DragDropMarkColor = System.Drawing.Color.MidnightBlue;
			this._tree.LineColor = System.Drawing.SystemColors.ControlDark;
			this._tree.Location = new System.Drawing.Point(0, 0);
			this._tree.Model = null;
			this._tree.Name = "_tree";
			this._tree.SelectedNode = null;
			this._tree.Size = new System.Drawing.Size(600, 195);
			this._tree.TabIndex = 0;
			// 
			// Form_Acc
			// 
			this.AcceptButton = this._bOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._bCancel;
			this.ClientSize = new System.Drawing.Size(616, 551);
			this.Controls.Add(this._bEtc);
			this.Controls.Add(this._info);
			this.Controls.Add(this._lSpeed);
			this.Controls.Add(this._bTest);
			this.Controls.Add(this._bCancel);
			this.Controls.Add(this._bOK);
			this.Controls.Add(this._cCapture);
			this.Controls.Add(this.splitContainer1);
			this.IsPopup = true;
			this.MinimumSize = new System.Drawing.Size(500, 400);
			this.Name = "Form_Acc";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Find accessible object";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private CheckBox _cCapture;
		private Button _bTest;
		private Label _lSpeed;
		private Au.Controls.ButtonOK _bOK;
		private Au.Controls.ButtonCancel _bCancel;
		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;
		private Au.Controls.ParamGrid _grid;
		private Au.Controls.ParamGrid _grid2;
		private Aga.Controls.Tree.TreeViewAdv _tree;
		private Controls.AuInfoBox _info;
		private ToolTip _toolTip;
		private CodeBox _code;
		private SplitContainer splitContainer3;
		private Button _bEtc;
	}
}
