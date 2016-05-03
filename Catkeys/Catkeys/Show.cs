using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
//using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
using System.Windows.Forms;
using System.Drawing;

using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

#pragma warning disable 649 //unused fields in API structs

namespace Catkeys
{
	/// <summary>
	/// Standard dialogs, tooltips and other windows to show information to the user or get instructions/input from the user.
	/// </summary>
	//[DebuggerStepThrough]
	public static partial class Show
	{

		//Other parts of this class are below in this file, wrapped in regions together with associated enums etc.
	}

#if use_MessageDialog
	//Almost complete. Need just to implement screen and EndThread option.
	//Instead use TaskDialogEx and co. If need classic message box, use MessageBox.Show(). Don't need 3 functions for the same.
	#region MessageDialog

	/// <summary>
	/// MessageDialog return value (user-clicked button).
	/// </summary>
	public enum MDResult
	{
		OK = 1, Cancel = 2, Abort = 3, Retry = 4, Ignore = 5, Yes = 6, No = 7/*, Timeout = 9*/, TryAgain = 10, Continue = 11,
	}

	/// <summary>
	/// MessageDialog buttons.
	/// </summary>
	public enum MDButtons
	{
		OK = 0, OKCancel = 1, AbortRetryIgnore = 2, YesNoCancel = 3, YesNo = 4, RetryCancel = 5, CancelTryagainContinue = 6,
	}

	/// <summary>
	/// MessageDialog icon.
	/// </summary>
	public enum MDIcon
	{
		None = 0, Error = 0x10, Question = 0x20, Warning = 0x30, Info = 0x40, Shield = 0x50, App = 0x60,
	}

	/// <summary>
	/// MessageDialog flags.
	/// </summary>
	[Flags]
	public enum MDFlag :uint
	{
		DefaultButton2 = 0x100, DefaultButton3 = 0x200, DefaultButton4 = 0x300,
		SystemModal = 0x1000, DisableThreadWindows = 0x2000, HelpButton = 0x4000,
		TryActivate = 0x10000, DefaultDesktopOnly = 0x20000, Topmost = 0x40000, RightAlign = 0x80000, RtlLayout = 0x100000, ServiceNotification = 0x200000,
		//not API flags
		NoSound = 0x80000000,
		//todo: EndThread.
	}

	public static partial class Show
	{
		/// <summary>
		/// Shows classic message box dialog.
		/// Like System.Windows.Forms.MessageBox.Show but has more options and is always-on-top by default.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="buttons">Example: MDButtons.YesNo.</param>
		/// <param name="icon">One of standard icons. Example: MDIcon.Info.</param>
		/// <param name="flags">One or more options. Example: MDFlag.NoTopmost|MDFlag.DefaultButton2.</param>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <remarks>
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, ScriptOptions.DisplayName (title).
		/// </remarks>
		public static MDResult MessageDialog(string text, MDButtons buttons, MDIcon icon = 0, MDFlag flags = 0, Wnd owner = default(Wnd), string title = null)
		{
			//const uint MB_SYSTEMMODAL = 0x1000; //same as MB_TOPMOST + adds system icon in title bar (why need it?)
			const uint MB_USERICON = 0x80;
			const uint IDI_APPLICATION = 32512;
			const uint IDI_ERROR = 32513;
			const uint IDI_QUESTION = 32514;
			const uint IDI_WARNING = 32515;
			const uint IDI_INFORMATION = 32516;
			const uint IDI_SHIELD = 106; //32x32 icon. The value is undocumented, but we use it instead of the documented IDI_SHIELD value which on Win7 displays clipped 128x128 icon. Tested: the function does not fail with invalid icon resource id.

			var p = new MSGBOXPARAMS();
			p.cbSize = Api.SizeOf(p);
			p.lpszCaption = _Util.Title(title);
			p.lpszText = text;

			bool alien = (flags & (MDFlag.DefaultDesktopOnly | MDFlag.ServiceNotification)) != 0;
			if(alien) owner = Wnd0; //API would fail. The dialog is displayed in csrss process.

			if(icon == MDIcon.None) { } //no sound
			else if(icon == MDIcon.Shield || icon == MDIcon.App || flags.HasFlag(MDFlag.NoSound)) {
				switch(icon) {
				case MDIcon.Error: p.lpszIcon = (IntPtr)IDI_ERROR; break;
				case MDIcon.Question: p.lpszIcon = (IntPtr)IDI_QUESTION; break;
				case MDIcon.Warning: p.lpszIcon = (IntPtr)IDI_WARNING; break;
				case MDIcon.Info: p.lpszIcon = (IntPtr)IDI_INFORMATION; break;
				case MDIcon.Shield: p.lpszIcon = (IntPtr)IDI_SHIELD; break;
				case MDIcon.App:
					p.lpszIcon = (IntPtr)IDI_APPLICATION;
					if(Util.Misc.GetAppIconHandle(32) != Zero) p.hInstance = Util.Misc.GetModuleHandleOfAppdomainEntryAssembly();
					//info: C# compiler adds icon to the native resources as IDI_APPLICATION.
					//	If assembly without icon, we set hInstance=0 and then the API shows common app icon.
					//	In any case, it will be the icon displayed in File Explorer etc.
					break;
				}
				p.dwStyle |= MB_USERICON; //disables sound
				icon = 0;
			}

			if(Script.Option.dialogRtlLayout) flags |= MDFlag.RtlLayout;
			if(owner.Is0) {
				flags |= MDFlag.TryActivate; //if foreground lock disabled, activates, else flashes taskbar button; without this flag the dialog woud just sit behind other windows, often unnoticed.
				if(Script.Option.dialogTopmostIfNoOwner) flags |= MDFlag.SystemModal; //makes topmost, always works, but also adds an unpleasant system icon in title bar
																					  //if(Script.Option.dialogTopmostIfNoOwner) flags|=MDFlag.Topmost; //often ignored, without a clear reason and undocumented, also noticed other anomalies
			}
			//tested: if owner is child, the API disables its top-level parent.
			//consider: if owner 0, create hidden parent window to:
			//	Avoid adding taskbar icon.
			//	Apply Option.dialogScreenIfNoOwner.
			//consider: if owner 0, and current foreground window is of this thread, let it be owner. Maybe a flag.
			//consider: if owner of other thread, don't disable it. But how to do it without hook? Maybe only inherit owner's monitor.
			//consider: play user-defined sound.

			p.hwndOwner = owner;

			flags &= ~(MDFlag.NoSound); //not API flags
			p.dwStyle |= (uint)buttons | (uint)icon | (uint)flags;

			int R = MessageBoxIndirect(ref p);
			if(R == 0) throw new CatkeysException();

			_Util.DoEventsAndWaitForAnActiveWindow();

			return (MDResult)R;

			//tested:
			//user32:MessageBoxTimeout. Undocumented. Too limited etc to be useful. If need timeout, use TaskDialogEx().
			//shlwapi:SHMessageBoxCheck. Too limited etc to be useful.
			//wtsapi32:WTSSendMessageW. In csrss process, no themes, etc. Has timeout.
		}

		/// <summary>
		/// Shows classic message box dialog.
		/// Returns clicked button's character (as in style), eg 'O' for OK.
		/// You can specify buttons etc in style string, which can contain:
		/// <para>Buttons: OC OKCancel, YN YesNo, YNC YesNoCancel, ARI AbortRetryIgnore, RC RetryCancel, CTE CancelTryagainContinue.</para>
		/// <para>Icon: x error, ! warning, i info, ? question, v shield, a app.</para>
		/// <para>Flags: s no sound, t topmost, d disable windows.</para>
		/// <para>Default button: 2 or 3.</para>
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="style">Example: "YN!".</param>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <remarks>
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, ScriptOptions.DisplayName (title).
		/// </remarks>
		public static char MessageDialog(string text, string style = null, Wnd owner = default(Wnd), string title = null)
		{
			MDButtons buttons = 0;
			MDIcon icon = 0;
			MDFlag flags = 0;

			if(!string.IsNullOrEmpty(style)) {
				if(style.Contains("OC")) buttons = MDButtons.OKCancel;
				else if(style.Contains("YNC")) buttons = MDButtons.YesNoCancel;
				else if(style.Contains("YN")) buttons = MDButtons.YesNo;
				else if(style.Contains("ARI")) buttons = MDButtons.AbortRetryIgnore;
				else if(style.Contains("RC")) buttons = MDButtons.RetryCancel;
				else if(style.Contains("CT")) buttons = MDButtons.CancelTryagainContinue; //not CTC, because Continue returns E

				if(style.Contains("x")) icon = MDIcon.Error;
				else if(style.Contains("?")) icon = MDIcon.Question;
				else if(style.Contains("!")) icon = MDIcon.Warning;
				else if(style.Contains("i")) icon = MDIcon.Info;
				else if(style.Contains("v")) icon = MDIcon.Shield;
				else if(style.Contains("a")) icon = MDIcon.App;

				if(style.Contains("t")) flags |= MDFlag.SystemModal; //MDFlag.Topmost often ignored etc
				if(style.Contains("s")) flags |= MDFlag.NoSound;
				if(style.Contains("d")) flags |= MDFlag.DisableThreadWindows;

				if(style.Contains("2")) flags |= MDFlag.DefaultButton2;
				else if(style.Contains("3")) flags |= MDFlag.DefaultButton3;
			}

			int r = (int)MessageDialog(text, buttons, icon, flags, owner, title);

			return (r > 0 && r < 12) ? "COCARIYNCCTE"[r] : 'C';
		}

		struct MSGBOXPARAMS
		{
			public uint cbSize;
			public Wnd hwndOwner;
			public IntPtr hInstance;
			public string lpszText;
			public string lpszCaption;
			public uint dwStyle;
			public IntPtr lpszIcon;
			public LPARAM dwContextHelpId;
			public IntPtr lpfnMsgBoxCallback;
			public uint dwLanguageId;
		}

		[DllImport("user32.dll", EntryPoint = "MessageBoxIndirectW")]
		static extern int MessageBoxIndirect([In] ref MSGBOXPARAMS lpMsgBoxParams);

	}
	#endregion MessageDialog
#endif

	#region TaskDialog

	/// <summary>
	/// TaskDialog() buttons.
	/// </summary>
	[Flags]
	public enum TDButton
	{
		OK = 1, //O
		Yes = 2, //Y
		No = 4, //N
		Cancel = 8, //C
		Retry = 0x10, //R
		Close = 0x20, //L
		OKCancel = OK | Cancel,
		YesNo = Yes | No,
		YesNoCancel = Yes | No | Cancel,
		RetryCancel = Retry | Cancel
	}

