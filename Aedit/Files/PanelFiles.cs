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

	FilesModel.FilesView _tv;

	public PanelFiles()
	{
		_tv = new FilesModel.FilesView { Name = "Files_list" };
		//System.Windows.Automation.AutomationProperties.SetName(this, "Files");
		this.Children.Add(_tv);
	}

	public FilesModel.FilesView TreeControl => _tv;


}
