 /dlg_QM_Grid
function NMHDR*nh

 Examples for QM_Grid notifications.

GRID.QM_NMLVDATA* cd=+nh
NMLVDISPINFO* di=+nh
NMLISTVIEW* nlv=+nh
str s
sel nh.code ;;nh is NMHDR*, which is lParam of WM_NOTIFY message
	case LVN_BEGINLABELEDIT
		out "begin edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
	case LVN_ENDLABELEDIT
		out "end edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
	case GRID.LVN_QG_BUTTONCLICK:
		out "button: item=%i subitem=%i text=%s" cd.item cd.subitem cd.txt
		if(OpenSaveDialog(0 s))
			s.setwintext(cd.hctrl)
	case GRID.LVN_QG_COMBOFILL
		out "combo fill: item=%i subitem=%i" cd.item cd.subitem
		TO_CBFill cd.hcb "one[]two[]show inp"
	case GRID.LVN_QG_COMBOITEMCLICK
		out "combo click: item=%i subitem=%i cbitem=%i text=%s" cd.item cd.subitem cd.cbindex cd.txt
		if(cd.cbindex=2)
			if(inp(s))
				s.setwintext(cd.hctrl)
				ret 1
	case GRID.LVN_QG_CHANGE
		if(cd.hctrl) out "text changed: item=%i, subitem=%i, text=%s, newtext=%s" cd.item cd.subitem cd.txt _s.getwintext(cd.hctrl)
		else out "grid changed" ;;eg row deleted
	
	 the following notifications are not QM_Grid-specific
	
	case LVN_INSERTITEM ;;when user inserts new row
		out "inserted %i" nlv.iItem
	
	case LVN_ITEMCHANGED ;;when row state changes. If user selects multiple items with Shift, this is sent once.
	if(nlv.iItem<0 or nlv.uNewState&LVIS_SELECTED=0) ret
	out "selected %i" nlv.iItem ;;selected listview item index
	
	case NM_CLICK ;;when user clicks a row and it does not begin cell edit mode
	NMITEMACTIVATE* na=+nh
	out "row click: %i %i" na.iItem na.iSubItem
	
	case LVN_COLUMNCLICK ;;click header
		DlgGrid g.Init(nh.hwndFrom)
		g.Sort(4|0x10000 nlv.iSubItem)
