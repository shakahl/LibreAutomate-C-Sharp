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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	public partial struct Wnd
	{
		/// <summary>
		/// For 'prop' of <see cref="Wnd.Child"/>.
		/// </summary>
		public class ChildProp :WinProp_ChildProp_Base
		{
			/// <summary>
			/// The programming name of the Windows Forms control.
			/// Not used for other types of controls.
			/// </summary>
			public string wfName;
			/// <summary>
			/// TODO
			/// Wildcard, case-insensitive.
			/// </summary>
			public string uiName;

			Misc.WindowsFormsControlNames _wfControls;

			//public static implicit operator ChildProp(string s)
			//{
			//	return null;
			//}

			/// <summary>
			/// Call this before starting to search for the window.
			/// Returns false if the window cannot be found, eg prop atom not found.
			/// Can throw; should not handle.
			/// </summary>
			/// <exception cref="CatException">
			/// When wfName used and cannot get form control names from this window, usually because of UAC.
			/// </exception>
			public bool Init(Wnd wParent)
			{
				if(!_Init()) return false;

				_xy = Coord.GetNormalizedInWindowClientArea(x, y, wParent);

				if(Empty(wfName)) _wfControls = null;
				else
					try {
						_wfControls = new Misc.WindowsFormsControlNames(wParent);
					}
					catch(CatException e) { throw new CatException("Cannot get wfName of controls. Try to run as admin.", e); }

				return true;
			}

			internal bool MatchControlProp(Wnd c)
			{
				if(_wfControls != null) {
					if(!wfName.Equals_(_wfControls.GetControlName(c))) return false;
				}

				if(uiName != null) {
					//TODO
				}

				return true;
			}

			internal void Clear()
			{
				if(_wfControls != null) { _wfControls.Dispose(); _wfControls = null; }
			}
		}

		/// <summary>
		/// 'flags' parameter of <see cref="Child">Child</see>.
		/// </summary>
		[Flags]
		public enum ChildFlag
		{
			/// <summary>Can find hidden controls.</summary>
			HiddenToo = 1,
			/// <summary>Skip indirect descendant controls (children of children and so on).</summary>
			DirectChild = 2,
			/// <summary>To get name always use only <see cref="GetControlText"/>, which is much slower.</summary>
			ControlText = 4,
		}

		/// <summary>
		/// Arguments of <see cref="Child">Child</see>.
		/// </summary>
		public class ChildDefinition
		{
			readonly Wildex _name;
			readonly Wildex _className;
			readonly ChildProp _prop;
			readonly Func<Wnd, bool> _f;
			readonly int _matchIndex;
			readonly int _id;
			readonly ChildFlag _flags;

			///
			public ChildDefinition(
				string name = null, string className = null, int id = 0,
				ChildFlag flags = 0, ChildProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
				)
			{
				_ThrowIfStringEmptyNotNull(className, nameof(className));

				_name = name;
				_className = className;
				_id = id;
				_flags = flags;
				_prop = prop;
				_f = f;
				_matchIndex = matchIndex;
			}

			/// <summary>
			/// The found control.
			/// </summary>
			public Wnd Result { get; private set; }

			/// <summary>
			/// Finds control, like Wnd.Child().
			/// Returns true if found.
			/// The Result property will be the control.
			/// </summary>
			public bool Find(Wnd w)
			{
				var a = w.ChildAllRaw(_flags.HasFlag(ChildFlag.DirectChild), !_flags.HasFlag(ChildFlag.HiddenToo), true);
				return _FindInList(w, a, false) >= 0;
			}

			/// <summary>
			/// Finds matching control in list of controls (handles).
			/// Returns 0-based index, or -1 if not found.
			/// The Result property will be the control.
			/// Does not skip hidden controls, even if flag HiddenToo is not set.
			/// </summary>
			/// <param name="w">Parent window.</param>
			/// <param name="a">List of controls (handles), for example returned by Wnd.ChildAllRaw.</param>
			public int FindInList(Wnd w, List<Wnd> a)
			{
				return _FindInList(w, a, true);
			}

			int _FindInList(Wnd wParent, List<Wnd> a, bool publicCall)
			{
				Result = Wnd0;
				if(a == null || a.Count == 0) return -1;

				if(_prop != null && !_prop.Init(wParent)) return -1;
				try { //will need to call _prop.Clear

					int matchInd = _matchIndex;

					bool mustBeDirectChild = publicCall && _flags.HasFlag(ChildFlag.DirectChild);

					//List<ushort> badAtoms = null;

					//bool useName = false, useText = false;
					//if(_name != null) {
					//	if(_flags.HasFlag(ChildFlag.ControlText)) useText = true; else useName = true;
					//}

					int index = -1;
					foreach(Wnd c in a) {
						index++;

						if(mustBeDirectChild) {
							if(c.DirectParentOrOwner != wParent) continue;
						}

						if(_id != 0) {
							if(c.ControlId != _id) continue;
						}

						//if(useName) {
						//	//if(!_name.Match(c.Name)) continue; //note: if using only Name, call it after ClassName
						//	if(!_name.Match(c.GetControlName())) continue;
						//	//speed:
						//	//	Tested with QM2 window, finding control Find -> Folder (class "*Edit"), using only name (no class etc). Maybe 10-15-th control.
						//	//	With GetControlName 4-5 times faster than with Name.
						//	//	With GetControlText 12 times slower than with GetControlName.
						//	//	When also using class, with GetControlText 2 times slower than with GetControlName.
						//}

						if(_className != null) {
							if(!_className.Match(c.ClassName)) continue;
						}

						//This does not make faster. Even if only class used (no name). Why? Getting atom does not slow down much. If using id, 2 times faster.
						//if(_className != null) {
						//	ushort atom = c.GetClassLong(Api.GCW_ATOM); if(atom == 0) continue;
						//	if(badAtoms != null && badAtoms.Contains(atom)) continue;
						//	string s = c.ClassName; if(s == null) continue;
						//	if(!_className.Match(s)) {
						//		if(badAtoms == null) badAtoms = new List<ushort>();
						//		badAtoms.Add(atom);
						//		continue;
						//	}
						//}

						if(_name != null) {
							string s;
							if(_flags.HasFlag(ChildFlag.ControlText)) s = c.GetControlText(); //only text
							else if(_className == null && _id == 0) s = c.GetControlName(); //only internal name; getting editable text would be slow and unreliable
							else s = c.Name; //internal name, then text if name empty; it is quite fast and reliable when class or id specified

							if(!_name.Match(s)) {
								if(!Util.Misc.StringRemoveMnemonicUnderlineAmpersand(ref s)) continue;
								if(!_name.Match(s)) continue;
							}
						}

						//if(useText) {
						//	if(!_name.Match(c.GetControlText())) continue;
						//}

						if(_prop != null) {
							if(!_prop.MatchPropStylesXY(c, wParent)) continue;

							if(!_prop.MatchControlProp(c)) continue;

							if(_prop._child != null) {
								if(!_prop._child.Find(c)) continue;
							}
						}

						if(_f != null && !_f(c)) continue;

						if(--matchInd > 0) continue;

						Result = c;
						return index;
					}

				}
				finally {
					if(_prop != null) { _prop.Clear(); }
				}

				return -1;
			}
		}

		/// <summary>
		/// Finds child control.
		/// Returns its handle as Wnd. Returns Wnd0 if not found.
		/// </summary>
		/// <param name="name">
		/// Control name or text.
		/// <see cref="Wildex">Wildcard expression</see> or null (null means 'any', "" means 'empty name/text').
		/// This function in most cases ignores editable or slow-to-get text unless one of these is used: className, id, ChildFlag.ControlText.
		/// Control text often contains an invisible '&amp;' character to underline the next character when using the keyboard to select dialog controls. You can use control name with or without '&amp;', this function supports both. 
		/// </param>
		/// <param name="className">Control class name. <see cref="Wildex">Wildcard expression</see> or null (null means 'any').</param>
		/// <param name="id">Control id.</param>
		/// <param name="flags"></param>
		/// <param name="prop">More properties, child control, etc. Example: <c>new Wnd.ChildProp(){ x=344, y=448}</c></param>
		/// <param name="f">
		/// Lambda etc callback function to call for each matching control.
		/// It can evaluate more properties of the control and return true when they match.
		/// Example: <c>e =&gt; { Print(e); return e.Name=="Find"; }</c>
		/// </param>
		/// <param name="matchIndex">1-based index of matching control. For example, if matchIndex is 2, the function skips the first matching control and returns the second.</param>
		/// <exception cref="CatException">
		/// 1. This window (parent window) is invalid (not found, closed, etc).
		/// 2. Used prop.wfName and cannot get form control names from this window, usually because of UAC.
		/// </exception>
		/// <exception cref="ArgumentException">className or prop.childClass is "". To match any, use null.</exception>
		public Wnd Child(
			string name = null, string className = null, int id = 0,
			ChildFlag flags = 0, ChildProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
			)
		{
			ValidateThrow();
			var d = new ChildDefinition(name, className, id, flags, prop, f, matchIndex);
			d.Find(this);
			return d.Result;
		}

		/// <summary>
		/// Calls <see cref="Child"/>.
		/// Returns true if it finds the child control (if returns not Wnd0).
		/// </summary>
		public bool HasChild(
			string name = null, string className = null, int id = 0,
			ChildFlag flags = 0, ChildProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
			)
		{
			return Wnd0 != Child(name, className, id, flags, prop, f, matchIndex);
		}

		//These could be used to find control including hidden controls and avoid using Wnd.ChildFlag.HiddenToo. But probably rarely used.
		///// <summary>
		///// Calls <see cref="Child"/> with flag ChildFlag.HiddenToo.
		///// </summary>
		//public Wnd ChildH(
		//	string name = null, string className = null, int id = 0,
		//	ChildProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
		//	)
		//{
		//	return Child(name, className, id, ChildFlag.HiddenToo, prop, f, matchIndex);
		//}

		///// <summary>
		///// Calls <see cref="Child"/> with flag ChildFlag.HiddenToo.
		///// Returns true if it finds the child control (if returns not Wnd0).
		///// </summary>
		//public bool HasChildH(
		//	string name = null, string className = null, int id = 0,
		//	ChildProp prop = null, Func<Wnd, bool> f = null, int matchIndex = 1
		//	)
		//{
		//	return Wnd0!=Child(name, className, id, ChildFlag.HiddenToo, prop, f, matchIndex);
		//}

		/// <summary>
		/// Finds child control by its id.<br/>
		/// Finds hidden controls too. Does not prefer visible controls.
		/// </summary>
		/// <param name="id">Control id.</param>
		/// <param name="directChild">Must be direct child, not a child of a child and so on.</param>
		/// <exception cref="CatException">When this window is invalid (not found, closed, etc).</exception>
		/// <remarks>
		/// Faster and more lightweight than Child().
		/// At first calls Api.GetDlgItem. It is fast and searches only direct children. If it does not find, and directChild is false, calls Api.EnumChildWindows.
		/// </remarks>
		public Wnd ChildById(int id, bool directChild = false)
		{
			Wnd R = Api.GetDlgItem(this, id);
			if(R.Is0) {
				ValidateThrow();
				if(directChild == false) {
					ChildAllRaw(e =>
					{
						if(e.ControlId != id) return false;
						R = e; return true;
					});
				}
			}
			return R;
		}

		/// <summary>
		/// Finds child control by its class name.
		/// By default finds hidden controls too. Does not prefer visible controls.
		/// More lightweight than Child().
		/// </summary>
		/// <param name="className">Class name. Case-insensitive <see cref="String_.Like_(string, string, bool)">wildcard</see>.</param>
		/// <param name="directChild">Must be direct child, not grandchild.</param>
		/// <param name="onlyVisible">Must be visible.</param>
		/// <param name="matchIndex">1-based match index. For example, if 2, will get the second matching control.</param>
		/// <exception cref="CatException">When this window is invalid (not found, closed, etc).</exception>
		public Wnd ChildByClass(string className, bool directChild = false, bool onlyVisible = false, int matchIndex = 1)
		{
			ValidateThrow();
			Wnd R = Wnd0;
			ChildAllRaw(e =>
			{
				if(!e.ClassNameIs(className)) return false;
				if(--matchIndex > 0) return false;
				R = e; return true;
			}, directChild, onlyVisible);
			return R;
		}

		/// <summary>
		/// Finds direct child control by its class name and optionally full name.
		/// Finds hidden controls too. Does not prefer visible controls.
		/// More lightweight than Child().
		/// Calls Api.FindWindowEx().
		/// </summary>
		/// <param name="className">Class name. Full, case-insensitive. Wildcard etc not supported. Can be null to match any.</param>
		/// <param name="fullName">Name. Full, case-insensitive. Wildcard etc not supported. Can be omitted or null to match any.</param>
		/// <param name="wAfter">If not Wnd0, starts searching from the next control in the Z order.</param>
		/// <remarks>This window also can be Wnd.Misc.SpecHwnd.Message to find a message-only window.</remarks>
		public Wnd ChildRaw(string className, string fullName = null, Wnd wAfter = default(Wnd))
		{
			//ValidateThrow(); //no, it can be Message
			return Api.FindWindowEx(this, wAfter, className, fullName);
		}

		/// <summary>
		/// Finds all matching child controls.
		/// Returns List containing 0 or more control handles as Wnd.
		/// Everything except the return type is the same as with Child().
		/// </summary>
		public List<Wnd> ChildAll(
			string name = null, string className = null, int id = 0,
			ChildFlag flags = 0, ChildProp prop = null, Func<Wnd, bool> f = null
			)
		{
			ValidateThrow();
			var a = new List<Wnd>();
			Child(name, className, id, flags, prop, e =>
			{
				if(f == null || f(e)) a.Add(e);
				return false;
			});
			return a;
		}

		/// <summary>
		/// Gets list of child controls.
		/// Returns List containing 0 or more control handles as Wnd.
		/// Uses Api.EnumChildWindows().
		/// </summary>
		/// <param name="directChild">Need only direct children, not grandchildren.</param>
		/// <param name="onlyVisible">Need only visible controls.</param>
		/// <param name="sortFirstVisible">Place all list elements of hidden controls at the end of the returned list.</param>
		public List<Wnd> ChildAllRaw(bool directChild = false, bool onlyVisible = false, bool sortFirstVisible = false)
		{
			List<Wnd> a = new List<Wnd>(), aHidden = null;
			if(onlyVisible) sortFirstVisible = false;

			ChildAllRaw(e =>
			{
				if(sortFirstVisible && !e.IsVisible) {
					if(aHidden == null) aHidden = new List<Wnd>();
					aHidden.Add(e);
				} else {
					a.Add(e);
				}
				return false;
			}, directChild, onlyVisible);

			if(aHidden != null) a.AddRange(aHidden);
			return a;

			//tested: using a non-anonymous callback function does not make faster.
		}

		/// <summary>
		/// Calls callback function for each child control.
		/// Uses Api.EnumChildWindows().
		/// </summary>
		/// <param name="f">Lambda etc callback function to call for each matching control. Can return true to stop.</param>
		/// <param name="directChild">Need only direct children, not grandchildren.</param>
		/// <param name="onlyVisible">Need only visible controls.</param>
		public void ChildAllRaw(Func<Wnd, bool> f, bool directChild = false, bool onlyVisible = false)
		{
			Wnd w = this;

			Api.EnumChildWindows(this, (c, param) =>
			{
				if(onlyVisible && !c.IsVisible) return 1;
				if(directChild && c.DirectParentOrOwner != w) return 1;
				return f(c) ? 0 : 1;
			}, Zero);
		}

		//Better don't use this.
		///// <summary>
		///// Gets list of direct child controls of a window.
		///// Uses Get.FirstChild and Get.NextSibling. It is faster than Api.EnumChildWindows.
		///// Should be used only with windows of current thread. Else it is unreliable because, if some controls are zordered or destroyed while enumerating, some controls can be skipped or retrieved more than once.
		///// </summary>
		//public static List<Wnd> DirectChildControlsFastUnsafe(string className = null)
		//{
		//	var wild = _GetWildcard(className);
		//	var a = new List<Wnd>();
		//	for(Wnd c = Get.FirstChild(this); !c.Is0; c = Get.NextSibling(c)) {
		//		if(wild != null && !c._ClassNameIs(wild)) continue;
		//		a.Add(c);
		//	}
		//	return a;
		//}

		//Cannot use this because need a callback function. Unless we at first get all and store in an array (similar speed).
		//public static IEnumerable<Wnd> ChildAllRaw(Wnd hwnd)
		//{
		//	Api.EnumChildWindows(hwnd, (t, param)=>
		//	{
		//		yield return t; //error, yield cannot be in an anonymous method etc
		//		return 1;
		//	}, Zero);
		//}
	}
}
