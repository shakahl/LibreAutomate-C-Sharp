using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Aga.Controls.Tree
{
	public interface ITreeModel
	{
		//au:
		IEnumerable GetChildren(object nodeTag);
		bool IsLeaf(object nodeTag);
		//IEnumerable GetChildren(TreePath treePath);
		//bool IsLeaf(TreePath treePath);

		event EventHandler<TreeModelEventArgs> NodesChanged;
		event EventHandler<TreeModelEventArgs> NodesInserted;
		event EventHandler<TreeModelEventArgs> NodesRemoved;
		event EventHandler<TreePathEventArgs> StructureChanged;
	}
}
