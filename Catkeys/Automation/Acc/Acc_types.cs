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

using static Catkeys.NoClass;

namespace Catkeys.Types
{
	/// <summary>
	/// Flags for <see cref="Acc.Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, string)"/> and similar functions.
	/// </summary>
	[Flags]
	public enum AFFlags
	{
		/// <summary>
		/// The accessible object must be direct child, not a descendant at any other level.
		/// This can be useful when searching in Acc. When searching in Wnd, these are children of the WINDOW object; if using control class name - of control's WINDOW object.
		/// </summary>
		DirectChild = 1,

		/// <summary>
		/// The accessible object can be invisible.
		/// Without this flag skips objects that are invisible (have state INVISIBLE) or are descendants of invisible WINDOW, DOCUMENT, PANE, PROPERTYPAGE, GROUPING, ALERT, MENUBAR, TITLEBAR, SCROLLBAR.
		/// </summary>
		HiddenToo = 2,

		/// <summary>
		/// Always search in MENUITEM.
		/// Without this flag skips MENUITEM descendant objects to improve speed, unless role is MENUITEM or MENUPOPUP. Except when searching in web page or when used path.
		/// </summary>
		MenuToo = 4,

		/// <summary>
		/// Search in reverse order. It can make faster.
		/// When using control class name, controls are searched not in reverse order. Only accessible objects in them are searched in reverse order.
		/// </summary>
		Reverse = 8,

		/// <summary>
		/// Skip descendant objects of OUTLINE (tree view), LIST, TITLEBAR and SCROLLBAR.
		/// Objects of OUTLINE and LIST sometimes contain large number of items, which makes finding other objects much slower.
		/// See also <see cref="Acc.Finder.SkipRoles"/>.
		/// </summary>
		SkipLists = 16,

		/// <summary>
		/// Skip objects in web pages and DOCUMENT.
		/// Objects of these roles often contain large number of descendants, which makes finding other objects much slower.
		/// See also <see cref="Acc.Finder.SkipRoles"/>.
		/// </summary>
		SkipWeb = 32,

		/// <summary>
		/// This flag can be used with role prefix "web:" or "firefox:", when searching in Firefox.
		/// It disables waiting while DOCUMENT has BUSY state. This waiting is to improve reliability. However some web pages have busy state always, and it makes the function very slow.
		/// </summary>
		WebBusy = 64,

		//CONSIDER: SkipAllHiddenAncestors
	}

	/// <summary>
	/// Contains data for callback function (<b>also</b>) of <see cref="Acc.Find(Wnd, string, string, AFFlags, Func{AFAcc, bool}, string)"/> and similar functions.
	/// This class is derived from <see cref="Acc"/> and therefore you can use all Acc properties in callback function. Also has several own properties and methods.
	/// The AFAcc variable passed to the callback function is valid only in the callback function. If you need to get normal Acc variable from it (for example to add to a List), use <see cref="ToAcc"/>.
	/// </summary>
	public class AFAcc :Acc
	{
		/// <summary>
		/// 0-based level of this accessible object in the tree of accessible objects.
		/// Direct children have level 0, their children have level 1, and so on.
		/// When searching in a window or control, at level 0 are direct children of the WINDOW object of the window or control. The WINDOW object itself is not included in the search; if you need it, instead use <see cref="Acc.FromWindow"/>.
		/// When searching in Acc, at level 0 are its direct children.
		/// When searching in web page (role has prefix "web:" etc), at level 0 is the root object (DOCUMENT in Firefox/Chrome, PANE in Internet Explorer).
		/// When using control class name, at level 0 are direct children of WINDOW objects of matching controls.
		/// </summary>
		public int Level { get; private set; }

		/// <summary>
		/// An int value that is 0 before searching and can be modified in the callback function.
		/// Can be used to implement 'find n-th matching object' or for any other purpose. See example.
		/// When searching in multiple controls, Counter is 0 before searching in each control.
		/// </summary>
		/// <example>
		/// Find 4-th LINK in web page.
		/// <code><![CDATA[
		/// var a = Acc.Find(w, "web:LINK", also: o => ++o.Counter == 4);
		/// ]]></code>
		/// </example>
		public int Counter { get; set; }

