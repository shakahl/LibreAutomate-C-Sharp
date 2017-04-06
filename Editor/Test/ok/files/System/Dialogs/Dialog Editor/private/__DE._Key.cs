 \Dialog_Editor
function! vk
 Called when _hwnd is focused. It is focused when _hform or its children clicked/rclicked.

int h(_hsel) mod=GetMod
sel vk
	case VK_TAB
	if(mod>1) ret
	rep ;;until a visible control
		if h=_hform
			h=GetWindow(h GW_CHILD)
			if(mod and h) h=GetWindow(h GW_HWNDLAST)
		else
			h=GetWindow(h iif(mod GW_HWNDPREV GW_HWNDNEXT))
			if(!h) h=_hform
		if(!h or h=_hform or IsWindowVisible(h)) break
	if(h) _Select(h)
	
	case [VK_LEFT,VK_RIGHT,VK_UP,VK_DOWN]
	if(mod>1) ret
	if(!mod and h=_hform) ret
	if(_arrowMovSiz) _save=1; else _Undo; _arrowMovSiz=1
	RECT p b ;;pixels, dialog units
	if(h=_hform) GetClientRect h &p; else GetWindowRect h &p; MapWindowPoints 0 _hform +&p 2
	b.left=MulDiv(p.left 4 _dbx); b.top=MulDiv(p.top 8 _dby); b.right=MulDiv(p.right-p.left 4 _dbx); b.bottom=MulDiv(p.bottom-p.top 8 _dby)
	if mod ;;resize
		sel vk
			case VK_LEFT if(b.right>1) b.right-1
			case VK_RIGHT b.right+1
			case VK_UP if(b.bottom>1) b.bottom-1
			case VK_DOWN b.bottom+1
		if(h=_hform) int ncx ncy; GetWinXY h 0 0 ncx ncy; ncx-p.right-p.left; ncy-p.bottom-p.top
		p.right=MulDiv(b.right _dbx 4); p.bottom=MulDiv(b.bottom _dby 8)
		int swp=SWP_NOMOVE|SWP_NOZORDER; if(h!_hform) swp|SWP_NOCOPYBITS
		SetWindowPos(h 0 0 0 p.right+ncx p.bottom+ncy swp)
		if(h=_hform) subs.AutoSizeEditor
	else ;;move
		sel vk
			case VK_LEFT b.left-1
			case VK_RIGHT b.left+1
			case VK_UP b.top-1
			case VK_DOWN b.top+1
		p.left=MulDiv(b.left _dbx 4); p.top=MulDiv(b.top _dby 8)
		SetWindowPos(h 0 p.left p.top 0 0 SWP_NOSIZE|SWP_NOZORDER|SWP_NOCOPYBITS)
		subs.SetMark
	
	case VK_DELETE if(!mod) _WmCommand(1008)
	case 'E' if(mod=4) _WmCommand(1013)
	case else if(mod=2) goto gCtrl
ret
 gCtrl
sel vk
	case 'Z' _WmCommand(1014)
	case 'Y' _WmCommand(1015)
	case 'S' _WmCommand(1001)
	case 'Z' _WmCommand(1014)
	case 'X' _WmCommand(1040 5)
	case 'C' _WmCommand(1040 1)
	case 'V' _WmCommand(1040 2)
	case 'D' _WmCommand(1040 3)
