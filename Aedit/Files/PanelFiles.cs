using Au;
using Au.Controls;
using Au.Types;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
//using System.Linq;

partial class PanelFiles : DockPanel
{
	//idea: when file clicked, open it and show AMenu of its functions (if > 1).

	FilesModel.FilesView _c;
	FilesModel _model;

	public PanelFiles()
	{
		_c = new FilesModel.FilesView();
		//_c.AccessibleName = _c.Name = "Files_list"; //TODO
		//this.AccessibleName = this.Name = "Files";
		this.Children.Add(_c);
	}

	public FilesModel.FilesView ZControl => _c;

	public FilesModel ZModel => _model;

}
