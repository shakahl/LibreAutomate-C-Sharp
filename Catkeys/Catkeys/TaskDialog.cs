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

#pragma warning disable 649 //unused fields in API structs

namespace Catkeys
{
	/// <summary>
	/// Standard dialogs to show information to the user or get input from the user.
	/// Uses Windows API function TaskDialogIndirect. You can find more info in MSDN.
	/// You can use static functions (less code) or create class instances (more options).
	/// More info: <see cref="TaskDialog.ShowEx"/>.
	/// </summary>
	/// <example>
	/// This example creates a class instance, sets properties, shows dialog and uses events.
	/// <code>
	/// var d = new TaskDialog(); //info: also there is constructor that has TaskDialog.ShowEx() parameters
	/// d.SetText("Main text.", "More text.\nSupports &lt;A HREF=\"link data\"&gt;links&lt;/A&gt; if you subscribe to HyperlinkClicked event.");
	/// d.SetButtons(TDButtons.OKCancel | TDButtons.No);
	/// d.SetIcon(TDIcon.Warning);
	/// d.SetExpandedText("Expanded info\nand more info.", true);
	/// d.FlagCanBeMinimized = true;
	/// d.SetCustomButtons("1 one|2 two\nzzz", true);
	/// d.SetRadioButtons("1001 r1|1002 r2");
	/// d.SetTimeout(30, "OK");
	/// d.HyperlinkClicked += e =&gt; { TaskDialog.Show("link clicked", e.LinkHref, ownerWindow: e.hwnd); };
	/// d.ButtonClicked += e =&gt; { Print(e.ButtonName); if(e.Button == TDResult.No) e.DoNotCloseDialog = true; };
	/// d.FlagShowProgressBar = true; d.Timer += e =&gt; { e.dialog.Send.Progress(e.TimerTimeMS / 100); };
	/// var r = d.ShowDialog();
	/// switch(r.Button) { case TDResult.OK: Print("OK"); break; case 1: Print(1); break; }
	/// Print(r.RadioButton);
	/// Print(r.IsChecked);
	/// </code></example>
	//[DebuggerStepThrough]
	public partial class TaskDialog
	{
		//This part of the class - low-level instance methods.
		//Other parts (below in this file) - static methods that use an instance.

		#region API
		#region public API
#pragma warning disable 1591 //missing XML documentation
		/// <summary>
		/// Constants for TaskDialog API messages etc.
		/// </summary>
		/// <tocexclude />
		public static class TDApi
		{
			/// <summary>
			/// Messages that your event handler can send to the dialog.
			/// Reference: MSDN -> "Task Dialog Messages".
			/// </summary>
			/// <tocexclude />
			public enum TDM :uint
			{
				NAVIGATE_PAGE = WM_USER + 101,
				CLICK_BUTTON = WM_USER + 102, // wParam = button id
				SET_MARQUEE_PROGRESS_BAR = WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
				SET_PROGRESS_BAR_STATE = WM_USER + 104, // wParam = new progress state (0, 1 or 2)
				SET_PROGRESS_BAR_RANGE = WM_USER + 105, // lParam = Calc.MakeUint(min, max)
				SET_PROGRESS_BAR_POS = WM_USER + 106, // wParam = new position
				SET_PROGRESS_BAR_MARQUEE = WM_USER + 107, // wParam = 0 (stop marquee), wParam != 0 (start marquee), lParam = speed (milliseconds between repaints)
				SET_ELEMENT_TEXT = WM_USER + 108, // wParam = element (enum TDApi.TDE), lParam = new element text (string)
				CLICK_RADIO_BUTTON = WM_USER + 110, // wParam = radio button id
				ENABLE_BUTTON = WM_USER + 111, // wParam = button id, lParam = 0 (disable), lParam != 0 (enable)
				ENABLE_RADIO_BUTTON = WM_USER + 112, // wParam = radio button id, lParam = 0 (disable), lParam != 0 (enable)
				CLICK_VERIFICATION = WM_USER + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
				UPDATE_ELEMENT_TEXT = WM_USER + 114, // wParam = element (enum TDApi.TDE), lParam = new element text (string)
				SET_BUTTON_ELEVATION_REQUIRED_STATE = WM_USER + 115, // wParam = button id, lParam = 0 (elevation not required), lParam != 0 (elevation required)
				UPDATE_ICON = WM_USER + 116  // wParam = icon element (enum TDApi.TDIE), lParam = new icon (icon handle or TDIcon)
			}
			const uint WM_USER = Api.WM_USER;

			/// <summary>
			/// Notification messages that your event handler receives.
			/// Reference: MSDN -> "Task Dialog Notifications".
			/// </summary>
			/// <tocexclude />
			public enum TDN :uint
			{
				CREATED = 0,
				NAVIGATED = 1,
				BUTTON_CLICKED = 2,
				HYPERLINK_CLICKED = 3,
				TIMER = 4,
				DESTROYED = 5,
				RADIO_BUTTON_CLICKED = 6,
				DIALOG_CONSTRUCTED = 7,
				VERIFICATION_CLICKED = 8,
				HELP = 9,
				EXPANDO_BUTTON_CLICKED = 10
			}

			/// <summary>
			/// Constants for TaskDialog.TDApi.TDM.SET_ELEMENT_TEXT and TaskDialog.TDApi.TDM.UPDATE_ELEMENT_TEXT messages and TaskDialog.Send.Text().
			/// </summary>
			/// <tocexclude />
			public enum TDE
			{
				CONTENT,
				EXPANDED_INFORMATION,
				FOOTER,
				MAIN_INSTRUCTION
			}

			/// <summary>
			/// Constants for TaskDialog.TDApi.TDM.UPDATE_ICON message.
			/// </summary>
			/// <tocexclude />
			public enum TDIE
			{
				ICON_MAIN,
				ICON_FOOTER
			}
		}

#pragma warning restore 1591 //missing XML documentation
		#endregion public API
		#region private API

		[DllImport("comctl32.dll")]
		static extern int TaskDialogIndirect([In] ref TASKDIALOGCONFIG c, out int pnButton, out int pnRadioButton, out int pChecked);

		//TASKDIALOGCONFIG flags.
		[Flags]
		enum TDF_
		{
			ENABLE_HYPERLINKS = 0x0001,
			USE_HICON_MAIN = 0x0002,
			USE_HICON_FOOTER = 0x0004,
			ALLOW_DIALOG_CANCELLATION = 0x0008,
			USE_COMMAND_LINKS = 0x0010,
			USE_COMMAND_LINKS_NO_ICON = 0x0020,
			EXPAND_FOOTER_AREA = 0x0040,
			EXPANDED_BY_DEFAULT = 0x0080,
			VERIFICATION_FLAG_CHECKED = 0x0100,
			SHOW_PROGRESS_BAR = 0x0200,
			SHOW_MARQUEE_PROGRESS_BAR = 0x0400,
			CALLBACK_TIMER = 0x0800,
			POSITION_RELATIVE_TO_WINDOW = 0x1000,
			RTL_LAYOUT = 0x2000,
			NO_DEFAULT_RADIO_BUTTON = 0x4000,
			CAN_BE_MINIMIZED = 0x8000,
			TDF_SIZE_TO_CONTENT = 0x1000000, //possibly added later than in Vista, don't know when
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct TASKDIALOG_BUTTON
		{
			public int id;
			public string text;
		}

		delegate int TaskDialogCallbackProc(Wnd hwnd, TDApi.TDN notification, LPARAM wParam, LPARAM lParam, IntPtr data);

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct TASKDIALOGCONFIG
		{
			public uint cbSize;
			public Wnd hwndParent;
			public IntPtr hInstance;
			public TDF_ dwFlags;
			public TDButtons dwCommonButtons;
			public string pszWindowTitle;
			public IntPtr hMainIcon;
			public string pszMainInstruction;
			public string pszContent;
			public int cButtons;
			public IntPtr pButtons;
			public int nDefaultButton;
			public int cRadioButtons;
			public IntPtr pRadioButtons;
			public int nDefaultRadioButton;
			public string pszVerificationText;
			public string pszExpandedInformation;
			public string pszExpandedControlText;
			public string pszCollapsedControlText;
			public IntPtr hFooterIcon;
			public string pszFooter;
			public TaskDialogCallbackProc pfCallback;
			public IntPtr lpCallbackData;
			public int cxWidth;
		}

		#endregion private API
		#endregion API

		#region static options

		/// <summary>
		/// Default options used by TaskDialog class functions called in this app domain.
		/// </summary>
		public static class Options
		{
			/// <summary>
			/// Default title bar text.
			/// Default value - current appdomain name. In exe it is exe file name like "example.exe".
			/// </summary>
			public static string DefaultTitle
			{
				get { return _defaultTitle ?? AppDomain.CurrentDomain.FriendlyName; }
				set { _defaultTitle = value; }
			}
			static string _defaultTitle;
			//consider: use [assembly: AssemblyTitle("...")]. var a=Assembly.GetEntryAssembly(); But exception if appdomain runs with DoCallBack().

			/// <summary>
			/// Right-to-left layout.
			/// </summary>
			/// <seealso cref="FlagRtlLayout"/>
			public static bool RtlLayout { get; set; }

			/// <summary>
			/// If owner window not specified, let the dialog be always on top of most other windows.
			/// </summary>
			/// <seealso cref="FlagTopmost"/>
			public static bool TopmostIfNoOwner { get; set; }

			/// <summary>
			/// Show dialog in this screen if owner window not specified and screen not specified.
			/// More info:  <see cref="Screen_.FromObject"/>, <see cref="Wnd.MoveInScreen"/>.
			/// </summary>
			/// <seealso cref="Screen"/>
			public static object ScreenIfNoOwner { get; set; }

			/// <summary>
			/// If icon not specified, use <see cref="TDIcon.App"/>.
			/// </summary>
			public static bool UseAppIcon { get; set; }
		}

		#endregion static options