		/// <summary>
		/// With <see cref="Acc.EnumChildren"/> - the param argument. With <see cref="Acc.Finder"/> - the Param property. Probably not useful with Acc.Find.
		/// The callback function can use it for any purpose.
		/// </summary>
		public object Param { get; set; }

		//TODO: IndexInParent, ParentChildCount

		/// <summary>
		/// The callback function can call this to stop searching. If the callback function then returns false, it means 'not found'. Don't need to call this if returns true.
		/// </summary>
		public void Stop() { stop = true; }
		internal bool stop;

		/// <summary>
		/// The callback function can call this to skip (don't search) descendants of current accessible object.
		/// </summary>
		public void SkipChildren() { skipChildren = true; }
		internal bool skipChildren;

		/// <summary>
		/// Creates new Acc variable from this AFAcc variable. It is a reference to the same accessible object.
		/// </summary>
		public Acc ToAcc() { return new Acc(this); }

		//CONSIDER:
		//public new AccSTATE State { get => _haveState ? _state : StateCurrent; }
		//internal void LibSetState(AccSTATE state) { _state=state; _haveState=true; }
		//AccSTATE _state; bool _haveState;
		//public AccSTATE StateCurrent { get => { _state = base.State; _haveState=true; return _state; } }

		/// <summary>
		/// Sets/clears fields/properties before calling the callback.
		/// </summary>
		internal void LibSet(IAccessible iacc, int elem, AccROLE role, int level)
		{
			_iacc = iacc; //info: this class will not release it. Finalizer disabled, Dispose not called.
			_elem = elem;
			_role = role;
			Level = level;
			stop = false;
			skipChildren = false;
		}

		/// <summary>
		/// Sets _iacc = default.
		/// </summary>
		internal void LibResetIacc()
		{
			_iacc = default;
		}

		/// <summary>
		/// Sets Counter = 0.
		/// </summary>
		internal void LibResetCounter()
		{
			Counter = 0;
		}
	}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member //FUTURE

	/// <summary>
	/// Accessible object ids of window parts and some special objects.
	/// Used with <see cref="Acc.FromWindow"/>
	/// </summary>
	/// <remarks>
	/// The names are as in API <msdn>AccessibleObjectFromWindow</msdn> documentation but without prefix "OBJID_".
	/// </remarks>
	public enum AccOBJID
	{
		WINDOW = 0,
		SYSMENU = -1,
		TITLEBAR = -2,
		MENU = -3,
		CLIENT = -4,
		VSCROLL = -5,
		HSCROLL = -6,
		SIZEGRIP = -7,
		CARET = -8,
		CURSOR = -9,
		ALERT = -10,
		SOUND = -11,
		QUERYCLASSNAMEIDX = -12,
		NATIVEOM = -16,
	}

