//Windows API constants common to multiple API functions, such as WM_, WS_, errors.

using System;

namespace Catkeys.Winapi
{
	public static unsafe partial class Api
	{
		#region WM_

		public const uint WM_NULL = 0;
		public const uint WM_CREATE = 0x0001;
		public const uint WM_DESTROY = 0x0002;
		public const uint WM_MOVE = 0x0003;
		public const uint WM_SIZE = 0x0005;
		public const uint WM_ACTIVATE = 0x0006;
		public const uint WM_SETFOCUS = 0x0007;
		public const uint WM_KILLFOCUS = 0x0008;
		public const uint WM_ENABLE = 0x000A;
		public const uint WM_SETREDRAW = 0x000B;
		public const uint WM_SETTEXT = 0x000C;
		public const uint WM_GETTEXT = 0x000D;
		public const uint WM_GETTEXTLENGTH = 0x000E;
		public const uint WM_PAINT = 0x000F;
		public const uint WM_CLOSE = 0x0010;
		public const uint WM_QUERYENDSESSION = 0x0011;
		public const uint WM_QUERYOPEN = 0x0013;
		public const uint WM_ENDSESSION = 0x0016;
		public const uint WM_QUIT = 0x0012;
		public const uint WM_ERASEBKGND = 0x0014;
		public const uint WM_SYSCOLORCHANGE = 0x0015;
		public const uint WM_SHOWWINDOW = 0x0018;
		public const uint WM_SETTINGCHANGE = 0x001A;
		public const uint WM_DEVMODECHANGE = 0x001B;
		public const uint WM_ACTIVATEAPP = 0x001C;
		public const uint WM_FONTCHANGE = 0x001D;
		public const uint WM_TIMECHANGE = 0x001E;
		public const uint WM_CANCELMODE = 0x001F;
		public const uint WM_SETCURSOR = 0x0020;
		public const uint WM_MOUSEACTIVATE = 0x0021;
		public const uint WM_CHILDACTIVATE = 0x0022;
		public const uint WM_QUEUESYNC = 0x0023;
		public const uint WM_GETMINMAXINFO = 0x0024;
		public const uint WM_PAINTICON = 0x0026;
		public const uint WM_ICONERASEBKGND = 0x0027;
		public const uint WM_NEXTDLGCTL = 0x0028;
		public const uint WM_SPOOLERSTATUS = 0x002A;
		public const uint WM_DRAWITEM = 0x002B;
		public const uint WM_MEASUREITEM = 0x002C;
		public const uint WM_DELETEITEM = 0x002D;
		public const uint WM_VKEYTOITEM = 0x002E;
		public const uint WM_CHARTOITEM = 0x002F;
		public const uint WM_SETFONT = 0x0030;
		public const uint WM_GETFONT = 0x0031;
		public const uint WM_SETHOTKEY = 0x0032;
		public const uint WM_GETHOTKEY = 0x0033;
		public const uint WM_QUERYDRAGICON = 0x0037;
		public const uint WM_COMPAREITEM = 0x0039;
		public const uint WM_GETOBJECT = 0x003D;
		public const uint WM_COMPACTING = 0x0041;
		public const uint WM_WINDOWPOSCHANGING = 0x0046;
		public const uint WM_WINDOWPOSCHANGED = 0x0047;
		public const uint WM_COPYDATA = 0x004A;
		public const uint WM_CANCELJOURNAL = 0x004B;
		public const uint WM_NOTIFY = 0x004E;
		public const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
		public const uint WM_INPUTLANGCHANGE = 0x0051;
		public const uint WM_TCARD = 0x0052;
		public const uint WM_HELP = 0x0053;
		public const uint WM_USERCHANGED = 0x0054;
		public const uint WM_NOTIFYFORMAT = 0x0055;
		public const uint WM_CONTEXTMENU = 0x007B;
		public const uint WM_STYLECHANGING = 0x007C;
		public const uint WM_STYLECHANGED = 0x007D;
		public const uint WM_DISPLAYCHANGE = 0x007E;
		public const uint WM_GETICON = 0x007F;
		public const uint WM_SETICON = 0x0080;
		public const uint WM_NCCREATE = 0x0081;
		public const uint WM_NCDESTROY = 0x0082;
		public const uint WM_NCCALCSIZE = 0x0083;
		public const uint WM_NCHITTEST = 0x0084;
		public const uint WM_NCPAINT = 0x0085;
		public const uint WM_NCACTIVATE = 0x0086;
		public const uint WM_GETDLGCODE = 0x0087;
		public const uint WM_SYNCPAINT = 0x0088;
		public const uint WM_NCMOUSEMOVE = 0x00A0;
		public const uint WM_NCLBUTTONDOWN = 0x00A1;
		public const uint WM_NCLBUTTONUP = 0x00A2;
		public const uint WM_NCLBUTTONDBLCLK = 0x00A3;
		public const uint WM_NCRBUTTONDOWN = 0x00A4;
		public const uint WM_NCRBUTTONUP = 0x00A5;
		public const uint WM_NCRBUTTONDBLCLK = 0x00A6;
		public const uint WM_NCMBUTTONDOWN = 0x00A7;
		public const uint WM_NCMBUTTONUP = 0x00A8;
		public const uint WM_NCMBUTTONDBLCLK = 0x00A9;
		public const uint WM_NCXBUTTONDOWN = 0x00AB;
		public const uint WM_NCXBUTTONUP = 0x00AC;
		public const uint WM_NCXBUTTONDBLCLK = 0x00AD;
		public const uint WM_INPUT_DEVICE_CHANGE = 0x00FE;
		public const uint WM_INPUT = 0x00FF;
		public const uint WM_KEYDOWN = 0x0100;
		public const uint WM_KEYUP = 0x0101;
		public const uint WM_CHAR = 0x0102;
		public const uint WM_DEADCHAR = 0x0103;
		public const uint WM_SYSKEYDOWN = 0x0104;
		public const uint WM_SYSKEYUP = 0x0105;
		public const uint WM_SYSCHAR = 0x0106;
		public const uint WM_SYSDEADCHAR = 0x0107;
		public const uint WM_UNICHAR = 0x0109;
		public const uint WM_IME_STARTCOMPOSITION = 0x010D;
		public const uint WM_IME_ENDCOMPOSITION = 0x010E;
		public const uint WM_IME_COMPOSITION = 0x010F;
		public const uint WM_INITDIALOG = 0x0110;
		public const uint WM_COMMAND = 0x0111;
		public const uint WM_SYSCOMMAND = 0x0112;
		public const uint WM_TIMER = 0x0113;
		public const uint WM_HSCROLL = 0x0114;
		public const uint WM_VSCROLL = 0x0115;
		public const uint WM_INITMENU = 0x0116;
		public const uint WM_INITMENUPOPUP = 0x0117;
		public const uint WM_MENUSELECT = 0x011F;
		public const uint WM_MENUCHAR = 0x0120;
		public const uint WM_ENTERIDLE = 0x0121;
		public const uint WM_MENURBUTTONUP = 0x0122;
		public const uint WM_MENUDRAG = 0x0123;
		public const uint WM_MENUGETOBJECT = 0x0124;
		public const uint WM_UNINITMENUPOPUP = 0x0125;
		public const uint WM_MENUCOMMAND = 0x0126;
		public const uint WM_CHANGEUISTATE = 0x0127;
		public const uint WM_UPDATEUISTATE = 0x0128;
		public const uint WM_QUERYUISTATE = 0x0129;
		public const uint WM_CTLCOLORMSGBOX = 0x0132;
		public const uint WM_CTLCOLOREDIT = 0x0133;
		public const uint WM_CTLCOLORLISTBOX = 0x0134;
		public const uint WM_CTLCOLORBTN = 0x0135;
		public const uint WM_CTLCOLORDLG = 0x0136;
		public const uint WM_CTLCOLORSCROLLBAR = 0x0137;
		public const uint WM_CTLCOLORSTATIC = 0x0138;
		public const uint WM_MOUSEFIRST = 0x0200;
		public const uint WM_MOUSEMOVE = 0x0200;
		public const uint WM_LBUTTONDOWN = 0x0201;
		public const uint WM_LBUTTONUP = 0x0202;
		public const uint WM_LBUTTONDBLCLK = 0x0203;
		public const uint WM_RBUTTONDOWN = 0x0204;
		public const uint WM_RBUTTONUP = 0x0205;
		public const uint WM_RBUTTONDBLCLK = 0x0206;
		public const uint WM_MBUTTONDOWN = 0x0207;
		public const uint WM_MBUTTONUP = 0x0208;
		public const uint WM_MBUTTONDBLCLK = 0x0209;
		public const uint WM_MOUSEWHEEL = 0x020A;
		public const uint WM_XBUTTONDOWN = 0x020B;
		public const uint WM_XBUTTONUP = 0x020C;
		public const uint WM_XBUTTONDBLCLK = 0x020D;
		public const uint WM_MOUSEHWHEEL = 0x020E;
		public const uint WM_MOUSELAST = 0x020E;
		public const uint WM_PARENTNOTIFY = 0x0210;
		public const uint WM_ENTERMENULOOP = 0x0211;
		public const uint WM_EXITMENULOOP = 0x0212;
		public const uint WM_NEXTMENU = 0x0213;
		public const uint WM_SIZING = 0x0214;
		public const uint WM_CAPTURECHANGED = 0x0215;
		public const uint WM_MOVING = 0x0216;
		public const uint WM_POWERBROADCAST = 0x0218;
		public const uint WM_DEVICECHANGE = 0x0219;
		public const uint WM_MDICREATE = 0x0220;
		public const uint WM_MDIDESTROY = 0x0221;
		public const uint WM_MDIACTIVATE = 0x0222;
		public const uint WM_MDIRESTORE = 0x0223;
		public const uint WM_MDINEXT = 0x0224;
		public const uint WM_MDIMAXIMIZE = 0x0225;
		public const uint WM_MDITILE = 0x0226;
		public const uint WM_MDICASCADE = 0x0227;
		public const uint WM_MDIICONARRANGE = 0x0228;
		public const uint WM_MDIGETACTIVE = 0x0229;
		public const uint WM_MDISETMENU = 0x0230;
		public const uint WM_ENTERSIZEMOVE = 0x0231;
		public const uint WM_EXITSIZEMOVE = 0x0232;
		public const uint WM_DROPFILES = 0x0233;
		public const uint WM_MDIREFRESHMENU = 0x0234;
		public const uint WM_IME_SETCONTEXT = 0x0281;
		public const uint WM_IME_NOTIFY = 0x0282;
		public const uint WM_IME_CONTROL = 0x0283;
		public const uint WM_IME_COMPOSITIONFULL = 0x0284;
		public const uint WM_IME_SELECT = 0x0285;
		public const uint WM_IME_CHAR = 0x0286;
		public const uint WM_IME_REQUEST = 0x0288;
		public const uint WM_IME_KEYDOWN = 0x0290;
		public const uint WM_IME_KEYUP = 0x0291;
		public const uint WM_MOUSEHOVER = 0x02A1;
		public const uint WM_MOUSELEAVE = 0x02A3;
		public const uint WM_NCMOUSEHOVER = 0x02A0;
		public const uint WM_NCMOUSELEAVE = 0x02A2;
		public const uint WM_WTSSESSION_CHANGE = 0x02B1;
		public const uint WM_CUT = 0x0300;
		public const uint WM_COPY = 0x0301;
		public const uint WM_PASTE = 0x0302;
		public const uint WM_CLEAR = 0x0303;
		public const uint WM_UNDO = 0x0304;
		public const uint WM_RENDERFORMAT = 0x0305;
		public const uint WM_RENDERALLFORMATS = 0x0306;
		public const uint WM_DESTROYCLIPBOARD = 0x0307;
		public const uint WM_DRAWCLIPBOARD = 0x0308;
		public const uint WM_PAINTCLIPBOARD = 0x0309;
		public const uint WM_VSCROLLCLIPBOARD = 0x030A;
		public const uint WM_SIZECLIPBOARD = 0x030B;
		public const uint WM_ASKCBFORMATNAME = 0x030C;
		public const uint WM_CHANGECBCHAIN = 0x030D;
		public const uint WM_HSCROLLCLIPBOARD = 0x030E;
		public const uint WM_QUERYNEWPALETTE = 0x030F;
		public const uint WM_PALETTEISCHANGING = 0x0310;
		public const uint WM_PALETTECHANGED = 0x0311;
		public const uint WM_HOTKEY = 0x0312;
		public const uint WM_PRINT = 0x0317;
		public const uint WM_PRINTCLIENT = 0x0318;
		public const uint WM_APPCOMMAND = 0x0319;
		public const uint WM_THEMECHANGED = 0x031A;
		public const uint WM_CLIPBOARDUPDATE = 0x031D;
		public const uint WM_DWMCOMPOSITIONCHANGED = 0x031E;
		public const uint WM_DWMNCRENDERINGCHANGED = 0x031F;
		public const uint WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
		public const uint WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321;
		public const uint WM_GETTITLEBARINFOEX = 0x033F;
		public const uint WM_APP = 0x8000;
		public const uint WM_USER = 0x0400;

