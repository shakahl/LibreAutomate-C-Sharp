using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	public unsafe partial class Acc
	{
		struct _BrowserInterface :IDisposable
		{
			public IHTMLElement ie;
			public ISimpleDOMNode ff;
			bool _dispose;
			[ThreadStatic] static bool t_preferIE;

			public _BrowserInterface(Acc a) : this()
			{
				//Acc FUTURE: cache ie/ff in AFAcc.
				//	That is why we have _dispose. If our ie/ff are from the AFAcc, _dispose would be false, because the AFAcc will dispose them.

				var iacc = a._iacc;
				if(t_preferIE) { //if previously was IE, now try IE first. Normally this would make getting IHTMLElement two times faster. Actually this does not have sense when getting IHTMLElement is so slow, but maybe the speed depends on PC, IE version, etc.
					_dispose = IHTMLElement.From(iacc, out ie) || ISimpleDOMNode.From(iacc, out ff);
				} else {
					_dispose = ISimpleDOMNode.From(iacc, out ff) || IHTMLElement.From(iacc, out ie);
				}
				t_preferIE = !ie.Is0;
				if(!_dispose) Output.Warning("Cannot get HTML properties of this accessible object. More info in the Help file.");
			}

			public void Dispose()
			{
				if(_dispose) {
					ie.Dispose();
					ff.Dispose();
					_dispose = false;
				}
			}
		}

		/// <summary>
		/// Gets a HTML attribute or "".
		/// </summary>
		/// <param name="name">Attribute name, for example "href", "id", "class".  Full, case-sensitive.</param>
		/// <param name="interpolated">Used only with Internet Explorer. If true, can get modified value, for example full URL instead of relative. If false (default), gets value as it is in HTML.</param>
		/// <remarks>
		/// Returns "" if this is not a HTML element or does not have the specified attribute or failed. Supports <see cref="Native.GetError"/>.
		/// Works with these web browsers: Firefox (must be non-portable), Chrome (if Firefox is installed), Internet Explorer (slow), and browsers based on their code.
		/// </remarks>
		/// <exception cref="ArgumentException">name is null/"".</exception>
		public string HtmlAttribute(string name, bool interpolated = false)
		{
			if(Empty(name)) throw new ArgumentException("invalid name");
			string s = null; int hr;
			using(var k = new _BrowserInterface(this)) {
				if(!k.ie.Is0) hr = k.ie.GetAttribute(name, interpolated ? 0 : 2, out s); //info: only "class" must match case
				else if(!k.ff.Is0) hr = k.ff.GetAttribute(name, out s); //info: with Chrome must match case
				else hr = Api.E_NOINTERFACE;
			}
			_Hresult(_FuncId.html, hr);
			return s ?? "";
		}

		/// <summary>
		/// Gets all HTML attributes.
		/// </summary>
		/// <param name="interpolated">Used only with Internet Explorer. If true, can get modified value, for example full URL instead of relative. If false (default), gets value as it is in HTML.</param>
		/// <remarks>
		/// Returns empty dictionary if this is not a HTML element or does not have attributes or failed. Supports <see cref="Native.GetError"/>.
		/// Works with these web browsers: Firefox (must be non-portable), Chrome (if Firefox is installed), Internet Explorer (slow), and browsers based on their code.
		/// </remarks>
		public Dictionary<string, string> HtmlAttributes(bool interpolated = false)
		{
			Dictionary<string, string> d = null; int hr;
			using(var k = new _BrowserInterface(this)) {
				if(!k.ie.Is0) {
					//IHTMLElement does not have a method to get all attributes.
					//	IHTMLElement5 has, but slow and much work.
					//	Better get attribute names from HTML and call IHTMLElement.GetAttribute for each.
					hr = k.ie.GetOuterHTML(out var html);
					if(hr == 0 && html.Length_() >= 7 && html[0] == '<') {
						int i = html.IndexOf('>');
						if(i >= 6 && html.IndexOf('=', 0, i) > 0) {
							if(i + 1 < html.Length) html = html.Remove(i);
							var m = s_rxAttr.Matches(html);
							int n = m.Count;
							d = new Dictionary<string, string>(n);
							for(i = 0; i < n; i++) {
								var name = m[i].Groups[1].Value;
								hr = k.ie.GetAttribute(name, interpolated ? 0 : 2, out var s);
								if(hr == 0) d[name] = s ?? "";
							}
							if(d.Count > 0) hr = 0;
						}
					}
				} else if(!k.ff.Is0) {
					hr = k.ff.GetAttributes(out var names, out var values, out var n);
					d = new Dictionary<string, string>(n);
					if(hr == 0) {
						for(int i = 0; i < n; i++) {
							d[names[i]] = values[i] ?? "";
						}
					}
				} else hr = Api.E_NOINTERFACE;
			}
			_Hresult(_FuncId.html, hr);
			return d ?? new Dictionary<string, string>();
		}
		static Regex s_rxAttr = new Regex(@"\s([^\s=]+)\s*=\s*"".*?""", RegexOptions.CultureInvariant);

		/// <summary>
		/// Compares values of specified properties with specified strings. Also supports HTML attributes.
		/// Returns true if all match, false if at least one doesn't.
		/// </summary>
		/// <param name="nameValuePairs">
		/// List of name/value pairs of properties and/or HTML attributes.
		/// Names must be of type string. Full, case-sensitive.
		/// Supported property names: "name" (<see cref="Name"/>), "value" (<see cref="Value"/>), "description" (<see cref="Description"/>).
		/// Names of HTML attributes must be with "a:" prefix, like "a:href". Supported are all attributes.
		/// Values can be of type string, <see cref="Regex"/> or <see cref="Wildex"/>. Other types are converted to string. Strings are compared as case-insensitive wildcard (see <see cref="String_.Like_(string, string, bool)"/>. If fails to get a property or attribute (eg this HTML element does not have the attribute), its value is considered "".
		/// Example (1 property): <c>"value", "*example*"</c>
		/// Example (2 attributes): <c>"a:class", "example", "a:href", "*/example.php*"</c>
		/// </param>
		/// <exception cref="ArgumentException">Odd number of arguments. A name is null, "" or not string. A name is not one of supported property names and does not have "a:" prefix.</exception>
		/// <remarks>
		/// Also returns false if cannot get HTML attributes (when an attribute is specified), for example if this is not a HTML element. Then <see cref="Native.GetError"/> will return error code E_NOINTERFACE (0x80004002).
		/// Can get attributes from these web browsers: Firefox (must be non-portable), Chrome (if Firefox is installed), Internet Explorer (slow), and browsers based on their code.
		/// </remarks>
		public bool Match(params object[] nameValuePairs)
		{
			int n = nameValuePairs.Length;
			if((n & 1) != 0) throw new ArgumentException("Odd number of arguments.");
			_BrowserInterface k = default; bool isBI = false;
			try {
				for(int i = 0; i < n; i += 2) {
					string name = nameValuePairs[i] as string, s = null;
					object o = nameValuePairs[i + 1];
					if(Empty(name)) throw new ArgumentException("Invalid name.");
					if(name.StartsWith_("a:")) {
						if(!isBI) {
							k = new _BrowserInterface(this);
							int hr = k.ie.Is0 && k.ff.Is0 ? Api.E_NOINTERFACE : 0;
							_Hresult(_FuncId.html, hr);
							if(hr != 0) return false;
							isBI = true;
						}
						name = Util.StringCache.LibAdd(name, 2, name.Length - 2);
						if(!k.ie.Is0) k.ie.GetAttribute(name, 2, out s); else k.ff.GetAttribute(name, out s);
						//Acc FUTURE: with isimpledomnode can get multiple names in single call. Probably faster.
					} else {
						switch(name) {
						case "name": s = Name; break;
						case "value": s = Value; break;
						case "description": s = Description; break;
						//case "rect": //rejected
						//	RECT r;
						//	if(o is RECT) r = (RECT)o; else if(o is Rectangle) r = (Rectangle)o; else throw new ArgumentException("Expected RECT.");
						//	if(!GetRect(out var rr) || rr != r) return false;
						//	continue;
						default: throw new ArgumentException("Unknown property name. Expected \"name\", \"value\", \"description\" or prefix \"a:\".");
						}
					}
					if(s == null) s = "";
					bool yes = false;
					switch(o) {
					case string str: yes = s.Like_(str, true); break;
					case Wildex wx: yes = wx.Match(s); break;
					case Regex rx: yes = rx.IsMatch(s); break;
					default: yes = s.Like_(o.ToString(), true); break;
					}
					if(!yes) return false;
				}
			}
			finally { k.Dispose(); }
			return true;
		}

		/// <summary>
		/// Gets HTML or "".
		/// </summary>
		/// <param name="outer">If true, gets outer HTML (with tag and attributes), else inner HTML.</param>
		/// <remarks>
		/// Returns "" if this is not a HTML element or if failed. Supports <see cref="Native.GetError"/>.
		/// Works with these web browsers: Firefox (must be non-portable), Chrome (if is installed Firefox of same 32/64 bit), Internet Explorer (can be slow), and browsers based on their code. With Chrome slow if there are many descendant HTML elements. With Firefox and Chrome, this process must be of same 32/64 bit.
		/// If this is the root of web page (role DOCUMENT or PANE), get web page body HTML.
		/// </remarks>
		public string Html(bool outer)
		{
			string s = null; int hr;
			using(var k = new _BrowserInterface(this)) {
				if(!k.ie.Is0) hr = outer ? k.ie.GetOuterHTML(out s) : k.ie.GetInnerHTML(out s);
				else if(!k.ff.Is0) hr = outer ? k.ff.GetOuterHTML(out s) : k.ff.GetInnerHTML(out s);
				else hr = Api.E_NOINTERFACE;
			}
			_Hresult(_FuncId.html, hr);
			return s ?? "";
		}

		/// <summary>
		/// Scrolls this accessible object into view.
		/// </summary>
		/// <exception cref="CatException">Failed to scroll, or the object does not support scrolling (then error "No such interface supported").</exception>
		/// <remarks>
		/// Supports web page elements in Internet Explorer, Firefox, Chrome and web browsers/controls based on their code.
		/// Also supports objects in some other windows, but in most window doesn't.
		/// </remarks>
		public void ScrollTo()
		{
			int hr = Api.E_NOINTERFACE;
			if(ISimpleDOMNode.From(_iacc, out var ff)) {
				hr = ff.scrollTo(0);
				ff.Dispose();
			} else {
				var e = AElement.FromAcc(this);
				if(e != null) {
					if(e.GetCurrentPattern(UIA.PatternId.ScrollItem) is UIA.IScrollItemPattern p) {
						p.ScrollIntoView();
						hr = 0;
					}
				}
			}
			CatException.ThrowIfHresultNot0(hr, "*scroll");

			//tested: from the 3 browsers, only IE supports UI Automation scrolling (IUIAutomationScrollItemPattern).
		}

		//IHTMLElement

		internal struct IHTMLElement :IDisposable
		{
			IntPtr _iptr;

			public static bool From(IAccessible iacc, out IHTMLElement x)
			{
				return Util.Marshal_.QueryService(iacc, out x, ref IID_IHTMLElement);
			}

			internal static Guid IID_IHTMLElement = new Guid("3050f1ff-98b5-11cf-bb82-00aa00bdce0b");

			public void Dispose()
			{
				if(_iptr != default) {
					Marshal.Release(_iptr);
					_iptr = default;
				}
			}

			public bool Is0 => _iptr == default;

			public static implicit operator IntPtr(IHTMLElement a) => a._iptr;

			/// <param name="name">If "class", calls get_className, else getAttribute.</param>
			/// <param name="flags">0 interpolated, 1 match case (don't use), 2 raw.</param>
			/// <param name="s"></param>
			public int GetAttribute(string name, int flags, out string s)
			{
				s = "";
				int hr;
				switch(name) {
				case "class":
					hr = _F.get_className(_iptr, out BSTR b);
					if(hr == 0) s = b.ToStringAndDispose();
					break;
				default:
					hr = _F.getAttribute(_iptr, name, flags, out VARIANT v);
					if(hr == 0) s = v.ToStringAndDispose();
					break;
				}
				if(s == null) s = "";
				return hr;
			}

			public int GetInnerHTML(out string s)
			{
				return _F.get_innerHTML(_iptr, out s);
			}

			public int GetOuterHTML(out string s)
			{
				return _F.get_outerHTML(_iptr, out s);
			}

			static ConcurrentDictionary<LPARAM, _Vtbl> s_vtbls = new ConcurrentDictionary<LPARAM, _Vtbl>();

			_Vtbl _F
			{
				get
				{
					if(_iptr == default) throw new ObjectDisposedException(nameof(IHTMLElement));
					return s_vtbls.GetOrAdd(*(IntPtr*)_iptr, vtbl => new _Vtbl(vtbl));
				}
			}

			class _Vtbl
			{
				public _Vtbl(long vtbl)
				{
#if DEBUG
					int n = s_vtbls.Count; if(n > 0) Debug_.Print("many VTBLs: " + (n + 1));
#endif
					var a = (IntPtr*)vtbl;
					Util.Marshal_.GetDelegate(a[8], out getAttribute);
					Util.Marshal_.GetDelegate(a[11], out get_className);
					Util.Marshal_.GetDelegate(a[58], out get_innerHTML);
					Util.Marshal_.GetDelegate(a[62], out get_outerHTML);
				}

				internal readonly getAttributeT getAttribute;
				internal readonly LibDelegateTypes.IntPtr_out_BSTR get_className;
				internal readonly LibDelegateTypes.IntPtr_out_string get_innerHTML;
				internal readonly LibDelegateTypes.IntPtr_out_string get_outerHTML;

				internal delegate int getAttributeT(IntPtr obj, [MarshalAs(UnmanagedType.BStr)] string name, int flags, out VARIANT value);
			}
		}

		/// <summary>
		/// We use this to get Firefox/Chrome HTML element attributes, HTML etc.
		/// rejected: find ISimpleDOMNode and convert to Acc, like QM2 FFNode/FindFF. Usually it is not significantly faster, sometimes much slower. For speed use UI Automation instead.
		/// </summary>
		internal struct ISimpleDOMNode :IDisposable
		{
			IntPtr _iptr;

			public static bool From(IAccessible iacc, out ISimpleDOMNode x)
			{
				return Util.Marshal_.QueryService(iacc, out x, ref IID_ISimpleDOMNodeService, ref IID_ISimpleDOMNode);
			}

			internal static Guid IID_ISimpleDOMNode = new Guid("1814ceeb-49e2-407f-af99-fa755a7d2607");
			internal static Guid IID_ISimpleDOMNodeService = new Guid(0x0c539790, 0x12e4, 0x11cf, 0xb6, 0x61, 0x00, 0xaa, 0x00, 0x4c, 0xd6, 0xd8);

			public void Dispose()
			{
				if(_iptr != default) {
					Marshal.Release(_iptr);
					_iptr = default;
				}
			}

			public bool Is0 => _iptr == default;

			public static implicit operator IntPtr(ISimpleDOMNode a) => a._iptr;

			public enum NodeType :short
			{
				Element = 1,
				Attribute,
				Text,
				Section,
				Reference,
				Entity,
				ProcessingInstruction,
				Comment,
				Document,
				DocumentType,
				DocumentFragment,
				Notation,
			}

			public struct NodeInfo
			{
				public string tag, text;
				public int childCount, uniqueId;
				public short namespaceId;
				public NodeType nodeType;
			}

			public int GetNodeInfo(out NodeInfo x, bool needText)
			{
				x = new NodeInfo();
				int hr = _F.get_nodeInfo(_iptr, out var tag, out x.namespaceId, out var text, out x.childCount, out x.uniqueId, out x.nodeType);
				if(hr == 0) {
					x.tag = tag.ToStringAndDispose();
					if(needText) {
						x.text = text.ToStringAndDispose();

						if(Empty(x.text) && x.nodeType == NodeType.Text) { //Chrome
							if(ISimpleDOMText.FromNode(this, out var nt)) {
								nt.GetText(out x.text);
								nt.Dispose();
							}
						}
					} else text.Dispose();
				}
				return hr;
			}

			public int GetAttributes(out string[] names, out string[] values, out int count)
			{
				for(int n = 16; ; n *= 2) { //max seen: 22 in FF UI, 16 in web page (rarely > 12)
					names = new string[n];
					values = new string[n];
					var nsids = new short[n];
					count = 0; //get_attributes last param actually is ushort*
					var hr = _F.get_attributes(_iptr, n, names, nsids, values, out count);
					if(hr != 0) count = 0;
					if(count < n) return hr;
				}
			}

			public int GetAttribute(string name, out string s)
			{
				int hr = _F.get_attribute(_iptr, 1, ref name, out _, out var b);
				s = hr == 0 ? b.ToStringAndDispose() : null;
				return hr;
			}

			public int GetInnerHTML(out string s)
			{
				int hr = _F.get_innerHTML(_iptr, out s);
				if(hr == Api.E_NOTIMPL) { //Chrome does not implement this method. Workaround: compose from descendants.
					if(0 == GetNodeInfo(out var x, true)) {
						hr = 0;
						if(x.childCount > 0) {
							var b = Util.LibStringBuilderCache.Acquire();
							_ChromeComposeInnerHTML(b, x.childCount);
							s = b.ToStringCached_();
						} else s = "";
					} else Debug_.Print("failed");
				}
				if(hr != 0) s = null;
				return hr;
			}

			void _ChromeComposeInnerHTML(StringBuilder b, int childCount)
			{
				for(int i = 0; i < childCount; i++) {
					if(0 != _F.get_childAt(_iptr, i, out var child)) { Debug_.Print("failed"); if(i == 0) return; continue; }
					child._ChromeComposeHTML(b);
					child.Dispose();
				}
			}

			void _ChromeComposeHTML(StringBuilder b)
			{
				if(0 != GetNodeInfo(out var x, needText: true)) { Debug_.Print("failed"); return; }
				if(Empty(x.tag)) {
					b.Append(x.text);
				} else {
					_HtmlAppendHead(b, x.tag);
					_ChromeComposeInnerHTML(b, x.childCount);
					_HtmlAppendTail(b, x.tag);
				}
			}

			void _HtmlAppendHead(StringBuilder b, string tag)
			{
				b.Append('<');
				b.Append(tag);
				if(0 == GetAttributes(out var names, out var vals, out int n) && n > 0) {
					for(int i = 0; i < n; i++) {
						b.Append(' '); b.Append(names[i]); b.Append('='); b.Append('\"'); b.Append(vals[i]); b.Append('\"');
					}
				}
				b.Append('>');
			}

			void _HtmlAppendTail(StringBuilder b, string tag)
			{
				b.Append('<'); b.Append('/'); b.Append(tag); b.Append('>');
			}

			public int GetOuterHTML(out string s)
			{
				s = null;
				int hr = GetNodeInfo(out var x, needText: true);
				if(hr == 0) {
					if(Empty(x.tag)) {
						Debug_.PrintIf(x.nodeType != NodeType.Text, "--- not Text: " + x.nodeType);
						s = x.text;
					} else {
						bool isDoc = x.nodeType == NodeType.Document;
						if(isDoc) x.tag = "body"; //"#document"
						Debug_.PrintIf(x.nodeType != NodeType.Element && !isDoc && x.tag != "br", "--- not Element: " + x.nodeType);
						//ISimpleDOMNode does not have a method to get outer HTML. Compose it from tag, attributes and inner HTML.
						var b = Util.LibStringBuilderCache.Acquire();
						_HtmlAppendHead(b, x.tag);
						if(x.childCount > 0) {
							int hr2 = _F.get_innerHTML(_iptr, out var inner);
							if(hr2 == 0) b.Append(inner);
							else if(hr2 == Api.E_NOTIMPL) { //Chrome does not implement this method. Workaround: compose from descendants.
								_ChromeComposeInnerHTML(b, x.childCount);
								hr = 0;
							} else if(hr2 != 0 && isDoc) { //Firefox does not give HTML for document. Get it from its descendant <BODY>.
								hr = hr2;
								var childHTML = _FindChild(x.childCount, NodeType.Element, "HTML", out var cc2);
								if(!childHTML.Is0) {
									//get BODY, not whole HTML. Like IE and Chrome.
									var childBODY = childHTML._FindChild(cc2, NodeType.Element, "BODY", out _);
									if(!childBODY.Is0) {
										hr = childBODY.GetOuterHTML(out s);
										childBODY.Dispose();
									}
									childHTML.Dispose();
								}
								Util.LibStringBuilderCache.Release(b);
								return hr;
							}
						}
						_HtmlAppendTail(b, x.tag);
						s = b.ToStringCached_();
					}
				}
				return hr;
			}

			ISimpleDOMNode _FindChild(int childCount, NodeType nodeType, string tag, out int childChildCount)
			{
				//search in reverse order, because usually what we need is the last child.
				//	Document often has 2 children: doctype and HTML.
				//	HTML usually has 2 children: HEAD and BODY.
				for(int i = childCount; i > 0; i--) {
					if(0 != _F.get_childAt(_iptr, i - 1, out var child)) continue;
					if(0 == child.GetNodeInfo(out var info, false)
						&& info.nodeType == nodeType
						&& info.tag.Equals_(tag, true)
						) {
						childChildCount = info.childCount;
						return child;
					}
					child.Dispose();
				}
				childChildCount = 0;
				return default;
			}

			//currently not used
			//public int GetTag(out string s)
			//{
			//	int hr = GetNodeInfo(out var x, needText: false);
			//	s = hr == 0 ? x.tag : null;
			//	return hr;
			//}

			//currently not used
			//public int parentNode(out ISimpleDOMNode parent)
			//{
			//	return _F.get_parentNode(_iptr, out parent);
			//}

			//currently not used
			///// <summary>
			///// Gets root node.
			///// It is root of web page, frame, UI, or an UI part, depending on where this node is.
			///// Returns this if this is root (then disposeThis is ignored).
			///// </summary>
			//public ISimpleDOMNode GetRoot(bool disposeThis)
			//{
			//	ISimpleDOMNode r = this;
			//	for(; 0 == r.parentNode(out var temp) && !temp.Is0; disposeThis = true) {
			//		if(disposeThis) r.Dispose();
			//		r._iptr = temp._iptr;
			//	}
			//	return r;
			//}

			public int scrollTo(byte placeTopLeft)
			{
				return _F.scrollTo(_iptr, placeTopLeft);
			}

			static ConcurrentDictionary<LPARAM, _Vtbl> s_vtbls = new ConcurrentDictionary<LPARAM, _Vtbl>();

			_Vtbl _F
			{
				get
				{
					if(_iptr == default) throw new ObjectDisposedException(nameof(ISimpleDOMNode));
					return s_vtbls.GetOrAdd(*(IntPtr*)_iptr, vtbl => new _Vtbl(vtbl));
				}
			}

			class _Vtbl
			{
				public _Vtbl(long vtbl)
				{
#if DEBUG
					int n = s_vtbls.Count; if(n > 0) Debug_.Print("many VTBLs: " + (n + 1));
#endif
					var a = (IntPtr*)vtbl;
					Util.Marshal_.GetDelegate(a[3], out get_nodeInfo);
					Util.Marshal_.GetDelegate(a[4], out get_attributes);
					Util.Marshal_.GetDelegate(a[5], out get_attribute);
					//Util.Marshal_.GetDelegate(a[5], out get_attributesForNames);
					Util.Marshal_.GetDelegate(a[8], out scrollTo);
					//Util.Marshal_.GetDelegate(a[9], out get_parentNode); //currently not used
					Util.Marshal_.GetDelegate(a[14], out get_childAt);
					Util.Marshal_.GetDelegate(a[15], out get_innerHTML);
				}

				internal readonly get_nodeInfoT get_nodeInfo;
				internal readonly get_attributesT get_attributes;
				//internal readonly get_attributesForNamesT get_attributesForNames;
				internal readonly get_attributeT get_attribute;
				//internal readonly get_parentNodeT get_parentNode;
				internal readonly scrollToT scrollTo;
				internal readonly get_childAtT get_childAt;
				internal readonly LibDelegateTypes.IntPtr_out_string get_innerHTML;

				//internal delegate int get_attributesT(IntPtr obj, int num, BSTR* names, short* nameSpaceID, BSTR* values, out ushort numHave);
				internal delegate int get_nodeInfoT(IntPtr obj, out BSTR tag, out short nameSpaceID, out BSTR text, out int nChildren, out int uid, out NodeType nodeType); //no optionals
				internal delegate int get_attributesT(IntPtr obj, int num, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr), Out] string[] names, [Out] short[] nameSpaceID, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr), Out] string[] values, out int numHave);
				internal delegate int get_attributeT(IntPtr obj, int _1, [MarshalAs(UnmanagedType.BStr), In] ref string name, out short nameSpaceID, out BSTR value);
				//internal delegate int get_attributesForNamesT(IntPtr obj, int num, BSTR* names, short* nameSpaceID, BSTR* values);
				internal delegate int scrollToT(IntPtr obj, byte placeTopLeft);
				//internal delegate int get_parentNodeT(IntPtr obj, out ISimpleDOMNode parent); //currently not used
				internal delegate int get_childAtT(IntPtr obj, int childIndex, out ISimpleDOMNode child);
			}
		}

		//ISimpleDOMText

		internal struct ISimpleDOMText :IDisposable
		{
			IntPtr _iptr;

			public static bool FromNode(ISimpleDOMNode node, out ISimpleDOMText text)
			{
				return Util.Marshal_.QueryInterface(node, out text, ref IID_ISimpleDOMText);
			}

			public void Dispose()
			{
				if(_iptr != default) {
					Marshal.Release(_iptr);
					_iptr = default;
				}
			}

			public int GetText(out string s)
			{
				int hr = _F.get_domText(_iptr, out var b);
				s = hr == 0 ? b.ToStringAndDispose() : null;
				return hr;
			}

			static ConcurrentDictionary<LPARAM, _Vtbl> s_vtbls = new ConcurrentDictionary<LPARAM, _Vtbl>();

			_Vtbl _F
			{
				get
				{
					if(_iptr == default) throw new ObjectDisposedException(nameof(ISimpleDOMText));
					return s_vtbls.GetOrAdd(*(IntPtr*)_iptr, vtbl => new _Vtbl(vtbl));
				}
			}

			class _Vtbl
			{
				public _Vtbl(long vtbl)
				{
#if DEBUG
					int n = s_vtbls.Count; if(n > 0) Debug_.Print("many VTBLs: " + (n + 1));
#endif
					var a = (IntPtr*)vtbl;
					Util.Marshal_.GetDelegate(a[3], out get_domText);
				}

				internal readonly LibDelegateTypes.IntPtr_out_BSTR get_domText;
			}

			static Guid IID_ISimpleDOMText = new Guid("4e747be5-2052-4265-8af0-8ecad7aad1c0");
		}

		//ISimpleDOMDocument
		//rejected. Quite much work. Maybe in the future.
		//	Would need it only to get web page URL and title.
		//		Can get it from DOCUMENT/PANE Acc. In all 3 browsers, Name is title, Value is URL.
		//		The difference is that this gets URL/title of direct parent document, which can be iframe.
		//	Now implemented only Firefox/Chrome URL.
		//		internal struct ISimpleDOMDocument :IDisposable
		//		{
		//			IntPtr _iptr;

		//			public static bool FromNode(ISimpleDOMNode node, bool disposeNode, out ISimpleDOMDocument text)
		//			{
		//				return Util.Marshal_.QueryInterface(node.GetRoot(disposeNode), out text, ref IID_ISimpleDOMDocument);
		//			}

		//			public void Dispose()
		//			{
		//				if(_iptr != default) {
		//					Marshal.Release(_iptr);
		//					_iptr = default;
		//				}
		//			}

		//			public bool Is0 => _iptr == default;

		//			public int GetURL(out string s)
		//			{
		//				int hr = _F.get_URL(_iptr, out var b);
		//				s = hr == 0 ? b.ToStringAndDispose() : null;
		//				return hr;
		//			}

		//			public int GetTitle(out string s)
		//			{
		//				int hr = _F.get_title(_iptr, out var b);
		//				s = hr == 0 ? b.ToStringAndDispose() : null;
		//				return hr;
		//			}

		//			static ConcurrentDictionary<LPARAM, _Vtbl> s_vtbls = new ConcurrentDictionary<LPARAM, _Vtbl>();

		//			_Vtbl _F
		//			{
		//				get
		//				{
		//					if(_iptr == default) throw new ObjectDisposedException(nameof(ISimpleDOMDocument));
		//					return s_vtbls.GetOrAdd(*(IntPtr*)_iptr, vtbl => new _Vtbl(vtbl));
		//				}
		//			}

		//			class _Vtbl
		//			{
		//				public _Vtbl(long vtbl)
		//				{
		//#if DEBUG
		//					int n = s_vtbls.Count; if(n > 0) Debug_.Print("many VTBLs: " + (n + 1));
		//#endif
		//					var a = (IntPtr*)vtbl;
		//					Util.Marshal_.GetDelegate(a[3], out get_URL);
		//					Util.Marshal_.GetDelegate(a[4], out get_title);
		//				}

		//				internal readonly del.out_BSTR get_URL;
		//				internal readonly del.out_BSTR get_title;
		//			}

		//			static Guid IID_ISimpleDOMDocument = new Guid("0D68D6D0-D93D-4d08-A30D-F00DD1F45B24");
		//		}

		//		public WebPageProperties WebPage
		//		{
		//			get
		//			{
		//				return WebPageProperties.FromAcc(this);
		//			}
		//		}

		//		public class WebPageProperties :IDisposable
		//		{
		//			ISimpleDOMDocument _isdd;

		//			internal static WebPageProperties FromAcc(Acc a)
		//			{
		//				Native.ClearError();
		//				int hr = Api.E_FAIL;
		//				using(var k = new _BrowserInterface(a)) {
		//					if(!k.ie.Is0) {

		//					} else if(!k.ff.Is0) {
		//						if(ISimpleDOMDocument.FromNode(k.ff, false, out var doc))
		//							return new WebPageProperties() { _isdd = doc };
		//					} else hr = Api.E_NOINTERFACE;
		//				}
		//				a._Hresult(_FuncId.html, hr);
		//				return null;
		//			}

		//			///
		//			public void Dispose()
		//			{
		//				_isdd.Dispose();
		//			}

		//			///
		//			~WebPageProperties() => Dispose();


		//			public string URL
		//			{
		//				get
		//				{
		//					string s = null;
		//					if(!_isdd.Is0) _isdd.GetURL(out s);
		//					return s ?? "";
		//				}
		//			}
		//		}
	}
}
