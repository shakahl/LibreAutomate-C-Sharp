//#define DESIGNER //makes the form class not nested, which allows to edit the form in the designer

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
//using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;

#if !DESIGNER
partial class FilesModel
{
#endif
	class FNewWorkspace :DialogForm
	{
		public FNewWorkspace()
		{
			InitializeComponent();

			textName.TextChanged += _TextChanged;
			textLocation.TextChanged += _TextChanged;
			buttonOK.Click += _ButtonOK_Click;
			buttonBrowse.Click += _ButtonBrowse_Click;
		}

		private void _ButtonBrowse_Click(object sender, EventArgs e)
		{
			var d = new FolderBrowserDialog();
			d.Description = "Location. In the selected folder will be created the main folder of the workspace.";
			d.ShowNewFolderButton = true;
			d.SelectedPath = AFile.ExistsAsDirectory(textLocation.Text) ? textLocation.Text : (string)AFolders.ThisAppDocuments;
			if(d.ShowDialog(this) != DialogResult.OK) return;
			textLocation.Text = d.SelectedPath;
		}

		private void _ButtonOK_Click(object sender, EventArgs e)
		{
			var ok = true;
			var path = textPath.Text;
			if(!APath.IsFullPath(path)) ok = false;
			else if(AFile.ExistsAsAny(path)) {
				ADialog.ShowError("Already exists", path, owner: this);
				ok = false;
			}
			this.DialogResult = ok ? DialogResult.OK : DialogResult.None;
		}

		private void _TextChanged(object sender, EventArgs e)
		{
			var location = textLocation.Text.Trim();
			var name = textName.Text.Trim();
			string path = null;
			if(location.Length > 0 && name.Length > 0) {
				name = APath.CorrectFileName(name);
				path = APath.Combine(location, name);
				try { path = APath.Normalize(path); } catch { path = null; }
			}
			textPath.Text = path;
			buttonOK.Enabled = path != null;
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonOK = new Au.Controls.AuButtonOK();
			this.buttonCancel = new Au.Controls.AuButtonCancel();
			this.label1 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textLocation = new System.Windows.Forms.TextBox();
			this.buttonBrowse = new Au.Controls.AuButton();
			this.label3 = new System.Windows.Forms.Label();
			this.textPath = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(416, 112);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(72, 24);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(496, 112);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(72, 24);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name";
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(64, 8);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(176, 23);
			this.textName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Location";
			// 
			// textLocation
			// 
			this.textLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textLocation.Location = new System.Drawing.Point(64, 40);
			this.textLocation.Name = "textLocation";
			this.textLocation.Size = new System.Drawing.Size(424, 23);
			this.textLocation.TabIndex = 3;
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrowse.Location = new System.Drawing.Point(496, 40);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(72, 24);
			this.buttonBrowse.TabIndex = 4;
			this.buttonBrowse.Text = "Browse...";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(25, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "Path";
			// 
			// textFile
			// 
			this.textPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textPath.CausesValidation = false;
			this.textPath.Location = new System.Drawing.Point(64, 72);
			this.textPath.Name = "textFile";
			this.textPath.ReadOnly = true;
			this.textPath.Size = new System.Drawing.Size(504, 23);
			this.textPath.TabIndex = 6;
			this.textPath.TabStop = false;
			// 
			// _FormNewWorkspace
			// 
			this.AcceptButton = this.buttonOK;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(579, 145);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonBrowse);
			this.Controls.Add(this.textLocation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "_FormNewWorkspace";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Workspace";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private AuButtonOK buttonOK;
		private AuButtonCancel buttonCancel;
		private Label label1;
		public TextBox textName;
		private Label label2;
		public TextBox textLocation;
		private Button buttonBrowse;
		private Label label3;
		public TextBox textPath;
	}
#if !DESIGNER
}
#endif
