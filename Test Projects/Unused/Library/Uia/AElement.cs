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
	/// <summary>
	/// UI Automation element.
	/// </summary>
	public static partial class AElement
	{
		[ThreadStatic] static UIA.IUIAutomation t_uiautomation;
		//static Lazy<UIA.IUIAutomation> c_uiautomation = new Lazy<UIA.IUIAutomation>(() => new UIA.CUIAutomation() as UIA.IUIAutomation, true);
		//maybe don't need [threadstatic], but I don't know whether its methods are thread-safe.

		[ThreadStatic] static UIA.ICondition t_condVisible, t_condRaw; //not offscreen, RawViewCondition
		[ThreadStatic] static UIA.ICondition t_condVisibleGroup, t_condVisibleDocument, t_condVisiblePane; //not offscreen Group, Document, Pane
		[ThreadStatic] static UIA.ITreeWalker t_walkerVisible; //not offscreen

		/// <summary>
		/// Gets the UIA.IUIAutomation object that is used by static functions of this class.
		/// The object is [ThreadStatic].
		/// </summary>
		public static UIA.IUIAutomation Factory
		{
			get
			{
				var f = t_uiautomation;
				if(f == null) {
					t_uiautomation = f = new UIA.CUIAutomation() as UIA.IUIAutomation;

					t_condVisible = f.CreatePropertyCondition(UIA.PropertyId.IsOffscreen, false);
					t_condRaw = f.RawViewCondition;

					t_condVisibleGroup = f.CreateAndCondition(f.CreatePropertyCondition(UIA.PropertyId.ControlType, UIA.TypeId.Group), t_condVisible);
					t_condVisibleDocument = f.CreateAndCondition(f.CreatePropertyCondition(UIA.PropertyId.ControlType, UIA.TypeId.Document), t_condVisible);
					t_condVisiblePane = f.CreateAndCondition(f.CreatePropertyCondition(UIA.PropertyId.ControlType, UIA.TypeId.Pane), t_condVisible);

					t_walkerVisible = f.CreateTreeWalker(t_condVisible);
				}
				return f;
			}
		}

		//rejected: not useful
		//public static UIA.IElement Root => Factory.GetRootElement();

		static UIA.IElement _FromWindow(Wnd w)
		{
			w.ThrowIfInvalid();
			var f = Factory;
			UIA.IElement e = null;
			using(new LibTempSetScreenReader(false)) {
				e = f.ElementFromHandle(w); //TODO: can throw if w destroyed just now
			}

			return e;
		}

		/// <summary>
		/// Gets UI Automation element of a window.
		/// </summary>
		/// <exception cref="WndException">Invalid window.</exception>
		public static UIA.IElement FromWindow(Wnd w)
		{
			return _FromWindow(w);
		}

		/// <summary>
		/// Gets UI Automation element corresponding to an accessible object.
		/// Returns null if fails.
		/// </summary>
		/// <param name="ao">Accessible object.</param>
		/// <param name="find">
		/// If false, uses IUIAutomation.ElementFromIAccessible. It fails for most objects. Also, the returned element may be useless (some methods are not supported or very slow).
		/// If true, finds element that matches ao properties. It is slow, but usually succeeds.
		/// If null (default), uses the first method, then the second method if the first fails.
		/// This parameter is not used when the Acc variable was found with flag <see cref="AFFlags.UIAutomation"/>. Then this function quickly and reliably returns the original UI element.
		/// </param>
		public static UIA.IElement FromAcc(Acc ao, bool? find = null)
		{
			if(ao == null) throw new ArgumentNullException();
			ao.LibThrowIfDisposed();

			//This quick way works
			var guid = typeof(UIA.IElement).GUID;
			if(0 == Marshal.QueryInterface(ao._iacc, ref guid, out IntPtr ielem)) {
				var r = Unsafe.As<UIA.IElement>(Marshal.GetObjectForIUnknown(ielem));
				Marshal.Release(ielem);
				return r;
				//note: currently this does not work. QI would not work for inproc AO. If need, try QS, not tested.
			}

			//CONSIDER: move [part of it] to cpp.
			//CONSIDER: support only our Acc that wrap UIA element. Anyway, ElementFromIAccessible makes lame elements, and FindFirst is slow/dirty/unreliable.

			if(!find.GetValueOrDefault()) { //if null or false
				var a = Unsafe.As<UIA.IAccessible>(Marshal.GetUniqueObjectForIUnknown(ao._iacc));
				Debug.Assert(a != null);
				try {
					var R = Factory.ElementFromIAccessible(a, ao._elem);
					if(R != null) return R;
				}
				catch { } //eg native ElementFromIAccessible returned E_INVALIDARG and it was translated to ArgumentException
				finally { Api.ReleaseComObject(a); }
			}

			//CONSIDER: remove this option. Slow and unreliable.
			if(!find.HasValue || find.GetValueOrDefault()) { //null or true
				try {
					if(ao.RoleEnum == 0) return null;
					if(!ao.GetRect(out var r) || r.IsEmpty) return null;
					var w = ao.WndContainer; if(w.Is0) return null;
					var ew = Factory.ElementFromHandle(w);
					var cond = new ACondition().Rect(r).LARole(ao.RoleEnum).Add(UIA.PropertyId.LegacyIAccessibleState, ao.State);
					var s = ao.Name; if(s.Length > 0) cond.Add(UIA.PropertyId.LegacyIAccessibleName, s);
					return ew.FindFirst(UIA.TreeScope.Subtree, cond.Condition);

					//note: cannot search only in ancestors whose rectangles intersect with r, because child rect not always is in parent rect.

					//Chrome bug: links that are offscreen when the page loaded, still have offscreen style when made visible (scrolled).
					//	Result: IElement.BoundingRectangle is empty.
					//	And vice versa: links that are made offscreen by scrolling don't have offscreen style.
				}
				catch { }
			}
			return null;
		}

		public static UIA.IElement FromPoint(Coord x, Coord y, CoordOptions co = null, bool preferLINK = false)
		{
			var p = Coord.Normalize(x, y, co);
			return FromPoint(p, preferLINK);
		}

		public static UIA.IElement FromPoint(Point pScreen, bool preferLINK = false)
		{
			//Perf.First();
			var f = Factory;
			UIA.IElement e;
			using(new LibTempSetScreenReader(false)) {
				e = f.ElementFromPoint(pScreen);
			}
			//Perf.Next();

			//workarounds
			var wTL = Wnd.FromXY(pScreen, WXYFlags.NeedWindow); //10-20 times faster than NativeWindowHandle
			if(wTL.ClassNameIs("Chrome*")) {
				//Chrome bug workaround: gets PANE of "Intermediate D3D Window" control.
				var wElem = e.NativeWindowHandle; //slow, but ~10 times faster than FromPoint
				bool isD3D = wElem.ClassNameIs("Intermediate D3D Window");
				if(isD3D && !wElem.WndNext.Is0 && wElem.ZorderBottom()) {
					//Print("---- Intermediate D3D Window ----");
					e = f.ElementFromPoint(pScreen);
					isD3D = wElem == e.NativeWindowHandle;
				}
				//ZorderBottom works for web page but not for other area - toolbar, address bar, tab buttons, etc.
				//	In the future may stop working with web page too, if they'll remove the legacy "Chrome_RenderWidgetHostHWND" control.
				//	Note: in web page the bug disappears after tab switching. Then Chrome itself zorders the D3D control behind the render controls.
				if(isD3D) {
					//Perf.Next();
#if false //slower. With Chrome (current version) this works better, but I don't want to add all that code just for this workaround.
					e = _DescendantFromPoint(f.ElementFromHandle(wTL), pScreen, true) ?? e; //60 ms
#else
					//this code works with non-web area, but would fail with web area because then Chrome does not give element from Acc.
					//Also the element may be half-valid, eg searching int it can be very slow. Not tested in this case.
					using(var acc = Acc.FromXY(pScreen.X, pScreen.Y, flags: AXYFlags.NoThrow)) { //5 ms
						if(acc != null) {
							var e2 = FromAcc(acc, false); //15-20 ms (ElementFromPoint 7 ms)
							Debug_.PrintIf(e2 == null, "fails to get element from Acc");
							//if(e2 == null) e2 = FromAcc(acc, true); //no, it's too slow, especially if large web page.
							e = e2 ?? e;
						}
					}
#endif
				}

				//Enable Chrome web page elements.
				if(!Wnd.Lib.WinFlags.Get(wTL).HasAny_(Wnd.Lib.WFlags.ChromeYes | Wnd.Lib.WFlags.ChromeNo)) {
					var ew = f.ElementFromHandle(wTL);
					var e1 = ew.FindFirst(UIA.TreeScope.Children, t_condVisibleDocument);
					if(e1 == null) e1 = ew.FindFirst(UIA.TreeScope.Descendants, t_condVisibleDocument); //note: this must be before enabling Chrome, or enabling fails when Chrome window is inactive
					if(e1 == null || null == e1.FindFirst(UIA.TreeScope.Children, t_condRaw)) {
						//PrintList("disabled", e1 != null);
						e1 = _ChromeEnable(wTL, ew);
						e = f.ElementFromPoint(pScreen);
					}
					Wnd.Lib.WinFlags.Set(wTL, e1 == null ? Wnd.Lib.WFlags.ChromeNo : Wnd.Lib.WFlags.ChromeYes, SetAddRemove.Add);
					//TODO: do we need both ChromeNo and ChromeYes. Maybe use single, eg ChromeEnabingDone.
				}
			}

			if(preferLINK) {
				//Perf.Next();
				UIA.IElement ep = null;
				try { ep = t_walkerVisible.GetParentElement(e); } //tested: as always, with cache slower
				catch(Exception ex) { Debug_.Print(ex); } //TODO: add exception handling everywhere
				if(ep != null) {
					switch(ep.ControlType) {
					case UIA.TypeId.Hyperlink:
					case UIA.TypeId.Button:
					case UIA.TypeId.SplitButton:
						switch(e.ControlType) {
						case UIA.TypeId.Hyperlink: case UIA.TypeId.Button: case UIA.TypeId.SplitButton: break;
						default: e = ep; break;
						}
						break;
					}
				}

				//tested: slightly faster with walker that uses Or condition for control types. But then Edge throws exception, which makes very slow.
			}

			//Perf.NW();
			return e;
		}

		public static UIA.IElement FromMouse(bool preferLINK = false)
		{
			return FromPoint(Mouse.XY, preferLINK);
		}

		//rejected: either too unreliable or too slow.
		//	The problem is that children of some elements are not in element's rect.
		//	The code moved to the Unused project.
		//public static UIA.IElement FromPoint(Wnd w, Point pScreen)

		internal static string LibToString_(this UIA.IElement e)
		{
			if(e == null) return null;
			var st = e.ControlType.ToString();
			var sn = e.Name;
			var sw = e.NativeWindowHandle.ToString();
			return $"{st}  name='{sn}'  window='{sw}'";
		}
	}
}

