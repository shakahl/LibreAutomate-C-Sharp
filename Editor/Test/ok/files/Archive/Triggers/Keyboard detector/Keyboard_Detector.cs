\Dialog_Editor
function# hDlg message wParam lParam

type RIDATA !k[4] !m[4] kt[4] mt[4] !kworking !mworking !vk !mb keyboard_id mouse_id hwnd
type RIDATAR flags k[4] m[4]
type RIHID handle kid
RIDATA+ g_ri
RIDATAR+ g_rir
ARRAY(RIHID)- ak am

#if (_winver>=0x501)
if(hDlg) goto messages

if(getopt(nthreads)>1)
	act g_ri.hwnd; err
	ret

SetThreadPriority GetCurrentThread 2

if(!rget(g_rir "data" "\KD")) g_rir.flags=1

str controls = "10 4 11 6"
str c10Use lb4k c11Use lb6m

if(g_rir.flags&1) c10Use=1
if(g_rir.flags&2) c11Use=1
lb4k="FF_Keyboard1[]FF_Keyboard2[]FF_Keyboard3[]FF_Keyboard4"
lb6m="FF_Mouse1[]FF_Mouse2[]FF_Mouse3[]FF_Mouse4"

g_ri.hwnd=ShowDialog("Keyboard_Detector" &Keyboard_Detector &controls 0 17 WS_VISIBLE|DS_SETFOREGROUND)
MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	if(m.message=WM_INPUT) RI_Input m.hwnd m.wParam m.lParam; continue
	TranslateMessage &m
	DispatchMessage &m

 BEGIN DIALOG
 0 "" 0x10C80848 0x100 0 0 261 193 "QM Keyboard & Mouse Detector"
 10 Button 0x54012003 0x4 4 2 96 12 "Use multiple keyboards"
 4 ListBox 0x54230101 0x204 4 16 84 40 "k"
 11 Button 0x54012003 0x4 4 62 94 12 "Use multiple mouses"
 6 ListBox 0x54230101 0x204 4 76 84 40 "m"
 2 Button 0x54030001 0x4 8 176 48 14 "Close"
 5 Button 0x54032000 0x4 58 176 48 14 "Exit"
 3 Static 0x54000000 0x4 100 16 158 44 "To associate a keyboard with a filter function, click the function in the list, and press spacebar or some other key on that keyboard. Repeat this for each keyboard."
 7 Static 0x54000000 0x4 100 76 158 40 "To associate a mouse with a filter function, double click the function in the list using that mouse. Repeat this for each mouse."
 8 Static 0x54000000 0x4 6 128 254 12 ""
 9 Button 0x54032000 0x0 210 176 48 14 "Help"
 12 Static 0x54000000 0x0 6 144 254 20 "Note: 'low level hook' must be unchecked in Options/Triggers. With LL hooks the detector does not work, and keyboard triggers also may stop working."
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	if(g_rir.flags&1) RI_RegisterDevices(hDlg 1 0); g_ri.kworking=1; else TO_Enable hDlg "4" 0
	if(g_rir.flags&2) RI_RegisterDevices(hDlg 1 1); g_ri.mworking=1; else TO_Enable hDlg "6" 0
	
	case WM_CLOSE if(wParam) DestroyWindow(hDlg)
	
	case WM_DESTROY
	RI_RegisterDevices hDlg 0 0 1
	RI_RegisterDevices hDlg 0 1 1
	PostQuitMessage 0
	memset &g_ri 0 sizeof(g_ri)
	
	case WM_COMMAND goto messages2
	
	case WM_TIMER
	KillTimer hDlg wParam
	_s.setwintext(id(8 hDlg))
	
ret
 messages2
sel wParam
	case IDCANCEL hid hDlg; ret
	case 5 DestroyWindow(hDlg)
	case 9 mac+ "Keyboard Detector Help"
	
	case [10,11]
	sel wParam
		case 10
		g_rir.flags^1
		ak.redim
		RI_RegisterDevices(hDlg g_rir.flags&1 0)
		TO_Enable hDlg "4" g_rir.flags&1
		g_ri.kworking=g_rir.flags&1
		case 11
		g_rir.flags^2
		am.redim
		RI_RegisterDevices(hDlg g_rir.flags&2 1)
		TO_Enable hDlg "6" g_rir.flags&2
		g_ri.mworking=g_rir.flags&2
	rset g_rir "data" "\KD"
	
	case LBN_DBLCLK<<16|6
	int i=LB_SelectedItem(lParam _s)
	if(i>=0 and i<4)
		g_rir.m[i]=g_ri.mouse_id
		rset g_rir "data" "\KD"
		_s+" has been associated with this mouse"
		_s.setwintext(id(8 hDlg))
		SetTimer hDlg 1 3000 0

ret 1

#else
out "Keyboard Detector requires Windows XP or later."

 TODO: an option "Any USB keyboard".
 TODO: test lock/sleep/hibernate: http://www.quickmacros.com/forum/viewtopic.php?f=4&t=6447
