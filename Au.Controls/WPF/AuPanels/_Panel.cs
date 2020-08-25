using Au.Types;
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
using System.Windows.Shapes;

namespace Au.Controls.WPF
{
	public partial class AuPanels
	{
		class _Panel : _Dragable, IPanel
		{
			DockPanel _panel = new();
			FrameworkElement _caption; //TextBlock if panel, Rectangle if toolbar, null if document or in tab 
			FrameworkElement _content;

			public _Panel(AuPanels pm, _Node parent, XElement x) : base(pm, parent, x) {
				pm._aPanel.Add(this);

				switch (x.Name.LocalName) {
				case "panel": IsPanel = true; break;
				case "toolbar": IsToolbar = true; break;
				case "document": IsDocument = true; Name = "Document"; break;
				default: throw new ArgumentException("unknown XML tag");
				}
				Name ??= x.Attr("name") ?? throw new ArgumentException("no panel name in XML");
				AddRemoveCaption();
			}

			public string Name { get; }

			public override FrameworkElement Elem => _panel;
			public override bool IsToolbar { get; }
			public override bool IsDocument { get; }
			public override bool IsPanel { get; }

			public FrameworkElement Content {
				get => _content;
				set {
					if (_content != null) _panel.Children.Remove(_content);
					_content = value;
					_panel.Children.Add(_content);
				}
			}

			public override Dock CaptionAt {
				set {
					var old = base.CaptionAt;
					base.CaptionAt = value;
					if (_caption != null) _SetCaptionAt(old);
				}
			}

			void _SetCaptionAt(Dock old) {
				var ca = base.CaptionAt;
				DockPanel.SetDock(_caption, ca);
				if (ca == old || !IsPanel) return;
				if (ca == Dock.Top || ca == Dock.Bottom) {
					if (old == Dock.Left || old == Dock.Right) _caption.LayoutTransform = null;
				} else {
					_caption.LayoutTransform = new RotateTransform(ca == Dock.Left ? 270d : 90d);
				}
			}

			public void AddRemoveCaption() {
				if ((_parent is _Tab && !IsFloating) || IsDocument) {
					if (_caption != null) {
						_panel.Children.Remove(_caption);
						_caption = null;
					}
				} else {
					if (_caption == null) {
						if (IsToolbar) {
							var v = new Rectangle {
								MinHeight = 8,
								MinWidth = 8,
								Fill = Brushes.Transparent //note: without Fill there are no events
							};
							v.MouseEnter += (_, _) => v.Fill = s_captionBrush;
							v.MouseLeave += (_, _) => v.Fill = Brushes.Transparent;
							_caption = v;
						} else {
							_caption = new TextBlock {
								Text = Name,
								TextAlignment = TextAlignment.Center,
								Padding = new Thickness(2, 1, 2, 2),
								Background = s_captionBrush,
								TextTrimming = TextTrimming.CharacterEllipsis
							};
						}
						_panel.Children.Insert(0, _caption);
						_SetCaptionAt(old: Dock.Top);
						_caption.ContextMenuOpening += _ContextMenu;
						_caption.MouseDown += _OnMouseDown;
					}
				}
			}

			public override void Save(XmlWriter x) {
				x.WriteStartElement(IsToolbar ? "toolbar" : (IsDocument ? "document" : "panel"));
				if (!IsDocument) x.WriteAttributeString("name", this.Name);
				base._SaveAttributes(x);
				x.WriteEndElement();
			}

			protected override void _OnSetDockState(bool before, _DockState state, _DockState oldState) {
				if (before) {
					if (oldState == 0) { //was docked; now hide or float

					}

				} else {

				}

			}
		}

		public interface IPanel
		{
			FrameworkElement Content { get; set; }
		}
	}
}
