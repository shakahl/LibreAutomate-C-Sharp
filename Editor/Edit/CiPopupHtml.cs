using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using TheArtOfDev.HtmlRenderer.WinForms;
using Au.Controls;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Shared.Extensions;

class CiPopupHtml
{
	_Window _w;
	_HtmlPanel _html;

	/// <summary>
	/// The top-level popup window.
	/// </summary>
	public Form PopupWindow => _w;

	public CiPopupHtml()
	{
		_w = new _Window(this);
		_w.SuspendLayout();
		_w.Size = Au.Util.ADpi.ScaleSize((600, 300));

		_html = new _HtmlPanel();
		_html.SuspendLayout();
		_html.AccessibleName = _html.Name = "Codein_info";
		_html.Dock = DockStyle.Fill;
		//_html.Font = EdStock.FontRegular;
		_html.BaseStylesheet = CiUtil.TaggedPartsHtmlStyleSheet;
		_html.BackColor = SystemColors.Info;

		_w.Controls.Add(_html);
		_html.ResumeLayout();
		_w.ResumeLayout(true);
	}

#if true
	//public void SetHtml(CiComplItem ci, ImmutableArray<TaggedText> tags)
	public void SetHtml(CiComplItem ci, IEnumerable<TaggedText> tags)
	{
		var b = new StringBuilder("<body>");
		CiUtil.TaggedPartsToHtml(b, tags);
		APerf.Next('s');

		var symbols = ci.ci.Symbols;
		if(symbols != null) {
			int i = -1;
			foreach(var v in symbols) {
				i++;
				b.Append(i==0?"<br/><br/>":"<br/>");
				//if(v is ReducedExtensionMethodSymbol) b.Append("(extension) ");
				//Print(v.Kind, v.ContainingType?.Name, v.IsStatic, v.CanBeReferencedByName, v.ContainingSymbol, v.GetType().Name, v.OriginalDefinition, v.Name, v.MetadataName);
				//Print(v.Name);
				//Print(v.GetDocumentationCommentId());
				//Print(v.GetDocumentationCommentXml());
				//Print("-----");
				var parts = v.ToDisplayParts(new SymbolDisplayFormat(
					SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
					v.Kind == SymbolKind.NamedType ? SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces : SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
					SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance | SymbolDisplayGenericsOptions.IncludeTypeConstraints,
					SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeConstantValue | SymbolDisplayMemberOptions.IncludeModifiers | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
					SymbolDisplayDelegateStyle.NameAndSignature,
					SymbolDisplayExtensionMethodStyle.InstanceMethod,
					SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeParamsRefOut | SymbolDisplayParameterOptions.IncludeOptionalBrackets | SymbolDisplayParameterOptions.IncludeDefaultValue | SymbolDisplayParameterOptions.IncludeExtensionThis,
					SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
					SymbolDisplayLocalOptions.IncludeType | SymbolDisplayLocalOptions.IncludeRef | SymbolDisplayLocalOptions.IncludeConstantValue,
					SymbolDisplayKindOptions.IncludeMemberKeyword | SymbolDisplayKindOptions.IncludeNamespaceKeyword | SymbolDisplayKindOptions.IncludeTypeKeyword,
					SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
					));
				//Print(v.ToMinimalDisplayString());
				CiUtil.TaggedPartsToHtml(b, parts.ToTaggedText());
				//foreach(var k in parts)Print(k.)
			}
			APerf.Next();
		}

		b.Append("</body>");

		var html = b.ToString();
		//Print(html);
		_html.Text = html;
		APerf.NW();
	}
#elif true
	//public void SetHtml(CiComplItem ci, ImmutableArray<TaggedText> tags)
	public void SetHtml(CiComplItem ci, IEnumerable<TaggedText> tags)
	{
		var b = new StringBuilder("<body>");
		CiUtil.TaggedPartsToHtml(b, tags);
		APerf.Next();

		var symbols = ci.ci.Symbols;
		if(symbols != null && symbols.Count > 1) {
			b.Append("<h4>Other</h4>");
			int i = -1;
			foreach(var v in symbols) {
				if(++i == 0) continue;
				if(i > 1) b.Append("<br/>");
				//if(v is ReducedExtensionMethodSymbol) b.Append("(extension) ");
				//Print(v.Kind, v.ContainingType?.Name, v.IsStatic, v.CanBeReferencedByName, v.ContainingSymbol, v.GetType().Name, v.OriginalDefinition, v.Name, v.MetadataName);
				//Print(v.Name);
				//Print(v.GetDocumentationCommentId());
				//Print(v.GetDocumentationCommentXml());
				//Print("-----");
				var parts = v.ToDisplayParts(new SymbolDisplayFormat(
					SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
					v.Kind == SymbolKind.NamedType ? SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces : SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
					SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance | SymbolDisplayGenericsOptions.IncludeTypeConstraints,
					SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeConstantValue | SymbolDisplayMemberOptions.IncludeModifiers | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
					SymbolDisplayDelegateStyle.NameAndSignature,
					SymbolDisplayExtensionMethodStyle.InstanceMethod,
					SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeParamsRefOut | SymbolDisplayParameterOptions.IncludeOptionalBrackets | SymbolDisplayParameterOptions.IncludeDefaultValue | SymbolDisplayParameterOptions.IncludeExtensionThis,
					SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
					SymbolDisplayLocalOptions.IncludeType | SymbolDisplayLocalOptions.IncludeRef | SymbolDisplayLocalOptions.IncludeConstantValue,
					SymbolDisplayKindOptions.IncludeMemberKeyword | SymbolDisplayKindOptions.IncludeNamespaceKeyword | SymbolDisplayKindOptions.IncludeTypeKeyword,
					SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
					));
				//Print(v.ToMinimalDisplayString());
				CiUtil.TaggedPartsToHtml(b, parts.ToTaggedText());
				//foreach(var k in parts)Print(k.)
			}
			APerf.Next();
		}

		b.Append("</body>");

		var html = b.ToString();
		//Print(html);
		_html.Text = html;
		APerf.NW();
	}
#else

