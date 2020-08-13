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
		/// <summary>
		/// Base of _Panel and _Tab.
		/// Has caption and can float.
		/// </summary>
		abstract class _Dragable : _Node
		{
			Dock _captionAt;
			_DockState _dockState;

			public _Dragable(AuPanels pm, _Node parent, XElement x) : base(pm, parent, x) {
				x.Attr(out _captionAt, "captionAt");
				x.Attr(out _dockState, "state");
			}

			public Dock CaptionAt => _captionAt;
			public _DockState DockState => _dockState;


			/// <summary>
			/// Writes attributes if need.
			/// </summary>
			protected override void _SaveAttributes(XmlWriter x) {
				base._SaveAttributes(x);
				if (_captionAt != default) x.WriteAttributeString("captionAt", _captionAt.ToString());
				if (_dockState != default) x.WriteAttributeString("state", _dockState.ToString());
			}
		}

		enum _DockState { Dock, Float, Hide }
	}
}