		TASKDIALOGCONFIG _c;
		string[] _buttons, _radioButtons; //before showing dialog these will be marshaled to IntPtr

		///
		public TaskDialog()
		{
			_c.cbSize = Api.SizeOf(_c);
			FlagRtlLayout = Options.RtlLayout;
		}

		/// <summary>
		/// Initializes a new <see cref="TaskDialog"/> instance and sets commonly used properties.
		/// All parameters are the same as of <see cref="ShowEx"/>.
		/// </summary>
		public TaskDialog(
			string text1, object text2 = null,
			TDButtons buttons = 0, TDIcon icon = 0, TDFlags flags = 0, int defaultButton = 0,
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			object ownerWindow = null, int x = 0, int y = 0, int timeoutS = 0,
			string expandedText = null, string footerText = null, string title = null,
			Action<TDEventArgs> onLinkClick = null
			) : this()
		{
			FlagEndThread = flags.HasFlag(TDFlags.EndThread);
			if(flags.HasFlag(TDFlags.Topmost)) FlagTopmost = true; //else use Options.TopmostIfNoOwner if no owner
			if(flags.HasFlag(TDFlags.Wider)) Width = 500;

			SetText(text1, text2);
			SetButtons(buttons);
			SetIcon(icon);
			SetCustomButtons(customButtons, flags.HasFlag(TDFlags.CommandLinks));
			DefaultButton = defaultButton;
			SetRadioButtons(radioButtons);
			_SetCheckboxFromText(checkBox);
			SetOwnerWindow(ownerWindow, flags.HasFlag(TDFlags.OwnerCenter));
			SetXY(x, y, flags.HasFlag(TDFlags.RawXY));
			SetTimeout(timeoutS);
			SetExpandedText(expandedText, flags.HasFlag(TDFlags.ExpandDown));
			SetFooterText(footerText);
			SetTitleBarText(title);
			if(onLinkClick != null) HyperlinkClicked += onLinkClick;
		}

		#region set properties

		void _SetFlag(TDF_ flag, bool on)
		{
			if(on) _c.dwFlags |= flag; else _c.dwFlags &= ~flag;
		}

		bool _HasFlag(TDF_ flag)
		{
			return _c.dwFlags.HasFlag(flag);
		}

		/// <summary>
		/// Changes title bar text.
		/// If you don't call this method or text is null or "", the dialog uses ScriptOptions.DefaultTitle (default is appdomain name).
		/// </summary>
		public void SetTitleBarText(string text)
		{
			_c.pszWindowTitle = Empty(text) ? Options.DefaultTitle : text;
			//info: if "", API uses "ProcessName.exe".
		}

		/// <summary>
		/// Sets text.
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		public void SetText(string text1, object text2 = null)
		{
			_c.pszMainInstruction = text1;
			_c.pszContent = text2?.ToString();
		}

		/// <summary>
		/// Sets common icon.
		/// </summary>
		/// <param name="icon"><see cref="TDIcon"/>.</param>
		public void SetIcon(TDIcon icon)
		{
			_c.hMainIcon = (IntPtr)(int)icon;
			_SetFlag(TDF_.USE_HICON_MAIN, false);
		}
		/// <summary>
		/// Sets custom icon.
		/// </summary>
		/// <param name="icon">Icon handle (IntPtr) or object (System.Drawing.Icon).</param>
		public void SetIcon(object icon)
		{
			_c.hMainIcon = _IconHandleFromObject(icon);
			_SetFlag(TDF_.USE_HICON_MAIN, _c.hMainIcon != Zero);
			//tested: displays original-size 32 and 16 icons, but shrinks bigger icons to 32.
			//note: for App icon Show() will execute more code. The same for footer icon.
		}

		static IntPtr _IconHandleFromObject(object icon)
		{
			IntPtr hi = Zero;
			if(icon != null) {
				if(icon is IntPtr) hi = (IntPtr)icon;
				else if(icon is Icon) hi = ((Icon)icon).Handle;
			}
			return hi;
		}

		/// <summary>
		/// Sets common buttons.
		/// </summary>
		/// <param name="buttons"><see cref="TDButtons"/>.</param>
		public void SetButtons(TDButtons buttons)
		{
			_c.dwCommonButtons = buttons;
		}

		/// <summary>
		/// Adds custom buttons and sets button style.
		/// </summary>
		/// <param name="buttons">A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List&lt;string&gt;. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).</param>
		/// <param name="asCommandLinks">false - row of classic buttons; true - column of command-link buttons that can have multiline text.</param>
		/// <param name="noCommandLinkIcon">No arrow icon on command-link buttons.</param>
		public void SetCustomButtons(StringList buttons, bool asCommandLinks = false, bool noCommandLinkIcon = false)
		{
			_buttons = buttons;
			_SetFlag(TDF_.USE_COMMAND_LINKS, asCommandLinks && !noCommandLinkIcon);
			_SetFlag(TDF_.USE_COMMAND_LINKS_NO_ICON, asCommandLinks && noCommandLinkIcon);
		}

		/// <summary>
		/// Specifies which button id to return on Enter key.
		/// If not set or defaultButton is 0, auto-selects.
		/// The value can be one of common buttons (eg TDResult.Cancel) or a custom button id (eg 1).
		/// </summary>
		public int DefaultButton { set { _c.nDefaultButton = -value; } }
		//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().

		/// <summary>
		/// Adds radio (option) buttons.
		/// To get selected radio button id after closing the dialog, use the RadioButton property of the TDResult object returned by Show() or Result.
		/// </summary>
		/// <param name="buttons">A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List&lt;string&gt;.</param>
		/// <param name="defaultId">Check the radio button that has this id. If omitted or 0, checks the first. If -1, does not check.</param>
		public void SetRadioButtons(StringList buttons, int defaultId = 0)
		{
			_radioButtons = buttons;
			_c.nDefaultRadioButton = defaultId;
			_SetFlag(TDF_.NO_DEFAULT_RADIO_BUTTON, defaultId < 0);
		}

		/// <summary>
		/// Adds check box (if text is not null/empty).
		/// To get check box state after closing the dialog, use the IsChecked property of the TDResult object returned by Show() or Result.
		/// </summary>
		public void SetCheckbox(string text, bool check = false)
		{
			_c.pszVerificationText = text;
			_SetFlag(TDF_.VERIFICATION_FLAG_CHECKED, check);
		}

		//Parses "Text|check" etc and calls SetCheckbox.
		void _SetCheckboxFromText(string checkBox)
		{
			string text = null; bool check = false;
			if(!Empty(checkBox)) {
				string[] a = checkBox.Split_("|", 2);
				text = a[0];
				if(a.Length == 2) switch(a[1]) { case "true": case "check": case "checked": check = true; break; }
			}
			SetCheckbox(text, check);
		}

		/// <summary>
		/// Adds text that the user can show and hide.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="showInFooter">Show the text at the bottom of the dialog.</param>
		public void SetExpandedText(string text, bool showInFooter = false)
		{
			if(Empty(text)) { text = null; showInFooter = false; }
			_SetFlag(TDF_.EXPAND_FOOTER_AREA, showInFooter);
			_c.pszExpandedInformation = text;
		}

		/// <summary>
		/// Set properties of the control that shows and hides text added by SetExpandedText().
		/// </summary>
		/// <param name="defaultExpanded"></param>
		/// <param name="collapsedText"></param>
		/// <param name="expandedText"></param>
		public void SetExpandControl(bool defaultExpanded, string collapsedText = null, string expandedText = null)
		{
			_SetFlag(TDF_.EXPANDED_BY_DEFAULT, defaultExpanded);
			_c.pszCollapsedControlText = collapsedText;
			_c.pszExpandedControlText = expandedText;
		}

		/// <summary>
		/// Adds text and common icon at the bottom of the dialog.
		/// </summary>
		/// <param name="text">Text, optionally preceded by an icon character and |, like "i|Text". Icons: x error, ! warning, i info, v shield, a app.</param>
		public void SetFooterText(string text)
		{
			TDIcon icon = 0;
			if(text != null && text.Length >= 2 && text[1] == '|') {
				switch(text[0]) {
				case 'x': icon = TDIcon.Error; break;
				case '!': icon = TDIcon.Warning; break;
				case 'i': icon = TDIcon.Info; break;
				case 'v': icon = TDIcon.Shield; break;
				case 'a': icon = TDIcon.App; break;
				}
				text = text.Substring(2);
			}
			SetFooterText(text, icon);
		}
		/// <summary>
		/// Adds text and common icon at the bottom of the dialog.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon"><see cref="TDIcon"/>.</param>
		public void SetFooterText(string text, TDIcon icon)
		{
			_c.pszFooter = text;
			_c.hFooterIcon = (IntPtr)(int)icon;
			_SetFlag(TDF_.USE_HICON_FOOTER, false);
		}
		/// <summary>
		/// Adds text and custom icon at the bottom of the dialog.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">Icon handle (IntPtr) or object (System.Drawing.Icon).</param>
		public void SetFooterText(string text, object icon)
		{
			_c.pszFooter = text;
			_c.hFooterIcon = _IconHandleFromObject(icon);
			_SetFlag(TDF_.USE_HICON_FOOTER, _c.hFooterIcon != Zero);
		}

		/// <summary>
		/// Adds Edit or Combo control (if editType is not TDEdit.None (0)).
		/// To get its text after closing the dialog, use the EditText property of the TDResult object returned by Show() or Result.
		/// </summary>
		/// <param name="editType">Control type/style.</param>
		/// <param name="initText">Initial text.</param>
		/// <param name="comboItems">Combo box items, like "one|two|three". Also can be string[] or List&lt;string&gt;.</param>
		/// <remarks>
		/// The API TaskDialogIndirect does not have an option to add an edit control. This class itself creates it.
		/// Does not support progress bar.
		/// </remarks>
		public void SetEditControl(TDEdit editType, string initText = null, StringList comboItems = null)
		{
			_editType = editType;
			_editText = initText;
			_editComboItems = comboItems;
			//will set other props later, because need to override user-set props
		}
		TDEdit _editType; string _editText; StringList _editComboItems;