		public const uint WM_CPL_LAUNCH = WM_USER+0x1000;
		public const uint WM_CPL_LAUNCHED = WM_USER+0x1001;
		public const uint WM_SYSTIMER = 0x118;
		public const uint WM_HSHELL_ACCESSIBILITYSTATE = 11;
		public const uint WM_HSHELL_ACTIVATESHELLWINDOW = 3;
		public const uint WM_HSHELL_APPCOMMAND = 12;
		public const uint WM_HSHELL_GETMINRECT = 5;
		public const uint WM_HSHELL_LANGUAGE = 8;
		public const uint WM_HSHELL_REDRAW = 6;
		public const uint WM_HSHELL_TASKMAN = 7;
		public const uint WM_HSHELL_WINDOWCREATED = 1;
		public const uint WM_HSHELL_WINDOWDESTROYED = 2;
		public const uint WM_HSHELL_WINDOWACTIVATED = 4;
		public const uint WM_HSHELL_WINDOWREPLACED = 13;

		#endregion

		#region WS_

		public const uint WS_POPUP = 0x80000000;
		public const uint WS_CHILD = 0x40000000;
		public const uint WS_MINIMIZE = 0x20000000;
		public const uint WS_VISIBLE = 0x10000000;
		public const uint WS_DISABLED = 0x08000000;
		public const uint WS_CLIPSIBLINGS = 0x04000000;
		public const uint WS_CLIPCHILDREN = 0x02000000;
		public const uint WS_MAXIMIZE = 0x01000000;
		public const uint WS_BORDER = 0x00800000;
		public const uint WS_DLGFRAME = 0x00400000;
		public const uint WS_VSCROLL = 0x00200000;
		public const uint WS_HSCROLL = 0x00100000;
		public const uint WS_SYSMENU = 0x00080000;
		public const uint WS_THICKFRAME = 0x00040000;
		public const uint WS_GROUP = 0x00020000;
		public const uint WS_TABSTOP = 0x00010000;
		public const uint WS_MINIMIZEBOX = 0x00020000;
		public const uint WS_MAXIMIZEBOX = 0x00010000;