	public void SetHtml(CiComplItem ci)
	{
		APerf.First();
		var b = new StringBuilder("<body>");
		var symbols = ci.ci.Symbols;
		if(symbols != null) {
			int i = -1;
			foreach(var v in symbols) {
				i++;
				if(i==1)b.Append("<h4>Other</h4>");
				if(i>1) b.Append("<br/>");
				//if(v is ReducedExtensionMethodSymbol) b.Append("(extension) ");
				//Print(v.Kind, v.ContainingType?.Name, v.IsStatic, v.CanBeReferencedByName, v.ContainingSymbol, v.GetType().Name, v.OriginalDefinition, v.Name, v.MetadataName);
				//Print(v.Name);
				//Print(v.GetDocumentationCommentId());
				//Print(v.GetDocumentationCommentXml());
				//Print("-----");
				var parts = v.ToDisplayParts(new SymbolDisplayFormat(
					SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
					v.Kind == SymbolKind.NamedType ? SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces : SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
					SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance | SymbolDisplayGenericsOptions.IncludeTypeConstraints,
					SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeConstantValue | SymbolDisplayMemberOptions.IncludeModifiers | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
					SymbolDisplayDelegateStyle.NameAndSignature,
					SymbolDisplayExtensionMethodStyle.InstanceMethod,
					SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeParamsRefOut | SymbolDisplayParameterOptions.IncludeOptionalBrackets | SymbolDisplayParameterOptions.IncludeDefaultValue | SymbolDisplayParameterOptions.IncludeExtensionThis,
					SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
					SymbolDisplayLocalOptions.IncludeType | SymbolDisplayLocalOptions.IncludeRef | SymbolDisplayLocalOptions.IncludeConstantValue,
					SymbolDisplayKindOptions.IncludeMemberKeyword | SymbolDisplayKindOptions.IncludeNamespaceKeyword | SymbolDisplayKindOptions.IncludeTypeKeyword,
					SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
					));
				//Print(v.ToMinimalDisplayString());
				CiUtil.TaggedPartsToHtml(b, parts.ToTaggedText());
				//foreach(var k in parts)Print(k.)

				//var xmlDoc = v.GetDocumentationCommentXml();
				////Print(xmlDoc);
				//if(s_rxSummary.MatchS(xmlDoc, out string summary, 1)) {
				//	Print(summary);
				//}

				//v.GetDocumentationParts()
				//Microsoft.CodeAnalysis.Shared.Extensions.ISymbolExtensions.GetDocumentationComment();
				//v.GetDocumentationComment()
			}
			APerf.Next();
		}

		b.Append("</body>");

		var html = b.ToString();
		//Print(html);
		_html.Text = html;
		APerf.NW();
	}
	static ARegex s_rxSummary = new ARegex(@"(?s)<summary>(.+?)</summary>");
#endif



