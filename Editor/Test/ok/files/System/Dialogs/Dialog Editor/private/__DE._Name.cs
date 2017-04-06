function# ___DE_CONTROL&c h str&name str&varname

 Gets control text and formats variable name.

name=c.txt
str cls.getwinclass(h) txt
int i j

sel cls 1
	case "Button" sel(c.style&BS_TYPEMASK) case [2,3,5,6] cls="c"; case [4,9] cls="o"; case 7 cls="gr"; case else cls="b"
	case "Static" sel(c.style&SS_TYPEMASK) case SS_ICON cls="si"; case SS_BITMAP cls="sb"; case else cls="st"
	case "Edit" cls="e"
	case ["RichEdit20A","RichEdit20W","RichEdit50W"] cls="re"
	case "ComboBox" cls="cb"
	case "QM_ComboBox" cls="qmcb"
	case "ListBox" cls="lb"
	case "ComboBoxEx32" cls="cbe"
	case "msctls_hotkey32" cls="hk"
	case "msctls_progress32" cls="pr"
	case "msctls_statusbar32" cls="stb"
	case "msctls_trackbar32" cls="trb"
	case "msctls_updown32" cls="ud"
	case "ReBarWindow32" cls="rb"
	case "ScrollBar" cls="scb"
	case "SysAnimate32" cls="ani"
	case "SysDateTimePick32" cls="dt"
	case "SysHeader32" cls="hd"
	case "SysIPAddress32" cls="ip"
	case "SysLink" cls="link"
	case "SysListView32" cls="lv"
	case "SysMonthCal32" cls="mc"
	case "SysPager" cls="pg"
	case "SysTabControl32" cls="tab"
	case "SysTreeView32" cls="tv"
	case "ToolbarWindow32" cls="tb"
	case "#32770" cls="d"
	case else
	for(i 0 cls.len) if(isupper(cls[i])) cls[j]=tolower(cls[i]); j+1; if(j>3) break
	if(j) cls.fix(j)
	else cls.replacerx("[^a-z]"); if(cls.len) cls.fix(4); else cls="_"

if(name.len) txt.gett(name 0 "," 0x200); txt.replacerx("[^A-Z_a-z]"); txt.fix(3)
varname.format("%s%i%s" cls c.cid txt) 