	/// <summary>
	/// TaskDialog() icon.
	/// </summary>
	[Flags]
	public enum TDIcon
	{
		Warning = 0xffff, //!
		Error = 0xfffe, //x
		Info = 0xfffd, //i
		Shield = 0xfffc, //v
		App = 32512 //a, IDI_APPLICATION
	}

	/// <summary>
	/// TaskDialog() flags.
	/// </summary>
	[Flags]
	public enum TDFlag
	{
		CommandLinks = 1, //c
		NoTaskbarButton = 2, //b
		EndThread = 4, //e
		NeverActivate = 8, //n
		OwnerCenter = 16, //o
		RawXY = 32, //r
		Topmost = 64, //t
	}

	/// <summary>
	/// Dialog buttons, icon, flags and default button. Used as 'style' parameter with TaskDialog() and similar functions.
	/// <para>
	/// Supports implicit cast from string, which can contain these characters:
	/// </para>
	/// <para>Buttons: O OK, C Cancel, Y Yes, N No, R Retry, L Close.</para>
	/// <para>Icon: x error, ! warning, i info, v shield, a app.</para>
	/// <para>Flags: c command links, b no taskbar button, e end thread, n never activate, o owner center, r raw x y, t topmost.</para>
	/// <para>Default button: d before a common button character (eg "OdC") or custom button id (eg "d3").</para>
	/// <para>Also supports implicit cast from TDButton, TDIcon, TDFlags, int (default button), IntPtr (icon handle) and System.Drawing.Icon.</para>
	/// </summary>
	/// <example>
	/// <code>TaskDialog(..., new TDStyle(TDButton.YesNo, TDIcon.Warning, TDFlag.OwnerCenter), ...);</code>
	/// <code>TaskDialog(..., "YN!o", ...); //does the same as the above</code>
	/// <code>TaskDialog(..., TDButton.YesNo|TDButton.Retry, ...);</code>
	/// <code>TaskDialog(..., TDIcon.Warning, ...);</code>
	/// <code>TaskDialog(..., TDFlag.CommandLinks|TDFlag.OwnerCenter, ...);</code>
	/// <code>TaskDialog(..., new System.Drawing.Icon(@"c:\icons\icon.ico", ...);</code>
	/// </example>
	public class TDStyle
	{
		public TDButton buttons;
		public TDIcon icon;
		public TDFlag flags;
		public int defaultButton;
		public object customIcon;

		public TDStyle(TDButton buttons = 0, TDIcon icon = 0, TDFlag flags = 0, int defaultButton = 0, object customIcon = null)
		{
			this.buttons = buttons; this.icon = icon; this.flags = flags; this.defaultButton = defaultButton; this.customIcon = customIcon;
        }

		public TDStyle(string style)
		{
			if(style == null) return;
			for(int i = 0, n = style.Length; i < n; i++) {
				char c = style[i];
				switch(c) {
				//buttons
				case 'O': buttons |= TDButton.OK; break;
				case 'C': buttons |= TDButton.Cancel; break;
				case 'R': buttons |= TDButton.Retry; break;
				case 'Y': buttons |= TDButton.Yes; break;
				case 'N': buttons |= TDButton.No; break;
				case 'L': buttons |= TDButton.Close; break;
				//icon
				case 'x': icon |= TDIcon.Error; break;
				case '!': icon |= TDIcon.Warning; break;
				case 'i': icon |= TDIcon.Info; break;
				case 'v': icon |= TDIcon.Shield; break;
				case 'a': icon |= TDIcon.App; break;
				//flags
				case 'c': flags |= TDFlag.CommandLinks; break;
				case 'b': flags |= TDFlag.NoTaskbarButton; break;
				case 'e': flags |= TDFlag.EndThread; break;
				case 'n': flags |= TDFlag.NeverActivate; break;
				case 'o': flags |= TDFlag.OwnerCenter; break;
				case 'r': flags |= TDFlag.RawXY; break;
				case 't': flags |= TDFlag.Topmost; break;
				//default button
				case 'd':
					if(i == n - 1) break; //incorrectly used
					switch(style[i + 1]) {
					case 'O': defaultButton = TDResult.OK; break;
					case 'C': defaultButton = TDResult.Cancel; break;
					case 'R': defaultButton = TDResult.Retry; break;
					case 'Y': defaultButton = TDResult.Yes; break;
					case 'N': defaultButton = TDResult.No; break;
					case 'L': defaultButton = TDResult.Close; break;
					default:
						int eon;
						defaultButton = style.ToInt_(i + 1, out eon);
						if(eon != 0) i = eon - 1; //else incorrectly used
						break;
					}
					break;
					//default: //unknown character
				}
				//note: don't throw if unknown or incorrectly used character. It is not so important. Maybe show warning, when we will have a Warning() function.
			}
		}

		public static implicit operator TDStyle(string style) { return new TDStyle(style); }
		public static implicit operator TDStyle(TDButton buttons) { return new TDStyle(buttons); }
		public static implicit operator TDStyle(TDIcon icon) { return new TDStyle(icon: icon); }
		public static implicit operator TDStyle(TDFlag flags) { return new TDStyle(flags: flags); }
		public static implicit operator TDStyle(int defaultButton) { return new TDStyle(defaultButton: defaultButton); }
		public static implicit operator TDStyle(IntPtr customIcon) { return new TDStyle(customIcon: customIcon); }
		public static implicit operator TDStyle(System.Drawing.Icon customIcon) { return new TDStyle(customIcon: customIcon); }
	}

	/// <summary>
	/// TaskDialog() return values (selected button, radio button, is checked).
	/// </summary>
	public class TDResult
	{
		public TDResult(int button, int radioButton, bool isChecked, string editText)
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
		/// Don't confuse with the TDButton enum.
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
		/// Gets Edit control text.
		/// </summary>
		public string EditText { get; set; }

		/// <summary>
		/// Converts TDResult to int.
		/// Allows to use code <c>switch(TaskDialog(...))</c> instead of <c>switch(TaskDialog(...).Button)</c>.
		/// </summary>
		public static implicit operator int (TDResult r) { return r.Button; }

		/// <summary>
		/// Gets English button name (eg "OK") or custom button id as string (eg "1"). If timeout, gets "Timeout".
		/// Allows to use switch(TaskDialog(...).ButtonName) { case "OK": ... }
		/// </summary>
		public string ButtonName
		{
			get
			{
				string s;
				switch(Button) {
				case OK: s = "OK"; break;
				case Cancel: s = "Cancel"; break;
				case Retry: s = "Retry"; break;
				case Yes: s = "Yes"; break;
				case No: s = "No"; break;
				case Close: s = "Close"; break;
				case Timeout: s = "Timeout"; break;
				default: s = Button.ToString(); break;
				}
				return s;
			}
		}

		/// <summary>
		/// Formats string $"{ButtonName}, RadioButton={RadioButton}, IsChecked={IsChecked}".
		/// </summary>
		public override string ToString()
		{
			return $"{ButtonName}, RadioButton={RadioButton}, IsChecked={IsChecked}";
		}
	}

	/// <summary>
	/// Edit control type for Show.InputDialogX and TaskDialogObject.SetEditControl().
	/// </summary>
	public enum TDEdit
	{
		None, Text, Multiline, Password, Number, Combo
	}

	/// <summary>
	/// Constants for TaskDialogObject messages and events.
	/// </summary>
	public class TDApi
	{
		/// <summary>
		/// Messages that your event handler can send to the dialog.
		/// Reference: MSDN -> "Task Dialog Messages".
		/// </summary>
		public enum TDM :uint
		{
			NAVIGATE_PAGE = WM_USER + 101,
			CLICK_BUTTON = WM_USER + 102, // wParam = button id
			SET_MARQUEE_PROGRESS_BAR = WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
			SET_PROGRESS_BAR_STATE = WM_USER + 104, // wParam = new progress state (0, 1 or 2)
			SET_PROGRESS_BAR_RANGE = WM_USER + 105, // lParam = Calc.MakeLparam(min, max)
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
		/// Constants for TDApi.TDM.SET_ELEMENT_TEXT and TDApi.TDM.UPDATE_ELEMENT_TEXT messages and TaskDialogObject.Send.Text().
		/// </summary>
		public enum TDE
		{
			CONTENT,
			EXPANDED_INFORMATION,
			FOOTER,
			MAIN_INSTRUCTION
		}

		/// <summary>
		/// Constants for TDApi.TDM.UPDATE_ICON message.
		/// </summary>
		public enum TDIE
		{
			ICON_MAIN,
			ICON_FOOTER
		}
	}

