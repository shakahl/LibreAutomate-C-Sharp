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
using Catkeys.Util;
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
			p.lpszCaption = _Title(title);
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
					if(Resources.AppIconHandle32 != Zero) p.hInstance = Misc.GetModuleHandleOfAppdomainEntryAssembly();
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
			//consider: play user-defined sound, eg default "meow".

			p.hwndOwner = owner;

			flags &= ~(MDFlag.NoSound); //not API flags
			p.dwStyle |= (uint)buttons | (uint)icon | (uint)flags;

			int R = MessageBoxIndirect(ref p);
			if(R == 0) throw new CatkeysException();

			//DoEvents(); //process messages, or later something may not work //TODO
			//WaitForAnActiveWindow(500, 2); //TODO
			Debug.Assert(Wnd.ActiveWindow != Wnd0);

			return (MDResult)R;

			//tested:
			//user32:MessageBoxTimeout. Undocumented. Too limited etc to be useful. If need timeout, use TaskDialog.
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

		[DllImport("user32.dll")]
		static extern int MessageBoxIndirect([In] ref MSGBOXPARAMS lpMsgBoxParams);

	}
	#endregion MessageDialog

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
		Topmost = 32, //t
	}

	public delegate void TDAsyncCloseDelegate(TDResult r);

	/// <summary>
	/// TaskDialog() return values (selected button, radio button, is checked).
	/// </summary>
	public class TDResult
	{
		public TDResult(int button, int radioButton = 0, bool isChecked = false)
		{
			Button = button; RadioButton = radioButton; IsChecked = isChecked;
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
		/// Converts TDResult to int.
		/// Allows to use code <c>switch(TaskDialog(...))</c> instead of <c>switch(TaskDialog(...).Button)</c>.
		/// </summary>
		/// <param name="r"></param>
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
		/// <param name="flags">Example: TDFlag.CommandLinks|TDFlag.NeverActivate.</param>
		/// <param name="defaultButton">Specifies which button id to return on Enter key. Can be one of common buttons (eg TDResult.OK) or a custom button id (eg 1). If omitted or 0, auto-selects.</param>
		/// <param name="customButtons">List of strings "id text" separated by |, like <c>"1 One|2 Two|3 Three"</c>. Or array or List, like <c>new string[]{"1 One", "2 Two", "3 Three"}</c>. Use TDFlag.CommandLinks in flags to change button style.</param>
		/// <param name="radioButtons">List of strings "id text" separated by |, like <c>"1 One|2 Two|3 Three"</c>. Or array or List, like <c>new string[]{"1 One", "2 Two", "3 Three"}</c>.</param>
		/// <param name="checkBox">Check box text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A lambda or other kind of delegate callback function to call on link click. Example: <c>Show.TaskDialog("", "Text ˂a href="example"˃link˂/a˃.", onLinkClick: ed => { Out(ed.LinkHref); });</c></param>
		/// <remarks>
		/// Uses TaskDialogObject class, which uses Windows API function TaskDialogIndirect (you can find more info in MSDN).
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner (more info in Wnd.MoveInScreen()), ScriptOptions.DisplayName (title).
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialog(...))</c> instead of <c>switch(TaskDialog(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>Show.TaskDialog("Text.", style: "YN!", title: "Title")</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// </remarks>
		public static TDResult TaskDialog(
			Wnd owner, string text1, string text2 = null, TDButton buttons = 0, TDIcon icon = 0, TDFlag flags = 0,
			int defaultButton = 0, StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null, int x = 0, int y = 0, int timeoutS = 0,
			EventHandler_<TaskDialogObject.TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialogObject(owner, text1, text2, buttons, icon, flags,
				defaultButton, customButtons, radioButtons, checkBox, expandedText, footerText, title, x, y, timeoutS, onLinkClick);
			return d.Show();
		}

		/// <summary>
		/// Shows task dialog.
		/// Returns clicked button id and other results packed in a TDResult object (more info in Remarks).
		/// <para>
		/// You can specify buttons etc in style string, which can contain these characters:
		/// </para>
		/// <para>Buttons: O OK, C Cancel, Y Yes, N No, R Retry, L Close.</para>
		/// <para>Icon: x error, ! warning, i info, v shield, a app.</para>
		/// <para>Flags: c command links, b no taskbar button, e end task, n never activate, o owner center, t topmost.</para>
		/// <para>Default button: d before a common button character (eg "OdC") or custom button id (eg "d3").</para>
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="style">Example: "YN!c".</param>
		/// <param name="owner">Owner window or Wnd0.</param>
		/// <param name="customButtons">List of strings "id text" separated by |, like <c>"1 One|2 Two|3 Three"</c>. Or array or List, like <c>new string[]{"1 One", "2 Two", "3 Three"}</c>. Use "c" in style to change button style.</param>
		/// <param name="radioButtons">List of strings "id text" separated by |, like <c>"1 One|2 Two|3 Three"</c>. Or array or List, like <c>new string[]{"1 One", "2 Two", "3 Three"}</c>.</param>
		/// <param name="checkBox">Check box text. To check, use "Text|true" or "Text|check" or "Text|checked".</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A lambda or other kind of delegate callback function to call on link click. Example: <c>Show.TaskDialog("", "Text ˂a href="example"˃link˂/a˃.", onLinkClick: ed => { Out(ed.LinkHref); });</c></param>
		/// <remarks>
		/// The difference from the other overload: here buttons, icon, flags and default button are specified in the style parameter (string) instead of using 4 parameters. This is more compact but less readable.
		/// Uses TaskDialogObject class, which uses Windows API function TaskDialogIndirect (you can find more info in MSDN).
		/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner (more info in Wnd.MoveInScreen()), ScriptOptions.DisplayName (title).
		/// The returned TDResult object has these properties: clicked button id (eg TDResult.OK, 1 (custom button), TDResult.Timeout), name, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialog(...))</c> instead of <c>switch(TaskDialog(...).Button)</c>.
		/// Tip: For optional parameters use named arguments. Example: <c>Show.TaskDialog("Text.", style: "YN!", title: "Title")</c>
		/// If common and custom buttons are not specified, the dialog will have OK button.
		/// </remarks>
		public static TDResult TaskDialog(
			string text1, string text2 = null, string style = null, Wnd owner = default(Wnd),
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null, int x = 0, int y = 0, int timeoutS = 0,
			EventHandler_<TaskDialogObject.TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialogObject(text1, text2, style, owner,
				customButtons, radioButtons, checkBox, expandedText, footerText, title, x, y, timeoutS, onLinkClick);
			return d.Show();
		}

		public static int TaskListDialog(
			StringList list, string text1 = null, string text2 = null, Wnd owner = default(Wnd), string style = null, string title = null)
		{
			var d = new TaskDialogObject(text1, text2, style, title: title);
			d.SetCustomButtons(list, true);
			d.FlagAllowCancel = true;
			d.SetOwnerWindow(owner);
			return d.Show();
		}
	}

	/// <summary>
	/// Shows task dialog like Show.TaskDialog() but has more options and allows you to use more object-oriented programming style.
	/// Uses Windows API function TaskDialogIndirect. You can find more info in MSDN.
	/// </summary>
	/// <example>
	/// <code>
	/// var d = new TaskDialogObject(); //this example uses the 0-parameters constructor, but also there are constructors that have all the same parameters as Show.TaskDialog()
	/// d.SetText("Main text.", "More text.\nSupports ˂A HREF=\"link data\"˃links˂/A˃ if you subscribe to HyperlinkClick event.");
	/// d.SetStyle("OC!");
	/// d.SetExpandedText("Expanded info\nand more info.", true);
	/// d.FlagCanBeMinimized=true;
	/// d.SetCustomButtons("1 one|2 two\nzzz", true);
	/// d.SetRadioButtons("1001 r1|1002 r2");
	/// d.SetTimeout(10, "OK");
	/// d.HyperlinkClicked += ed => { Out($"{ed.Message} {ed.LinkHref}"); };
	/// d.ButtonClicked += ed => { Out($"{ed.Message} {ed.WParam}"); if(ed.WParam==TDResult.No) ed.Return=1; }; //ed.Return=1 prevents closing
	/// d.FlagShowProgressBar=true; d.Timer += ed => { ed.Obj.Send.Progress(ed.WParam/100); };
	/// TDResult r = d.Show();
	/// switch(r.Button) { case TDResult.OK: ... case 1: ... }
	/// switch(r.RadioButton) { ... }
	/// if(r.IsChecked) { ... }
	/// </code>
	/// </example>
	/// <remarks>
	/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, Script.Option.dialogScreenIfNoOwner, ScriptOptions.DisplayName (title).
	/// </remarks>
	public class TaskDialogObject
	{
		#region API

		[DllImport("comctl32.dll")]
		static extern int TaskDialogIndirect([In] ref TASKDIALOGCONFIG c, out int pnButton, out int pnRadioButton, out int pChecked);

		/// <summary>
		/// Messages that your event handler can send to the dialog.
		/// Reference: MSDN -> "Task Dialog Messages".
		/// </summary>
		public enum TDM_ :uint
		{
			NAVIGATE_PAGE = WM_USER + 101,
			CLICK_BUTTON = WM_USER + 102, // wParam = button id
			SET_MARQUEE_PROGRESS_BAR = WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
			SET_PROGRESS_BAR_STATE = WM_USER + 104, // wParam = new progress state (0, 1 or 2)
			SET_PROGRESS_BAR_RANGE = WM_USER + 105, // lParam = Calc.MakeLparam(min, max)
			SET_PROGRESS_BAR_POS = WM_USER + 106, // wParam = new position
			SET_PROGRESS_BAR_MARQUEE = WM_USER + 107, // wParam = 0 (stop marquee), wParam != 0 (start marquee), lParam = speed (milliseconds between repaints)
			SET_ELEMENT_TEXT = WM_USER + 108, // wParam = element (enum TDE_), lParam = new element text (string)
			CLICK_RADIO_BUTTON = WM_USER + 110, // wParam = radio button id
			ENABLE_BUTTON = WM_USER + 111, // wParam = button id, lParam = 0 (disable), lParam != 0 (enable)
			ENABLE_RADIO_BUTTON = WM_USER + 112, // wParam = radio button id, lParam = 0 (disable), lParam != 0 (enable)
			CLICK_VERIFICATION = WM_USER + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
			UPDATE_ELEMENT_TEXT = WM_USER + 114, // wParam = element (enum TDE_), lParam = new element text (string)
			SET_BUTTON_ELEVATION_REQUIRED_STATE = WM_USER + 115, // wParam = button id, lParam = 0 (elevation not required), lParam != 0 (elevation required)
			UPDATE_ICON = WM_USER + 116  // wParam = icon element (enum TDIE_), lParam = new icon (icon handle or TDIcon)
		}
		const uint WM_USER = Api.WM_USER;

		/// <summary>
		/// Notification messages that your event handler receives.
		/// Reference: MSDN -> "Task Dialog Notifications".
		/// </summary>
		public enum TDN_ :uint
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
		/// Constants for TDM_.SET_ELEMENT_TEXT and TDM_.UPDATE_ELEMENT_TEXT messages.
		/// </summary>
		public enum TDE_
		{
			CONTENT,
			EXPANDED_INFORMATION,
			FOOTER,
			MAIN_INSTRUCTION
		}

		/// <summary>
		/// Constants for TDM_.UPDATE_ICON message.
		/// </summary>
		public enum TDIE_
		{
			ICON_MAIN,
			ICON_FOOTER
		}

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

		delegate int TaskDialogCallbackProc(Wnd hwnd, TDN_ notification, LPARAM wParam, LPARAM lParam, IntPtr data);

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
		/// All parameters are the same as of Show.TaskDialog.
		/// </summary>
		public TaskDialogObject(
			Wnd owner, string text1, string text2 = null, TDButton buttons = 0, TDIcon icon = 0, TDFlag flags = 0,
			int defaultButton = 0, StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null, int x = 0, int y = 0, int timeoutS = 0,
			EventHandler_<TDEventArgs> onLinkClick = null
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
			SetXY(x, y);
			SetTimeout(timeoutS);
			if(onLinkClick != null) HyperlinkClicked += onLinkClick;
		}

		/// <summary>
		/// Creates new object and sets commonly used properties.
		/// All parameters are the same as of Show.TaskDialog.
		/// </summary>
		public TaskDialogObject(
			string text1, string text2 = null, string style = null, Wnd owner = default(Wnd),
			StringList customButtons = null, StringList radioButtons = null, string checkBox = null,
			string expandedText = null, string footerText = null, string title = null, int x = 0, int y = 0, int timeoutS = 0,
			EventHandler_<TDEventArgs> onLinkClick = null
			) : this()
		{
			SetStyle(style);
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
			_c.pszWindowTitle = Catkeys.Show._Title(text);
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
		/// <param name="iconHandle">Native icon handle.</param>
		public void SetIcon(IntPtr iconHandle)
		{
			_c.hMainIcon = iconHandle;
			_USE_HICON_MAIN = iconHandle != Zero;
			//tested: displays original-size 32 and 16 icons, but shrinks bigger icons to 32.
			//note: for App icon Show() will execute more code. The same for footer icon.
		}

		//TODO: add more SetIcon overloads (maybe also for footer icon): file, resource, Icon.

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
		/// <para>Flags: c command links, b no taskbar button, e end task, n never activate, o owner center, t topmost.</para>
		/// <para>Default button: d before a common button character (eg "OdC") or custom button id (eg "d3").</para>
		/// </summary>
		/// <param name="style">Example: "YN!".</param>
		public void SetStyle(string style)
		{
			var t = new _TDStyle(style);
			SetButtons(t.buttons);
			DefaultButton = t.defaultButton;
			SetIcon(t.icon);
			_USE_COMMAND_LINKS = t.commandLinks; //SetCustomButtons has a bool? parameter for this
			FlagNoTaskbarButton = t.noTaskbarButton;
			FlagEndThread = t.endThread;
			FlagNeverActivate = t.noActivate;
			_POSITION_RELATIVE_TO_WINDOW = t.ownerCenter; //SetOwnerWindow has a bool? parameter for this
			if(t.topmost) FlagTopmost = t.topmost; //else use Option.dialogTopmostIfNoOwner if no owner
		}

		struct _TDStyle
		{
			public TDButton buttons;
			public int defaultButton;
			public TDIcon icon;
			public bool commandLinks, noTaskbarButton, endThread, noActivate, ownerCenter, topmost;

			public _TDStyle(string style) : this()
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
					case 'c': commandLinks = true; break;
					case 'b': noTaskbarButton = true; break;
					case 'e': endThread = true; break;
					case 'n': noActivate = true; break;
					case 'o': ownerCenter = true; break;
					case 't': topmost = true; break;
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
		}

		/// <summary>
		/// Adds custom buttons and sets button style.
		/// </summary>
		/// <param name="buttons">List of strings "id text" separated by |, like <c>"1 One|2 Two|3 Three"</c>. Or array or List, like <c>new string[]{"1 One", "2 Two", "3 Three"}</c>.</param>
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
		/// The value can be one of common buttons (eg TDResult.OK) or a custom button id (eg 1). If omitted or 0, auto-selects.
		/// </summary>
		public int DefaultButton { set { _c.nDefaultButton = -value; } }
		//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().

		/// <summary>
		/// Adds radio (option) buttons.
		/// To get selected radio button when Show() returns, use member RadioButton of the returned TDResult object.
		/// </summary>
		/// <param name="buttons">List of strings "id text" separated by |, like <c>"1 One|2 Two|3 Three"</c>. Or array or List, like <c>new string[]{"1 One", "2 Two", "3 Three"}</c>.</param>
		/// <param name="defaultId">Select (check) this radio button. If omitted or 0, selects the first. If -1, does not select.</param>
		public void SetRadioButtons(StringList buttons, int defaultId = 0)
		{
			_radioButtons = buttons;
			_c.nDefaultRadioButton = defaultId;
			_NO_DEFAULT_RADIO_BUTTON = defaultId < 0;
		}

		/// <summary>
		/// Adds check box.
		/// To get check box state when Show() returns, use member IsChecked of the returned TDResult object.
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
		/// Adds text and icon at the bottom of the dialog.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">One of standard icons, eg TDIcon.Warning.</param>
		public void SetFooterText(string text, TDIcon icon = 0)
		{
			_c.pszFooter = text;
			_c.hFooterIcon = (IntPtr)(int)icon; _USE_HICON_FOOTER = false;
		}
		/// <summary>
		/// Adds text and icon at the bottom of the dialog.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="icon">Native icon handle.</param>
		public void SetFooterText(string text, IntPtr iconHandle)
		{
			_c.pszFooter = text;
			_c.hFooterIcon = iconHandle; _USE_HICON_FOOTER = iconHandle != Zero;
		}

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
		/// <param name="rawXY">Don't interpret 0 and negative x y in a special way.</param>
		public void SetXY(int x, int y, bool rawXY = false)
		{
			_x = x; _y = y; _xyIsRaw = rawXY;
		}

		int _x, _y; bool _xyIsRaw;

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

		public bool FlagAllowCancel { set; private get; }
		public bool FlagRtlLayout { set; private get; }
		public bool FlagCanBeMinimized { set; private get; }
		public bool FlagShowProgressBar { set; private get; }
		public bool FlagShowMarqueeProgressBar { set; private get; }

		/// <summary>
		/// Makes the dialog window topmost or non-topmost.
		/// If true, will set topmost style when creating the dialog. If false, will not set.
		/// If null (default), the dialog will be topmost if both these are true: no owner window, Script.Option.dialogTopmostIfNoOwner is true.
		/// </summary>
		public bool? FlagTopmost { set; private get; }
		public bool FlagNeverActivate { set; private get; }
		public bool FlagNoTaskbarButton { set; private get; }
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
		/// The return value is the same as of Show.TaskDialog.
		/// Call this method after setting text and other properties.
		/// </summary>
		public TDResult Show()
		{
			_result = null;
			_isClosed = false;

			int rButton = 0, rRadioButton = 0, rIsChecked = 0, hr = 0;

			if(_c.pszWindowTitle == null) _c.pszWindowTitle = Catkeys.Show._Title(null);

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

			if((_c.hMainIcon == (IntPtr)TDIcon.App || _c.hFooterIcon == (IntPtr)TDIcon.App) && Catkeys.Show.Resources.AppIconHandle32 != Zero)
				_c.hInstance = Misc.GetModuleHandleOfAppdomainEntryAssembly();
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

				if(_timeoutActive) {
					//need to receive mouse and keyboard messages to stop countdown on click or key
					hhook = Api.SetWindowsHookEx(Api.WH_CBT, hpHolder = _HookProcCBT, Zero, Api.GetCurrentThreadId());
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
					_result = new TDResult(rButton, rRadioButton, rIsChecked != 0);
				}
			} finally {
				_threadIdInShow = 0;

				if(hhook != Zero) Api.UnhookWindowsHookEx(hhook);

				_MarshalFreeButtons(ref _c.pButtons, ref _c.cButtons);
				_MarshalFreeButtons(ref _c.pRadioButtons, ref _c.cRadioButtons);

				_SetClosed();
			}

			if(hr != 0) throw new Win32Exception(hr);

			//DoEvents(); //process messages, or later something may not work //TODO
			//WaitForAnActiveWindow(500, 2); //TODO
			//Debug.Assert(Wnd.ActiveWindow != Wnd0); //sometimes asserts when without waiting

			if(FlagEndThread) {
				bool endThread = false;
				switch(rButton) {
				case TDResult.Cancel: case TDResult.No: case TDResult.Timeout: endThread = true; break;
				case TDResult.OK: endThread = (_c.dwCommonButtons == 0 || _c.dwCommonButtons == TDButton.OK) && _c.pButtons == Zero; break;
				}

				if(endThread) Thread.CurrentThread.Abort();
				//if(endThread) AppDomain.Unload(AppDomain.CurrentDomain); //TODO: consider this
			}

			return _result;
		}

		int _CallbackProc(Wnd w, TDN_ message, LPARAM wParam, LPARAM lParam, IntPtr data)
		{
			EventHandler_<TDEventArgs> e = null;
			int R = 0;

			//Out(message);
			switch(message) {
			case TDN_.DIALOG_CONSTRUCTED:
				Send = new Sender(w, this); //note: must be before setting _dlg, because another thread may call if(d.IsOpen) d.Send.Message(..).
				_dlg = w;
				break;
			case TDN_.DESTROYED:
				//Out(w.IsWindow); //valid
				e = Destroyed;
				break;
			case TDN_.CREATED:
				if(_enableOwner) _c.hwndParent.Enabled = true;
				//if(_enableOwner || !_c.hwndParent.IsOfThisThread) _c.hwndParent.Enabled=true; //not sure if it would be useful

				if(FlagNoTaskbarButton) w.SetExStyleAdd(Api.WS_EX_TOOLWINDOW);

				if(!_POSITION_RELATIVE_TO_WINDOW) {
					object scr = Screen; if(scr == null && _c.hwndParent.Is0) scr = Script.Option.dialogScreenIfNoOwner;
					if((_x != 0 || _y != 0 || _xyIsRaw || scr != null)) {
						w.MoveInScreen(_x, _y, scr, rawXY: _xyIsRaw);
					}
				}

				bool topmost = false;
				if(FlagTopmost != null) topmost = FlagTopmost.Value; else if(_c.hwndParent == null) topmost = Script.Option.dialogTopmostIfNoOwner;
				if(topmost) w.ZorderTopmost();

				e = Created;
				break;
			case TDN_.TIMER:
				if(_timeoutActive) {
					int timeElapsed = wParam / 1000;
					if(timeElapsed < _timeoutS) {
						if(!_timeoutNoInfo) w.SendS((uint)TDM_.UPDATE_ELEMENT_TEXT, (int)TDE_.FOOTER, _TimeoutFooterText(_timeoutS - timeElapsed - 1));
					} else {
						_timeoutActive = false;
						Send.Close(TDResult.Timeout);
					}
				}

				if(_lockForegroundWindow) {
					_lockForegroundWindow = false;
					Api.LockSetForegroundWindow(Api.LSFW_UNLOCK);
					w.FlashStop();
				}

				e = Timer;
				break;
			case TDN_.BUTTON_CLICKED: e = ButtonClicked; wParam = -wParam; break; //info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
			case TDN_.HYPERLINK_CLICKED: e = HyperlinkClicked; break;
			case TDN_.HELP: e = HelpF1; break;
			default: e = OtherEvents; break;
			}

			if(e != null) {
				var ed = new TDEventArgs(this, _dlg, message, wParam, lParam);
				e(ed);
				R = ed.Return;
			}

			if(message == TDN_.DESTROYED) _SetClosed();

			return R;
		}

		/// <summary>
		/// TaskDialogObject events.
		/// Raised when the internal TaskDialogCallbackProc function (see in MSDN) is called by the task dialog API.
		/// </summary>
		public event EventHandler_<TDEventArgs>
			Created, Destroyed, Timer, ButtonClicked, HyperlinkClicked, HelpF1, OtherEvents;

		/// <summary>
		/// Provides arguments for TaskDialogObject event handlers.
		/// They are the arguments of the TaskDialogCallbackProc function (see in MSDN).
		/// To return a non-zero value from the callback function, assign the value to the Return property.
		/// </summary>
		public class TDEventArgs :EventArgs
		{
			public TDEventArgs(TaskDialogObject obj, Wnd dlg, TDN_ message, LPARAM wParam, LPARAM lParam)
			{
				Obj = obj; DialogWindow = dlg; Message = message; WParam = wParam;
				if(message == TDN_.HYPERLINK_CLICKED) LinkHref = Marshal.PtrToStringUni(lParam); else LinkHref = null;
			}
			public TaskDialogObject Obj;
			public Wnd DialogWindow; //not a property because then handler could not do ed.DialogWindow.Property=value. Also cannot be readonly.
			public TDN_ Message;
			public LPARAM WParam;
			public string LinkHref;
			public int Return;
		}

		/// <summary>
		/// Shows the dialog in another thread and waits a while until the dialog is open.
		/// Calls <c>Task.Run(() =˃ { Show(); }); ThreadWaitOpen();</c>
		/// </summary>
		/// <param name="onClose">A callback function (lambda etc) to call when the dialog is closed. It will be called in another thread too.</param>
		public void ShowAsync(TDAsyncCloseDelegate onClose = null)
		{
			Task.Run(() =>
			{
				Show();
				if(onClose != null) onClose(Result);
				//note: currently onClose not called if main thread is already returned at that time.
				//	Because then called AppDomain.Unload, which aborts thread, which aborts executing managed code.
				//	In the future before calling AppDomain.Unload should close all thread windows, or wait for windows marked 'fait for it on script exit'.
			});
			if(!ThreadWaitOpen()) throw new Exception();
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
				throw new Exception("wrong thread");
		}

		/// <summary>
		/// Returns true if the dialog is open and you can send messages to it.
		/// </summary>
		public bool IsOpen { get { return _dlg != Wnd0; } }

		void _SetClosed()
		{
			_isClosed = true;
			_dlg = Wnd0;
			Send = new Sender(); //clear it
		}
		bool _isClosed;

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
			TaskDialogObject _tdo; //used only to get _tdo._c for TDM_.NAVIGATE_PAGE. Using _tdo._hdlg would be unsafe in multithreaded context because may be already set to null, even if caller called IsOpen before.
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
			/// Example (in an event handler): <c>ed.Obj.Send.Message(TaskDialogObject.TDM_.CLICK_VERIFICATION, 1);</c>
			/// Also there are several other functions to send some messages.
			/// Message reference: MSDN.
			/// </summary>
			public int Message(TDM_ message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
			{
				//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().
				switch(message) { case TDM_.CLICK_BUTTON: case TDM_.ENABLE_BUTTON: case TDM_.SET_BUTTON_ELEVATION_REQUIRED_STATE: wParam = -wParam; break; }

				//if(!_IsOpen()) return 0;
				return _dlg.Send((uint)message, wParam, lParam);
			}

			/// <summary>
			/// Sends message TDM_.SET_ELEMENT_TEXT or TDM_.UPDATE_ELEMENT_TEXT to the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example (in an event handler): <c>ed.Obj.Send.Text(true, TaskDialogObject.TDE_.CONTENT, "New text.");</c>
			/// </summary>
			public void Text(bool updateText, TDE_ partId, string text)
			{
				//if(!_IsOpen()) return;
				_dlg.SendS((uint)(updateText ? TDM_.UPDATE_ELEMENT_TEXT : TDM_.SET_ELEMENT_TEXT), (int)partId, text);
			}

			/// <summary>
			/// Applies new properties to the dialog while it is already open.
			/// Can be used for example to create wizard-like dialog with custom buttons "Next" and "Back".
			/// Call this method while the dialog is open, eg in an event handler, after setting new properties.
			/// Sends message TDM_.NAVIGATE_PAGE.
			/// </summary>
			public void NavigatePage()
			{
				//if(!_IsOpen()) return;
				TaskDialogObject o = _tdo; if(o == null) return;
				_ApiSendMessageTASKDIALOGCONFIG(_dlg, (uint)TDM_.NAVIGATE_PAGE, 0, ref o._c);
			}

			[DllImport("user32.dll", EntryPoint = "SendMessageW")]
			static extern LPARAM _ApiSendMessageTASKDIALOGCONFIG(Wnd hWnd, uint msg, LPARAM wParam, [In] ref TASKDIALOGCONFIG c);

			/// <summary>
			/// Clicks a button. Normally it closes the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Sends message TDM_.CLICK_BUTTON.
			/// </summary>
			/// <param name="buttonId">A button id or some other number that will be returned in Show() results. Default: TDResult.Close.</param>
			public bool Close(int buttonId = TDResult.Close)
			{
				//if(!_IsOpen()) return false;
				return 0 != Message(TDM_.CLICK_BUTTON, buttonId);
			}

			/// <summary>
			/// Enables or disables a button. Normally it closes the dialog.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example: <c>d.Created += ed => { ed.Obj.Send.EnableButton(TDResult.Yes, false); };</c>
			/// Sends message TDM_.ENABLE_BUTTON.
			/// </summary>
			public void EnableButton(int buttonId, bool enable)
			{
				//if(!_IsOpen()) return;
				Message(TDM_.ENABLE_BUTTON, buttonId, enable);
			}

			/// <summary>
			/// Sets progress bar value, 0 to 100.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Sends message TDM_.SET_PROGRESS_BAR_POS.
			/// </summary>
			public int Progress(int percent)
			{
				//if(!_IsOpen()) return 0;
				return Message(TDM_.SET_PROGRESS_BAR_POS, percent);
			}
		} //class Sender

		#region TaskDialogObject util
#if DEBUG //TODO: consider: use this func always.
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
			IntPtr R = Marshal.AllocHGlobal(structSize * a.Length);

			for(int i = 0; i < a.Length; i++) {
				TASKDIALOG_BUTTON b; b.id = a[i].ToInt_(out b.text); //"id text" -> TASKDIALOG_BUTTON

				if(string.IsNullOrEmpty(b.text)) { b.text = a[i]; if(string.IsNullOrEmpty(b.text)) b.text = " "; } //exception if null or ""
				else b.text = b.text.Replace("\r\n", "\n"); //the API adds 2 newlines for \r\n. Only for custom buttons, not for other controls/parts.

				if(escapeId) { b.id = -b.id; if(b.id > 0) throw new ArgumentException("button id < 0"); }
				//info: internally button ids are negative (internalId = -specifiedId), to avoid user button id conflict with common button ids. See _MarshalButtons().

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
		IntPtr _HookProcCBT(int code, LPARAM wParam, IntPtr lParam)
		{
			switch(code) {
			case Api.HCBT_CLICKSKIPPED:
				if(_timeoutActive) switch((uint)wParam) { case Api.WM_LBUTTONUP: case Api.WM_NCLBUTTONUP: goto g1; }
				break;
			case Api.HCBT_KEYSKIPPED:
				g1:
				if(_timeoutActive && _dlg.IsActiveWindow) {
					_timeoutActive = false;
					if(!_timeoutNoInfo) {
						if(Empty(_timeoutFooterText)) {
							_dlg.Send((uint)TDM_.UPDATE_ICON, (int)TDIE_.ICON_FOOTER, 0);
							_dlg.SendS((uint)TDM_.SET_ELEMENT_TEXT, (int)TDE_.FOOTER, ""); //null does not change text; however still remains some space for footer

							//c.pszFooter=null; SendNavigatePage(); //don't use this because interferes with the expand/collapse control
						} else _dlg.SendS((uint)TDM_.SET_ELEMENT_TEXT, (int)TDE_.FOOTER, _timeoutFooterText);
					}
				}
				break;
				//case Api.HCBT_ACTIVATE: //does not work. Disables activating this window, but another window is deactivated anyway.
				//	if(_lockForegroundWindow) { //return 1 to prevent activating
				//		if(_dlg==Wnd0) return (IntPtr)1; //before TDN_.DIALOG_CONSTRUCTED
				//		if(_dlg==(IntPtr)wParam) { if(_dlg.Visible) _lockForegroundWindow=false; else return (IntPtr)1; }
				//		//For our dialog HCBT_ACTIVATE received 4 times: 2 before TDN_.DIALOG_CONSTRUCTED/TDN_.CREATED and 2 after, but before visible.
				//	}
				//	break;
			}

			return Api.CallNextHookEx(Zero, code, wParam, lParam);
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
		#endregion
	} //TaskDialogObject
	#endregion TaskDialog

	#region InputDialog

	public static partial class Show
	{


		public static bool InputDialog(string s)
		{
			return false;
		}

	}
	#endregion InputDialog

	#region Show util
	public static partial class Show
	{
		internal static string _Title(string title) { return string.IsNullOrEmpty(title) ? ScriptOptions.DisplayName : title; }
		//info: IsNullOrEmpty because if "", API TaskDialog uses "ProcessName.exe".

		public static class Resources
		{
			/// <summary>
			/// Gets native icon handle of the entry assembly of current appdomain.
			/// Returns Zero if the assembly is without icon.
			/// The icon is extracted first time and then cached in a static variable. Don't destroy the icon.
			/// </summary>
			public static IntPtr AppIconHandle32 { get { return _GetAppIconHandle(ref _AppIcon32, false); } }
			public static IntPtr AppIconHandle16 { get { return _GetAppIconHandle(ref _AppIcon16, true); } }
			static IntPtr _AppIcon32, _AppIcon16;

			static IntPtr _GetAppIconHandle(ref IntPtr hicon, bool small = false)
			{
				if(hicon == Zero) {
					var asm = Misc.AppdomainAssembly; if(asm == null) return Zero;
					IntPtr hinst = Misc.GetModuleHandleOf(asm);
					int size = small ? 16 : 32;
					hicon = Api.LoadImageRes(hinst, 32512, Api.IMAGE_ICON, size, size, Api.LR_SHARED);
					//note:
					//This is not 100% reliable because the icon id 32512 (IDI_APPLICATION) is undocumented.
					//I could not find a .NET method to get icon directly from native resources of assembly.
					//Could use Icon.ExtractAssociatedIcon(asm.Location), but it always gets 32 icon and is several times slower.
					//Also could use PrivateExtractIcons. But it uses file path, not module handle.
					//Also could use the resource emumeration API...
					//Never mind. Anyway, we use hInstance/resId with MessageBoxIndirect (which does not support handles) etc.
					//info: MSDN says that LR_SHARED gets cached icon regardless of size, but it is not true. Caches each size separately. Tested on Win 10, 7, XP.
				}
				return hicon;
			}
		}
	}
	#endregion util
}
