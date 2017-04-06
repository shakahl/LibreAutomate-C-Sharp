\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages


str controls = "3"
str rea3
rea3="rich text"
if(!ShowDialog("Dialog_richedit_colors" &Dialog_richedit_colors &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 0 0 222 84 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int h(id(3 hDlg)) bcol(0x000000) tcol(0x00FF00)
	
	SendMessage h EM_SETBKGNDCOLOR 0 bcol
	
	CHARFORMAT cf.cbSize=sizeof(CHARFORMAT)
	cf.dwMask=CFM_COLOR|CFM_BOLD|CFM_FACE|CFM_SIZE
	cf.crTextColor=tcol
	cf.dwEffects=CFE_BOLD
	lstrcpyn(&cf.szFaceName "Comic Sans MS" 32)
	cf.yHeight=400 ;;twips
	
	SendMessage h EM_SETCHARFORMAT SCF_ALL &cf
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