	public static partial class Show
	{
		/// <summary>
		/// Shows task dialog.
		/// Returns clicked button id and other results packed in a TDResult object (more info in Remarks).
		/// </summary>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="buttons">Examples: TDButton.YesNo, TDButton.OK|TDButton.Close. If omitted or 0, adds OK button.</param>
		/// <param name="icon">One of four standard icons, eg TDIcon.Info.</param>
		/// <param name="flags">Example: TDFlag.CommandLinks|TDFlag.OwnerCenter.</param>
		/// <param name="defaultButton">Specifies which button id to return on Enter key. Can be one of common buttons (eg TDResult.Cancel) or a custom button id (eg 1).</param>
		/// <param name="customButtons">Adds buttons that have custom text and id. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃. Use TDFlag.CommandLinks in flags to change button style. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).</param>
		/// <param name="radioButtons">Adds radio (option) buttons. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃.</param>
		/// <param name="checkBox">Check box text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A link-clicked event handler function, eg lambda. Example: <c>Show.TaskDialogEx("", "Text ˂a href="example"˃link˂/a˃.", onLinkClick: ed => { Out(ed.linkHref); });</c></param>
		/// <remarks>
		/// Uses TaskDialogObject class, which uses Windows API function TaskDialogIndirect (you can find more info in MSDN).
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialogEx(...))</c> instead of <c>switch(TaskDialogEx(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>Show.TaskDialogEx("Text.", icon: TDIcon.Info, title: "Title")</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner (more info in Wnd.MoveInScreen()), ScriptOptions.DisplayName (title).
		/// If owner, x, y, flag 'owner center', flag 'raw x y' and screen are not specified, the dialog will be in the center of the primary screen or of the screen that contains another window of current process.
		/// </remarks>
		public static TDResult TaskDialogEx(
			Wnd owner, string text1, string text2 = null,
			TDButton buttons = 0, TDIcon icon = 0, TDFlag flags = 0, int defaultButton = 0,
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialogObject(owner, text1, text2, buttons, icon, flags,
				defaultButton, customButtons, radioButtons, checkBox, expandedText, footerText, title, x, y, timeoutS, onLinkClick);
			return d.Show();
		}

		/// <summary>
		/// Shows task dialog.
		/// Returns clicked button id and other results packed in a TDResult object (more info in Remarks).
		/// </summary>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="buttons">Examples: TDButton.YesNo, TDButton.OK|TDButton.Close. If omitted or 0, adds OK button.</param>
		/// <param name="icon">One of four standard icons, eg TDIcon.Info.</param>
		/// <param name="flags">Example: TDFlag.CommandLinks|TDFlag.OwnerCenter.</param>
		/// <param name="defaultButton">Specifies which button id to return on Enter key. Can be one of common buttons (eg TDResult.Cancel) or a custom button id (eg 1).</param>
		/// <param name="customButtons">Adds buttons that have custom text and id. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃. Use TDFlag.CommandLinks in flags to change button style. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).</param>
		/// <remarks>
		/// Calls TaskDialogEx(), which uses TaskDialogObject class, which uses Windows API function TaskDialogIndirect (you can find more info in MSDN).
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialog(...))</c> instead of <c>switch(TaskDialog(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>Show.TaskDialog("Text.", icon: TDIcon.Info)</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner (more info in Wnd.MoveInScreen()), ScriptOptions.DisplayName (title).
		/// </remarks>
		public static TDResult TaskDialog(
			Wnd owner, string text1, string text2 = null, TDButton buttons = 0, TDIcon icon = 0, TDFlag flags = 0,
			int defaultButton = 0, StringList customButtons = null
			)
		{
			return TaskDialogEx(owner, text1, text2, buttons, icon, flags, defaultButton, customButtons);
		}

		/// <summary>
		/// Shows task dialog.
		/// Returns clicked button id and other results packed in a TDResult object (more info in Remarks).
		/// <para>
		/// You can specify dialog style (buttons, icon etc) as string which can contain these characters:
		/// </para>
		/// <para>Buttons: O OK, C Cancel, Y Yes, N No, R Retry, L Close.</para>
		/// <para>Icon: x error, ! warning, i info, v shield, a app.</para>
		/// <para>Flags: c command links, b no taskbar button, e end thread, n never activate, o owner center, r raw x y, t topmost.</para>
		/// <para>Default button: d before a common button character (eg "OdC") or custom button id (eg "d3").</para>
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="style">Style string (like "YN!e"), or TDButton (like TDButton.YesNo|TDButton.Retry), or TDIcon (like TDIcon.Info), or TDFlags (like TDFlag.NeverActivate|TDFlag.EndThread), or TDStyle object (like new TDStyle(TDButton.YesNo, TDIcon.Warning, TDFlag.OwnerCenter)).</param>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="customButtons">Adds buttons that have custom text and id. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃. Use "c" in style to change button style. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).</param>
		/// <param name="radioButtons">Adds radio (option) buttons. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃.</param>
		/// <param name="checkBox">Check box text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A link-clicked event handler function, eg lambda. Example: <c>Show.TaskDialogEx("", "Text ˂a href="example"˃link˂/a˃.", onLinkClick: ed => { Out(ed.linkHref); });</c></param>
		/// <remarks>
		/// The difference from the other overload: instead of 4 parameters for buttons, icon, flags and default button here is used 1 parameter 'style'.
		/// Uses TaskDialogObject class, which uses Windows API function TaskDialogIndirect (you can find more info in MSDN).
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialogEx(...))</c> instead of <c>switch(TaskDialogEx(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>Show.TaskDialogEx("Text.", style: "YN!", title: "Title")</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner (more info in Wnd.MoveInScreen()), ScriptOptions.DisplayName (title).
		/// If owner, x, y, flag 'owner center', flag 'raw x y' and screen are not specified, the dialog will be in the center of the primary screen or of the screen that contains another window of current process.
		/// </remarks>
		public static TDResult TaskDialogEx(
			string text1, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd),
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialogObject(text1, text2, style, owner,
				customButtons, radioButtons, checkBox, expandedText, footerText, title, x, y, timeoutS, onLinkClick);
			return d.Show();
		}

		/// <summary>
		/// Shows task dialog.
		/// Returns clicked button id and other results packed in a TDResult object (more info in Remarks).
		/// <para>
		/// You can specify dialog style (buttons, icon etc) as string which can contain these characters:
		/// </para>
		/// <para>Buttons: O OK, C Cancel, Y Yes, N No, R Retry, L Close.</para>
		/// <para>Icon: x error, ! warning, i info, v shield, a app.</para>
		/// <para>Flags: c command links, b no taskbar button, e end thread, n never activate, o owner center, r raw x y, t topmost.</para>
		/// <para>Default button: d before a common button character (eg "OdC") or custom button id (eg "d3").</para>
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="style">Style string (like "YN!e"), or TDButton (like TDButton.YesNo|TDButton.Retry), or TDIcon (like TDIcon.Info), or TDFlags (like TDFlag.NeverActivate|TDFlag.EndThread), or TDStyle object (like new TDStyle(TDButton.YesNo, TDIcon.Warning, TDFlag.OwnerCenter)).</param>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="customButtons">Adds buttons that have custom text and id. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃. Use "c" in style to change button style. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).</param>
		/// <remarks>
		/// The difference from the other overload: instead of 4 parameters for buttons, icon, flags and default button here is used 1 parameter 'style'.
		/// Calls TaskDialogEx(), which uses TaskDialogObject class, which uses Windows API function TaskDialogIndirect (you can find more info in MSDN).
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialog(...))</c> instead of <c>switch(TaskDialog(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>Show.TaskDialog("Text.", style: "YN!")</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner (more info in Wnd.MoveInScreen()), ScriptOptions.DisplayName (title).
		/// </remarks>
		public static TDResult TaskDialog(string text1, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd), StringList customButtons = null)
		{
			return TaskDialogEx(text1, text2, style, owner, customButtons);
		}
	}

	/// <summary>
	/// Shows task dialog like Show.TaskDialogEx() but has more options and allows you to use more object-oriented programming style.
	/// Uses Windows API function TaskDialogIndirect. You can find more info in MSDN.
	/// </summary>
	/// <example>
	/// <code>
	/// var d = new TaskDialogObject(); //this example uses the 0-parameters constructor, but also there are constructors that have all the same parameters as Show.TaskDialogEx()
	/// d.SetText("Main text.", "More text.\nSupports ˂A HREF=\"link data\"˃links˂/A˃ if you subscribe to HyperlinkClick event.");
	/// d.SetStyle("OC!");
	/// d.SetExpandedText("Expanded info\nand more info.", true);
	/// d.FlagCanBeMinimized=true;
	/// d.SetCustomButtons("1 one|2 two\nzzz", true);
	/// d.SetRadioButtons("1001 r1|1002 r2");
	/// d.SetTimeout(30, "OK");
	/// d.HyperlinkClicked += ed => { Out($"{ed.message} {ed.linkHref}"); };
	/// d.ButtonClicked += ed => { Out($"{ed.message} {ed.wParam}"); if(ed.wParam==TDResult.No) ed.returnValue=1; }; //ed.returnValue=1 prevents closing
	/// d.FlagShowProgressBar=true; d.Timer += ed => { ed.obj.Send.Progress(ed.wParam/100); };
	/// TDResult r = d.Show();
	/// switch(r.Button) { case TDResult.OK: ... case 1: ... }
	/// switch(r.RadioButton) { ... }
	/// if(r.IsChecked) { ... }
	/// </code>
	/// </example>
	/// <remarks>
	/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner, ScriptOptions.DisplayName (title).
	/// More info: see Show.TaskDialogEx().
	/// </remarks>
	public partial class TaskDialogObject
	{
		#region API

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
			public TDButton dwCommonButtons;
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

		#endregion

		TASKDIALOGCONFIG _c;
		string[] _buttons, _radioButtons; //before showing dialog these will be marshaled to IntPtr

		/// <summary>
		/// Creates new object.
		/// </summary>
		public TaskDialogObject()
		{
			_c.cbSize = Api.SizeOf(_c);
			FlagRtlLayout = Script.Option.dialogRtlLayout;
		}

		/// <summary>
		/// Creates new object and sets commonly used properties.
		/// All parameters are the same as of Show.TaskDialogEx().
		/// </summary>
		public TaskDialogObject(
			Wnd owner, string text1, string text2 = null, TDButton buttons = 0, TDIcon icon = 0, TDFlag flags = 0,
			int defaultButton = 0, StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
			) : this()
		{
			SetOwnerWindow(owner, flags.HasFlag(TDFlag.OwnerCenter));
			SetText(text1, text2);
			SetButtons(buttons);
			SetIcon(icon);

			FlagNoTaskbarButton = flags.HasFlag(TDFlag.NoTaskbarButton);
			FlagEndThread = flags.HasFlag(TDFlag.EndThread);
			FlagNeverActivate = flags.HasFlag(TDFlag.NeverActivate);
			if(flags.HasFlag(TDFlag.Topmost)) FlagTopmost = true; //else use Script.Option.dialogTopmostIfNoOwner if no owner

			SetCustomButtons(customButtons, flags.HasFlag(TDFlag.CommandLinks));
			DefaultButton = defaultButton;
			SetRadioButtons(radioButtons);
			_SetCheckboxFromText(checkBox);
			SetExpandedText(expandedText);
			SetFooterText(footerText);
			SetTitleBarText(title);
			SetXY(x, y, flags.HasFlag(TDFlag.RawXY));
			SetTimeout(timeoutS);
			if(onLinkClick != null) HyperlinkClicked += onLinkClick;
		}

		/// <summary>
		/// Creates new object and sets commonly used properties.
		/// All parameters are the same as of Show.TaskDialogEx().
		/// </summary>
		public TaskDialogObject(
			string text1, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd),
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
			) : this()
		{
			if(style != null) SetStyle(style);
			SetOwnerWindow(owner);
			SetText(text1, text2);
			SetCustomButtons(customButtons);
			SetRadioButtons(radioButtons);
			_SetCheckboxFromText(checkBox);
			SetExpandedText(expandedText);
			SetFooterText(footerText);
			SetTitleBarText(title);
			SetXY(x, y);
			SetTimeout(timeoutS);
			if(onLinkClick != null) HyperlinkClicked += onLinkClick;
		}

