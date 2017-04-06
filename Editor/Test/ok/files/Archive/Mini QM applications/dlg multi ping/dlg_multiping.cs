\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 ------------------------
 Create sample file containing list of IPs and/or web servers and/or network computer names
 Remove this code if not needed.
str pl="$my qm$\ping list.txt"
if(!dir(pl))
	str spl=
  SAMPLE LIST
 
 ;web servers
 www.quickmacros.com
 www.google.com
 www.download.com
 
 ;IP
 255.255.255.255
 84.32.123.1
	if(!dir("$my qm$" 1)) mkdir "$my qm$"
	spl.setfile(pl)
 ------------------------

str controls = "7"
str e7
e7=5
if(!ShowDialog("dlg_multiping" &dlg_multiping &controls)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A48 0x100 0 0 224 224 "QM Multi Ping"
 8 Button 0x54032000 0x0 0 0 40 14 "Start"
 9 Button 0x54032000 0x0 40 0 40 14 "Stop"
 6 Static 0x54000000 0x0 86 2 24 10 "Period"
 7 Edit 0x54032000 0x200 110 0 28 14 ""
 5 Button 0x54032000 0x0 144 0 40 14 "List"
 4 Button 0x54032000 0x0 184 0 40 14 "Help"
 3 SysListView32 0x54000049 0x0 0 16 216 188 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030008 "*" "" ""

ret
 messages
int+ __multiping_started
int hlv

sel message
	case WM_INITDIALOG
	EnableWindow(id(9 hDlg) 0)
	
	hlv=id(3 hDlg)
	int es=LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP|LVS_EX_GRIDLINES
	SendMessage hlv LVM_SETEXTENDEDLISTVIEWSTYLE es es
	TO_LvAddCol hlv 0 "" 150
	TO_LvAddCol hlv 1 "Time" 50
	TO_LvAddCol hlv 2 "TTL" 50
	
	dlg_multiping hDlg WM_SIZE 0 0
	goto start
	 dlg_multiping_load hDlg
	
	case WM_DESTROY
	__multiping_started=0
	
	case WM_SIZE
	 size
	RECT rc rb
	GetClientRect(hDlg &rc)
	GetWindowRect(id(8 hDlg) &rb)
	int gtop=rb.bottom-rb.top+2
	hlv=id(3 hDlg)
	MoveWindow hlv 0 gtop rc.right rc.bottom-gtop 1
	GetClientRect(hlv &rc)
	SendMessage hlv LVM_SETCOLUMNWIDTH 0 rc.right-100
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	
	case WM_APP ;;from dlg_multiping_thread
	hlv=id(3 hDlg)
	LVITEM li lip
	lip.mask=LVIF_PARAM; lip.lParam=!lParam; lip.iItem=wParam; SendMessage hlv LVM_SETITEM 0 &lip
	str t(lParam&0xffff) ttl(lParam>>16)
	li.pszText=iif(lParam t ""); li.iSubItem=1; SendMessage hlv LVM_SETITEMTEXT wParam &li
	li.pszText=iif(lParam ttl ""); li.iSubItem=2; SendMessage hlv LVM_SETITEMTEXT wParam &li
	
ret
 messages2
sel wParam
	case 8 ;;Start
	 start
	__multiping_started=1
	EnableWindow(id(8 hDlg) 0)
	EnableWindow(id(9 hDlg) 1)
	if(!dlg_multiping_load(hDlg 1)) goto stop
	
	case 9 ;;Stop
	 stop
	__multiping_started=0
	EnableWindow(id(8 hDlg) 1)
	EnableWindow(id(9 hDlg) 0)
	
	case 5 ;;List
	run "$my qm$\ping list.txt"; err
	
	case 4 ;;Help
	str sh=
 Pings multiple computers on the internet or local network.
 Displays roundtrip time in milliseconds and TTL.
 If a computer is inaccessible, the line is yellow.
 
 List of computers must be in "%s".
 A computer can be specified by IP (eg 123.45.67.89), or web server (www.xxx.com), or network computer name.
 Lines starting with space or semicolon are ignored. Also empty lines.
 If the file does not exist, creates sample file.
 You can edit the file in notepad.
 If started, after editing list or period press Stop and Start.
 
 Creates thread for each computer. Don't monitor too many computers. 100 is OK.
	mes sh "" "" pl.expandpath("$my qm$\ping list.txt")
	
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3 ;;list
	sel nh.code
		case NM_CUSTOMDRAW
		NMLVCUSTOMDRAW* cd=+nh
		sel cd.nmcd.dwDrawStage
			case CDDS_PREPAINT ret DT_Ret(hDlg CDRF_NOTIFYITEMDRAW)
			case CDDS_ITEMPREPAINT
			cd.clrTextBk=iif(cd.nmcd.lItemlParam 0xffff iif(cd.nmcd.dwItemSpec&1 0xF8F8F8 0xFFFFFF))
