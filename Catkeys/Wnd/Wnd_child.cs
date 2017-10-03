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

using Catkeys.Types;
using static Catkeys.NoClass;

#pragma warning disable 282 //intellisense bug: it thinks that Wnd has multiple fields.

namespace Catkeys
{
	public unsafe partial struct Wnd
	{
		/// <summary>
		/// Contains control (child window) properties and is used to find the control.
		/// Can be used instead of <see cref="Wnd.Child"/> or <see cref="Wnd.ChildAll"/>.
		/// Also can be used to find window that contains certain control, like in the example.
		/// </summary>
		/// <example>
		/// Find window that contains certain control, and get the control too.
		/// <code><![CDATA[
		/// var f = new Wnd.ChildFinder("Password*", "Static"); //control properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => f.FindIn(t));
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
				if(className != null) {
					if(className.Length == 0) throw new ArgumentException("Class name cannot be \"\". Use null to match any.");
					_className = className;
				}
				if(name != null) {
					int i = 0;
					if(name.Length >= 5 && name[0] == '*' && name[1] == '*') {
						i = name.EqualsAt_(2, false, "id:", "text:", "accName:", "wfName:");
						int iFrom = 0;
						switch(i) {
						case 1: _nameIs = _NameIs.id; _id = name.ToInt32_(5); break;
						case 2: _nameIs = _NameIs.text; iFrom = 7; break;
						case 3: _nameIs = _NameIs.accName; iFrom = 10; break;
						case 4: _nameIs = _NameIs.wfName; iFrom = 9; break;
						}
						if(iFrom != 0) name = name.Substring(iFrom);
					}
					if(i != 1) _name = name;
				}
				_flags = flags;
				_also = also;
				_skipCount = skip;
			}

			/// <summary>
			/// The found control.
			/// </summary>
			public Wnd Result { get; internal set; }

			/// <summary>
			/// Finds the specified child control, like <see cref="Wnd.Child"/>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the control.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window. Can be top-level window or control.</param>
			/// <exception cref="WndException">Invalid wParent.</exception>
			public bool FindIn(Wnd wParent)
			{
				using(var k = new _WndList(_AllChildren(wParent)))
					return _FindInList(wParent, k) >= 0;
			}

			Util.LibArrayBuilder<Wnd> _AllChildren(Wnd wParent)
			{
				wParent.ThrowIfInvalid();
				return Lib.EnumWindows2(Lib.EnumWindowsAPI.EnumChildWindows,
						onlyVisible: 0 == (_flags & WCFlags.HiddenToo),
						sortFirstVisible: true,
						wParent: wParent,
						directChild: 0 != (_flags & WCFlags.DirectChild));
			}

			/// <summary>
			/// Finds the specified control in a list of controls.
			/// Returns 0-based index, or -1 if not found.
			/// The <see cref="Result"/> property will be the control.
			/// </summary>
			/// <param name="a">List of controls, for example returned by <see cref="AllChildren"/>.</param>
			/// <param name="wParent">Direct or indirect parent window. Used only for flag DirectChild.</param>
			public int FindInList(IEnumerable<Wnd> a, Wnd wParent = default)
			{
				using(var k = new _WndList(a))
					return _FindInList(wParent, k);
			}

			/// <summary>
			/// Finds all matching child controls, like <see cref="ChildAll"/>.
			/// Returns array containing 0 or more control handles as Wnd.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window. Can be top-level window or control.</param>
			/// <exception cref="WndException">Invalid wParent.</exception>
			public Wnd[] FindAllIn(Wnd wParent)
			{
				return _FindAll(new _WndList(_AllChildren(wParent)), wParent);
			}

			/// <summary>
			/// Finds all matching controls in a list of controls.
			/// Returns array containing 0 or more control handles as Wnd.
			/// </summary>
			/// <param name="a">List of controls, for example returned by <see cref="AllChildren"/>.</param>
			/// <param name="wParent">Direct or indirect parent window. Used only for flag DirectChild.</param>
			public Wnd[] FindAllInList(IEnumerable<Wnd> a, Wnd wParent = default)
			{
				return _FindAll(new _WndList(a), wParent);
			}

