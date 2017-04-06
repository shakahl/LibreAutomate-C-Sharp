 /
function# hwnd message wParam lParam

 QM toolbar hook function that manages button checked states.
 In toolbar text finds text [check id] in button code comments, sets checkbox style for these buttons.
 Here id is some alphanumeric text, unique in the toolbar, used to save/restore button states with this name. Example: Label :out 1 ;;[check c1] * icon.ico
 When closing toolbar, saves button checked states in macro resources of the toolbar. When toolbar created or modified, restores button checked states.


 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	 gInit
	sub.CheckboxesInitOrSave 1 hwnd
	
	case WM_DESTROY
	sub.CheckboxesInitOrSave 0 hwnd
	
	case WM_NOTIFY
	NMHDR* nh=+lParam
	sel nh.code
		case TBN_DELETINGBUTTON ;;toolbar modified, need to update checkboxes
		SetTimer hwnd 8216 50 0
	
	case WM_TIMER
	sel wParam
		case 8216
		KillTimer hwnd wParam
		goto gInit


#sub CheckboxesInitOrSave
function !init hwnd

int htb=id(9999 hwnd)
int* p=+GetWindowLong(hwnd 0)
int iid=p[2]
str s1.getmacro(iid)
ARRAY(str) a=s1; int i
for i 1 a.len
	str& s=a[i]
	str cName
	if(findrx(s "[^ ;].+? :.+?;;.*?\[check +(\w+).*?\]" 0 0 cName 1)<0) continue
	 out s; out cName
	TBBUTTONINFOW b.cbSize=sizeof(b); b.dwMask=TBIF_STYLE|TBIF_STATE
	if(SendMessage(htb TB_GETBUTTONINFOW i &b)<0) continue
	int check=_qmfile.SettingGetI(+iid cName); err
	if init
		b.fsStyle|BTNS_CHECK
		if(check) b.fsState|TBSTATE_CHECKED
		SendMessage(htb TB_SETBUTTONINFOW i &b)
	else
		int icCheckedNow=b.fsState&TBSTATE_CHECKED!=0
		if(icCheckedNow!=check) _qmfile.SettingAddI(+iid cName icCheckedNow)
