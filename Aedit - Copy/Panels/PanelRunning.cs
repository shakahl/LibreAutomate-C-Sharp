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
//using System.Linq;

class PanelRunning : DockPanel
{
	AuTreeView _tv;
	bool _updatedOnce;

	public PanelRunning() {
		_tv = new AuTreeView { Name = "Running_list", ImageCache = App.ImageCache };
		//System.Windows.Automation.AutomationProperties.SetName(this, "Running");
		this.Children.Add(_tv);
	}

	public void ZUpdateList() {
		_tv.SetItems(App.Tasks.Items, _updatedOnce);
		if (!_updatedOnce) {
			_updatedOnce = true;
			FilesModel.NeedRedraw += v => { _tv.Redraw(v.remeasure); };
			_tv.ItemClick += _tv_ItemClick;
		}
	}

	private void _tv_ItemClick(object sender, TVItemEventArgs e) {
		if (e.ModifierKeys != 0 || e.ClickCount != 1) return;
		var t = e.Item as RunningTask;
		var f = t.f;
		switch (e.MouseButton) {
		case MouseButton.Left:
			App.Model.SetCurrentFile(f);
			break;
		case MouseButton.Right:
			_tv.Select(t);
			var name = f.DisplayName;
			var m = new AWpfMenu();
			m["End task  " + name] = o => App.Tasks.EndTask(t);
			m["End all  " + name] = o => App.Tasks.EndTasksOf(f);
			m.Separator();
			m["Close\tM-click"] = o => App.Model.CloseFile(f, true);
			if (null == Panels.Editor.ZGetOpenDocOf(f)) m.Last.IsEnabled = false;
			m.Show(_tv);
			break;
		case MouseButton.Middle:
			App.Model.CloseFile(f, true);
			break;
		}
	}
}
