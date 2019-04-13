partial class EdOptions
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
			this.buttonOK1 = new Au.Controls.ButtonOK();
			this.buttonCancel1 = new Au.Controls.ButtonCancel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._tabGeneral = new System.Windows.Forms.TabPage();
			this._tabEditor = new System.Windows.Forms.TabPage();
			this._tabFiles = new System.Windows.Forms.TabPage();
			this._tabHotkeys = new System.Windows.Forms.TabPage();
			this._tabSounds = new System.Windows.Forms.TabPage();
			this._runAtStartup = new System.Windows.Forms.CheckBox();
			this._versionCheck = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this._startupScripts = new System.Windows.Forms.TextBox();
			this.tabControl1.SuspendLayout();
			this._tabGeneral.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK1
			// 
			this.buttonOK1.Location = new System.Drawing.Point(280, 304);
			this.buttonOK1.Name = "buttonOK1";
			this.buttonOK1.Size = new System.Drawing.Size(72, 24);
			this.buttonOK1.TabIndex = 0;
			// 
			// buttonCancel1
			// 
			this.buttonCancel1.Location = new System.Drawing.Point(360, 304);
			this.buttonCancel1.Name = "buttonCancel1";
			this.buttonCancel1.Size = new System.Drawing.Size(72, 24);
			this.buttonCancel1.TabIndex = 1;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this._tabGeneral);
			this.tabControl1.Controls.Add(this._tabEditor);
			this.tabControl1.Controls.Add(this._tabFiles);
			this.tabControl1.Controls.Add(this._tabHotkeys);
			this.tabControl1.Controls.Add(this._tabSounds);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(448, 296);
			this.tabControl1.TabIndex = 2;
			// 
			// _tabGeneral
			// 
			this._tabGeneral.Controls.Add(this._startupScripts);
			this._tabGeneral.Controls.Add(this.label1);
			this._tabGeneral.Controls.Add(this._versionCheck);
			this._tabGeneral.Controls.Add(this._runAtStartup);
			this._tabGeneral.Location = new System.Drawing.Point(4, 24);
			this._tabGeneral.Name = "_tabGeneral";
			this._tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this._tabGeneral.Size = new System.Drawing.Size(440, 268);
			this._tabGeneral.TabIndex = 0;
			this._tabGeneral.Text = "General";
			this._tabGeneral.UseVisualStyleBackColor = true;
			// 
			// _tabEditor
			// 
			this._tabEditor.Location = new System.Drawing.Point(4, 24);
			this._tabEditor.Name = "_tabEditor";
			this._tabEditor.Padding = new System.Windows.Forms.Padding(3);
			this._tabEditor.Size = new System.Drawing.Size(440, 268);
			this._tabEditor.TabIndex = 1;
			this._tabEditor.Text = "Editor";
			this._tabEditor.UseVisualStyleBackColor = true;
			// 
			// _tabFiles
			// 
			this._tabFiles.Location = new System.Drawing.Point(4, 24);
			this._tabFiles.Name = "_tabFiles";
			this._tabFiles.Size = new System.Drawing.Size(440, 268);
			this._tabFiles.TabIndex = 2;
			this._tabFiles.Text = "Files";
			this._tabFiles.UseVisualStyleBackColor = true;
			// 
			// _tabHotkeys
			// 
			this._tabHotkeys.Location = new System.Drawing.Point(4, 24);
			this._tabHotkeys.Name = "_tabHotkeys";
			this._tabHotkeys.Size = new System.Drawing.Size(440, 268);
			this._tabHotkeys.TabIndex = 3;
			this._tabHotkeys.Text = "Hotkeys";
			this._tabHotkeys.UseVisualStyleBackColor = true;
			// 
			// _tabSounds
			// 
			this._tabSounds.Location = new System.Drawing.Point(4, 24);
			this._tabSounds.Name = "_tabSounds";
			this._tabSounds.Size = new System.Drawing.Size(440, 268);
			this._tabSounds.TabIndex = 4;
			this._tabSounds.Text = "Sounds";
			this._tabSounds.UseVisualStyleBackColor = true;
			// 
			// _runAtStartup
			// 
			this._runAtStartup.AutoSize = true;
			this._runAtStartup.Location = new System.Drawing.Point(8, 12);
			this._runAtStartup.Name = "_runAtStartup";
			this._runAtStartup.Size = new System.Drawing.Size(162, 19);
			this._runAtStartup.TabIndex = 0;
			this._runAtStartup.Text = "Run when Windows starts";
			this._runAtStartup.UseVisualStyleBackColor = true;
			// 
			// _versionCheck
			// 
			this._versionCheck.AutoSize = true;
			this._versionCheck.Location = new System.Drawing.Point(8, 40);
			this._versionCheck.Name = "_versionCheck";
			this._versionCheck.Size = new System.Drawing.Size(171, 19);
			this._versionCheck.TabIndex = 1;
			this._versionCheck.Text = "Check for program updates";
			this._versionCheck.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(216, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(195, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "Run scripts when workspace loaded";
			// 
			// _startupScripts
			// 
			this._startupScripts.Location = new System.Drawing.Point(216, 32);
			this._startupScripts.Multiline = true;
			this._startupScripts.Name = "_startupScripts";
			this._startupScripts.Size = new System.Drawing.Size(216, 52);
			this._startupScripts.TabIndex = 3;
			// 
			// EdOptions
			// 
			this.AcceptButton = this.buttonOK1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel1;
			this.ClientSize = new System.Drawing.Size(446, 336);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonCancel1);
			this.Controls.Add(this.buttonOK1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.IsPopup = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EdOptions";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.tabControl1.ResumeLayout(false);
			this._tabGeneral.ResumeLayout(false);
			this._tabGeneral.PerformLayout();
			this.ResumeLayout(false);

	}

	#endregion

	private Au.Controls.ButtonOK buttonOK1;
	private Au.Controls.ButtonCancel buttonCancel1;
	private System.Windows.Forms.TabControl tabControl1;
	private System.Windows.Forms.TabPage _tabGeneral;
	private System.Windows.Forms.TabPage _tabEditor;
	private System.Windows.Forms.TabPage _tabFiles;
	private System.Windows.Forms.TabPage _tabHotkeys;
	private System.Windows.Forms.TabPage _tabSounds;
	private System.Windows.Forms.CheckBox _versionCheck;
	private System.Windows.Forms.CheckBox _runAtStartup;
	private System.Windows.Forms.TextBox _startupScripts;
	private System.Windows.Forms.Label label1;
}