		/// <summary>
		/// Sets the width of the dialog's client area.
		/// The actual width will depend on DPI (the Windows "text size" setting).
		/// If less than default width, will be used default width.
		/// </summary>
		public int Width { set { _c.cxWidth = value/2; } }
		//tested: TDIF_SIZE_TO_CONTENT can make wider, but not much. The flag is obsolete.

		/// <summary>
		/// Sets owner window.
		/// The owner window will be disabled, and this dialog will be on top of it.
		/// This window will be in owner's screen, if screen was not explicitly specified with the Screen property. TaskDialog.Options.ScreenIfNoOwner is ignored.
		/// <param name="ownerWindow">Owner window. Can be Form, Control, IWin32Window, Wnd, IntPtr.</param>
		/// <param name="ownerCenter">Show the dialog in the center of the owner window. SetXY() and the Screen property are ignored.</param>
		/// <param name="enableOwner">Don't disable the owner window. If false, disables if it belongs to this thread.</param>
		/// </summary>
		public void SetOwnerWindow(object ownerWindow, bool ownerCenter = false, bool enableOwner = false)
		{
			if(ownerWindow != null) __SetOwner(ownerWindow);
			_SetFlag(TDF_.POSITION_RELATIVE_TO_WINDOW, ownerCenter);
			_enableOwner = enableOwner;
		}
		bool _enableOwner;
		[MethodImpl(MethodImplOptions.NoInlining)] //this func used to avoid loading System.Windows.Forms.dll when ownerWindow is null
		void __SetOwner(object o)
		{
			Wnd w = Wnd0;
			var iw = o as IWin32Window;
			if(iw != null) w = (Wnd)iw.Handle;
			else if(o is Wnd) w = (Wnd)o;
			else if(o is IntPtr) w = (Wnd)(IntPtr)o;
			_c.hwndParent = w.ToplevelParentOrThis;
		}

		/// <summary>
		/// Sets dialog position in screen.
		/// </summary>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="rawXY">Don't interpret 0 and negative x y in a special way.</param>
		public void SetXY(int x, int y, bool rawXY = false)
		{
			_x = x; _y = y;
			_rawXY = rawXY;
		}

		int _x, _y; bool _rawXY;

		/// <summary>
		/// Sets the screen (display monitor) where to show the dialog in multi-screen environment.
		/// If null or not set, will be used owner window's screen or TaskDialog.Options.ScreenIfNoOwner.
		/// More info: <see cref="Screen_.FromObject"/>, <see cref="Wnd.MoveInScreen"/>.
		/// </summary>
		public object Screen { set; private get; }

		/// <summary>
		/// Let the dialog close itself after closeAfterS seconds.
		/// On timeout Show() returns TDResult.Timeout.
		/// <para>Example: <c>d.SetTimeout(30, "OK");</c></para>
		/// </summary>
		public void SetTimeout(int closeAfterS, string timeoutActionText = null, bool noInfo = false)
		{
			_timeoutS = closeAfterS;
			_timeoutActionText = timeoutActionText;
			_timeoutNoInfo = noInfo;
		}
		int _timeoutS; bool _timeoutActive, _timeoutNoInfo; string _timeoutActionText, _timeoutFooterText;

		/// <summary>
		/// Add 'Close' button to the title bar even if there is no 'Cancel' button.
		/// </summary>
		public bool FlagAllowCancel { set; private get; }

		/// <summary>
		/// Right-to left layout.
		/// Default is TaskDialog.Options.RtlLayout.
		/// </summary>
		public bool FlagRtlLayout { set; private get; }

		/// <summary>
		/// Add 'Minimize' button to the title bar.
		/// </summary>
		public bool FlagCanBeMinimized { set; private get; }

		/// <summary>
		/// Show progress bar.
		/// </summary>
		public bool FlagShowProgressBar { set; private get; }

		/// <summary>
		/// Show progress bar that just plays an animation but does not indicate which part of the work is already done.
		/// </summary>
		public bool FlagShowMarqueeProgressBar { set; private get; }

		/// <summary>
		/// Makes the dialog window topmost or non-topmost.
		/// If true, will set topmost style when creating the dialog. If false, will not set.
		/// If null (default), the dialog will be topmost if both these are true: no owner window, TaskDialog.Options.TopmostIfNoOwner is true.
		/// </summary>
		public bool? FlagTopmost { set; private get; }

		/// <summary>
		/// Call Thread.Abort() if clicked OK when there are no other buttons. Also when clicked Cancel, No, and on timeout.
		/// </summary>
		public bool FlagEndThread { set; private get; }

		#endregion set properties

		Wnd _dlg;
		int _threadIdInShow;

		/// <summary>
		/// Shows the dialog.
		/// The return value is the same as of <see cref="ShowEx"/>.
		/// Call this method after setting text and other properties.
		/// </summary>
		public TDResult ShowDialog()
		{
			//info: named ShowDialog, not Show, to not confuse with the static Show() which is used almost everywhere in documentation.

			_result = null;
			_isClosed = false;

			SetTitleBarText(_c.pszWindowTitle); //if not set, sets default
			_EditControlInitBeforeShowDialog(); //don't reorder, must be before flags

			_SetFlag(TDF_.ALLOW_DIALOG_CANCELLATION, FlagAllowCancel);
			_SetFlag(TDF_.RTL_LAYOUT, FlagRtlLayout);
			_SetFlag(TDF_.CAN_BE_MINIMIZED, FlagCanBeMinimized);
			_SetFlag(TDF_.SHOW_PROGRESS_BAR, FlagShowProgressBar);
			_SetFlag(TDF_.SHOW_MARQUEE_PROGRESS_BAR, FlagShowMarqueeProgressBar);
			_SetFlag(TDF_.ENABLE_HYPERLINKS, HyperlinkClicked != null);
			_SetFlag(TDF_.CALLBACK_TIMER, (_timeoutS > 0 || Timer != null));

			_timeoutActive = false;
			if(_timeoutS > 0) {
				_timeoutActive = true;
				if(!_timeoutNoInfo) {
					_timeoutFooterText = _c.pszFooter;
					_c.pszFooter = _TimeoutFooterText(_timeoutS);
					if(_c.hFooterIcon == Zero) _c.hFooterIcon = (IntPtr)TDIcon.Info;
				}
			}

			if(_c.hMainIcon == Zero && Options.UseAppIcon) SetIcon(TDIcon.App);
			if(_c.hMainIcon == (IntPtr)TDIcon.App || _c.hFooterIcon == (IntPtr)TDIcon.App) {
				if(Icons.GetAppIconHandle(32) != Zero) _c.hInstance = Util.ModuleHandle.OfAppDomainEntryAssembly();
				else if(Icons.GetProcessExeIconHandle(32) != Zero) _c.hInstance = Util.ModuleHandle.OfProcessExe();
			}
			//info: TDIcon.App is IDI_APPLICATION (32512).
			//Although MSDN does not mention that IDI_APPLICATION can be used when hInstance is NULL, it works. Even works for many other undocumented system resource ids, eg 100.
			//Non-NULL hInstance is ignored for the icons specified as TD_x. It is documented and logical.
			//For App icon we could instead use icon handle, but then the small icon for the title bar and taskbar button can be distorted because shrinked from the big icon. Now extracts small icon from resources.

			_c.pfCallback = _CallbackProc;

			IntPtr hhook = Zero; Api.HOOKPROC hpHolder = null;
			int rButton = 0, rRadioButton = 0, rIsChecked = 0, hr = 0;

			try {
				_threadIdInShow = Thread.CurrentThread.ManagedThreadId;

				_c.pButtons = _MarshalButtons(_buttons, out _c.cButtons, true);
				if(_c.pButtons == Zero) _SetFlag(TDF_.USE_COMMAND_LINKS | TDF_.USE_COMMAND_LINKS_NO_ICON, false); //to avoid exception
				_c.pRadioButtons = _MarshalButtons(_radioButtons, out _c.cRadioButtons);

				if(_timeoutActive) { //Need mouse/key messages to stop countdown on click or key.
					hhook = Api.SetWindowsHookEx(Api.WH_GETMESSAGE, hpHolder = _HookProc, Zero, Api.GetCurrentThreadId());
				}

				Wnd.AllowActivate(true);

				for(int i = 0; i < 100; i++) { //see the API bug-workaround comment below
#if DEBUG
					hr = _CallTDI(ref _c, out rButton, out rRadioButton, out rIsChecked);
#else
					hr = TaskDialogIndirect(ref _c, out rButton, out rRadioButton, out rIsChecked);
#endif
					//TaskDialog[Indirect] API bug: if called simultaneously by 2 threads, fails and returns an unknown error code 0x800403E9.
					//Workaround: retry. Also retry for other errors, eg E_OUTOFMEMORY and all other unexpected errors.
					//In all my tests with 2 threads, was enough 1 retry, but in real life may be more threads.
					if(hr == 0 || hr == Api.E_INVALIDARG || !_dlg.Is0) break; //about _dlg.Is0: _dlg is set if our callback function was called; then don't retry, because the dialog was possibly shown, and only then error.
					WaitMS(20); //100*20=2000
				}

				if(hr == 0) {
					//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
					rButton = -rButton;
					_result = new TDResult(rButton, rRadioButton, rIsChecked != 0, _editText);

					Wnd.Misc.WaitForAnActiveWindow();
				}
			}
			finally {
				_threadIdInShow = 0;

				if(hhook != Zero) Api.UnhookWindowsHookEx(hhook);

				_MarshalFreeButtons(ref _c.pButtons, ref _c.cButtons);
				_MarshalFreeButtons(ref _c.pRadioButtons, ref _c.cRadioButtons);

				_SetClosed();
			}

			if(hr != 0) throw new Win32Exception(hr);

			if(FlagEndThread) {
				bool endThread = false;
				switch(rButton) {
				case TDResult.Cancel: case TDResult.No: case TDResult.Timeout: endThread = true; break;
				case TDResult.OK: endThread = (_c.dwCommonButtons == 0 || _c.dwCommonButtons == TDButtons.OK) && _c.pButtons == Zero; break;
				}

				if(endThread) Thread.CurrentThread.Abort();
			}

			return _result;
		}

