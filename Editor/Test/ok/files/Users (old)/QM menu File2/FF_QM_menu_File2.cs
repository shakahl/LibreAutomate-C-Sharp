 /
function# iid FILTER&f

def TVM_HITTEST (TV_FIRST + 17)
type TVHITTESTINFO POINT'pt flags hItem
type TVITEM mask hItem state stateMask $pszText cchTextMax iImage iSelectedImage cChildren lParam

if(GetWinId(f.hwnd2)!=2202) ret

int+ g_qm_menu_iid
TVHITTESTINFO ht
GetCursorPos &ht.pt; ScreenToClient f.hwnd2 &ht.pt
SendMessage f.hwnd2 TVM_HITTEST 0 &ht
if(ht.hItem)
	TVITEM ti.hItem=ht.hItem
	SendMessage f.hwnd2 TVM_GETITEM 0 &ti
	g_qm_menu_iid=ti.lParam
else g_qm_menu_iid=0
 out g_qm_menu_iid

ret iid
