using Au;
using Au.Types;
using Au.Controls;
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
using System.Windows.Input;
using Au.Util;
using System.Linq;

class PanelOpen : DockPanel
{
	AuTreeView _tv;
	bool _updatedOnce;

	public PanelOpen() {
		_tv = new AuTreeView { Name = "Open_list", ImageCache = App.ImageCache };
		this.Children.Add(_tv);
	}

	public void ZUpdateList() {
		//_tv.SetItems(App.Model.OpenFiles, _updatedOnce); //this would be ok, but displays yellow etc
		var a = App.Model.OpenFiles;
		_tv.SetItems(a.Select(o => new _Item { f = o }), _updatedOnce);
		if (a.Count > 0) _tv.SelectSingle(0, andFocus: true);
		if (!_updatedOnce) {
			_updatedOnce = true;
			FilesModel.NeedRedraw += v => { _tv.Redraw(v.remeasure); };
			_tv.ItemClick += _tv_ItemClick;
		}
	}

	private void _tv_ItemClick(object sender, TVItemEventArgs e) {
		if (e.ModifierKeys != 0 || e.ClickCount != 1) return;
		var f = (e.Item as _Item).f;
		switch (e.MouseButton) {
		case MouseButton.Left:
			App.Model.SetCurrentFile(f);
			break;
		case MouseButton.Right:
			_tv.Select(e.Item);
			switch (ClassicMenu_.ShowSimple("Close\tM-click|Close all other|Close all", this)) {
			case 1:
				_CloseFile();
				break;
			case 2:
				App.Model.CloseEtc(FilesModel.ECloseCmd.CloseAll, dontClose: f);
				break;
			case 3:
				App.Model.CloseEtc(FilesModel.ECloseCmd.CloseAll);
				break;
			}
			break;
		case MouseButton.Middle:
			_CloseFile();
			break;
		}

		void _CloseFile() {
			App.Model.CloseFile(f, selectOther: true);
		}
	}

	class _Item : ITreeViewItem
	{
		public FileNode f;

		#region ITreeViewItem

		string ITreeViewItem.DisplayText => (f as ITreeViewItem).DisplayText;

		string ITreeViewItem.ImageSource => (f as ITreeViewItem).ImageSource;

		#endregion
	}
}
