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

using static Catkeys.NoClass;

#pragma warning disable 282 //C#7 intellisense bug: it thinks that Wnd has multiple fields.

namespace Catkeys
{
	/// <summary>
	/// 'flags' parameter of <see cref="Wnd.Child"/>.
	/// <note>
	/// </note>
	/// </summary>
	/// <tocexclude />
	[Flags]
	public enum WCFlags
	{
		/// <summary>Can find hidden controls.</summary>
		HiddenToo = 1,
		/// <summary>Skip indirect descendant controls (children of children and so on).</summary>
		DirectChild = 2,
	}

	public partial struct Wnd
	{
		/// <summary>
		/// Contains the same fields as <see cref="Wnd.Child"/> parameters, and can be used instead of it.
		/// See also: <see cref="Wnd.Find"/>.
		/// </summary>
		public class ChildParams
		{
			enum _NameIs { name, text, id, accName, wfName }

			readonly Wildex _name;
			readonly Wildex _className;
			readonly Func<Wnd, bool> _also;
			Misc.WinFormsControlNames _wfControls;
			readonly int _skipCount;
			readonly WCFlags _flags;
			readonly _NameIs _nameIs;
			readonly int _id;

			/// <summary>
			/// See <see cref="Wnd.Child">Wnd.Child</see>.
			/// </summary>
			/// <exception cref="ArgumentException">
			/// className is "". To match any, use null.
			/// Invalid wildcard expression ("**options|" or regular expression).
			/// </exception>
			public ChildParams(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null, int skip = 0)
			{
				_ThrowIfStringEmptyNotNull(className, nameof(className));

				if(name != null) {
					int i = 0;
					if(name.Length >= 5 && name[0] == '*' && name[1] == '*') {
						i = name.EqualsPart_(2, false, "id:", "text:", "accName:", "wfName:");
						switch(i) {
						case 1: _nameIs = _NameIs.id; _id = name.ToInt32_(5); break;
						case 2: _nameIs = _NameIs.text; _name = name.Substring(7); break;
						case 3: _nameIs = _NameIs.accName; _name = name.Substring(10); break;
						case 4: _nameIs = _NameIs.wfName; _name = name.Substring(9); break;
						}
					}
					if(i == 0) _name = name;
				}

				_className = className;
				_flags = flags;
				_also = also;
				_skipCount = skip;
			}

			/// <summary>
			/// The found control.
			/// </summary>
			public Wnd Result { get; private set; }

			/// <summary>
			/// Finds the specified control, like <see cref="Wnd.Child">Wnd.Child</see>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the control.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window.</param>
			public bool Find(Wnd wParent)
			{
				var a = wParent.AllChildren(0 != (_flags & WCFlags.DirectChild), 0 == (_flags & WCFlags.HiddenToo), true);
				return _FindInList(wParent, a, false) >= 0;
			}

			/// <summary>
			/// Finds the specified control in a list of controls.
			/// Returns 0-based index, or -1 if not found.
			/// The <see cref="Result"/> property will be the control.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window.</param>
			/// <param name="a">List of controls, for example returned by <see cref="AllChildren"/>.</param>
			public int FindInList(Wnd wParent, List<Wnd> a)
			{
				return _FindInList(wParent, a, true);
			}

			int _FindInList(Wnd wParent, List<Wnd> a, bool inList)
			{
				Result = Wnd0;
				if(a == null || a.Count == 0) return -1;

				try { //will need to dispose something

					int skipCount = _skipCount;
					bool mustBeVisible = inList && (_flags & WCFlags.HiddenToo) == 0;
					bool mustBeDirectChild = inList && (_flags & WCFlags.DirectChild) != 0;

					for(int index = 0; index < a.Count; index++) {
						Wnd c = a[index];

						if(mustBeVisible) {
							if(!c.IsVisible) continue;
						}

						if(mustBeDirectChild) {
							if(c.WndDirectParentOrOwner != wParent) continue;
						}

						if(_nameIs == _NameIs.id) {
							if(c.ControlId != _id) continue;
						}

						if(_className != null) {
							if(!_className.Match(c.ClassName)) continue;
						}

						if(_name != null) {
							string s;
							switch(_nameIs) {
							case _NameIs.text:
								s = c.ControlText;
								break;
							case _NameIs.accName:
								s = c.NameAcc;
								break;
							case _NameIs.wfName:
								if(_wfControls == null) {
									try {
										_wfControls = new Misc.WinFormsControlNames(wParent);
									}
									catch(WndException) { //invalid parent window
										return -1;
									}
									catch(CatException e) { //probably process of higher UAC integrity level
										Output.Warning($"Failed to get .NET control names. {e.Message}");
										return -1;
									}
								}
								s = _wfControls.GetControlName(c);
								break;
							default:
								Debug.Assert(_nameIs == _NameIs.name);
								s = c.Name;
								break;
							}

							if(!_name.Match(s)) continue;
						}

						if(_also != null && !_also(c)) continue;

						if(skipCount-- > 0) continue;

						Result = c;
						return index;
					}
				}
				finally {
					if(_wfControls != null) { _wfControls.Dispose(); _wfControls = null; }
				}

				return -1;
			}
		}

