\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD="http://www.google.com"
if(!ShowDialog("dlg_web_browser_zoom" &dlg_web_browser_zoom &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 327 191 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 328 170 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 12 174 48 14 "Zoom font"
 5 Button 0x54032000 0x0 66 174 48 14 "Zoom optical"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	 we3._getcontrol(id(3 hDlg))
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 ;;Zoom font
	
	 remember font size in registry
	str fs fsrk="Software\Microsoft\Internet Explorer\International\Scripts\3"
	rget(fs "IEFontSize" fsrk 0 "" REG_BINARY)
	
	we3._getcontrol(id(3 hDlg))
	VARIANT v=0 ;;0 to 4, default 2
	we3.ExecWB(OLECMDID_ZOOM OLECMDEXECOPT_DONTPROMPTUSER v 0)
	
	 restore font size in registry
	if(fs.len) rset(fs "IEFontSize" fsrk 0 REG_BINARY)
	
	case 5 ;;Zoom optical
	if(_iever<0x700) mes "supported in IE7 and later"; ret
	we3._getcontrol(id(3 hDlg))
	v=200 ;;10 to 1000 %, default 100
	we3.ExecWB(OLECMDID_OPTICAL_ZOOM OLECMDEXECOPT_DONTPROMPTUSER v 0)
	
	case IDOK
	case IDCANCEL
ret 1