		public const uint WS_CAPTION = WS_BORDER | WS_DLGFRAME;
		public const uint WS_OVERLAPPEDWINDOW = WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
		public const uint WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU;

		//WS_EX_

		public const uint WS_EX_DLGMODALFRAME = 0x00000001;
		public const uint WS_EX_NOPARENTNOTIFY = 0x00000004;
		public const uint WS_EX_TOPMOST = 0x00000008;
		public const uint WS_EX_ACCEPTFILES = 0x00000010;
		public const uint WS_EX_TRANSPARENT = 0x00000020;
		public const uint WS_EX_MDICHILD = 0x00000040;
		public const uint WS_EX_TOOLWINDOW = 0x00000080;
		public const uint WS_EX_WINDOWEDGE = 0x00000100;
		public const uint WS_EX_CLIENTEDGE = 0x00000200;
		public const uint WS_EX_CONTEXTHELP = 0x00000400;
		public const uint WS_EX_RIGHT = 0x00001000;
		public const uint WS_EX_LEFT = 0x00000000;
		public const uint WS_EX_RTLREADING = 0x00002000;
		public const uint WS_EX_LTRREADING = 0x00000000;
		public const uint WS_EX_LEFTSCROLLBAR = 0x00004000;
		public const uint WS_EX_RIGHTSCROLLBAR = 0x00000000;
		public const uint WS_EX_CONTROLPARENT = 0x00010000;
		public const uint WS_EX_STATICEDGE = 0x00020000;
		public const uint WS_EX_APPWINDOW = 0x00040000;
		public const uint WS_EX_LAYERED = 0x00080000;
		public const uint WS_EX_NOINHERITLAYOUT = 0x00100000;
		public const uint WS_EX_LAYOUTRTL = 0x00400000;
		public const uint WS_EX_COMPOSITED = 0x02000000;
		public const uint WS_EX_NOACTIVATE = 0x08000000;
		public const uint WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

