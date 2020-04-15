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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Collections;

class PanelRunning : AuUserControlBase, ITreeModel
{
	_TreeViewAdv _c;
	NodeIcon _ccIcon;
	NodeTextBox _ccName;

	public PanelRunning()
	{
		this.AccessibleName = this.Name = "Running";
		_c = new _TreeViewAdv();
		_c.AccessibleName = _c.Name = "Running_list";
		_c.BorderStyle = BorderStyle.None;
		_c.Dock = DockStyle.Fill;

		_c.ShowLines = false;
		_c.ShowPlusMinus = false;
		_c.ShowNodeToolTips = true;
		_c.FullRowSelect = true;

		_ccIcon = new NodeIcon();
		_c.NodeControls.Add(_ccIcon);
		_ccIcon.ScaleMode = ImageScaleMode.ScaleUp;
		_ccIcon.DpiStretch = true;
		_ccIcon.ValueNeeded = node => (node.Tag as RunningTask).f.GetIcon();

		_ccName = new NodeTextBox();
		_c.NodeControls.Add(_ccName);
		//_ccName.Trimming = StringTrimming.EllipsisCharacter;
		_ccName.ValueNeeded = node => (node.Tag as RunningTask).f.DisplayName;
		_ccName.DrawText += _ccName_DrawText;

		_c.NodeMouseClick += _c_NodeMouseClick;

		this.Controls.Add(_c);
	}

	private void _ccName_DrawText(object sender, DrawEventArgs e)
	{
		var t = e.Node.Tag as RunningTask;
		e.TextColor = t.isBlue ? Color.Blue : Color.Green;
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	public void ZUpdateList()
	{
		if(_c.Model == null) _c.Model = this; else StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
	}

	#region ITreeModel

	public IEnumerable GetChildren(object nodeTag)
	{
		//ADebug.PrintFunc();
		Debug.Assert(nodeTag == null);
		return Program.Tasks.Items;
		//FUTURE: also add folder with queed items.
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
		var t = e.Node.Tag as RunningTask;
		var f = t.f;
		switch(e.Button) {
		case MouseButtons.Left:
			Program.Model.SetCurrentFile(f);
			break;
		case MouseButtons.Right:
			var name = f.DisplayName;
			var m = new AMenu();
			m["End task  " + name] = o => Program.Tasks.EndTask(t);
			m["End all  " + name] = o => Program.Tasks.EndTasksOf(f);
			m.Separator();
			m["Close\tM-click"] = o => Program.Model.CloseFile(f, true);
			if(null == Panels.Editor.ZGetOpenDocOf(f)) m.LastMenuItem.Enabled = false;
			m.Show(_c);
			break;
		case MouseButtons.Middle:
			Program.Model.CloseFile(f, true);
			break;
		}
	}

	class _TreeViewAdv : TreeViewAdv
	{
		public _TreeViewAdv()
		{
			this.SetStyle(ControlStyles.Selectable, false);
		}
	}
}
