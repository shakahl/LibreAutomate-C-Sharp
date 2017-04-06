\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 582 382 "Dialog"
 3 ActiveX 0x54030000 0x0 0 48 582 334 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 4 ToolbarWindow32 0x54030001 0x0 0 0 582 17 ""
 5 Edit 0x54030080 0x200 0 28 582 13 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "3 5"
str ax3SHD e5
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

int- t_hdlg
__ImageList-- t_il
sel message
	case WM_INITDIALOG
	t_hdlg=hDlg
	DT_SetAutoSizeControls hDlg "3s 5sh"
	sub.ToolbarInit id(4 hDlg) t_il
	
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3._setevents("sub.we3")
	
	we3.Silent=TRUE ;;prevent script error messages
	we3.Navigate("http://www.google.com") ;;or we3.GoHome
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [101,102,103,104,105,106]
	err-
	we3._getcontrol(id(3 hDlg))
	sel wParam
		case 101 we3.GoBack
		case 102 we3.GoForward
		case 103 we3.Stop
		case 104 we3.Refresh
		case 105 we3.GoHome
		case 106 we3.Navigate(_s.getwintext(id(5 hDlg)))
	err+
	
	case IDOK
	if(GetFocus=id(5 hDlg)) PostMessage hDlg WM_COMMAND 106 0 ;;on Enter press Go
	ret 0 ;;disable closing on Enter
	case IDCANCEL
	ifk(Z) ret 0 ;;disable closing on Esc
ret 1


#sub ToolbarInit
function htb __ImageList&il

il.Load("$qm$\il_qm.bmp") ;;load an imagelist created with the imagelist editor
  or
 il.Load("resource:<QM web browser>image:QM web browser") ;;load an imagelist created with the imagelist editor and added to macro resources
  or
 il.Create("file1.ico[]file2.ico[]file3.ico") ;;create imagelist at run time from icons. Slower.

SendMessage htb TB_SETIMAGELIST 0 il
SetWinStyle htb TBSTYLE_FLAT|TBSTYLE_TOOLTIPS 1

ARRAY(str) as="Back[]Forward[]Stop[]Refresh[]Home[]Go"
ARRAY(TBBUTTON) ab.create(as.len)
int i
for i 0 ab.len
	TBBUTTON& t=ab[i]
	t.idCommand=101+i
	t.iBitmap=i
	t.iString=SendMessage(htb TB_ADDSTRINGA 0 as[i])
	t.fsState=TBSTATE_ENABLED

SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
SendMessage(htb TB_ADDBUTTONS ab.len &ab[0])
SendMessage(htb TB_AUTOSIZE 0 0)


#sub we3_DocumentComplete
function IDispatch'pDisp VARIANT&URL SHDocVw.WebBrowser'c

 out URL

int- t_hdlg
str s=c.LocationURL
s.setwintext(id(5 t_hdlg))

 BEGIN PROJECT
 main_function  QM web browser
 exe_file  $desktop$\QM web browser.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {5C25336D-BE35-4BAF-8461-DABD57D6FB06}
 END PROJECT
