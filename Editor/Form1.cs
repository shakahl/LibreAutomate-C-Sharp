using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ScintillaNET;

namespace Editor
{
	public partial class Form1 :Form
	{
		Scintilla editor;

		public Form1()
		{
			InitializeComponent();

			this.SuspendLayout();
			editor = new Scintilla();
			editor.Location = new Point(0, 30);
			editor.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 30);
			this.Controls.Add(editor);
			this.ResumeLayout(false);
		}
	}
}