		#endregion

		#region control styles, messages etc

		//ES_, EM_, EN_
		public const uint ES_WANTRETURN = 4096;
		//public const uint ES_UPPERCASE = 8;
		//public const uint ES_RIGHT = 2;
		public const uint ES_READONLY = 2048;
		public const uint ES_PASSWORD = 32;
		public const uint ES_NUMBER = 8192;
		public const uint ES_NOHIDESEL = 256;
		public const uint ES_MULTILINE = 4;
		//public const uint ES_LOWERCASE = 16;
		//public const uint ES_CENTER = 1;
		public const uint ES_AUTOVSCROLL = 64;
		public const uint ES_AUTOHSCROLL = 128;

		public const int EM_SETSEL = 177;

		//public const int EN_CHANGE = 768;

		//CBS_, CB_, CBN_
		public const int CBS_SIMPLE = 1;
		public const int CBS_DROPDOWN = 2;
		public const int CBS_DROPDOWNLIST = 3;
		public const int CBS_AUTOHSCROLL = 64;

		public const int CB_INSERTSTRING = 330;
		public const int CB_RESETCONTENT = 331;

		#endregion

		#region CS_

		public const uint CS_VREDRAW = 0x1;
		public const uint CS_HREDRAW = 0x2;
		public const uint CS_DBLCLKS = 0x8;
		public const uint CS_OWNDC = 0x20;
		public const uint CS_CLASSDC = 0x40;
		public const uint CS_PARENTDC = 0x80;
		public const uint CS_NOCLOSE = 0x200;
		public const uint CS_SAVEBITS = 0x800;
		public const uint CS_BYTEALIGNCLIENT = 0x1000;
		public const uint CS_BYTEALIGNWINDOW = 0x2000;
		public const uint CS_GLOBALCLASS = 0x4000;
		public const uint CS_IME = 0x10000;
		public const uint CS_DROPSHADOW = 0x20000;

		#endregion

		#region VK_

