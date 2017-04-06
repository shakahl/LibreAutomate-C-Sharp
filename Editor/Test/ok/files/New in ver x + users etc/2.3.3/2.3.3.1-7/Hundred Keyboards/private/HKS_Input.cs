 /Hundred_Keyboards
function hDlg wParam lParam

ARRAY(HKS_HID)- ak am

int dwSize
if(GetRawInputData(lParam, RID_INPUT, 0, &dwSize, sizeof(RAWINPUTHEADER)) = -1) ret
str s1.all(dwSize)
if(GetRawInputData(lParam, RID_INPUT, s1, &dwSize, sizeof(RAWINPUTHEADER)) != dwSize) ret
RAWINPUT* raw = +s1
if(!raw.header.hDevice) ret ;;injected
if(raw.header.dwType!RIM_TYPEKEYBOARD) ret
if(raw.data.keyboard.Flags&RI_KEY_BREAK) ret ;;key up

 out raw.data.keyboard.VKey
int i k retry
 g1
for(i 0 ak.len) if(raw.header.hDevice=ak[i].handle) k=ak[i].kid; break
if(i=ak.len and !retry) retry=1; HKS_GetDevices ak RIM_TYPEKEYBOARD; goto g1
g_hks_ri.keyboard_id=k
 out k

if(wParam&0xff=RIM_INPUT and win=hDlg and GetFocus=id(4 hDlg))
	i=LB_SelectedItem(id(4 hDlg) _s)
	if(i>=0 and i<100)
		g_hks_rir.k[i]=k
		rset g_hks_rir "data" "\HKS"
		_s+" has been associated with this keyboard"
		_s.setwintext(id(8 hDlg))
		SetTimer hDlg 1 3000 0

for(i 0 100) if(k=g_hks_rir.k[i]) break
if(i=100 or !k) ret
g_hks_ri.vk=raw.data.keyboard.VKey
g_hks_ri.k[i]=g_hks_ri.vk
g_hks_ri.kt[i]=GetTickCount

HKS_Key i raw.data.keyboard.VKey