		/// <summary>
		/// Finds child control.
		/// Returns its handle as Wnd. Returns Wnd0 if not found.
		/// </summary>
		/// <param name="name">
		/// Control name.
		/// By default the function gets control names with <see cref="Name"/>.
		/// Can start with these prefix strings:
		/// <list type="bullet">
		/// <item>
		/// "**text:" - use control text, which can be editable.
		/// To get it this function uses <see cref="ControlText"/>. It is slower and can be less reliable (because can get editable text), especially if className not used. It does not remove the invisible '&amp;' characters that are used to underline keyboard shortcuts with the Alt key.
		/// </item>
		/// <item>
		/// "**accName:" - use MSAA IAccessible.Name property.
		/// To get it this function uses <see cref="NameAcc"/>. It is slower.
		/// </item>
		/// <item>
		/// "**wfName:" - use .NET Windows Forms Control Name property.
		/// To get it this function uses <see cref="Misc.WinFormsControlNames"/>. It is slower and can fail because of UAC.
		/// </item>
		/// <item>
		/// "**id:" (like "**id:15") - use control id.
		/// To get it this function uses <see cref="ControlId"/>.
		/// Cannot be wildcard expression.
		/// You can instead use <see cref="ChildById"/>, it is faster than <b>Child</b>.</item>
		/// </list>
		/// String format (not including the prefix): <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'any'.
		/// "" means 'no name'.
		/// </param>
		/// <param name="className">
		/// Control class name.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'any'.
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Lambda etc callback function to call for each matching control.
		/// It can evaluate more properties of the control and return true when they match.
		/// Example: <c>also: t =&gt; t.IsEnabled</c>
		/// </param>
		/// <param name="skip">0-based index of matching control. For example, if it is 1, the function skips the first matching control and returns the second.</param>
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		/// <exception cref="ArgumentException">
		/// className is "". To match any, use null.
		/// Invalid wildcard expression ("**options|" or regular expression).
		/// </exception>
		public Wnd Child(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null, int skip = 0)
		{
			ThrowIfInvalid();
			var d = new ChildParams(name, className, flags, also, skip);
			d.Find(this);
			return d.Result;
		}

		/// <summary>
		/// Calls <see cref="Child"/>.
		/// Returns true if it finds the child control.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <exception cref="ArgumentException"/>
		public bool HasChild(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null, int skip = 0)
		{
			return Wnd0 != Child(name, className, flags, also, skip);
		}

		/// <summary>
		/// Finds child control by its id.
		/// Finds hidden controls too. Does not prefer visible controls.
		/// </summary>
		/// <param name="id">Control id.</param>
		/// <param name="directChild">Must be direct child, not a child of a child and so on.</param>
		/// <remarks>
		/// Faster and more lightweight than <see cref="Child">Child</see>.
		/// At first calls API <msdn>GetDlgItem</msdn>. It is fast and searches only direct children. If it does not find, and directChild is false, calls API <msdn>EnumChildWindows</msdn>.
		/// <note>Not all controls have a useful id. If control id is 0 or different in each window instance, this function cannot be used.</note>
		/// </remarks>
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		public Wnd ChildById(int id, bool directChild = false)
		{
			Wnd R = Api.GetDlgItem(this, id);
			if(R.Is0) {
				ThrowIfInvalid();
				if(directChild == false) {
					LibAllChildren(e =>
					{
						if(e.ControlId != id) return false;
						R = e; return true;
					});
				}
			}
			return R;
		}

		//Not very useful when we have Child, although in some cases faster.
		///// <summary>
		///// Finds child control by its class name.
		///// By default finds hidden controls too. Does not prefer visible controls.
		///// More lightweight than <see cref="Child">Child</see>.
		///// </summary>
		///// <param name="className">Class name. Case-insensitive <see cref="String_.Like_(string, string, bool)">wildcard</see>.</param>
		///// <param name="directChild">Must be direct child, not grandchild.</param>
		///// <param name="onlyVisible">Must be visible.</param>
		///// <param name="skip">0-based match index. For example, if 1, will get the second matching control.</param>
		///// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		//public Wnd ChildByClass(string className, bool directChild = false, bool onlyVisible = false, int skip = 0)
		//{
		//	ThrowIfInvalid();
		//	Wnd R = Wnd0;
		//	LibAllChildren(e =>
		//	{
		//		if(!e.ClassNameIs(className)) return false;
		//		if(skip-- > 0) return false;
		//		R = e; return true;
		//	}, directChild, onlyVisible);
		//	return R;
		//}