	/// <summary>
	/// Standard roles of accessible objects.
	/// Used with <see cref="Acc.RoleEnum"/>
	/// </summary>
	/// <remarks>
	/// The names are as in API <msdn>IAccessible.get_accRole</msdn> documentation but without prefix "ROLE_SYSTEM_".
	/// </remarks>
	public enum AccROLE
	{
		TITLEBAR = 0x1,
		MENUBAR = 0x2,
		SCROLLBAR = 0x3,
		GRIP = 0x4,
		SOUND = 0x5,
		CURSOR = 0x6,
		CARET = 0x7,
		ALERT = 0x8,
		WINDOW = 0x9,
		CLIENT = 0xA,
		MENUPOPUP = 0xB,
		MENUITEM = 0xC,
		TOOLTIP = 0xD,
		APPLICATION = 0xE,
		DOCUMENT = 0xF,
		PANE = 0x10,
		CHART = 0x11,
		DIALOG = 0x12,
		BORDER = 0x13,
		GROUPING = 0x14,
		SEPARATOR = 0x15,
		TOOLBAR = 0x16,
		STATUSBAR = 0x17,
		TABLE = 0x18,
		COLUMNHEADER = 0x19,
		ROWHEADER = 0x1A,
		COLUMN = 0x1B,
		ROW = 0x1C,
		CELL = 0x1D,
		LINK = 0x1E,
		HELPBALLOON = 0x1F,
		CHARACTER = 0x20,
		LIST = 0x21,
		LISTITEM = 0x22,
		OUTLINE = 0x23,
		OUTLINEITEM = 0x24,
		PAGETAB = 0x25,
		PROPERTYPAGE = 0x26,
		INDICATOR = 0x27,
		GRAPHIC = 0x28,
		STATICTEXT = 0x29,
		TEXT = 0x2A,
		PUSHBUTTON = 0x2B,
		CHECKBUTTON = 0x2C,
		RADIOBUTTON = 0x2D,
		COMBOBOX = 0x2E,
		DROPLIST = 0x2F,
		PROGRESSBAR = 0x30,
		DIAL = 0x31,
		HOTKEYFIELD = 0x32,
		SLIDER = 0x33,
		SPINBUTTON = 0x34,
		DIAGRAM = 0x35,
		ANIMATION = 0x36,
		EQUATION = 0x37,
		BUTTONDROPDOWN = 0x38,
		BUTTONMENU = 0x39,
		BUTTONDROPDOWNGRID = 0x3A,
		WHITESPACE = 0x3B,
		PAGETABLIST = 0x3C,
		CLOCK = 0x3D,
		SPLITBUTTON = 0x3E,
		IPADDRESS = 0x3F,
		OUTLINEBUTTON = 0x40,
	}

	/// <summary>
	/// Accessible object state flags.
	/// Used by <see cref="Acc.State"/>.
	/// </summary>
	/// <remarks>
	/// The names are as in API <msdn>IAccessible.get_accState</msdn> documentation but without prefix "STATE_SYSTEM_".
	/// </remarks>
	[Flags]
	public enum AccSTATE
	{
		//NORMAL = 0x0,
		UNAVAILABLE = 0x1,
		SELECTED = 0x2,
		FOCUSED = 0x4,
		PRESSED = 0x8,
		CHECKED = 0x10,
		//MIXED = 0x20,
		INDETERMINATE = 0x20,
		READONLY = 0x40,
		HOTTRACKED = 0x80,
		DEFAULT = 0x100,
		EXPANDED = 0x200,
		COLLAPSED = 0x400,
		BUSY = 0x800,
		FLOATING = 0x1000,
		MARQUEED = 0x2000,
		ANIMATED = 0x4000,
		INVISIBLE = 0x8000,
		OFFSCREEN = 0x10000,
		SIZEABLE = 0x20000,
		MOVEABLE = 0x40000,
		SELFVOICING = 0x80000,
		FOCUSABLE = 0x100000,
		SELECTABLE = 0x200000,
		LINKED = 0x400000,
		TRAVERSED = 0x800000,
		MULTISELECTABLE = 0x1000000,
		EXTSELECTABLE = 0x2000000,
		ALERT_LOW = 0x4000000,
		ALERT_MEDIUM = 0x8000000,
		ALERT_HIGH = 0x10000000,
		PROTECTED = 0x20000000,
		HASPOPUP = 0x40000000,
	}

	/// <summary>
	/// Accessible object navigation direction.
	/// Used by <see cref="Acc.Navigate(AccNAVDIR, int, bool)"/>.
	/// </summary>
	/// <remarks>
	/// The names are as in API <msdn>IAccessible.accNavigate</msdn> documentation but without prefix "NAVDIR_".
	/// Many objects don't support NEXT, PREVIOUS, FIRSTCHILD, LASTCHILD. Some objects skip invisible siblings. When unavailable, functions of this library try a workaround, which works well with FIRSTCHILD and LASTCHILD, but with NEXT and PREVIOUS it is slow and not always succeeds. Instead you can use PARENT/CHILD.
	/// UP, DOWN, LEFT and RIGHT - spatial navigation in the same container object. Rarely supported or useful. String names - "#1", "#2", "#3", "#4".
	/// </remarks>
	public enum AccNAVDIR
	{
		UP = 1,
		DOWN = 2,
		LEFT = 3,
		RIGHT = 4,