		/// <summary>
		/// Changes title bar text.
		/// If you don't call this method or text is null or "", the dialog uses ScriptOptions.DisplayName (default is appdomain name).
		/// </summary>
		public void SetTitleBarText(string text)
		{
			_c.pszWindowTitle = _Util.Title(text);
		}

		/// <summary>
		/// Sets text.
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		public void SetText(string text1, string text2 = null)
		{
			_c.pszMainInstruction = text1;
			_c.pszContent = text2;
		}

		/// <summary>
		/// Sets common icon.
		/// </summary>
		/// <param name="icon">One of four standard icons, eg TDIcon.Info.</param>
		public void SetIcon(TDIcon icon)
		{
			_c.hMainIcon = (IntPtr)(int)icon;
			_USE_HICON_MAIN = false;
		}
		/// <summary>
		/// Sets custom icon.
		/// </summary>
		/// <param name="icon">Icon handle (IntPtr) or object (System.Drawing.Icon).</param>
		public void SetIcon(object icon)
		{
			_c.hMainIcon = _IconHandleFromObject(icon);
			_USE_HICON_MAIN = _c.hMainIcon != Zero;
			//tested: displays original-size 32 and 16 icons, but shrinks bigger icons to 32.
			//note: for App icon Show() will execute more code. The same for footer icon.
		}

		static IntPtr _IconHandleFromObject(object icon)
		{
			IntPtr hi = Zero;
			if(icon != null) {
				if(icon is IntPtr) hi = (IntPtr)icon;
				else if(icon is System.Drawing.Icon) hi = ((System.Drawing.Icon)icon).Handle;
			}
			return hi;
		}

		/// <summary>
		/// Sets common buttons.
		/// </summary>
		/// <param name="buttons">Examples: TDButton.YesNo, TDButton.OK|TDButton.Close.</param>
		public void SetButtons(TDButton buttons)
		{
			_c.dwCommonButtons = buttons;
		}

		/// <summary>
		/// Sets common buttons, icon etc.
		/// You can call this function instead of SetButtons(), SetIcon() etc if you prefer to specify all in string:
		/// <para>Buttons: O OK, C Cancel, Y Yes, N No, R Retry, L Close.</para>
		/// <para>Icon: x error, ! warning, i info, v shield, a app.</para>
		/// <para>Flags: c command links, b no taskbar button, e end thread, n never activate, o owner center, r raw x y, t topmost.</para>
		/// <para>Default button: d before a common button character (eg "OdC") or custom button id (eg "d3").</para>
		/// </summary>
		/// <param name="style">Style string (like "YN!e"), or TDButton (like TDButton.YesNo|TDButton.Retry), or TDIcon (like TDIcon.Info), or TDFlags (like TDFlag.NeverActivate|TDFlag.EndThread), or TDStyle object (like new TDStyle(TDButton.YesNo, TDIcon.Warning, TDFlag.OwnerCenter)).</param>
		public void SetStyle(TDStyle style)
		{
			if(style == null) style = new TDStyle();
			SetButtons(style.buttons);
			DefaultButton = style.defaultButton;
			if(style.customIcon != null) SetIcon(style.customIcon); else SetIcon(style.icon);
			var f = style.flags;
			_USE_COMMAND_LINKS = f.HasFlag(TDFlag.CommandLinks); //SetCustomButtons has a bool? parameter for this
			FlagNoTaskbarButton = f.HasFlag(TDFlag.NoTaskbarButton);
			FlagEndThread = f.HasFlag(TDFlag.EndThread);
			FlagNeverActivate = f.HasFlag(TDFlag.NeverActivate);
			_POSITION_RELATIVE_TO_WINDOW = f.HasFlag(TDFlag.OwnerCenter); //SetOwnerWindow has a bool? parameter for this
			_rawXY = f.HasFlag(TDFlag.RawXY); //SetXY has a bool? parameter for this
			if(f.HasFlag(TDFlag.Topmost)) FlagTopmost = true; //else use Option.dialogTopmostIfNoOwner if no owner
		}

		/// <summary>
		/// Adds custom buttons and sets button style.
		/// </summary>
		/// <param name="buttons">A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel) makes the button work like the common Cancel button (adds title bar x button, enables Esc key).</param>
		/// <param name="asCommandLinks">false - row of classic buttons; true - column of command-link buttons that can have multiline text. If omitted or null, does not change the current value, which may be previously set with SetStyle(), and default is false.</param>
		/// <param name="noCommandLinkIcon">No arrow icon on command-link buttons.</param>
		public void SetCustomButtons(StringList buttons, bool? asCommandLinks = null, bool noCommandLinkIcon = false)
		{
			_buttons = buttons;
			if(asCommandLinks != null) {
				bool cl = asCommandLinks.Value;
				_USE_COMMAND_LINKS = cl && !noCommandLinkIcon;
				_USE_COMMAND_LINKS_NO_ICON = cl && noCommandLinkIcon;
			}
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
		/// <param name="buttons">A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃.</param>
		/// <param name="defaultId">Select (check) this radio button. If omitted or 0, selects the first. If -1, does not select.</param>
		public void SetRadioButtons(StringList buttons, int defaultId = 0)
		{
			_radioButtons = buttons;
			_c.nDefaultRadioButton = defaultId;
			_NO_DEFAULT_RADIO_BUTTON = defaultId < 0;
		}

		/// <summary>
		/// Adds check box (if text is not null/empty).
		/// To get check box state after closing the dialog, use the IsChecked property of the TDResult object returned by Show() or Result.
		/// </summary>
		public void SetCheckbox(string text, bool check = false)
		{
			_c.pszVerificationText = text;
			_VERIFICATION_FLAG_CHECKED = check;
		}

		//Parses "Text|check" etc and calls SetCheckbox.
		void _SetCheckboxFromText(string checkBox)
		{
			string text = null; bool check = false;
			if(!string.IsNullOrEmpty(checkBox)) {
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
			_EXPAND_FOOTER_AREA = showInFooter;
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
			_EXPANDED_BY_DEFAULT = defaultExpanded;
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
			_c.pszFooter = text;
			_c.hFooterIcon = (IntPtr)(int)icon; _USE_HICON_FOOTER = false;
		}
		/// <summary>
		/// Adds text and common icon at the bottom of the dialog.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">One of standard icons, eg TDIcon.Warning.</param>
		public void SetFooterText(string text, TDIcon icon)
		{
			_c.pszFooter = text;
			_c.hFooterIcon = (IntPtr)(int)icon; _USE_HICON_FOOTER = false;
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
			_USE_HICON_FOOTER = _c.hFooterIcon != Zero;
		}

		/// <summary>
		/// Adds Edit or Combo control (if editType is not TDEdit.None (0)).
		/// To get its text after closing the dialog, use the EditText property of the TDResult object returned by Show() or Result.
		/// </summary>
		/// <param name="editType">Control type/style.</param>
		/// <param name="initText">Initial text.</param>
		/// <param name="comboItems">Combo box items, like "one|two|three". Also can be string[] or List˂string˃.</param>
		/// <remarks>
		/// The API TaskDialogIndirect does not have an option to add an edit control. Show() itself creates it.
		/// To reserve space for it, it adds progress bar (hidden under the edit control), therefore you cannot add progress bar.
		/// If TDEdit.Multiline, also adds hidden radio buttons, therefore you cannot add radio buttons.
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
		/// Sets preferred width of the dialog, in dialog units.
		/// If 0 (default), calculates optimal width.
		/// </summary>
		public int Width { set { _c.cxWidth = value; } }

		/// <summary>
		/// Sets owner window.
		/// The owner window will be disabled, and this dialog will be on top of it.
		/// This window will be in owner's screen, if screen was not explicitly specified with the Screen property. Script.Option.dialogScreenIfNoOwner is ignored.
		/// <param name="owner">Owner window.</param>
		/// <param name="ownerCenter">Show the dialog in the center of the owner window. SetXY() and the Screen propert are ignored. If omitted or null, does not change the current value, which may be previously set with SetStyle(), and default is false.</param>
		/// <param name="enableOwner">Don't disable the owner window.</param>
		/// </summary>
		public void SetOwnerWindow(Wnd owner, bool? ownerCenter = null, bool enableOwner = false)
		{
			_c.hwndParent = owner;
			if(ownerCenter != null) _POSITION_RELATIVE_TO_WINDOW = ownerCenter.Value;
			_enableOwner = enableOwner;
		}
		bool _enableOwner;

		/// <summary>
		/// Sets dialog position in screen.
		/// </summary>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="rawXY">Don't interpret 0 and negative x y in a special way. If omitted or null, does not change the current value, which may be previously set with SetStyle(), and default is false.</param>
		public void SetXY(int x, int y, bool? rawXY = null)
		{
			_x = x; _y = y;
			if(rawXY != null) _rawXY = rawXY.Value;
		}

		int _x, _y; bool _rawXY;

		/// <summary>
		/// Sets the screen (display monitor) where to show the dialog in multi-screen environment.
		/// If null or not set, will be used owner window's screen. If no owner, will be used Script.Option.dialogScreenIfNoOwner.
		/// More info: see Screen_.FromObject().
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
		/// If null (default), the dialog will be topmost if both these are true: no owner window, Script.Option.dialogTopmostIfNoOwner is true.
		/// </summary>
		public bool? FlagTopmost { set; private get; }

		/// <summary>
		/// Prevents adding taskbar button.
		/// By default it adds WS_EX_TOOLWINDOW style, which also makes the taskbar smaller and disables FlagCanBeMinimized.
		/// If combined with FlagNeverActivate, it instead adds WS_EX_NOACTIVATE style, which prevents activating when clicked.
		/// </summary>
		public bool FlagNoTaskbarButton { set; private get; }

		/// <summary>
		/// Don't activate the dialog window when it starts.
		/// If combined with FlagNoTaskbarButton, also prevents activating when clicked.
		/// </summary>
		public bool FlagNeverActivate { set; private get; }

		/// <summary>
		/// Call Thread.Abort() if clicked OK when there are no other buttons. Also when clicked Cancel, No, and on timeout.
		/// </summary>
		public bool FlagEndThread { set; private get; }

		bool _USE_HICON_MAIN;
		bool _USE_HICON_FOOTER;
		bool _VERIFICATION_FLAG_CHECKED;
		bool _EXPAND_FOOTER_AREA;
		bool _EXPANDED_BY_DEFAULT;
		bool _USE_COMMAND_LINKS;
		bool _USE_COMMAND_LINKS_NO_ICON;
		bool _NO_DEFAULT_RADIO_BUTTON;
		bool _POSITION_RELATIVE_TO_WINDOW;

		Wnd _dlg;
		int _threadIdInShow;
		bool _lockForegroundWindow;

		/// <summary>
		/// Shows the dialog.
		/// The return value is the same as of Show.TaskDialogEx().
		/// Call this method after setting text and other properties.
		/// </summary>
		public TDResult Show()
		{
			_result = null;
			_isClosed = false;

			int rButton = 0, rRadioButton = 0, rIsChecked = 0, hr = 0;

			if(_c.pszWindowTitle == null) _c.pszWindowTitle = _Util.Title(null);

			_EditControlInitBeforeShowDialog(); //don't reorder, must be before flags

			TDF_ f = 0;
			if(FlagAllowCancel) f |= TDF_.ALLOW_DIALOG_CANCELLATION;
			if(FlagRtlLayout) f |= TDF_.RTL_LAYOUT;
			if(FlagCanBeMinimized) f |= TDF_.CAN_BE_MINIMIZED;
			if(FlagShowProgressBar) f |= TDF_.SHOW_PROGRESS_BAR;
			if(FlagShowMarqueeProgressBar) f |= TDF_.SHOW_MARQUEE_PROGRESS_BAR;
			if(_USE_COMMAND_LINKS) f |= TDF_.USE_COMMAND_LINKS;
			if(_USE_COMMAND_LINKS_NO_ICON) f |= TDF_.USE_COMMAND_LINKS_NO_ICON;
			if(_EXPAND_FOOTER_AREA) f |= TDF_.EXPAND_FOOTER_AREA;
			if(_EXPANDED_BY_DEFAULT) f |= TDF_.EXPANDED_BY_DEFAULT;
			if(_NO_DEFAULT_RADIO_BUTTON) f |= TDF_.NO_DEFAULT_RADIO_BUTTON;
			if(_USE_HICON_MAIN) f |= TDF_.USE_HICON_MAIN;
			if(_USE_HICON_FOOTER) f |= TDF_.USE_HICON_FOOTER;
			if(_VERIFICATION_FLAG_CHECKED) f |= TDF_.VERIFICATION_FLAG_CHECKED;
			if(_POSITION_RELATIVE_TO_WINDOW) f |= TDF_.POSITION_RELATIVE_TO_WINDOW;
			if(HyperlinkClicked != null) f |= TDF_.ENABLE_HYPERLINKS;

			if(_timeoutS > 0 || Timer != null || FlagNeverActivate) f |= TDF_.CALLBACK_TIMER;

			_timeoutActive = false;
			if(_timeoutS > 0) {
				_timeoutActive = true;
				if(!_timeoutNoInfo) {
					_timeoutFooterText = _c.pszFooter;
					_c.pszFooter = _TimeoutFooterText(_timeoutS);
					if(_c.hFooterIcon == Zero) _c.hFooterIcon = (IntPtr)TDIcon.Info;
				}
			}

			_c.dwFlags = f;

			if((_c.hMainIcon == (IntPtr)TDIcon.App || _c.hFooterIcon == (IntPtr)TDIcon.App) && Util.Misc.GetAppIconHandle(32) != Zero)
				_c.hInstance = Util.Misc.GetModuleHandleOfAppdomainEntryAssembly();
			//info: TDIcon.App is IDI_APPLICATION (32512).
			//Although MSDN does not mention that IDI_APPLICATION can be used when hInstance is NULL, it works. Even works for many other undocumented system resource ids, eg 100.
			//Non-NULL hInstance is ignored for the icons specified as TD_x. It is documented and logical.
			//For App icon we could instead use icon handle, but then the small icon for the title bar and taskbar button can be distorted because shrinked from the big icon. Now extracts small icon from resources.
			//More info in Show.MessageDialog().

			_c.pfCallback = _CallbackProc;

			IntPtr hhook = Zero; Api.HOOKPROC hpHolder = null;

			try {
				_threadIdInShow = Thread.CurrentThread.ManagedThreadId;

				_c.pButtons = _MarshalButtons(_buttons, out _c.cButtons, true);
				_c.pRadioButtons = _MarshalButtons(_radioButtons, out _c.cRadioButtons);

				if(_timeoutActive) { //Need mouse/key messages to stop countdown on click or key.
					hhook = Api.SetWindowsHookEx(Api.WH_GETMESSAGE, hpHolder = _HookProc, Zero, Api.GetCurrentThreadId());
				}

				if(_lockForegroundWindow = FlagNeverActivate) Api.LockSetForegroundWindow(Api.LSFW_LOCK);

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
																			  //Out("retry");
					WaitMS(20); //100*20=2000
				}

				if(hr == 0) {
					//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
					rButton = -rButton;
					_result = new TDResult(rButton, rRadioButton, rIsChecked != 0, _editText);

					_Util.DoEventsAndWaitForAnActiveWindow();
				}
			} finally {
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
				case TDResult.OK: endThread = (_c.dwCommonButtons == 0 || _c.dwCommonButtons == TDButton.OK) && _c.pButtons == Zero; break;
				}

				if(endThread) Thread.CurrentThread.Abort();
			}

			return _result;
		}

