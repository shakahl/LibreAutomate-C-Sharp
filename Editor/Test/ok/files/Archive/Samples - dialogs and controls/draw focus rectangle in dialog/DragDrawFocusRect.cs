 /
function! hDlg wParam lParam [idCtrl]

 Draws focus rectangle in your dialog.
 Call this function in your dialog procedure, on WM_SETCURSOR.
 If it returns not 0, execute this code: ret DT_Ret(hDlg 1)

 hDlg, wParam, lParam - hDlg, wParam, lParam.
 idCtrl - id of control in which to draw. If omitted or 0, draws in dialog.

 EXAMPLE
	 case WM_SETCURSOR
	 if(DragDrawFocusRect(hDlg wParam lParam)) ret DT_Ret(hDlg 1)


type DFR_DATA hwnd dc POINT'p RECT'r
DFR_DATA d

if(lParam&0xffff!=HTCLIENT or lParam>>16!=WM_LBUTTONDOWN) ret
d.hwnd=iif(idCtrl id(idCtrl hDlg) hDlg); if(wParam!=d.hwnd) ret
d.dc=GetDC(d.hwnd)
xm d.p d.hwnd 1
Drag hDlg &DDFR_Proc &d
ReleaseDC(d.hwnd d.dc)
ret 1
