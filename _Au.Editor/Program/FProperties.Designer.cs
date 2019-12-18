partial class FProperties
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
		this._bOK = new Au.Controls.AuButtonOK();
		this._bCancel = new Au.Controls.AuButtonCancel();
		this._info = new Au.Controls.InfoBox();
		this._grid = new Au.Controls.ParamGrid();
		this.label4 = new System.Windows.Forms.Label();
		this._tFindInList = new System.Windows.Forms.TextBox();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this._bAddComBrowse = new Au.Controls.AuButton();
		this._bAddNet = new Au.Controls.AuButton();
		this._bAddLibraryProject = new Au.Controls.AuButton();
		this._bAddComRegistry = new Au.Controls.AuButton();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this._bAddClass = new Au.Controls.AuButton();
		this._bAddResource = new Au.Controls.AuButton();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.SuspendLayout();
		// 
		// _bOK
		// 
		this._bOK.Location = new System.Drawing.Point(472, 404);
		this._bOK.Name = "_bOK";
		this._bOK.Size = new System.Drawing.Size(72, 24);
		this._bOK.TabIndex = 0;
		this._bOK.Click += new System.EventHandler(this._bOK_Click);
		// 
		// _bCancel
		// 
		this._bCancel.Location = new System.Drawing.Point(552, 404);
		this._bCancel.Name = "_bCancel";
		this._bCancel.Size = new System.Drawing.Size(72, 24);
		this._bCancel.TabIndex = 1;
		// 
		// _info
		// 
		this._info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		| System.Windows.Forms.AnchorStyles.Right)));
		this._info.Location = new System.Drawing.Point(8, 8);
		this._info.Name = "_info";
		this._info.Size = new System.Drawing.Size(624, 80);
		this._info.TabIndex = 7;
		// 
		// _grid
		// 
		this._grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this._grid.ClipboardMode = SourceGrid.ClipboardMode.Copy;
		this._grid.ColumnsCount = 2;
		this._grid.EnableSort = false;
		this._grid.Location = new System.Drawing.Point(8, 100);
		this._grid.MinimumHeight = 18;
		this._grid.Name = "_grid";
		this._grid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
		this._grid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
		this._grid.Size = new System.Drawing.Size(296, 328);
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
		// groupBox1
		// 
		this.groupBox1.Controls.Add(this._bAddComBrowse);
		this.groupBox1.Controls.Add(this._bAddNet);
		this.groupBox1.Controls.Add(this._bAddLibraryProject);
		this.groupBox1.Controls.Add(this._bAddComRegistry);
		this.groupBox1.Location = new System.Drawing.Point(320, 100);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(126, 140);
		this.groupBox1.TabIndex = 7;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Add reference";
		// 
		// _bAddNet
		// 
		this._bAddNet.Location = new System.Drawing.Point(8, 24);
		this._bAddNet.Name = "_bAddNet";
		this._bAddNet.Size = new System.Drawing.Size(110, 24);
		this._bAddNet.TabIndex = 1;
		this._bAddNet.Text = "Assembly...";
		this._bAddNet.Click += new System.EventHandler(this._bAddNet_Click);
		// 
		// _bAddComRegistry
		// 
		this._bAddComRegistry.Location = new System.Drawing.Point(8, 52);
		this._bAddComRegistry.Name = "_bAddComRegistry";
		this._bAddComRegistry.Size = new System.Drawing.Size(68, 24);
		this._bAddComRegistry.TabIndex = 1;
		this._bAddComRegistry.Text = "COM";
		this._bAddComRegistry.Click += new System.EventHandler(this._bAddComRegistry_Click);
		// 
		// _bAddComBrowse
		// 
		this._bAddComBrowse.Location = new System.Drawing.Point(78, 52);
		this._bAddComBrowse.Name = "_bAddComBrowse";
		this._bAddComBrowse.Size = new System.Drawing.Size(40, 24);
		this._bAddComBrowse.TabIndex = 8;
		this._bAddComBrowse.Text = "...";
		this._bAddComBrowse.Click += new System.EventHandler(this._bAddComBrowse_Click);
		// 
		// _bAddLibraryProject
		// 
		this._bAddLibraryProject.Location = new System.Drawing.Point(8, 80);
		this._bAddLibraryProject.Name = "_bAddLibraryProject";
		this._bAddLibraryProject.Size = new System.Drawing.Size(110, 24);
		this._bAddLibraryProject.TabIndex = 1;
		this._bAddLibraryProject.Text = "Library project";
		this._bAddLibraryProject.Click += new System.EventHandler(this._bAddLibraryProject_Click);
		// 
		// groupBox2
		// 
		this.groupBox2.Controls.Add(this._bAddClass);
		this.groupBox2.Controls.Add(this._bAddResource);
		this.groupBox2.Location = new System.Drawing.Point(536, 100);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(96, 84);
		this.groupBox2.TabIndex = 6;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Add file";
		// 
		// _bAddClass
		// 
		this._bAddClass.Location = new System.Drawing.Point(8, 24);
		this._bAddClass.Name = "_bAddClass";
		this._bAddClass.Size = new System.Drawing.Size(80, 24);
		this._bAddClass.TabIndex = 1;
		this._bAddClass.Text = "Class file...";
		this._bAddClass.Click += new System.EventHandler(this._bAddClass_Click);
		// 
		// _bAddResource
		// 
		this._bAddResource.Location = new System.Drawing.Point(8, 52);
		this._bAddResource.Name = "_bAddResource";
		this._bAddResource.Size = new System.Drawing.Size(80, 24);
		this._bAddResource.TabIndex = 1;
		this._bAddResource.Text = "Resource...";
		this._bAddResource.Click += new System.EventHandler(this._bAddResource_Click);
		// 
		// label4
		// 
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(544, 188);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(66, 15);
		this.label4.TabIndex = 5;
		this.label4.Text = "Find in lists";
		// 
		// _tFindInList
		// 
		this._tFindInList.Location = new System.Drawing.Point(544, 208);
		this._tFindInList.Name = "_tFindInList";
		this._tFindInList.Size = new System.Drawing.Size(80, 23);
		this._tFindInList.TabIndex = 4;
		// 
		// FProperties
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.ClientSize = new System.Drawing.Size(640, 436);
		this.Controls.Add(this.label4);
		this.Controls.Add(this._grid);
		this.Controls.Add(this._tFindInList);
		this.Controls.Add(this.groupBox2);
		this.Controls.Add(this._bOK);
		this.Controls.Add(this.groupBox1);
		this.Controls.Add(this._bCancel);
		this.Controls.Add(this._info);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.ShowIcon = false;
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.ResumeLayout(false);
		this.PerformLayout();

	}

	#endregion

	private Au.Controls.AuButtonOK _bOK;
	private Au.Controls.AuButtonCancel _bCancel;
	private Au.Controls.InfoBox _info;
	private Au.Controls.ParamGrid _grid;
	private Au.Controls.AuButton _bAddLibraryProject;
	private Au.Controls.AuButton _bAddResource;
	private Au.Controls.AuButton _bAddClass;
	private Au.Controls.AuButton _bAddNet;
	private Au.Controls.AuButton _bAddComRegistry;
	private System.Windows.Forms.GroupBox groupBox2;
	private System.Windows.Forms.GroupBox groupBox1;
	private System.Windows.Forms.TextBox _tFindInList;
	private System.Windows.Forms.Label label4;
	private Au.Controls.AuButton _bAddComBrowse;
}
