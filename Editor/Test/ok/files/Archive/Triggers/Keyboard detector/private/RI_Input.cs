 /Keyboard_Detector
function hDlg wParam lParam

ARRAY(RIHID)- ak am

int dwSize
if(GetRawInputData(lParam, RID_INPUT, 0, &dwSize, sizeof(RAWINPUTHEADER)) = -1) ret
str s1.all(dwSize)
if(GetRawInputData(lParam, RID_INPUT, s1, &dwSize, sizeof(RAWINPUTHEADER)) != dwSize) ret
RAWINPUT* raw = +s1
if(!raw.header.hDevice) ret ;;injected

sel raw.header.dwType
	case RIM_TYPEKEYBOARD
	 out raw.data.keyboard.VKey
	if(raw.data.keyboard.Flags&RI_KEY_BREAK) ret ;;key up
	int i k mb retry
	 g1
	for(i 0 ak.len) if(raw.header.hDevice=ak[i].handle) k=ak[i].kid; break
	if(i=ak.len and !retry) retry=1; RI_GetDevices ak RIM_TYPEKEYBOARD; goto g1
	g_ri.keyboard_id=k
	 out k
	
	if(wParam&0xff=RIM_INPUT and win=hDlg and GetFocus=id(4 hDlg))
		i=LB_SelectedItem(id(4 hDlg) _s)
		if(i>=0 and i<4)
			g_rir.k[i]=k
			rset g_rir "data" "\KD"
			_s+" has been associated with this keyboard"
			_s.setwintext(id(8 hDlg))
			SetTimer hDlg 1 3000 0
	
	for(i 0 4) if(k=g_rir.k[i]) break
	if(i=4 or !k) ret
	g_ri.vk=raw.data.keyboard.VKey
	g_ri.k[i]=g_ri.vk
	g_ri.kt[i]=GetTickCount
	
	case RIM_TYPEMOUSE
	 out raw.data.mouse.usButtonFlags
	sel(raw.data.mouse.usButtonFlags) case [2 8 32 128 512] ret ;;up
	 g2
	for(i 0 am.len) if(raw.header.hDevice=am[i].handle) k=am[i].kid; break
	if(i=am.len and !retry) retry=1; RI_GetDevices am RIM_TYPEMOUSE; goto g2
	g_ri.mouse_id=k
	
	sel(raw.data.mouse.usButtonFlags)
		case 0 mb=0
		case 1 mb=6
		case 4 mb=7
		case 16 mb=8
		case 64 mb=4
		case 256 mb=5
		case 1024 mb=iif(raw.data.mouse.usButtonData<0x7fff 1 2)
	
	for(i 0 4) if(k=g_rir.m[i]) break
	if(i=4 or !k) ret
	g_ri.mb=mb
	g_ri.m[i]=mb
	g_ri.mt[i]=GetTickCount
	
	two_mouse_pointers
#err
	
	 case RIM_TYPEHID
	 out raw.data.hid.dwCount
