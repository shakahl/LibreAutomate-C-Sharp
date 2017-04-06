 /dlg_attached
function hDlg hwndOwner x y

 Attaches toolbar to owner window:
   If owner closed, closes toolbar.
   If owner hidden, hides toolbar.
   If owner unhidden, unhides toolbar.
   If owner moved, moves toolbar.

 hDlg - toolbar window.
 hwndOwner - owner window.
 x y - toolbar position relative to owner window.


if(!IsWindow(hwndOwner))
	clo hDlg
else if(!IsWindowVisible(hwndOwner))
	if(IsWindowVisible(hDlg)) hid hDlg
else if(!IsIconic(hwndOwner))
	RECT ro rd; GetWindowRect hwndOwner &ro; GetWindowRect hDlg &rd
	x+ro.left; y+ro.top
	if(x!rd.left or y!rd.top) mov x y hDlg
	if(!IsWindowVisible(hDlg))
		int wp(GetWindow(hwndOwner GW_HWNDPREV)) fl(SWP_SHOWWINDOW|SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_NOOWNERZORDER)
		if(wp=hDlg) wp=0; fl|SWP_NOZORDER
		SetWindowPos hDlg wp 0 0 0 0 fl
