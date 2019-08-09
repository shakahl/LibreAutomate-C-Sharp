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
using Au.Editor.Properties;

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
	FastListBox _list;

	public FastListBox Control => _list;

	public PanelCodein()
	{
		this.AccessibleName = this.Name = "Codein";
		_list = new FastListBox();
		_list.AccessibleName = _list.Name = "Codein_list";
		//_list.BorderStyle = BorderStyle.None;
		_list.Dock = DockStyle.Fill;
		_list.ItemClick += _list_ItemClick;
		this.Controls.Add(_list);
	}

	private void _list_ItemClick(object sender, FastListBox.ItemClickArgs e)
	{
		Print(e.Index, e.DoubleClick);
		//_list.Focus();
	}

	List<CodeinItem> _a;

	public void SetListItems(List<CodeinItem> a)
	{
		_a = a;
		if(Empty(a)) {
			_list.AddItems(0, null, 0, null);
		} else {
			_list.AddItems(_a.Count, i => _a[i].DisplayText, Au.Util.ADpi.ScaleInt(30), _DrawItem);
		}
	}

	void _DrawItem(FastListBox.ItemDrawArgs e)
	{
		var g = e.Graphics;
		var ci = _a[e.Index];
		var r = e.Bounds;
		//Print(e.Index, r);

		var p1 = APerf.Create();
		if(ci.Image != null) g.DrawImage(ci.Image, r.X + 1, (r.Y + r.Bottom + 1) / 2 - ci.Image.Height / 2); //note: would be very slow, but is very fast if DoubleBuffered = true (the control sets it in ctor).
		p1.Next();

		int xText = 22; //TODO: DPI scale image and this
		r.Width -= xText; r.X += xText;
		if(e.IsSelected) g.FillRectangle(SystemBrushes.Control, r);

		var s = ci.DisplayText;
		ADebug.PrintIf(!Empty(ci.CI.DisplayTextPrefix), s); //we don't support prefix; never seen.
		var font = _list.Font;

		var a = ci.Hilite;
		if(a != null) {
			for(int i = 0, x = 0; i < a.Length; i++) {
				int j = i == 0 ? 0 : a[i - 1];
				x += _Measure(j, a[i] - j);
				j = a[i++];
				var z2 = _Measure(j, a[i] - j);
				g.FillRectangle(Brushes.GreenYellow, r.X + x, r.Y, z2 + 1, r.Height);
				x += z2;
			}

			int _Measure(int from, int len)
			{
				if(len == 0) return 0;
				return TextRenderer.MeasureText(g, s.Substring(from, len), font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
			}
		}

		TextRenderer.DrawText(g, ci.DisplayText, font, r, Color.Black, TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
		//p1.NW();
	}
}

class CodeinItem
{
	public CompletionItem CI { get; }

	public int[] Hilite { get; }

	public string DisplayText => _text ??= CI.DisplayText + CI.DisplayTextSuffix;
	string _text;

	public Bitmap Image => _image ??= _GetImage();
	Bitmap _image;

	public CodeinItem(CompletionItem ci, int[] hilite = null)
	{
		CI = ci;
		Hilite = hilite;
	}

	Bitmap _GetImage()
	{
		string s = null;

		//_PrintCI("green");

		int pubPrivProtInt = 0;
		if(CI.Tags.Length > 1) {
			if(CI.Tags.Length != 2) _PrintCI("green");
			switch(CI.Tags[1]) {
			case WellKnownTags.Public: break;
			case WellKnownTags.Private: pubPrivProtInt = 1; break;
			case WellKnownTags.Protected: pubPrivProtInt = 2; break;
			case WellKnownTags.Internal: pubPrivProtInt = 3; break;
			default: ADebug.Print($"<><c green>{CI.Tags[1]}<>"); break;
			}
		}

		var v = CI.Tags[0];
		switch(v) {
		//types
		case WellKnownTags.Class: s = _Sel(nameof(Resources.Class_16x), nameof(Resources.ClassPrivate_16x), nameof(Resources.ClassProtected_16x), nameof(Resources.ClassFriend_16x)); break;
		case WellKnownTags.Structure: s = _Sel(nameof(Resources.ValueType_left_16x), nameof(Resources.ValueTypePrivate_16x), nameof(Resources.ValueTypeProtect_16x), nameof(Resources.ValueTypeFriend_16x)); break;
		case WellKnownTags.Enum: s = _Sel(nameof(Resources.Enumerator_left_16x), nameof(Resources.EnumPrivate_16x), nameof(Resources.EnumProtect_16x), nameof(Resources.EnumFriend_16x)); break;
		case WellKnownTags.Delegate: s = _Sel(nameof(Resources.Delegate_left_16x), nameof(Resources.DelegatePrivate_16x), nameof(Resources.DelegateProtected_16x), nameof(Resources.DelegateFriend_16x)); break;
		case WellKnownTags.Interface: s = _Sel(nameof(Resources.Interface_16x), nameof(Resources.InterfacePrivate_16x), nameof(Resources.InterfaceProtect_16x), nameof(Resources.InterfaceFriend_16x)); break;
		//functions, events
		case WellKnownTags.Method: s = _Sel(nameof(Resources.Method_left_16x), nameof(Resources.MethodPrivate_16x), nameof(Resources.MethodProtect_16x), nameof(Resources.MethodFriend_16x)); break;
		case WellKnownTags.ExtensionMethod: s = nameof(Resources.ExtensionMethod_16x); break;
		case WellKnownTags.Property: s = _Sel(nameof(Resources.Property_left_16x), nameof(Resources.PropertyPrivate_16x), nameof(Resources.PropertyProtect_16x), nameof(Resources.PropertyFriend_16x)); break;
		case WellKnownTags.Event: s = _Sel(nameof(Resources.Event_left_16x), nameof(Resources.EventPrivate_16x), nameof(Resources.EventProtect_16x), nameof(Resources.EventFriend_16x)); break;
		//values
		case WellKnownTags.Field: s = _Sel(nameof(Resources.Field_left_16x), nameof(Resources.FieldPrivate_16x), nameof(Resources.FieldProtect_16x), nameof(Resources.FieldFriend_16x)); break;
		case WellKnownTags.Local: case WellKnownTags.Parameter: case WellKnownTags.RangeVariable: s = nameof(Resources.LocalVariable_16x); break;
		case WellKnownTags.Constant: s = _Sel(nameof(Resources.Constant_left_16x), nameof(Resources.ConstantPrivate_16x), nameof(Resources.ConstantProtected_16x), nameof(Resources.ConstantFriend_16x)); break;
		case WellKnownTags.EnumMember: s = nameof(Resources.EnumItem_left_16x); break;
		//other
		case WellKnownTags.Keyword: s = nameof(Resources.IntelliSenseKeyword_16x); break;
		case WellKnownTags.Namespace: s = nameof(Resources.Namespace_16x); break;
		case WellKnownTags.Label: s = nameof(Resources.Label_16xMD); break;
		case WellKnownTags.TypeParameter: s = nameof(Resources.Type_left_16x); break;
		case WellKnownTags.Snippet: s = nameof(Resources.Snippet_16x); break;
		default: _PrintCI("blue"); break;
		}

		string _Sel(string sPublic, string sPrivate, string sProtected, string sInternal)
		{
			var r = pubPrivProtInt switch { 0 => sPublic, 1 => sPrivate, 2 => sProtected, _ => sInternal };
			ADebug.PrintIf(r == null, v);
			return r ?? sPublic;
		}

		//s ??= nameof(Resources.empty_16x);
		return s == null ? null : EdResources.GetImageUseCache(s);
	}

	[Conditional("DEBUG")]
	void _PrintCI(string color)
	{
		Print($"<><c {color}>{CI.DisplayText}<>");
		foreach(var v in CI.Tags) Print(v);
		//foreach(var v in CI.Properties) Print(v);
	}
}