	public void Show(SciCode doc, int position)
	{
		var r = CiUtil.GetCaretRectFromPos(doc, position);
		r.Inflate(100, 10);
		_w.ShowAt(doc, new Point(r.Left, r.Bottom), r, 0);
	}

	public void Show(SciCode doc, Rectangle exclude)
	{
		_w.ShowAt(doc, new Point(exclude.Right, exclude.Top), exclude, 0);
	}

	public void Hide()
	{
		_w.Hide();
		_html.Text = null;
	}

	class _Window : Form
	{
		CiPopupHtml _p;
		Control _owner;
		bool _showedOnce;

		public _Window(CiPopupHtml p)
		{
			_p = p;

			this.AutoScaleMode = AutoScaleMode.None;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.Text = "Au.CiPopupHtml";
			this.MinimumSize = Au.Util.ADpi.ScaleSize((150, 150));
			this.Font = Au.Util.AFonts.Regular;
		}

		protected override CreateParams CreateParams {
			get {
				var p = base.CreateParams;
				p.Style = unchecked((int)(WS.POPUP | WS.THICKFRAME));
				p.ExStyle = (int)(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE);
				return p;
			}
		}

		protected override bool ShowWithoutActivation => true;

		public void ShowAt(Control c, Point pos, Rectangle exclude, PopupAlignment align)
		{
			Api.CalculatePopupWindowPosition(pos, this.Size, (uint)align, exclude, out var r);
			Bounds = r;

			var owner = c.TopLevelControl;
			bool changedOwner = false;
			if(_showedOnce) {
				changedOwner = owner != _owner;
				if(Visible) {
					if(!changedOwner) return;
					Visible = false;
				}
			}
			_owner = owner;

			Show(_owner);
			if(changedOwner) ((AWnd)this).ZorderAbove((AWnd)_owner);
			_showedOnce = true;
		}

		protected override void WndProc(ref Message m)
		{
			//AWnd.More.PrintMsg(m);

			switch(m.Msg) {
			case Api.WM_MOUSEACTIVATE:
				m.Result = (IntPtr)Api.MA_NOACTIVATE;
				return;
				//case Api.WM_ACTIVATEAPP:
				//	if(m.WParam == default) _p.Hide();
				//	break;			}
			}

			base.WndProc(ref m);
		}
	}

	class _HtmlPanel : HtmlPanel
	{
		//CiPopupHtml _p;

		public _HtmlPanel(/*CiPopupHtml p*/)
		{
			//_p = p;
			//this.SetStyle(ControlStyles.Selectable, false); //prevent focusing control and activating window on click. Does not work with this control. Activates on click.
		}

		protected override void OnClick(EventArgs e)
		{
			//base.OnClick(e); //sets focus
		}
	}
}