		/// <summary>
		/// Next sibling object in the same parent (container) object.
		/// String name - "next", "ne" or "n".
		/// </summary>
		NEXT = 5,

		/// <summary>
		/// Previous sibling object in the same parent (container) object.
		/// String name - "previous", "pr" or "prev".
		/// </summary>
		PREVIOUS = 6,

		/// <summary>
		/// First direct child object.
		/// String name - "first", "fi" or "f".
		/// </summary>
		FIRSTCHILD = 7,

		/// <summary>
		/// Last direct child object.
		/// String name - "last", "la" or "l".
		/// </summary>
		LASTCHILD = 8,

		//ours

		/// <summary>
		/// The parent (container) object.
		/// String name - "parent", "pa" or "p".
		/// Few objects don't support it.
		/// Used only with functions of this library.
		/// </summary>
		PARENT = 9,

		/// <summary>
		/// A direct child object by 1-based index.
		/// String name - "child", "ch" or "c".
		/// Negative index means from end, for example -1 is the last child.
		/// Used only with functions of this library.
		/// </summary>
		CHILD = 10,
	}

	/// <summary>
	/// Accessible object selection flags.
	/// Used by <see cref="Acc.Select"/>.
	/// </summary>
	/// <remarks>
	/// The names are as in API <msdn>IAccessible.accSelect</msdn> documentation but without prefix "SELFLAG_".
	/// </remarks>
	[Flags]
	public enum AccSELFLAG
	{
		TAKEFOCUS = 0x1,
		TAKESELECTION = 0x2,
		EXTENDSELECTION = 0x4,
		ADDSELECTION = 0x8,
		REMOVESELECTION = 0x10,
	}

