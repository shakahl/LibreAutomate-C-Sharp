using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
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
//using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;

public class FormWinapi : DialogForm
{
	public static void ZShowDialog(string name = null)
	{
		var f = new FormWinapi();
		f._eName.Text = name ?? Panels.Editor.ZActiveDoc.Z.SelectedText();
		f.Show(Program.MainForm);
	}
	ASqlite _db;

	void _Dispose()
	{
		_db?.Dispose();
		_db = null;
	}

	public FormWinapi()
	{
		InitializeComponent();

		_db = EdDatabases.OpenWinapi();

		_eName.TextChanged += _eName_TextChanged;
		_bOK.Click += (unu, sed) => {
			string s = _code.Text;
			if(!Empty(s)) Clipboard.SetText(s);
		};
	}

	private void _eName_TextChanged(object sender, EventArgs e)
	{
		string name = _eName.Text;
		var a = new List<(string name, string code)>();

		int nWild = 0; for(int i = 0; i < name.Length; i++) { switch(name[i]) { case '*': case '?': nWild++; break; } }
		if(name.Length > 0 && (nWild == 0 || name.Length - nWild >= 2)) {
			string sql;
			if(name.Contains(' ')) sql = $"SELECT * FROM api WHERE name in ('{string.Join("', '", name.RegexFindAll(@"\b[A-Za-z_]\w\w+", 0))}')";
			else if(name.FindAny("*?") >= 0) sql = $"SELECT * FROM api WHERE name GLOB '{name}'";
			else sql = $"SELECT * FROM api WHERE name = '{name}'";
			try {
				using var stat = _db.Statement(sql);
				//APerf.First();
				while(stat.Step()) a.Add((stat.GetText(0), stat.GetText(1)));
				//APerf.NW(); //30 ms cold, 10 ms warm. Without index.
			}
			catch(SLException ex) { ADebug.Print(ex.Message); }
		}

		string code = "";
		if(a.Count != 0) {
			code = a[0].code;
			if(a.Count > 1) code = string.Join(code.Starts("internal const") ? "\r\n" : "\r\n\r\n", a.Select(o => o.code));
			code += "\r\n";
		}
		_code.ZSetText(code);
	}

	private AuButtonOK _bOK;
	private AuButtonCancel _bCancel;
	private AuLabel auLabel1;
	private System.Windows.Forms.TextBox _eName;
	private Au.Tools.CodeBox _code;
	private ToolTip _toolTip;

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
		_Dispose();
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
		this.auLabel1 = new Au.Controls.AuLabel();
		this._eName = new System.Windows.Forms.TextBox();
		this._code = new Au.Tools.CodeBox();
		this._toolTip = new System.Windows.Forms.ToolTip(this.components);
		this.SuspendLayout();
		// 
		// _bOK
		// 
		this._bOK.Location = new System.Drawing.Point(8, 428);
		this._bOK.Name = "_bOK";
		this._bOK.Size = new System.Drawing.Size(152, 24);
		this._bOK.TabIndex = 3;
		this._bOK.Text = "&OK, copy to clipboard";
		// 
		// _bCancel
		// 
		this._bCancel.Location = new System.Drawing.Point(168, 428);
		this._bCancel.Name = "_bCancel";
		this._bCancel.Size = new System.Drawing.Size(72, 24);
		this._bCancel.TabIndex = 4;
		// 
		// auLabel1
		// 
		this.auLabel1.AutoSize = true;
		this.auLabel1.Location = new System.Drawing.Point(8, 8);
		this.auLabel1.Name = "auLabel1";
		this.auLabel1.Size = new System.Drawing.Size(39, 17);
		this.auLabel1.TabIndex = 0;
		this.auLabel1.Text = "Name";
		// 
		// _eName
		// 
		this._eName.Location = new System.Drawing.Point(56, 8);
		this._eName.Name = "_eName";
		this._eName.Size = new System.Drawing.Size(688, 23);
		this._eName.TabIndex = 1;
		this._toolTip.SetToolTip(this._eName,
			"Case-sensitive name of a function, struct, constant, interface, callback.\r\n" +
			"Use wildcard to specify partial name. Examples: Start*, *End, *AnyPart*.\r\n" +
			"Or text containing multiple full names. Example: Name1 Name2 Name3.");
		// 
		// _code
		// 
		this._code.Location = new System.Drawing.Point(8, 40);
		this._code.Name = "_code";
		this._code.Size = new System.Drawing.Size(736, 380);
		this._code.TabIndex = 2;
		this._code.ZInitReadOnlyAlways = true;
		// 
		// _toolTip
		// 
		this._toolTip.AutomaticDelay = 100;
		this._toolTip.AutoPopDelay = 10000;
		// 
		// FormWinapi
		// 
		this.AcceptButton = this._bOK;
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.CancelButton = this._bCancel;
		this.ClientSize = new System.Drawing.Size(751, 457);
		this.Controls.Add(this._code);
		this.Controls.Add(this._eName);
		this.Controls.Add(this.auLabel1);
		this.Controls.Add(this._bCancel);
		this.Controls.Add(this._bOK);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "FormWinapi";
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Find Windows API declaration";
		this.ResumeLayout(false);
		this.PerformLayout();

	}

	#endregion
}