		/// <summary>
		/// Finds all matching child controls.
		/// Returns List containing 0 or more control handles as Wnd.
		/// Everything except the return type is the same as with <see cref="Child">Child</see>.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <exception cref="ArgumentException"/>
		public List<Wnd> ChildAll(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null)
		{
			ThrowIfInvalid();
			var a = new List<Wnd>();
			Child(name, className, flags, e =>
			{
				if(also == null || also(e)) a.Add(e);
				return false;
			});
			return a;
		}

		/// <summary>
		/// Finds direct child control.
		/// Returns its handle as Wnd. Returns Wnd0 if not found.
		/// Calls API <msdn>FindWindowEx</msdn>.
		/// Faster than <see cref="Child">Child</see>, which uses API <msdn>EnumChildWindows</msdn>.
		/// Can be used only when you know full name and/or class name.
		/// Finds hidden controls too.
		/// </summary>
		/// <param name="name">
		/// Name. Can be null to match any.
		/// Full, case-insensitive. Wildcard etc not supported.
		/// Must include the invisible '&amp;' characters that are used to underline keyboard shortcuts with the Alt key.
		/// </param>
		/// <param name="className">
		/// Class name. Can be null to match any. Cannot be "".
		/// Full, case-insensitive. Wildcard etc not supported.
		/// </param>
		/// <param name="wAfter">If used, starts searching from the next control in the Z order.</param>
		/// <remarks>
		/// If this window is Wnd.Misc.SpecHwnd.Message, finds a message-only window.
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public Wnd ChildFast(string name, string className, Wnd wAfter = default(Wnd))
		{
			//ThrowIfInvalid(); //no, it can be Message
			if(Is0) {
				Api.SetLastError(Api.ERROR_INVALID_WINDOW_HANDLE);
				return Wnd0;
			}
			return Api.FindWindowEx(this, wAfter, className, name);
		}

		/// <summary>
		/// Gets list of child controls.
		/// Returns List containing 0 or more control handles as Wnd.
		/// Calls API <msdn>EnumChildWindows</msdn>.
		/// </summary>
		/// <param name="directChild">Need only direct children, not grandchildren.</param>
		/// <param name="onlyVisible">Need only visible controls.</param>
		/// <param name="sortFirstVisible">Place all list elements of hidden controls at the end of the returned list.</param>
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		public List<Wnd> AllChildren(bool directChild = false, bool onlyVisible = false, bool sortFirstVisible = false)
		{
			List<Wnd> a = new List<Wnd>(), aHidden = null;
			if(onlyVisible) sortFirstVisible = false;

			LibAllChildren(e =>
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
		/// Calls API <msdn>EnumChildWindows</msdn>.
		/// </summary>
		/// <param name="f">Lambda etc callback function to call for each matching control. Can return true to stop.</param>
		/// <param name="directChild">Need only direct children, not grandchildren.</param>
		/// <param name="onlyVisible">Need only visible controls.</param>
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		internal void LibAllChildren(Func<Wnd, bool> f, bool directChild = false, bool onlyVisible = false)
		{
			ThrowIfInvalid();
			Wnd w = this;

			Api.EnumChildWindows(this, (c, param) =>
			{
				if(onlyVisible && !c.IsVisible) return 1;
				if(directChild && c.WndDirectParentOrOwner != w) return 1;
				return f(c) ? 0 : 1;
			}, Zero);
		}

		//Better don't use this.
		///// <summary>
		///// Gets list of direct child controls of a window.
		///// Uses WndChild and WndNext. It is faster than API EnumChildWindows.
		///// Should be used only with windows of current thread. Else it is unreliable because, if some controls are zordered or destroyed while enumerating, some controls can be skipped or retrieved more than once.
		///// </summary>
		//public static List<Wnd> DirectChildControlsFastUnsafe(string className = null)
		//{
		//	var wild = _GetWildcard(className);
		//	var a = new List<Wnd>();
		//	for(Wnd c = WndChild; !c.Is0; c = c.WndNext) {
		//		if(wild != null && !c._ClassNameIs(wild)) continue;
		//		a.Add(c);
		//	}
		//	return a;
		//}

		//Cannot use this because need a callback function. Unless we at first get all and store in an array (similar speed).
		//public static IEnumerable<Wnd> AllChildren(Wnd hwnd)
		//{
		//	Api.EnumChildWindows(hwnd, (t, param)=>
		//	{
		//		yield return t; //error, yield cannot be in an anonymous method etc
		//		return 1;
		//	}, Zero);
		//}
	}
}
