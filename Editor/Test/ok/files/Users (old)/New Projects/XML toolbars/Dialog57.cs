\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

dll "qm.exe" #CreateXmlToolbar flags $xmlFile $tbName $bmpFile maskColor hwndParent ctrlId [style] [tbexstyle]

if(!ShowDialog("Dialog57" &Dialog57)) ret

 BEGIN DIALOG
 0 "" 0x90CC0A44 0x100 0 0 219 132 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	lpstr xml=
 <toolbars>
 	<tools>
 		<b id="2011" i="9">Icons    Alt+F11</b>
 		<b id="2010" i="10">Options    Alt+F12</b>
 		<sep></sep>
 		<b id="2024" i="12" style="8">Help</b>
 	</tools>
 	<edit>
 		<b id="2009" i="10">Record    Ctrl+K</b>
 		<b id="2003" i="8">My Macros    Alt+F9</b>
 		<sep></sep>
 		<b id="2005" i="3" style="8">Run    Ctrl+R</b>
 		<sep></sep>
 		<b id="2006" i="4" state="0">Undo    Ctrl+Z</b>
 		<b id="2007" i="5" state="0">Redo    Ctrl+Y</b>
 		<b id="2002" i="6" style="8">Previous    Ctrl+Tab</b>
 	</edit>
 </toolbars>
	int h=CreateXmlToolbar(1 xml "edit" "de_ctrl.bmp" 0xff00ff hDlg 300 0 TBSTYLE_EX_DRAWDDARROWS)
	 out h
	case WM_DESTROY
	case WM_SIZE
	SendMessage id(300 hDlg) TB_AUTOSIZE 0 0
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

