\Dialog_Editor
function# hDlg message wParam lParam

type HKS_DATA !k[100] kt[100] !kworking !vk keyboard_id hwnd
type HKS_DATAR flags k[100]
type HKS_HID handle kid
HKS_DATA+ g_hks_ri
HKS_DATAR+ g_hks_rir
ARRAY(HKS_HID)- ak am

#if (_winver>=0x501)
if(hDlg) goto messages

if(getopt(nthreads)>1)
	act g_hks_ri.hwnd; err
	ret

SetThreadPriority GetCurrentThread 2

if(!rget(g_hks_rir "data" "\HKS")) g_hks_rir.flags=1

g_hks_ri.hwnd=ShowDialog("Hundred_Keyboards" &Hundred_Keyboards 0 0 17 WS_VISIBLE|DS_SETFOREGROUND)
MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	if(m.message=WM_INPUT) HKS_Input m.hwnd m.wParam m.lParam; continue
	TranslateMessage &m
	DispatchMessage &m

 BEGIN DIALOG
 0 "" 0x10C80848 0x100 0 0 261 193 "Hundred Keyboards"
 4 ListBox 0x54230101 0x204 4 16 84 40 "k"
 2 Button 0x54030001 0x4 8 176 48 14 "Close"
 5 Button 0x54032000 0x4 58 176 48 14 "Exit"
 3 Static 0x54000000 0x4 100 16 158 44 "To associate a keyboard with a number, click the number in the list, and press spacebar or some other key on that keyboard. Repeat this for each keyboard."
 8 Static 0x54000000 0x4 6 128 254 12 ""
 9 Button 0x54032000 0x0 210 176 48 14 "Help"
 12 Static 0x54000000 0x0 6 144 254 20 "Note: 'low level hook' must be unchecked in Options/Triggers. With LL hooks does not work, and keyboard triggers also may stop working in QM."
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	HKS_RegisterDevices(hDlg 1 0); g_hks_ri.kworking=1
	
	case WM_CLOSE if(wParam) DestroyWindow(hDlg)
	
	case WM_DESTROY
	HKS_RegisterDevices hDlg 0 0 1
	PostQuitMessage 0
	memset &g_hks_ri 0 sizeof(g_hks_ri)
	
	case WM_COMMAND goto messages2
	
	case WM_TIMER
	KillTimer hDlg wParam
	_s.setwintext(id(8 hDlg))
	
ret
 messages2
sel wParam
	case IDCANCEL hid hDlg; ret
	case 5 DestroyWindow(hDlg)
	case 9 mac+ "Hundred Keyboards Help"

ret 1

#else
out "Requires Windows XP or later."