		int _CallbackProc(Wnd w, TDApi.TDN message, LPARAM wParam, LPARAM lParam, IntPtr data)
		{
			Action<TDEventArgs> e = null;
			int R = 0;

			//Print(message);
			switch(message) {
			case TDApi.TDN.DIALOG_CONSTRUCTED:
				Send = new TDMessageSender(w, this); //note: must be before setting _dlg, because another thread may call if(d.IsOpen) d.Send.Message(..).
				_dlg = w;
				break;
			case TDApi.TDN.DESTROYED:
				//Print(w.IsValid); //valid
				e = Destroyed;
				break;
			case TDApi.TDN.CREATED:
				if(_enableOwner || !_c.hwndParent.IsOfThisThread) _c.hwndParent.Enable(true);

				if(!_HasFlag(TDF_.POSITION_RELATIVE_TO_WINDOW)) {
					object scr = Screen; if(scr == null && _c.hwndParent.Is0) scr = Options.ScreenIfNoOwner;
					if((_x != 0 || _y != 0 || _rawXY || scr != null)) {
						w.MoveInScreen(_x, _y, scr, rawXY: _rawXY);
					}
				}

				bool topmost = false;
				if(FlagTopmost != null) topmost = FlagTopmost.Value; else if(_c.hwndParent.Is0) topmost = Options.TopmostIfNoOwner;
				if(topmost) w.ZorderTopmost();

				//w.SetStyleAdd(Api.WS_THICKFRAME); //does not work

				if(_IsEdit) _EditControlCreate();

				e = Created;
				break;
			case TDApi.TDN.TIMER:
				if(_timeoutActive) {
					int timeElapsed = wParam / 1000;
					if(timeElapsed < _timeoutS) {
						if(!_timeoutNoInfo) Send.ChangeFooterText(_TimeoutFooterText(_timeoutS - timeElapsed - 1), false);
					} else {
						_timeoutActive = false;
						Send.Close(TDResult.Timeout);
					}
				}

				e = Timer;
				break;
			case TDApi.TDN.BUTTON_CLICKED: e = ButtonClicked; wParam = -wParam; break; //info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
			case TDApi.TDN.HYPERLINK_CLICKED: e = HyperlinkClicked; break;
			case TDApi.TDN.HELP: e = HelpF1; break;
			default: e = OtherEvents; break;
			}

			if(_IsEdit) _EditControlOnMessage(message);

			if(e != null) {
				var ed = new TDEventArgs(this, _dlg, message, wParam, lParam);
				e(ed);
				R = ed.returnValue;
			}

			if(message == TDApi.TDN.DESTROYED) _SetClosed();

			return R;
		}

		/// <summary>
		/// TaskDialog events.
		/// Raised when the internal TaskDialogCallbackProc function (see in MSDN) is called by the task dialog API.
		/// </summary>
		public event Action<TDEventArgs>
			Created, Destroyed, Timer, ButtonClicked, HyperlinkClicked, HelpF1, OtherEvents;

		/// <summary>
		/// Arguments for TaskDialog event handlers.
		/// More info: MSDN -> TaskDialogCallbackProc function.
		/// To return a non-zero value from the callback function, assign the value to the returnValue field.
		/// </summary>
		public class TDEventArgs :EventArgs
		{
			internal TDEventArgs(TaskDialog obj_, Wnd hwnd_, TDApi.TDN message_, LPARAM wParam_, LPARAM lParam_)
			{
				dialog = obj_; hwnd = hwnd_; message = message_; wParam = wParam_;
				LinkHref = (message_ == TDApi.TDN.HYPERLINK_CLICKED) ? Marshal.PtrToStringUni(lParam_) : null;
			}

#pragma warning disable 1591 //missing XML documentation
			public TaskDialog dialog;
			public Wnd hwnd;
			public TDApi.TDN message;
			public LPARAM wParam;
			public int returnValue;
#pragma warning restore 1591 //missing XML documentation

			/// <summary>
			/// Clicked hyperlink href attribute value. Use in HyperlinkClicked event handler.
			/// </summary>
			public string LinkHref { get; private set; }

			/// <summary>
			/// Clicked button id. Use in ButtonClicked event handler.
			/// </summary>
			public int Button { get { return wParam; } }

			/// <summary>
			/// Clicked button English name or custom button id string. Use in ButtonClicked event handler.
			/// </summary>
			public string ButtonName { get { return TDResult.ButtonIdToName(wParam); } }

			/// <summary>
			/// Dialog timer time in milliseconds. Use in Timer event handler.
			/// The event handler can set returnValue=1 to reset this.
			/// </summary>
			public int TimerTimeMS { get { return wParam; } }

			/// <summary>
			/// Your ButtonClicked event handler function can use this to prevent closing the dialog.
			/// </summary>
			public bool DoNotCloseDialog { set { returnValue = value ? 1 : 0; } }

			/// <summary>
			/// Gets or sets edit field text.
			/// </summary>
			public string EditText
			{
				get { return dialog.EditControl.GetControlText(); }
				set { dialog.EditControl.SetControlText(value); }
			}
		}

		#region async etc

		/// <summary>
		/// Shows the dialog in new thread and returns without waiting until it is closed.
		/// </summary>
		/// <param name="whenClosed">
		/// A callback function (lambda etc) to call when the dialog closed. It receives TDResult.
		/// In what thread it is called: if owner window is specified and it is a Form or Control of this AppDomain and it still exists at that time, calls in its thread; else calls in dialog's thread, which is not this thread.
		/// </param>
		/// <param name="closeOnExit">If true, the dialog disappears when all other threads of this app domain end.</param>
		/// <remarks>Calls <see cref="ThreadWaitOpen"/>, therefore the dialog is already open when this function returns.</remarks>
		public void ShowNoWait(Action<TDResult> whenClosed = null, bool closeOnExit = false)
		{
			//note: not Task.Run, because we need STA, etc.
			var t = new Thread(() =>
			  {
				  ShowDialog();
				  if(whenClosed != null) {
					  Control c = null;
					  try {
						  var w = _c.hwndParent;
						  if(w.IsValid && (c = (Control)w) != null) c.BeginInvoke(whenClosed, Result);
					  }
					  catch { c = null; }
					  if(c == null) whenClosed.Invoke(Result);
				  }
				  //note: currently onClose not called if main thread is already returned at that time.
				  //	Because then called AppDomain.Unload, which aborts thread, which aborts executing managed code.
				  //	In the future before calling AppDomain.Unload should close all thread windows, or wait for windows marked 'wait for it on script exit'.
			  });
			t.SetApartmentState(ApartmentState.STA);
			t.IsBackground = closeOnExit;
			t.Start();
			if(!ThreadWaitOpen()) throw new CatException();
		}

		/// <summary>
		/// Gets clicked button id and other results packed in a TDResult object.
		/// It is the same object as the Show() return value.
		/// If the result is still unavailable (eg the dialog still not closed):
		///		If called from the same thread that called Show(), returns null.
		///		If called from another thread, waits until the dialog is closed and the return value is available.
		///		Note that ShowNoWait() calls Show() in another thread.
		/// </summary>
		public TDResult Result
		{
			get
			{
				if(!_WaitWhileInShow()) return null;
				return _result;
			}
		}
		TDResult _result;

		bool _WaitWhileInShow()
		{
			if(_threadIdInShow != 0) {
				if(_threadIdInShow == Thread.CurrentThread.ManagedThreadId) return false;
				while(_threadIdInShow != 0) WaitMS(20);
			}
			return true;
		}

		/// <summary>
		/// Can be used by other threads to wait until the dialog is open.
		/// If returns true, the dialog is open and you can send messages to it.
		/// If returns false, the dialog is already closed or failed to show.
		/// </summary>
		public bool ThreadWaitOpen()
		{
			_AssertIsOtherThread();
			while(!IsOpen) {
				if(_isClosed) return false;
				WaitMS(10); //need 3-4 loops if 10
				Time.DoEvents(); //without it this func hangs if a form is the dialog owner
			}
			return true;
		}

		/// <summary>
		/// Can be used by other threads to wait until the dialog is closed.
		/// </summary>
		public void ThreadWaitClosed()
		{
			_AssertIsOtherThread();
			while(!_isClosed) {
				WaitMS(50);
			}
			_WaitWhileInShow();
		}

		void _AssertIsOtherThread()
		{
			if(_threadIdInShow != 0 && _threadIdInShow == Thread.CurrentThread.ManagedThreadId)
				throw new CatException("wrong thread");
		}

		/// <summary>
		/// Returns true if the dialog is open and your code can send messages to it.
		/// </summary>
		public bool IsOpen { get { return !_dlg.Is0; } }

		void _SetClosed()
		{
			_isClosed = true;
			if(_dlg.Is0) return;
			_dlg = Wnd0;
			Send.Clear();
		}
		bool _isClosed;

		#endregion async etc

		#region send messages

		/// <summary>
		/// Gets dialog window handle as Wnd.
		/// Returns Wnd0 if the dialog is not open.
		/// </summary>
		public Wnd DialogWindow { get { return _dlg; } }

		/// <summary>
		/// Used to send task dialog API messages, like <c>td.Send.Message(TaskDialog.TDApi.TDM.CLICK_VERIFICATION, 1);</c>
		/// Before showing the dialog returns null. After closing the dialog the returned object is deactivated; its method calls are ignored.
		/// </summary>
		public TDMessageSender Send { get; private set; }

