 /
function$ hDlg [$password]

 Sets dialog control values from XML created by GetDialogXml.

 hDlg - hDlg.
 password - password control text encryption key.


if(!this.len) ret
IXml xml=CreateXml
xml.FromString(this)
ARRAY(IXmlNode) a
xml.Path("r/*" a)

int i j h cid
str s
for i 0 a.len
	IXmlNode& n=a[i]
	cid=val(n.Name+1)
	h=id(cid hDlg)
	if(!h) out "Warning: there is no control %i in the dialog." cid; continue
	s=n.Value
	j=val(s)
	
	sel _s.getwinclass(h) 1
		case "Edit"
		if(!empty(password) and GetWinStyle(h)&ES_PASSWORD) s.decrypt(9 s password)
		s.setwintext(h)
		
		case "Button"
		sel(GetWinStyle(h)&15) case [2,3,4,5,6,9] CheckDlgButton(hDlg cid j)
		
		case "ComboBox"
		SendMessage(h CB_SETCURSEL j 0)
		
		case "ListBox"
		if(SendMessage(h LB_GETSELCOUNT 0 0)<0) ;;singlesel
			SendMessage(h LB_SETCURSEL j 0)
		else ;;multisel
			ARRAY(str) asi; tok s asi
			for(j 0 asi.len) SendMessage(h LB_SETSEL 1 val(asi[j]))
		
		case "msctls_hotkey32"
		SendMessage(h HKM_SETHOTKEY j 0)
		
		case "msctls_trackbar32"
		SendMessage(h TBM_SETPOS 1 j)
		
		case "SysIPAddress32"
		SendMessage(h IPM_SETADDRESS 0 j)
		
		case "SysDateTimePick32"
		SYSTEMTIME st
		s.setstruct(st 1)
		SendMessage(h DTM_SETSYSTEMTIME GDT_VALID &st)
		
		case "SysMonthCal32"
		s.setstruct(st 1)
		SendMessage(h MCM_SETCURSEL 0 &st)
		
		case else
		s.setwintext(h)

err+ end _error