			Wnd[] _FindAll(_WndList k, Wnd wParent)
			{
				using(k) {
					using(var ab = new Util.LibArrayBuilder<Wnd>()) {
						_FindInList(wParent, k, w => ab.Add(w)); //CONSIDER: ab could be part of _WndList. Now the delegate creates garbage.
						return ab.ToArray();
					}
				}
			}

			/// <summary>
			/// Returns index of matching element or -1.
			/// Returns -1 if using getAll.
			/// </summary>
			/// <param name="wParent">Parent window. Can be default(Wnd) if inList is true and no DirectChild flag and not using winforms name.</param>
			/// <param name="a">List of Wnd. Does not dispose it.</param>
			/// <param name="getAll">If not null, calls it for all matching and returns -1.</param>
			int _FindInList(Wnd wParent, _WndList a, Action<Wnd> getAll = null)
			{
				Result = default;
				if(a.Type == _WndList.ListType.None) return -1;
				bool inList = a.Type != _WndList.ListType.ArrayBuilder;
				int skipCount = _skipCount;

				try { //will need to dispose something
					for(int index = 0; a.Next(out Wnd w); index++) {
						if(w.Is0) continue;

						if(inList) { //else the enum function did this
							if(!_flags.Has_(WCFlags.HiddenToo)) {
								if(!w.IsVisible) continue;
							}

							if(_flags.Has_(WCFlags.DirectChild) && !wParent.Is0) {
								if(w.WndDirectParentOrOwner != wParent) continue;
							}
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

						if(getAll != null) {
							getAll(w);
							continue;
						}

						if(skipCount-- > 0) continue;

						Result = w;
						return index;
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
			public bool IsMatch(Wnd c, Wnd wParent = default)
			{
				if(!wParent.Is0 && !c.IsChildOf(wParent)) {
					Result = default;
					return false;
				}
				return 0 == _FindInList(wParent, new _WndList(c));
			}
		}

		/// <summary>
		/// Finds a child control and returns its handle as Wnd.
		/// Returns default(Wnd) if not found. To check it you can use <see cref="Is0"/> or <see cref="ExtensionMethods.OrThrow(Wnd)"/>.
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
		/// null means 'can be any'. "" means 'must not have name'.
		/// </param>
		/// <param name="className">
		/// Control class name.
		/// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		/// null means 'can be any'. Cannot be "".
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
			return default != Child(name, className, flags, also, skip);
		}

		/// <summary>
		/// Returns true if this window contains the specified control.
		/// Calls <see cref="ChildFinder.FindIn"/>.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <example>
		/// Find window that contains certain control, and get the control too.
		/// <code><![CDATA[
		/// var cf = new Wnd.ChildFinder("Password*", "Static"); //control properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => t.HasChild(cf));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public bool HasChild(ChildFinder f)
		{
			return f.FindIn(this);
		}

		/// <summary>
		/// Returns true if this window contains the specified accessible object.
		/// Calls <see cref="Acc.Finder.FindIn(Wnd)"/>.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <example>
		/// Find window that contains certain accessible object (AO), and get the AO too.
		/// <code><![CDATA[
		/// var af = new Acc.Finder("PUSHBUTTON", "OK"); //AO properties
		/// Wnd w = Wnd.Find(className: "#32770", also: t => t.HasAcc(af));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public bool HasAcc(Acc.Finder f)
		{
			return f.FindIn(this);
		}

		/// <summary>
		/// Finds a child control by its id and returns its handle as Wnd.
		/// Returns default(Wnd) if not found. To check it you can use <see cref="Is0"/> or <see cref="ExtensionMethods.OrThrow(Wnd)"/>.
		/// </summary>
		/// <param name="id">Control id.</param>
		/// <param name="directChild">Must be direct child, not a child of a child and so on.</param>
		/// <remarks>
		/// Finds hidden controls too. Does not prefer visible controls.
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
					return new _ChildByIdEnum(id).Find(this);
				}
			}
			return R;
		}

		//Used for API EnumChildWindows lParam instead of lambda, to avoid garbage.
		struct _ChildByIdEnum
		{
			int _id;
			Wnd _wFound;

			public _ChildByIdEnum(int id)
			{
				_id = id;
				_wFound = default;
			}

			public Wnd Find(Wnd wParent)
			{
				EnumChildWindows(wParent, _wndEnumProc, ref this);
				return _wFound;
			}

			delegate int WndEnumProcT(Wnd hwnd, ref _ChildByIdEnum d);

			static int _WndEnumProc(Wnd w, ref _ChildByIdEnum d) => d._WndEnumProc(w);
			static WndEnumProcT _wndEnumProc = _WndEnumProc;

			int _WndEnumProc(Wnd w)
			{
				if(w.ControlId != _id) return 1;
				_wFound = w; return 0;
			}

			[DllImport("user32.dll", SetLastError = true)]
			static extern bool EnumChildWindows(Wnd hWndParent, WndEnumProcT lpEnumFunc, ref _ChildByIdEnum d);
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
		//	Wnd R = default;
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
		public Wnd[] ChildAll(string name = null, string className = null, WCFlags flags = 0, Func<Wnd, bool> also = null)
		{
			//ThrowIfInvalid(); //will be called later
			var f = new ChildFinder(name, className, flags, also);
			var a = f.FindAllIn(this);
			//CONSIDER: add property LastChildParams, like LastFind.
			return a;
		}

		/// <summary>
		/// Finds a direct child control and returns its handle as Wnd.
		/// Returns default(Wnd) if not found. To check it you can use <see cref="Is0"/> or <see cref="ExtensionMethods.OrThrow(Wnd)"/>.
		/// Calls API <msdn>FindWindowEx</msdn>.
		/// Faster than <see cref="Child">Child</see>, which uses API <msdn>EnumChildWindows</msdn>.
		/// Can be used only when you know full name and/or class name.
		/// Finds hidden controls too.
		/// </summary>
		/// <param name="name">
		/// Name.
		/// Use null to match any. "" matches "" (no name).
		/// Full, case-insensitive. Wildcard etc not supported.
		/// Must include the invisible '&amp;' characters that are used to underline keyboard shortcuts with the Alt key.
		/// </param>
		/// <param name="className">
		/// Class name.
		/// Use null to match any. Cannot be "".
		/// Full, case-insensitive. Wildcard etc not supported.
		/// </param>
		/// <param name="wAfter">If used, starts searching from the next control in the Z order.</param>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>.
		/// </remarks>
		public Wnd ChildFast(string name, string className, Wnd wAfter = default)
		{
			//ThrowIfInvalid(); //no, it can be Message
			if(Is0) {
				Api.SetLastError(Api.ERROR_INVALID_WINDOW_HANDLE);
				return default;
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
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		/// <remarks>
		/// Calls API <msdn>EnumChildWindows</msdn>.
		/// </remarks>
		public Wnd[] AllChildren(bool directChild = false, bool onlyVisible = false, bool sortFirstVisible = false)
		{
			ThrowIfInvalid();
			return Lib.EnumWindows(Lib.EnumWindowsAPI.EnumChildWindows, onlyVisible, sortFirstVisible, this, directChild);
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

		/// <summary>
		/// Like <see cref="Wnd"/>, but has only button, check box and radio button functions - Click, Check etc.
		/// </summary>
		public struct WButton
		{
			/// <summary>
			/// Button handle as Wnd.
			/// </summary>
			public Wnd W { get; }

			WButton(Wnd w) { W = w; }

			/// <summary>
			/// Implicit cast Wnd=WButton.
			/// </summary>
			public static implicit operator Wnd(WButton b) => b.W;
			/// <summary>
			/// Explicit cast WButton=(WButton)Wnd.
			/// </summary>
			public static explicit operator WButton(Wnd w) => new WButton(w);

			///
			public override string ToString()
			{
				return W.ToString();
			}

			/// <summary>
			/// Sends a "click" message to this button control. Does not use the mouse.
			/// </summary>
			/// <param name="useAcc">Use <see cref="Acc.DoDefaultAction"/>. If false (default), posts <msdn>BM_CLICK</msdn> message.</param>
			/// <exception cref="WndException">This window is invalid.</exception>
			/// <exception cref="CatException">Failed.</exception>
			/// <remarks>
			/// Works not with all button controls. Sometimes does not work if the window is inactive.
			/// Check boxes and radio buttons also are buttons. This function can click them.
			/// </remarks>
			/// <example>
			/// <code><![CDATA[
			/// Wnd.Find("Options").Child("Cancel").AsButton.Click();
			/// ]]></code>
			/// </example>
			public void Click(bool useAcc = false)
			{
				W.ThrowIfInvalid();
				if(useAcc) {
					var a = Acc.FromWindow(W, AccOBJID.CLIENT); //tested: fails if objid_window
					a.DoDefaultAction();
				} else {
					_PostBmClick(); //async if other thread, because may show a dialog.
				}
				W._MinimalWaitIfOtherThread();
				//FUTURE: sync better
			}

			void _PostBmClick()
			{
				var w = W.WndWindow;
				bool workaround = !w.IsActive;
				if(workaround) w.Post(Api.WM_ACTIVATE, 1); //workaround for the documented BM_CLICK bug
				W.Post(BM_CLICK); //it sends WM_LBUTTONDOWN/UP
				if(workaround) w.Post(Api.WM_ACTIVATE, 0);
			}

			/// <summary>
			/// Checks or unchecks this check box. Does not use the mouse.
			/// Calls <see cref="SetCheckState"/> with state 0 or 1.
			/// </summary>
			/// <param name="on">Checks if true, unchecks if false.</param>
			/// <param name="useAcc"></param>
			/// <exception cref="WndException">This window is invalid.</exception>
			/// <exception cref="CatException">Failed.</exception>
			/// <remarks>
			/// Works not with all button controls. Sometimes does not work if the window is inactive.
			/// If this is a radio button, does not uncheck other radio buttons in its group.
			/// </remarks>
			public void Check(bool on, bool useAcc = false)
			{
				SetCheckState(on ? 1 : 0, useAcc);
			}

			/// <summary>
			/// Sets checkbox state. Does not use the mouse.
			/// </summary>
			/// <param name="state">0 unchecked, 1 checked, 2 indeterminate.</param>
			/// <param name="useAcc">Use <see cref="Acc.DoDefaultAction"/>. If false (default), posts <msdn>BM_SETCHECK</msdn> message and also BN_CLICKED notification to the parent window; if that is not possible, instead uses <msdn>BM_CLICK</msdn> message.</param>
			/// <exception cref="ArgumentOutOfRangeException">Invalid state.</exception>
			/// <exception cref="WndException">This window is invalid.</exception>
			/// <exception cref="CatException">Failed.</exception>
			/// <remarks>
			/// Does nothing if the check box already has the specified check state (if can get it).
			/// Works not with all button controls. Sometimes does not work if the window is inactive.
			/// If this is a radio button, does not uncheck other radio buttons in its group.
			/// </remarks>
			public void SetCheckState(int state, bool useAcc = false)
			{
				if(state < 0 || state > 2) throw new ArgumentOutOfRangeException();
				W.ThrowIfInvalid();
				int id;
				if(useAcc || !_IsCheckbox() || (uint)((id = W.ControlId) - 1) >= 0xffff) {
					var a = Acc.FromWindow(W, AccOBJID.CLIENT);
					int k = _GetAccCheckState(ref a);
					if(k == state) return;
					if(useAcc) a.DoDefaultAction(); else _PostBmClick();
					bool clickAgain = false;
					switch(state) {
					case 0:
						if(k == 1) {
							W._MinimalWaitIfOtherThread();
							if(GetCheckState(true) == 2) clickAgain = true;
							else return;
						}
						break;
					case 1:
						if(k == 2) clickAgain = true;
						break;
					case 2:
						if(k == 0) clickAgain = true;
						break;
					}
					if(clickAgain) {
						if(useAcc) a.DoDefaultAction(); else _PostBmClick();
					}
				} else {
					if(state == W.Send(BM_GETCHECK)) return;
					W.Post(BM_SETCHECK, state);
					W.WndDirectParent.Post(Api.WM_COMMAND, id, (LPARAM)W);
				}
				W._MinimalWaitIfOtherThread();
			}

			/// <summary>
			/// Gets check state of this check box or radio button.
			/// Calls <see cref="GetCheckState"/> and returns true if it returns 1.
			/// </summary>
			public bool IsChecked(bool useAcc = false)
			{
				return 1 == GetCheckState(useAcc);
			}

			/// <summary>
			/// Gets check state of this check box or radio button.
			/// Returns 0 if unchecked, 1 if checked, 2 if indeterminate. Also returns 0 if this is not a button or if failed to get state.
			/// </summary>
			/// <param name="useAcc">Use <see cref="Acc.State"/>. If false (default) and this button has a standard checkbox style, uses API <msdn>BM_GETCHECK</msdn>.</param>
			public int GetCheckState(bool useAcc = false)
			{
				if(useAcc || !_IsCheckbox()) {
					//info: Windows Forms controls are user-drawn and don't have one of the styles, therefore BM_GETCHECK does not work.
					try { //avoid exception in property-get functions
						var a = Acc.FromWindow(W, AccOBJID.CLIENT, noThrow: true);
						return _GetAccCheckState(ref a);
					}
					catch(Exception ex) { Debug_.Print(ex); } //CONSIDER: if fails, show warning. In all Wnd property-get functions.
					return 0;
				} else {
					return W.Send(BM_GETCHECK);
				}
			}

			int _GetAccCheckState(ref Acc a)
			{
				var state = a.State;
				if(state.Has_(AccSTATE.INDETERMINATE)) return 2;
				if(state.Has_(AccSTATE.CHECKED)) return 1;
				return 0;
			}

			bool _IsCheckbox()
			{
				switch(W.Style & 15) {
				case BS_CHECKBOX:
				case BS_AUTOCHECKBOX:
				case BS_RADIOBUTTON:
				case BS_3STATE:
				case BS_AUTO3STATE:
				case BS_AUTORADIOBUTTON:
					return true;
				}
				return false;
			}

			internal const uint BM_CLICK = 0xF5;
			internal const uint BM_GETCHECK = 0xF0;
			internal const uint BM_SETCHECK = 0xF1;

			internal const uint BS_CHECKBOX = 0x2;
			internal const uint BS_AUTOCHECKBOX = 0x3;
			internal const uint BS_RADIOBUTTON = 0x4;
			internal const uint BS_3STATE = 0x5;
			internal const uint BS_AUTO3STATE = 0x6;
			internal const uint BS_AUTORADIOBUTTON = 0x9;

		}

		/// <summary>
		/// Casts this to <see cref="WButton"/>.
		/// </summary>
		public WButton AsButton { get => (WButton)this; }

		/// <summary>
		/// Finds a child button by id and sends a "click" message. Does not use the mouse.
		/// Calls <see cref="WButton.Click(bool)"/>.
		/// </summary>
		/// <param name="buttonId">Control id of the button. This function calls <see cref="ChildById"/> to find the button.</param>
		/// <param name="useAcc">Use <see cref="Acc.DoDefaultAction"/>. If false (default), posts <msdn>BM_CLICK</msdn> message.</param>
		/// <exception cref="NotFoundException">Button not found.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="ChildById"/> and <see cref="WButton.Click(bool)"/>.</exception>
		/// <example>
		/// <code><![CDATA[
		/// Wnd.Find("Options").ButtonClick(2);
		/// ]]></code>
		/// </example>
		public void ButtonClick(int buttonId, bool useAcc = false)
		{
			var c = ChildById(buttonId);
			if(c.Is0) throw new NotFoundException();
			c.AsButton.Click(useAcc);
		}

		/// <summary>
		/// Finds a child button by name and sends a "click" message. Does not use the mouse.
		/// Calls <see cref="WButton.Click(bool)"/>.
		/// </summary>
		/// <param name="buttonName">Button name. This function calls <see cref="Child"/> to find the button.</param>
		/// <param name="className">Button class name to pass to <see cref="Child"/>.</param>
		/// <param name="useAcc">Use <see cref="Acc.DoDefaultAction"/>. If false (default), posts <msdn>BM_CLICK</msdn> message.</param>
		/// <exception cref="NotFoundException">Button not found.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Child"/> and <see cref="WButton.Click(bool)"/>.</exception>
		/// <example>
		/// <code><![CDATA[
		/// Wnd.Find("Options").ButtonClick("Cancel");
		/// ]]></code>
		/// </example>
		public void ButtonClick(string buttonName, string className = null, bool useAcc = false)
		{
			var c = Child(buttonName, className);
			if(c.Is0) throw new NotFoundException(); //CONSIDER: try to find accessible object. Eg toolbar button.
			c.AsButton.Click(useAcc);
		}

		/// <summary>
		/// Posts a "menu item clicked" notification (<msdn>WM_COMMAND</msdn>) as if that menu item was clicked. Does not use the mouse.
		/// </summary>
		/// <param name="itemId">Menu item id. Must be in range 1 to 0xffff.</param>
		/// <param name="systemMenu">The menu item is in the title bar's context menu, not in the menu bar. Posts <msdn>WM_SYSCOMMAND</msdn> instead.</param>
		/// <exception cref="WndException">Invalid window.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid itemId.</exception>
		/// <remarks>
		/// Works only with standard (classic) menus. The drop-down menu window class name must be "#32768". Works with menu items in window menu bar, system menu and some context menus.
		/// Does not use the menu itself. Just posts WM_COMMAND or WM_SYSCOMMAND message. Even if a menu item with this id does not exist.
		/// This variable is the window that contains the menu bar or system menu. Or the drop-down menu window (class "#32768") that contains the menu item.
		/// </remarks>
		public void MenuClick(int itemId, bool systemMenu = false)
		{
			if((uint)(itemId - 1) >= 0xffff) throw new ArgumentOutOfRangeException();
			ThrowIfInvalid();
			var w = this;
			if(ClassNameIs("#32768") && Misc.GetGUIThreadInfo(out var g, ThreadId) && !g.hwndMenuOwner.Is0) w = g.hwndMenuOwner;
			w.Post(systemMenu ? Api.WM_SYSCOMMAND : Api.WM_COMMAND, itemId);
			w._MinimalWaitIfOtherThread();
		}

		//rejected: use Acc functions instead.
		///// <summary>
		///// Finds a menu item by name and posts a "menu item clicked" notification as if that menu item was clicked. Does not use the mouse.
		///// Works with all standard menus and some non-standard menus.
		///// </summary>
		///// <param name="itemName">
		///// Menu item name.
		///// String format: <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink>.
		///// </param>
		///// <param name="systemMenu">The menu item is in the title bar's context menu, not in the menu bar.</param>
		//public void Click(string itemName, bool systemMenu = false)
		//{
		//	
		//}

		//rejected: need just 1 function. To get state, use Acc.
		///// <summary>
		///// Click standard (classic) menu items, get state.
		///// </summary>
		//public static class Menu
		//{

		//}
	}
}

namespace Catkeys.Types
{
	/// <summary>
	/// 'flags' parameter of <see cref="Wnd.Child"/>.
	/// <note>
	/// </note>
	/// </summary>
	[Flags]
	public enum WCFlags
	{
		/// <summary>Can find hidden controls.</summary>
		HiddenToo = 1,
		/// <summary>Skip indirect descendant controls (children of children and so on).</summary>
		DirectChild = 2,
	}
}
