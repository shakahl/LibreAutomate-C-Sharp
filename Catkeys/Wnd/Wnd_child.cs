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
		/// Contains control (child window) properties and can be used to find the control.
		/// Can be used instead of <see cref="Wnd.Child"/> or <see cref="Wnd.ChildAll"/>.
		/// Also can be used to find window that contains certain control, like in the example.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// //find window that contains certain control, and get the control too
		/// var f = new Wnd.ChildFinder("Password*", "Static"); //control properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => f.Find(t));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public class ChildFinder
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
			public ChildFinder(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null, int skip = 0)
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
			/// Finds the specified child control, like <see cref="Wnd.Child"/>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the control.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window.</param>
			/// <exception cref="WndException">Invalid wParent.</exception>
			public bool FindIn(Wnd wParent)
			{
				var a = wParent.AllChildren(0 != (_flags & WCFlags.DirectChild), 0 == (_flags & WCFlags.HiddenToo), true);
				return _FindInList(wParent, a, false) >= 0;
			}

			/// <summary>
			/// Finds the specified control in a list of controls.
			/// Returns 0-based index, or -1 if not found.
			/// The <see cref="Result"/> property will be the control.
			/// </summary>
			/// <param name="a">List of controls, for example returned by <see cref="AllChildren"/>.</param>
			/// <param name="wParent">Direct or indirect parent window. Used only for flag DirectChild.</param>
			public int FindInList(IEnumerable<Wnd> a, Wnd wParent = default(Wnd))
			{
				return _FindInList(wParent, a, true);
			}

			/// <summary>
			/// Finds all matching child controls, like <see cref="ChildAll"/>.
			/// Returns List containing 0 or more control handles as Wnd.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window.</param>
			/// <exception cref="WndException">Invalid wParent.</exception>
			public List<Wnd> FindAllIn(Wnd wParent)
			{
				var a = wParent.AllChildren(0 != (_flags & WCFlags.DirectChild), 0 == (_flags & WCFlags.HiddenToo), true);
				var R = new List<Wnd>();
				_FindInList(wParent, a, false, R);
				return R;
			}

			/// <summary>
			/// Finds all matching controls in a list of controls.
			/// Returns List containing 0 or more control handles as Wnd.
			/// </summary>
			/// <param name="a">List of controls, for example returned by <see cref="AllChildren"/>.</param>
			/// <param name="wParent">Direct or indirect parent window. Used only for flag DirectChild.</param>
			public List<Wnd> FindAllInList(IEnumerable<Wnd> a, Wnd wParent = default(Wnd))
			{
				var R = new List<Wnd>();
				_FindInList(wParent, a, true, R);
				return R;
			}

			/// <summary>
			/// If a is not null, returns index of matching element or -1.
			/// Else returns -2 if wSingle matches, else -1.
			/// Returns -1 if using aFindAll.
			/// </summary>
			/// <param name="wParent">Parent window. Can be Wnd0 if inList is true and no DirectChild flag and not using winforms name.</param>
			/// <param name="a">Array etc of Wnd.</param>
			/// <param name="inList">Called by FindInList or FindAllInList.</param>
			/// <param name="aFindAll">If not null, adds all matching to it and returns -1.</param>
			/// <param name="wSingle">Can be used instead of a. Then a must be null.</param>
			int _FindInList(Wnd wParent, IEnumerable<Wnd> a, bool inList, [Out] List<Wnd> aFindAll = null, Wnd wSingle = default(Wnd))
			{
				Result = Wnd0;
				if(a == null) return -1;

				int skipCount = _skipCount;
				bool mustBeVisible = inList && (_flags & WCFlags.HiddenToo) == 0;
				bool mustBeDirectChild = inList && (_flags & WCFlags.DirectChild) != 0 && !wParent.Is0;

				try { //will need to dispose something

					//this thing is a foreach that supports either IEnumerable (a) or single Wnd (wSingle)
					using(var en = a?.GetEnumerator()) {
						for(int index = 0; index >= 0; index++) {
							Wnd w;
							if(en != null) {
								if(!en.MoveNext()) break;
								w = en.Current;
							} else {
								w = wSingle;
								index = -2; //if matches, returns -2, else breaks at -1
							}
							if(w.Is0) continue;

							if(mustBeVisible) {
								if(!w.IsVisible) continue;
							}

							if(mustBeDirectChild) {
								if(w.WndDirectParentOrOwner != wParent) continue;
							}

							if(_nameIs == _NameIs.id) {
								if(w.ControlId != _id) continue;
							}

							if(_className != null) {
								if(!_className.Match(w.ClassName)) continue;
							}

							if(_name != null) {
								string s;
								switch(_nameIs) {
								case _NameIs.text:
									s = w.ControlText;
									break;
								case _NameIs.accName:
									s = w.NameAcc;
									break;
								case _NameIs.wfName:
									if(_wfControls == null) {
										try {
											_wfControls = new Misc.WinFormsControlNames(wParent.Is0 ? w : wParent);
										}
										catch(WndException) { //invalid parent window
											return -1;
										}
										catch(CatException e) { //probably process of higher UAC integrity level
											Output.Warning($"Failed to get .NET control names. {e.Message}");
											return -1;
										}
									}
									s = _wfControls.GetControlName(w);
									break;
								default:
									Debug.Assert(_nameIs == _NameIs.name);
									s = w.Name;
									break;
								}

								if(!_name.Match(s)) continue;
							}

							if(_also != null && !_also(w)) continue;

							if(aFindAll != null) {
								aFindAll.Add(w);
								continue;
							}

							if(skipCount-- > 0) continue;

							Result = w;
							return index;
						}
					}
				}
				finally {
					if(_wfControls != null) { _wfControls.Dispose(); _wfControls = null; }
				}

				return -1;
			}

			/// <summary>
			/// Returns true if control c properties match the specified properties.
			/// </summary>
			/// <param name="c">A control. Can be 0/invalid, then returns false.</param>
			/// <param name="wParent">Direct or indirect parent window. If used, returns false if it isn't parent (also depends on flag DirectChild).</param>
			public bool IsMatch(Wnd c, Wnd wParent = default(Wnd))
			{
				if(!wParent.Is0 && !c.IsChildOf(wParent)) {
					Result = default(Wnd);
					return false;
				}
				return -2 == _FindInList(wParent, null, true, null, c);
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
			//ThrowIfInvalid(); //will be called later
			var f = new ChildFinder(name, className, flags, also, skip);
			f.FindIn(this);
			return f.Result;
		}

		/// <summary>
		/// Returns true if this window contains the specified control.
		/// Calls <see cref="Child"/>.
		/// <note type="note">
		/// Using this function many times with same parameters is inefficient. Instead create new <see cref="ChildFinder"/> and call <see cref="ChildFinder.FindIn"/> or <see cref="HasChild(ChildFinder)"/>. See example.
		/// </note>
		/// </summary>
		/// <exception cref="WndException"/>
		/// <exception cref="ArgumentException"/>
		/// <example>
		/// <code><![CDATA[
		/// //find window that contains certain control, and get the control too
		/// var f = new Wnd.ChildFinder("Password*", "Static"); //control properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => t.HasChild(f));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public bool HasChild(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null, int skip = 0)
		{
			return Wnd0 != Child(name, className, flags, also, skip);
		}

		/// <summary>
		/// Returns true if this window contains the specified control.
		/// Calls <see cref="ChildFinder.FindIn"/>.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <example>
		/// <code><![CDATA[
		/// //find window that contains certain control, and get the control too
		/// var f = new Wnd.ChildFinder("Password*", "Static"); //control properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => t.HasChild(f));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public bool HasChild(ChildFinder f)
		{
			return f.FindIn(this);
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
					Api.EnumChildWindows(this, (c, param) =>
					{
						if(c.ControlId != id) return 1;
						R = c; return 0;
					}, Zero);
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
		/// <remarks>
		/// In the returned list, hidden controls (when using WCFlags.HiddenToo) are always after visible controls.
		/// </remarks>
		public List<Wnd> ChildAll(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null)
		{
			//ThrowIfInvalid(); //will be called later
			var f = new ChildFinder(name, className, flags, also);
			var a = f.FindAllIn(this);
			//CONSIDER: add property LastChildParams, like LastFind.
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
		/// Gets child controls.
		/// Returns array containing 0 or more control handles as Wnd.
		/// </summary>
		/// <param name="directChild">Need only direct children, not grandchildren.</param>
		/// <param name="onlyVisible">Need only visible controls.</param>
		/// <param name="sortFirstVisible">Place all array elements of hidden controls at the end of the array.</param>
		/// <param name="also">
		/// Lambda etc callback function to call for each matching control.
		/// It can evaluate more properties of the control and return true when they match.
		/// Example: <c>also: t =&gt; t.ClassNameIs("Edit")</c>.
		/// </param>
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		/// <remarks>
		/// Calls API <msdn>EnumChildWindows</msdn>.
		/// </remarks>
		public Wnd[] AllChildren(bool directChild = false, bool onlyVisible = false, bool sortFirstVisible = false, Func<Wnd, bool> also = null)
		{
			ThrowIfInvalid();
			return Misc.LibEnumWindows(Misc.LibEnumWindowsAPI.EnumChildWindows, onlyVisible, sortFirstVisible, also, this, directChild);
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