		/// <summary>
		/// Used to send task dialog API messages, like <c>td.Send.Message(TaskDialog.TDApi.TDM.CLICK_VERIFICATION, 1);</c>
		/// </summary>
		//[DebuggerStepThrough]
		public class TDMessageSender
		{
			Wnd _dlg;
			TaskDialog _tdo; //note: using _tdo._hdlg would be unsafe in multithreaded context because may be already set to null, even if caller called IsOpen before.

			internal TDMessageSender(Wnd dlg, TaskDialog tdo) { _dlg = dlg; _tdo = tdo; }
			internal void Clear() { _dlg = Wnd0; _tdo = null; }

			/// <summary>
			/// Sends a message to the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example (in an event handler): <c>e.dialog.Send.Message(TaskDialog.TDApi.TDM.CLICK_VERIFICATION, 1);</c>
			/// Also there are several other functions to send some messages: change text, close dialog, enable/disable buttons, set progress.
			/// Message reference: MSDN.
			/// NAVIGATE_PAGE currently not supported.
			/// </summary>
			public int Message(TDApi.TDM message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
			{
				//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
				switch(message) { case TDApi.TDM.CLICK_BUTTON: case TDApi.TDM.ENABLE_BUTTON: case TDApi.TDM.SET_BUTTON_ELEVATION_REQUIRED_STATE: wParam = -wParam; break; }

				//if(!_IsOpen()) return 0;
				return _dlg.Send((uint)message, wParam, lParam);
			}

			void _SetText(bool resizeDialog, TDApi.TDE partId, string text)
			{
				var td = _tdo; if(td == null) return;

				if(partId == TDApi.TDE.CONTENT && td._editType == TDEdit.Multiline) {
					text = td._c.pszContent = text + _multilineString;
				}

				_dlg.SendS((uint)(resizeDialog ? TDApi.TDM.SET_ELEMENT_TEXT : TDApi.TDM.UPDATE_ELEMENT_TEXT), (int)partId, text ?? "");
				//info: null does not change text.

				if(td._IsEdit) td._EditControlUpdateAsync(!resizeDialog);
				//info: sometimes even UPDATE_ELEMENT_TEXT sends our control to the bottom of the Z order.
			}

			/// <summary>
			/// Changes the main big-font text.
			/// Call this method while the dialog is open, eg in an event handler.
			/// </summary>
			public void ChangeText1(string text, bool resizeDialog)
			{
				_SetText(resizeDialog, TDApi.TDE.MAIN_INSTRUCTION, text);
			}

			/// <summary>
			/// Changes the main small-font text.
			/// Call this method while the dialog is open, eg in an event handler.
			/// </summary>
			public void ChangeText2(string text, bool resizeDialog)
			{
				_SetText(resizeDialog, TDApi.TDE.CONTENT, text);
			}

			/// <summary>
			/// Changes the footer text.
			/// Call this method while the dialog is open, eg in an event handler.
			/// </summary>
			public void ChangeFooterText(string text, bool resizeDialog)
			{
				_SetText(resizeDialog, TDApi.TDE.FOOTER, text);
			}

			/// <summary>
			/// Changes the expanded area text.
			/// Call this method while the dialog is open, eg in an event handler.
			/// </summary>
			public void ChangeExpandedText(string text, bool resizeDialog)
			{
				_SetText(resizeDialog, TDApi.TDE.EXPANDED_INFORMATION, text);
			}

#if false //currently not implemented
			/// <summary>
			/// Applies new properties to the dialog while it is already open.
			/// Call this method while the dialog is open, eg in an event handler, after setting new properties.
			/// Sends message TaskDialog.TDApi.TDM.NAVIGATE_PAGE.
			/// </summary>
			public void Reconstruct()
			{
				TaskDialog td = _tdo; if(td == null) return;
				_ApiSendMessageTASKDIALOGCONFIG(_dlg, (uint)TDApi.TDM.NAVIGATE_PAGE, 0, ref td._c);
			}

			[DllImport("user32.dll", EntryPoint = "SendMessageW")]
			static extern LPARAM _ApiSendMessageTASKDIALOGCONFIG(Wnd hWnd, uint msg, LPARAM wParam, [In] ref TASKDIALOGCONFIG c);
#endif
			/// <summary>
			/// Clicks a button. Normally it closes the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Sends message TaskDialog.TDApi.TDM.CLICK_BUTTON.
			/// </summary>
			/// <param name="buttonId">A button id or some other number that will be returned in Show() results. Default: TDResult.Close.</param>
			public bool Close(int buttonId = TDResult.Close)
			{
				return 0 != Message(TDApi.TDM.CLICK_BUTTON, buttonId);
			}

			/// <summary>
			/// Enables or disables a button. Normally it closes the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example: <c>d.Created += e => { e.dialog.Send.EnableButton(TDResult.Yes, false); };</c>
			/// Sends message TaskDialog.TDApi.TDM.ENABLE_BUTTON.
			/// </summary>
			public void EnableButton(int buttonId, bool enable)
			{
				Message(TDApi.TDM.ENABLE_BUTTON, buttonId, enable);
			}

			/// <summary>
			/// Sets progress bar value, 0 to 100.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Sends message TaskDialog.TDApi.TDM.SET_PROGRESS_BAR_POS.
			/// </summary>
			public int Progress(int percent)
			{
				return Message(TDApi.TDM.SET_PROGRESS_BAR_POS, percent);
			}
		} //class TDMessageSender
		#endregion send messages

