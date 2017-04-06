 /DRT_Main
function hDlg

int hrb=id(3 hDlg)
SendMessage hrb WM_SIZE 0 0 ;;rebar sets its width/height itself

RECT r; GetClientRect hDlg &r
RECT rt; GetWindowRect hrb &rt
r.top=rt.bottom-rt.top

MoveWindow id(50 hDlg) 0 r.top r.right r.bottom-r.top 1
RedrawWindow id(50 hDlg) 0 0 RDW_INVALIDATE|RDW_ERASE ;;because Vista incorrectly redraws the control after moving a rebar band; this probably not needed for other controls
