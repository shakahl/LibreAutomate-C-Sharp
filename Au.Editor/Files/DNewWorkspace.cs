using System.Windows;
using System.Windows.Controls;
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

			var b = new wpfBuilder(this).WinSize(600).Columns(-3, 0, -1);
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
					SelectedPath = filesystem.exists(tLocation.Text).isDir ? tLocation.Text : folders.ThisAppDocuments,
					ShowNewFolderButton = true
				};
				if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK) tLocation.Text = d.SelectedPath;
			}

			string _Validate(FrameworkElement e) {
				var s = (e as TextBox).Text;
				if (e == tLocation) {
					if (!filesystem.exists(s).isDir) return "Folder does not exist";
				} else {
					if (pathname.isInvalidName(s)) return "Invalid filename";
					ResultPath = pathname.combine(tLocation.Text, s); //validation is when OK clicked
					if (filesystem.exists(ResultPath)) return s + " already exists";
				}
				return null;
			}

			//b.OkApply += e => {
			//	print.it(ResultPath); e.Cancel = true;
			//};
		}
	}
}
