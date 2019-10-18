using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class InteractiveControl :BindableControl
	{
		private bool _editEnabled = false;
		[DefaultValue(false)]
		public bool EditEnabled
		{
			get { return _editEnabled; }
			set { _editEnabled = value; }
		}

		protected bool IsEditEnabled(TreeNodeAdv node)
		{
			if(!EditEnabled) return false;
			if(IsEditEnabledValueNeeded == null) return true;
			return IsEditEnabledValueNeeded(node);
		}

		public Func<TreeNodeAdv, bool> IsEditEnabledValueNeeded;
	}
}