		#region marshalButtons, hookProc, timeoutText
#if DEBUG //CONSIDER: use this func always, or add the attribute etc to allow native exception handling.
		//The API throws 'access violation' exception if some value is invalid (eg unknown flags in dwCommonButtons) or it does not like something.
		//.NET does not allow to handle such exceptions, unless we use [HandleProcessCorruptedStateExceptions] or <legacyCorruptedStateExceptionsPolicy enabled="true"/> in config file.
		//It makes dev/debug more difficult.
		[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
		static int _CallTDI(ref TASKDIALOGCONFIG c, out int pnButton, out int pnRadioButton, out int pChecked)
		{
			pnButton = pnRadioButton = pChecked = 0;
			try {
				return TaskDialogIndirect(ref c, out pnButton, out pnRadioButton, out pChecked);
			}
			catch(Exception e) { throw new Win32Exception($"_CallTDI: {e.Message}"); } //note: not just throw;, and don't add inner exception
		}
#endif

		static IntPtr _MarshalButtons(string[] a, out int cButtons, bool escapeId = false)
		{
			if(a == null || a.Length == 0) { cButtons = 0; return Zero; }
			cButtons = a.Length;

			int structSize = Marshal.SizeOf(typeof(TASKDIALOG_BUTTON));
			IntPtr R = Marshal.AllocHGlobal(structSize * a.Length);

			for(int i = 0; i < a.Length; i++) {
				TASKDIALOG_BUTTON b; b.id = a[i].ToIntAndString_(out b.text); //"id text" -> TASKDIALOG_BUTTON

				if(string.IsNullOrEmpty(b.text)) { b.text = a[i]; if(string.IsNullOrEmpty(b.text)) b.text = " "; } //exception if null or ""
				else b.text = b.text.Replace("\r\n", "\n").TrimStart('\n'); //the API adds 2 newlines for \r\n. Only for custom buttons, not for other controls/parts. Also does not like if begins with \n;

				//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids.
				if(escapeId) {
					b.id = -b.id;
					//if(b.id > 0) throw new ArgumentException("button id < 0"); //don't. Allow to use special ids, eg to add a Cancel button that has different text but still works as Cancel (adds x button in titlebar etc).
				}

				unsafe { Marshal.StructureToPtr(b, (IntPtr)((byte*)R + (structSize * i)), false); }
			}

			return R;
		}

		static void _MarshalFreeButtons(ref IntPtr a, ref int cButtons)
		{
			if(a == Zero) return;

			int structSize = Marshal.SizeOf(typeof(TASKDIALOG_BUTTON));

			for(int i = 0; i < cButtons; i++) {
				unsafe { Marshal.DestroyStructure((IntPtr)((byte*)a + (structSize * i)), typeof(TASKDIALOG_BUTTON)); }
			}

			Marshal.FreeHGlobal(a);
			a = Zero; cButtons = 0;
		}

		//Disables timeout on click or key.
		unsafe LPARAM _HookProc(int code, LPARAM wParam, LPARAM lParam)
		{
			if(code >= 0 && _HookProc(code, wParam, ref *(Native.MSG*)lParam)) return 1;

			return Api.CallNextHookEx(Zero, code, wParam, lParam);
		}

		bool _HookProc(int code, LPARAM wParam, ref Native.MSG m)
		{
			switch(m.message) {
			case Api.WM_LBUTTONDOWN:
			case Api.WM_NCLBUTTONDOWN:
			case Api.WM_RBUTTONDOWN:
			case Api.WM_NCRBUTTONDOWN:
			case Api.WM_KEYDOWN:
			case Api.WM_SYSKEYDOWN:
				if(_timeoutActive && m.hwnd.ToplevelParentOrThis == _dlg) {
					_timeoutActive = false;
					//_TimeoutFooterTextHide();
					Send.ChangeFooterText(_timeoutFooterText, false);
				}
				break;
			}
			return false;
		}

		string _TimeoutFooterText(int timeLeft)
		{
			var s = new StringBuilder(FlagRtlLayout ? "." : "");
			s.AppendFormat("This dialog will disappear if not clicked in {0} s.", timeLeft);
			if(!Empty(_timeoutActionText)) s.AppendFormat(" Timeout action: {0}.", _timeoutActionText);
			if(FlagRtlLayout) s.Length--;
			if(!Empty(_timeoutFooterText)) s.AppendFormat("\n{0}", _timeoutFooterText);
			return s.ToString();
		}

		#endregion marshalButtons, hookProc, timeoutText

		#region Edit control

		bool _IsEdit { get { return _editType != TDEdit.None; } }

		void _EditControlInitBeforeShowDialog()
		{
			if(!_IsEdit) return;
			FlagShowMarqueeProgressBar = true;
			FlagShowProgressBar = false;
			if(_c.pszExpandedInformation != null && _editType == TDEdit.Multiline) _SetFlag(TDF_.EXPAND_FOOTER_AREA, true);

			//create font and calculate control height
			//note: don't use system messagebox font. TaskDialog API does not depend on it.
			_editFont = new Util.NativeFont("Verdana", 9, true);
		}

		void _EditControlUpdate(bool onlyZorder = false)
		{
			if(_editWnd.Is0) return;
			if(!onlyZorder) {
				RECT r;
				_EditControlGetPlace(out r);
				_editParent._Move(r.left, r.top, r.Width, r.Height);
				_editWnd._Move(0, 0, r.Width, r.Height);
			}
			_editParent.ZorderTop();
		}

		void _EditControlUpdateAsync(bool onlyZorder = false)
		{
			_editParent.Post(Api.WM_APP + 111, onlyZorder);
		}

		//used to reserve space for multiline Edit control by appending this to text2
		const string _multilineString = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n ";

		Wnd _EditControlGetPlace(out RECT r)
		{
			Wnd parent = _dlg; //don't use the DirectUIHWND control for it, it can create problems

			//We'll hide the progress bar control and create our Edit control in its place.
			Wnd prog = parent.ChildByClass("msctls_progress32");
			prog.GetRectInClientOf(parent, out r);

			if(_editType == TDEdit.Multiline) {
				int top = r.top;
				if(!_c.pszContent.EndsWith_(_multilineString)) {
					_c.pszContent += _multilineString;
					_dlg.SendS((uint)TDApi.TDM.SET_ELEMENT_TEXT, (int)TDApi.TDE.CONTENT, _c.pszContent);
					prog.GetRectInClientOf(parent, out r); //used to calculate Edit control height: after changing text, prog is moved down, and we know its previous location...
				}
				if(_editMultilineHeight == 0) { _editMultilineHeight = r.bottom - top; } else top = r.bottom - _editMultilineHeight;
				r.top = top;
			} else {
				r.top = r.bottom - (_editFont.HeightOnScreen + 8);
			}

			prog.Show(false);
			return parent;
		}
		int _editMultilineHeight;

		void _EditControlCreate()
		{
			RECT r;
			Wnd parent = _EditControlGetPlace(out r);

			//Create an intermediate "#32770" to be direct parent of the Edit control.
			//It is safer (the dialog will not receive Edit notifications) and helps to solve Tab/Esc problems.
			uint pStyle = Api.WS_CHILD | Api.WS_VISIBLE | Api.WS_CLIPCHILDREN | Api.WS_CLIPSIBLINGS; //don't need WS_TABSTOP
			uint pExStyle = Api.WS_EX_NOPARENTNOTIFY; //not Api.WS_EX_CONTROLPARENT
			_editParent = Api.CreateWindowEx(pExStyle, "#32770", null, pStyle, r.left, r.top, r.Width, r.Height, parent, 0, Zero, 0);
			_EditControlParentProcHolder = _EditControlParentProc;
			_editParent.SetWindowLong(Api.DWLP_DLGPROC, Marshal.GetFunctionPointerForDelegate(_EditControlParentProcHolder));

			//Create Edit or ComboBox control.
			string className = "Edit";
			uint style = Api.WS_CHILD | Api.WS_VISIBLE; //don't need WS_TABSTOP
			switch(_editType) {
			case TDEdit.Text: style |= Api.ES_AUTOHSCROLL; break;
			case TDEdit.Password: style |= Api.ES_PASSWORD | Api.ES_AUTOHSCROLL; break;
			case TDEdit.Number: style |= Api.ES_NUMBER | Api.ES_AUTOHSCROLL; break;
			case TDEdit.Multiline: style |= Api.ES_MULTILINE | Api.ES_AUTOVSCROLL | Api.ES_WANTRETURN | Api.WS_VSCROLL; break;
			case TDEdit.Combo: style |= Api.CBS_DROPDOWN | Api.CBS_AUTOHSCROLL | Api.WS_VSCROLL; className = "ComboBox"; break;
			}
			_editWnd = Api.CreateWindowEx(Api.WS_EX_CLIENTEDGE, className, null, style, 0, 0, r.Width, r.Height, _editParent, 0, Zero, 0);

			//Init the control.
			_editWnd.FontHandle = _editFont;
			if(_editType == TDEdit.Combo) {
				if(_editComboItems != null) {
					foreach(string s in _editComboItems.Arr) _editWnd.SendS(Api.CB_INSERTSTRING, -1, s);
					_editWnd.SetControlText(_editText);
				} else {
					bool addItem = false;
					foreach(string s in _editText.SplitLines_()) {
						if(!addItem) { addItem = true; _editWnd.SetControlText(s); } else _editWnd.SendS(Api.CB_INSERTSTRING, -1, s);
					}
				}
				RECT cbr = _editWnd.Rect;
				_editParent._Resize(cbr.Width, cbr.Height); //because ComboBox resizes itself
			} else {
				_editWnd.SetControlText(_editText);
				_editWnd.Send(Api.EM_SETSEL, 0, -1);
			}
			_editParent.ZorderTop();
			_editWnd.FocusControlOfThisThread();
		}

		void _EditControlOnMessage(TDApi.TDN message)
		{
			switch(message) {
			case TDApi.TDN.BUTTON_CLICKED:
				_editText = _editWnd.GetControlText();
				break;
			case TDApi.TDN.EXPANDO_BUTTON_CLICKED:
			case TDApi.TDN.NAVIGATED:
				_EditControlUpdateAsync(); //when expando clicked, sync does not work even with doevents
				break;
			}
		}

		/// <summary>
		/// Gets edit control handle as Wnd.
		/// </summary>
		public Wnd EditControl { get { return _editWnd; } }
		Wnd _editWnd, _editParent;
		Util.NativeFont _editFont;

		//Dlgproc of our intermediate #32770 control, the parent of out Edit control.
		int _EditControlParentProc(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
		{
			//PrintList(msg, wParam, lParam);
			switch(msg) {
			case Api.WM_SETFOCUS: //enables Tab when in single-line Edit control
				_dlg.ChildByClass("DirectUIHWND", true).FocusControlOfThisThread();
				return 1;
			case Api.WM_NEXTDLGCTL: //enables Tab when in multi-line Edit control
				_dlg.ChildByClass("DirectUIHWND", true).FocusControlOfThisThread();
				return 1;
			case Api.WM_CLOSE: //enables Esc when in edit control
				_dlg.Send(msg);
				return 1;
			case Api.WM_APP + 111: //async update edit control pos
				_EditControlUpdate(wParam != 0);
				return 1;
			case Api.WM_DESTROY:
				_editFont.Dispose();
				break;
			}
			return 0;
			//tested: WM_GETDLGCODE, no results.
		}
		DLGPROC _EditControlParentProcHolder;
		delegate int DLGPROC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam);

		#endregion Edit control
	}

	#region global definitions
#pragma warning disable 1591 //missing XML documentation

	/// <summary>
	/// TaskDialog buttons. Used with <see cref="TaskDialog.Show"/> and similar functions.
	/// </summary>
	[Flags]
	public enum TDButtons
	{
		OK = 1,
		Yes = 2,
		No = 4,
		Cancel = 8,
		Retry = 0x10,
		Close = 0x20,

		OKCancel = OK | Cancel,
		YesNo = Yes | No,
		YesNoCancel = Yes | No | Cancel,
	}

	/// <summary>
	/// TaskDialog icon. Used with <see cref="TaskDialog.Show"/> and similar functions.
	/// </summary>
	public enum TDIcon
	{
		Warning = 0xffff,
		Error = 0xfffe,
		Info = 0xfffd,
		Shield = 0xfffc,

		/// <summary>
		/// Your application icon.
		/// It is the first native icon of the entry assembly of this app domain; if there are no icons - of the program file of this process (if it's different); if there are no icons too - the default program icon.
		/// </summary>
		App = Api.IDI_APPLICATION
	}

	/// <summary>
	/// Text edit field type for <see cref="TaskDialog.ShowInputEx"/> and <see cref="TaskDialog.SetEditControl"/>.
	/// </summary>
	public enum TDEdit
	{
		None, Text, Multiline, Password, Number, Combo
	}
#pragma warning restore 1591 //missing XML documentation

	/// <summary>
	/// TaskDialog flags. Used with <see cref="TaskDialog.Show"/> and similar functions.
	/// </summary>
	[Flags]
	public enum TDFlags
	{
		/// <summary>
		/// Display custom buttons as a column of commandl-links, not as a row of classic buttons.
		/// Command links can have multi-line text. The first line has bigger font.
		/// More info about custom buttons: <see cref="TaskDialog.ShowEx"/>.
		/// </summary>
		CommandLinks = 1,

		/// <summary>
		/// Call Thread.Abort() if clicked OK when there are no other buttons. Also when clicked Cancel, No, and on timeout.
		/// </summary>
		EndThread = 2,

		/// <summary>
		/// Show expanded text in footer.
		/// </summary>
		ExpandDown = 4,

		/// <summary>
		/// Show the dialog in the center of the owner window.
		/// </summary>
		OwnerCenter = 8,

		/// <summary>
		/// Don't interpret 0 and negative x y in a special way.
		/// More info: <see cref="TaskDialog.SetXY"/>. 
		/// </summary>
		RawXY = 16,

		/// <summary>
		/// Make the dialog a topmost window (always on top of other windows), regardless of TaskDialog.Options.TopmostIfNoOwner etc.
		/// More info: <see cref="TaskDialog.FlagTopmost"/>. 
		/// </summary>
		Topmost = 32,

		/// <summary>
		/// Set <see cref="TaskDialog.Width"/> = 500.
		/// </summary>
		Wider = 64,

