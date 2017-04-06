 /dlg_hidden_modal
function# [$macro] [dlgproc] [!*controls] [hwndowner] [flags] [style] [notstyle] [param] [x] [y] [$icon] [$menu] ;;flags: 1 modeless, 4 set style (default is to add), 64 raw x y, 128 hidden.

 For QM <2.3.3. Unfinished.

 Creates hidden dialog.
 Same as <tip>ShowDialog</tip>, but the dialog initially is invisible.

 To show, use act. To show without activating, use SetWindowPos with flags SWP_SHOWWINDOW|SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOMOVE|SWP_NOSIZE.


int hDlg=ShowDialog(macro dlgproc controls hwndowner flags|1 style notstyle|WS_VISIBLE|DS_SETFOREGROUND param x y icon menu)
if(flags&1) ret hDlg


MSG m; int r
rep
	if(GetMessage(&m 0 0 0)<1) PostQuitMessage m.wParam; break
	 OutWinMsg m.message m.wParam m.lParam
	if(m.message=WM_COMMAND and m.wParam&0xffff0000=0) ;;does not work. Works only when click X button, else sends. Need to subclass.
		if(m.wParam!IDCANCEL) DT_GetControls hDlg
		DispatchMessage(&m)
		if(IsWindow(hDlg)) continue
		r=m.wParam; break
	if(IsDialogMessage(hDlg &m)) continue
	TranslateMessage &m
	DispatchMessage &m


0
ret r
err+ end _error
