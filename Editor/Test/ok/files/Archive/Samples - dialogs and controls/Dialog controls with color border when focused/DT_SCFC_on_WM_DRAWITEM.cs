 /dialog_color_focus_rect
function hDlg wParam lParam

 Call this from dialog procedure on WM_DRAWITEM.
 Makes the focus rectangle blue.

int color=0xff0000 ;;blue

if(wParam!999) ret
__GdiHandle-- t_brush=CreateSolidBrush(color)
DRAWITEMSTRUCT& d=+lParam
RECT r; GetClientRect d.hWndItem &r
FillRect d.hDC &r t_brush
