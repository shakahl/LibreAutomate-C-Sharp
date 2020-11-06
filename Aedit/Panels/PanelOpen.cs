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
using System.Linq;

using Au;
using Au.Types;
using Au.Controls;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Collections;

class PanelOpen : AuUserControlBase, ITreeModel
{
	_TreeViewAdv _c;
	NodeIcon _ccIcon;
	NodeTextBox _ccName;

	public PanelOpen()
	{
		this.AccessibleName = this.Name = "Open";
		_c = new _TreeViewAdv();
		_c.AccessibleName = _c.Name = "Open_list";
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
		_ccIcon.ValueNeeded = node => (node.Tag as FileNode).GetIcon();

		_ccName = new NodeTextBox();
		_c.NodeControls.Add(_ccName);
		//_ccName.Trimming = StringTrimming.EllipsisCharacter;
		_ccName.ValueNeeded = node => (node.Tag as FileNode).DisplayName;

		_c.NodeMouseClick += _c_NodeMouseClick;
		_c.NodeMouseDoubleClick += _c_NodeMouseClick;

		this.Controls.Add(_c);
	}

	protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	public void ZUpdateList()
	{
		//ADebug.PrintFunc();
		bool cmdPrevDisable = (App.Model?.OpenFiles.Count ?? 0) < 2;
		if(cmdPrevDisable != _cmdPrevDisabled) {
			_cmdPrevDisabled = cmdPrevDisable;
			App.Commands[nameof(Menus.File.OpenClose.Previous_document)].Enabled = !cmdPrevDisable;
		}
		if(_c.Model == null) _c.Model = this; else StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
	}
	bool _cmdPrevDisabled;

	public void ZUpdateCurrent(FileNode fn)
	{
		//ADebug.PrintFunc();
		if(fn == null) _c.ClearSelection();
		else _c.SelectedNode = _c.FindNodeByTag(fn);
	}

	#region ITreeModel

	public IEnumerable GetChildren(object nodeTag) => App.Model?.OpenFiles;

	public bool IsLeaf(object nodeTag) => true;

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
		var f = e.Node.Tag as FileNode;
		switch(e.Button) {
		case MouseButtons.Left:
			App.Model.SetCurrentFile(f);
			break;
		case MouseButtons.Right:
			var m = new AMenu();
			m["Close\tM-click"] = o => App.Model.CloseFile(f, true);
			using(m.Submenu("Multiple")) {
				m["Close all other"]=o => App.Model.CloseEtc(FilesModel.ECloseCmd.CloseAll, dontClose: f);
				m["Close all"]=o => App.Model.CloseEtc(FilesModel.ECloseCmd.CloseAll);
			}
			m.Show(_c);
			break;
		case MouseButtons.Middle:
			App.Model.CloseFile(f, true);
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