		int _CallbackProc(Wnd w, TDApi.TDN message, LPARAM wParam, LPARAM lParam, IntPtr data)
		{
			Action<TDEventArgs> e = null;
			int R = 0;

			//Out(message);
			switch(message) {
			case TDApi.TDN.DIALOG_CONSTRUCTED:
				Send = new Sender(w, this); //note: must be before setting _dlg, because another thread may call if(d.IsOpen) d.Send.Message(..).
				_dlg = w;
				break;
			case TDApi.TDN.DESTROYED:
				//Out(w.IsValid); //valid
				e = Destroyed;
				break;
			case TDApi.TDN.CREATED:
				if(_enableOwner) _c.hwndParent.Enabled = true;
				//if(_enableOwner || !_c.hwndParent.IsOfThisThread) _c.hwndParent.Enabled=true; //not sure if it would be useful

				//Add a style to prevent adding taskbar button.
				//With WS_EX_TOOLWINDOW, title bar looks not good.
				//With WS_EX_NOACTIVATE, cannot activate after deactivating, although a click works, and the dialog starts active
				if(FlagNoTaskbarButton) w.SetExStyleAdd(FlagNeverActivate ? Api.WS_EX_NOACTIVATE : Api.WS_EX_TOOLWINDOW); //when combined with the 'never activate' flag, we have a new useful effect

				if(!_POSITION_RELATIVE_TO_WINDOW) {
					object scr = Screen; if(scr == null && _c.hwndParent.Is0) scr = Script.Option.dialogScreenIfNoOwner;
					if((_x != 0 || _y != 0 || _rawXY || scr != null)) {
						w.MoveInScreen(_x, _y, scr, rawXY: _rawXY);
					}
				}

				bool topmost = false;
				if(FlagTopmost != null) topmost = FlagTopmost.Value; else if(_c.hwndParent.Is0) topmost = Script.Option.dialogTopmostIfNoOwner;
				if(topmost) w.ZorderTopmost();

				if(_editType != TDEdit.None) _EditControlCreate();

				e = Created;
				break;
			case TDApi.TDN.TIMER:
				if(_timeoutActive) {
					int timeElapsed = wParam / 1000;
					if(timeElapsed < _timeoutS) {
						if(!_timeoutNoInfo) Send.SetText(true, TDApi.TDE.FOOTER, _TimeoutFooterText(_timeoutS - timeElapsed - 1));
					} else {
						_timeoutActive = false;
						Send.Close(TDResult.Timeout);
					}
				}

				if(_lockForegroundWindow) {
					_lockForegroundWindow = false;
					Api.LockSetForegroundWindow(Api.LSFW_UNLOCK);
					w.FlashStop();
					//info: cannot use Api.HCBT_ACTIVATE, does not work. Disables activating this window, but another window is deactivated anyway.
				}

				e = Timer;
				break;
			case TDApi.TDN.BUTTON_CLICKED: e = ButtonClicked; wParam = -wParam; break; //info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
			case TDApi.TDN.HYPERLINK_CLICKED: e = HyperlinkClicked; break;
			case TDApi.TDN.HELP: e = HelpF1; break;
			default: e = OtherEvents; break;
			}

			if(e != null) {
				var ed = new TDEventArgs(this, _dlg, message, wParam, lParam);
				e(ed);
				R = ed.returnValue;
			}

			if(message == TDApi.TDN.DESTROYED) _SetClosed();

			return R;
		}

		/// <summary>
		/// TaskDialogObject events.
		/// Raised when the internal TaskDialogCallbackProc function (see in MSDN) is called by the task dialog API.
		/// </summary>
		public event Action<TDEventArgs>
			Created, Destroyed, Timer, ButtonClicked, HyperlinkClicked, HelpF1, OtherEvents;
	}

	/// <summary>
	/// Arguments for event handlers of TaskDialogObject, TaskDialogEx() (onLinkClick) and some other functions.
	/// More info: MSDN -> TaskDialogCallbackProc function.
	/// To return a non-zero value from the callback function, assign the value to the returnValue field.
	/// </summary>
	public class TDEventArgs :EventArgs
	{
		internal TDEventArgs(TaskDialogObject obj_, Wnd hwnd_, TDApi.TDN message_, LPARAM wParam_, LPARAM lParam_)
		{
			obj = obj_; hwnd = hwnd_; message = message_; wParam = wParam_;
			linkHref = (message_ == TDApi.TDN.HYPERLINK_CLICKED) ? Marshal.PtrToStringUni(lParam_) : null;
		}
		public TaskDialogObject obj;
		public Wnd hwnd; //not a property because then handler could not do ed.Hwnd.Property=value. Also cannot be readonly.
		public TDApi.TDN message;
		public LPARAM wParam;
		public string linkHref;
		public int returnValue;
	}

	public partial class TaskDialogObject
	{
		#region async_etc

