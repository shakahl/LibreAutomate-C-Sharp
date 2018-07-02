namespace Au.Tools
{
	partial class Form_WinImage
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
			this._info = new Au.Controls.AuInfoBox();
			this._bCopy = new System.Windows.Forms.Button();
			this._lSpeed = new System.Windows.Forms.Label();
			this._bTest = new System.Windows.Forms.Button();
			this._bCancel = new System.Windows.Forms.Button();
			this._bOK = new System.Windows.Forms.Button();
			this._tFind = new System.Windows.Forms.TextBox();
			this._bCapture = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._grid = new Au.Controls.ParamGrid();
			this._pict = new System.Windows.Forms.PictureBox();
			this._toolTipTextBoxBugWorkaround = new System.Windows.Forms.ToolTip(this.components);
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._bEtc = new System.Windows.Forms.Button();
			this._tWnd = new Au.Tools.TextBoxWnd();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._pict)).BeginInit();
			this.SuspendLayout();
			// 
			// _info
			// 
			this._info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._info.Location = new System.Drawing.Point(8, 8);
			this._info.Name = "_info";
			this._info.Size = new System.Drawing.Size(552, 56);
			this._info.TabIndex = 9;
			// 
			// _bCopy
			// 
			this._bCopy.Enabled = false;
			this._bCopy.Location = new System.Drawing.Point(408, 128);
			this._bCopy.Name = "_bCopy";
			this._bCopy.Size = new System.Drawing.Size(72, 24);
			this._bCopy.TabIndex = 5;
			this._bCopy.Text = "Co&py";
			this._toolTip.SetToolTip(this._bCopy, "Copies the code to the clipboard.");
			this._bCopy.UseVisualStyleBackColor = true;
			this._bCopy.Click += new System.EventHandler(this._bCopy_Click);
			// 
			// _lSpeed
			// 
			this._lSpeed.Location = new System.Drawing.Point(167, 133);
			this._lSpeed.Name = "_lSpeed";
			this._lSpeed.Size = new System.Drawing.Size(81, 15);
			this._lSpeed.TabIndex = 2;
			this._toolTip.SetToolTip(this._lSpeed, "Shows the Test execution time. Red if not found.");
			// 
			// _bTest
			// 
			this._bTest.Enabled = false;
			this._bTest.Location = new System.Drawing.Point(88, 128);
			this._bTest.Name = "_bTest";
			this._bTest.Size = new System.Drawing.Size(72, 24);
			this._bTest.TabIndex = 1;
			this._bTest.Text = "&Test";
			this._toolTip.SetToolTip(this._bTest, "Executes the code. Shows the rectangle of the found image.");
			this._bTest.UseVisualStyleBackColor = true;
			this._bTest.Click += new System.EventHandler(this._bTest_Click);
			// 
			// _bCancel
			// 
			this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._bCancel.Location = new System.Drawing.Point(328, 128);
			this._bCancel.Name = "_bCancel";
			this._bCancel.Size = new System.Drawing.Size(72, 24);
			this._bCancel.TabIndex = 4;
			this._bCancel.Text = "&Cancel";
			this._bCancel.UseVisualStyleBackColor = true;
			// 
			// _bOK
			// 
			this._bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._bOK.Enabled = false;
			this._bOK.Location = new System.Drawing.Point(248, 128);
			this._bOK.Name = "_bOK";
			this._bOK.Size = new System.Drawing.Size(72, 24);
			this._bOK.TabIndex = 3;
			this._bOK.Text = "&OK";
			this._bOK.UseVisualStyleBackColor = true;
			this._bOK.Click += new System.EventHandler(this._bOK_Click);
			// 
			// _tFind
			// 
			this._tFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tFind.Location = new System.Drawing.Point(8, 96);
			this._tFind.Name = "_tFind";
			this._tFind.ReadOnly = true;
			this._tFind.Size = new System.Drawing.Size(552, 23);
			this._tFind.TabIndex = 11;
			this._toolTipTextBoxBugWorkaround.SetToolTip(this._tFind, "The find-image code. Read-only. To change it, use controls below.");
			// 
			// _bCapture
			// 
			this._bCapture.Location = new System.Drawing.Point(8, 128);
			this._bCapture.Name = "_bCapture";
			this._bCapture.Size = new System.Drawing.Size(72, 24);
			this._bCapture.TabIndex = 0;
			this._bCapture.Text = "C&apture";
			this._bCapture.UseVisualStyleBackColor = true;
			this._bCapture.Click += new System.EventHandler(this._bCapture_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(8, 160);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._grid);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer1.Panel2.Controls.Add(this._pict);
			this.splitContainer1.Size = new System.Drawing.Size(552, 192);
			this.splitContainer1.SplitterDistance = 312;
			this.splitContainer1.SplitterWidth = 8;
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
			this._grid.Size = new System.Drawing.Size(312, 192);
			this._grid.SpecialKeys = ((SourceGrid.GridSpecialKeys)((((((SourceGrid.GridSpecialKeys.Arrows | SourceGrid.GridSpecialKeys.PageDownUp) 
            | SourceGrid.GridSpecialKeys.Enter) 
            | SourceGrid.GridSpecialKeys.Escape) 
            | SourceGrid.GridSpecialKeys.Control) 
            | SourceGrid.GridSpecialKeys.Shift)));
			this._grid.TabIndex = 1;
			this._grid.TabStop = true;
			this._grid.ToolTipText = "";
			// 
			// _pict
			// 
			this._pict.BackColor = System.Drawing.SystemColors.ControlDark;
			this._pict.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._pict.Location = new System.Drawing.Point(0, 0);
			this._pict.Name = "_pict";
			this._pict.Size = new System.Drawing.Size(16, 16);
			this._pict.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._pict.TabIndex = 0;
			this._pict.TabStop = false;
			// 
			// _toolTipTextBoxBugWorkaround
			// 
			this._toolTipTextBoxBugWorkaround.AutoPopDelay = 5000;
			this._toolTipTextBoxBugWorkaround.InitialDelay = 50;
			this._toolTipTextBoxBugWorkaround.ReshowDelay = 100;
			// 
			// _bEtc
			// 
			this._bEtc.Enabled = false;
			this._bEtc.Location = new System.Drawing.Point(488, 128);
			this._bEtc.Name = "_bEtc";
			this._bEtc.Size = new System.Drawing.Size(24, 24);
			this._bEtc.TabIndex = 6;
			this._bEtc.Text = "...";
			this._bEtc.UseVisualStyleBackColor = true;
			this._bEtc.Click += new System.EventHandler(this._bEtc_Click);
			// 
			// _tWnd
			// 
			this._tWnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tWnd.Location = new System.Drawing.Point(8, 72);
			this._tWnd.Name = "_tWnd";
			this._tWnd.ReadOnly = true;
			this._tWnd.Size = new System.Drawing.Size(552, 23);
			this._tWnd.TabIndex = 10;
			this._toolTipTextBoxBugWorkaround.SetToolTip(this._tWnd, "The find-window code. You can edit it, for example rename the variable w.");
			// 
			// Form_WinImage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(568, 361);
			this.Controls.Add(this._bEtc);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this._info);
			this.Controls.Add(this._bCopy);
			this.Controls.Add(this._lSpeed);
			this.Controls.Add(this._bCapture);
			this.Controls.Add(this._bTest);
			this.Controls.Add(this._bCancel);
			this.Controls.Add(this._bOK);
			this.Controls.Add(this._tFind);
			this.Controls.Add(this._tWnd);
			this.Name = "Form_WinImage";
			this.Text = "Find image or color in window";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._pict)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.AuInfoBox _info;
		private System.Windows.Forms.Button _bCopy;
		private System.Windows.Forms.Label _lSpeed;
		private System.Windows.Forms.Button _bTest;
		private System.Windows.Forms.Button _bCancel;
		private System.Windows.Forms.Button _bOK;
		private System.Windows.Forms.TextBox _tFind;
		private Au.Tools.TextBoxWnd _tWnd;
		private System.Windows.Forms.Button _bCapture;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private Controls.ParamGrid _grid;
		private System.Windows.Forms.PictureBox _pict;
		private System.Windows.Forms.ToolTip _toolTipTextBoxBugWorkaround;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.Button _bEtc;
	}
}