//#define DESIGNER //makes the form class not nested, which allows to edit the form in the designer
//	But it shows a warning without a number. Tried to disable 3042, does not work.
//	Workaround: set GSMProp.resx Build Action = None.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using System.Xml.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
#if !DESIGNER
	partial class GStripManager
	{
#endif
		partial class GSMProp :Form
		{
			internal GSMProp()
			{
				InitializeComponent();
				this.toolTip1.InitialDelay = 50; //TextBox tooltip bug workaround
				this.StartPosition = FormStartPosition.CenterParent;

				_InitColors();

				textIcon.AllowDrop = true;
				textIcon.DragEnter += _TextIcon_DragEnter;
				textIcon.DragDrop += _TextIcon_DragDrop;
			}

			private void _TextIcon_DragEnter(object sender, DragEventArgs e)
			{
				//PrintFunc();
				if(e.Data.GetDataPresent(DataFormats.FileDrop, false)) e.Effect = DragDropEffects.Link;
				else if(e.Data.GetDataPresent(DataFormats.UnicodeText, false)) e.Effect = DragDropEffects.Copy;
			}

			private void _TextIcon_DragDrop(object sender, DragEventArgs e)
			{
				//PrintFunc();
				string s = null;
				if(e.Data.GetData(DataFormats.FileDrop, false) is string[] a && a.Length > 0) s = a[0];
				else s = e.Data.GetData(DataFormats.UnicodeText, false) as string;
				s = s?.Trim();
				if(!Empty(s)) textIcon.Text = s;
			}

			internal GroupBox groupStyle;
			internal RadioButton radioDefault;
			internal RadioButton radioIconAndText;
			internal RadioButton radioOnlyIcon;
			internal RadioButton radioOnlyText;
			private Label label1;
			internal TextBox textText;
			private Label label2;
			internal ComboBox comboColor;
			private Label label3;
			internal TextBox textIcon;
			private Button button1;
			private Button button2;
			private ToolTip toolTip1;

			void _InitColors()
			{
				PropertyInfo[] propInfoList = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
				foreach(PropertyInfo c in propInfoList) {
					comboColor.Items.Add(c.Name);
				}
				comboColor.DrawItem += _ComboColor_DrawItem;
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
				g.DrawString(n, f, b, r.X + 150, r.Top);
				b.Dispose();
			}

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
			this.groupStyle.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupStyle
			// 
			this.groupStyle.Controls.Add(this.radioIconAndText);
			this.groupStyle.Controls.Add(this.radioOnlyIcon);
			this.groupStyle.Controls.Add(this.radioDefault);
			this.groupStyle.Controls.Add(this.radioOnlyText);
			this.groupStyle.Location = new System.Drawing.Point(8, 8);
			this.groupStyle.Name = "groupStyle";
			this.groupStyle.Size = new System.Drawing.Size(448, 56);
			this.groupStyle.TabIndex = 0;
			this.groupStyle.TabStop = false;
			this.groupStyle.Text = "Style, when icon available";
			// 
			// radioIconAndText
			// 
			this.radioIconAndText.AutoSize = true;
			this.radioIconAndText.Location = new System.Drawing.Point(248, 24);
			this.radioIconAndText.Name = "radioIconAndText";
			this.radioIconAndText.Size = new System.Drawing.Size(93, 19);
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
			this.label1.Location = new System.Drawing.Point(8, 80);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Custom text";
			// 
			// textText
			// 
			this.textText.Location = new System.Drawing.Point(96, 80);
			this.textText.Name = "textText";
			this.textText.Size = new System.Drawing.Size(360, 23);
			this.textText.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "Text color";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "Custom icon";
			// 
			// textIcon
			// 
			this.textIcon.Location = new System.Drawing.Point(96, 144);
			this.textIcon.Name = "textIcon";
			this.textIcon.Size = new System.Drawing.Size(360, 23);
			this.textIcon.TabIndex = 6;
			this.toolTip1.SetToolTip(this.textIcon, "Icon file path like c:\\a\\b.ico or c:\\a\\b.dll,5\r\nYou can drag and drop a file here" +
        ".");
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(296, 192);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 24);
			this.button1.TabIndex = 7;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(384, 192);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(72, 24);
			this.button2.TabIndex = 8;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// comboColor
			// 
			this.comboColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboColor.FormattingEnabled = true;
			this.comboColor.Location = new System.Drawing.Point(96, 112);
			this.comboColor.Name = "comboColor";
			this.comboColor.Size = new System.Drawing.Size(360, 24);
			this.comboColor.TabIndex = 4;
			this.toolTip1.SetToolTip(this.comboColor, "Color name or 0xRRGGBB");
			// 
			// GSMProp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 225);
			this.Controls.Add(this.comboColor);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textIcon);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textText);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupStyle);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Name = "GSMProp";
			this.ShowInTaskbar = false;
			this.Text = "Properties";
			this.groupStyle.ResumeLayout(false);
			this.groupStyle.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

			#endregion
		}
#if !DESIGNER
	}
#endif
}