		/// <summary>
		/// Shows the dialog in another thread and waits a while until the dialog is open.
		/// Calls <c>Task.Run(() =˃ { Show(); }); ThreadWaitOpen();</c>
		/// </summary>
		/// <param name="whenClosed">An event handler function (lambda etc) to call when the dialog is closed. It will be called in other thread too.</param>
		public void ShowAsync(Action<TDResult> whenClosed = null)
		{
			Task.Run(() =>
			{
				Show();
				if(whenClosed != null) whenClosed(Result);
				//note: currently onClose not called if main thread is already returned at that time.
				//	Because then called AppDomain.Unload, which aborts thread, which aborts executing managed code.
				//	In the future before calling AppDomain.Unload should close all thread windows, or wait for windows marked 'fait for it on script exit'.
			});
			if(!ThreadWaitOpen()) throw new CatkeysException();
		}

		/// <summary>
		/// Gets clicked button id and other results packed in a TDResult object.
		/// It is the same object as the Show() return value.
		/// If the result is still unavailable (eg the dialog still not closed):
		///		If called from the same thread that called Show(), returns null.
		///		If called from another thread, waits until the dialog is closed and the return value is available.
		///		Note that ShowAsync() calls Show() in another thread.
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
				//Out("waiting for result");
				while(_threadIdInShow != 0) WaitMS(20);
			}
			return true;
		}

		/// <summary>
		/// Can be used by other threads to wait until the dialog is open.
		/// If returns true, the dialog is open and you can send messages to it.
		/// If returns false, the dialog is already closed or failed to show.
		/// Example: <c>var d = new TaskDialogObject("text"); Task.Run(() => { d.Show(); }); if(!d.ThreadWaitOpen()) return;</c>
		/// </summary>
		public bool ThreadWaitOpen()
		{
			_AssertIsOtherThread();
			while(!IsOpen) {
				if(_isClosed) return false;
				WaitMS(20); //need 2 loops if 20, or 3-4 if 10
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
				throw new CatkeysException("wrong thread");
		}

		/// <summary>
		/// Returns true if the dialog is open and you can send messages to it.
		/// </summary>
		public bool IsOpen { get { return !_dlg.Is0; } }

		void _SetClosed()
		{
			_isClosed = true;
			_dlg = Wnd0;
			Send = new Sender(); //clear it
		}
		bool _isClosed;

		#endregion

		/// <summary>
		/// Gets dialog window handle as Wnd.
		/// Returns Wnd0 if the dialog is not open.
		/// </summary>
		public Wnd DialogWindow { get { return _dlg; } }

		public Sender Send { get; private set; }

		[DebuggerStepThrough]
		public struct Sender
		{
			Wnd _dlg;
			TaskDialogObject _tdo; //used only to get _tdo._c for TDApi.TDM.NAVIGATE_PAGE. Using _tdo._hdlg would be unsafe in multithreaded context because may be already set to null, even if caller called IsOpen before.
			internal Sender(Wnd dlg, TaskDialogObject tdo) { _dlg = dlg; _tdo = tdo; }
			//internal void SetObj(TaskDialogObject tdo) { _tdo=tdo; } //does not work because the Send property returns a copy of our Sender

			//Don't use this, to avoid unnecessary warnings in certain multi-threaded situations.
			//bool _IsOpen()
			//{
			//	//if(_tdo == null) throw new Exception("the dialog must be open");
			//	if(_tdo != null) return true;
			//	Output.Warning("incorrect Send usage. The dialog is still not created, or already closed.", 1);
			//	return false;
			//}

			/// <summary>
			/// Sends a message to the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example (in an event handler): <c>ed.obj.Send.Message(TDApi.TDM.CLICK_VERIFICATION, 1);</c>
			/// Also there are several other functions to send some messages.
			/// Message reference: MSDN.
			/// </summary>
			public int Message(TDApi.TDM message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
			{
				//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
				switch(message) { case TDApi.TDM.CLICK_BUTTON: case TDApi.TDM.ENABLE_BUTTON: case TDApi.TDM.SET_BUTTON_ELEVATION_REQUIRED_STATE: wParam = -wParam; break; }

				//if(!_IsOpen()) return 0;
				return _dlg.Send((uint)message, wParam, lParam);
			}

			/// <summary>
			/// Sends message TDApi.TDM.SET_ELEMENT_TEXT or TDApi.TDM.UPDATE_ELEMENT_TEXT to the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example (in an event handler): <c>ed.obj.Send.Text(true, TDApi.TDE.CONTENT, "New text.");</c>
			/// </summary>
			public void SetText(bool updateText, TDApi.TDE partId, string text)
			{
				//if(!_IsOpen()) return;
				_dlg.SendS((uint)(updateText ? TDApi.TDM.UPDATE_ELEMENT_TEXT : TDApi.TDM.SET_ELEMENT_TEXT), (int)partId, text ?? "");
				//info: null does not change text.

				if(_tdo != null) _tdo._EditControlUpdateAsync(updateText);
				//info: sometimes even UPDATE_ELEMENT_TEXT sends our control to the bottom of the Z order.
			}

			/// <summary>
			/// Applies new properties to the dialog while it is already open.
			/// Can be used for example to create wizard-like dialog with custom buttons "Next" and "Back".
			/// Call this method while the dialog is open, eg in an event handler, after setting new properties.
			/// Sends message TDApi.TDM.NAVIGATE_PAGE.
			/// </summary>
			public void NavigatePage()
			{
				//if(!_IsOpen()) return;
				TaskDialogObject o = _tdo; if(o == null) return;
				_ApiSendMessageTASKDIALOGCONFIG(_dlg, (uint)TDApi.TDM.NAVIGATE_PAGE, 0, ref o._c);
			}

			[DllImport("user32.dll", EntryPoint = "SendMessageW")]
			static extern LPARAM _ApiSendMessageTASKDIALOGCONFIG(Wnd hWnd, uint msg, LPARAM wParam, [In] ref TASKDIALOGCONFIG c);

			/// <summary>
			/// Clicks a button. Normally it closes the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Sends message TDApi.TDM.CLICK_BUTTON.
			/// </summary>
			/// <param name="buttonId">A button id or some other number that will be returned in Show() results. Default: TDResult.Close.</param>
			public bool Close(int buttonId = TDResult.Close)
			{
				//if(!_IsOpen()) return false;
				return 0 != Message(TDApi.TDM.CLICK_BUTTON, buttonId);
			}

			/// <summary>
			/// Enables or disables a button. Normally it closes the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example: <c>d.Created += ed => { ed.obj.Send.EnableButton(TDResult.Yes, false); };</c>
			/// Sends message TDApi.TDM.ENABLE_BUTTON.
			/// </summary>
			public void EnableButton(int buttonId, bool enable)
			{
				//if(!_IsOpen()) return;
				Message(TDApi.TDM.ENABLE_BUTTON, buttonId, enable);
			}

			/// <summary>
			/// Sets progress bar value, 0 to 100.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Sends message TDApi.TDM.SET_PROGRESS_BAR_POS.
			/// </summary>
			public int Progress(int percent)
			{
				//if(!_IsOpen()) return 0;
				return Message(TDApi.TDM.SET_PROGRESS_BAR_POS, percent);
			}
		} //class Sender

		#region marshalButtons_hookProc_timeoutText
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
			} catch(Exception e) { throw new Win32Exception($"_CallTDI: {e.Message}"); } //note: not just throw;, and don't add inner exception
		}
