\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD="<HTML></HTML>"
if(!ShowDialog("dlg_html_editor" &dlg_html_editor &controls 0 0 0 0 0 0 0 "" "dlg_html_editor")) ret

 BEGIN DIALOG
 0 "" 0x90CF0A48 0x100 0 0 489 360 ""
 3 ActiveX 0x54030000 0x0 0 0 444 322 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
type DHEDATA hDlg hwb MSHTML.IHTMLDocument2'doc
DHEDATA- t

sel message
	case WM_INITDIALOG
	t.hDlg=hDlg; t.hwb=id(3 hDlg)
	SHDocVw.WebBrowser we3
	we3._getcontrol(t.hwb); t.doc=we3.Document
	t.doc.designMode="On"
	dlg_html_editor hDlg WM_SIZE 0 0
	PostMessage hDlg WM_APP 0 0 ;;sometimes cannot get HTML now
	
	case WM_APP
	DHE_OpenSave -1
	
	case WM_DESTROY
	
	case WM_SIZE
	RECT rc; GetClientRect hDlg &rc; MoveWindow t.hwb 0 0 rc.right rc.bottom 1
	
	case WM_INITMENUPOPUP
	_s=t.doc.designMode
	CheckMenuItem wParam 301 iif(_s~"On" 0 MF_CHECKED)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 100 ;;New
	DHE_OpenSave 0
	
	case 101 ;;Open
	DHE_OpenSave 1
	
	case [102,103] ;;Save, Save As
	DHE_OpenSave wParam-100
	
	case 301 ;;Browse Mode
	_s=t.doc.designMode; t.doc.designMode=iif(_s~"On" "Off" "On")
	
	case 901 ;;Help
	mes "Right click to insert image etc" "" "i"
	
	case IDCANCEL
	if(!DHE_OpenSave(4)) ret
ret 1

err+ mes "Error: %s[]In: %s" "" "!" _error.description _error.line

 BEGIN MENU
 >&File
	 &New : 100 0 0 Cn
	 &Open : 101 0 0 Co
	 &Save : 102 0 0 Cs
	 Save &As... : 103
	 -
	 >&Recent
		 not implemented : 190 0 1
		 <
	 -
	 E&xit : 2 0 0 AF4
	 <
 >&Edit
	 Empty : 201 0 1
	 <
 >&View
	 Browse Mode : 301 0 0 Cb
	 <
 &Help : 901 0
 END MENU