		//NoTaskbarButton = , //not so useful
		//NeverActivate = , //don't know how to implement. LockSetForegroundWindow does not work if we can activate windows. HCBT_ACTIVATE can prevent activating but does not prevent deactivating.
		//AlwaysActivate = , //Don't use. Always allow. Because after AllowActivate (which is also used by Activate etc) always activates dialogs regardless of anything. As well as in uiAccess process.
	}

	/// <summary>
	/// TaskDialog result. Used with <see cref="TaskDialog.Show"/> and similar functions.
	/// Contains multiple result values: clicked button, selected radio button, check box state, input text.
	/// </summary>
	public class TDResult
	{
		internal TDResult(int button, int radioButton, bool isChecked, string editText)
		{
			Button = button; RadioButton = radioButton; IsChecked = isChecked; EditText = editText;
		}

		/// <summary>
		/// Common button ids.
		/// </summary>
		public const int OK = -1, Cancel = -2, Retry = -4, Yes = -6, No = -7, Close = -8, Timeout = -32000;

		/// <summary>
		/// Gets selected button id.
		/// It can be a common button id (eg TDResult.OK), a custom button id (eg 1) or TDResult.Timeout.
		/// Don't confuse with the TDButtons enum.
		/// </summary>
		public int Button { get; set; }

		/// <summary>
		/// Gets selected (checked) radio button id.
		/// </summary>
		public int RadioButton { get; set; }

		/// <summary>
		/// Gets check box state.
		/// </summary>
		public bool IsChecked { get; set; }

		/// <summary>
		/// Gets edit field text.
		/// </summary>
		public string EditText { get; set; }

		/// <summary>
		/// Converts TDResult to int.
		/// Allows to use code <c>switch(TaskDialog.Show(...))</c> instead of <c>switch(TaskDialog.Show(...).Button)</c>.
		/// </summary>
		public static implicit operator int(TDResult r) { return r.Button; }

		/// <summary>
		/// Gets English button name (eg "OK") or custom button id as string (eg "1"). If timeout, gets "Timeout".
		/// Allows to use switch(TaskDialog.Show(...).ButtonName) { case "OK": ... }
		/// </summary>
		public string ButtonName { get { return ButtonIdToName(Button); } }

		internal static string ButtonIdToName(int id)
		{
			string s;
			switch(id) {
			case OK: s = "OK"; break;
			case Cancel: s = "Cancel"; break;
			case Retry: s = "Retry"; break;
			case Yes: s = "Yes"; break;
			case No: s = "No"; break;
			case Close: s = "Close"; break;
			case Timeout: s = "Timeout"; break;
			default: s = id.ToString(); break;
			}
			return s;
		}

		/// <summary>
		/// Formats string $"{ButtonName}, RadioButton={RadioButton}, IsChecked={IsChecked}, EditText={EditText}".
		/// </summary>
		public override string ToString()
		{
			return $"{ButtonName}, RadioButton={RadioButton}, IsChecked={IsChecked}, EditText={EditText}";
		}
	}

	#endregion global definitions

	#region Show

