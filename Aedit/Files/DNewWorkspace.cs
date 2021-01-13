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
using System.Windows;
using System.Windows.Controls;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

partial class FilesModel
{
	class DNewWorkspace : KDialogWindow
	{
		string _name, _location;
		public string ResultPath { get; private set; }

		public DNewWorkspace(string name, string location) {
			_name = name;
			_location = location;

			Title = "New Workspace";

			var b = new AWpfBuilder(this).WinSize(600).Columns(-3, 0, -1);
			b.WinProperties(WindowStartupLocation.CenterOwner, showInTaskbar: false);
			b.R.Add<Label>("Parent folder").Skip().Add<Label>("Name");
			b.R.Add(out TextBox tLocation, _location).Validation(_Validate)
				.Add<Label>("\\")
				.Add(out TextBox tName, _name).Validation(_Validate);
			b.R.AddButton("Browse...", _Browse).Width(70).Align("L");
			b.R.AddOkCancel();
			b.End();

			void _Browse(WBButtonClickArgs e) {
				using var d = new System.Windows.Forms.FolderBrowserDialog {
					SelectedPath = AFile.ExistsAsDirectory(tLocation.Text) ? tLocation.Text : AFolders.ThisAppDocuments,
					ShowNewFolderButton = true
				};
				if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK) tLocation.Text = d.SelectedPath;
			}

			string _Validate(FrameworkElement e) {
				var s = (e as TextBox).Text;
				if (e == tLocation) {
					if (!AFile.ExistsAsDirectory(s)) return "Folder does not exist";
				} else {
					if (APath.IsInvalidName(s)) return "Invalid filename";
					ResultPath = APath.Combine(tLocation.Text, s); //validation is when OK clicked
					if (AFile.ExistsAsAny(ResultPath)) return s + " already exists";
				}
				return null;
			}

			//b.OkApply += e => {
			//	AOutput.Write(ResultPath); e.Cancel = true;
			//};
		}
	}
}
