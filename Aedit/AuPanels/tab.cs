using Au.Types;
using Au.Util;
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
using System.Windows.Input;
using System.Globalization;

namespace Au.Controls
{
	public partial class AuPanels
	{
		partial class _Node
		{
			void _InitTabControl() {
				var tc = _tab.tc;
				tc.Padding = default;
				tc.TabStripPlacement = _captionAt;
				tc.ApplyTemplate(); if (VisualTreeHelper.GetChild(tc, 0) is Grid tg) tg.Background = _pm.CaptionBrush; //note: tc must have a parent.
				tc.SizeChanged += (_, e) => {
					switch (tc.TabStripPlacement) { case Dock.Top: case Dock.Bottom: return; }
					bool bigger = e.NewSize.Height > e.PreviousSize.Height;
					if (bigger != _tab.isVerticalHeader) _VerticalTabHeader(e.NewSize.Height);
				};
				tc.ContextMenuOpening += _CaptionContextMenu;
				tc.PreviewMouseDown += _OnMouseDown;
			}

			void _VerticalTabHeader(double height = -1, bool onMove = false) {
				var tc = _tab.tc;
				switch (tc.TabStripPlacement) { case Dock.Top: case Dock.Bottom: return; }

				if (height < 0) height = tc.ActualHeight;
				var tabs = tc.Items.Cast<TabItem>();

				//APerf.First();
				var d = _CalcHeight();
				//APerf.NW();
				//AOutput.Write(d, height);
				bool vert2 = d < height - 10;
				if (vert2 == _tab.isVerticalHeader && !onMove) return;
				_tab.isVerticalHeader = vert2;
				var dock = tc.TabStripPlacement;
				foreach (var v in tabs) v.Style = vert2 ? (dock == Dock.Left ? s_styleL : s_styleR) : null;

				//if (vert2) foreach (var v in tabs) v.Height = 50;

				double _CalcHeight() {
					var cult = CultureInfo.InvariantCulture;
					var fdir = tc.FlowDirection;
					//AOutput.Write(tc.FontFamily.FamilyTypefaces);
					//var ffam = tc.FontFamily.FamilyTypefaces[0];
					var font = new Typeface(tc.FontFamily, tc.FontStyle, tc.FontWeight, tc.FontStretch);
					var fsize = tc.FontSize;
					var brush = SystemColors.ControlTextBrush;
					//var ppd = VisualTreeHelper.GetDpi(tc).PixelsPerDip; AOutput.Write(ppd); //ignored, and we don't need it
					//APerf.Next();
					double r = 4;
					foreach (var v in tabs) {
						var f = new FormattedText(v.Header.ToString(), cult, fdir, font, fsize, brush, 1);
						//AOutput.Write(f.Width + 11);
						r += f.Width + 11;
					}
					return r;
				}
			}

			/// <summary>
			/// Adds this to parent tab at startup or when moving.
			/// Caller before must call AddChild (or AddSibling) and set _index.
			/// </summary>
			void _AddToTab(bool moving) {
				_panel.ti = new TabItem { Header = _panel.name, Content = _elem, Tag = this };
				var tc = Parent._tab.tc;
				tc.Items.Insert(_index, _panel.ti);
				if (moving) {
					_ShiftSiblingIndices(1);
					Parent._VerticalTabHeader(onMove: true);
				}
			}

			void _ShowHideInTab(bool show) {
				var tc = Parent._tab.tc;
				var ti = tc.Items[_index] as TabItem;
				if (!show) {
					if (ti == tc.SelectedItem) {
						var a = tc.Items.OfType<TabItem>().Where(o => o.IsVisible).ToArray();
						if (a.Length > 1) {
							int i = Array.IndexOf(a, ti);
							if (++i == a.Length) i -= 2;
							tc.SelectedItem = a[i];
						} else if (!_IsDocumentHost) {
							if (Parent._state == _DockState.Float) Parent._SetDockState(_DockState.Hide);
							else Parent._ShowHideInStack(show);
						}
					}
					ti.Visibility = Visibility.Collapsed;
					ti.Content = null;
				} else {
					if (tc.Parent == null) {
						if (Parent._state.Has(_DockState.Float)) Parent._SetDockState(_DockState.Float);
						else Parent._ShowHideInStack(show);
					}

					ti.Content = _elem;
					ti.Visibility = Visibility.Visible;
					tc.SelectedItem = ti;
				}
			}

			static _Node _NodeFromTabItem(TabItem ti) => ti.Tag as _Node;
		}
	}
}
