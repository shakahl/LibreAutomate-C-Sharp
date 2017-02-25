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
	/// Standard dialogs to show information or get user input.
	/// You can use static functions (less code) or create class instances (more options).
	/// More info: <see cref="ShowEx"/>.
	/// </summary>
	/// <remarks>
	/// Uses API <msdn>TaskDialogIndirect</msdn>.
	/// Don't use in services, because the dialog will not be visible. In services you can use MessageBox.Show with flag MessageBoxOptions.ServiceNotification or MessageBoxOptions.DefaultDesktopOnly, or the MessageBox API with corresponding flags.
	/// </remarks>
	/// <example>
	/// This example creates a class instance, sets properties, shows dialog, uses events, uses result.
	/// <code><![CDATA[
	/// var d = new TaskDialog(); //info: another constructor has the same parameters as ShowEx
	/// d.SetText("Main text.", "More text.\nSupports &lt;A HREF=\"link data\"&gt;links&lt;/A&gt; if you subscribe to HyperlinkClicked event.");
	/// d.SetButtons("1 OK|2 Cancel|3 Custom|4 Custom2");
	/// d.SetIcon(TDIcon.Warning);
	/// d.SetExpandedText("Expanded info\nand more info.", true);
	/// d.FlagCanBeMinimized = true;
	/// d.SetRadioButtons("1 r1|2 r2");
	/// d.SetCheckbox("Check");
	/// d.SetTimeout(30, "OK");
	/// d.HyperlinkClicked += e =&gt; { TaskDialog.Show("link clicked", e.LinkHref, owner: e.hwnd); };
	/// d.ButtonClicked += e =&gt; { Print(e.Button); if(e.Button == 4) e.DoNotCloseDialog = true; };
	/// d.FlagShowProgressBar = true; d.Timer += e =&gt; { e.dialog.Send.Progress(e.TimerTimeMS / 100); };
	/// var r = d.ShowDialog();
	/// Print(r);
	/// switch(r.Button) { case 1: Print("OK"); break; case TDResult.Timeout: Print("timeout"); break; }
	/// ]]></code>
	/// </example>
	//[DebuggerStepThrough]
	public partial class TaskDialog
	{
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
			/// Reference: <msdn>task dialog messages</msdn>.
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
			/// Reference: <msdn>task dialog notifications</msdn>.
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

		//TASKDIALOGCONFIG buttons.
		[Flags]
		enum TDCBF_
		{
			OK = 1, Yes = 2, No = 4, Cancel = 8, Retry = 0x10, Close = 0x20,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct TASKDIALOG_BUTTON
		{
			public int id;
			public char* text;
		}

		delegate int TaskDialogCallbackProc(Wnd hwnd, TDApi.TDN notification, LPARAM wParam, LPARAM lParam, IntPtr data);

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct TASKDIALOGCONFIG
		{
			public uint cbSize;
			public Wnd hwndParent;
			public IntPtr hInstance;
			public TDF_ dwFlags;
			public TDCBF_ dwCommonButtons;
			public string pszWindowTitle;
			public IntPtr hMainIcon;
			public string pszMainInstruction;
			public string pszContent;
			public int cButtons;
			public TASKDIALOG_BUTTON* pButtons;
			public int nDefaultButton;
			public int cRadioButtons;
			public TASKDIALOG_BUTTON* pRadioButtons;
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
				get => _defaultTitle ?? AppDomain.CurrentDomain.FriendlyName;
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
			/// If there is no owner window, let the dialog be always on top of most other windows.
			/// </summary>
			/// <seealso cref="FlagTopmost"/>
			public static bool TopmostIfNoOwnerWindow { get; set; }

			/// <summary>
			/// Show dialogs on this screen when screen is not explicitly specified (<see cref="Screen"/>) and there is no owner window.
			/// </summary>
			public static object DefaultScreen { get; set; }

			/// <summary>
			/// If icon not specified, use <see cref="TDIcon.App"/>.
			/// </summary>
			public static bool UseAppIcon { get; set; }

			/// <summary>
			/// If owner window not specified, use the active window of current thread as owner window (disable it, etc).
			/// </summary>
			/// <seealso cref="SetOwnerWindow"/>
			public static bool AutoOwnerWindow { get; set; }
		}

		#endregion static options

		TASKDIALOGCONFIG _c;

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
			string text1 = null, string text2 = null, string buttons = null, TDIcon icon = 0, TDFlags flags = 0, Types<Control, Wnd>? owner = null,
			string expandedText = null, string footerText = null, string title = null, string radioButtons = null, string checkBox = null,
			int defaultButton = 0, int x = 0, int y = 0, int timeoutS = 0, Action<TDEventArgs> onLinkClick = null
			) : this()
		{
			FlagEndThread = 0 != (flags & TDFlags.EndThread);
			if(0 != (flags & TDFlags.Topmost)) FlagTopmost = true; //else use Options.TopmostIfNoOwnerWindow if no owner
			FlagXCancel = 0 != (flags & TDFlags.XCancel);
			if(0 != (flags & TDFlags.Wider)) Width = 500;
			//FlagKeyboardShortcutsVisible=0 != (flags&TDFlags.KeyboardShortcutsVisible);

			SetText(text1, text2);
			SetIcon(icon);
			SetButtons(buttons, 0 != (flags & TDFlags.CommandLinks));
			if(defaultButton != 0) DefaultButton = defaultButton;
			SetRadioButtons(radioButtons);
			_SetCheckboxFromText(checkBox);
			if(owner != null) SetOwnerWindow(owner.GetValueOrDefault(), 0 != (flags & TDFlags.OwnerCenter));
			SetXY(x, y, 0 != (flags & TDFlags.RawXY));
			SetTimeout(timeoutS);
			SetExpandedText(expandedText, 0 != (flags & TDFlags.ExpandDown));
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
			return (_c.dwFlags & flag) != 0;
		}

		/// <summary>
		/// Changes title bar text.
		/// If you don't call this method or title is null or "", dialogs will use <see cref="Options.DefaultTitle"/>.
		/// </summary>
		public void SetTitleBarText(string title)
		{
			_c.pszWindowTitle = Empty(title) ? Options.DefaultTitle : title;
			//info: if "", API uses "ProcessName.exe".
		}

		/// <summary>
		/// Sets text.
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		public void SetText(string text1 = null, string text2 = null)
		{
			_c.pszMainInstruction = text1;
			_c.pszContent = text2;
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
		/// <param name="icon">System.Drawing.Icon or IntPtr (native icon handle). Size should be 32 or 16. The icon must remain valid until the dialog is closed, then you can dispose/destroy it.</param>
		public void SetIcon(Types<Icon, IntPtr> icon)
		{
			_c.hMainIcon = icon.type == 1 ? icon.v1.Handle : icon.v2;
			_SetFlag(TDF_.USE_HICON_MAIN, _c.hMainIcon != Zero);
			//tested: displays original-size 32 and 16 icons, but shrinks bigger icons to 32.
			//note: for App icon ShowDialog will execute more code. The same for footer icon.
		}

		#region buttons

		const int _idOK = 1;
		const int _idCancel = 2;
		const int _idRetry = 4;
		const int _idYes = 6;
		const int _idNo = 7;
		const int _idClose = 8;
		const int _idTimeout = int.MinValue;

		_Buttons _buttons;

		struct _Buttons
		{
			struct _Button
			{
				internal int id;
				internal string s;

				internal _Button(int id, string s) { this.id = id; this.s = s; }
			}
			List<_Button> _customButtons, _radioButtons;

			static char[] _sep = { '|' };
			static char[] _newlines = { '\r', '\n' };

			int _defaultButtonUserId;
			bool _isDefaultButtonSet;
			internal int DefaultButtonUserId { get => _defaultButtonUserId; set { _defaultButtonUserId = value; _isDefaultButtonSet = true; } }

			bool _hasXButton;

			internal TDCBF_ SetButtons(string buttons, Types<string, IEnumerable<string>> customButtons)
			{
				_customButtons = null;
				_mapIdUserNative = null;
				_defaultButtonUserId = 0;
				_isDefaultButtonSet = false;

				switch(customButtons.type) {
				case 1:
					_ParseStringList(customButtons.v1, true);
					break;
				case 2:
					int id = 0;
					foreach(var v in customButtons.v2) {
						if(_customButtons == null) _customButtons = new List<_Button>();
						string s = _ParseSingleString(v, ref id, true);
						_customButtons.Add(new _Button(id, s));
						DefaultButtonUserId = 1;
					}
					break;
				}

				return _ParseStringList(buttons, false);
			}

			TDCBF_ _ParseStringList(string b, bool onlyCustom)
			{
				if(Empty(b)) return 0;

				TDCBF_ commonButtons = 0;
				int id = 0, nextNativeId = 100;

				foreach(var v in b.Split(_sep)) {
					string s = _ParseSingleString(v, ref id, onlyCustom);

					int nativeId = 0;
					if(!onlyCustom) {
						switch(s) {
						case "OK": commonButtons |= TDCBF_.OK; nativeId = _idOK; break;
						case "Yes": commonButtons |= TDCBF_.Yes; nativeId = _idYes; break;
						case "No": commonButtons |= TDCBF_.No; nativeId = _idNo; break;
						case "Cancel": commonButtons |= TDCBF_.Cancel; nativeId = _idCancel; break;
						case "Retry": commonButtons |= TDCBF_.Retry; nativeId = _idRetry; break;
						case "Close": commonButtons |= TDCBF_.Close; nativeId = _idClose; break;
						}
					}

					if(nativeId == 0) { //custom button
						if(_customButtons == null) _customButtons = new List<_Button>();
						_customButtons.Add(new _Button(id, s));
						if(id < 0) nativeId = nextNativeId++; //need to map, because native ids of positive user ids are minus user ids
					}
					if(nativeId != 0) {
						if(_mapIdUserNative == null) _mapIdUserNative = new List<_IdMapItem>();
						_mapIdUserNative.Add(new _IdMapItem(id, nativeId));
					}

					if(!_isDefaultButtonSet) DefaultButtonUserId = id;
				}

				return commonButtons;
			}

			internal void SetRadioButtons(string buttons)
			{
				_radioButtons = null;
				if(Empty(buttons)) return;

				_radioButtons = new List<_Button>();
				int id = 0;
				foreach(var v in buttons.Split(_sep)) {
					string s = _ParseSingleString(v, ref id, false);
					_radioButtons.Add(new _Button(id, s));
				}
			}

			static string _ParseSingleString(string s, ref int id, bool dontSplit)
			{
				string r = null;
				if(!dontSplit) { int i = s.ToIntAndString_(out r); if(r != null) id = i; }
				if(r == null) { r = s; id++; }
				r = r.Trim(_newlines); //API does not like newline at start, etc
				if(r.Length == 0) r = " "; //else API exception
				else r = r.Replace("\r\n", "\n"); //API adds 2 newlines for \r\n. Only for custom buttons, not for other controls/parts.
				return r;
			}

			struct _IdMapItem
			{
				internal int userId, nativeId;

				internal _IdMapItem(int userId, int nativeId) { this.userId = userId; this.nativeId = nativeId; }
			}

			List<_IdMapItem> _mapIdUserNative;

			internal int MapIdUserToNative(int userId)
			{
				if(userId == _idTimeout) return userId; //0x80000000
				if(_mapIdUserNative != null) { //common buttons, and custom buttons with negative user id
					foreach(var v in _mapIdUserNative) if(v.userId == userId) return v.nativeId;
				}
				return -userId; //custom button with positive user id
			}

			internal int MapIdNativeToUser(int nativeId)
			{
				if(nativeId == _idTimeout) return nativeId; //0x80000000
				if(nativeId <= 0) return -nativeId; //custom button with positive user id
				if(_mapIdUserNative != null) { //common buttons, and custom buttons with negative user id
					foreach(var v in _mapIdUserNative) if(v.nativeId == nativeId) return v.userId;
				}
				if(nativeId == _idOK) return nativeId; //single OK button auto-added when no buttons specified
				Debug.Assert(nativeId == _idCancel && _hasXButton);
				return 0;
			}

			/// <summary>
			/// Sets c.pButtons, c.cButtons, c.pRadioButtons and c.cRadioButtons.
			/// Later call MarshalFreeButtons.
			/// </summary>
			internal unsafe void MarshalButtons(ref TASKDIALOGCONFIG c)
			{
				c.pButtons = _MarshalButtons(false, out c.cButtons);
				c.pRadioButtons = _MarshalButtons(true, out c.cRadioButtons);

				_hasXButton = ((c.dwFlags & TDF_.ALLOW_DIALOG_CANCELLATION) != 0);
			}

			/// <summary>
			/// Frees memory allocated by MarshalButtons and sets the c members to null/0.
			/// </summary>
			internal unsafe void MarshalFreeButtons(ref TASKDIALOGCONFIG c)
			{
				Marshal.FreeHGlobal((IntPtr)c.pButtons);
				Marshal.FreeHGlobal((IntPtr)c.pRadioButtons);
				c.pButtons = null; c.pRadioButtons = null;
				c.cButtons = 0; c.cRadioButtons = 0;
			}

			unsafe TASKDIALOG_BUTTON* _MarshalButtons(bool radio, out int nButtons)
			{
				var a = radio ? _radioButtons : _customButtons;
				int n = a == null ? 0 : a.Count;
				nButtons = n;
				if(n == 0) return null;
				int nba = n * Marshal.SizeOf(typeof(TASKDIALOG_BUTTON)), nb = nba;
				foreach(var v in a) nb += (v.s.Length + 1) * 2;
				var r = (TASKDIALOG_BUTTON*)Marshal.AllocHGlobal(nb);
				char* s = (char*)((byte*)r + nba);
				for(int i = 0; i < n; i++) {
					var v = a[i];
					r[i].id = radio ? v.id : MapIdUserToNative(v.id);
					int len = v.s.Length + 1;
					r[i].text = Api.lstrcpyn(s, v.s, len);
					s += len;
				}
				return r;
			}
		}

		/// <summary>
		/// Sets common and/or custom buttons and custom buttons style.
		/// </summary>
		/// <param name="buttons">
		/// Common and/or custom buttons, like with <see cref="Show"/>.
		/// These ids should be negative if you use customButtons too, because ids of customButtons are 1, 2, ... .
		/// </param>
		/// <param name="asCommandLinks">Custom buttons style. If false - row of classic buttons. If true - column of command-link buttons that can have multiline text.</param>
		/// <param name="customButtons">
		/// Additional custom buttons. All will be custom, even if named "OK" etc.
		/// List of labels without ids. Can be string like "One|Two|..." or IEnumerable&lt;string&gt;.
		/// Button ids will be 1, 2, ... . DefaultButton will be 1 (you can change it later).
		/// </param>
		public void SetButtons(string buttons, bool asCommandLinks = false, Types<string, IEnumerable<string>>? customButtons = null)
		{
			_c.dwCommonButtons = _buttons.SetButtons(buttons, customButtons.GetValueOrDefault());
			_SetFlag(TDF_.USE_COMMAND_LINKS, asCommandLinks);
		}

		/// <summary>
		/// Specifies which button responds to the Enter key.
		/// If 0 or not set, auto-selects.
		/// </summary>
		/// <value>Button id.</value>
		public int DefaultButton { set { _c.nDefaultButton = _buttons.MapIdUserToNative(value); } }

		/// <summary>
		/// Adds radio buttons.
		/// To get selected radio button id after closing the dialog, use the RadioButton property of the <see cref="TDResult"/> variable returned by <see cref="ShowDialog"/> or the <see cref="Result"/> property.
		/// </summary>
		/// <param name="buttons">A list of strings "id text" separated by |, like "1 One|2 Two|3 Three".</param>
		/// <param name="defaultId">Check the radio button that has this id. If omitted or 0, checks the first. If negative, does not check.</param>
		public void SetRadioButtons(string buttons, int defaultId = 0)
		{
			//_radioButtonsStr = buttons;
			_buttons.SetRadioButtons(buttons);
			_c.nDefaultRadioButton = defaultId;
			_SetFlag(TDF_.NO_DEFAULT_RADIO_BUTTON, defaultId < 0);
		}

		#endregion buttons

		/// <summary>
		/// Adds check box (if text is not null/empty).
		/// To get check box state after closing the dialog, use the IsChecked property of the <see cref="TDResult"/> variable returned by <see cref="ShowDialog"/> or the <see cref="Result"/> property.
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
		/// Set properties of the control that shows and hides text added by <see cref="SetExpandedText">SetExpandedText</see>.
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
		/// <param name="icon">System.Drawing.Icon or IntPtr (native icon handle). Size should be 16. The icon must remain valid until the dialog is closed, then you can dispose/destroy it.</param>
		public void SetFooterText(string text, Types<Icon, IntPtr> icon)
		{
			_c.pszFooter = text;
			_c.hFooterIcon = icon.type == 1 ? icon.v1.Handle : icon.v2;
			_SetFlag(TDF_.USE_HICON_FOOTER, _c.hFooterIcon != Zero);
		}

		/// <summary>
		/// Adds Edit or Combo control (if editType is not TDEdit.None (0)).
		/// To get its text after closing the dialog, use the EditText property of the <see cref="TDResult"/> variable returned by <see cref="ShowDialog"/> or the <see cref="Result"/> property.
		/// </summary>
		/// <param name="editType">Control type/style.</param>
		/// <param name="editText">Initial text. If editType is TDEdit.Combo, it can be a string array, List or IEnumerable; the first item sets combo-box editable text, other items - combo box drop-down list items.</param>
		/// <remarks>
		/// The API TaskDialogIndirect does not have an option to add an edit control. This class itself creates it.
		/// Does not support progress bar.
		/// </remarks>
		public void SetEditControl(TDEdit editType, object editText = null)
		{
			_editType = editType;
			_editText = editText;
			//will set other props later, because need to override user-set props
		}
		TDEdit _editType; object _editText;

		/// <summary>
		/// Sets the width of the dialog's client area.
		/// The actual width will depend on DPI (the Windows "text size" setting).
		/// If less than default width, will be used default width.
		/// </summary>
		public int Width { set { _c.cxWidth = value / 2; } }
		//tested: TDIF_SIZE_TO_CONTENT can make wider, but not much. The flag is obsolete.

		/// <summary>
		/// Sets owner window.
		/// The owner window will be disabled, and this dialog will be on top of it.
		/// This window will be in owner's screen, if screen was not explicitly specified with the <see cref="Screen"/> property. <see cref="TaskDialog.Options.DefaultScreen"/> is ignored.
		/// </summary>
		/// <param name="owner">Owner window, or one of its child/descendant controls. Can be Control (eg Form) or Wnd (window handle).</param>
		/// <param name="ownerCenter">Show the dialog in the center of the owner window. <see cref="SetXY">SetXY</see> and <see cref="Screen">Screen</see> are ignored.</param>
		/// <param name="doNotDisable">Don't disable the owner window. If false, disables if it belongs to this thread.</param>
		/// <seealso cref="Options.AutoOwnerWindow"/>
		public void SetOwnerWindow(Types<Control, Wnd> owner, bool ownerCenter = false, bool doNotDisable = false)
		{
			Wnd w = Wnd0;
			switch(owner.type) {
			case 1: w = (Wnd)owner.v1?.Handle; break;
			case 2: w = owner.v2; break;
			}

			_c.hwndParent = w.WndWindow;
			_SetFlag(TDF_.POSITION_RELATIVE_TO_WINDOW, ownerCenter);
			_enableOwner = doNotDisable;
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
			_x = x; _y = y;
			_rawXY = rawXY;
		}

		int _x, _y; bool _rawXY;

		/// <summary>
		/// Sets the screen (display monitor) where to show the dialog in multi-screen environment.
		/// If null or not set, will be used owner window's screen or <see cref="Options.DefaultScreen"/>.
		/// More info: <see cref="Screen_.FromObject"/>, <see cref="Wnd.MoveInScreen"/>.
		/// </summary>
		public object Screen { set; private get; }

		/// <summary>
		/// Let the dialog close itself after closeAfterS seconds.
		/// On timeout ShowDialog returns TDResult.Timeout.
		/// Example: <c>d.SetTimeout(30, "OK");</c>
		/// </summary>
		public void SetTimeout(int closeAfterS, string timeoutActionText = null, bool noInfo = false)
		{
			_timeoutS = closeAfterS;
			_timeoutActionText = timeoutActionText;
			_timeoutNoInfo = noInfo;
		}
		int _timeoutS; bool _timeoutActive, _timeoutNoInfo; string _timeoutActionText, _timeoutFooterText;

		/// <summary>
		/// Allow to cancel even if there is no Cancel button.
		/// It adds X (Close) button to the title bar, and also allows to close the dialog with the Esc key.
		/// When the dialog is closed with the X button or Esc, the returned result button id is 0 if there is no Cancel button, else the same as of Cancel button.
		/// The same as <see cref="TDFlags.XCancel"/>.
		/// </summary>
		public bool FlagXCancel { set; private get; }

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
		/// If null (default), the dialog will be topmost if both these are true: no owner window, TaskDialog.Options.TopmostIfNoOwnerWindow is true.
		/// </summary>
		public bool? FlagTopmost { set; private get; }

		/// <summary>
		/// Call <see cref="Thread.Abort()"/> if selected OK button when there are no other buttons. Also when selected Cancel, No, and on timeout.
		/// </summary>
		public bool FlagEndThread { set; private get; }

		///// <summary>
		///// Show keyboard shortcuts (underlined characters), like when you press the Alt key.
		///// Tip: to create keyboard shortcuts for custom buttons, use &amp; character, like "&amp;One|&amp;Two|T&amp;hree".
		///// </summary>
		///// <seealso cref="TDFlags.KeyboardShortcutsVisible"/>
		//public bool FlagKeyboardShortcutsVisible { set; private get; }

		#endregion set properties

		Wnd _dlg;
		int _threadIdInShow;

		/// <summary>
		/// Shows the dialog.
		/// The return value is the same as of <see cref="ShowEx"/>.
		/// Call this method after setting text and other properties.
		/// </summary>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public unsafe TDResult ShowDialog()
		{
			//info: named ShowDialog, not Show, to not confuse with the static Show() which is used almost everywhere in documentation.

			_result = null;
			_isClosed = false;

			SetTitleBarText(_c.pszWindowTitle); //if not set, sets default
			_EditControlInitBeforeShowDialog(); //don't reorder, must be before flags

			if(_c.hwndParent.Is0 && Options.AutoOwnerWindow) _c.hwndParent = Wnd.Misc.WndActiveOfThisThread; //info: MessageBox.Show also does it, but it also disables all thread windows
			if(_c.hwndParent.IsAlive) {
				if(!_enableOwner && !_c.hwndParent.IsOfThisThread) _enableOwner = true;
				if(_enableOwner && !_c.hwndParent.IsEnabled) _enableOwner = false;
			}

			_SetFlag(TDF_.ALLOW_DIALOG_CANCELLATION, FlagXCancel);
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
			int rNativeButton = 0, rRadioButton = 0, rIsChecked = 0, hr = 0;
			bool hasCustomButtons = false;
			IntPtr actCtxCookie = Zero;

			try {
				_threadIdInShow = Thread.CurrentThread.ManagedThreadId;

				_buttons.MarshalButtons(ref _c);
				if(_c.pButtons == null) _SetFlag(TDF_.USE_COMMAND_LINKS | TDF_.USE_COMMAND_LINKS_NO_ICON, false); //to avoid exception
				else hasCustomButtons = true;

				if(_timeoutActive) { //Need mouse/key messages to stop countdown on click or key.
					hhook = Api.SetWindowsHookEx(Api.WH_GETMESSAGE, hpHolder = _HookProc, Zero, Api.GetCurrentThreadId());
				}

				Wnd.LibAllowActivate(true);

				//Activate manifest that tells to use comctl32.dll version 6. The API is unavailable in version 5.
				//Need this if the host app does not have such manifest, eg if uses the default manifest added by Visual Studio.
				actCtxCookie = Util.LibActCtx.Activate();

				for(int i = 0; i < 100; i++) { //see the API bug-workaround comment below
					hr = _CallTDI(out rNativeButton, out rRadioButton, out rIsChecked);
					//TaskDialog[Indirect] API bug: if called simultaneously by 2 threads, fails and returns an unknown error code 0x800403E9.
					//Workaround: retry. Also retry for other errors, eg E_OUTOFMEMORY and all other unexpected errors.
					//In all my tests with 2 threads, was enough 1 retry, but in real life may be more threads.
					if(hr == 0 || hr == Api.E_INVALIDARG || !_dlg.Is0) break; //about _dlg.Is0: _dlg is set if our callback function was called; then don't retry, because the dialog was possibly shown, and only then error.
					Time.WaitMS(30); //100*30=3000
				}

				if(hr == 0) {
					_result = new TDResult(_buttons.MapIdNativeToUser(rNativeButton), rRadioButton, rIsChecked != 0, _editText?.ToString());

					Wnd.Misc.WaitForAnActiveWindow();
				}
			}
			finally {
				//Normally the dialog now is destroyed and _dlg now is 0, because _SetClosed called on the destroy message.
				//But on Thread.Abort or other exception it is not called and the dialog is still alive and visible.
				//Therefore Windows shows its annoying "stopped working" UI.
				//To avoid it, destroy the dialog now. Also to avoid possible memory leaks etc.
				//However still bad if we use a timer (of TaskDialog API or own).
				if(!_dlg.Is0) Api.DestroyWindow(_dlg);

				_SetClosed();

				_threadIdInShow = 0;

				//DeactivateActCtx throws SEHException on Thread.Abort, don't know why.
				try { Util.LibActCtx.Deactivate(actCtxCookie); } catch(SEHException) { /*DebugPrint("exc");*/ }

				if(hhook != Zero) Api.UnhookWindowsHookEx(hhook);

				_buttons.MarshalFreeButtons(ref _c);
			}

			if(hr != 0) throw new Win32Exception(hr);

			if(FlagEndThread) {
				bool endThread = false;
				switch(rNativeButton) {
				case _idCancel: case _idNo: case TDResult.Timeout: endThread = true; break;
				case _idOK: endThread = (_c.dwCommonButtons == 0 || _c.dwCommonButtons == TDCBF_.OK) && !hasCustomButtons; break;
				}

				if(endThread) Thread.CurrentThread.Abort();
			}

			return _result;
		}

		[HandleProcessCorruptedStateExceptions]
		int _CallTDI(out int pnButton, out int pnRadioButton, out int pChecked)
		{
#if DEBUG
			pnButton = pnRadioButton = pChecked = 0;
			try {
#endif
				return TaskDialogIndirect(ref _c, out pnButton, out pnRadioButton, out pChecked);
#if DEBUG
			}
			catch(Exception e) when(!(e is ThreadAbortException)) {
				throw new Win32Exception($"_CallTDI: {e.Message}"); //note: not just throw;, and don't add inner exception
			}

			//The API throws 'access violation' exception if some value is invalid (eg unknown flags in dwCommonButtons) or it does not like something.
			//.NET does not allow to handle such exceptions, unless we use [HandleProcessCorruptedStateExceptions] or <legacyCorruptedStateExceptionsPolicy enabled="true"/> in config file.
			//It makes dev/debug more difficult.
#endif
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
				//Print(w.IsAlive); //valid
				e = Destroyed;
				break;
			case TDApi.TDN.CREATED:
				if(_enableOwner) _c.hwndParent.Enable(true);

				if(!_HasFlag(TDF_.POSITION_RELATIVE_TO_WINDOW)) {
					object scr = Screen; if(scr == null && _c.hwndParent.Is0) scr = Options.DefaultScreen;
					if((_x != 0 || _y != 0 || _rawXY || scr != null)) {
						w.MoveInScreen(_x, _y, scr, rawXY: _rawXY);
					}
				}

				bool topmost = false;
				if(FlagTopmost != null) topmost = FlagTopmost.Value; else if(_c.hwndParent.Is0) topmost = Options.TopmostIfNoOwnerWindow;
				if(topmost) w.ZorderTopmost();

				//w.SetStyleAdd(Native.WS_THICKFRAME); //does not work

				if(_IsEdit) _EditControlCreate();

				//if(FlagKeyboardShortcutsVisible) w.Post(Api.WM_UPDATEUISTATE, 0x30002);

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
			case TDApi.TDN.BUTTON_CLICKED:
				e = ButtonClicked;
				wParam = _buttons.MapIdNativeToUser(wParam);
				break;
			case TDApi.TDN.HYPERLINK_CLICKED:
				e = HyperlinkClicked;
				break;
			case TDApi.TDN.HELP:
				e = HelpF1;
				break;
			default:
				e = OtherEvents;
				break;
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
		/// Raised when the internal <msdn>TaskDialogCallbackProc</msdn> function is called by the task dialog API.
		/// </summary>
		public event Action<TDEventArgs>
			Created, Destroyed, Timer, ButtonClicked, HyperlinkClicked, HelpF1, OtherEvents;

		/// <summary>
		/// Arguments for TaskDialog event handlers.
		/// More info: <msdn>TaskDialogCallbackProc</msdn>.
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
			public int Button { get => wParam; }

			/// <summary>
			/// Dialog timer time in milliseconds. Use in Timer event handler.
			/// The event handler can set returnValue=1 to reset this.
			/// </summary>
			public int TimerTimeMS { get => wParam; }

			/// <summary>
			/// Your ButtonClicked event handler function can use this to prevent closing the dialog.
			/// </summary>
			public bool DoNotCloseDialog { set { returnValue = value ? 1 : 0; } }

			/// <summary>
			/// Gets or sets edit field text.
			/// </summary>
			public string EditText
			{
				get => dialog.EditControl.ControlText;
				set { dialog.EditControl.SetText(value); }
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
		/// <exception cref="CatException">Failed to show dialog.</exception>
		public void ShowNoWait(Action<TDResult> whenClosed = null, bool closeOnExit = false)
		{
			//note: not Task.Run, because we need STA, etc.
			var t = new Thread(() =>
			  {
				  ShowDialog(); //TODO: try/catch
				  if(whenClosed != null) {
					  Control c = null;
					  try {
						  var w = _c.hwndParent;
						  if(w.IsAlive && (c = (Control)w) != null) c.BeginInvoke(whenClosed, Result);
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
		/// Gets selected button id and other results packed in a TDResult variable.
		/// It is the same variable as the ShowDialog return value.
		/// If the result is still unavailable (eg the dialog still not closed):
		///		If called from the same thread that called ShowDialog, returns null.
		///		If called from another thread, waits until the dialog is closed and the return value is available.
		///		Note that ShowNoWait calls ShowDialog in another thread.
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
				while(_threadIdInShow != 0) Time.WaitMS(15);
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
				Time.WaitMS(15); //need ~3 loops if 15
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
				Time.WaitMS(30);
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
		public bool IsOpen { get => !_dlg.Is0; }

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
		public Wnd DialogWindow { get => _dlg; }

		/// <summary>
		/// Used to send task dialog API messages, like <c>td.Send.Message(TaskDialog.TDApi.TDM.CLICK_VERIFICATION, 1);</c> .
		/// Before showing the dialog returns null. After closing the dialog the returned variable is deactivated; its method calls are ignored.
		/// </summary>
		public TDMessageSender Send { get; private set; }

		/// <summary>
		/// Used to send task dialog API messages, like <c>td.Send.Message(TaskDialog.TDApi.TDM.CLICK_VERIFICATION, 1);</c> .
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
			/// Reference: <msdn>task dialog messages</msdn>.
			/// NAVIGATE_PAGE currently not supported.
			/// </summary>
			public int Message(TDApi.TDM message, LPARAM wParam = default(LPARAM), LPARAM lParam = default(LPARAM))
			{
				var td = _tdo; if(td == null) return 0;

				switch(message) {
				case TDApi.TDM.CLICK_BUTTON:
				case TDApi.TDM.ENABLE_BUTTON:
				case TDApi.TDM.SET_BUTTON_ELEVATION_REQUIRED_STATE:
					wParam = td._buttons.MapIdUserToNative(wParam);
					break;
				}

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
			/// <param name="buttonId">A button id or some other number that will be returned by ShowDialog.</param>
			public bool Close(int buttonId = 0)
			{
				return 0 != Message(TDApi.TDM.CLICK_BUTTON, buttonId);
			}

			/// <summary>
			/// Enables or disables a button.
			/// Call this method while the dialog is open, eg in an event handler.
			/// Example: <c>d.Created += e => { e.dialog.Send.EnableButton(4, false); };</c>
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

		#region hookProc, timeoutText

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
				if(_timeoutActive && m.hwnd.WndWindow == _dlg) {
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

		#endregion hookProc, timeoutText

		#region Edit control

		bool _IsEdit { get => _editType != TDEdit.None; }

		void _EditControlInitBeforeShowDialog()
		{
			if(!_IsEdit) return;
			FlagShowMarqueeProgressBar = true;
			FlagShowProgressBar = false;
			if(_c.pszContent == null) _c.pszContent = "";
			if(_c.pszExpandedInformation != null && _editType == TDEdit.Multiline) _SetFlag(TDF_.EXPAND_FOOTER_AREA, true);

			//create font and calculate control height
			//note: don't use system messagebox font. TaskDialog API does not depend on it.
			_editFont = new Util.LibNativeFont("Verdana", 9, true);
		}

		void _EditControlUpdate(bool onlyZorder = false)
		{
			if(_editWnd.Is0) return;
			if(!onlyZorder) {
				_EditControlGetPlace(out RECT r);
				_editParent.LibMove(r.left, r.top, r.Width, r.Height);
				_editWnd.LibMove(0, 0, r.Width, r.Height);
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
			Wnd prog = parent.Child(className: "msctls_progress32", flags: WCFlags.HiddenToo);
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

			prog.ShowLL(false);
			return parent;
		}
		int _editMultilineHeight;

		void _EditControlCreate()
		{
			Wnd parent = _EditControlGetPlace(out RECT r);

			//Create an intermediate "#32770" to be direct parent of the Edit control.
			//It is safer (the dialog will not receive Edit notifications) and helps to solve Tab/Esc problems.
			uint pStyle = Native.WS_CHILD | Native.WS_VISIBLE | Native.WS_CLIPCHILDREN | Native.WS_CLIPSIBLINGS; //don't need WS_TABSTOP
			uint pExStyle = Native.WS_EX_NOPARENTNOTIFY; //not Native.WS_EX_CONTROLPARENT
			_editParent = Wnd.Misc.CreateWindow(pExStyle, "#32770", null, pStyle, r.left, r.top, r.Width, r.Height, parent);
			_EditControlParentProcHolder = _EditControlParentProc;
			_editParent.SetWindowLong(Native.DWLP_DLGPROC, Marshal.GetFunctionPointerForDelegate(_EditControlParentProcHolder));

			//Create Edit or ComboBox control.
			string className = "Edit";
			uint style = Native.WS_CHILD | Native.WS_VISIBLE; //don't need WS_TABSTOP
			switch(_editType) {
			case TDEdit.Text: style |= Api.ES_AUTOHSCROLL; break;
			case TDEdit.Password: style |= Api.ES_PASSWORD | Api.ES_AUTOHSCROLL; break;
			case TDEdit.Number: style |= Api.ES_NUMBER | Api.ES_AUTOHSCROLL; break;
			case TDEdit.Multiline: style |= Api.ES_MULTILINE | Api.ES_AUTOVSCROLL | Api.ES_WANTRETURN | Native.WS_VSCROLL; break;
			case TDEdit.Combo: style |= Api.CBS_DROPDOWN | Api.CBS_AUTOHSCROLL | Native.WS_VSCROLL; className = "ComboBox"; break;
			}
			_editWnd = Wnd.Misc.CreateWindowAndSetFont(Native.WS_EX_CLIENTEDGE, className, null, style, 0, 0, r.Width, r.Height, _editParent, customFontHandle: _editFont);

			//Init the control.
			if(_editType == TDEdit.Combo) {
				if(_editText is IEnumerable<string> en) {
					bool addItem = false;
					foreach(var s in en) {
						if(!addItem) { addItem = true; _editWnd.SetText(s); } else _editWnd.SendS(Api.CB_INSERTSTRING, -1, s);
					}
				} else _editWnd.SetText(_editText?.ToString());

				RECT cbr = _editWnd.Rect;
				_editParent.LibResize(cbr.Width, cbr.Height); //because ComboBox resizes itself
			} else {
				_editWnd.SetText(_editText?.ToString());
				_editWnd.Send(Api.EM_SETSEL, 0, -1);
			}
			_editParent.ZorderTop();
			_editWnd.FocusLL();
		}

		void _EditControlOnMessage(TDApi.TDN message)
		{
			switch(message) {
			case TDApi.TDN.BUTTON_CLICKED:
				_editText = _editWnd.ControlText;
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
		public Wnd EditControl { get => _editWnd; }
		Wnd _editWnd, _editParent;
		Util.LibNativeFont _editFont;

		//Dlgproc of our intermediate #32770 control, the parent of out Edit control.
		int _EditControlParentProc(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam)
		{
			//PrintList(msg, wParam, lParam);
			switch(msg) {
			case Api.WM_SETFOCUS: //enables Tab when in single-line Edit control
				_dlg.ChildFast(null, "DirectUIHWND").FocusLL();
				return 1;
			case Api.WM_NEXTDLGCTL: //enables Tab when in multi-line Edit control
				_dlg.ChildFast(null, "DirectUIHWND").FocusLL();
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
		/// More info about custom buttons: <see cref="TaskDialog.Show"/>.
		/// </summary>
		CommandLinks = 1,

		/// <summary>
		/// Call <see cref="Thread.Abort()"/> if selected OK button when there are no other buttons. Also when selected Cancel, No, and on timeout.
		/// The same as <see cref="TaskDialog.FlagEndThread"/>.
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
		/// Make the dialog a topmost window (always on top of other windows), regardless of TaskDialog.Options.TopmostIfNoOwnerWindow etc.
		/// More info: <see cref="TaskDialog.FlagTopmost"/>. 
		/// </summary>
		Topmost = 32,

		/// <summary>
		/// Set <see cref="TaskDialog.Width"/> = 500.
		/// </summary>
		Wider = 64,

		/// <summary>
		/// Allow to cancel even if there is no Cancel button.
		/// It adds X (Close) button to the title bar, and also allows to close the dialog with the Esc key.
		/// When the dialog is closed with the X button or Esc, the returned result button id is 0 if there is no Cancel button, else the same as of Cancel button.
		/// The same as <see cref="TaskDialog.FlagXCancel"/>.
		/// </summary>
		XCancel = 128,

		//This was implemented, it's easy, but then I changed my mind, don't need too many features.
		///// <summary>
		///// Show keyboard shortcuts (underlined characters), like when you press the Alt key.
		///// More info: <see cref="TaskDialog.FlagKeyboardShortcutsVisible"/>.
		///// </summary>
		//KeyboardShortcutsVisible = 0x100,

		//NoTaskbarButton = , //not so useful
		//NeverActivate = , //don't know how to implement. LockSetForegroundWindow does not work if we can activate windows. HCBT_ACTIVATE can prevent activating but does not prevent deactivating.
		//AlwaysActivate = , //Don't use. Always allow. Because after AllowActivate (which is also used by Activate etc) always activates dialogs regardless of anything. As well as in uiAccess process.
	}

	/// <summary>
	/// TaskDialog result, returned by <see cref="TaskDialog.ShowEx"/> and similar functions.
	/// Contains multiple result values: selected button id, selected radio button id, check box state, edit text.
	/// </summary>
	public class TDResult
	{
		internal TDResult(int button, int radioButton, bool isChecked, string editText)
		{
			Button = button; RadioButton = radioButton; IsChecked = isChecked; EditText = editText;
		}

		/// <summary>
		/// Returned <see cref="Button"/> value on timeout.
		/// </summary>
		public const int Timeout = int.MinValue;

		/// <summary>
		/// Gets selected button id.
		/// On timeout it is TDResult.Timeout.
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
		/// Allows to use code <c>switch(TaskDialog.ShowEx(...))</c> instead of <c>switch(TaskDialog.ShowEx(...).Button)</c> .
		/// </summary>
		public static implicit operator int(TDResult r) { return r.Button; }

		/// <summary>
		/// Formats string $"Button={Button}, RadioButton={RadioButton}, IsChecked={IsChecked}, EditText={EditText}".
		/// </summary>
		public override string ToString()
		{
			return $"Button={Button}, RadioButton={RadioButton}, IsChecked={IsChecked}, EditText={EditText}";
		}
	}

	#endregion global definitions

	public partial class TaskDialog
	{
		#region Show

		/// <summary>
		/// Shows task dialog.
		/// Returns selected button id and other results packed in a TDResult variable (more info in Remarks).
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="buttons">See <see cref="Show"/>.</param>
		/// <param name="icon"></param>
		/// <param name="flags"></param>
		/// <param name="owner">Owner window or null. See <see cref="SetOwnerWindow"/>.</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses <see cref="Options.DefaultTitle"/>.</param>
		/// <param name="radioButtons">Adds radio buttons. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three".</param>
		/// <param name="checkBox">If not null/"", shows a check box with this text. To make it checked, append "|true", "|check" or "|checked".</param>
		/// <param name="defaultButton">id of button that responds to the Enter key.</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time (seconds) and set result's Button property = <see cref="TDResult.Timeout"/>.</param>
		/// <param name="onLinkClick">
		/// A link-clicked event handler function, eg lambda. Enables hyperlinks in small-font text.
		/// Example:
		/// <code><![CDATA[
		/// TaskDialog.ShowEx("", "Text <a href=\"example\">link</a>.", onLinkClick: e => { Print(e.LinkHref); });
		/// ]]></code>
		/// </param>
		/// <remarks>
		/// The returned TDResult variable has these properties: selected button id, selected radio button id, check box state.
		/// Tip: TDResult supports implicit cast to int. You can use code <c>switch(TaskDialog.ShowEx(...))</c> instead of <c>switch(TaskDialog.ShowEx(...).Button)</c> .
		/// Tip: For optional parameters use named arguments. Example: <c>TaskDialog.ShowEx("Text.", icon: TDIcon.Info, title: "Title")</c> .
		/// This function allows you to use most of the task dialog features, but not all. Alternatively you can create a TaskDialog class instance, set properties and call ShowDialog.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var r = TaskDialog.ShowEx("Main text", "More text.", "1 OK|2 Cancel", expandedText: "Expanded text", radioButtons: "1 One|2 Two|3 Three", checkBox: "Check", timeoutS: 30);
		/// Print(r);
		/// switch(r) {
		/// case 1: Print("OK"); break;
		/// case TDResult.Timeout: Print("timeout"); break;
		/// default: Print("Cancel"); break;
		/// }
		/// ]]></code>
		/// </example>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static TDResult ShowEx(
			string text1 = null, string text2 = null, string buttons = null, TDIcon icon = 0, TDFlags flags = 0, Types<Control, Wnd>? owner = null,
			string expandedText = null, string footerText = null, string title = null, string radioButtons = null, string checkBox = null,
			int defaultButton = 0, int x = 0, int y = 0, int timeoutS = 0, Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialog(text1, text2, buttons, icon, flags, owner,
				expandedText, footerText, title, radioButtons, checkBox,
				defaultButton, x, y, timeoutS, onLinkClick);
			return d.ShowDialog();
		}

		/// <summary>
		/// Shows task dialog.
		/// Returns selected button id.
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="buttons">
		/// A list of button ids and labels in this format: "id label|id label|...". Example: "1 OK|2 Cancel|5 Save|4 Don't Save".
		/// Missing ids are auto-generated, for example "OK|Cancel|100 Custom1|Custom2" is the same as "1 OK|2 Cancel|100 Custom1|101 Custom2".
		/// The first in the list button is <i>default</i>, ie responds to the Enter key. For example, "2 No|1 Yes" adds Yes and No buttons and makes No default.
		/// Trims newlines around ids and labels. For example, "\r\n1 One\r\n|\r\n2\r\nTwo\r\n\r\n" is the same as "1 One|2 Two".
		/// 
		/// To create keyboard shortcuts, use &amp; character in custom button labels. Use &amp;&amp; for literal &amp;. Example: "1 &amp;Tuesday[]2 T&amp;hursday[]3 Saturday &amp;&amp; Sunday".
		/// 
		/// There are 6 <i>common buttons</i>: OK, Yes, No, Retry, Cancel, Close. Buttons that have other labels are <i>custom buttons</i>.
		/// How common buttons are different:
		///		1. TDFlags.CommandLinks does not change their style.
		///		2. They have keyboard shortcuts that cannot be changed. Inserting &amp; in a label makes it a custom button.
		///		3. Button Cancel can be selected with the Esc key. It also adds X (Close) button in title bar, which selects Cancel.
		///		4. Always displayed in standard order (eg Yes No, never No Yes). But you can for example use "2 No|1 Yes" to set default button = No.
		///		5. The displayed button label is localized, ie different when the Windows UI language is not English.
		///	
		/// If omitted, null or "", the dialog will have OK button, id 1.
		/// You can use <see cref="TDFlags.CommandLinks"/> in flags to change the style of custom buttons.
		/// See also: <see cref="SetButtons"/>.
		/// </param>
		/// <param name="icon"><see cref="TDIcon"/>.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="owner">Owner window or null. See <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowEx"/>.
		/// Tip: For optional parameters use named arguments. Example: <c>TaskDialog.Show("Text.", icon: TDIcon.Info)</c> .
		/// </remarks>
		/// <seealso cref="ShowInfo"/>
		/// <seealso cref="ShowWarning"/>
		/// <seealso cref="ShowError"/>
		/// <seealso cref="ShowOKCancel"/>
		/// <seealso cref="ShowYesNo"/>
		/// <seealso cref="DebugDialog">DebugDialog</seealso>
		/// <example>
		/// <code><![CDATA[
		/// if(TaskDialog.Show("Show another example?", null, "1 OK|2 Cancel", TDIcon.Info) != 1) return;
		/// Print("OK");
		/// 
		/// switch(TaskDialog.Show("Save changes?", "More info.", "1 Save|2 Don't Save|Cancel")) {
		/// case 1: Print("save"); break;
		/// case 2: Print("don't"); break;
		/// default: Print("cancel"); break;
		/// }
		/// ]]></code>
		/// </example>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static int Show(string text1 = null, string text2 = null, string buttons = null, TDIcon icon = 0, TDFlags flags = 0, Types<Control, Wnd>? owner = null)
		{
			return ShowEx(text1, text2, buttons, icon, flags, owner).Button;
		}

		/// <summary>
		/// Shows task dialog with OK button and TDIcon.Info icon.
		/// Calls <see cref="Show"/>.
		/// </summary>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static void ShowInfo(string text1 = null, string text2 = null, TDFlags flags = 0, Types<Control, Wnd>? owner = null)
		{
			Show(text1, text2, null, TDIcon.Info, flags, owner);
		}

		/// <summary>
		/// Shows task dialog with OK button and TDIcon.Warning icon.
		/// Calls <see cref="Show"/>.
		/// </summary>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static void ShowWarning(string text1 = null, string text2 = null, TDFlags flags = 0, Types<Control, Wnd>? owner = null)
		{
			Show(text1, text2, null, TDIcon.Warning, flags, owner);
		}

		/// <summary>
		/// Shows task dialog with OK button and TDIcon.Error icon.
		/// Calls <see cref="Show"/>.
		/// </summary>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static void ShowError(string text1 = null, string text2 = null, TDFlags flags = 0, Types<Control, Wnd>? owner = null)
		{
			Show(text1, text2, null, TDIcon.Error, flags, owner);
		}

		/// <summary>
		/// Shows task dialog with OK and Cancel buttons.
		/// Returns true if selected OK.
		/// Calls <see cref="Show"/>.
		/// </summary>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static bool ShowOKCancel(string text1 = null, string text2 = null, TDIcon icon = 0, TDFlags flags = 0, Types<Control, Wnd>? owner = null)
		{
			return 1 == Show(text1, text2, "OK|Cancel", icon, flags, owner);
		}

		/// <summary>
		/// Shows task dialog with Yes and No buttons.
		/// Returns true if selected Yes.
		/// Calls <see cref="Show"/>.
		/// </summary>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static bool ShowYesNo(string text1 = null, string text2 = null, TDIcon icon = 0, TDFlags flags = 0, Types<Control, Wnd>? owner = null)
		{
			return 1 == Show(text1, text2, "Yes|No", icon, flags, owner);
		}

		#endregion Show

		#region ShowInput

		/// <summary>
		/// Shows dialog with a text edit field, buttons OK and Cancel, optionally check box, radio buttons and custom buttons.
		/// Returns results packed in a TDResult variable: selected button id (1 for OK, 2 for Cancel), text and check box state.
		/// </summary>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Read-only text below main instruction, above the edit field.</param>
		/// <param name="editType">Edit field type. It can be simple text (TDEdit.Text, default), multiline, number, password or combo box.</param>
		/// <param name="editText">Initial edit field text. If editType is Combo, it can be a string array, List or IEnumerable; the first item sets combo-box editable text, other items - combo box drop-down list items.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="owner">Owner window or null. See <see cref="SetOwnerWindow"/>.</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses <see cref="Options.DefaultTitle"/>.</param>
		/// <param name="checkBox">If not empty, shows a check box with this text. To make it checked, append "|true", "|check" or "|checked".</param>
		/// <param name="radioButtons">Adds radio buttons. A list of strings "id text" separated by |, like "1 One|2 Two|3 Three".</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A link-clicked event handler function, like with <see cref="ShowEx"/>.</param>
		/// <param name="buttons">You can use this to add more buttons. A list of strings "id text" separated by |, like "1 OK|2 Cancel|10 Browse...". See <see cref="Show"/>.</param>
		/// <param name="onButtonClick">
		/// A button-clicked event handler function, eg lambda.
		/// Examples:
		/// <code><![CDATA[
		/// TaskDialog.ShowInputEx("Example", flags: TDFlags.CommandLinks, buttons: "OK|Cancel|10 Browse\nSets edit control text.",
		///		onButtonClick: e => { if(e.Button == 10) { e.EditText = "text"; e.DoNotCloseDialog = true; } });
		/// 
		/// TaskDialog.ShowInputEx("Example", "Try to click OK while text is empty.", onButtonClick: e =>
		/// {
		/// 	if(e.Button == 1 && Empty(e.EditText)) {
		/// 		TaskDialog.Show("Text cannot be empty.", owner: e.hwnd);
		/// 		e.dialog.EditControl.FocusControlOfThisThread();
		/// 		e.DoNotCloseDialog = true;
		/// 	}
		/// });
		/// ]]></code>
		/// </param>
		/// <remarks>
		/// This function allows you to use most of the task dialog features, but not all. Alternatively you can create a TaskDialog class instance, set properties and call ShowDialog.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var r = TaskDialog.ShowInputEx("Example", "Comments.", checkBox: "Check");
		/// if(r.Button != 1) return;
		/// Print(r.EditText);
		/// Print(r.IsChecked);
		/// ]]></code>
		/// </example>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static TDResult ShowInputEx(
			string text1 = null, string text2 = null,
			TDEdit editType = TDEdit.Text, object editText = null,
			TDFlags flags = 0, Types<Control, Wnd>? owner = null,
			string expandedText = null, string footerText = null, string title = null, string checkBox = null, string radioButtons = null,
			int x = 0, int y = 0, int timeoutS = 0, Action<TDEventArgs> onLinkClick = null,
			string buttons = "1 OK|2 Cancel", Action<TDEventArgs> onButtonClick = null
			)
		{
			if(Empty(buttons)) buttons = "1 OK|2 Cancel";

			var d = new TaskDialog(text1, text2, buttons, 0, flags, owner,
				expandedText, footerText, title, radioButtons, checkBox,
				0, x, y, timeoutS, onLinkClick);

			d.SetEditControl((editType == TDEdit.None) ? TDEdit.Text : editType, editText);
			if(onButtonClick != null) d.ButtonClicked += onButtonClick;

			return d.ShowDialog();
		}

		/// <summary>
		/// Shows dialog with a text edit field and buttons OK and Cancel, and gets that text.
		/// Returns true if selected OK, false if Cancel.
		/// </summary>
		/// <param name="s">Variable that receives the text.</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Read-only text below main instruction, above the edit field.</param>
		/// <param name="editType">Edit field type.</param>
		/// <param name="editText">Initial edit field text. If editType is TDEdit.Combo, it can be a string array, List or IEnumerable; the first item sets combo-box editable text, other items - combo box drop-down list items.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="owner">Owner window or null. See <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowInputEx"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// string s;
		/// if(!TaskDialog.ShowInput(out s, "Example")) return;
		/// Print(s);
		/// 
		/// //or you can declare the variable like this
		/// if(!TaskDialog.ShowInput(out string s2, "Example")) return;
		/// Print(s2);
		/// ]]></code>
		/// </example>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static bool ShowInput(
			out string s,
			string text1 = null, string text2 = null,
			TDEdit editType = TDEdit.Text, object editText = null,
			TDFlags flags = 0, Types<Control, Wnd>? owner = null
			)
		{
			s = null;
			TDResult r = ShowInputEx(text1, text2, editType, editText, flags, owner);
			if(r.Button != 1) return false;
			s = r.EditText;
			return true;
		}

		/// <summary>
		/// Shows dialog with a number edit field and buttons OK and Cancel, and gets that number.
		/// Returns true if selected OK, false if Cancel.
		/// </summary>
		/// <param name="i">Variable that receives the number.</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Read-only text below main instruction, above the edit field.</param>
		/// <param name="editType">Edit field type.</param>
		/// <param name="editText">Initial edit field text.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="owner">Owner window or null. See <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowInputEx"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// int i;
		/// if(!TaskDialog.ShowInput(out i, "Example")) return;
		/// Print(i);
		/// ]]></code>
		/// </example>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static bool ShowInput(
			out int i,
			string text1 = null, string text2 = null, TDEdit editType = TDEdit.Number, object editText = null,
			TDFlags flags = 0, Types<Control, Wnd>? owner = null
			)
		{
			i = 0;
			if(!ShowInput(out string s, text1, text2, editType, editText, flags, owner)) return false;
			i = s.ToInt32_();
			return true;
		}

		#endregion ShowInput

		#region ShowList

		/// <summary>
		/// Shows task dialog with a list of command-link buttons.
		/// Returns results packed in a <see cref="TDResult"/> variable. Its Button property is id of the selected button, which is its 1-based index in the list; it is 0 if clicked the X (close window) button or pressed Esc.
		/// The return value can be assigned to an int variable or used in switch; then it is the id (1-based index or 0).
		/// </summary>
		/// <param name="list">List items (buttons). Can be string like "One|Two|Three" or IEnumerable&lt;string&gt;. See <see cref="SetButtons"/>.</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="owner">Owner window or null. See <see cref="SetOwnerWindow"/>.</param>
		/// <param name="expandedText">Text that the user can show and hide.</param>
		/// <param name="footerText">Text at the bottom of the dialog. Icon can be specified like "i|Text", where i is: x error, ! warning, i info, v shield, a app.</param>
		/// <param name="title">Title bar text. If omitted, null or "", uses <see cref="Options.DefaultTitle"/>.</param>
		/// <param name="checkBox">If not empty, shows a check box with this text. To make it checked, append "|true", "|check" or "|checked".</param>
		/// <param name="defaultButton">id (1-based index) of button that responds to the Enter key.</param>
		/// <param name="x">X position. 0 - screen center; negative - relative to the right edge.</param>
		/// <param name="y">Y position. 0 - screen center; negative - relative to the bottom edge.</param>
		/// <param name="timeoutS">If not 0, auto-close the dialog after this time, number of seconds.</param>
		/// <param name="onLinkClick">Enables hyperlinks in small-font text. A link-clicked event handler function, like with <see cref="ShowEx"/>.</param>
		/// <remarks>
		/// This function allows you to use most of the task dialog features, but not all. Alternatively you can create a TaskDialog class instance, set properties and call ShowDialog.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// int r = TaskDialog.ShowListEx("One|Two|Three", "Example", y: -1, timeoutS: 15);
		/// if(r <= 0) return; //X/Esc or timeout
		/// Print(r);
		/// ]]></code>
		/// </example>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static TDResult ShowListEx(
			Types<string, IEnumerable<string>> list, string text1 = null, string text2 = null, TDFlags flags = 0, Types<Control, Wnd>? owner = null,
			string expandedText = null, string footerText = null, string title = null, string checkBox = null,
			int defaultButton = 0, int x = 0, int y = 0, int timeoutS = 0,
			Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialog(text1, text2, null, 0, flags, owner,
				expandedText, footerText, title, null, checkBox,
				defaultButton, x, y, timeoutS, onLinkClick);

			d.SetButtons(null, true, list);
			d.FlagXCancel = true;
			d.SetExpandedText(expandedText, true);
			return d.ShowDialog();
		}

		/// <summary>
		/// Shows dialog with a list of command-link buttons.
		/// Returns 1-based index of the selected button. Returns 0 if clicked the X (close window) button or pressed Esc.
		/// </summary>
		/// <param name="list">List items (buttons). Can be string like "One|Two|Three" or IEnumerable&lt;string&gt;. See <see cref="SetButtons"/>.</param>
		/// <param name="text1">Main instruction. Bigger font.</param>
		/// <param name="text2">Text below main instruction.</param>
		/// <param name="flags"><see cref="TDFlags"/>.</param>
		/// <param name="owner">Owner window or null. See <see cref="SetOwnerWindow"/>.</param>
		/// <remarks>
		/// Calls <see cref="ShowListEx"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// int r = TaskDialog.ShowList("One|Two|Three", "Example");
		/// if(r == 0) return; //X/Esc
		/// Print(r);
		/// ]]></code>
		/// </example>
		/// <exception cref="Win32Exception">Failed to show dialog.</exception>
		public static int ShowList(Types<string, IEnumerable<string>> list, string text1 = null, string text2 = null, TDFlags flags = 0, Types<Control, Wnd>? owner = null)
		{
			return ShowListEx(list, text1, text2, flags, owner);
		}

		#endregion ShowList

		#region ShowProgress
#pragma warning disable 1573 //missing XML documentation for parameters

		/// <summary>
		/// Shows dialog with progress bar.
		/// Creates dialog in new thread and returns without waiting until it is closed.
		/// Returns TaskDialog variable that can be used to control the dialog: set progress bar position, update text, close etc.
		/// Most parameters are the same as with <see cref="ShowEx"/>.
		/// </summary>
		/// <param name="marquee">Let the progress bar animate without indicating a percent of work done.</param>
		/// <remarks>
		/// This function allows you to use most of the task dialog features, but not all. Alternatively you can create a TaskDialog class instance, set properties and call ShowNoWait.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var pd = TaskDialog.ShowProgressEx(false, "Working", buttons: "1 Stop", y: -1);
		/// for(int i = 1; i != 100; i++) {
		/// 	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		/// 	pd.Send.Progress(i); //don't need this if marquee
		/// 	WaitMS(50); //do something in the loop
		/// }
		/// pd.Send.Close();
		/// ]]></code>
		/// </example>
		/// <exception cref="CatException">Failed to show dialog.</exception>
		public static TaskDialog ShowProgressEx(bool marquee,
			string text1 = null, string text2 = null, string buttons = "0 Cancel", TDFlags flags = 0, Types<Control, Wnd>? owner = null,
			string expandedText = null, string footerText = null, string title = null, string radioButtons = null, string checkBox = null,
			int x = 0, int y = 0, int timeoutS = 0, Action<TDEventArgs> onLinkClick = null
		)
		{
			if(Empty(buttons)) buttons = "0 Cancel";

			var d = new TaskDialog(text1, text2, buttons, 0, flags, owner,
				expandedText, footerText, title, radioButtons, checkBox,
				0, x, y, timeoutS, onLinkClick);

			if(marquee) d.FlagShowMarqueeProgressBar = true; else d.FlagShowProgressBar = true;

			d.ShowNoWait();

			if(marquee) d.Send.Message(TDApi.TDM.SET_PROGRESS_BAR_MARQUEE, true);

			return d;
		}

		/// <summary>
		/// Shows dialog with progress bar.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialog variable that can be used to control the dialog: set progress bar position, close etc.
		/// All parameters except marquee are the same as with <see cref="ShowEx"/>.
		/// </summary>
		/// <param name="marquee">Let the progress bar animate without indicating a percent of work done.</param>
		/// <remarks>
		/// Calls <see cref="ShowProgressEx"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var pd = TaskDialog.ShowProgress(false, "Working");
		/// for(int i = 1; i != 100; i++) {
		/// 	if(!pd.IsOpen) { Print(pd.Result); break; } //if the user closed the dialog
		/// 	pd.Send.Progress(i); //don't need this if marquee
		/// 	WaitMS(50); //do something in the loop
		/// }
		/// pd.Send.Close();
		/// ]]></code>
		/// </example>
		/// <exception cref="CatException">Failed to show dialog.</exception>
		public static TaskDialog ShowProgress(bool marquee,
			string text1 = null, string text2 = null, string buttons = "0 Cancel", TDFlags flags = 0, Types<Control, Wnd>? owner = null,
			int x = 0, int y = 0)
		{
			return ShowProgressEx(marquee, text1, text2, buttons, flags, owner, x: x, y: y);
		}

#pragma warning restore 1573 //missing XML documentation for parameters
		#endregion ShowProgress

		#region ShowNoWait
#pragma warning disable 1573 //missing XML documentation for parameters

		/// <summary>
		/// Shows task dialog like <see cref="ShowEx"/> but does not wait.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialog variable that can be used to control the dialog, eg close.
		/// Most parameters are the same as with <see cref="ShowEx"/>.
		/// </summary>
		/// <param name="whenClosed">null or an event handler function (lambda etc) to call when the dialog closed.</param>
		/// <param name="closeOnExit">If true, the dialog disappears when all other threads of this app domain end.</param>
		/// <remarks>
		/// This function allows you to use most of the task dialog features, but not all. Alternatively you can create a TaskDialog class instance, set properties and call ShowNoWait.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// TaskDialog.ShowNoWait(null, false, "Simplest example");
		/// 
		/// var td = TaskDialog.ShowNoWaitEx(e => { Print(e); }, true, "Another example", buttons: "1 OK|2 Cancel", y: -1, timeoutS: 30);
		/// Wait(2); //do something while the dialog is open in other thread
		/// td.ThreadWaitClosed(); //wait until dialog closed (optional, just an example)
		/// ]]></code>
		/// </example>
		/// <exception cref="CatException">Failed to show dialog.</exception>
		public static TaskDialog ShowNoWaitEx(
			Action<TDResult> whenClosed, bool closeOnExit,
			string text1 = null, string text2 = null, string buttons = null, TDIcon icon = 0, TDFlags flags = 0, Types<Control, Wnd>? owner = null,
			string expandedText = null, string footerText = null, string title = null, string radioButtons = null, string checkBox = null,
			int defaultButton = 0, int x = 0, int y = 0, int timeoutS = 0, Action<TDEventArgs> onLinkClick = null
			)
		{
			var d = new TaskDialog(text1, text2, buttons, icon, flags, owner,
				expandedText, footerText, title, radioButtons, checkBox,
				defaultButton, x, y, timeoutS, onLinkClick);
			d.ShowNoWait(whenClosed, closeOnExit);
			return d;
		}

		/// <summary>
		/// Shows task dialog like <see cref="Show"/> but does not wait.
		/// Creates dialog in other thread and returns without waiting until it is closed.
		/// Returns TaskDialog variable that can be used to control the dialog, eg close.
		/// Most parameters are the same as with <see cref="Show"/>.
		/// </summary>
		/// <param name="whenClosed">null or an event handler function (lambda etc) to call when the dialog closed.</param>
		/// <param name="closeOnExit">If true, the dialog disappears when all other threads of this app domain end.</param>
		/// <remarks>
		/// Calls <see cref="ShowNoWaitEx"/>. Examples are there.
		/// </remarks>
		/// <exception cref="CatException">Failed to show dialog.</exception>
		public static TaskDialog ShowNoWait(
			Action<TDResult> whenClosed, bool closeOnExit,
			string text1 = null, string text2 = null,
			string buttons = null, TDIcon icon = 0, TDFlags flags = 0,
			Types<Control, Wnd>? owner = null
			)
		{
			return ShowNoWaitEx(whenClosed, closeOnExit, text1, text2, buttons, icon, flags, owner);
		}

#pragma warning restore 1573 //missing XML documentation for parameters
		#endregion ShowNoWait
	}
}
