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

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		class _Panel : _Dragable, IPanel
		{
			Grid _grid = new();
			FrameworkElement _content;

			public _Panel(AuPanels pm, _Node parent, XElement x) : base(pm, parent, x) {
				pm._aPanel.Add(this);

				switch (x.Name.LocalName) {
				case "panel": break;
				case "toolbar": IsToolbar = true; break;
				case "document": IsDocument = true; Name = "Document"; break;
				default: throw new ArgumentException("unknown XML tag");
				}
				Name ??= x.Attr("name") ?? throw new ArgumentException("no panel name in XML");

			}

			public string Name { get; }

			public override FrameworkElement Elem => _grid;
			public override bool IsToolbar { get; }
			public override bool IsDocument { get; }

			public FrameworkElement Content {
				get => _content;
				set {
					_content = value;
					_grid.Children.Add(_content);
					_SetContentCell();
				}
			}

			void _SetContentCell() {
				Grid.SetColumn(_content, CaptionAt == Dock.Left ? 1 : 0);
				Grid.SetRow(_content, CaptionAt == Dock.Top ? 1 : 0);
			}

			public override void Save(XmlWriter x) {
				x.WriteStartElement(IsToolbar ? "toolbar" : (IsDocument ? "document" : "panel"));
				if(!IsDocument) x.WriteAttributeString("name", this.Name);
				base._SaveAttributes(x);
				x.WriteEndElement();
			}
		}

		public interface IPanel
		{
			FrameworkElement Content { get; set; }
		}
	}
}
