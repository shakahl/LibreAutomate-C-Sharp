using System;
using System.Collections.Generic;
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
	public partial class Acc
	{
		/// <summary>
		/// Simple IAccessible/childId pair.
		/// </summary>
		internal struct _Acc :IDisposable
		{
			public IAccessible a;
			public int elem;

			public _Acc(IAccessible a, int elem) { this.a = a; this.elem = elem; }

			public void Dispose() { a.Dispose(); elem = 0; }

			public string ToString(int level) => a.ToString(elem, level);
			public override string ToString() => ToString(0);

			/// <summary>
			/// Navigates to another object and replaces fields of this _Acc.
			/// Returns HRESULT.
			/// This will be empty/disposed if returns not 0. Else the caller must finally dispose this new _Acc.
			/// navig must be valid, for example created by NavigN.Parse. This function does not check it.
			/// </summary>
			public int Navigate(List<NavdirN> navig)
			{
				for(int i = 0; i < navig.Count; i++) {
					int hr = Navigate(navig[i].navDir, navig[i].n);
					if(hr != 0) return hr;
				}
				return 0;
			}

			/// <summary>
			/// Navigates to another object and replaces fields of this _Acc.
			/// Returns HRESULT.
			/// This will be empty/disposed if returns not 0. Else the caller must finally dispose this new _Acc.
			/// n is child index if CHILD (cannot be 0), else it is the number of navigations (cannot be less than 1). This function does not check it.
			/// </summary>
			public int Navigate(AccNAVDIR navDir, int n)
			{
				int childIndex; if(navDir == AccNAVDIR.CHILD) { childIndex = n; n = 1; } else childIndex = 0;
				while(n-- > 0) {
					int hr = _Navigate(out var t, navDir, childIndex);
					if(t.a != this.a) Dispose();
					this = t;
					if(hr != 0) return hr;
				}
				return 0;
			}

			int _Navigate(out _Acc ar, AccNAVDIR navDir, int childIndex)
			{
				ar = default;
				int hr = 0;
				if(this.elem != 0 && navDir >= AccNAVDIR.FIRSTCHILD && navDir <= AccNAVDIR.CHILD) { //first, last, parent, child
					if(navDir == AccNAVDIR.PARENT) ar.a = this.a;
					else hr = Api.S_FALSE;
				} else {
					if(navDir == AccNAVDIR.CHILD) {
						using(var c = new _Children(this.a, childIndex, exactIndex: true)) {
							c.GetNext(out ar);
						}
						//Acc FUTURE: test, maybe significantly faster with IEnumVARIANT/get_accChild, like in QM2. But with some objects it does not work.
					} else {
						if(navDir == AccNAVDIR.PARENT) {
							hr = this.a.get_accParent(out ar.a);
						} else {
							hr = this.a.accNavigate(navDir, this.elem, out ar);
							if(hr != 0) {
								//Perf.First();
								if(_NavigateAlt(out ar, navDir)) hr = 0;
								//Perf.NW();
							}
						}
					}
				}
				Debug.Assert((hr != 0) == ar.a.Is0);
				return hr;
			}

			/// <summary>
			/// For objects that don't support accNavigate, tries a workaround if navDir is FIRSTCHILD, LASTCHILD, NEXT or PREVIOUS.
			/// Many objects don't support it, eg WPF, SPLITBUTTON, Explorer listview. MSDN says it's deprecated. Also, many objects skip invisible siblings.
			/// </summary>
			bool _NavigateAlt(out _Acc ar, AccNAVDIR navDir)
			{
				ar = default;
				bool R = false;
				if(navDir == AccNAVDIR.FIRSTCHILD || navDir == AccNAVDIR.LASTCHILD) {
					if(this.elem != 0) return false;
					using(var c = new _Children(this.a, navDir == AccNAVDIR.FIRSTCHILD ? 1 : -1, exactIndex: true)) {
						R = c.GetNext(out ar);
					}
					//never mind: maybe significantly faster with IEnumVARIANT/get_accChild. But less reliable. Anyway, this is much faster than the NEXT/PREVIOUS code.
				} else if(navDir == AccNAVDIR.NEXT || navDir == AccNAVDIR.PREVIOUS) {
					//Get parent, then enum its children to find this by rect/role and get next.
					//	note: cannot compare IAccessibles, they always different.
					IAccessible iaccParent; bool releaseParent;
					if(this.elem != 0) {
						iaccParent = this.a;
						releaseParent = false;
						//never mind: faster would be just to use this.elem-1 or this.elem+1 (with get_accChildCount) and try get_accChild. But less reliable (for some AO get_accChild fails). Anyway, non-0 this.elem is quite rare.
					} else {
						if(0 != this.a.get_accParent(out iaccParent)) return false;
						releaseParent = true;
						//note: will get wrong object if get_accParent is broken.
						//	For example WinForms TOOLBAR gets parent WINDOW which does not exist in the tree (must get parent of that WINDOW).
						//		Then this func gets WINDOW's child MENUBAR or SCROLLBAR which does not exist in the tree.
						//		Noticed it in our editor.
						//	We cannot detect/workaround it, or it would be too difficult/unreliable/slow. Better let the user try another way.
					}
					try {
						using(var c = new _Children(iaccParent, reverse: navDir == AccNAVDIR.PREVIOUS)) {
							bool found = false; _AccProps props = default;
							while(c.GetNext(out var t)) {
								try {
									if(!found) {
										if(!props.Init(this.a, this.elem)) break;
										if(!props.Match(t.a, t.elem)) continue;
										found = true;
									} else {
										if(t.a == iaccParent) releaseParent = false;
										ar = t; t.a = default;
										R = true;
										break;
									}
								}
								finally { c.FinallyInLoop(t); }
							}
						}
					}
					finally { if(releaseParent) iaccParent.Dispose(); }
				}
				//if(R) Print("<><c 0x8000>" + t.a.ToString(t.elem) + "</c>");
				return R;
			}

			public struct NavdirN
			{
				public AccNAVDIR navDir;
				public int n;

				/// <summary>
				/// Converts navig string to List{_NavdirN}.
				/// Throws ArgumentException if navig is invalid, eg contains unknown strings or invalid n.
				/// </summary>
				public static List<NavdirN> Parse(string navig)
				{
					var a = new List<NavdirN>();
					foreach(var s in navig.Segments_(" ", SegFlags.NoEmpty)) {
						int navDir, offs = s.Offset;
						if(navig[offs] == '#') { //custom, or by numeric value
							navDir = navig.ToInt32_(offs + 1, out offs);
							if(offs == 0 || offs > s.End) goto ge;
						} else { //standard, by name
							navDir = navig.EqualsAt_(offs, false, c_aNavig);
							if(navDir == 0) goto ge;
							offs += c_aNavig[navDir - 1].Length;
							if(navDir <= 6) navDir += 4; else if(navDir <= 12) navDir -= 2; else navDir -= 8;
						}
						int n = 1;
						if(offs < s.End) {
							if(navig[offs] == ',') offs++;
							n = navig.ToInt32_(offs, out offs);
							if(offs != s.End || n == 0 || (n < 0 && navDir != (int)AccNAVDIR.CHILD)) goto ge; //note: negative n is valid with child
						}

						a.Add(new NavdirN() { navDir = (AccNAVDIR)navDir, n = n });
					}
					return a;
					ge:
					throw new ArgumentException("Invalid navig string.");
				}

				readonly static string[] c_aNavig = new string[] {
				"next", "previous", "first", "last", "parent", "child",
				"ne", "pr", "fi", "la", "pa", "ch",
				"n", "prev", "f", "l", "p", "c"
				};
			}

			/// <summary>
			/// Gets object from point, which can be a child, descendant or this.
			/// Returns HRESULT if fails or if p is not in this.
			/// If isThis receives true, does not AddRef. Else does AddRef even if ar.a==this.a.
			/// </summary>
			/// <param name="p">Point in screen coordinates.</param>
			/// <param name="ar">Result.</param>
			/// <param name="isThis">Receives true if ar is this.</param>
			/// <param name="directChild"></param>
			public int DescendantFromPoint(Point p, out _Acc ar, out bool isThis, bool directChild = false)
			{
				isThis=false;
				int hr = this.a.accHitTest(p.X, p.Y, out ar);
				if(hr != 0) return hr;
				if(ar.a == this.a){
					if(ar.elem == this.elem) {
						isThis = true;
						return 0;
					}
					ar.a.AddRef();
				} else if(!directChild) {
					if(0 == ar.DescendantFromPoint(p, out var t, out var isThis2) && !isThis2) {
						Math_.Swap(ref ar, ref t);
						t.Dispose();
					}
				}
				return 0;
			}

			/// <summary>
			/// Gets focused, which can be a child, descendant or this.
			/// Returns HRESULT if fails or if nothing is focused in this.
			/// If isThis receives true, does not AddRef. Else does AddRef even if ar.a==this.a.
			/// </summary>
			/// <param name="ar">Result.</param>
			/// <param name="isThis">Receives true if ar is this.</param>
			/// <param name="directChild"></param>
			public int DescendantFocused(out _Acc ar, out bool isThis, bool directChild = false)
			{
				isThis=false;
				int hr = this.a.get_accFocus(out ar);
				if(hr != 0) return hr;
				if(ar.a == this.a){
					if(ar.elem == this.elem) {
						isThis = true;
						return 0;
					}
					ar.a.AddRef();
				} else if(!directChild) {
					if(0 == ar.DescendantFocused(out var t, out var isThis2) && !isThis2) {
						Math_.Swap(ref ar, ref t);
						t.Dispose();
					}
				}
				return 0;
			}

		}

		/// <summary>
		/// Compares an object with other objects using some properties - rectangle, role.
		/// </summary>
		struct _AccProps
		{
			RECT _rect;
			string _role;

			/// <summary>
			/// Gets properties of the object that will be compared with other objects.
			/// Does nothing if already done (can be called multiple times).
			/// Returns false if failed to get properties.
			/// </summary>
			public bool Init(IAccessible a, int elem)
			{
				if(_role == null) {
					if(0 != a.accLocation(elem, out _rect) || _rect.IsEmpty) return false;
					if(0 != a.GetRoleString(elem, out _role)) return false;
				}
				return true;
			}

			/// <summary>
			/// Compares properties of another object with properties of the Init object.
			/// Returns true if they match.
			/// Init must be called (asserts).
			/// </summary>
			public bool Match(IAccessible a, int elem)
			{
				Debug.Assert(_role != null);
				if(0 != a.accLocation(elem, out RECT rect2) || rect2 != _rect) return false;
				if(0 != a.GetRoleString(elem, out string role2) || role2 != _role) return false;
				return true;
			}
		}
	}
}
