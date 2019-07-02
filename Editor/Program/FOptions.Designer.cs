partial class FOptions
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
			this._bOK = new Au.Controls.AuButtonOK();
			this._bCancel = new Au.Controls.AuButtonCancel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._tabGeneral = new System.Windows.Forms.TabPage();
			this._startupScripts = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._alwaysVisible = new Au.Controls.AuCheckBox();
			this._versionCheck = new Au.Controls.AuCheckBox();
			this._runAtStartup = new Au.Controls.AuCheckBox();
			this._tabFiles = new System.Windows.Forms.TabPage();
			this._tabEditor = new System.Windows.Forms.TabPage();
			this._tabHotkeys = new System.Windows.Forms.TabPage();
			this._tabSounds = new System.Windows.Forms.TabPage();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.tabControl1.SuspendLayout();
			this._tabGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _bOK
			// 
			this._bOK.Location = new System.Drawing.Point(320, 304);
			this._bOK.Name = "_bOK";
			this._bOK.Size = new System.Drawing.Size(72, 24);
			this._bOK.TabIndex = 0;
			this._bOK.Click += new System.EventHandler(this._bOK_Click);
			// 
			// _bCancel
			// 
			this._bCancel.Location = new System.Drawing.Point(400, 304);
			this._bCancel.Name = "_bCancel";
			this._bCancel.Size = new System.Drawing.Size(72, 24);
			this._bCancel.TabIndex = 1;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this._tabGeneral);
			this.tabControl1.Controls.Add(this._tabFiles);
			this.tabControl1.Controls.Add(this._tabEditor);
			this.tabControl1.Controls.Add(this._tabHotkeys);
			this.tabControl1.Controls.Add(this._tabSounds);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(480, 296);
			this.tabControl1.TabIndex = 2;
			// 
			// _tabGeneral
			// 
			this._tabGeneral.Controls.Add(this._startupScripts);
			this._tabGeneral.Controls.Add(this.label1);
			this._tabGeneral.Controls.Add(this._alwaysVisible);
			this._tabGeneral.Controls.Add(this._versionCheck);
			this._tabGeneral.Controls.Add(this._runAtStartup);
			this._tabGeneral.Location = new System.Drawing.Point(4, 24);
			this._tabGeneral.Name = "_tabGeneral";
			this._tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this._tabGeneral.Size = new System.Drawing.Size(472, 268);
			this._tabGeneral.TabIndex = 0;
			this._tabGeneral.Text = "General";
			this._tabGeneral.UseVisualStyleBackColor = true;
			// 
			// _startupScripts
			// 
			this._startupScripts.AcceptsReturn = true;
			this._startupScripts.Location = new System.Drawing.Point(232, 32);
			this._startupScripts.Multiline = true;
			this._startupScripts.Name = "_startupScripts";
			this._startupScripts.Size = new System.Drawing.Size(232, 52);
			this._startupScripts.TabIndex = 3;
			this._toolTip.SetToolTip(this._startupScripts, "Script name or path, and delay ms or s.\r\nExample:\r\nscript4.cs, 500 ms\r\n\\folder\\sc" +
        "ript5.cs, 2 s");
			this._startupScripts.WordWrap = false;
			this._startupScripts.Validating += new System.ComponentModel.CancelEventHandler(this._startupScripts_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(232, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(217, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "Run scripts when this workspace loaded";
			// 
			// _alwaysVisible
			// 
			this._alwaysVisible.AutoSize = true;
			this._alwaysVisible.Location = new System.Drawing.Point(8, 32);
			this._alwaysVisible.Name = "_alwaysVisible";
			this._alwaysVisible.Size = new System.Drawing.Size(180, 19);
			this._alwaysVisible.TabIndex = 1;
			this._alwaysVisible.Text = "Start visible; exit when closed";
			// 
			// _versionCheck
			// 
			this._versionCheck.AutoSize = true;
			this._versionCheck.Location = new System.Drawing.Point(8, 52);
			this._versionCheck.Name = "_versionCheck";
			this._versionCheck.Size = new System.Drawing.Size(171, 19);
			this._versionCheck.TabIndex = 2;
			this._versionCheck.Text = "Check for program updates";
			// 
			// _runAtStartup
			// 
			this._runAtStartup.AutoSize = true;
			this._runAtStartup.Location = new System.Drawing.Point(8, 12);
			this._runAtStartup.Name = "_runAtStartup";
			this._runAtStartup.Size = new System.Drawing.Size(162, 19);
			this._runAtStartup.TabIndex = 0;
			this._runAtStartup.Text = "Run when Windows starts";
			// 
			// _tabFiles
			// 
			this._tabFiles.Location = new System.Drawing.Point(4, 24);
			this._tabFiles.Name = "_tabFiles";
			this._tabFiles.Size = new System.Drawing.Size(472, 268);
			this._tabFiles.TabIndex = 2;
			this._tabFiles.Text = "Files";
			this._tabFiles.UseVisualStyleBackColor = true;
			// 
			// _tabEditor
			// 
			this._tabEditor.Location = new System.Drawing.Point(4, 24);
			this._tabEditor.Name = "_tabEditor";
			this._tabEditor.Padding = new System.Windows.Forms.Padding(3);
			this._tabEditor.Size = new System.Drawing.Size(472, 268);
			this._tabEditor.TabIndex = 1;
			this._tabEditor.Text = "Editor";
			this._tabEditor.UseVisualStyleBackColor = true;
			// 
			// _tabHotkeys
			// 
			this._tabHotkeys.Location = new System.Drawing.Point(4, 24);
			this._tabHotkeys.Name = "_tabHotkeys";
			this._tabHotkeys.Size = new System.Drawing.Size(472, 268);
			this._tabHotkeys.TabIndex = 3;
			this._tabHotkeys.Text = "Hotkeys";
			this._tabHotkeys.UseVisualStyleBackColor = true;
			// 
			// _tabSounds
			// 
			this._tabSounds.Location = new System.Drawing.Point(4, 24);
			this._tabSounds.Name = "_tabSounds";
			this._tabSounds.Size = new System.Drawing.Size(472, 268);
			this._tabSounds.TabIndex = 4;
			this._tabSounds.Text = "Sounds";
			this._tabSounds.UseVisualStyleBackColor = true;
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// _toolTip
			// 
			this._toolTip.AutoPopDelay = 5000;
			this._toolTip.InitialDelay = 100;
			this._toolTip.ReshowDelay = 100;
			// 
			// FOptions
			// 
			this.AcceptButton = this._bOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._bCancel;
			this.ClientSize = new System.Drawing.Size(484, 336);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._bCancel);
			this.Controls.Add(this._bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.IsPopup = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.tabControl1.ResumeLayout(false);
			this._tabGeneral.ResumeLayout(false);
			this._tabGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

	}

	#endregion

	private Au.Controls.AuButtonOK _bOK;
	private Au.Controls.AuButtonCancel _bCancel;
	private System.Windows.Forms.TabControl tabControl1;
	private System.Windows.Forms.TabPage _tabGeneral;
	private System.Windows.Forms.TabPage _tabEditor;
	private System.Windows.Forms.TabPage _tabFiles;
	private System.Windows.Forms.TabPage _tabHotkeys;
	private System.Windows.Forms.TabPage _tabSounds;
	private Au.Controls.AuCheckBox _versionCheck;
	private Au.Controls.AuCheckBox _runAtStartup;
	private System.Windows.Forms.TextBox _startupScripts;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.ErrorProvider _errorProvider;
	private System.Windows.Forms.ToolTip _toolTip;
	private Au.Controls.AuCheckBox _alwaysVisible;
}
