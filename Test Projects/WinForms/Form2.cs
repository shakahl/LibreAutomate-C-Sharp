using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms
{
	public partial class Form2 :Form
	{
		public Form2()
		{
			var f = new System.Drawing.Font("Segoe UI", 9F);
			this.Font = f;
			InitializeComponent();
			f.Dispose();
		}
	}
}
