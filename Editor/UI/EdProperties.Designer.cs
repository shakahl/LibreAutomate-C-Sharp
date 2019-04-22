partial class EdCodeFileProperties
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
			this._bOK = new Au.Controls.ButtonOK();
			this._bCancel = new Au.Controls.ButtonCancel();
			this._info = new Au.Controls.AuInfoBox();
			this._grid = new Au.Controls.ParamGrid();
			this.label4 = new System.Windows.Forms.Label();
			this._tFindInList = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this._bAddComBrowse = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._bAddBrowseNet = new System.Windows.Forms.Button();
			this._bAddBrowseOther = new System.Windows.Forms.Button();
			this._bAddGacNewest = new System.Windows.Forms.Button();
			this._bAddGacVersion = new System.Windows.Forms.Button();
			this._bAddMyLibraryProject = new System.Windows.Forms.Button();
			this._bAddComRegistry = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._bAddClass = new System.Windows.Forms.Button();
			this._bAddResource = new System.Windows.Forms.Button();
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
			this._info.AccessibleName = "_info";
			this._info.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this._info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._info.DisableModifiedNotifications = false;
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
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(544, 200);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(66, 15);
			this.label4.TabIndex = 5;
			this.label4.Text = "Find in lists";
			// 
			// _tFindInList
			// 
			this._tFindInList.Location = new System.Drawing.Point(544, 220);
			this._tFindInList.Name = "_tFindInList";
			this._tFindInList.Size = new System.Drawing.Size(80, 23);
			this._tFindInList.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this._bAddComBrowse);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._bAddBrowseNet);
			this.groupBox1.Controls.Add(this._bAddBrowseOther);
			this.groupBox1.Controls.Add(this._bAddGacNewest);
			this.groupBox1.Controls.Add(this._bAddGacVersion);
			this.groupBox1.Controls.Add(this._bAddMyLibraryProject);
			this.groupBox1.Controls.Add(this._bAddComRegistry);
			this.groupBox1.Location = new System.Drawing.Point(320, 100);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(208, 152);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Add assembly reference";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 124);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(24, 15);
			this.label5.TabIndex = 9;
			this.label5.Text = "My";
			// 
			// _bAddComBrowse
			// 
			this._bAddComBrowse.Location = new System.Drawing.Point(128, 88);
			this._bAddComBrowse.Name = "_bAddComBrowse";
			this._bAddComBrowse.Size = new System.Drawing.Size(72, 24);
			this._bAddComBrowse.TabIndex = 8;
			this._bAddComBrowse.Text = "Browse...";
			this._bAddComBrowse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddComBrowse.UseVisualStyleBackColor = true;
			this._bAddComBrowse.Click += new System.EventHandler(this._bAddComBrowse_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 92);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 15);
			this.label3.TabIndex = 7;
			this.label3.Text = "COM";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(31, 15);
			this.label2.TabIndex = 6;
			this.label2.Text = "GAC";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 15);
			this.label1.TabIndex = 5;
			this.label1.Text = "Browse";
			// 
			// _bAddBrowseNet
			// 
			this._bAddBrowseNet.Location = new System.Drawing.Point(56, 24);
			this._bAddBrowseNet.Name = "_bAddBrowseNet";
			this._bAddBrowseNet.Size = new System.Drawing.Size(72, 24);
			this._bAddBrowseNet.TabIndex = 1;
			this._bAddBrowseNet.Text = ".NET...";
			this._bAddBrowseNet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddBrowseNet.UseVisualStyleBackColor = true;
			this._bAddBrowseNet.Click += new System.EventHandler(this._bAddBrowse_Click);
			// 
			// _bAddBrowseOther
			// 
			this._bAddBrowseOther.Location = new System.Drawing.Point(128, 24);
			this._bAddBrowseOther.Name = "_bAddBrowseOther";
			this._bAddBrowseOther.Size = new System.Drawing.Size(72, 24);
			this._bAddBrowseOther.TabIndex = 1;
			this._bAddBrowseOther.Text = "Other...";
			this._bAddBrowseOther.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddBrowseOther.UseVisualStyleBackColor = true;
			this._bAddBrowseOther.Click += new System.EventHandler(this._bAddBrowse_Click);
			// 
			// _bAddGacNewest
			// 
			this._bAddGacNewest.Location = new System.Drawing.Point(56, 56);
			this._bAddGacNewest.Name = "_bAddGacNewest";
			this._bAddGacNewest.Size = new System.Drawing.Size(72, 24);
			this._bAddGacNewest.TabIndex = 1;
			this._bAddGacNewest.Text = "Newest...";
			this._bAddGacNewest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddGacNewest.UseVisualStyleBackColor = true;
			this._bAddGacNewest.Click += new System.EventHandler(this._bAddGac_Click);
			// 
			// _bAddGacVersion
			// 
			this._bAddGacVersion.Location = new System.Drawing.Point(128, 56);
			this._bAddGacVersion.Name = "_bAddGacVersion";
			this._bAddGacVersion.Size = new System.Drawing.Size(72, 24);
			this._bAddGacVersion.TabIndex = 1;
			this._bAddGacVersion.Text = "Version...";
			this._bAddGacVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddGacVersion.UseVisualStyleBackColor = true;
			this._bAddGacVersion.Click += new System.EventHandler(this._bAddGac_Click);
			// 
			// _bAddMyLibraryProject
			// 
			this._bAddMyLibraryProject.Location = new System.Drawing.Point(56, 120);
			this._bAddMyLibraryProject.Name = "_bAddMyLibraryProject";
			this._bAddMyLibraryProject.Size = new System.Drawing.Size(144, 24);
			this._bAddMyLibraryProject.TabIndex = 1;
			this._bAddMyLibraryProject.Text = "Library project...";
			this._bAddMyLibraryProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddMyLibraryProject.UseVisualStyleBackColor = true;
			this._bAddMyLibraryProject.Click += new System.EventHandler(this._bAddMyLibraryProject_Click);
			// 
			// _bAddComRegistry
			// 
			this._bAddComRegistry.Location = new System.Drawing.Point(56, 88);
			this._bAddComRegistry.Name = "_bAddComRegistry";
			this._bAddComRegistry.Size = new System.Drawing.Size(72, 24);
			this._bAddComRegistry.TabIndex = 1;
			this._bAddComRegistry.Text = "Registry...";
			this._bAddComRegistry.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddComRegistry.UseVisualStyleBackColor = true;
			this._bAddComRegistry.Click += new System.EventHandler(this._bAddComRegistry_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._bAddClass);
			this.groupBox2.Controls.Add(this._bAddResource);
			this.groupBox2.Location = new System.Drawing.Point(536, 100);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(96, 84);
			this.groupBox2.TabIndex = 3;
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
			this._bAddClass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddClass.UseVisualStyleBackColor = true;
			this._bAddClass.Click += new System.EventHandler(this._bAddClass_Click);
			// 
			// _bAddResource
			// 
			this._bAddResource.Location = new System.Drawing.Point(8, 52);
			this._bAddResource.Name = "_bAddResource";
			this._bAddResource.Size = new System.Drawing.Size(80, 24);
			this._bAddResource.TabIndex = 1;
			this._bAddResource.Text = "Resource...";
			this._bAddResource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._bAddResource.UseVisualStyleBackColor = true;
			this._bAddResource.Click += new System.EventHandler(this._bAddResource_Click);
			// 
			// EdCodeFileProperties
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
			this.IsPopup = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EdCodeFileProperties";
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

	private Au.Controls.ButtonOK _bOK;
	private Au.Controls.ButtonCancel _bCancel;
	private Au.Controls.AuInfoBox _info;
	private Au.Controls.ParamGrid _grid;
	private System.Windows.Forms.Button _bAddBrowseNet;
	private System.Windows.Forms.Button _bAddMyLibraryProject;
	private System.Windows.Forms.Button _bAddResource;
	private System.Windows.Forms.Button _bAddClass;
	private System.Windows.Forms.Button _bAddBrowseOther;
	private System.Windows.Forms.Button _bAddGacNewest;
	private System.Windows.Forms.Button _bAddComRegistry;
	private System.Windows.Forms.Button _bAddGacVersion;
	private System.Windows.Forms.GroupBox groupBox2;
	private System.Windows.Forms.GroupBox groupBox1;
	private System.Windows.Forms.TextBox _tFindInList;
	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.Label label5;
	private System.Windows.Forms.Button _bAddComBrowse;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label1;
}
