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
			this._eStartupScripts = new System.Windows.Forms.TextBox();
			this.label1 = new Au.Controls.AuLabel();
			this._cRunHidden = new Au.Controls.AuCheckBox();
			this._cCheckVersion = new Au.Controls.AuCheckBox();
			this._cRunAtStartup = new Au.Controls.AuCheckBox();
			this._tabFiles = new System.Windows.Forms.TabPage();
			this._tabTemplates = new System.Windows.Forms.TabPage();
			this._comboUseTemplate = new System.Windows.Forms.ComboBox();
			this._comboTemplate = new System.Windows.Forms.ComboBox();
			this.auLabel2 = new Au.Controls.AuLabel();
			this._sciTemplate = new Au.Tools.CodeBox();
			this.auLabel1 = new Au.Controls.AuLabel();
			this._tabFont = new System.Windows.Forms.TabPage();
			this._pColor = new System.Windows.Forms.Panel();
			this.label4 = new Au.Controls.AuLabel();
			this.label5 = new Au.Controls.AuLabel();
			this._eColor = new System.Windows.Forms.TextBox();
			this.label6 = new Au.Controls.AuLabel();
			this._cBold = new Au.Controls.AuCheckBox();
			this.label7 = new Au.Controls.AuLabel();
			this._nSat = new System.Windows.Forms.NumericUpDown();
			this.label8 = new Au.Controls.AuLabel();
			this._nHue = new System.Windows.Forms.NumericUpDown();
			this._nRed = new System.Windows.Forms.NumericUpDown();
			this._nLum = new System.Windows.Forms.NumericUpDown();
			this.label9 = new Au.Controls.AuLabel();
			this._nBlue = new System.Windows.Forms.NumericUpDown();
			this._nGreen = new System.Windows.Forms.NumericUpDown();
			this.label10 = new Au.Controls.AuLabel();
			this._pFont = new System.Windows.Forms.Panel();
			this.label2 = new Au.Controls.AuLabel();
			this._nFontSize = new System.Windows.Forms.NumericUpDown();
			this.label3 = new Au.Controls.AuLabel();
			this._comboFont = new System.Windows.Forms.ComboBox();
			this._sciStyles = new Au.Controls.AuScintilla();
			this._tabCode = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._cCustomSnippets = new Au.Controls.AuCheckBox();
			this._cComplParenSpace = new Au.Controls.AuCheckBox();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._bApply = new Au.Controls.AuButton();
			this._cStringEnterRN = new Au.Controls.AuCheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tabControl1.SuspendLayout();
			this._tabGeneral.SuspendLayout();
			this._tabTemplates.SuspendLayout();
			this._tabFont.SuspendLayout();
			this._pColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._nSat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._nHue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._nRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._nLum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._nBlue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._nGreen)).BeginInit();
			this._pFont.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._nFontSize)).BeginInit();
			this._tabCode.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _bOK
			// 
			this._bOK.Location = new System.Drawing.Point(256, 304);
			this._bOK.Name = "_bOK";
			this._bOK.Size = new System.Drawing.Size(72, 24);
			this._bOK.TabIndex = 0;
			this._bOK.Click += new System.EventHandler(this._bOK_Click);
			// 
			// _bCancel
			// 
			this._bCancel.Location = new System.Drawing.Point(336, 304);
			this._bCancel.Name = "_bCancel";
			this._bCancel.Size = new System.Drawing.Size(72, 24);
			this._bCancel.TabIndex = 1;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this._tabGeneral);
			this.tabControl1.Controls.Add(this._tabFiles);
			this.tabControl1.Controls.Add(this._tabTemplates);
			this.tabControl1.Controls.Add(this._tabFont);
			this.tabControl1.Controls.Add(this._tabCode);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(496, 296);
			this.tabControl1.TabIndex = 2;
			// 
			// _tabGeneral
			// 
			this._tabGeneral.Controls.Add(this._eStartupScripts);
			this._tabGeneral.Controls.Add(this.label1);
			this._tabGeneral.Controls.Add(this._cRunHidden);
			this._tabGeneral.Controls.Add(this._cCheckVersion);
			this._tabGeneral.Controls.Add(this._cRunAtStartup);
			this._tabGeneral.Location = new System.Drawing.Point(4, 24);
			this._tabGeneral.Name = "_tabGeneral";
			this._tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this._tabGeneral.Size = new System.Drawing.Size(488, 268);
			this._tabGeneral.TabIndex = 0;
			this._tabGeneral.Text = "General";
			this._tabGeneral.UseVisualStyleBackColor = true;
			// 
			// _eStartupScripts
			// 
			this._eStartupScripts.AcceptsReturn = true;
			this._eStartupScripts.Location = new System.Drawing.Point(232, 32);
			this._eStartupScripts.Multiline = true;
			this._eStartupScripts.Name = "_eStartupScripts";
			this._eStartupScripts.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._eStartupScripts.Size = new System.Drawing.Size(248, 60);
			this._eStartupScripts.TabIndex = 3;
			this._toolTip.SetToolTip(this._eStartupScripts, "Script name or path, and delay ms or s.\r\nExample:\r\nscript4.cs, 500 ms\r\n\\folder\\sc" +
        "ript5.cs, 2 s\r\n//disabled, 100 ms");
			this._eStartupScripts.WordWrap = false;
			this._eStartupScripts.Validating += new System.ComponentModel.CancelEventHandler(this._startupScripts_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(232, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(217, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Run scripts when this workspace loaded";
			// 
			// _cRunHidden
			// 
			this._cRunHidden.AutoSize = true;
			this._cRunHidden.Location = new System.Drawing.Point(8, 32);
			this._cRunHidden.Name = "_cRunHidden";
			this._cRunHidden.Size = new System.Drawing.Size(198, 20);
			this._cRunHidden.TabIndex = 1;
			this._cRunHidden.Text = "Start hidden; hide when closing";
			this._toolTip.SetToolTip(this._cRunHidden, "Start this program without a window.\r\nTo show it, click the tray icon or run this" +
        " program again.\r\nThe \'close window\' button hides it instead. To exit, use a menu" +
        ".");
			this._cRunHidden.UseVisualStyleBackColor = false;
			// 
			// _cCheckVersion
			// 
			this._cCheckVersion.AutoSize = true;
			this._cCheckVersion.Location = new System.Drawing.Point(8, 52);
			this._cCheckVersion.Name = "_cCheckVersion";
			this._cCheckVersion.Size = new System.Drawing.Size(177, 20);
			this._cCheckVersion.TabIndex = 2;
			this._cCheckVersion.Text = "Check for program updates";
			this._cCheckVersion.UseVisualStyleBackColor = false;
			this._cCheckVersion.Visible = false;
			// 
			// _cRunAtStartup
			// 
			this._cRunAtStartup.AutoSize = true;
			this._cRunAtStartup.Location = new System.Drawing.Point(8, 12);
			this._cRunAtStartup.Name = "_cRunAtStartup";
			this._cRunAtStartup.Size = new System.Drawing.Size(134, 20);
			this._cRunAtStartup.TabIndex = 0;
			this._cRunAtStartup.Text = "Start with Windows";
			this._toolTip.SetToolTip(this._cRunAtStartup, "Run this program when Windows starts and user logs on.\r\nThis setting is in Regist" +
        "ry.");
			this._cRunAtStartup.UseVisualStyleBackColor = false;
			// 
			// _tabFiles
			// 
			this._tabFiles.Location = new System.Drawing.Point(4, 24);
			this._tabFiles.Name = "_tabFiles";
			this._tabFiles.Size = new System.Drawing.Size(488, 268);
			this._tabFiles.TabIndex = 2;
			this._tabFiles.Text = "Files";
			this._tabFiles.UseVisualStyleBackColor = true;
			// 
			// _tabTemplates
			// 
			this._tabTemplates.Controls.Add(this._comboUseTemplate);
			this._tabTemplates.Controls.Add(this._comboTemplate);
			this._tabTemplates.Controls.Add(this.auLabel2);
			this._tabTemplates.Controls.Add(this._sciTemplate);
			this._tabTemplates.Controls.Add(this.auLabel1);
			this._tabTemplates.Location = new System.Drawing.Point(4, 24);
			this._tabTemplates.Name = "_tabTemplates";
			this._tabTemplates.Size = new System.Drawing.Size(488, 268);
			this._tabTemplates.TabIndex = 6;
			this._tabTemplates.Text = "Templates";
			this._tabTemplates.UseVisualStyleBackColor = true;
			// 
			// _comboUseTemplate
			// 
			this._comboUseTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboUseTemplate.FormattingEnabled = true;
			this._comboUseTemplate.Location = new System.Drawing.Point(360, 8);
			this._comboUseTemplate.Name = "_comboUseTemplate";
			this._comboUseTemplate.Size = new System.Drawing.Size(120, 23);
			this._comboUseTemplate.TabIndex = 5;
			// 
			// _comboTemplate
			// 
			this._comboTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboTemplate.FormattingEnabled = true;
			this._comboTemplate.Location = new System.Drawing.Point(72, 8);
			this._comboTemplate.Name = "_comboTemplate";
			this._comboTemplate.Size = new System.Drawing.Size(168, 23);
			this._comboTemplate.TabIndex = 5;
			// 
			// auLabel2
			// 
			this.auLabel2.AutoSize = true;
			this.auLabel2.Location = new System.Drawing.Point(328, 8);
			this.auLabel2.Name = "auLabel2";
			this.auLabel2.Size = new System.Drawing.Size(26, 17);
			this.auLabel2.TabIndex = 4;
			this.auLabel2.Text = "Use";
			// 
			// _sciTemplate
			// 
			this._sciTemplate.AccessibleName = "";
			this._sciTemplate.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this._sciTemplate.Location = new System.Drawing.Point(8, 40);
			this._sciTemplate.Name = "_sciTemplate";
			this._sciTemplate.Size = new System.Drawing.Size(472, 220);
			this._sciTemplate.TabIndex = 0;
			this._sciTemplate.ZAcceptsReturn = true;
			// 
			// auLabel1
			// 
			this.auLabel1.AutoSize = true;
			this.auLabel1.Location = new System.Drawing.Point(8, 8);
			this.auLabel1.Name = "auLabel1";
			this.auLabel1.Size = new System.Drawing.Size(55, 17);
			this.auLabel1.TabIndex = 4;
			this.auLabel1.Text = "Template";
			// 
			// _tabFont
			// 
			this._tabFont.Controls.Add(this._pColor);
			this._tabFont.Controls.Add(this._pFont);
			this._tabFont.Controls.Add(this._sciStyles);
			this._tabFont.Location = new System.Drawing.Point(4, 24);
			this._tabFont.Name = "_tabFont";
			this._tabFont.Padding = new System.Windows.Forms.Padding(3);
			this._tabFont.Size = new System.Drawing.Size(488, 268);
			this._tabFont.TabIndex = 1;
			this._tabFont.Text = "Font";
			this._tabFont.UseVisualStyleBackColor = true;
			// 
			// _pColor
			// 
			this._pColor.Controls.Add(this.label4);
			this._pColor.Controls.Add(this.label5);
			this._pColor.Controls.Add(this._eColor);
			this._pColor.Controls.Add(this.label6);
			this._pColor.Controls.Add(this._cBold);
			this._pColor.Controls.Add(this.label7);
			this._pColor.Controls.Add(this._nSat);
			this._pColor.Controls.Add(this.label8);
			this._pColor.Controls.Add(this._nHue);
			this._pColor.Controls.Add(this._nRed);
			this._pColor.Controls.Add(this._nLum);
			this._pColor.Controls.Add(this.label9);
			this._pColor.Controls.Add(this._nBlue);
			this._pColor.Controls.Add(this._nGreen);
			this._pColor.Controls.Add(this.label10);
			this._pColor.Location = new System.Drawing.Point(184, 8);
			this._pColor.Name = "_pColor";
			this._pColor.Size = new System.Drawing.Size(296, 92);
			this._pColor.TabIndex = 13;
			this._pColor.Visible = false;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(36, 17);
			this.label4.TabIndex = 5;
			this.label4.Text = "Color";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(0, 32);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(14, 17);
			this.label5.TabIndex = 7;
			this.label5.Text = "R";
			// 
			// _eColor
			// 
			this._eColor.Location = new System.Drawing.Point(40, 0);
			this._eColor.Name = "_eColor";
			this._eColor.Size = new System.Drawing.Size(104, 23);
			this._eColor.TabIndex = 10;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(80, 32);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(15, 17);
			this.label6.TabIndex = 7;
			this.label6.Text = "G";
			// 
			// _cBold
			// 
			this._cBold.AutoSize = true;
			this._cBold.Location = new System.Drawing.Point(176, 0);
			this._cBold.Name = "_cBold";
			this._cBold.Size = new System.Drawing.Size(56, 20);
			this._cBold.TabIndex = 9;
			this._cBold.Text = "Bold";
			this._cBold.UseVisualStyleBackColor = false;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(160, 32);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(14, 17);
			this.label7.TabIndex = 7;
			this.label7.Text = "B";
			// 
			// _nSat
			// 
			this._nSat.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._nSat.Location = new System.Drawing.Point(176, 64);
			this._nSat.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
			this._nSat.Name = "_nSat";
			this._nSat.Size = new System.Drawing.Size(48, 23);
			this._nSat.TabIndex = 8;
			this._toolTip.SetToolTip(this._nSat, "Saturation 0-240");
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(80, 64);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(13, 17);
			this.label8.TabIndex = 7;
			this.label8.Text = "L";
			// 
			// _nHue
			// 
			this._nHue.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._nHue.Location = new System.Drawing.Point(16, 64);
			this._nHue.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
			this._nHue.Name = "_nHue";
			this._nHue.Size = new System.Drawing.Size(48, 23);
			this._nHue.TabIndex = 8;
			this._toolTip.SetToolTip(this._nHue, "Hue 0-240");
			// 
			// _nRed
			// 
			this._nRed.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this._nRed.Location = new System.Drawing.Point(16, 32);
			this._nRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this._nRed.Name = "_nRed";
			this._nRed.Size = new System.Drawing.Size(48, 23);
			this._nRed.TabIndex = 8;
			this._toolTip.SetToolTip(this._nRed, "Red 0-255");
			// 
			// _nLum
			// 
			this._nLum.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._nLum.Location = new System.Drawing.Point(96, 64);
			this._nLum.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
			this._nLum.Name = "_nLum";
			this._nLum.Size = new System.Drawing.Size(48, 23);
			this._nLum.TabIndex = 8;
			this._toolTip.SetToolTip(this._nLum, "Luminance 0-240");
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(0, 64);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(16, 17);
			this.label9.TabIndex = 7;
			this.label9.Text = "H";
			// 
			// _nBlue
			// 
			this._nBlue.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this._nBlue.Location = new System.Drawing.Point(176, 32);
			this._nBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this._nBlue.Name = "_nBlue";
			this._nBlue.Size = new System.Drawing.Size(48, 23);
			this._nBlue.TabIndex = 8;
			this._toolTip.SetToolTip(this._nBlue, "Blue 0-255");
			// 
			// _nGreen
			// 
			this._nGreen.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this._nGreen.Location = new System.Drawing.Point(96, 32);
			this._nGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this._nGreen.Name = "_nGreen";
			this._nGreen.Size = new System.Drawing.Size(48, 23);
			this._nGreen.TabIndex = 8;
			this._toolTip.SetToolTip(this._nGreen, "Green 0-255");
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(160, 64);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(13, 17);
			this.label10.TabIndex = 7;
			this.label10.Text = "S";
			// 
			// _pFont
			// 
			this._pFont.Controls.Add(this.label2);
			this._pFont.Controls.Add(this._nFontSize);
			this._pFont.Controls.Add(this.label3);
			this._pFont.Controls.Add(this._comboFont);
			this._pFont.Location = new System.Drawing.Point(184, 204);
			this._pFont.Name = "_pFont";
			this._pFont.Size = new System.Drawing.Size(296, 56);
			this._pFont.TabIndex = 12;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(31, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Font";
			// 
			// _nFontSize
			// 
			this._nFontSize.Location = new System.Drawing.Point(40, 32);
			this._nFontSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this._nFontSize.Name = "_nFontSize";
			this._nFontSize.Size = new System.Drawing.Size(48, 23);
			this._nFontSize.TabIndex = 3;
			this._nFontSize.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(0, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(27, 17);
			this.label3.TabIndex = 2;
			this.label3.Text = "Size";
			// 
			// _comboFont
			// 
			this._comboFont.FormattingEnabled = true;
			this._comboFont.Location = new System.Drawing.Point(40, 0);
			this._comboFont.Name = "_comboFont";
			this._comboFont.Size = new System.Drawing.Size(256, 23);
			this._comboFont.TabIndex = 1;
			// 
			// _sciStyles
			// 
			this._sciStyles.AccessibleName = "";
			this._sciStyles.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this._sciStyles.Location = new System.Drawing.Point(8, 8);
			this._sciStyles.Name = "_sciStyles";
			this._sciStyles.Size = new System.Drawing.Size(160, 252);
			this._sciStyles.TabIndex = 4;
			this._sciStyles.ZInitBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._sciStyles.ZInitReadOnlyAlways = true;
			// 
			// _tabCode
			// 
			this._tabCode.Controls.Add(this.groupBox2);
			this._tabCode.Controls.Add(this.groupBox1);
			this._tabCode.Location = new System.Drawing.Point(4, 24);
			this._tabCode.Name = "_tabCode";
			this._tabCode.Size = new System.Drawing.Size(488, 268);
			this._tabCode.TabIndex = 5;
			this._tabCode.Text = "Code";
			this._tabCode.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._cCustomSnippets);
			this.groupBox1.Controls.Add(this._cComplParenSpace);
			this.groupBox1.Location = new System.Drawing.Point(8, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(472, 72);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Completion lists";
			// 
			// _cCustomSnippets
			// 
			this._cCustomSnippets.AutoSize = true;
			this._cCustomSnippets.Location = new System.Drawing.Point(8, 44);
			this._cCustomSnippets.Name = "_cCustomSnippets";
			this._cCustomSnippets.Size = new System.Drawing.Size(141, 20);
			this._cCustomSnippets.TabIndex = 2;
			this._cCustomSnippets.Text = "Use custom snippets";
			this._cCustomSnippets.UseVisualStyleBackColor = false;
			// 
			// _cComplParenSpace
			// 
			this._cComplParenSpace.AutoSize = true;
			this._cComplParenSpace.Location = new System.Drawing.Point(8, 20);
			this._cComplParenSpace.Name = "_cComplParenSpace";
			this._cComplParenSpace.Size = new System.Drawing.Size(188, 20);
			this._cComplParenSpace.TabIndex = 1;
			this._cComplParenSpace.Text = "Only spacebar adds () and <>";
			this._cComplParenSpace.UseVisualStyleBackColor = false;
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
			// _bApply
			// 
			this._bApply.Location = new System.Drawing.Point(416, 304);
			this._bApply.Name = "_bApply";
			this._bApply.Size = new System.Drawing.Size(72, 24);
			this._bApply.TabIndex = 3;
			this._bApply.Text = "&Apply";
			this._bApply.Click += new System.EventHandler(this._bOK_Click);
			// 
			// _cStringEnterRN
			// 
			this._cStringEnterRN.AutoSize = true;
			this._cStringEnterRN.Location = new System.Drawing.Point(8, 20);
			this._cStringEnterRN.Name = "_cStringEnterRN";
			this._cStringEnterRN.Size = new System.Drawing.Size(157, 20);
			this._cStringEnterRN.TabIndex = 0;
			this._cStringEnterRN.Text = "Enter in string adds \\r\\n";
			this._cStringEnterRN.UseVisualStyleBackColor = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._cStringEnterRN);
			this.groupBox2.Location = new System.Drawing.Point(8, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(472, 48);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Auto correction";
			// 
			// FOptions
			// 
			this.AcceptButton = this._bOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.CancelButton = this._bCancel;
			this.ClientSize = new System.Drawing.Size(498, 336);
			this.Controls.Add(this._bApply);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._bCancel);
			this.Controls.Add(this._bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FOptions";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.tabControl1.ResumeLayout(false);
			this._tabGeneral.ResumeLayout(false);
			this._tabGeneral.PerformLayout();
			this._tabTemplates.ResumeLayout(false);
			this._tabTemplates.PerformLayout();
			this._tabFont.ResumeLayout(false);
			this._pColor.ResumeLayout(false);
			this._pColor.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._nSat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._nHue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._nRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._nLum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._nBlue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._nGreen)).EndInit();
			this._pFont.ResumeLayout(false);
			this._pFont.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._nFontSize)).EndInit();
			this._tabCode.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

	}

	#endregion

	private Au.Controls.AuButtonOK _bOK;
	private Au.Controls.AuButtonCancel _bCancel;
	private System.Windows.Forms.TabControl tabControl1;
	private System.Windows.Forms.TabPage _tabGeneral;
	private System.Windows.Forms.TabPage _tabFont;
	private System.Windows.Forms.TabPage _tabFiles;
	private Au.Controls.AuCheckBox _cCheckVersion;
	private Au.Controls.AuCheckBox _cRunAtStartup;
	private System.Windows.Forms.TextBox _eStartupScripts;
	private Au.Controls.AuLabel label1;
	private System.Windows.Forms.ErrorProvider _errorProvider;
	private System.Windows.Forms.ToolTip _toolTip;
	private Au.Controls.AuCheckBox _cRunHidden;
	private System.Windows.Forms.NumericUpDown _nFontSize;
	private Au.Controls.AuLabel label3;
	private System.Windows.Forms.ComboBox _comboFont;
	private Au.Controls.AuLabel label2;
	private Au.Controls.AuButton _bApply;
	private Au.Controls.AuScintilla _sciStyles;
	private System.Windows.Forms.NumericUpDown _nLum;
	private System.Windows.Forms.NumericUpDown _nBlue;
	private System.Windows.Forms.NumericUpDown _nGreen;
	private System.Windows.Forms.NumericUpDown _nRed;
	private Au.Controls.AuLabel label8;
	private Au.Controls.AuLabel label7;
	private Au.Controls.AuLabel label6;
	private Au.Controls.AuLabel label5;
	private Au.Controls.AuLabel label4;
	private Au.Controls.AuCheckBox _cBold;
	private System.Windows.Forms.NumericUpDown _nSat;
	private System.Windows.Forms.NumericUpDown _nHue;
	private Au.Controls.AuLabel label10;
	private Au.Controls.AuLabel label9;
	private System.Windows.Forms.TextBox _eColor;
	private System.Windows.Forms.Panel _pColor;
	private System.Windows.Forms.Panel _pFont;
	private System.Windows.Forms.TabPage _tabCode;
	private System.Windows.Forms.GroupBox groupBox1;
	private Au.Controls.AuCheckBox _cComplParenSpace;
	private System.Windows.Forms.TabPage _tabTemplates;
	private Au.Tools.CodeBox _sciTemplate;
	private Au.Controls.AuLabel auLabel1;
	private Au.Controls.AuCheckBox _cCustomSnippets;
	private System.Windows.Forms.ComboBox _comboUseTemplate;
	private System.Windows.Forms.ComboBox _comboTemplate;
	private Au.Controls.AuLabel auLabel2;
	private System.Windows.Forms.GroupBox groupBox2;
	private Au.Controls.AuCheckBox _cStringEnterRN;
}