		public const int VK_ZOOM = 251;
		public const int VK_XBUTTON2 = 6;
		public const int VK_XBUTTON1 = 5;
		public const int VK_VOLUME_UP = 175;
		public const int VK_VOLUME_MUTE = 173;
		public const int VK_VOLUME_DOWN = 174;
		public const int VK_UP = 38;
		public const int VK_TAB = 9;
		public const int VK_SUBTRACT = 109;
		public const int VK_SPACE = 32;
		public const int VK_SNAPSHOT = 44;
		public const int VK_SLEEP = 95;
		public const int VK_SHIFT = 16;
		public const int VK_SEPARATOR = 108;
		public const int VK_SELECT = 41;
		public const int VK_SCROLL = 145;
		public const int VK_RWIN = 92;
		public const int VK_RSHIFT = 161;
		public const int VK_RMENU = 165;
		public const int VK_RIGHT = 39;
		public const int VK_RETURN = 13;
		public const int VK_RCONTROL = 163;
		public const int VK_RBUTTON = 2;
		public const int VK_PROCESSKEY = 229;
		public const int VK_PRIOR = 33;
		public const int VK_PRINT = 42;
		public const int VK_PLAY = 250;
		public const int VK_PAUSE = 19;
		public const int VK_PACKET = 231;
		public const int VK_PA1 = 253;
		public const int VK_OEM_WSCTRL = 238;
		public const int VK_OEM_RESET = 233;
		public const int VK_OEM_PLUS = 187;
		public const int VK_OEM_PERIOD = 190;
		public const int VK_OEM_PA3 = 237;
		public const int VK_OEM_PA2 = 236;
		public const int VK_OEM_PA1 = 235;
		public const int VK_OEM_NEC_EQUAL = 146;
		public const int VK_OEM_MINUS = 189;
		public const int VK_OEM_JUMP = 234;
		public const int VK_OEM_FJ_TOUROKU = 148;
		public const int VK_OEM_FJ_ROYA = 150;
		public const int VK_OEM_FJ_MASSHOU = 147;
		public const int VK_OEM_FJ_LOYA = 149;
		public const int VK_OEM_FJ_JISHO = 146;
		public const int VK_OEM_FINISH = 241;
		public const int VK_OEM_ENLW = 244;
		public const int VK_OEM_CUSEL = 239;
		public const int VK_OEM_COPY = 242;
		public const int VK_OEM_COMMA = 188;
		public const int VK_OEM_CLEAR = 254;
		public const int VK_OEM_BACKTAB = 245;
		public const int VK_OEM_AX = 225;
		public const int VK_OEM_AUTO = 243;
		public const int VK_OEM_ATTN = 240;
		public const int VK_OEM_8 = 223;
		public const int VK_OEM_7 = 222;
		public const int VK_OEM_6 = 221;
		public const int VK_OEM_5 = 220;
		public const int VK_OEM_4 = 219;
		public const int VK_OEM_3 = 192;
		public const int VK_OEM_2 = 191;
		public const int VK_OEM_102 = 226;
		public const int VK_OEM_1 = 186;
		public const int VK_NUMPAD9 = 105;
		public const int VK_NUMPAD8 = 104;
		public const int VK_NUMPAD7 = 103;
		public const int VK_NUMPAD6 = 102;
		public const int VK_NUMPAD5 = 101;
		public const int VK_NUMPAD4 = 100;
		public const int VK_NUMPAD3 = 99;
		public const int VK_NUMPAD2 = 98;
		public const int VK_NUMPAD1 = 97;
		public const int VK_NUMPAD0 = 96;
		public const int VK_NUMLOCK = 144;
		public const int VK_NONCONVERT = 29;
		public const int VK_NONAME = 252;
		public const int VK_NEXT = 34;
		public const int VK_MULTIPLY = 106;
		public const int VK_MODECHANGE = 31;
		public const int VK_MENU = 18;
		public const int VK_MEDIA_STOP = 178;
		public const int VK_MEDIA_PREV_TRACK = 177;
		public const int VK_MEDIA_PLAY_PAUSE = 179;
		public const int VK_MEDIA_NEXT_TRACK = 176;
		public const int VK_MBUTTON = 4;
		public const int VK_LWIN = 91;
		public const int VK_LSHIFT = 160;
		public const int VK_LMENU = 164;
		public const int VK_LEFT = 37;
		public const int VK_LCONTROL = 162;
		public const int VK_LBUTTON = 1;
		public const int VK_LAUNCH_MEDIA_SELECT = 181;
		public const int VK_LAUNCH_MAIL = 180;
		public const int VK_LAUNCH_APP2 = 183;
		public const int VK_LAUNCH_APP1 = 182;
		public const int VK_KANJI = 25;
		public const int VK_KANA = 21;
		public const int VK_JUNJA = 23;
		public const int VK_INSERT = 45;
		public const int VK_ICO_HELP = 227;
		public const int VK_ICO_CLEAR = 230;
		public const int VK_ICO_00 = 228;
		public const int VK_HOME = 36;
		public const int VK_HELP = 47;
		public const int VK_HANJA = 25;
		public const int VK_HANGUL = 21;
		public const int VK_HANGEUL = 21;
		public const int VK_FINAL = 24;
		public const int VK_F9 = 120;
		public const int VK_F8 = 119;
		public const int VK_F7 = 118;
		public const int VK_F6 = 117;
		public const int VK_F5 = 116;
		public const int VK_F4 = 115;
		public const int VK_F3 = 114;
		public const int VK_F24 = 135;
		public const int VK_F23 = 134;
		public const int VK_F22 = 133;
		public const int VK_F21 = 132;
		public const int VK_F20 = 131;
		public const int VK_F2 = 113;
		public const int VK_F19 = 130;
		public const int VK_F18 = 129;
		public const int VK_F17 = 128;
		public const int VK_F16 = 127;
		public const int VK_F15 = 126;
		public const int VK_F14 = 125;
		public const int VK_F13 = 124;
		public const int VK_F12 = 123;
		public const int VK_F11 = 122;
		public const int VK_F10 = 121;
		public const int VK_F1 = 112;
		public const int VK_EXSEL = 248;
		public const int VK_EXECUTE = 43;
		public const int VK_ESCAPE = 27;
		public const int VK_EREOF = 249;
		public const int VK_END = 35;
		public const int VK_DOWN = 40;
		public const int VK_DIVIDE = 111;
		public const int VK_DELETE = 46;
		public const int VK_DECIMAL = 110;
		public const int VK_CRSEL = 247;
		public const int VK_CONVERT = 28;
		public const int VK_CONTROL = 17;
		public const int VK_CLEAR = 12;
		public const int VK_CAPITAL = 20;
		public const int VK_CANCEL = 3;
		public const int VK_BROWSER_STOP = 169;
		public const int VK_BROWSER_SEARCH = 170;
		public const int VK_BROWSER_REFRESH = 168;
		public const int VK_BROWSER_HOME = 172;
		public const int VK_BROWSER_FORWARD = 167;
		public const int VK_BROWSER_FAVORITES = 171;
		public const int VK_BROWSER_BACK = 166;
		public const int VK_BACK = 8;
		public const int VK_ATTN = 246;
		public const int VK_APPS = 93;
		public const int VK_ADD = 107;
		public const int VK_ACCEPT = 30;

