 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &TO_Options 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Options for floating toolbar dialogs"
 3 Button 0x54032000 0x0 4 4 98 14 "Multiline edit font..." "Change font used in dialogs 'Text', 'Message box' and some other"
 1 Button 0x54030001 0x4 4 116 48 14 "OK"
 2 Button 0x54030000 0x4 54 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\options.ico")) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 ;;set font used by sub_to.SetUserFont
	rget _s "toolfont" "\Tools"
	if(FontDialog(_s hDlg)) rset _s "toolfont" "\Tools"
	
	case IDOK
ret 1
