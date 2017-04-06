\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
rea3="simple text red text link[]"
if(!ShowDialog("dlg_rich_text" &dlg_rich_text &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 4 4 216 104 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010800 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	int re=id(3 hDlg)
	CHARRANGE cr
	CHARFORMAT2 cf.cbSize=sizeof(CHARFORMAT2)
	 red text
	cr.cpMin=12
	cr.cpMax=20
	SendMessage(re EM_EXSETSEL 0 &cr)
	cf.dwMask=CFM_COLOR
	cf.crTextColor=0xff
	SendMessage(re EM_SETCHARFORMAT SCF_SELECTION &cf)
	 link
	cr.cpMin=21
	cr.cpMax=25
	SendMessage(re EM_EXSETSEL 0 &cr)
	cf.dwMask=CFM_LINK
	cf.dwEffects=CFE_LINK
	SendMessage(re EM_SETCHARFORMAT SCF_SELECTION &cf)
	 enable events
	SendMessage(re EM_SETEVENTMASK 0 ENM_LINK)
	 remove selection
	SendMessage(re EM_SETSEL 0 0)
	
	ret 1
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* n=+lParam
sel n.code
	case EN_LINK
	ENLINK* el=+n
	if(el.msg=WM_LBUTTONUP)
		out "a link clicked at %i-%i" el.chrg.cpMin el.chrg.cpMax
