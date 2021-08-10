using System.Windows.Controls;

partial class PanelFiles : DockPanel
{
	FilesModel.FilesView _tv;

	public PanelFiles() {
		_tv = new() { Name = "Files_list" };
		this.Children.Add(_tv);
	}

	public FilesModel.FilesView TreeControl => _tv;
}
