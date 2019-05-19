#define DESIGNER //makes the form class not nested, which allows to edit the form in the designer
//	But it shows a warning without a number. Tried to disable 3042, does not work.
//	Workaround: set AuStripManagerPropertiesDialog.resx Build Action = None.
//	But then next time cannot edit form in designer. Workaround: briefly set build action =Compile, then set =None again. Default action is Embedded Resource.

using System;
using System.Collections.Generic;
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
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
#if !DESIGNER
	public partial class AuStripManager
	{
#endif
	class AuStripManagerPropertiesDialog : AFormBase
	{
		AuStripManager _man;
		XElement _x;
		bool _isMenu;
		List<XElement> _hotkeys;

		internal AuStripManagerPropertiesDialog(AuStripManager man, XElement x, bool isMenu)
		{
			_man = man;
			_x = x;
			_isMenu = isMenu;

			InitializeComponent();

			this.toolTip1.InitialDelay = 50; //TextBox tooltip bug workaround
			this.Text = (isMenu ? "Menu item " : "Button ") + x.Name;

			textText.Text = x.Attr("t2");
			comboColor.Text = x.Attr("color");
			textIcon.Text = x.Attr("i2");
			if(isMenu) {
				groupStyle.Enabled = false;
				textHotkey.Text = x.Attr("hk");
			} else {
				groupHotkey.Enabled = false;
				switch(x.Attr("style", 0)) {
				case 0: radioDefault.Checked = true; break;
				case 1: radioOnlyText.Checked = true; break;
				case 2: radioOnlyIcon.Checked = true; break;
				case 3: radioIconAndText.Checked = true; break;
				}
			}

			_InitColors();
			textIcon.AllowDrop = true;
			textIcon.DragEnter += _TextIcon_DragEnter;
			textIcon.DragDrop += _TextIcon_DragDrop;

			if(isMenu) _InitHotkeys();
		}

		#region color
		void _InitColors()
		{
			PropertyInfo[] propInfoList = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
			foreach(PropertyInfo c in propInfoList) {
				comboColor.Items.Add(c.Name);
			}
			comboColor.DrawItem += _ComboColor_DrawItem;
			comboColor.Validating += _ComboColor_Validating;
			comboColor.TextChanged += (unu, sed) => _errorProvider?.Clear();
		}

		private void _ComboColor_Validating(object sender, CancelEventArgs e)
		{
			_errorProvider?.Clear();
			var s = comboColor.Text.Trim();
			if(Empty(s) || ColorInt.FromString(s, out _)) return;
			e.Cancel = true;
			_SetError(comboColor, "Invalid color name");
		}

		private void _ComboColor_DrawItem(object sender, DrawItemEventArgs e)
		{
			if(e.Index < 0) return;
			Graphics g = e.Graphics;
			Rectangle r = e.Bounds;
			string n = ((ComboBox)sender).Items[e.Index].ToString();
			Font f = this.Font;
			Color c = Color.FromName(n);
			Brush b = new SolidBrush(c);
			g.FillRectangle(Brushes.White, r);
			g.DrawString(n, f, Brushes.Black, r.X, r.Top);
			g.DrawString("Color", f, b, r.X + r.Width / 2, r.Top);
			b.Dispose();
		}
		#endregion

		#region drag-drop
		private void _TextIcon_DragEnter(object sender, DragEventArgs e)
		{
			//ADebug.PrintFunc();
			if(e.Data.GetDataPresent(DataFormats.FileDrop, false)) e.Effect = DragDropEffects.Link;
			else if(e.Data.GetDataPresent(DataFormats.UnicodeText, false)) e.Effect = DragDropEffects.Copy;
		}

		private void _TextIcon_DragDrop(object sender, DragEventArgs e)
		{
			//ADebug.PrintFunc();
			string s = null;
			if(e.Data.GetData(DataFormats.FileDrop, false) is string[] a && a.Length > 0) s = a[0];
			else s = e.Data.GetData(DataFormats.UnicodeText, false) as string;
			s = s?.Trim();
			if(!Empty(s)) textIcon.Text = s;
		}
		#endregion

		#region hotkey
		void _InitHotkeys()
		{
			var xMenu = _man.MenuBar.Tag as XElement;
			_hotkeys = xMenu.Descendants().Where(t => t.HasAttr("hk")).ToList();
			_hotkeys.Sort((x1, x2) => string.CompareOrdinal(x1.Attr("hk"), x2.Attr("hk")));

			foreach(var x in _hotkeys) {
				var s = x.Attr("hk");
				comboUsedHotkeys.Items.Add($"{s,-24} {x.Name}");
			}
			comboUsedHotkeys.SelectionChangeCommitted += _ComboUsedHotkeys_SelectionChangeCommitted;

			textHotkey.Validating += _TextHotkey_Validating;
			textHotkey.TextChanged += _TextHotkey_TextChanged;
		}

		//prevents user selecting an item because it makes no sense etc
		private void _ComboUsedHotkeys_SelectionChangeCommitted(object sender, EventArgs e)
		{
			_TextHotkey_TextChanged(null, e);
		}

		//selects used hotkey in comboUsedHotkeys
		private void _TextHotkey_TextChanged(object sender, EventArgs e)
		{
			int iSel = -1;
			var s = textHotkey.Text.Trim();
			if(AKeyboard.More.ParseHotkeyString(s, out var hk)) {
				var x = FindUsedHotkey(hk, _x);
				if(x != null) iSel = _hotkeys.IndexOf(x);
			}
			comboUsedHotkeys.SelectedIndex = iSel;
			_errorProvider?.Clear();
		}

		private void _TextHotkey_Validating(object sender, CancelEventArgs e)
		{
			var s = textHotkey.Text.Trim();
			if(Empty(s)) return;
			bool ok = true;
			if(!AKeyboard.More.ParseHotkeyString(s, out var mod, out var k) || mod.Has(KMod.Win)) ok = false;
			else if(!mod.HasAny(KMod.Ctrl | KMod.Alt)) ok = (k >= KKey.F2 && k <= KKey.F24);

			if(!ok) {
				e.Cancel = true;
				_SetError(textHotkey, "Invalid hotkey.\nHotkey examples:\nF12, Ctrl+D, Ctrl+PageUp, Ctrl+Alt+Shift+[");
			}
		}

		internal XElement FindUsedHotkey(Keys hk, XElement xSkip)
		{
			if(_hotkeys != null) {
				foreach(var x in _hotkeys) {
					if(x == xSkip) continue;
					var s = x.Attr("hk");
					if(AKeyboard.More.ParseHotkeyString(s, out var k) && k == hk) return x;
				}
			}
			return null;
		}
		#endregion

		void _SetError(Control c, string s)
		{
			if(_errorProvider == null) _errorProvider = new ErrorProvider(components);
			_errorProvider.SetIconAlignment(c, ErrorIconAlignment.BottomLeft);
			_errorProvider.SetError(c, s);
		}
		ErrorProvider _errorProvider;

		private Label label1;
		internal TextBox textText;
		private Label label2;
		internal ComboBox comboColor;
		private Label label3;
		internal TextBox textIcon;
		private GroupBox groupStyle;
		internal RadioButton radioDefault;
		internal RadioButton radioIconAndText;
		internal RadioButton radioOnlyIcon;
		internal RadioButton radioOnlyText;
		private GroupBox groupHotkey;
		private Label label4;
		internal TextBox textHotkey;
		private Label label6;
		private ComboBox comboUsedHotkeys;
		private Button button1;
		private Button button2;
		private ToolTip toolTip1;

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
			this.groupStyle = new System.Windows.Forms.GroupBox();
			this.radioIconAndText = new System.Windows.Forms.RadioButton();
			this.radioOnlyIcon = new System.Windows.Forms.RadioButton();
			this.radioDefault = new System.Windows.Forms.RadioButton();
			this.radioOnlyText = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.textText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textIcon = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.comboColor = new System.Windows.Forms.ComboBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textHotkey = new System.Windows.Forms.TextBox();
			this.comboUsedHotkeys = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupHotkey = new System.Windows.Forms.GroupBox();
			this.groupStyle.SuspendLayout();
			this.groupHotkey.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupStyle
			// 
			this.groupStyle.Controls.Add(this.radioIconAndText);
			this.groupStyle.Controls.Add(this.radioOnlyIcon);
			this.groupStyle.Controls.Add(this.radioDefault);
			this.groupStyle.Controls.Add(this.radioOnlyText);
			this.groupStyle.Location = new System.Drawing.Point(8, 112);
			this.groupStyle.Name = "groupStyle";
			this.groupStyle.Size = new System.Drawing.Size(488, 48);
			this.groupStyle.TabIndex = 6;
			this.groupStyle.TabStop = false;
			this.groupStyle.Text = "Button style, when icon available";
			// 
			// radioIconAndText
			// 
			this.radioIconAndText.AutoSize = true;
			this.radioIconAndText.Location = new System.Drawing.Point(248, 24);
			this.radioIconAndText.Name = "radioIconAndText";
			this.radioIconAndText.Size = new System.Drawing.Size(94, 19);
			this.radioIconAndText.TabIndex = 3;
			this.radioIconAndText.Text = "Icon and text";
			this.radioIconAndText.UseVisualStyleBackColor = true;
			// 
			// radioOnlyIcon
			// 
			this.radioOnlyIcon.AutoSize = true;
			this.radioOnlyIcon.Location = new System.Drawing.Point(168, 24);
			this.radioOnlyIcon.Name = "radioOnlyIcon";
			this.radioOnlyIcon.Size = new System.Drawing.Size(48, 19);
			this.radioOnlyIcon.TabIndex = 2;
			this.radioOnlyIcon.Text = "Icon";
			this.radioOnlyIcon.UseVisualStyleBackColor = true;
			// 
			// radioDefault
			// 
			this.radioDefault.AutoSize = true;
			this.radioDefault.Checked = true;
			this.radioDefault.Location = new System.Drawing.Point(8, 24);
			this.radioDefault.Name = "radioDefault";
			this.radioDefault.Size = new System.Drawing.Size(63, 19);
			this.radioDefault.TabIndex = 0;
			this.radioDefault.TabStop = true;
			this.radioDefault.Text = "Default";
			this.radioDefault.UseVisualStyleBackColor = true;
			// 
			// radioOnlyText
			// 
			this.radioOnlyText.AutoSize = true;
			this.radioOnlyText.Location = new System.Drawing.Point(88, 24);
			this.radioOnlyText.Name = "radioOnlyText";
			this.radioOnlyText.Size = new System.Drawing.Size(46, 19);
			this.radioOnlyText.TabIndex = 1;
			this.radioOnlyText.Text = "Text";
			this.radioOnlyText.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Custom text";
			// 
			// textText
			// 
			this.textText.Location = new System.Drawing.Point(104, 8);
			this.textText.Name = "textText";
			this.textText.Size = new System.Drawing.Size(384, 23);
			this.textText.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Text color";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 15);
			this.label3.TabIndex = 4;
			this.label3.Text = "Custom icon";
			// 
			// textIcon
			// 
			this.textIcon.Location = new System.Drawing.Point(104, 72);
			this.textIcon.Name = "textIcon";
			this.textIcon.Size = new System.Drawing.Size(384, 23);
			this.textIcon.TabIndex = 5;
			this.toolTip1.SetToolTip(this.textIcon, "Icon file path like c:\\a\\b.ico or c:\\a\\b.dll,5\r\nYou can drag and drop a file here" +
        ".");
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(336, 280);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(72, 24);
			this.button1.TabIndex = 8;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.CausesValidation = false;
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(416, 280);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(72, 24);
			this.button2.TabIndex = 9;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// comboColor
			// 
			this.comboColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboColor.FormattingEnabled = true;
			this.comboColor.Location = new System.Drawing.Point(104, 40);
			this.comboColor.Name = "comboColor";
			this.comboColor.Size = new System.Drawing.Size(384, 24);
			this.comboColor.TabIndex = 3;
			this.toolTip1.SetToolTip(this.comboColor, "Color name or 0xRRGGBB or #RRGGBB");
			// 
			// textHotkey
			// 
			this.textHotkey.Location = new System.Drawing.Point(96, 24);
			this.textHotkey.Name = "textHotkey";
			this.textHotkey.Size = new System.Drawing.Size(384, 23);
			this.textHotkey.TabIndex = 1;
			this.toolTip1.SetToolTip(this.textHotkey, "Hotkey, eg Ctrl+H");
			// 
			// comboUsedHotkeys
			// 
			this.comboUsedHotkeys.CausesValidation = false;
			this.comboUsedHotkeys.DropDownHeight = 400;
			this.comboUsedHotkeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUsedHotkeys.Font = new System.Drawing.Font("Courier New", 8F);
			this.comboUsedHotkeys.IntegralHeight = false;
			this.comboUsedHotkeys.Location = new System.Drawing.Point(96, 56);
			this.comboUsedHotkeys.MaxDropDownItems = 20;
			this.comboUsedHotkeys.Name = "comboUsedHotkeys";
			this.comboUsedHotkeys.Size = new System.Drawing.Size(384, 22);
			this.comboUsedHotkeys.TabIndex = 4;
			this.toolTip1.SetToolTip(this.comboUsedHotkeys, "This is just for information");
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 15);
			this.label4.TabIndex = 0;
			this.label4.Text = "Hotkey";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 56);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(76, 15);
			this.label6.TabIndex = 3;
			this.label6.Text = "Now used by";
			// 
			// groupHotkey
			// 
			this.groupHotkey.Controls.Add(this.textHotkey);
			this.groupHotkey.Controls.Add(this.label6);
			this.groupHotkey.Controls.Add(this.label4);
			this.groupHotkey.Controls.Add(this.comboUsedHotkeys);
			this.groupHotkey.Location = new System.Drawing.Point(8, 176);
			this.groupHotkey.Name = "groupHotkey";
			this.groupHotkey.Size = new System.Drawing.Size(488, 88);
			this.groupHotkey.TabIndex = 7;
			this.groupHotkey.TabStop = false;
			this.groupHotkey.Text = "Menu item hotkey";
			// 
			// AuStripManagerPropertiesDialog
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(503, 313);
			this.Controls.Add(this.groupHotkey);
			this.Controls.Add(this.comboColor);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textIcon);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textText);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupStyle);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AuStripManagerPropertiesDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.groupStyle.ResumeLayout(false);
			this.groupStyle.PerformLayout();
			this.groupHotkey.ResumeLayout(false);
			this.groupHotkey.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
#if !DESIGNER
	}
#endif
}