	/// <summary>
	/// Event constants for API <msdn>SetWinEventHook</msdn>.
	/// </summary>
	/// <remarks>
	/// The names are as in API documentation but without prefix "EVENT_".
	/// </remarks>
	public enum AccEVENT
	{
		MIN = 0x1,
		MAX = 0x7FFFFFFF,
		SYSTEM_SOUND = 0x1,
		SYSTEM_ALERT = 0x2,
		SYSTEM_FOREGROUND = 0x3,
		SYSTEM_MENUSTART = 0x4,
		SYSTEM_MENUEND = 0x5,
		SYSTEM_MENUPOPUPSTART = 0x6,
		SYSTEM_MENUPOPUPEND = 0x7,
		SYSTEM_CAPTURESTART = 0x8,
		SYSTEM_CAPTUREEND = 0x9,
		SYSTEM_MOVESIZESTART = 0xA,
		SYSTEM_MOVESIZEEND = 0xB,
		SYSTEM_CONTEXTHELPSTART = 0xC,
		SYSTEM_CONTEXTHELPEND = 0xD,
		SYSTEM_DRAGDROPSTART = 0xE,
		SYSTEM_DRAGDROPEND = 0xF,
		SYSTEM_DIALOGSTART = 0x10,
		SYSTEM_DIALOGEND = 0x11,
		SYSTEM_SCROLLINGSTART = 0x12,
		SYSTEM_SCROLLINGEND = 0x13,
		SYSTEM_SWITCHSTART = 0x14,
		SYSTEM_SWITCHEND = 0x15,
		SYSTEM_MINIMIZESTART = 0x16,
		SYSTEM_MINIMIZEEND = 0x17,
		SYSTEM_DESKTOPSWITCH = 0x20,
		SYSTEM_SWITCHER_APPGRABBED = 0x24,
		SYSTEM_SWITCHER_APPOVERTARGET = 0x25,
		SYSTEM_SWITCHER_APPDROPPED = 0x26,
		SYSTEM_SWITCHER_CANCELLED = 0x27,
		SYSTEM_IME_KEY_NOTIFICATION = 0x29,
		SYSTEM_END = 0xFF,
		OEM_DEFINED_START = 0x101,
		OEM_DEFINED_END = 0x1FF,
		UIA_EVENTID_START = 0x4E00,
		UIA_EVENTID_END = 0x4EFF,
		UIA_PROPID_START = 0x7500,
		UIA_PROPID_END = 0x75FF,
		CONSOLE_CARET = 0x4001,
		CONSOLE_UPDATE_REGION = 0x4002,
		CONSOLE_UPDATE_SIMPLE = 0x4003,
		CONSOLE_UPDATE_SCROLL = 0x4004,
		CONSOLE_LAYOUT = 0x4005,
		CONSOLE_START_APPLICATION = 0x4006,
		CONSOLE_END_APPLICATION = 0x4007,
		CONSOLE_END = 0x40FF,
		OBJECT_CREATE = 0x8000,
		OBJECT_DESTROY = 0x8001,
		OBJECT_SHOW = 0x8002,
		OBJECT_HIDE = 0x8003,
		OBJECT_REORDER = 0x8004,
		OBJECT_FOCUS = 0x8005,
		OBJECT_SELECTION = 0x8006,
		OBJECT_SELECTIONADD = 0x8007,
		OBJECT_SELECTIONREMOVE = 0x8008,
		OBJECT_SELECTIONWITHIN = 0x8009,
		OBJECT_STATECHANGE = 0x800A,
		OBJECT_LOCATIONCHANGE = 0x800B,
		OBJECT_NAMECHANGE = 0x800C,
		OBJECT_DESCRIPTIONCHANGE = 0x800D,
		OBJECT_VALUECHANGE = 0x800E,
		OBJECT_PARENTCHANGE = 0x800F,
		OBJECT_HELPCHANGE = 0x8010,
		OBJECT_DEFACTIONCHANGE = 0x8011,
		OBJECT_ACCELERATORCHANGE = 0x8012,
		OBJECT_INVOKED = 0x8013,
		OBJECT_TEXTSELECTIONCHANGED = 0x8014,
		OBJECT_CONTENTSCROLLED = 0x8015,
		SYSTEM_ARRANGMENTPREVIEW = 0x8016,
		OBJECT_CLOAKED = 0x8017,
		OBJECT_UNCLOAKED = 0x8018,
		OBJECT_LIVEREGIONCHANGED = 0x8019,
		OBJECT_HOSTEDOBJECTSINVALIDATED = 0x8020,
		OBJECT_DRAGSTART = 0x8021,
		OBJECT_DRAGCANCEL = 0x8022,
		OBJECT_DRAGCOMPLETE = 0x8023,
		OBJECT_DRAGENTER = 0x8024,
		OBJECT_DRAGLEAVE = 0x8025,
		OBJECT_DRAGDROPPED = 0x8026,
		OBJECT_IME_SHOW = 0x8027,
		OBJECT_IME_HIDE = 0x8028,
		OBJECT_IME_CHANGE = 0x8029,
		OBJECT_TEXTEDIT_CONVERSIONTARGETCHANGED = 0x8030,
		OBJECT_END = 0x80FF,
		AIA_START = 0xA000,
		AIA_END = 0xAFFF,
	}

	/// <summary>
	/// Flags for API <msdn>SetWinEventHook</msdn>.
	/// </summary>
	/// <remarks>
	/// The names are as in API documentation but without prefix "WINEVENT_".
	/// There are no flags for OUTOFCONTEXT and INCONTEXT. OUTOFCONTEXT is default (0). INCONTEXT cannot be used in managed code.
	/// </remarks>
	[Flags]
	public enum AHFlags
	{
		//OUTOFCONTEXT = 0x0,
		SKIPOWNTHREAD = 0x1,
		SKIPOWNPROCESS = 0x2,
		//INCONTEXT = 0x4,
	}

	//currently not used
	//public enum AccBrowser
	//{
	//	Unknown,
	//	Firefox,
	//	Chrome,
	//	InternetExplorer,
	//}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
