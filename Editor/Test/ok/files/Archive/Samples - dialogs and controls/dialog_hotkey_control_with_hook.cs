\Dialog_Editor

str controls = "3"
str e3
if(!ShowDialog("" &sub.DlgProc &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 64 8 152 12 ""
 4 Static 0x54000200 0x0 8 8 48 12 "Hotkey"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040105 "*" "" "" ""

#sub DlgProc
function# hDlg message wParam lParam

int- t_hhk t_hk
sel message
	case WM_INITDIALOG
	t_hhk=id(3 hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case EN_SETFOCUS<<16|3
	int-- t_hh
	t_hh=SetWindowsHookEx(WH_KEYBOARD &sub.Hook_WH_KEYBOARD _hinst GetCurrentThreadId)
	case EN_KILLFOCUS<<16|3
	UnhookWindowsHookEx t_hh
	case IDOK
	out "hotkey: vk=%i, mod=%i" t_hk&255 t_hk>>8
	 note: mod used with RegisterHotKey is different: need to swap the first and third bits.
	case IDCANCEL
ret 1


#sub Hook_WH_KEYBOARD
function# nCode vk lparam
if(nCode<0) goto gNext

int up(lparam&0x80000000) m mod

sel(vk) case VK_SHIFT m=1; case VK_CONTROL m=2; case VK_MENU m=4; case [VK_LWIN,VK_RWIN] m=8
int-- t_mod
if(m) vk=0; if(up) t_mod~m; else t_mod|m
else mod=t_mod

if(!up) sub.SetHotkey vk mod

ret 1

 gNext
ret CallNextHookEx(0 nCode vk +lparam)


#sub SetHotkey
function vk mod

int- t_hhk t_hk
FormatKeyString vk mod &_s
_s.setwintext(t_hhk)
t_hk=mod<<8|vk
