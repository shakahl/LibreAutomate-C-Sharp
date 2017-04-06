 /
function$ hDlg $controls [$password]

 Gets dialog control values and creates XML.

 hDlg - hDlg.
 controls - space-delimited list of control ids. Example: "3 4 15".
 password - password control text encryption key. If not used, password control text will not be encryted.

 Format:
 Edit - text. Can optionally encrypt password control text (algorithm 9).
 Check, radio - 0, 1 or 2 (IsDlgButtonChecked).
 Button - nothing.
 Listbox - selected item index.
 Listbox multisel - list of selected items, like "0 3 4 ".
 Hotkey - hotkey as int (HKM_GETHOTKEY).
 Trackbar - position as int (TBM_GETPOS).
 IP address - IP as int (IPM_GETADDRESS).
 DateTime - SYSTEMTIME as string (DTM_GETSYSTEMTIME, getstruct).
 MonthCal - SYSTEMTIME as string (MCM_GETCURSEL, getstruct).
 Other - text (getwintext).


ARRAY(str) ac
tok controls ac

IXml xml=CreateXml
IXmlNode xr=xml.Add("r")

int i j h cid
str s
for i 0 ac.len
	cid=val(ac[i])
	h=id(cid hDlg)
	if(!h) out "Warning: there is no control %i in the dialog." cid; continue
	s=""
	sel _s.getwinclass(h) 1
		case "Edit"
		s.getwintext(h)
		if(!empty(password) and GetWinStyle(h)&ES_PASSWORD and s.len) s.encrypt(9 s password)
		
		case "Button"
		sel(GetWinStyle(h)&15) case [2,3,4,5,6,9] s=IsDlgButtonChecked(hDlg cid)
		
		case "ComboBox"
		j=SendMessage(h CB_GETCURSEL 0 0)
		if(j>=0) s=j
		
		case "ListBox"
		j=SendMessage(h LB_GETSELCOUNT 0 0)
		if(j<0) ;;singlesel
			j=SendMessage(h LB_GETCURSEL 0 0)
			if(j>=0) s=j
		else if(j) ;;multisel
			ARRAY(int) asi.create(j)
			SendMessage(h LB_GETSELITEMS j &asi[0])
			for(j 0 asi.len) s+asi[j]; s+" "
		
		case "msctls_hotkey32"
		j=SendMessage(h HKM_GETHOTKEY 0 0)
		if(j) s=j
		
		case "msctls_trackbar32"
		s=SendMessage(h TBM_GETPOS 0 0)
		
		case "SysIPAddress32"
		if(SendMessage(h IPM_GETADDRESS 0 &j)) s=j
		
		case "SysDateTimePick32"
		SYSTEMTIME st
		if(GDT_VALID=SendMessage(h DTM_GETSYSTEMTIME 0 &st)) s.getstruct(st 1)
		
		case "SysMonthCal32"
		if(SendMessage(h MCM_GETCURSEL 0 &st)) s.getstruct(st 1)
		
		case else
		s.getwintext(h)
	
	if s.len
		IXmlNode xc=xr.Add(_s.from("c" cid) s)

xml.ToString(this)

err+ end _error
