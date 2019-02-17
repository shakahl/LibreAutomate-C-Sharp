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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Collections;

class PanelRecent :Control, ITreeModel
{
	_TreeViewAdv _c;
	NodeIcon _ccIcon;
	NodeTextBox _ccName;
	NodeTextBox _ccStatus;

	public PanelRecent()
	{
		_c = new _TreeViewAdv();
		_c.BorderStyle = BorderStyle.None;
		_c.Dock = DockStyle.Fill;
		_c.AccessibleName = this.Name = "Recent";

		_c.ShowLines = false;
		_c.ShowPlusMinus = false;
		_c.ShowNodeToolTips = true;
		_c.FullRowSelect = true;

		_ccIcon = new NodeIcon();
		_c.NodeControls.Add(_ccIcon);
		_ccIcon.ScaleMode = ImageScaleMode.ScaleUp;
		_ccIcon.DpiStretch = true;
		_ccIcon.ValueNeeded = node => (node.Tag as RunningTasks.RecentTask).f.GetIcon();

		_ccName = new NodeTextBox();
		_c.NodeControls.Add(_ccName);
		//_ccName.Trimming = StringTrimming.EllipsisCharacter;
		_ccName.ValueNeeded = node => (node.Tag as RunningTasks.RecentTask).f.DisplayName;

		_ccStatus = new NodeTextBox();
		_c.NodeControls.Add(_ccStatus);
		_ccStatus.ValueNeeded = node => (node.Tag as RunningTasks.RecentTask).running ? "(running)" : "";

		_c.NodeMouseClick += _c_NodeMouseClick;

		this.Controls.Add(_c);

	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	public void UpdateList()
	{
		if(_c.Model == null) _c.Model = this; else StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
	}

	#region ITreeModel

	public IEnumerable GetChildren(object nodeTag)
	{
		//Debug_.PrintFunc();
		Debug.Assert(nodeTag == null);
		return Tasks.Recent;
	}

	public bool IsLeaf(object nodeTag)
	{
		return true;
	}

#pragma warning disable 67
	public event EventHandler<TreeModelEventArgs> NodesChanged;
	public event EventHandler<TreeModelEventArgs> NodesInserted;
	public event EventHandler<TreeModelEventArgs> NodesRemoved;
	public event EventHandler<TreePathEventArgs> StructureChanged;
#pragma warning restore 67

	#endregion

	private void _c_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
	{
		if(e.ModifierKeys != 0) return;
		var t = e.Node.Tag as RunningTasks.RecentTask;
		var f = t.f;
		switch(e.Button) {
		case MouseButtons.Left:
			Model.SetCurrentFile(f);
			break;
		case MouseButtons.Middle:
			Model.CloseFile(f, true);
			break;
		}
	}

	class _TreeViewAdv :TreeViewAdv
	{
		public _TreeViewAdv()
		{
			this.SetStyle(ControlStyles.Selectable, false);
		}
	}
}
