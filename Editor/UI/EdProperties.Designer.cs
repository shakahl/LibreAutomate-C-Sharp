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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._grid = new Au.Controls.ParamGrid();
			this._bAddResource = new System.Windows.Forms.Button();
			this._bAddClass = new System.Windows.Forms.Button();
			this._bAddLib = new System.Windows.Forms.Button();
			this._bAddRefOther = new System.Windows.Forms.Button();
			this._bAddRefNet = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _bOK
			// 
			this._bOK.Location = new System.Drawing.Point(224, 408);
			this._bOK.Name = "_bOK";
			this._bOK.Size = new System.Drawing.Size(72, 24);
			this._bOK.TabIndex = 0;
			this._bOK.Click += new System.EventHandler(this._bOK_Click);
			// 
			// _bCancel
			// 
			this._bCancel.Location = new System.Drawing.Point(304, 408);
			this._bCancel.Name = "_bCancel";
			this._bCancel.Size = new System.Drawing.Size(72, 24);
			this._bCancel.TabIndex = 1;
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
			this._info.TabIndex = 7;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Location = new System.Drawing.Point(8, 72);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._grid);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._bAddResource);
			this.splitContainer1.Panel2.Controls.Add(this._bAddClass);
			this.splitContainer1.Panel2.Controls.Add(this._bAddLib);
			this.splitContainer1.Panel2.Controls.Add(this._bAddRefOther);
			this.splitContainer1.Panel2.Controls.Add(this._bAddRefNet);
			this.splitContainer1.Size = new System.Drawing.Size(600, 328);
			this.splitContainer1.SplitterDistance = 300;
			this.splitContainer1.TabIndex = 8;
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
			this._grid.Size = new System.Drawing.Size(300, 328);
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
			// _bAddResource
			// 
			this._bAddResource.Location = new System.Drawing.Point(0, 96);
			this._bAddResource.Name = "_bAddResource";
			this._bAddResource.Size = new System.Drawing.Size(168, 24);
			this._bAddResource.TabIndex = 1;
			this._bAddResource.Text = "Add resource...";
			this._bAddResource.UseVisualStyleBackColor = true;
			this._bAddResource.Click += new System.EventHandler(this._bAddResource_Click);
			// 
			// _bAddClass
			// 
			this._bAddClass.Location = new System.Drawing.Point(0, 72);
			this._bAddClass.Name = "_bAddClass";
			this._bAddClass.Size = new System.Drawing.Size(168, 24);
			this._bAddClass.TabIndex = 1;
			this._bAddClass.Text = "Add class file...";
			this._bAddClass.UseVisualStyleBackColor = true;
			this._bAddClass.Click += new System.EventHandler(this._bAddClass_Click);
			// 
			// _bAddLib
			// 
			this._bAddLib.Location = new System.Drawing.Point(0, 48);
			this._bAddLib.Name = "_bAddLib";
			this._bAddLib.Size = new System.Drawing.Size(168, 24);
			this._bAddLib.TabIndex = 1;
			this._bAddLib.Text = "Add project reference...";
			this._bAddLib.UseVisualStyleBackColor = true;
			this._bAddLib.Click += new System.EventHandler(this._bAddLib_Click);
			// 
			// _bAddRefOther
			// 
			this._bAddRefOther.Location = new System.Drawing.Point(0, 24);
			this._bAddRefOther.Name = "_bAddRefOther";
			this._bAddRefOther.Size = new System.Drawing.Size(168, 24);
			this._bAddRefOther.TabIndex = 1;
			this._bAddRefOther.Text = "Add other reference...";
			this._bAddRefOther.UseVisualStyleBackColor = true;
			this._bAddRefOther.Click += new System.EventHandler(this._bAddRefOther_Click);
			// 
			// _bAddRefNet
			// 
			this._bAddRefNet.Location = new System.Drawing.Point(0, 0);
			this._bAddRefNet.Name = "_bAddRefNet";
			this._bAddRefNet.Size = new System.Drawing.Size(168, 24);
			this._bAddRefNet.TabIndex = 1;
			this._bAddRefNet.Text = "Add .NET/GAC reference...";
			this._bAddRefNet.UseVisualStyleBackColor = true;
			this._bAddRefNet.Click += new System.EventHandler(this._bAddRefNet_Click);
			// 
			// EdCodeFileProperties
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(616, 441);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this._info);
			this.Controls.Add(this._bCancel);
			this.Controls.Add(this._bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.IsPopup = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EdCodeFileProperties";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

	}

	#endregion

	private Au.Controls.ButtonOK _bOK;
	private Au.Controls.ButtonCancel _bCancel;
	private Au.Controls.AuInfoBox _info;
	private System.Windows.Forms.SplitContainer splitContainer1;
	private Au.Controls.ParamGrid _grid;
	private System.Windows.Forms.Button _bAddRefNet;
	private System.Windows.Forms.Button _bAddLib;
	private System.Windows.Forms.Button _bAddResource;
	private System.Windows.Forms.Button _bAddClass;
	private System.Windows.Forms.Button _bAddRefOther;
}