#endif

		static IntPtr _MarshalButtons(string[] a, out int cButtons, bool escapeId = false)
		{
			if(a == null || a.Length == 0) { cButtons = 0; return Zero; }
			cButtons = a.Length;

			int structSize = Marshal.SizeOf(typeof(TASKDIALOG_BUTTON));
			IntPtr R = Marshal.AllocHGlobal(structSize * a.Length); //TODO: don't use Marshal Alloc/Free, because in other place it was slow etc

			for(int i = 0; i < a.Length; i++) {
				TASKDIALOG_BUTTON b; b.id = a[i].ToInt_(out b.text); //"id text" -> TASKDIALOG_BUTTON

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
			if(code >= 0 && _HookProc(code, wParam, ref *(Api.MSG*)lParam)) return 1;

			return Api.CallNextHookEx(Zero, code, wParam, lParam);
		}

		bool _HookProc(int code, LPARAM wParam, ref Api.MSG m)
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
					Send.SetText(true, TDApi.TDE.FOOTER, _timeoutFooterText);
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

		//Don't use this because changing dialog size is not good.
		//void _TimeoutFooterTextHide()
		//{
		//	if(_timeoutNoInfo) return;
		//	if(Empty(_timeoutFooterText)) {
		//		_dlg.Send((uint)TDApi.TDM.UPDATE_ICON, (int)TDApi.TDIE.ICON_FOOTER, 0);
		//		Send.Text(false, TDApi.TDE.FOOTER, ""); //however still remains some space for footer

		//		//_c.pszFooter=null; SendNavigatePage(); //don't use this because interferes with the expand/collapse control
		//	} else Send.Text(false, TDApi.TDE.FOOTER, _timeoutFooterText);
		//}
		#endregion

		#region Edit_control

		void _EditControlInitBeforeShowDialog()
		{
			if(_editType == TDEdit.None) return;
			FlagShowMarqueeProgressBar = true;
			FlagShowProgressBar = false;
			if(_editType == TDEdit.Multiline) _radioButtons = new string[] { ".1", ".2", ".3", ".4" }; //reserve space for multiline Edit control
		}

		void _EditControlUpdate(bool onlyZorder = false)
		{
			if(_editWnd.Is0) return;
			if(!onlyZorder) {
				RECT r;
				_EditControlGetPlace(out r);
				_editParent._MoveResize(r.left, r.top, r.Width, r.Height);
				_editWnd._MoveResize(0, 0, r.Width, r.Height);
			}
			_editParent.ZorderTop();
		}

		void _EditControlUpdateAsync(bool onlyZorder = false)
		{
			_editParent.Post(Api.WM_APP + 111, onlyZorder);
		}

		Wnd _EditControlGetPlace(out RECT r)
		{
			Wnd parent = _dlg; //don't use the DirectUIHWND control for it, it can create problems

			//We'll hide the progress bar control and create our Edit control in its place.
			Wnd prog = parent.ChildByClassName("msctls_progress32");
			prog.Visible = false;

			prog.GetRectInClientOf(parent, out r);
			r.Inflate(0, (Form.DefaultFont.Height + 9 - r.Height) / 2);
			if(_editComboHeight != 0) r.Height = _editComboHeight; //combo height, remembered when creating, used when updating

			if(_editType == TDEdit.Multiline) {
				//as bottom, use the last radio button
				Wnd rLast = Wnd0;
				parent.ChildAllRaw(e =>
				{
					var s = e.w.Name;
					if(s.Length==2 && s[0]=='.' && s[1]>='1' && s[1]<='4') {
						e.w.Visible = false;
						//e.w.Enabled = false; //hiding and disabling does not exclude the radio button from Tab
						if(s[1] == '4') {
							rLast = e.w;
							e.Stop();
						}
					}
				}, "Button");

				RECT u; rLast.GetRectInClientOf(parent, out u);
				r.bottom = u.bottom;
			}

			return parent;
		}

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
			_editWnd.Font = Form.DefaultFont.ToHfont();
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
				_editParent._Resize(cbr.Width, _editComboHeight = cbr.Height); //because ComboBox resizes itself, usually makes bigger
			} else {
				_editWnd.SetControlText(_editText);
				_editWnd.Send(Api.EM_SETSEL, 0, -1);
			}
			_editParent.ZorderTop();
			_editWnd.FocusControlOfThisThread();

			//Set events.
			ButtonClicked += ed => _editText = _editWnd.GetControlText();
			OtherEvents += ed =>
			{
				switch(ed.message) {
				case TDApi.TDN.EXPANDO_BUTTON_CLICKED:
				case TDApi.TDN.NAVIGATED:
					_EditControlUpdateAsync();
					break;
				}
			};
		}

		public Wnd EditControl { get { return _editWnd; } }
		Wnd _editWnd, _editParent;
		int _editComboHeight;

		//Dlgproc of our intermediate #32770 control, the parent of out Edit control.
		int _EditControlParentProc(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
		{
			//OutList(msg, wParam, lParam);
			switch(msg) {
			case Api.WM_SETFOCUS: //enables Tab when in single-line Edit control
				_dlg.ChildByClassName("DirectUIHWND", true).FocusControlOfThisThread();
				return 1;
			case Api.WM_NEXTDLGCTL: //enables Tab when in multi-line Edit control
				_dlg.ChildByClassName("DirectUIHWND", true).FocusControlOfThisThread();
				return 1;
			case Api.WM_CLOSE: //enables Esc when in edit control
				_dlg.Send(msg);
				return 1;
			case Api.WM_APP + 111: //async update edit control pos
				_EditControlUpdate(wParam != 0);
				return 1;
			}
			return 0;
			//tested: WM_GETDLGCODE, no results.
		}
		Api.DLGPROC _EditControlParentProcHolder;

		#endregion
	} //TaskDialogObject
	#endregion TaskDialog

	#region InputDialog

	public static partial class Show
	{
		/// <summary>
		/// Shows dialog with a text edit field, and gets that text as string.
		/// Also has a checkbox, and get its checked state.
		/// Returns true on OK, false on Cancel.
		/// </summary>
		/// <param name="s">Variable that receives text.</param>
		/// <param name="isChecked">Variable that receives checkbox state.</param>
		/// <param name="checkBoxText">Checkbox text.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial Edit control text. If editType == TDEdit.Combo, the first line sets Edit control text, other lines add drop-down list items.</param>
		/// <param name="owner">Owner window.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <param name="style">The same as with Show.TaskDialogEx(). Can be specified flags and icon, but not buttons (the dialog always has OK and Cancel).</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="customButtons">Adds buttons that have custom text and id. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃. In the 'style' parameter you can change button style.</param>
		/// <param name="onButtonClick">
		/// A button-clicked event handler function, eg lambda.
		/// For example, it can set edit control text when a custom button clicked. Example (lambda):
		/// <c>ed => { if(ed.wParam == 1) { string _s; if(Show.InputDialog(out _s, owner:ed.hwnd)) ed.obj.EditControl.SetControlText(_s); ed.returnValue = 1; } }</c>
		/// Or on OK click it can verify edit control text and don't allow to close the dialog if the text is invalid. Example (lambda):
		/// <c>ed => { if(ed.wParam == TDResult.OK) { string _s=ed.obj.EditControl.Name; if(Empty(_s)) { Show.TaskDialog("Text cannot be empty.", owner: ed.hwnd); ed.returnValue = 1; } } }</c>
		/// </param>
		/// <param name="onLinkClick">Enables hyperlinks in text. A link-clicked event handler function, eg lambda. Example: see Show.TaskDialogEx().</param>
		/// <remarks>
		/// Uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static bool InputDialogEx(
			out string s, out bool isChecked, string checkBoxText, string staticText = null,
			string initText = null, Wnd owner = default(Wnd), TDEdit editType = TDEdit.Text, TDStyle style = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			StringList customButtons = null, Action<TDEventArgs> onButtonClick = null,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			s = null;
			isChecked = false;

			var d = new TaskDialogObject(null, staticText, style, owner, customButtons, null,
				checkBoxText, expandedText, footerText, title, x, y, timeoutS, onLinkClick);

			d.SetButtons(TDButton.OKCancel);

			if(editType == TDEdit.None) editType = TDEdit.Text;
			d.SetEditControl(editType, initText);

			if(onButtonClick != null) d.ButtonClicked += onButtonClick;

			TDResult r = d.Show();
			if(r != TDResult.OK) return false;

			s = r.EditText;
			isChecked = r.IsChecked;
			return true;
		}

		/// <summary>
		/// Shows dialog with a text edit field, and gets that text as string.
		/// Also has a checkbox, and get its checked state.
		/// Returns true on OK, false on Cancel.
		/// </summary>
		/// <param name="s">Variable that receives text.</param>
		/// <param name="isChecked">Variable that receives checkbox state.</param>
		/// <param name="checkBoxText">Checkbox text.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial Edit control text.</param>
		/// <param name="owner">Owner window.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <remarks>
		/// Calls InputDialogEx, which uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static bool InputDialog(
			out string s, out bool isChecked, string checkBoxText, string staticText = null,
			string initText = null, Wnd owner = default(Wnd), TDEdit editType = TDEdit.Text)
		{
			return InputDialogEx(out s, out isChecked, checkBoxText, staticText, initText, owner, editType);
		}

		/// <summary>
		/// Shows dialog with a text edit field, and gets that text as int.
		/// Also has a checkbox, and get its checked state.
		/// Returns true on OK, false on Cancel.
		/// </summary>
		/// <param name="i">Variable that receives the value.</param>
		/// <param name="isChecked">Variable that receives checkbox state.</param>
		/// <param name="checkBoxText">Checkbox text.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial Edit control text.</param>
		/// <param name="owner">Owner window.</param>
		/// <param name="editType">Edit field type. It can be number (TDEdit.Number, default), simple text or combo box.</param>
		/// <remarks>
		/// Calls InputDialogEx, which uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static bool InputDialog(
			out int i, out bool isChecked, string checkBoxText, string staticText = null,
			string initText = null, Wnd owner = default(Wnd), TDEdit editType = TDEdit.Number)
		{
			i = 0;
			string s;
			if(!InputDialogEx(out s, out isChecked, checkBoxText, staticText, initText, owner, editType)) return false;
			i = s.ToInt_();
			return true;
		}

		/// <summary>
		/// Shows dialog with a text edit field, and gets that text as string.
		/// Returns true on OK, false on Cancel.
		/// </summary>
		/// <param name="s">Variable that receives text.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial Edit control text. If editType == TDEdit.Combo, the first line sets Edit control text, other lines add drop-down list items.</param>
		/// <param name="owner">Owner window.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <param name="style">The same as with Show.TaskDialogEx(). Can be specified flags and icon, but not buttons (the dialog always has OK and Cancel).</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="customButtons">Adds buttons that have custom text and id. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three". Also can be string[] or List˂string˃. In the 'style' parameter you can change button style.</param>
		/// <param name="onButtonClick">
		/// A button-clicked event handler function, eg lambda.
		/// For example, it can set edit control text when a custom button clicked. Example (lambda):
		/// <c>ed => { if(ed.wParam == 1) { string _s; if(Show.InputDialog(out _s, owner:ed.hwnd)) ed.obj.EditControl.SetControlText(_s); ed.returnValue = 1; } }</c>
		/// Or on OK click it can verify edit control text and don't allow to close the dialog if the text is invalid. Example (lambda):
		/// <c>ed => { if(ed.wParam == TDResult.OK) { string _s=ed.obj.EditControl.Name; if(Empty(_s)) { Show.TaskDialog("Text cannot be empty.", owner: ed.hwnd); ed.returnValue = 1; } } }</c>
		/// </param>
		/// <param name="onLinkClick">Enables hyperlinks in text. A link-clicked event handler function, eg lambda. Example: see Show.TaskDialogEx().</param>
		/// <remarks>
		/// Uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static bool InputDialogEx(
			out string s, string staticText = null,
			string initText = null, Wnd owner = default(Wnd), TDEdit editType = TDEdit.Text, TDStyle style = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			StringList customButtons = null, Action<TDEventArgs> onButtonClick = null,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			bool isChecked;
			return InputDialogEx(out s, out isChecked, null, staticText, initText, owner, editType, style,
				expandedText, footerText, title, x, y, timeoutS, customButtons, onButtonClick, onLinkClick);
		}

		/// <summary>
		/// Shows dialog with a text edit field, and gets that text as string.
		/// Returns true on OK, false on Cancel.
		/// </summary>
		/// <param name="s">Variable that receives text.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial Edit control text.</param>
		/// <param name="owner">Owner window.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <remarks>
		/// Calls InputDialogEx, which uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static bool InputDialog(out string s, string staticText = null, string initText = null, Wnd owner = default(Wnd), TDEdit editType = TDEdit.Text)
		{
			return InputDialogEx(out s, staticText, initText, owner, editType);
		}

		/// <summary>
		/// Shows dialog with a text edit field, and gets that text as int.
		/// Returns true on OK, false on Cancel.
		/// </summary>
		/// <param name="i">Variable that receives the number.</param>
		/// <param name="staticText">Read-only text above the edit field.</param>
		/// <param name="initText">Initial Edit control text.</param>
		/// <param name="owner">Owner window.</param>
		/// <param name="editType">Edit field type. It can be number (TDEdit.Number, default), simple text or combo box.</param>
		/// <remarks>
		/// Calls InputDialogEx, which uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static bool InputDialog(out int i, string staticText = null, int initValue = 0, Wnd owner = default(Wnd), TDEdit editType = TDEdit.Number)
		{
			i = 0;
			string s;
			if(!InputDialogEx(out s, staticText, initValue.ToString(), owner, editType)) return false;
			i = s.ToInt_();
			return true;
		}

