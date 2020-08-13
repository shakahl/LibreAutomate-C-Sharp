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
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Au;
using Au.Types;
using System.Windows.Input;

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		class _Stack : _Node
		{
			Grid _grid = new();
			List<_Node> _nodes = new();

			public _Stack(AuPanels pm, _Stack parent, XElement x) : base(pm, parent, x) {
				pm._rootStack ??= this;
				pm._aStack.Add(this);

				IsVertical = x.Attr("o") == "v";

				foreach (var e in x.Elements()) {
					_Node node = e.Name.LocalName switch
					{
						"stack" => new _Stack(pm, this, e),
						"tab" => new _Tab(pm, this, e),
						_ => new _Panel(pm, this, e),
					};
					node.LoadAttributesAndAddToStack_(e, _nodes.Count);
					_nodes.Add(node);
				}
				Debug.Assert(_nodes.Count >= 2); //see _AutoUpdateXml
			}

			public override FrameworkElement Elem => _grid;
			public bool IsVertical { get; }

			public override void Save(XmlWriter x) {
				x.WriteStartElement("stack");
				x.WriteAttributeString("o", IsVertical ? "v" : "h");
				base._SaveAttributes(x);
				foreach (var v in _nodes) v.Save(x);
				x.WriteEndElement();
			}
		}
	}
}
