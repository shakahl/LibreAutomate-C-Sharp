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

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	public static partial class AElement
	{
		static UIA.IElement _DefaultWebPageFinder(Wnd w)
		{
			//info: in current Opera versions everything is the same as in Chrome, even window class name.

			var f = Factory;

			int browser = w.ClassNameIs("Mozilla*", "Chrome*", Api.string_IES, "Windows.UI.Core.CoreWindow", "ApplicationFrameWindow");
			if(!Ver.MinWin10) { //no Edge
				if(browser == 4) browser = 0; //IE Metro-style. Find child control.
			} else if(browser == 5) { //Edge in "ApplicationFrameWindow"
				var core = w.Child(className: "Windows.UI.Core.CoreWindow");
				if(!core.Is0) { browser = 4; w = core; } else browser = 0;
			}
			if(browser == 0) {
				var ies = w.Child(className: Api.string_IES);
				if(!ies.Is0) { w = ies; browser = 3; }
			}
			//Print(browser);

			var ew = f.ElementFromHandle(w);

			//Firefox, Edge
			if(browser == 1 || browser == 4) {
				//In current versions, ew has many direct children. One of them is GROUP, and it contains ancestor DOCUMENT/PANE.
				//In Edge there are no branches in the path from GROUP to PANE. In Firefox more difficult; more comments in _FindFF.

				if(browser == 1) { //Firefox
					var t = _FindFF(ew, 0);
					if(t != null) return t;
				} else { //Edge
					var a = ew.FindAll(UIA.TreeScope.Children, t_condVisibleGroup);
					int n = a.Length;
					if(n > 0) {
						var condEdge = f.CreateAndCondition(t_condVisiblePane, f.CreatePropertyCondition(UIA.PropertyId.IsTextPatternAvailable, true)); //other PANE in the path don't have this pattern
						for(int i = 0; i < n; i++) {
							var e3 = a.GetElement(i);
							var t = e3.FindFirst(UIA.TreeScope.Descendants, condEdge);
							if(t != null) return t;
							Debug_.PrintIf(i == 0 && n > 1, "not found in first GROUP");
						}
					}
					browser = 3; //in current version there are no other PANE, so should find the web page PANE anyway, although it probably will be an ancestor of the true web page PANE
				}
				Debug_.Print("web page not found");
			}

			UIA.ICondition condType;
			switch(browser) {
			default: condType = t_condVisibleDocument; break; //Chrome/Opera, unknown, failed Firefox
			case 3: condType = t_condVisiblePane; break; //IE, failed Edge
			}

			var e1 = ew.FindFirst(UIA.TreeScope.Children, condType); //current Chrome and IE versions
			if(e1 == null) e1 = ew.FindFirst(UIA.TreeScope.Descendants, condType); //note: this must be before enabling Chrome, or enabling fails when Chrome window is inactive
			if(browser == 2) { //Chrome/Opera. Maybe web page elements disabled.
							   //Perf.First();
				if(!Wnd.Lib.WinFlags.Get(w).HasAny_(Wnd.Lib.WFlags.ChromeYes | Wnd.Lib.WFlags.ChromeNo)) {
					if(e1 == null || null == e1.FindFirst(UIA.TreeScope.Children, t_condRaw)) {
						//When disabled, there is a ghost element of type Document, or first time can be Pane.
						//	When Document, it is offscreen.
						//	When Pane, enabling fails. The above 'FindFirst(UIA.TreeScope.Descendants)' makes it Document. I could not find a better way.
						//The 'FindFirst(condType)' does not find it (e1==null). If would find, the next 'FindFirst' would detect when disabled.

						//PrintList("disabled", e1!=null);
						e1 = _ChromeEnable(w, ew);
					}
					Wnd.Lib.WinFlags.Set(w, e1 == null ? Wnd.Lib.WFlags.ChromeNo : Wnd.Lib.WFlags.ChromeYes, SetAddRemove.Add);
				}
				//Perf.NW();
			}
			if(e1 != null) return e1;
			Output.Warning("Failed to find web page in this window. More info in " + nameof(AElement) + "." + nameof(FindCurrentWebPage) + " help.", -1);
			return ew;

			UIA.IElement _FindFF(UIA.IElement eParent, int level)
			{
				//In current versions, ew has many direct children.
				//One of them is GROUP. It contains children for tabs. They contain ancestor DOCUMENT.
				//Normally the active tab is the first, and inactive DOCUMENTs are marked offscreen.
				//But in FF 58 Nightly the active tab can be the last. Inactive DOCUMENTs are not marked offscreen. We cannot use FindFirst.
				//	But one element in the path is marked offscreen. We can use recursive code.

				var a = eParent.FindAll(UIA.TreeScope.Children, level == 0 ? t_condVisibleGroup : t_condVisible);
				for(int i = 0, n = a.Length; i < n; i++) {
					var e2 = a.GetElement(i);
					if(level > 0) { //at level 0 is a GROUP
						if(e2.ControlType == UIA.TypeId.Document) return e2;
					}
					if(level < 5) { //limit level. In current version it is at level 3.
						e2 = _FindFF(e2, level + 1);
						if(e2 != null) return e2;
					}
				}
				return null;
			}
		}
		static Func<Wnd, UIA.IElement> _defaultWebPageFinder = _DefaultWebPageFinder;

		/// <summary>
		/// Enables UI Automation in Chrome/Opera web page.
		/// </summary>
		/// <param name="w">The main window.</param>
		/// <param name="ew">Element of the main window.</param>
		static UIA.IElement _ChromeEnable(Wnd w, UIA.IElement ew)
		{
			using(var a = Acc.FromWindow(w, AccOBJID.CLIENT)) { //throws if failed

				//This code starts enabling Chrome/Opera web page elements.
				//IAccessible2 normally is unavailable, but enabling works anyway.
				//ew is CLIENT of the main window. In old Chrome versions would need to use the ghost DOCUMENT instead.
				if(Util.Marshal_.QueryService(a._iacc, out IntPtr ia2, ref Api.IID_IAccessible, ref Api.IID_IAccessible2)) Marshal.Release(ia2);

				//Wait until enabling finished.
				//int n = 0;
				for(int i = 10; i < 150;) //max 11 s. The required time depends on page size. When fast CPU, rarely need 10 loops.
				{
					//n++;
					Thread.Sleep(i++); //tested: Send(0) does not work.
					var e = ew.FindFirst(UIA.TreeScope.Children, t_condVisibleDocument);
					if(e != null) {
						//does it have children? In Current Chrome version don't need it, but maybe will need in the future.
						if(i < 80) { //3115 ms. Don't wait more, maybe the document is empty, although I never saw it.
							bool hasChildren = null != e.FindFirst(UIA.TreeScope.Children, t_condRaw);
							Debug_.PrintIf(!hasChildren, "0 children");
							if(!hasChildren) continue;
						}
						//var state = (e.GetCurrentPattern(UIA.PatternId.LegacyIAccessible) as UIA.ILegacyIAccessiblePattern).State;
						//PrintList(n, state, hasChildren);
						return e;
					}
				}
				//This is reliable with most pages, ie the enabling is finished when this function returns.
				//Unreliable with pages containing frames, eg http://referencesource.microsoft.com/#q=Control.
				//	Then will not find object until finished enabling in that frame.
				//	We cannot know when enabling finished.
				//	Never mind. Let the user use a 'wait' function for such pages.
			}

			return null;
		}

		/// <summary>
		/// Gets or sets a custom function to find the root element of the current web page in a window.
		/// </summary>
		/// <remarks>
		/// The function is called by <see cref="WebFind"/> and <see cref="FindCurrentWebPage"/>.
		/// If null, is used default finder.
		/// You can use a custom finder when the default finder does not work. It can happen after updating your web browser or if your web browser is not recognized by the default finder.
		/// </remarks>
		public static Func<Wnd, UIA.IElement> WebPageFinder { get; set; }

		/// <summary>
		/// Finds the root element of the current web page (tab) in window w.
		/// </summary>
		/// <param name="w">Window containing web page. Or web browser control, class name "Internet Explorer_Server".</param>
		/// <exception cref="WndException">This window is invalid.</exception>
		/// <exception cref="AuException">Failed.</exception> //TODO: everywhere
		/// <remarks>
		/// Supports Firefox, Chrome, Internet Explorer (IE), Edge and apps that use their code to display web page - Opera, Thunderbird, etc.
		/// The element type is Document (in Firefox, Chrome, Opera, Thunderbird) or Pane (in IE, Edge and IE-based web browser controls).
		/// 
		/// This function is used by <see cref="WebFind"/>. It allows to avoid searching in the non-web part of the window and in inactive tabs (currently it is actual in Firefox and Thunderbird).
		/// 
		/// May stop working with new versions of these apps, until this library will be updated to fix it. See also <see cref="WebPageFinder"/>.
		/// Web page elements in Chrome and Opera are disabled by default. This function tries to enable. It may fail in new versions of these app.
		/// </remarks>
		public static UIA.IElement FindCurrentWebPage(Wnd w)
		{
			w.ThrowIfInvalid();
			var k = WebPageFinder; if(k == null) k = _defaultWebPageFinder;
			return k(w);
		}

		/// <summary>
		/// Finds an element in the active web page in window w.
		/// </summary>
		/// <remarks>
		/// The same as <see cref="Find"/>, but searches only in the active page (tab) and not in other parts of the window.
		/// To find the web page, uses <see cref="FindCurrentWebPage"/>. More info there.
		/// </remarks>
		public static UIA.IElement WebFind(Wnd w, string name = null, UIA.TypeId type = 0, object conditions = null, int flags = 0, Func<UIA.IElement, bool> also = null, int skip = 0, string navig = null)
		{
			var e = FindCurrentWebPage(w);
			if(e == null) throw new NotFoundException("Failed to find web page.");
			//Perf.Next();
			return Find(e, name, type, conditions, flags, also, skip, navig);
		}

		public static UIA.IElement Find(Wnd w, string name = null, UIA.TypeId type = 0, object conditions = null, int flags = 0, Func<UIA.IElement, bool> also = null, int skip = 0, string navig = null)
		{
			w.ThrowIfInvalid();
			var e = _FromWindow(w);
			return Find(e, name, type, conditions, flags, also, skip, navig);
		}


		internal enum TestMethod
		{
			Find,
			Walker,
			FindAll,
			WalkerCache,
			FindAllCache,
		}
		internal static TestMethod s_testMethod;

		public static UIA.IElement Find(UIA.IElement e, string name = null, UIA.TypeId type = 0, object conditions = null, int EFFlags = 0, Func<UIA.IElement, bool> also = null, int skip = 0, string navig = null)
		{
			if(e == null) throw new ArgumentNullException(nameof(e));

			var f = Factory;
			var cond = new ACondition();
			UIA.ICacheRequest cache = null;
			bool walk = also != null || skip > 0;

			Wildex wName = name;
			if(wName != null) {
				if(s_testMethod == TestMethod.Find) {
					cond.Name(wName.Text, wName.IgnoreCase);
				} else if(s_testMethod == TestMethod.WalkerCache || s_testMethod == TestMethod.FindAllCache) {
					cache = f.CreateCacheRequest();
					cache.AddProperty(UIA.PropertyId.Name);

					//no
					//cache.AutomationElementMode = UIA.AutomationElementMode.AutomationElementMode_Full;
					//cache.TreeScope = UIA.TreeScope.Subtree;
				}
			}
			//if(wName != null) {
			//	if(wName.TextType != Wildex.WildType.Text) {
			//		walk = true; //SHOULDDO: don't need to walk if Multi and all Text
			//		if(s_testMethod == TestMethod.WalkerCache || s_testMethod == TestMethod.FindAllCache) {
			//			cache = f.CreateCacheRequest();
			//			cache.AddProperty(UIA.PropertyId.Name);

			//			//no
			//			//cache.AutomationElementMode = UIA.AutomationElementMode.AutomationElementMode_Full;
			//			//cache.TreeScope = UIA.TreeScope.Subtree;
			//		}
			//	} else {
			//		cond.Name(wName.Text, wName.IgnoreCase);
			//		if(wName.Not) if(wName.Not) cond.Condition = f.CreateNotCondition(cond.Condition);
			//		wName = null;
			//	}
			//}

			if(type != 0) cond.Type(type);

			var c1 = cond.Condition ?? t_condRaw;
			//walk = true;
			//if(walk) {
			if(s_testMethod != TestMethod.Find) {
				//info: how tree walker works is undocumented. From my tests, it seems:
				//	GetFirstChildElement finds first matching descendant at any level.
				//	GetNextSiblingElement finds next matching element at any level (less, equal or greater than current element).
				//		But it does not search descendants of current element. Need to call GetFirstChildElement for it.

				if(s_testMethod == TestMethod.FindAll || s_testMethod == TestMethod.FindAllCache) {
					var a = cache != null ? e.FindAllBuildCache(UIA.TreeScope.Descendants, c1, cache) : e.FindAll(UIA.TreeScope.Descendants, c1);
					//Print(a.Length);
					for(int i = 0, n = a.Length; i < n; i++) {
						e = a.GetElement(i);
						//Print(e.Name);
						if(wName != null) {
							if(!wName.Match(cache != null ? e.CachedName : e.Name)) continue;
						}
						return e;
					}
					return null;
				} else {
					c1 = cond.Condition;
					var x = c1 != null ? f.CreateTreeWalker(c1) : f.RawViewWalker;
					e = _Walk(x, e, wName, cache);
				}
			} else {
				e = e.FindFirst(UIA.TreeScope.Descendants, c1);
			}

			return e;
		}

		static UIA.IElement _Walk(UIA.ITreeWalker x, UIA.IElement e, Wildex wName, UIA.ICacheRequest cache)
		{
			//why with cache slower?

			for(e = cache != null ? x.GetFirstChildElementBuildCache(e, cache) : x.GetFirstChildElement(e);
				e != null;
				e = cache != null ? x.GetNextSiblingElementBuildCache(e, cache) : x.GetNextSiblingElement(e)) {
				//var s = e.Name;
				//PrintList(e.ControlType, s, e.BoundingRectangle);

				if(wName != null) {
					if(!wName.Match(cache != null ? e.CachedName : e.Name)) goto gc;
				}
				break;
				gc:
				//continue;
				var t = _Walk(x, e, wName, cache);
				if(t != null) return t;
			}
			return e;
		}


	}
}
