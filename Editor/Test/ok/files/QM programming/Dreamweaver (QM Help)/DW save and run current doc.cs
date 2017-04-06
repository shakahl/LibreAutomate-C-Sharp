function browser [$_file] ;;browser: 0 FF, 1 IE, 2 Chrome, 3 Opera

act "Dreamweaver"
key Cs

str s
if(empty(_file)) s=DW_GetCurrentDocumentFile
else sel(_file) case "<refresh>" int refresh=1; goto g1; case else s=_file

if(!dir(s)) ret

 not all browsers support eg .dwt (Dreamweaver template), so save it as html
if(!s.endi(".html"))
	str st=s;
	st.replacerx("\..+?$" "_temp.html")
	cop- s st
	s=st

 g1
if browser=0
	int h=FindTaggedWindow("test" "Firefox" "MozillaWindowClass")
	if(h) act h
	else run "firefox" "" "" "" 0x2800 win("Firefox" "MozillaWindowClass") h; TagWindow h "test"

if refresh
	sel browser
		case 0 act "Firefox"
		case 1 act "Internet Explorer"
		case 2 act "Chrome"
		case 3 act "Opera"
	key F5
else
	sel browser
		case 0 run "firefox" s; 0.5; act h
		case 1 run "iexplore" s
		case 2 run "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" s
		case 3 run "$pf$\Opera\opera.exe" s
