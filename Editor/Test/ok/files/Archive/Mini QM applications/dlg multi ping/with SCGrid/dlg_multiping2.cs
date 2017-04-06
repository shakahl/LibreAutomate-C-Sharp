\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib prjSCGrid {28FDFE2B-68C4-11D4-92BF-8364E6807441} 13.8
 note: scgrid must be downloaded and registered. If version is different, delete the above line and insert again using Type Libraries dialog.

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
if(!ShowDialog("dlg_multiping2" &dlg_multiping2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A48 0x100 0 0 224 224 "QM Multi Ping"
 8 Button 0x54032000 0x0 0 0 40 14 "Start"
 9 Button 0x54032000 0x0 40 0 40 14 "Stop"
 6 Static 0x54000000 0x0 86 2 24 10 "Period"
 7 Edit 0x54032000 0x200 110 0 28 14 ""
 5 Button 0x54032000 0x0 144 0 40 14 "List"
 4 Button 0x54032000 0x0 184 0 40 14 "Help"
 3 ActiveX 0x54000000 0x0 0 16 216 188 "prjSCGrid.SCGrid"
 END DIALOG
 DIALOG EDITOR: "" 0x2030008 "*" "" ""

ret
 messages
int+ __multiping_started

sel message
	case WM_INITDIALOG
	EnableWindow(id(9 hDlg) 0)

 In QM, scgrid does not work well:
    1. Cells are not displayed until LoadFromArray.
    2. Fails to create control after RT error. Restart QM.

	prjSCGrid.SCGrid gridping
	gridping._getcontrol(id(3 hDlg))
	 gridping._setevents("gridping___SCGrid")
	
	gridping.DefaultHeight=18
	ARRAY(VARIANT) _a.create(1 1); gridping.LoadFromArray(_a -1); gridping.ResetGrid
	gridping.Cols=3
	gridping.RowBackColor(0)=0xe0c0c0
	gridping.RowStyle(0)=2
	gridping.Text(0 1)="time"
	gridping.Text(0 2)="TTL"
	
	dlg_multiping_load2 hDlg
	
	goto size
	
	case WM_DESTROY
	__multiping_started=0
	
	case WM_SIZE
	 size
	RECT rc rb
	GetClientRect(hDlg &rc)
	GetWindowRect(id(8 hDlg) &rb)
	int gtop=rb.bottom-rb.top+2
	MoveWindow id(3 hDlg) 0 gtop rc.right rc.bottom-gtop 1
	gridping._getcontrol(id(3 hDlg))
	GetClientRect(id(3 hDlg) &rc)
	gridping.ColWidth(0)=rc.right-gridping.ColWidth(1)-gridping.ColWidth(2)
	
	case WM_COMMAND goto messages2
	
	case WM_APP ;;from dlg_multiping_thread
	gridping._getcontrol(id(3 hDlg))
	gridping.RowBackColor(wParam)=iif(lParam 0xffffff 0xffff); err ret
	str t(lParam&0xffff) ttl(lParam>>16)
	gridping.Text(wParam 1)=iif(lParam t "")
	gridping.Text(wParam 2)=iif(lParam ttl "")
	
ret
 messages2
sel wParam
	case 8 ;;Start
	__multiping_started=1
	EnableWindow(id(8 hDlg) 0)
	EnableWindow(id(9 hDlg) 1)
	if(!dlg_multiping_load2(hDlg 1)) goto stop
	
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
