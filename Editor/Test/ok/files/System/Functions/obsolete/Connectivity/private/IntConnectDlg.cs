 \Dialog_Editor
function# hDlg message wParam lParam

#compile rasapi
int-- hconn

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 179 20 "QM - connecting"
 2 Button 0x54030001 0x4 128 4 48 14 "Cancel"
 3 Static 0x54020000 0x4 8 6 110 10 "Connecting"
 4 Edit 0x44030080 0x204 142 32 8 8 "Use"
 5 Edit 0x44030080 0x204 152 32 8 8 "Pas"
 6 Edit 0x44030080 0x204 162 32 8 8 "Num"
 7 Edit 0x44030080 0x204 8 34 8 8 "Con"
 END DIALOG
 DIALOG EDITOR: "___RASDIALQM" 0x2010400 "*" ""

 messages
sel message
	case WM_INITDIALOG
	int+ ___WM_RASDIALEVENT; if(!___WM_RASDIALEVENT) ___WM_RASDIALEVENT=RegisterWindowMessage("RasDialEvent")
	__DIALOG* dl=+lParam; ___RASDIALQM& d=dl.controls
	if(d.e7Con.len) _s.from("QM - connecting to " d.e7Con) _s.setwintext(hDlg)
	int r=RasConnect(d.e7Con d.e6Num d.e4Use d.e5Pas hDlg hconn)
	if(r) EndDialog hDlg r
	
	case WM_COMMAND goto messages2
	case else
	if(message=___WM_RASDIALEVENT)
		str s.getmacro("rasapi") ss p
		if(wParam<0x100) p.format("(?<=def RASCS_)\w+(?= %i)" wParam)
		else p.format("(?<=def RASCS_)\w+(?= 0x%X)" wParam)
		if(findrx(s p 0 0 ss)) ss.setwintext(id(3 hDlg))
		if(lParam)
			if(hconn) RasHangUp hconn; hconn=0
			EndDialog hDlg lParam
		if(wParam=RASCS_Connected or wParam=RASCS_Connected) 0.1; EndDialog hDlg 0
			
ret
 messages2
sel wParam
	case [IDOK,IDCANCEL]
	if(hconn) RasHangUp hconn; hconn=0
	EndDialog hDlg 2
	ret
ret 1
