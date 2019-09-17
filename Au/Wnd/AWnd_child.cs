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
//using System.Linq;

using Au.Types;
using static Au.AStatic;
using Au.Util;

namespace Au
{
	public unsafe partial struct AWnd
	{
		/// <summary>
		/// Contains control (child window) properties and is used to find the control.
		/// </summary>
		/// <remarks>
		/// Can be used instead of <see cref="AWnd.Child"/> or <see cref="AWnd.ChildAll"/>.
		/// Also can be used to find window that contains certain control, like in the example.
		/// </remarks>
		/// <example>
		/// Find window that contains certain control, and get the control too.
		/// <code><![CDATA[
		/// var f = new AWnd.ChildFinder("Password*", "Static"); //control properties
		/// AWnd w = AWnd.Find(cn: "#32770", also: t => f.Find(t));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public class ChildFinder
		{
			enum _NameIs { name, id, text, accName, wfName }

			readonly AWildex _name;
			readonly AWildex _className;
			readonly Func<AWnd, bool> _also;
			AWinFormsControlNames _wfControls;
			readonly int _skipCount;
			readonly WCFlags _flags;
			readonly _NameIs _nameIs;
			readonly int _id;

			/// <summary>
			/// See <see cref="AWnd.Child"/>.
			/// </summary>
			/// <exception cref="ArgumentException">See <see cref="Child"/>.</exception>
			public ChildFinder(string name = null, string cn = null, WCFlags flags = 0, Func<AWnd, bool> also = null, int skip = 0)
			{
				if(cn != null) {
					if(cn.Length == 0) throw new ArgumentException("Class name cannot be \"\". Use null.");
					_className = cn;
				}
				if(name != null) {
					switch(AStringUtil.ParseParam3Stars(ref name, "id", "text", "accName", "wfName"/*, "label"*/)) {
					case -1: throw new ArgumentException("Invalid name prefix. Can be: \"***id \", \"***text \", \"***accName \", \"***wfName \"."); //, \"***label \"
					case 1: _nameIs = _NameIs.id; _id = name.ToInt(); break;
					case 2: _nameIs = _NameIs.text; break;
					case 3: _nameIs = _NameIs.accName; break;
					case 4: _nameIs = _NameIs.wfName; break;
						//case 5: _nameIs = _NameIs.label; break;
					}
					if(_nameIs != _NameIs.id) _name = name;
				}
				_flags = flags;
				_also = also;
				_skipCount = skip;
			}

			/// <summary>
			/// The found control.
			/// </summary>
			public AWnd Result { get; internal set; }

			/// <summary>
			/// Finds the specified child control, like <see cref="AWnd.Child"/>.
			/// Returns true if found.
			/// The <see cref="Result"/> property will be the control.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window. Can be top-level window or control.</param>
			/// <exception cref="WndException">Invalid wParent.</exception>
			public bool Find(AWnd wParent)
			{
				using var k = new _WndList(_AllChildren(wParent));
				return _FindInList(wParent, k) >= 0;
			}

			Util.LibArrayBuilder<AWnd> _AllChildren(AWnd wParent)
			{
				wParent.ThrowIfInvalid();
				return Lib.EnumWindows2(Lib.EnumAPI.EnumChildWindows,
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
			/// <param name="a">List of controls, for example returned by <see cref="GetWnd.Children"/>.</param>
			/// <param name="wParent">Direct or indirect parent window. Used only for flag DirectChild.</param>
			public int FindInList(IEnumerable<AWnd> a, AWnd wParent = default)
			{
				using var k = new _WndList(a);
				return _FindInList(wParent, k);
			}

			/// <summary>
			/// Finds all matching child controls, like <see cref="ChildAll"/>.
			/// Returns array containing 0 or more control handles as AWnd.
			/// </summary>
			/// <param name="wParent">Direct or indirect parent window. Can be top-level window or control.</param>
			/// <exception cref="WndException">Invalid wParent.</exception>
			public AWnd[] FindAll(AWnd wParent)
			{
				return _FindAll(new _WndList(_AllChildren(wParent)), wParent);
			}

			/// <summary>
			/// Finds all matching controls in a list of controls.
			/// Returns array containing 0 or more control handles as AWnd.
			/// </summary>
			/// <param name="a">List of controls, for example returned by <see cref="GetWnd.Children"/>.</param>
			/// <param name="wParent">Direct or indirect parent window. Used only for flag DirectChild.</param>
			public AWnd[] FindAllInList(IEnumerable<AWnd> a, AWnd wParent = default)
			{
				return _FindAll(new _WndList(a), wParent);
			}

			AWnd[] _FindAll(_WndList k, AWnd wParent)
			{
				using(k) {
					using var ab = new Util.LibArrayBuilder<AWnd>();
					_FindInList(wParent, k, w => ab.Add(w)); //CONSIDER: ab could be part of _WndList. Now the delegate creates garbage.
					return ab.ToArray();
				}
			}

			/// <summary>
			/// Returns index of matching element or -1.
			/// Returns -1 if using getAll.
			/// </summary>
			/// <param name="wParent">Parent window. Can be default(AWnd) if inList is true and no DirectChild flag and not using winforms name.</param>
			/// <param name="a">List of AWnd. Does not dispose it.</param>
			/// <param name="getAll">If not null, calls it for all matching and returns -1.</param>
			int _FindInList(AWnd wParent, _WndList a, Action<AWnd> getAll = null)
			{
				Result = default;
				if(a.Type == _WndList.ListType.None) return -1;
				bool inList = a.Type != _WndList.ListType.ArrayBuilder;
				int skipCount = _skipCount;

				try { //will need to dispose something
					for(int index = 0; a.Next(out AWnd w); index++) {
						if(w.Is0) continue;

						if(inList) { //else the enum function did this
							if(!_flags.Has(WCFlags.HiddenToo)) {
								if(!w.LibIsVisibleIn(wParent)) continue;
							}

							if(_flags.Has(WCFlags.DirectChild) && !wParent.Is0) {
								if(w.LibParentGWL != wParent) continue;
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
							//case _NameIs.label:
							//	s = w.NameLabel;
							//	break;
							case _NameIs.accName:
								s = w.NameAcc;
								break;
							case _NameIs.wfName:
								if(_wfControls == null) {
									try {
										_wfControls = new AWinFormsControlNames(wParent.Is0 ? w : wParent);
									}
									catch(WndException) { //invalid parent window
										return -1;
									}
									catch(AuException e) { //probably process of higher UAC integrity level
										PrintWarning($"Failed to get .NET control names. {e.Message}");
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
			public bool IsMatch(AWnd c, AWnd wParent = default)
			{
				if(!wParent.Is0 && !c.IsChildOf(wParent)) {
					Result = default;
					return false;
				}
				return 0 == _FindInList(wParent, new _WndList(c));
			}
		}

		/// <summary>
		/// Finds a child control and returns its handle as AWnd.
		/// Returns default(AWnd) if not found. See also: <see cref="Is0"/>, <see cref="AExtAu.OrThrow(AWnd)"/>.
		/// </summary>
		/// <param name="name">
		/// Control name.
		/// String format: [](xref:wildcard_expression).
		/// null means 'can be any'. "" means 'no name'.
		/// 
		/// By default to get control names this function uses <see cref="Name"/>.
		/// Can start with these prefix strings:
		/// - <c>"***text "</c> - use <see cref="ControlText"/>.
		/// <br/>Slower and can be less reliable (because can get editable text), especially if not used cn (class name). Does not remove the invisible '&amp;' characters that are used to underline keyboard shortcuts with the Alt key.
		/// - <c>"***accName "</c> - use <see cref="NameAcc"/>.
		/// <br/>Useful when the control itself does not have a name but an adjacent Static text control is used as its name. Examples - Edit controls in dialogs. Slower.
		/// - <c>"***wfName "</c> - use .NET Windows Forms Control Name property.
		/// <br/>To get it this function uses <see cref="AWinFormsControlNames"/>. Slower and can fail because of [](xref:uac).
		/// - <c>"***id "</c> like <c>"***id 15"</c> - use control id.
		/// <br/>To get it this function uses <see cref="ControlId"/>.
		/// <br/>The id value cannot be wildcard expression.
		/// <br/>See also <see cref="ChildById"/>.
		/// </param>
		/// <param name="cn">
		/// Control class name.
		/// String format: [](xref:wildcard_expression).
		/// null means 'can be any'. Cannot be "".
		/// You can see control class name etc in editor's status bar and dialog "Find window or control".
		/// </param>
		/// <param name="flags"></param>
		/// <param name="also">
		/// Callback function. Called for each matching control.
		/// It can evaluate more properties of the control and return true when they match.
		/// Example: <c>also: t =&gt; t.IsEnabled</c>
		/// </param>
		/// <param name="skip">
		/// 0-based index of matching control.
		/// For example, if 1, the function skips the first matching control and returns the second.
		/// </param>
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		/// <exception cref="ArgumentException">
		/// - <i>name</i> starts with <c>"***"</c>, but the prefix is invalid.
		/// - <i>cn</i> is "". To match any, use null.
		/// - Invalid wildcard expression (<c>"**options "</c> or regular expression).
		/// </exception>
		/// <remarks>
		/// To create code for this function, use dialog "Find window or control". It is form <b>Au.Tools.FormAWnd</b> in Au.Tools.dll.
		/// </remarks>
		public AWnd Child(string name = null, string cn = null, WCFlags flags = 0, Func<AWnd, bool> also = null, int skip = 0)
		{
			//ThrowIfInvalid(); //will be called later
			var f = new ChildFinder(name, cn, flags, also, skip);
			f.Find(this);
			return f.Result;
		}

		/// <summary>
		/// Returns true if this window contains the specified control.
		/// Calls <see cref="Child"/>.
		/// <note>
		/// Using this function many times with same parameters is inefficient. Instead create new <see cref="ChildFinder"/> and call <see cref="ChildFinder.Find"/> or <see cref="HasChild(ChildFinder)"/>. See example.
		/// </note>
		/// </summary>
		/// <exception cref="WndException"/>
		/// <exception cref="ArgumentException"/>
		/// <example>
		/// <code><![CDATA[
		/// //find window that contains certain control, and get the control too
		/// var f = new AWnd.ChildFinder("Password*", "Static"); //control properties
		/// AWnd w = AWnd.Find(cn: "#32770", also: t => t.HasChild(f));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public bool HasChild(string name = null, string cn = null, WCFlags flags = 0, Func<AWnd, bool> also = null, int skip = 0)
		{
			return default != Child(name, cn, flags, also, skip);
		}

		/// <summary>
		/// Returns true if this window contains the specified control.
		/// Calls <see cref="ChildFinder.Find"/>.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <example>
		/// Find window that contains certain control, and get the control too.
		/// <code><![CDATA[
		/// var cf = new AWnd.ChildFinder("Password*", "Static"); //control properties
		/// AWnd w = AWnd.Find(cn: "#32770", also: t => t.HasChild(cf));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public bool HasChild(ChildFinder f)
		{
			return f.Find(this);
		}

		/// <summary>
		/// Returns true if this window contains the specified accessible object.
		/// Calls <see cref="AAcc.Finder.Find(AWnd, AWnd.ChildFinder)"/>.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <example>
		/// Find window that contains certain accessible object (AO), and get the AO too.
		/// <code><![CDATA[
		/// var af = new AAcc.Finder("BUTTON", "OK"); //AO properties
		/// AWnd w = AWnd.Find(cn: "#32770", also: t => t.HasAcc(af));
		/// Print(w);
		/// Print(f.Result);
		/// ]]></code>
		/// </example>
		public bool HasAcc(AAcc.Finder f)
		{
			return f.Find(this);
		}

		/// <summary>
		/// Finds a child control by its id and returns its handle as AWnd.
		/// Returns default(AWnd) if not found. See also: <see cref="Is0"/>, <see cref="AExtAu.OrThrow(AWnd)"/>.
		/// </summary>
		/// <param name="id">Control id.</param>
		/// <param name="flags">This function supports flags DirectChild and HiddenToo. If both are set, it is much faster because uses API <msdn>GetDlgItem</msdn>. Else uses API <msdn>EnumChildWindows</msdn>, like <see cref="Child"/>.</param>
		/// <remarks>
		/// To create code for this function, use dialog "Find window or control". It is form <b>Au.Tools.FormAWnd</b> in Au.Tools.dll.
		/// 
		/// Not all controls have a useful id. If control id is not unique or is different in each window instance, this function is not useful.
		/// </remarks>
		/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
		public AWnd ChildById(int id, WCFlags flags = 0)
		{
			ThrowIfInvalid();
			if(flags.Has(WCFlags.DirectChild | WCFlags.HiddenToo)) return Api.GetDlgItem(this, id); //fast

			var d = new _KidEnumData() { wThis = this, id = id }; //info: to avoid garbage delegates, we use _KidEnumData instead of captured variables
			var wParent = this;
			Api.EnumChildWindows(this, (c, p) => {
				ref var x = ref *(_KidEnumData*)p;
				if(c.ControlId == x.id) {
					if(x.flags.Has(WCFlags.DirectChild) && c.LibParentGWL != x.wThis) return 1;
					if(c.LibIsVisibleIn(wParent)) { x.cVisible = c; return 0; }
					if(x.flags.Has(WCFlags.HiddenToo) && x.cHidden.Is0) x.cHidden = c;
				}
				return 1;
			}, &d);
			return d.cVisible.Is0 ? d.cHidden : d.cVisible;
		}

		struct _KidEnumData
		{
			public AWnd wThis, cVisible, cHidden;
			public int id;
			public WCFlags flags;
		}

		/// <summary>
		/// Finds all matching child controls.
		/// Returns List containing 0 or more control handles as AWnd.
		/// Everything except the return type is the same as with <see cref="Child"/>.
		/// </summary>
		/// <exception cref="WndException"/>
		/// <exception cref="ArgumentException"/>
		/// <remarks>
		/// In the returned list, hidden controls (when using WCFlags.HiddenToo) are always after visible controls.
		/// </remarks>
		/// <seealso cref="GetWnd.Children"/>
		public AWnd[] ChildAll(string name = null, string cn = null, WCFlags flags = 0, Func<AWnd, bool> also = null)
		{
			//ThrowIfInvalid(); //will be called later
			var f = new ChildFinder(name, cn, flags, also);
			return f.FindAll(this);
		}

		/// <summary>
		/// Finds a direct child control and returns its handle as AWnd.
		/// Returns default(AWnd) if not found. See also: <see cref="Is0"/>, <see cref="AExtAu.OrThrow(AWnd)"/>.
		/// Calls API <msdn>FindWindowEx</msdn>.
		/// Faster than <see cref="Child"/>, which uses API <msdn>EnumChildWindows</msdn>.
		/// Can be used only when you know full name and/or class name.
		/// Finds hidden controls too.
		/// </summary>
		/// <param name="name">
		/// Name.
		/// Full, case-insensitive. Wildcard etc not supported.
		/// null means 'can be any'. "" means 'no name'.
		/// Must include the invisible '&amp;' characters that are used to underline keyboard shortcuts with the Alt key.
		/// </param>
		/// <param name="cn">
		/// Class name.
		/// Full, case-insensitive. Wildcard etc not supported.
		/// null means 'can be any'. Cannot be "".
		/// </param>
		/// <param name="wAfter">If used, starts searching from the next control in the Z order.</param>
		/// <remarks>
		/// Supports <see cref="ALastError"/>.
		/// </remarks>
		public AWnd ChildFast(string name, string cn, AWnd wAfter = default)
		{
			//ThrowIfInvalid(); //no, it can be Message
			if(Is0) {
				Api.SetLastError(Api.ERROR_INVALID_WINDOW_HANDLE);
				return default;
			}
			return Api.FindWindowEx(this, wAfter, cn, name);
		}

		public partial struct GetWnd
		{
			/// <summary>
			/// Gets child controls, including all descendants.
			/// Returns array containing 0 or more control handles as AWnd.
			/// </summary>
			/// <param name="onlyVisible">Need only visible controls.</param>
			/// <param name="sortFirstVisible">Place all array elements of hidden controls at the end of the array.</param>
			/// <param name="directChild">Need only direct children, not all descendants.</param>
			/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// Calls API <msdn>EnumChildWindows</msdn>.
			/// </remarks>
			/// <seealso cref="ChildAll"/>
			public AWnd[] Children(bool onlyVisible = false, bool sortFirstVisible = false, bool directChild = false)
			{
				_w.ThrowIfInvalid();
				return Lib.EnumWindows(Lib.EnumAPI.EnumChildWindows, onlyVisible, sortFirstVisible, _w, directChild);
			}

			/// <summary>
			/// Gets child controls, including all descendants.
			/// </summary>
			/// <param name="a">Receives window handles as AWnd. If null, this function creates new List, else clears before adding items.</param>
			/// <param name="onlyVisible">Need only visible controls.</param>
			/// <param name="sortFirstVisible">Place all array elements of hidden controls at the end of the array.</param>
			/// <param name="directChild">Need only direct children, not all descendants.</param>
			/// <exception cref="WndException">This variable is invalid (window not found, closed, etc).</exception>
			/// <remarks>
			/// Use this overload to avoid much garbage when calling frequently with the same List variable. Other overload always allocates new array. This overload in most cases reuses memory allocated for the list variable.
			/// </remarks>
			public void Children(ref List<AWnd> a, bool onlyVisible = false, bool sortFirstVisible = false, bool directChild = false)
			{
				_w.ThrowIfInvalid();
				Lib.EnumWindows2(Lib.EnumAPI.EnumChildWindows, onlyVisible, sortFirstVisible, _w, directChild, list: a ??= new List<AWnd>());
			}

			//rejected: unreliable.
			///// <summary>
			///// Gets list of direct child controls.
			///// Faster than API EnumChildWindows.
			///// Should be used only with windows of current thread. Else it is unreliable because, if some controls are zordered or destroyed while enumerating, some controls can be skipped or retrieved more than once.
			///// </summary>
			//public static AWnd[] DirectChildrenFastUnsafe(string cn = null)
			//{
			//	AWildex wild = cn;
			//	var a = new List<AWnd>();
			//	for(AWnd c = FirstChild; !c.Is0; c = c.Next) {
			//		if(wild != null && !c._ClassNameIs(wild)) continue;
			//		a.Add(c);
			//	}
			//	return a.ToArray();
			//}
		}

		/// <summary>
		/// Like <see cref="AWnd"/>, but has only button, check box and radio button functions - Click, Check etc.
		/// </summary>
		public struct WButton
		{
			/// <summary>
			/// Button handle as AWnd.
			/// </summary>
			public AWnd W { get; }

			WButton(AWnd w) { W = w; }

			/// <summary>
			/// Implicit cast AWnd=WButton.
			/// </summary>
			public static implicit operator AWnd(WButton b) => b.W;
			/// <summary>
			/// Explicit cast WButton=(WButton)AWnd.
			/// </summary>
			public static explicit operator WButton(AWnd w) => new WButton(w);

			///
			public override string ToString()
			{
				return W.ToString();
			}

			/// <summary>
			/// Sends a "click" message to this button control. Does not use the mouse.
			/// </summary>
			/// <param name="useAcc">Use <see cref="AAcc.DoAction"/>. If false (default), posts <msdn>BM_CLICK</msdn> message.</param>
			/// <exception cref="WndException">This window is invalid.</exception>
			/// <exception cref="AuException">Failed.</exception>
			/// <remarks>
			/// Works not with all button controls. Sometimes does not work if the window is inactive.
			/// Check boxes and radio buttons also are buttons. This function can click them.
			/// </remarks>
			/// <example>
			/// <code><![CDATA[
			/// AWnd.Find("Options").Child("Cancel").AsButton.Click();
			/// ]]></code>
			/// </example>
			public void Click(bool useAcc = false)
			{
				W.ThrowIfInvalid();
				if(useAcc) {
					using var a = AAcc.FromWindow(W, AccOBJID.CLIENT); //throws if failed
					a.DoAction();
				} else {
					_PostBmClick(); //async if other thread, because may show a dialog.
				}
				W.LibMinimalSleepIfOtherThread();
				//FUTURE: sync better
			}

			void _PostBmClick()
			{
				var w = W.Window;
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
			/// <exception cref="AuException">Failed.</exception>
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
			/// <param name="useAcc">Use <see cref="AAcc.DoAction"/>. If false (default), posts <msdn>BM_SETCHECK</msdn> message and also BN_CLICKED notification to the parent window; if that is not possible, instead uses <msdn>BM_CLICK</msdn> message.</param>
			/// <exception cref="ArgumentOutOfRangeException">Invalid state.</exception>
			/// <exception cref="WndException">This window is invalid.</exception>
			/// <exception cref="AuException">Failed.</exception>
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
					using var a = AAcc.FromWindow(W, AccOBJID.CLIENT); //throws if failed
					int k = _GetAccCheckState(a);
					if(k == state) return;
					if(useAcc) a.DoAction(); else _PostBmClick();
					bool clickAgain = false;
					switch(state) {
					case 0:
						if(k == 1) {
							W.LibMinimalSleepIfOtherThread();
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
						if(useAcc) a.DoAction(); else _PostBmClick();
					}
				} else {
					if(state == W.Send(BM_GETCHECK)) return;
					W.Post(BM_SETCHECK, state);
					W.Get.DirectParent.Post(Api.WM_COMMAND, id, (LPARAM)W);
				}
				W.LibMinimalSleepIfOtherThread();
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
			/// <param name="useAcc">Use <see cref="AAcc.State"/>. If false (default) and this button has a standard checkbox style, uses API <msdn>BM_GETCHECK</msdn>.</param>
			public int GetCheckState(bool useAcc = false)
			{
				if(useAcc || !_IsCheckbox()) {
					//info: Windows Forms controls are user-drawn and don't have one of the styles, therefore BM_GETCHECK does not work.
					try { //avoid exception in property-get functions
						using var a = AAcc.FromWindow(W, AccOBJID.CLIENT, flags: AWFlags.NoThrow);
						if(a == null) return 0;
						return _GetAccCheckState(a);
					}
					catch(Exception ex) { ADebug.Print(ex); } //CONSIDER: if fails, show warning. In all AWnd property-get functions.
					return 0;
				} else {
					return (int)W.Send(BM_GETCHECK);
				}
			}

			int _GetAccCheckState(AAcc a)
			{
				var state = a.State;
				if(state.Has(AccSTATE.INDETERMINATE)) return 2;
				if(state.Has(AccSTATE.CHECKED)) return 1;
				return 0;
			}

			bool _IsCheckbox()
			{
				switch((uint)W.Style & 15) {
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

			internal const int BM_CLICK = 0xF5;
			internal const int BM_GETCHECK = 0xF0;
			internal const int BM_SETCHECK = 0xF1;

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
		public WButton AsButton => (WButton)this;

		/// <summary>
		/// Finds a child button by id and sends a "click" message. Does not use the mouse.
		/// Calls <see cref="WButton.Click(bool)"/>.
		/// </summary>
		/// <param name="buttonId">Control id of the button. This function calls <see cref="ChildById"/> to find the button.</param>
		/// <param name="useAcc">Use <see cref="AAcc.DoAction"/>. If false (default), posts <msdn>BM_CLICK</msdn> message.</param>
		/// <exception cref="NotFoundException">Button not found.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="ChildById"/> and <see cref="WButton.Click(bool)"/>.</exception>
		/// <example>
		/// <code><![CDATA[
		/// AWnd.Find("Options").ButtonClick(2);
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
		/// <param name="cn">Button class name to pass to <see cref="Child"/>.</param>
		/// <param name="useAcc">Use <see cref="AAcc.DoAction"/>. If false (default), posts <msdn>BM_CLICK</msdn> message.</param>
		/// <exception cref="NotFoundException">Button not found.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Child"/> and <see cref="WButton.Click(bool)"/>.</exception>
		/// <example>
		/// <code><![CDATA[
		/// AWnd.Find("Options").ButtonClick("Cancel");
		/// ]]></code>
		/// </example>
		public void ButtonClick(string buttonName, string cn = null, bool useAcc = false)
		{
			var c = Child(buttonName, cn);
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
			if(ClassNameIs("#32768") && More.GetGUIThreadInfo(out var g, ThreadId) && !g.hwndMenuOwner.Is0) w = g.hwndMenuOwner;
			w.Post(systemMenu ? Api.WM_SYSCOMMAND : Api.WM_COMMAND, itemId);
			w.LibMinimalSleepIfOtherThread();
		}

		//rejected: use AAcc functions instead.
		///// <summary>
		///// Finds a menu item by name and posts a "menu item clicked" notification as if that menu item was clicked. Does not use the mouse.
		///// Works with all standard menus and some non-standard menus.
		///// </summary>
		///// <param name="itemName">
		///// Menu item name.
		///// String format: [](xref:wildcard_expression).
		///// </param>
		///// <param name="systemMenu">The menu item is in the title bar's context menu, not in the menu bar.</param>
		//public void Click(string itemName, bool systemMenu = false)
		//{
		//	
		//}

		//rejected: need just 1 function. To get state, use AAcc.
		///// <summary>
		///// Click standard (classic) menu items, get state.
		///// </summary>
		//public static class Menu
		//{

		//}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="AWnd.Child"/>.
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