		#endregion

		#region Errors

		public const int S_OK = 0;
		public const int S_FALSE = 1;
		public const int ERROR_FILE_NOT_FOUND = 2;
		public const int ERROR_ACCESS_DENIED = 5;
		public const int ERROR_INVALID_HANDLE = 6;
		public const int ERROR_FILE_EXISTS = 80;
		public const int ERROR_INVALID_PARAMETER = 87;
		public const int ERROR_INSUFFICIENT_BUFFER = 122;
		public const int ERROR_INVALID_WINDOW_HANDLE = 1400;
		public const int E_INVALIDARG = unchecked((int)0x80070057);
		public const int E_FAIL = unchecked((int)0x80004005);
		public const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);

		#endregion

		#region HT (hit-test)

		public const int HTVSCROLL = 7;
		public const int HTTRANSPARENT = -1;
		public const int HTTOPRIGHT = 14;
		public const int HTTOPLEFT = 13;
		public const int HTTOP = 12;
		public const int HTSYSMENU = 3;
		public const int HTSIZELAST = HTBOTTOMRIGHT;
		public const int HTSIZEFIRST = HTLEFT;
		public const int HTSIZE = 4;
		public const int HTRIGHT = 11;
		public const int HTOBJECT = 19;
		public const int HTNOWHERE = 0;
		public const int HTMINBUTTON = 8;
		public const int HTMENU = 5;
		public const int HTMAXBUTTON = 9;
		public const int HTLEFT = 10;
		public const int HTHSCROLL = 6;
		public const int HTHELP = 21;
		public const int HTERROR = -2;
		public const int HTCLOSE = 20;
		public const int HTCLIENT = 1;
		public const int HTCAPTION = 2;
		public const int HTBOTTOMRIGHT = 17;
		public const int HTBOTTOMLEFT = 16;
		public const int HTBOTTOM = 15;
		public const int HTBORDER = 18;

		#endregion

		#region SC_
		public const uint SC_SIZE = 0xF000;
		public const uint SC_MOVE = 0xF010;
		public const uint SC_MINIMIZE = 0xF020;
		public const uint SC_MAXIMIZE = 0xF030;
		public const uint SC_NEXTWINDOW = 0xF040;
		public const uint SC_PREVWINDOW = 0xF050;
		public const uint SC_CLOSE = 0xF060;
		public const uint SC_VSCROLL = 0xF070;
		public const uint SC_HSCROLL = 0xF080;
		public const uint SC_MOUSEMENU = 0xF090;
		public const uint SC_KEYMENU = 0xF100;
		public const uint SC_ARRANGE = 0xF110;
		public const uint SC_RESTORE = 0xF120;
		public const uint SC_TASKLIST = 0xF130;
		public const uint SC_SCREENSAVE = 0xF140;
		public const uint SC_HOTKEY = 0xF150;
		public const uint SC_DEFAULT = 0xF160;
		public const uint SC_MONITORPOWER = 0xF170;
		public const uint SC_CONTEXTHELP = 0xF180;
		public const uint SC_SEPARATOR = 0xF00F;
		#endregion

		#region SMTO_
		public const uint SMTO_BLOCK = 0x0001;
		public const uint SMTO_ABORTIFHUNG = 0x0002;
		public const uint SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;
		public const uint SMTO_ERRORONEXIT = 0x0020;
		#endregion

		#region COLOR_

