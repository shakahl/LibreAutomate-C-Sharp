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
			this._tWnd = new System.Windows.Forms.TextBox();
			this._tree = new Aga.Controls.Tree.TreeViewAdv();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this._grid = new Au.Controls.ParamGrid();
			this._grid2 = new Au.Controls.ParamGrid();
			this._cCapture = new System.Windows.Forms.CheckBox();
			this._tAcc = new System.Windows.Forms.TextBox();
			this._bOK = new System.Windows.Forms.Button();
			this._bCancel = new System.Windows.Forms.Button();
			this._bTest = new System.Windows.Forms.Button();
			this._lSpeed = new System.Windows.Forms.Label();
			this._bCopy = new System.Windows.Forms.Button();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _tWnd
			// 
			this._tWnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this._tWnd.Location = new System.Drawing.Point(8, 8);
			this._tWnd.Name = "_tWnd";
			this._tWnd.Size = new System.Drawing.Size(603, 23);
			this._tWnd.TabIndex = 1;
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
			this._tree.Size = new System.Drawing.Size(600, 306);
			this._tree.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(8, 96);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._tree);
			this.splitContainer1.Size = new System.Drawing.Size(600, 500);
			this.splitContainer1.SplitterDistance = 170;
			this.splitContainer1.SplitterWidth = 8;
			this.splitContainer1.TabIndex = 0;
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
			this.splitContainer2.Size = new System.Drawing.Size(600, 186);
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
			this._grid.Size = new System.Drawing.Size(398, 186);
			this._grid.SpecialKeys = ((SourceGrid.GridSpecialKeys)((((((SourceGrid.GridSpecialKeys.Arrows | SourceGrid.GridSpecialKeys.PageDownUp)
			| SourceGrid.GridSpecialKeys.Enter)
			| SourceGrid.GridSpecialKeys.Escape)
			| SourceGrid.GridSpecialKeys.Control)
			| SourceGrid.GridSpecialKeys.Shift)));
			this._grid.TabIndex = 0;
			this._grid.TabStop = true;
			this._grid.ToolTipText = "";
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
			this._grid2.Size = new System.Drawing.Size(194, 186);
			this._grid2.SpecialKeys = ((SourceGrid.GridSpecialKeys)((((((SourceGrid.GridSpecialKeys.Arrows | SourceGrid.GridSpecialKeys.PageDownUp)
			| SourceGrid.GridSpecialKeys.Enter)
			| SourceGrid.GridSpecialKeys.Escape)
			| SourceGrid.GridSpecialKeys.Control)
			| SourceGrid.GridSpecialKeys.Shift)));
			this._grid2.TabIndex = 0;
			this._grid2.TabStop = true;
			this._grid2.ToolTipText = "";
			// 
			// _cCapture
			// 
			this._cCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cCapture.Location = new System.Drawing.Point(520, 67);
			this._cCapture.Name = "_cCapture";
			this._cCapture.Size = new System.Drawing.Size(96, 19);
			this._cCapture.TabIndex = 3;
			this._cCapture.Text = "C&apture, F3";
			this._cCapture.UseVisualStyleBackColor = true;
			this._cCapture.CheckedChanged += new System.EventHandler(this._cCapture_CheckedChanged);
			// 
			// _tAcc
			// 
			this._tAcc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this._tAcc.Location = new System.Drawing.Point(8, 32);
			this._tAcc.Name = "_tAcc";
			this._tAcc.ReadOnly = true;
			this._tAcc.Size = new System.Drawing.Size(603, 23);
			this._tAcc.TabIndex = 2;
			// 
			// _bOK
			// 
			this._bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._bOK.Location = new System.Drawing.Point(192, 64);
			this._bOK.Name = "_bOK";
			this._bOK.Size = new System.Drawing.Size(72, 24);
			this._bOK.TabIndex = 6;
			this._bOK.Text = "&OK";
			this._bOK.UseVisualStyleBackColor = true;
			this._bOK.Click += new System.EventHandler(this._bOK_Click);
			// 
			// _bCancel
			// 
			this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._bCancel.Location = new System.Drawing.Point(272, 64);
			this._bCancel.Name = "_bCancel";
			this._bCancel.Size = new System.Drawing.Size(72, 24);
			this._bCancel.TabIndex = 5;
			this._bCancel.Text = "&Cancel";
			this._bCancel.UseVisualStyleBackColor = true;
			// 
			// _bTest
			// 
			this._bTest.Location = new System.Drawing.Point(8, 64);
			this._bTest.Name = "_bTest";
			this._bTest.Size = new System.Drawing.Size(72, 24);
			this._bTest.TabIndex = 7;
			this._bTest.Text = "&Test";
			this._bTest.UseVisualStyleBackColor = true;
			this._bTest.Click += new System.EventHandler(this._bTest_Click);
			// 
			// _lSpeed
			// 
			this._lSpeed.Location = new System.Drawing.Point(87, 69);
			this._lSpeed.Name = "_lSpeed";
			this._lSpeed.Size = new System.Drawing.Size(100, 15);
			this._lSpeed.TabIndex = 0;
			// 
			// _bCopy
			// 
			this._bCopy.Location = new System.Drawing.Point(352, 64);
			this._bCopy.Name = "_bCopy";
			this._bCopy.Size = new System.Drawing.Size(72, 24);
			this._bCopy.TabIndex = 4;
			this._bCopy.Text = "Co&py";
			this._bCopy.UseVisualStyleBackColor = true;
			this._bCopy.Click += new System.EventHandler(this._bCopy_Click);
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// Acc_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(616, 604);
			this.Controls.Add(this._bCopy);
			this.Controls.Add(this._lSpeed);
			this.Controls.Add(this._bTest);
			this.Controls.Add(this._bCancel);
			this.Controls.Add(this._bOK);
			this.Controls.Add(this._cCapture);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this._tAcc);
			this.Controls.Add(this._tWnd);
			this.MinimumSize = new System.Drawing.Size(550, 330);
			this.Name = "Acc_form";
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
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private TextBox _tWnd;
		private TextBox _tAcc;
		private CheckBox _cCapture;
		private Button _bTest;
		private Label _lSpeed;
		private Button _bOK;
		private Button _bCancel;
		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;
		private Au.Controls.ParamGrid _grid;
		private Au.Controls.ParamGrid _grid2;
		private Aga.Controls.Tree.TreeViewAdv _tree;
		private Button _bCopy;
		private ErrorProvider _errorProvider;
	}
}
