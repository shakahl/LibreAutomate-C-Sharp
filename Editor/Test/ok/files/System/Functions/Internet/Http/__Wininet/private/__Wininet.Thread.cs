function# func !*args $dlgTitle $dlgUrl

 Runs sub.Thread in separate thread.

 func - what function to call. See sub.Thread.
 args - address of first arg.
 dlgTitle, dlgUrl - strings to display.


__WininetT t
t.func=func
t.h=&this
t.a=args
if(m_dlg) sub.Dlg(t m_dlg iif(m_dlgTitle.len m_dlgTitle dlgTitle) dlgUrl); m_hdlg=t.hdlg
t.hthread=mac("sub.Thread" "" &t)
opt waitmsg 1
wait 0 H t.hthread
t.hthread=0
m_hdlg=0
ret t.r


#sub Thread
function __WininetT&t

int* a=t.a
sel t.func
	 Http
	case 0 t.r=t.h.GetUrl(+a[0] +a[1] a[2]|0x10000 a[3] 0 0 0 +a[7])
	case 1 t.r=t.h.Get(+a[0] +a[1] a[2]|0x10000 a[3] +a[4])
	case 2 t.r=t.h.Post(+a[0] +a[1] +a[2] +a[3] a[4] +a[5] a[6]|0x10000)
	case 3
		ARRAY(POSTFIELD)* ap=+a[1]
		t.r=t.h.PostFormData(+a[0] ap +a[2] +a[3] a[4] a[5] a[6] +a[7] a[8]|0x10000)
	
	 Ftp
	case 10 t.r=t.f.FileGetStr(+a[0] +a[1] a[2] a[3]|0x10000)
	case 11 t.r=t.f.FilePutStr(+a[0] +a[1] a[2] a[3] a[4]|0x10000)


#sub Dlg
function __WininetT&t dlg $title $url

str dd=
F
 BEGIN DIALOG
 0 "" 0x80C80848 0x100 0 0 257 57 "{title}"
 2 Button 0x54030001 0x4 4 40 48 14 "Cancel"
 6 Static 0x54020000 0x4 4 4 252 13 "{url}"
 4 Static 0x54020000 0x4 4 22 86 12 "Connecting"
 5 Static 0x54000000 0x0 94 22 80 12 ""
 3 Static 0x54000000 0x0 178 22 78 12 ""
 7 msctls_progress32 0x54000000 0x4 68 42 186 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030207 "" "" ""

#opt nowarnings 1
t.hdlg=ShowDialog(dd &sub.DlgProc 0 iif(IsWindow(dlg) dlg 0) 1)
SetWindowPos t.hdlg 0 0 0 0 0 SWP_SHOWWINDOW|SWP_NOACTIVATE|SWP_NOMOVE|SWP_NOSIZE|SWP_NOZORDER|SWP_NOOWNERZORDER


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_APP
	SendDlgItemMessage hDlg 7 PBM_SETPOS iif(wParam>0 MulDiv(lParam 100 wParam) 0) 0
	if(wParam>=0) _s=wParam/1024; else _s="???"
	SetDlgItemText(hDlg 4 F"{lParam/1024} KB of {_s} KB")
	
	int-- t_time t_nbytes
	int t(GetTickCount) td(t-t_time)
	if !lParam or td>=2000
		if(lParam) SetDlgItemText(hDlg 5 F"Speed {MulDiv(lParam-t_nbytes 1000 td)/1024} KB/s")
		t_time=t; t_nbytes=lParam

	case WM_COMMAND ret 1
