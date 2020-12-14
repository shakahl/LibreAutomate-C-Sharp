using Au;
using Au.Controls;
using Au.Types;
using Au.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
//using System.Linq;

partial class PanelFiles : DockPanel
{
	FilesModel.FilesView _tv;

	public PanelFiles() {
		_tv = new() { Name = "Files_list" };
		this.Children.Add(_tv);
	}

	public FilesModel.FilesView TreeControl => _tv;
}
