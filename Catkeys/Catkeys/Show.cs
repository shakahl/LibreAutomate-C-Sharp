using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
using System.Windows.Forms;

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
	/// MessageDialog return value (user-clicked button).
	/// </summary>
	public enum MDResult
	{
		OK = 1, Cancel = 2, Abort = 3, Retry = 4, Ignore = 5, Yes = 6, No = 7, Timeout = 9, TryAgain = 10, Continue = 11,
	}

	public enum MDButtons
	{
		OK = 0, OKCancel = 1, AbortRetryIgnore = 2, YesNoCancel = 3, YesNo = 4, RetryCancel = 5, CancelTryagainContinue = 6,
		//TODO: consider: Abort = -1 (QM2 mes-). In TaskDialog too.
	}

	public enum MDIcon
	{
		None = 0, Error = 0x10, Question = 0x20, Warning = 0x30, Info = 0x40, Shield = 0x50,
	}

	[Flags]
	public enum MDFlag :uint
	{
		DefaultButton2 = 0x100, DefaultButton3 = 0x200, DefaultButton4 = 0x300,
		DisableThreadWindows = 0x2000, HelpButton = 0x4000, //TODO: test receiving WM_HELP on HelpButton
		Activate = 0x10000, DefaultDesktopOnly = 0x20000, Topmost = 0x40000, RightAlign = 0x80000, Rtl = 0x100000, Service = 0x200000,
		NoSound = 0x80000000, NoTopmost = 0x40000000,
	}

	//[DebuggerStepThrough]
	public static partial class Show
	{
		static string _Title(string title) { return string.IsNullOrEmpty(title) ? ScriptOptions.DisplayName : title; }
		//info: IsNullOrEmpty because if "", API TaskDialog uses "ProcessName.exe".

		//MessageDialog

		/// <summary>
		/// Shows standard message box dialog.
		/// </summary>
		/// <param name="owner">Owner window or Zero.</param>
		/// <param name="text">Text.</param>
		/// <param name="buttons">Example: MDButtons.YesNo.</param>
		/// <param name="icon">One of standard icons. Example: MDIcon.Info.</param>
		/// <param name="flags">One or more options. Example: MDFlag.NoTopmost|MDFlag.DefaultButton2.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		public static MDResult MessageDialog(Wnd owner, string text, MDButtons buttons = 0, MDIcon icon = 0, MDFlag flags = 0, string title = null)
		{
			//const uint MB_SYSTEMMODAL = 0x1000; //same as MB_TOPMOST + adds system icon in title bar (why need it?)
			const uint MB_USERICON = 0x80;
			//const uint IDI_APPLICATION = 32512; //standard icon, not useful
			const uint IDI_ERROR = 32513;
			const uint IDI_QUESTION = 32514;
			const uint IDI_WARNING = 32515;
			const uint IDI_INFORMATION = 32516;
			const uint IDI_SHIELD = 106; //32x32 icon. The value is undocumented, but we use it instead of the documented IDI_SHIELD value which on Win7 displays clipped 128x128 icon. Tested: the function does not fail with invalid icon resource id.

			var p = new MSGBOXPARAMS(title);
			p.lpszText=text;

			bool alien = flags.HasAny(MDFlag.DefaultDesktopOnly|MDFlag.Service);
			if(alien) owner=Zero; //API would fail. The dialog is displayed in csrss process.

			if(icon==MDIcon.None) { } //no sound
			else if(icon==MDIcon.Shield || flags.HasFlag(MDFlag.NoSound)) {
				switch(icon) {
				case MDIcon.Error: p.lpszIcon=(IntPtr)IDI_ERROR; break;
				case MDIcon.Question: p.lpszIcon=(IntPtr)IDI_QUESTION; break;
				case MDIcon.Warning: p.lpszIcon=(IntPtr)IDI_WARNING; break;
				case MDIcon.Info: p.lpszIcon=(IntPtr)IDI_INFORMATION; break;
				case MDIcon.Shield: p.lpszIcon=(IntPtr)IDI_SHIELD; break;
				}
				p.dwStyle|=MB_USERICON; //disables sound
				icon=0;
			}
			//note: Too difficult to add app icon because it must be in unmanaged resources. If need, use TaskDialog, it accepts icon handle.

			if(owner.IsZero && !flags.HasFlag(MDFlag.NoTopmost)) flags|=MDFlag.Topmost;
			//tested: if owner is child, the API disables its top-level parent.
			//consider: if owner 0, create hidden parent window to:
			//	Avoid adding taskbar icon.
			//	Apply Option.Monitor.
			//consider: if owner 0, and current foreground window is of this thread, let it be owner. Maybe a flag.
			//consider: if owner of other thread, don't disable it. But how to do it without hook? Maybe only inherit owner's monitor.
			//consider: play user-defined sound, eg default "meow".

			p.hwndOwner=owner;

			flags&=~(MDFlag.NoSound|MDFlag.NoTopmost);
			p.dwStyle|=(uint)buttons | (uint)icon | (uint)flags;

			//TODO: if(flags.HasFlag(MDIcon.Activate)) Wnd.AllowActivateWindows();

			int R = MessageBoxIndirect(ref p);
			if(R==0) throw new CatkeysException();
			//DoEvents(); //process messages, or later something may not work //TODO
			//WaitForAnActiveWindow(500, 2); //TODO
			return (MDResult)R;

			//tested:
			//user32:MessageBoxTimeout. Undocumented. Too limited etc to be useful. If need timeout, use TaskDialog.
			//shlwapi:SHMessageBoxCheck. Too limited etc to be useful.
		}

		/// <summary>
		/// Shows standard message box dialog.
		/// Returns clicked button's character (as in style), eg 'O' for OK.
		/// You can specify buttons etc in style string, which can contain:
		/// <para>
		/// Buttons: OC OKCancel, YN YesNo, YNC YesNoCancel, ARI AbortRetryIgnore, RC RetryCancel, CTE CancelTryagainContinue.
		/// Icon: x error, ! warning, i info, ? question, v shield.
		/// Flags: s no sound, a activate, t topmost, n no topmost, d disable windows, h Help button.
		/// Default button: 2, 3 or 4.
		/// </para>
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="style">Example: "YN!".</param>
		/// <param name="owner">Owner window or Zero.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		public static char MessageDialog(string text, string style = null, Wnd owner = default(Wnd), string title = null)
		{
			MDButtons buttons = 0;
			MDIcon icon = 0;
			MDFlag flags = 0;

			if(!string.IsNullOrEmpty(style)) {
				if(style.Contains("OC")) buttons=MDButtons.OKCancel;
				else if(style.Contains("YNC")) buttons=MDButtons.YesNoCancel;
				else if(style.Contains("YN")) buttons=MDButtons.YesNo;
				else if(style.Contains("ARI")) buttons=MDButtons.AbortRetryIgnore;
				else if(style.Contains("RC")) buttons=MDButtons.RetryCancel;
				else if(style.Contains("CT")) buttons=MDButtons.CancelTryagainContinue; //not CTC, because Continue returns E

				if(style.Contains("x")) icon=MDIcon.Error;
				else if(style.Contains("?")) icon=MDIcon.Question;
				else if(style.Contains("!")) icon=MDIcon.Warning;
				else if(style.Contains("i")) icon=MDIcon.Info;
				else if(style.Contains("v")) icon=MDIcon.Shield;

				if(style.Contains("t")) flags|=MDFlag.Topmost; else if(style.Contains("n")) flags|=MDFlag.NoTopmost;
				if(style.Contains("a")) flags|=MDFlag.Activate;
				if(style.Contains("s")) flags|=MDFlag.NoSound;
				if(style.Contains("d")) flags|=MDFlag.DisableThreadWindows;
				if(style.Contains("h")) flags|=MDFlag.HelpButton;

				if(style.Contains("2")) flags|=MDFlag.DefaultButton2;
				else if(style.Contains("3")) flags|=MDFlag.DefaultButton3;
				else if(style.Contains("4")) flags|=MDFlag.DefaultButton4;
			}

			int r = (int)MessageDialog(owner, text, buttons, icon, flags, title);

			return (r>0 && r<12) ? "COCARIYNCCTE"[r] : 'C';
		}

		struct MSGBOXPARAMS
		{
			public int cbSize;
			public Wnd hwndOwner;
			public IntPtr hInstance;
			public string lpszText;
			public string lpszCaption;
			public uint dwStyle;
			public IntPtr lpszIcon;
			public LPARAM dwContextHelpId;
			public IntPtr lpfnMsgBoxCallback;
			public uint dwLanguageId;

			public MSGBOXPARAMS(string title) : this()
			{
				cbSize=Marshal.SizeOf(typeof(MSGBOXPARAMS));
				lpszCaption=_Title(title);
			}
		}

		[DllImport("user32.dll")]
		static extern int MessageBoxIndirect([In] ref MSGBOXPARAMS lpMsgBoxParams);

	}
	//_________________________________________________________________

	//TaskDialog

	[Flags]
	public enum TDButton
	{
		OK = 1, Yes = 2, No = 4, Cancel = 8, Retry = 0x10, Close = 0x20,
		OKCancel = OK|Cancel, YesNo = Yes|No, RetryCancel = Retry|Cancel
	}

	[Flags]
	public enum TDIcon
	{
		Warning = 0xffff, Error = 0xfffe, Info = 0xfffd, Shield = 0xfffc
	}

	public enum TDResult
	{
		Cancel = 0, OK = -1, Retry = -4, Yes = -6, No = -7, Close = -8,
		Timeout = -99
	}

	public static partial class Show
	{
		const int E_INVALIDARG = unchecked((int)0x80070057);

		[DllImport("comctl32.dll", EntryPoint = "TaskDialog")]
		static extern int _TaskDialog(Wnd hWndParent, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, TDButton dwCommonButtons, IntPtr pszIcon, out int pnButton);

		/// <summary>
		/// Shows simple task dialog.
		/// Return clicked button, eg TDResult.OK. Note: TDResult.Cancel value is 0.
		/// </summary>
		/// <param name="owner">Owner window or Zero.</param>
		/// <param name="mainText">Main instruction. Bigger font.</param>
		/// <param name="moreText">Text below main instruction.</param>
		/// <param name="buttons">Buttons, eg TDButton.YesNo or TDButton.OK|TDButton.Close. If omitted or 0, adds OK button.</param>
		/// <param name="icon">One of four standard icons, eg TDIcon.Info.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		public static TDResult TaskDialog(Wnd owner, string mainText, string moreText = null, TDButton buttons = 0, TDIcon icon = 0, string title = null)
		{
			int R, hr = _TaskDialog(owner, Zero, _Title(title), mainText, moreText, buttons, (IntPtr)icon, out R);

			if(hr!=0) throw new Win32Exception(hr);

			if(R==2) R=0; else R=-R; //TDResult.Cancel is 0, other TDResult values negative
			return (TDResult)R;
		}

		/// <summary>
		/// Shows simple task dialog.
		/// Returns clicked button's character (as in style), eg 'O' for OK.
		/// You can specify buttons etc in style string, which can contain:
		/// <para>
		/// Buttons: O OK, C Cancel, Y Yes, N No, R Retry, L Close.
		/// Icon: x error, ! warning, i info, v shield.
		/// </para>
		/// </summary>
		/// <param name="mainText">Main instruction. Bigger font.</param>
		/// <param name="moreText">Text below main instruction.</param>
		/// <param name="style">Example: "YN!".</param>
		/// <param name="owner">Owner window or Zero.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
		public static char TaskDialog(string mainText, string moreText = null, string style = null, Wnd owner = default(Wnd), string title = null)
		{
			TDButton buttons = 0;
			TDIcon icon = 0;
			if(style!=null) {
				foreach(char c in style) {
					switch(c) {
					case 'O': buttons|=TDButton.OK; break;
					case 'C': buttons|=TDButton.Cancel; break;
					case 'R': buttons|=TDButton.Retry; break;
					case 'Y': buttons|=TDButton.Yes; break;
					case 'N': buttons|=TDButton.No; break;
					case 'L': buttons|=TDButton.Close; break;
					case 'x': icon|=TDIcon.Error; break;
					case '!': icon|=TDIcon.Warning; break;
					case 'i': icon|=TDIcon.Info; break;
					case 'v': icon|=TDIcon.Shield; break;
					}
				}
			}

			int r = -(int)TaskDialog(owner, mainText, moreText, buttons, icon, title);

			return (r>=0 && r<9) ? "COCCRCYNL"[r] : 'C';
		}

		public static int TaskListDialog(string[] list, string mainText = null, string moreText = null, Wnd owner = default(Wnd), string title = null)
		{
			var d = new AdvancedTaskDialog(mainText, moreText, 0, 0, title);
			d.SetButtons(list, true);
			d.FlagAllowCancel=true; //instead of TDButton.Cancel; users can add Cancel as custom button
			return (int)d.Show(owner);
		}

		public static int TaskListDialog(string list, string mainText = null, string moreText = null, Wnd owner = default(Wnd), string title = null)
		{
			return TaskListDialog(_ButtonsStringToArray(list), mainText, moreText, owner, title);
		}

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

		enum TDM_
		{
			WM_USER = 0x400,
			NAVIGATE_PAGE = WM_USER+101,
			CLICK_BUTTON = WM_USER+102, // wParam = Button ID
			SET_MARQUEE_PROGRESS_BAR = WM_USER+103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
			SET_PROGRESS_BAR_STATE = WM_USER+104, // wParam = new progress state
			SET_PROGRESS_BAR_RANGE = WM_USER+105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
			SET_PROGRESS_BAR_POS = WM_USER+106, // wParam = new position
			SET_PROGRESS_BAR_MARQUEE = WM_USER+107, // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
			SET_ELEMENT_TEXT = WM_USER+108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
			CLICK_RADIO_BUTTON = WM_USER+110, // wParam = Radio Button ID
			ENABLE_BUTTON = WM_USER+111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
			ENABLE_RADIO_BUTTON = WM_USER+112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
			CLICK_VERIFICATION = WM_USER+113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
			UPDATE_ELEMENT_TEXT = WM_USER+114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
			SET_BUTTON_ELEVATION_REQUIRED_STATE = WM_USER+115, // wParam = Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
			UPDATE_ICON = WM_USER+116  // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
		}

		enum TDN_ :uint
		{
			TDN_CREATED = 0,
			TDN_NAVIGATED = 1,
			TDN_BUTTON_CLICKED = 2,            // wParam = Button ID
			TDN_HYPERLINK_CLICKED = 3,            // lParam = (LPCWSTR)pszHREF
			TDN_TIMER = 4,            // wParam = Milliseconds since dialog created or timer reset
			TDN_DESTROYED = 5,
			TDN_RADIO_BUTTON_CLICKED = 6,            // wParam = Radio Button ID
			TDN_DIALOG_CONSTRUCTED = 7,
			TDN_VERIFICATION_CLICKED = 8,             // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
			TDN_HELP = 9,
			TDN_EXPANDO_BUTTON_CLICKED = 10            // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
		}

		enum TDE_
		{
			TDE_CONTENT,
			TDE_EXPANDED_INFORMATION,
			TDE_FOOTER,
			TDE_MAIN_INSTRUCTION
		}

		enum TDIE_
		{
			TDIE_ICON_MAIN,
			TDIE_ICON_FOOTER
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct TASKDIALOG_BUTTON
		{
			public int id;
			public string text;
		}

		delegate int TaskDialogCallbackProc(Wnd hwnd, TDN_ notification, LPARAM wParam, LPARAM lParam, IntPtr data);

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct TASKDIALOGCONFIG
		{
			public int cbSize;
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

			public TASKDIALOGCONFIG(string title) : this()
			{
				cbSize=Marshal.SizeOf(typeof(TASKDIALOGCONFIG));
				pszWindowTitle=_Title(title);
			}
		}

		[DllImport("comctl32.dll")]
		static extern int TaskDialogIndirect([In] ref TASKDIALOGCONFIG c, out int pnButton, out int pnRadioButton, out int pChecked);

		public class AdvancedTaskDialog
		{
			TASKDIALOGCONFIG c;
			string[] _buttons, _radioButtons; //before showing dialog these will be marshaled to IntPtr

			/// <summary>
			/// Sets text and other most commonly used properties.
			/// </summary>
			public AdvancedTaskDialog(string mainText, string moreText = null, TDButton buttons = 0, TDIcon icon = 0, string title = null)
			{
				c.cbSize=Marshal.SizeOf(typeof(TASKDIALOGCONFIG));
				c.pszMainInstruction=mainText;
				c.pszContent=moreText;
				c.dwCommonButtons=buttons;
				c.hMainIcon=(IntPtr)(int)icon;
				c.pszWindowTitle=_Title(title);
			}

			public IntPtr MainIconHandle { set { c.hMainIcon=value; USE_HICON_MAIN=value!=Zero; } } //TODO: use overloaded method: handle, file (when getfileicon, expandpath etc available), resource. Tested: displays original-size 32 and 16 icons, but shrinks bigger icons to 32. Also add TDIcon.App (script's icon or exe icon). Maybe also all the same for the footer icon.

			public void SetButtons(string[] buttons, bool asCommandLinks = false, bool noCommandLinkIcon = false)
			{
				_buttons=buttons;
				USE_COMMAND_LINKS=asCommandLinks && !noCommandLinkIcon;
				USE_COMMAND_LINKS_NO_ICON=asCommandLinks && noCommandLinkIcon;
			}
			public void SetButtons(string buttons, bool asCommandLinks = false, bool noCommandLinkIcon = false)
			{
				SetButtons(_ButtonsStringToArray(buttons), asCommandLinks, noCommandLinkIcon);
			}

			public void SetDefaultButton(TDResult button) { c.nDefaultButton=button==0 ? 2 : -(int)button; } //standard button. TDResult.Cancel is 0, other TDResult values negative.
			public void SetDefaultButton(int buttonId) { c.nDefaultButton=-buttonId; } //custom button. Button ids are -idSpecified. See _MarshalButtons().

			public void SetRadioButtons(string[] buttons, int defaultId = 0, bool noDefaultButton = false)
			{
				_radioButtons=buttons;
				c.nDefaultRadioButton=defaultId;
				NO_DEFAULT_RADIO_BUTTON=noDefaultButton;
			}
			public void SetRadioButtons(string buttons, int defaultId = 0, bool noDefaultButton = false)
			{
				SetRadioButtons(_ButtonsStringToArray(buttons), defaultId, noDefaultButton);
			}

			public void SetCheckbox(string text, bool check)
			{
				c.pszVerificationText=text;
				VERIFICATION_FLAG_CHECKED=check;
			}

			public void SetExpandedText(string text, bool showInFooter = false)
			{
				EXPAND_FOOTER_AREA=showInFooter;
				c.pszExpandedInformation=text;
			}

			public void SetExpandControl(bool defaultExpanded, string collapsedText = null, string expandedText = null)
			{
				EXPANDED_BY_DEFAULT=defaultExpanded;
				c.pszCollapsedControlText=collapsedText;
				c.pszExpandedControlText=expandedText;
			}

			public void SetFooterText(string text, TDIcon icon = 0)
			{
				c.pszFooter=text;
				c.hFooterIcon=(IntPtr)(int)icon; USE_HICON_FOOTER=false;
			}
			public void SetFooterText(string text, IntPtr iconHandle)
			{
				c.pszFooter=text;
				c.hFooterIcon=iconHandle; USE_HICON_FOOTER=iconHandle!=Zero;
			}

			public int Width { set { c.cxWidth=value; } }

			int _x, _y; bool _xyIsRaw;

			public void SetXY(int x, int y, bool relativeToOwner, bool raw)
			{
				_x=x; _y=y; POSITION_RELATIVE_TO_WINDOW=relativeToOwner; _xyIsRaw =raw;
            }

			public int TimeoutS { set; private get; }
			int _timeElapsed;

			public bool FlagEnableHyperlinks { set; private get; } //TODO: event OnHyperlinkClick
			public bool FlagAllowCancel { set; private get; }
			public bool FlagRtlLayout { set; private get; }
			public bool FlagCanBeMinimized { set; private get; }

			bool USE_HICON_MAIN;
			bool USE_HICON_FOOTER;
			bool VERIFICATION_FLAG_CHECKED;
			//bool CALLBACK_TIMER;
			//bool SHOW_PROGRESS_BAR;
			//bool SHOW_MARQUEE_PROGRESS_BAR;
			bool EXPAND_FOOTER_AREA;
			bool EXPANDED_BY_DEFAULT;
			bool USE_COMMAND_LINKS;
			bool USE_COMMAND_LINKS_NO_ICON;
			bool NO_DEFAULT_RADIO_BUTTON;
			bool POSITION_RELATIVE_TO_WINDOW;

			//Get results as properties.

			public int ResultRadioButton { get; private set; }
			public bool ResultIsChecked { get; private set; }

			Wnd _hdlg; //need in hook proc

			public TDResult Show(Wnd owner = default(Wnd))
			{
				ResultRadioButton=0; ResultIsChecked=false;
				_timeElapsed=0;
				_hdlg=Zero;

				int hr = 0, R = 0;

				c.hwndParent=owner;

				TDF_ f = 0;
				if(FlagEnableHyperlinks) f|=TDF_.ENABLE_HYPERLINKS;
				if(FlagAllowCancel) f|=TDF_.ALLOW_DIALOG_CANCELLATION;
				if(FlagRtlLayout) f|=TDF_.RTL_LAYOUT;
				if(FlagCanBeMinimized) f|=TDF_.CAN_BE_MINIMIZED;
				if(USE_COMMAND_LINKS) f|=TDF_.USE_COMMAND_LINKS;
				if(USE_COMMAND_LINKS_NO_ICON) f|=TDF_.USE_COMMAND_LINKS_NO_ICON;
				if(EXPAND_FOOTER_AREA) f|=TDF_.EXPAND_FOOTER_AREA;
				if(EXPANDED_BY_DEFAULT) f|=TDF_.EXPANDED_BY_DEFAULT;
				if(NO_DEFAULT_RADIO_BUTTON) f|=TDF_.NO_DEFAULT_RADIO_BUTTON;
				if(USE_HICON_MAIN) f|=TDF_.USE_HICON_MAIN;
				if(USE_HICON_FOOTER) f|=TDF_.USE_HICON_FOOTER;
				if(VERIFICATION_FLAG_CHECKED) f|=TDF_.VERIFICATION_FLAG_CHECKED;
				if(POSITION_RELATIVE_TO_WINDOW) f|=TDF_.POSITION_RELATIVE_TO_WINDOW;
				//if(SHOW_PROGRESS_BAR) f|=TDF_.SHOW_PROGRESS_BAR;
				//if(SHOW_MARQUEE_PROGRESS_BAR) f|=TDF_.SHOW_MARQUEE_PROGRESS_BAR;
				//if(CALLBACK_TIMER) f|=TDF_.CALLBACK_TIMER;
				if(TimeoutS>0) { f|=TDF_.CALLBACK_TIMER; f|=TDF_.SHOW_PROGRESS_BAR; }
				c.dwFlags=f;

				c.pfCallback=_CallbackProc;

				IntPtr hhook=Zero; Api.HookProc hpHolder = null;

				try {
					c.pButtons=_MarshalButtons(_buttons, out c.cButtons, true);
					c.pRadioButtons=_MarshalButtons(_radioButtons, out c.cRadioButtons);

					if(TimeoutS>0) {
						//need to receive mouse and keyboard messages to stop countdown on click or key
						hhook=Api.SetWindowsHookEx(Api.WH_.CBT, hpHolder=_HookProcCBT, Zero, Api.GetCurrentThreadId());
					}

					int rRadioButton, rIsChecked;
#if DEBUG
					hr = _CallTDI(ref c, out R, out rRadioButton, out rIsChecked);
#else
					hr = TaskDialogIndirect(ref c, out R, out _rRadioButton, out _rIsChecked);
#endif
					ResultRadioButton=rRadioButton; ResultIsChecked=rIsChecked!=0;

				} finally {
					if(hhook!=Zero) Api.UnhookWindowsHookEx(hhook);

					_MarshalFreeButtons(ref c.pButtons, ref c.cButtons);
					_MarshalFreeButtons(ref c.pRadioButtons, ref c.cRadioButtons);
				}

				if(hr!=0) throw new Win32Exception(hr);

				if(R==2) R=0; else R=-R; //user button ids are -idSpecified; TDResult.Cancel is 0, other TDResult values negative. See _MarshalButtons().
				return (TDResult)R;
			}

			int _CallbackProc(Wnd w, TDN_ code, LPARAM wParam, LPARAM lParam, IntPtr data)
			{
				//Out(code);
				switch(code) {
				case TDN_.TDN_CREATED:
					if(TimeoutS>0) {
						_hdlg=w;
                        w.Send((uint)TDM_.SET_PROGRESS_BAR_POS, 100, 0); //right-to-left; tested: cannot prevent the initial inertion.
                    }
					break;
				case TDN_.TDN_TIMER:
					if(TimeoutS>0 && _timeElapsed>=0) {
						int timeoutTicks = TimeoutS*5; _timeElapsed++; //200 ms timer ticks
						if(_timeElapsed<timeoutTicks) {
							w.Send((uint)TDM_.SET_PROGRESS_BAR_POS, 100-Calc.Percent(timeoutTicks, _timeElapsed), 0);
						} else {
							//Out("timeout");
							_timeElapsed=-1;
                            w.Send((uint)TDM_.CLICK_BUTTON, -(int)TDResult.Timeout, 0);
                        }
                    }
					break;
				}
				//Out(code);
				return 0;
			}

			IntPtr _HookProcCBT(int code, LPARAM wParam, IntPtr lParam)
			{
				switch((Api.HCBT_)code) {
				case Api.HCBT_.CLICKSKIPPED:
					switch((WM_)(int)wParam) { case WM_.LBUTTONDOWN: case WM_.NCLBUTTONDOWN: goto case Api.HCBT_.KEYSKIPPED; }
					//TODO: only if _hdlg
					break;
				case Api.HCBT_.KEYSKIPPED:
					_timeElapsed=-1;
					break;
                }
				return Api.CallNextHookEx(Zero, code, wParam, lParam);
            }

			//LPARAM SubclassProc(Wnd hWnd, WM_ msg, LPARAM wParam, LPARAM lParam, LPARAM uIdSubclass, IntPtr dwRefData)
			//{

			//}

			#region forget
#if DEBUG //TODO: consider: use this func always.
			//The API throws 'access violation' exception if some value is invalid (eg unknown flags in dwCommonButtons) or it does not like something.
			//.NET does not allow to handle such exceptions, unless we use [HandleProcessCorruptedStateExceptions] or <legacyCorruptedStateExceptionsPolicy enabled="true"/> in config file.
			//It makes dev/debug more difficult.
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			static int _CallTDI(ref TASKDIALOGCONFIG c, out int pnButton, out int pnRadioButton, out int pChecked)
			{
				pnButton=pnRadioButton=pChecked=0;
				try {
					return TaskDialogIndirect(ref c, out pnButton, out pnRadioButton, out pChecked);
				} catch(Exception e) { throw new Win32Exception($"_CallTDI: {e.Message}"); } //note: not just throw;, and don't add inner exception
			}
#endif

			static IntPtr _MarshalButtons(string[] a, out int cButtons, bool escapeId = false)
			{
				if(a==null || a.Length==0) { cButtons=0; return Zero; }
				cButtons=a.Length;

				int structSize = Marshal.SizeOf(typeof(TASKDIALOG_BUTTON));
				IntPtr R = Marshal.AllocHGlobal(structSize * a.Length);

				for(int i = 0; i < a.Length; i++) {
					TASKDIALOG_BUTTON b; b.id = a[i].ToInt_(out b.text); //"id text" -> TASKDIALOG_BUTTON

					if(string.IsNullOrEmpty(b.text)) { b.text=a[i]; if(string.IsNullOrEmpty(b.text)) b.text=" "; } //exception if null or ""

					if(escapeId) { b.id=-b.id; if(b.id>0) throw new ArgumentException("button id < 0"); } //because 2==IDCANCEL, and most popular custom ids will be 1, 2, 3...

					unsafe { Marshal.StructureToPtr(b, (IntPtr)((byte*)R + (structSize * i)), false); }
				}

				return R;
			}

			static void _MarshalFreeButtons(ref IntPtr a, ref int cButtons)
			{
				if(a==Zero) return;

				int structSize = Marshal.SizeOf(typeof(TASKDIALOG_BUTTON));

				for(int i = 0; i < cButtons; i++) {
					unsafe { Marshal.DestroyStructure((IntPtr)((byte*)a + (structSize * i)), typeof(TASKDIALOG_BUTTON)); }
				}

				Marshal.FreeHGlobal(a);
				a=Zero; cButtons=0;
			}
			#endregion
		} //AdvancedTaskDialog

		static string[] _ButtonsStringToArray(string buttons)
		{
			return buttons.Replace("\r\n", "\n").Replace("\n|", "|").Trim_("\r\n|").Split('|');
			//info: the API adds 2 newlines for \r\n. Only for custom buttons, not for other controls/parts.
		}

		//_________________________________________________________________

		//InputDialog


		public static bool InputDialog(string s)
		{
			return false;
		}
	}
}