	public partial class TaskDialog
	{
		/// <summary>
		/// Shows task dialog.
		/// Returns clicked button id and other results packed in a TDResult object (more info in Remarks).
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="buttons"><see cref="TDButtons"/>. If omitted or 0, adds OK button.</param>
		/// <param name="icon"><see cref="TDIcon"/>.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="defaultButton">Specifies which button id to return on Enter key. Can be one of common buttons (eg TDResult.Cancel) or a custom button id (eg 1).</param>
		/// <param name="customButtons">
		/// Adds buttons that have custom text and id.
		/// A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List&lt;string&gt;.
		/// You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).
		/// You can use <see cref="TDFlags.CommandLinks"/> in flags to change button style.
		/// </param>
		/// <param name="radioButtons">Adds radio (option) buttons. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List&lt;string&gt;.</param>
		/// <param name="checkBox">If not empty, shows a check box with this text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="ownerWindow">Owner window or null. <see cref="SetOwnerWindow"/>.</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DefaultTitle (default is appdomain name).</param>
		/// <param name="onLinkClick">
		/// A link-clicked event handler function, eg lambda. Enables hyperlinks in small-font text.
		/// Example: <c>TaskDialog.ShowEx("", "Text &lt;a href="example"&gt;link&lt;/a&gt;.", onLinkClick: e => { Print(e.LinkHref); });</c>
		/// </param>
		/// <remarks>
		/// Uses TaskDialog class instance, which uses Windows API function TaskDialogIndirect (you can find more info in MSDN).
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialog.ShowEx(...))</c> instead of <c>switch(TaskDialog.ShowEx(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>TaskDialog.ShowEx("Text.", icon: TDIcon.Info, title: "Title")</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// If ownerWindow, x, y, flag 'owner center', flag 'raw x y' and screen are not specified, the dialog will be in the center of the primary screen or of the screen that contains another window of current process.
		/// Note: the program must have manifest that tells to use comctl32.dll version 6 or later. C# projects by default use old dll, and this function fails.
		/// </remarks>
		public static TDResult ShowEx(
			string text1, object text2 = null,
			TDButtons buttons = 0, TDIcon icon = 0, TDFlags flags = 0, int defaultButton = 0,
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			object ownerWindow = null, int x = 0, int y = 0, int timeoutS = 0,
			string expandedText = null, string footerText = null, string title = null,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialog(text1, text2, buttons, icon, flags, defaultButton,
				customButtons, radioButtons, checkBox, ownerWindow, x, y, timeoutS, expandedText, footerText, title, onLinkClick);
			return d.ShowDialog();
		}

		/// <summary>
		/// Shows task dialog.
		/// Returns clicked button id and other results packed in a TDResult object (more info in Remarks).
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="buttons"><see cref="TDButtons"/>. If omitted or 0, adds OK button.</param>
		/// <param name="icon"><see cref="TDIcon"/>.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="defaultButton">Specifies which button id to return on Enter key. Can be one of common buttons (eg TDResult.Cancel) or a custom button id (eg 1).</param>
		/// <param name="customButtons">
		/// Adds buttons that have custom text and id.
		/// A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List&lt;string&gt;.
		/// You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).
		/// You can use <see cref="TDFlags.CommandLinks"/> in flags to change button style.
		/// </param>
		/// <param name="ownerWindow">Owner window or null. <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowEx"/>.
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialog.Show(...))</c> instead of <c>switch(TaskDialog.Show(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>TaskDialog.Show("Text.", icon: TDIcon.Info)</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// Note: the program must have manifest that tells to use comctl32.dll version 6 or later. C# projects by default use old dll, and this function fails.
		/// </remarks>
		public static TDResult Show(
			string text1, object text2 = null,
			TDButtons buttons = 0, TDIcon icon = 0, TDFlags flags = 0, int defaultButton = 0,
			StringList customButtons = null, object ownerWindow = null
			)
		{
			return ShowEx(text1, text2, buttons, icon, flags, defaultButton, customButtons, ownerWindow: ownerWindow);
		}

		#endregion Show

		#region ShowInput

		/// <summary>
		/// Shows dialog with a text edit field, buttons OK and Cancel, optionally check box and custom buttons.
		/// Returns results packed in a TDResult object: clicked button id (TDResult.OK or TDResult.Cancel), text and check box state.
		/// </summary>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial edit field text. If editType == TDEdit.Combo, the first line sets edit field text, other lines add drop-down list items.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="checkBox">If not empty, shows a check box with this text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="radioButtons">Adds radio (option) buttons. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List&lt;string&gt;.</param>
		/// <param name="ownerWindow">Owner window or null. <see cref="SetOwnerWindow"/>.</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DefaultTitle (default is appdomain name).</param>
		/// <param name="customButtons">Adds buttons that have custom text and id. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List&lt;string&gt;. In the 'style' parameter you can change button style.</param>
		/// <param name="onButtonClick">
		/// A button-clicked event handler function, eg lambda.
		/// For example, it can set edit control text when a custom button clicked. Example (lambda):
		/// <c>e => { if(e.Button == 1) { e.EditText="text"; e.DoNotCloseDialog = true; } }</c>
		/// Or on OK click it can verify edit control text and don't allow to close the dialog if the text is invalid. Example (lambda):
		/// <c>e => { if(e.Button == TDResult.OK) { string _s=e.EditText; if(Empty(_s)) { TaskDialog.Show("Text cannot be empty.", ownerWindow: e.hwnd); e.DoNotCloseDialog = true; } } }</c>
		/// </param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A link-clicked event handler function, like with <see cref="ShowEx"/>.</param>
		/// <remarks>
		/// Uses TaskDialog class instance, like <see cref="ShowEx"/>.
		/// </remarks>
		public static TDResult ShowInputEx(
			string staticText, string initText = null, TDEdit editType = TDEdit.Text,
			TDFlags flags = 0, string checkBox = null, StringList radioButtons = null,
			object ownerWindow = null, int x = 0, int y = 0, int timeoutS = 0,
			string expandedText = null, string footerText = null, string title = null,
			StringList customButtons = null, Action<TDEventArgs> onButtonClick = null,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialog(null, staticText, TDButtons.OKCancel, 0, flags, 0, customButtons, radioButtons,
				checkBox, ownerWindow, x, y, timeoutS, expandedText, footerText, title, onLinkClick);

			d.SetEditControl((editType == TDEdit.None) ? TDEdit.Text : editType, initText);
			if(onButtonClick != null) d.ButtonClicked += onButtonClick;

			return d.ShowDialog();
		}

		/// <summary>
		/// Shows dialog with a text edit field, and gets that text.
		/// Returns true if clicked OK, false if Cancel.
		/// </summary>
		/// <param name="s">Variable that receives the text.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial edit field text.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="ownerWindow">Owner window or null. <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowInputEx"/>.
		/// </remarks>
		public static bool ShowInput(
			out string s,
			string staticText = null, string initText = null, TDEdit editType = TDEdit.Text,
			TDFlags flags = 0, object ownerWindow = null
			)
		{
			s = null;
			TDResult r = ShowInputEx(staticText, initText, editType, flags, ownerWindow: ownerWindow);
			if(r != TDResult.OK) return false;
			s = r.EditText;
			return true;
		}

		/// <summary>
		/// Shows dialog with a numeric text edit field, and gets that number.
		/// Returns true if clicked OK, false if Cancel.
		/// </summary>
		/// <param name="i">Variable that receives the number.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initValue">Initial edit field text.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="ownerWindow">Owner window or null. <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowInputEx"/>.
		/// </remarks>
		public static bool ShowInput(
			out int i,
			string staticText = null, int initValue = 0, TDEdit editType = TDEdit.Text,
			TDFlags flags = 0, object ownerWindow = null
			)
		{
			i = 0;
			string s;
			if(!ShowInput(out s, staticText, initValue.ToString(), editType, flags, ownerWindow)) return false;
			i = s.ToInt32_();
			return true;
		}
	}

	#endregion ShowInput

	#region ShowList

	public partial class TaskDialog
	{
		/// <summary>
		/// Shows task dialog with a list of command-link buttons.
		/// Returns clicked button id and other results packed in a TDResult object (if assigned to an int variable or switch, it is button id). Returns 0 if closed with the X button or the clicked button has no id.
		/// </summary>
		/// <param name="list">List items (buttons). A list of strings "id text" separated by |, like "1 One|2 Two|3 Three|Cancel". Also can be string[] or List&lt;string&gt;. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel).</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="defaultButton">Specifies which button id to return on Enter key.</param>
		/// <param name="checkBox">If not empty, shows a check box with this text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="ownerWindow">Owner window or null. <see cref="SetOwnerWindow"/>.</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DefaultTitle (default is appdomain name).</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A link-clicked event handler function, like with <see cref="ShowEx"/>.</param>
		/// <remarks>
		/// Uses TaskDialog class instance, like <see cref="ShowEx"/>.
		/// </remarks>
		public static TDResult ShowListEx(
			StringList list, string text1 = null, object text2 = null,
			TDFlags flags = 0, int defaultButton = 0, string checkBox = null,
			object ownerWindow = null, int x = 0, int y = 0, int timeoutS = 0,
			string expandedText = null, string footerText = null, string title = null,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialog(text1, text2, 0, 0, flags, defaultButton, null, null, checkBox, ownerWindow, x, y, timeoutS, expandedText, footerText, title, onLinkClick);

			d.SetCustomButtons(list, true);
			d.FlagAllowCancel = true;
			d.SetExpandedText(expandedText, true);
			TDResult R = d.ShowDialog();
			if(R.Button == TDResult.Cancel) R.Button = 0;
			return R;
		}

		/// <summary>
		/// Shows dialog with a list of command-link buttons.
		/// Returns clicked button id. Returns 0 if the window closed with the X button or the clicked button has no id.
		/// </summary>
		/// <param name="list">List items (buttons). A list of strings "id text" separated by |, like "1 One|2 Two|3 Three|Cancel". Also can be string[] or List&lt;string&gt;. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel).</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="ownerWindow">Owner window or null. <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowListEx"/>.
		/// </remarks>
		public static int ShowList(StringList list, string text1 = null, object text2 = null, TDFlags flags = 0, object ownerWindow = null)
		{
			return ShowListEx(list, text1, text2, flags, ownerWindow: ownerWindow);
		}
	}

	#endregion ShowList

	#region ShowProgress
#pragma warning disable 1573 //missing XML documentation for parameters

	public partial class TaskDialog
	{
		/// <summary>
		/// Shows dialog with progress bar.
		/// Creates dialog in new thread and returns without waiting until it is closed.
		/// Returns TaskDialog object that can be used to control the dialog: set progress bar position, update text, close etc.
		/// Most parameters are the same as with <see cref="ShowEx"/>.
		/// If no custom buttons specified, adds Cancel.
		/// </summary>
		/// <param name="marquee">Let the progress bar animate without indicating a percent of work done.</param>
		/// <example><code>
		/// var pd = TaskDialog.ShowProgressEx(false, "Working", customButtons: "1 Stop", y: -1);
		/// for(int i = 1; i != 100; i++) {
		/// 	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		/// 	pd.Send.Progress(i); //don't need this if marquee
		/// 	WaitMS(50); //do something in the loop
		/// }
		/// pd.Send.Close();
		/// </code></example>
		/// <remarks>
		/// Uses TaskDialog class instance, like <see cref="ShowEx"/>.
		/// </remarks>
		public static TaskDialog ShowProgressEx(bool marquee,
			string text1, object text2 = null, TDFlags flags = 0,
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			object ownerWindow = null, int x = 0, int y = 0, int timeoutS = 0,
			string expandedText = null, string footerText = null, string title = null,
			Action<TDEventArgs> onLinkClick = null
		)
		{
			TDButtons buttons = (customButtons == null) ? TDButtons.Cancel : 0; //OK -> Cancel

			var d = new TaskDialog(text1, text2, buttons, 0, flags, 0,
				customButtons, radioButtons, checkBox, ownerWindow, x, y, timeoutS, expandedText, footerText, title, onLinkClick);

			if(marquee) d.FlagShowMarqueeProgressBar = true; else d.FlagShowProgressBar = true;

			d.ShowNoWait();

			if(marquee) d.Send.Message(TDApi.TDM.SET_PROGRESS_BAR_MARQUEE, true);

			return d;
		}

		/// <summary>
		/// Shows dialog with progress bar.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialog object that can be used to control the dialog: set progress bar position, close etc.
		/// All parameters except marquee are the same as with <see cref="ShowEx"/>.
		/// </summary>
		/// <param name="marquee">Let the progress bar animate without indicating a percent of work done.</param>
		/// <example><code>
		/// var pd = TaskDialog.ShowProgress(false, "Working", customButtons: "1 Stop", y: -1);
		/// for(int i = 1; i != 100; i++) {
		/// 	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		/// 	pd.Send.Progress(i); //don't need this if marquee
		/// 	WaitMS(50); //do something in the loop
		/// }
		/// pd.Send.Close();
		/// </code></example>
		/// <remarks>
		/// Calls <see cref="ShowProgressEx"/>.
		/// </remarks>
		public static TaskDialog ShowProgress(bool marquee,
			string text1, object text2 = null, TDFlags flags = 0,
			StringList customButtons = null, object ownerWindow = null, int x = 0, int y = 0)
		{
			return ShowProgressEx(marquee, text1, text2, flags, customButtons, ownerWindow: ownerWindow, x: x, y: y);
		}
	}

#pragma warning restore 1573 //missing XML documentation for parameters
	#endregion ShowProgress

	#region ShowNoWait
#pragma warning disable 1573 //missing XML documentation for parameters

	public partial class TaskDialog
	{
		/// <summary>
		/// Shows task dialog like <see cref="ShowEx"/> but does not wait.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialog object that can be used to control the dialog, eg close.
		/// Most parameters are the same as with <see cref="ShowEx"/>.
		/// </summary>
		/// <param name="whenClosed">null or an event handler function (lambda etc) to call when the dialog closed.</param>
		/// <param name="closeOnExit">If true, the dialog disappears when all other threads of this app domain end.</param>
		/// <example><code>
		/// //TaskDialog.ShowNoWaitEx(null, false, "Text."); //simplest example
		/// var td = TaskDialog.ShowNoWaitEx(e =˃ { Print(e); }, true, "Text.", buttons:TDButtons.OKCancel);
		/// Wait(2); //do something while the dialog is open in other thread
		/// td.ThreadWaitClosed(); //wait until dialog closed (optional, just an example)
		/// </code></example>
		/// <remarks>
		/// Uses TaskDialog class instance, like <see cref="ShowEx"/>.
		/// </remarks>
		public static TaskDialog ShowNoWaitEx(
			Action<TDResult> whenClosed, bool closeOnExit,
			string text1, object text2 = null,
			TDButtons buttons = 0, TDIcon icon = 0, TDFlags flags = 0, int defaultButton = 0,
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			object ownerWindow = null, int x = 0, int y = 0, int timeoutS = 0,
			string expandedText = null, string footerText = null, string title = null,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialog(text1, text2, buttons, icon, flags, defaultButton,
				customButtons, radioButtons, checkBox, ownerWindow, x, y, timeoutS, expandedText, footerText, title, onLinkClick);
			d.ShowNoWait(whenClosed, closeOnExit);
			return d;
		}

		/// <summary>
		/// Shows task dialog like <see cref="Show"/> but does not wait.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialog object that can be used to control the dialog, eg close.
		/// Most parameters are the same as with <see cref="Show"/>.
		/// </summary>
		/// <param name="whenClosed">null or an event handler function (lambda etc) to call when the dialog closed.</param>
		/// <param name="closeOnExit">If true, the dialog disappears when all other threads of this app domain end.</param>
		/// <example><code>
		/// //TaskDialog.ShowNoWait(null, false, "Text."); //simplest example
		/// var td = TaskDialog.ShowNoWait(e =˃ { Print(e); }, true, "Text.", buttons:TDButtons.OKCancel);
		/// Wait(2); //do something while the dialog is open in other thread
		/// td.ThreadWaitClosed(); //wait until dialog closed (optional, just an example)
		/// </code></example>
		/// <remarks>
		/// Calls <see cref="ShowNoWaitEx"/>.
		/// </remarks>
		public static TaskDialog ShowNoWait(
			Action<TDResult> whenClosed, bool closeOnExit,
			string text1, object text2 = null,
			TDButtons buttons = 0, TDIcon icon = 0, TDFlags flags = 0, int defaultButton = 0,
			StringList customButtons = null, object ownerWindow = null
			)
		{
			return ShowNoWaitEx(whenClosed, closeOnExit, text1, text2, buttons, icon, flags, defaultButton, customButtons, ownerWindow: ownerWindow);
		}
	}

#pragma warning restore 1573 //missing XML documentation for parameters
	#endregion ShowNoWait
}