		public const int COLOR_SCROLLBAR = 0;
		public const int COLOR_BACKGROUND = 1;
		public const int COLOR_ACTIVECAPTION = 2;
		public const int COLOR_INACTIVECAPTION = 3;
		public const int COLOR_MENU = 4;
		public const int COLOR_WINDOW = 5;
		public const int COLOR_WINDOWFRAME = 6;
		public const int COLOR_MENUTEXT = 7;
		public const int COLOR_WINDOWTEXT = 8;
		public const int COLOR_CAPTIONTEXT = 9;
		public const int COLOR_ACTIVEBORDER = 10;
		public const int COLOR_INACTIVEBORDER = 11;
		public const int COLOR_APPWORKSPACE = 12;
		public const int COLOR_HIGHLIGHT = 13;
		public const int COLOR_HIGHLIGHTTEXT = 14;
		public const int COLOR_BTNFACE = 15;
		public const int COLOR_BTNSHADOW = 16;
		public const int COLOR_GRAYTEXT = 17;
		public const int COLOR_BTNTEXT = 18;
		public const int COLOR_INACTIVECAPTIONTEXT = 19;
		public const int COLOR_BTNHIGHLIGHT = 20;
		public const int COLOR_3DDKSHADOW = 21;
		public const int COLOR_3DLIGHT = 22;
		public const int COLOR_INFOTEXT = 23;
		public const int COLOR_INFOBK = 24;
		public const int COLOR_HOTLIGHT = 26;
		public const int COLOR_GRADIENTACTIVECAPTION = 27;
		public const int COLOR_GRADIENTINACTIVECAPTION = 28;
		public const int COLOR_MENUHILIGHT = 29;
		public const int COLOR_MENUBAR = 30;
		public const int COLOR_DESKTOP = 1;
		public const int COLOR_3DFACE = 15;
		public const int COLOR_3DSHADOW = 16;
		public const int COLOR_3DHIGHLIGHT = 20;
		public const int COLOR_3DHILIGHT = 20;
		public const int COLOR_BTNHILIGHT = 20;

		#endregion

		#region IDC_

		public const int IDC_ARROW = 32512;
		public const int IDC_IBEAM = 32513;
		public const int IDC_WAIT = 32514;
		public const int IDC_CROSS = 32515;
		public const int IDC_UPARROW = 32516;
		public const int IDC_SIZE = 32640;
		public const int IDC_ICON = 32641;
		public const int IDC_SIZENWSE = 32642;
		public const int IDC_SIZENESW = 32643;
		public const int IDC_SIZEWE = 32644;
		public const int IDC_SIZENS = 32645;
		public const int IDC_SIZEALL = 32646;
		public const int IDC_NO = 32648;
		public const int IDC_HAND = 32649;
		public const int IDC_APPSTARTING = 32650;
		public const int IDC_HELP = 32651;

		#endregion

		#region IDI_

		public const int IDI_APPLICATION = 32512;

		#endregion

		#region QS_

		public const uint QS_KEY = 0x1;
		public const uint QS_MOUSEMOVE = 0x2;
		public const uint QS_MOUSEBUTTON = 0x4;
		public const uint QS_POSTMESSAGE = 0x8;
		public const uint QS_TIMER = 0x10;
		public const uint QS_PAINT = 0x20;
		public const uint QS_SENDMESSAGE = 0x40;
		public const uint QS_HOTKEY = 0x80;
		public const uint QS_ALLPOSTMESSAGE = 0x100;
		public const uint QS_RAWINPUT = 0x400;
		public const uint QS_TOUCH = 0x800;
		public const uint QS_POINTER = 0x1000;
		public const uint QS_MOUSE = 0x6;
		public const uint QS_INPUT = 0x1C07;
		public const uint QS_ALLEVENTS = 0x1CBF;
		public const uint QS_ALLINPUT = 0x1CFF;

		#endregion

		#region WAIT_

		public const int WAIT_TIMEOUT = 258;
		public const uint WAIT_FAILED = 0xFFFFFFFF;
		public const uint WAIT_OBJECT_0 = 0x0;
		public const uint WAIT_ABANDONED = 0x80;
		public const uint WAIT_ABANDONED_0 = 0x80;
		public const uint WAIT_IO_COMPLETION = 0xC0;

		#endregion

		#region LR_, IMAGE_

		public const int IMAGE_BITMAP = 0;
		public const int IMAGE_ICON = 1;
		public const int IMAGE_CURSOR = 2;
		public const uint LR_MONOCHROME = 0x1;
		public const uint LR_COLOR = 0x2;
		public const uint LR_COPYRETURNORG = 0x4;
		public const uint LR_COPYDELETEORG = 0x8;
		public const uint LR_LOADFROMFILE = 0x10;
		public const uint LR_LOADTRANSPARENT = 0x20;
		public const uint LR_DEFAULTSIZE = 0x40;
		public const uint LR_VGACOLOR = 0x80;
		public const uint LR_LOADMAP3DCOLORS = 0x1000;
		public const uint LR_CREATEDIBSECTION = 0x2000;
		public const uint LR_COPYFROMRESOURCE = 0x4000;
		public const uint LR_SHARED = 0x8000;

		#endregion

		#region SFGAO_