#if use_vb_inputbox
		public static string InputDialogVB(string staticText, string defaultValue = null, string title = null, int x = 0, int y = 0)
		{
			if(staticText == null) staticText = "";
			if(defaultValue == null) defaultValue = "";
			title = _Util.Title(title);

			var p = new RECT(x, y, 400, 180, true);
			Wnd.RectMoveInScreen(ref p);

			string r = Microsoft.VisualBasic.Interaction.InputBox(staticText, title, defaultValue, p.left, p.top);
			//if(Empty(r)) return null;
			return r;

			//problems:
			//	Cannot set owner window.
			//	Cannot set topmost.
			//	Cannot set multiline or password.
			//	Does not indicate whether OK or Cancel clicked.
			//	Could be useful: checkbox, timeout, flags (topmost, owner center, end thread), etc.
			//possible problems:
			//	MSDN says that x y must be in twips. But here pixels work well.
			//	MSDN says that requires UIPermission at the SafeTopLevelWindows level, which may affect its execution in partial-trust situations.
		}
#endif
	}
	#endregion InputDialog

	#region ListDialog
	public static partial class Show
	{
		/// <summary>
		/// Shows task dialog with a list (column) of command-link buttons.
		/// Returns clicked button id and other results packed in a TDResult object (if assigned to an int variable or switch, it is button id). On Cancel/X click the returned id is 0 (not TDResult.Cancel, which is -2).
		/// </summary>
		/// <param name="list">List items (buttons). A list of strings "id text" separated by |, like "1 One|2 Two|3 Three|Cancel". Also can be string[] or List˂string˃. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel).</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="style">The same as with Show.TaskDialogEx(). Example: "Co" (adds Cancel button and owner-center flag). Example: TDButton.Cancel.</param>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="checkBox">Check box text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A link-clicked event handler function, eg lambda. Example: <c>Show.TaskDialogEx("", "Text ˂a href="example"˃link˂/a˃.", onLinkClick: ed => { Out(ed.linkHref); });</c></param>
		/// <remarks>
		/// Uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static TDResult ListDialogEx(
			StringList list, string text1 = null, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd),
			string checkBox = null, string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialogObject(text1, text2, style, owner, null, null, checkBox, null, footerText, title, x, y, timeoutS, onLinkClick);
			d.SetCustomButtons(list, true);
			d.FlagAllowCancel = true;
			d.SetExpandedText(expandedText, true);
			TDResult R = d.Show();
			if(R.Button == TDResult.Cancel) R.Button = 0;
			return R;
		}

		/// <summary>
		/// Shows task dialog with a list (column) of command-link buttons.
		/// Returns clicked button id. On Cancel/X click returns 0 (not TDResult.Cancel, which is -2).
		/// </summary>
		/// <param name="list">List items (buttons). A list of strings "id text" separated by |, like "1 One|2 Two|3 Three|Cancel". Also can be string[] or List˂string˃. You can also use common button ids, which are negative, for example id -2 (TDResult.Cancel).</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="style">The same as with Show.TaskDialogEx(). Example: "Co" (adds Cancel button and owner-center flag). Example: TDButton.Cancel.</param>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <remarks>
		/// Calls ListDialogEx, which uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static int ListDialog(StringList list, string text1 = null, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd))
		{
			return ListDialogEx(list, text1, text2, style, owner);
		}
	}
	#endregion

	#region ProgressDialog

	public static partial class Show
	{
		/// <summary>
		/// Shows dialog with progress bar.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialogObject object that can be used to control the dialog: set progress bar position, update text, close etc.
		/// All parameters are the same as with Show.TaskDialogEx(). If no buttons specified, adds Cancel.
		/// </summary>
		/// <param name="marquee">Let the progress bar animate without indicating a percent of work done.</param>
		/// <example>
		/// <code>
		/// var pd = Show.ProgressDialogEx(false, "Working", customButtons: "1 Stop", y: -1);
		/// for(int i = 1; i != 100; i++) {
		/// 	if(!pd.IsOpen) { Out(pd.Result); break; } //if the user closed the dialog
		/// 	pd.Send.Progress(i); //don't need this if marquee
		/// 	WaitMS(50); //do something in the loop
		/// }
		/// pd.Send.Close();
		/// </code>
		/// </example>
		/// <remarks>
		/// Uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static TaskDialogObject ProgressDialogEx(bool marquee,
			string text1, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd),
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
		)
		{
			//If no buttons specified, add Cancel instead of OK.
			if((style == null || style.buttons == 0) && customButtons == null) {
				if(style == null) style = new TDStyle(TDButton.Cancel);
				else style.buttons = TDButton.Cancel;
			}

			var d = new TaskDialogObject(text1, text2, style, owner,
				customButtons, radioButtons, checkBox, expandedText, footerText, title, x, y, timeoutS, onLinkClick);

			if(marquee) d.FlagShowMarqueeProgressBar = true;
			else d.FlagShowProgressBar = true;

			d.ShowAsync();

			if(marquee) d.Send.Message(TDApi.TDM.SET_PROGRESS_BAR_MARQUEE, true);

			return d;
		}

		/// <summary>
		/// Shows dialog with progress bar.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialogObject object that can be used to control the dialog: set progress bar position, close etc.
		/// All parameters except marquee are the same as with Show.TaskDialogEx().
		/// </summary>
		/// <param name="marquee">Let the progress bar animate without indicating a percent of work done.</param>
		/// <example>
		/// <code>
		/// var pd = Show.ProgressDialog(false, "Working", customButtons: "1 Stop", y: -1);
		/// for(int i = 1; i != 100; i++) {
		/// 	if(!pd.IsOpen) { Out(pd.Result); break; } //if the user closed the dialog
		/// 	pd.Send.Progress(i); //don't need this if marquee
		/// 	WaitMS(50); //do something in the loop
		/// }
		/// pd.Send.Close();
		/// </code>
		/// </example>
		/// <remarks>
		/// Calls ProgressDialogEx, which uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static TaskDialogObject ProgressDialog(bool marquee,
			string text1, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd),
			StringList customButtons = null, int x = 0, int y = 0)
		{
			return ProgressDialogEx(marquee, text1, text2, style, owner, customButtons, x: x, y: y);
		}
	}

	#endregion

	#region TaskDialogNoWait

	public static partial class Show
	{
		/// <summary>
		/// Shows task dialog like Show.TaskDialogEx() but does not wait.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialogObject object that can be used to control the dialog, eg close.
		/// All parameters except whenClosed are the same as with Show.TaskDialogEx().
		/// </summary>
		/// <param name="whenClosed">null or an event handler function (lambda etc) to call when the dialog is closed. It will be called in other thread too.</param>
		/// <example><code>
		/// //Show.TaskDialogNoWaitEx(null, "Text."); //simplest example
		/// var td = Show.TaskDialogNoWaitEx(ed => { Out(ed); }, "Text.", style: "OCi");
		/// Wait(3); //do something while the dialog is open in other thread
		/// td.ThreadWaitClosed(); //wait until dialog closed (optional, but if the main thread will exit before closing the dialog, dialog's thread then will be aborted)
		/// </code></example>
		/// <remarks>
		/// Uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static TaskDialogObject TaskDialogNoWaitEx(Action<TDResult> whenClosed,
			string text1, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd),
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null,
			int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialogObject(text1, text2, style, owner,
				customButtons, radioButtons, checkBox, expandedText, footerText, title, x, y, timeoutS, onLinkClick);
			d.ShowAsync(whenClosed);
			return d;
		}

		/// <summary>
		/// Shows task dialog like Show.TaskDialog() but does not wait.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialogObject object that can be used to control the dialog, eg close.
		/// All parameters except whenClosed are the same as with Show.TaskDialog().
		/// </summary>
		/// <param name="whenClosed">null or an event handler function (lambda etc) to call when the dialog is closed. It will be called in other thread too.</param>
		/// <example><code>
		/// //Show.TaskDialogNoWait(null, "Text."); //simplest example
		/// var td = Show.TaskDialogNoWait(ed => { Out(ed); }, "Text.", style: "OCi");
		/// Wait(3); //do something while the dialog is open in other thread
		/// td.ThreadWaitClosed(); //wait until dialog closed (optional, but if the main thread will exit before closing the dialog, dialog's thread then will be aborted)
		/// </code></example>
		/// <remarks>
		/// Calls TaskDialogNoWaitEx, which uses TaskDialogObject class, like Show.TaskDialogEx(). More info there.
		/// </remarks>
		public static TaskDialogObject TaskDialogNoWait(Action<TDResult> whenClosed,
			string text1, string text2 = null, TDStyle style = null, Wnd owner = default(Wnd), StringList customButtons = null
			)
		{
			return TaskDialogNoWaitEx(whenClosed, text1, text2, style, owner, customButtons);
		}
	}

	#endregion

	#region OutputWindow

	//public static partial class Show
	//{
	//	/// <summary>
	//	/// Shows a scrolling read-only text window where you then can write text (append lines) like in a console window.
	//	/// Just creates the window and returns. The window runs in another thread.
	//	/// </summary>
	//	public class OutputWindow
	//	{

	//	}
	//}

	#endregion OutputWindow

	#region Show util

	internal static class _Util
	{
		internal static string Title(string title) { return string.IsNullOrEmpty(title) ? ScriptOptions.DisplayName : title; }
		//info: IsNullOrEmpty because if "", API TaskDialog uses "ProcessName.exe".

		//Call this after showing a dialog API (eg MessageBoxIndirect).
		//In my previous experience, in a thread that does not process messages, after closing a dialog may be not updated key states.
		//Processing remaining unprocessed messages fixes it. It is what this func does.
		//In my tests, it fixed even the no-active-window-briefly-after-a-dialog problem, although probably it is not reliable.
		//It spends about 250 mcs, an it is enough for the system to activate a window. Even Thread.Sleep() often works.
		//This func also waits for an active window, max 500 ms. Processes messages while waiting.
		internal static void DoEventsAndWaitForAnActiveWindow()
		{
			for(int i = 0; i < 50; i++) {
				Application.DoEvents();

				////It seems that with this behaves the same:
				//Api.MSG m;
				//while(Api.PeekMessageW(out m, Wnd0, 0, 0, Api.PM_REMOVE)) {
				//	Out(m.message);
				//	Api.TranslateMessage(ref m);
				//	Api.DispatchMessage(ref m);
				//}

				if(!Wnd.ActiveWindow.Is0) break;
				WaitMS(10);
			}
		}


	}

	#endregion util
}