namespace Catkeys.Types
{
#pragma warning disable CS0419 // Ambiguous reference in cref attribute

	/// <summary>
	/// Flags for <see cref="AElement.Find"/>
	/// </summary>
	public enum EFFlags
	{

	}
#pragma warning restore CS0419 // Ambiguous reference in cref attribute

	/// <summary>
	/// Temporarily sets SPI_SETSCREENREADER. Restores in Dispose.
	/// It enables accessible objects and UI automation elements (AO/AE) in OpenOffice.
	/// Speed: 10-20 mcs, which is about 20% of accessibleobjectfromwindow.
	/// Note: LibreOffice AE does not work. AO does not work if this process is 64-bit (why?). Although SPI_SETSCREENREADER works.
	/// </summary>
	unsafe struct LibTempSetScreenReader :IDisposable
	{
		bool _restore;

		/// <summary>
		/// If SPI_GETSCREENREADER says false, sets SPI_SETSCREENREADER = true, and Dispose will set it false.
		/// Note: Windows does not use a reference counting for this setting.
		/// </summary>
		public LibTempSetScreenReader(bool unused)
		{
			int r = 0;
			Api.SystemParametersInfo(Api.SPI_GETSCREENREADER, 0, &r, 0);
			_restore = r == 0 && Api.SystemParametersInfo(Api.SPI_SETSCREENREADER, 1, 0, 0);
		}

		public void Dispose()
		{
			if(_restore) _restore = !Api.SystemParametersInfo(Api.SPI_SETSCREENREADER, 0, 0, 0);
		}
	}
}
