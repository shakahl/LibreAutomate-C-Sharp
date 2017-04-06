 /draw_on_splitter
function hWnd hdc RECT&ru cbParam

RECT r; GetClientRect hWnd &r
int w(r.right) h(r.bottom) ww(w/2) hh(h/2)

__Font-- fontSym.Create("Marlett" w) ;;symbol font
__Hicon-- iconCut=GetFileIcon("$qm$\cut.ico")

int oldFont=SelectObject(hdc fontSym)
SetBkMode hdc TRANSPARENT

sel cbParam
	case 0 ;;vertical splitter
	TextOutW hdc 0 hh-w L"3" 1 ;;<
	TextOutW hdc -w/6 hh L"4" 1 ;;>
	DrawIconEx hdc -3 h-18 iconCut 16 16 0 0 DI_NORMAL
	
	case 1 ;;horizontal splitter
	TextOutW hdc ww-h 0 L"5" 1 ;;^
	TextOutW hdc ww -h/6 L"6" 1 ;;v
	DrawIconEx hdc w-18 -3 iconCut 16 16 0 0 DI_NORMAL

SelectObject(hdc oldFont)