		public const uint SFGAO_CANCOPY = 1;
		public const uint SFGAO_CANMOVE = 2;
		public const uint SFGAO_CANLINK = 4;
		public const uint SFGAO_STORAGE = 0x00000008;
		public const uint SFGAO_CANRENAME = 0x00000010;
		public const uint SFGAO_CANDELETE = 0x00000020;
		public const uint SFGAO_HASPROPSHEET = 0x00000040;
		public const uint SFGAO_DROPTARGET = 0x00000100;
		public const uint SFGAO_CAPABILITYMASK = 0x00000177;
		public const uint SFGAO_SYSTEM = 0x00001000;
		public const uint SFGAO_ENCRYPTED = 0x00002000;
		public const uint SFGAO_ISSLOW = 0x00004000;
		public const uint SFGAO_GHOSTED = 0x00008000;
		public const uint SFGAO_LINK = 0x00010000;
		public const uint SFGAO_SHARE = 0x00020000;
		public const uint SFGAO_READONLY = 0x00040000;
		public const uint SFGAO_HIDDEN = 0x00080000;
		public const uint SFGAO_DISPLAYATTRMASK = 0x000FC000;
		public const uint SFGAO_FILESYSANCESTOR = 0x10000000;
		public const uint SFGAO_FOLDER = 0x20000000;
		public const uint SFGAO_FILESYSTEM = 0x40000000;
		public const uint SFGAO_HASSUBFOLDER = 0x80000000;
		public const uint SFGAO_CONTENTSMASK = 0x80000000;
		public const uint SFGAO_VALIDATE = 0x01000000;
		public const uint SFGAO_REMOVABLE = 0x02000000;
		public const uint SFGAO_COMPRESSED = 0x04000000;
		public const uint SFGAO_BROWSABLE = 0x08000000;
		public const uint SFGAO_NONENUMERATED = 0x00100000;
		public const uint SFGAO_NEWCONTENT = 0x00200000;
		public const uint SFGAO_CANMONIKER = 0x00400000;
		public const uint SFGAO_HASSTORAGE = 0x00400000;
		public const uint SFGAO_STREAM = 0x00400000;
		public const uint SFGAO_STORAGEANCESTOR = 0x00800000;
		public const uint SFGAO_STORAGECAPMASK = 0x70C50008;
		public const uint SFGAO_PKEYSFGAOMASK = 0x81044000;

		#endregion

		#region STGM_

		public const uint STGM_DIRECT = 0x0;
		public const uint STGM_TRANSACTED = 0x10000;
		public const uint STGM_SIMPLE = 0x8000000;
		public const uint STGM_READ = 0x0;
		public const uint STGM_WRITE = 0x1;
		public const uint STGM_READWRITE = 0x2;
		public const uint STGM_SHARE_DENY_NONE = 0x40;
		public const uint STGM_SHARE_DENY_READ = 0x30;
		public const uint STGM_SHARE_DENY_WRITE = 0x20;
		public const uint STGM_SHARE_EXCLUSIVE = 0x10;
		public const uint STGM_PRIORITY = 0x40000;
		public const uint STGM_DELETEONRELEASE = 0x4000000;
		public const uint STGM_NOSCRATCH = 0x100000;
		public const uint STGM_CREATE = 0x1000;
		public const uint STGM_CONVERT = 0x20000;
		public const uint STGM_FAILIFTHERE = 0x0;
		public const uint STGM_NOSNAPSHOT = 0x200000;
		public const uint STGM_DIRECT_SWMR = 0x400000;

		#endregion





		public const uint INFINITE = 0xFFFFFFFF;





		#region ENUM

		[Flags]
		public enum VARENUM :ushort
		{
			VT_EMPTY,
			VT_NULL,
			VT_I2,
			VT_I4,
			VT_R4,
			VT_R8,
			VT_CY,
			VT_DATE,
			VT_BSTR,
			VT_DISPATCH,
			VT_ERROR,
			VT_BOOL,
			VT_VARIANT,
			VT_UNKNOWN,
			VT_DECIMAL,
			VT_I1 = 16,
			VT_UI1,
			VT_UI2,
			VT_UI4,
			VT_I8,
			VT_UI8,
			VT_INT,
			VT_UINT,
			VT_VOID,
			VT_HRESULT,
			VT_PTR,
			VT_SAFEARRAY,
			VT_CARRAY,
			VT_USERDEFINED,
			VT_LPSTR,
			VT_LPWSTR,
			VT_RECORD = 36,
			VT_INT_PTR,
			VT_UINT_PTR,
			VT_FILETIME = 64,
			VT_BLOB,
			VT_STREAM,
			VT_STORAGE,
			VT_STREAMED_OBJECT,
			VT_STORED_OBJECT,
			VT_BLOB_OBJECT,
			VT_CF,
			VT_CLSID,
			VT_VERSIONED_STREAM,
			VT_BSTR_BLOB = 0xFFF,
			VT_VECTOR,
			VT_ARRAY = 0x2000,
			VT_BYREF = 0x4000,
			VT_RESERVED = 0x8000,
			VT_ILLEGAL = 0xFFFF,
			VT_ILLEGALMASKED = 0xFFF,
			VT_TYPEMASK = 0xFFF
		}

		#endregion
	}
}
