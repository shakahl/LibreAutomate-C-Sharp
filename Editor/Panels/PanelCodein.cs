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
using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Controls;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Collections;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Tags;

class PanelCodein : AuUserControlBase
{
	//AuListControl _list;

	//public AuListControl Control => _list;

	public PanelCodein()
	{
		this.AccessibleName = this.Name = "Codein";
		//_list = new AuListControl();
		//_list.AccessibleName = _list.Name = "Codein_list";
		////_list.BorderStyle = BorderStyle.None;
		//_list.Dock = DockStyle.Fill;
		//_list.ItemClick += _list_ItemClick;
		//this.Controls.Add(_list);
	}
}
