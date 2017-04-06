/exe
PF
CsScript x.Init
x.SetOptions("references=System.Core")
PN
x.AddCode("")
PN
rep 1
	x.Main()
	PN
PO

 BEGIN PROJECT
 END PROJECT

#ret
//C# code
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

public class Form1 : Form
{
    private System.Windows.Forms.ComboBox lstColors;

  public Form1() {
        InitializeComponent();
        string[] colorNames;
        colorNames = System.Enum.GetNames(typeof(KnownColor));
        lstColors.Items.AddRange(colorNames);
  }

    private void InitializeComponent()
    {
        this.lstColors = new System.Windows.Forms.ComboBox();
        this.SuspendLayout();

        this.lstColors.AutoCompleteMode = ((System.Windows.Forms.AutoCompleteMode)((System.Windows.Forms.AutoCompleteMode.Suggest | System.Windows.Forms.AutoCompleteMode.Append)));
        this.lstColors.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
        this.lstColors.FormattingEnabled = true;
        this.lstColors.Location = new System.Drawing.Point(13, 13);
        this.lstColors.Name = "lstColors";
        this.lstColors.Size = new System.Drawing.Size(267, 21);
        this.lstColors.TabIndex = 0;

        this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
        this.ClientSize = new System.Drawing.Size(296, 82);
        this.Controls.Add(this.lstColors);
        this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.Name = "Form1";
        this.Text = "AutoComplete";
        this.ResumeLayout(false);
    }

  [STAThread]
  static void Main()
  {
    Application.EnableVisualStyles();
    Application.Run(new Form1());
  }
}