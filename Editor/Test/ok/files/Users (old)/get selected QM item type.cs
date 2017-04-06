int htv=id(2202 _hwndqm) ;;get QM tree view handle (HWND)
int hitem=SendMessage(htv TVM_GETNEXTITEM TVGN_CARET 0) ;;get selected item (HTREEITEM)
int iid=TvGetParam(htv hitem) ;;get lParam. It is QM item id. This function is in System. It is private, but it is safe to use in macros.
QMITEM qi; qmitem(iid 0 qi) ;;get QM item info
sel qi.itype ;;see qmitem in QM help
	case 0 out "macro"
	case 1 out "function"
	case 2 out "menu"
	case 3 out "tb"
	case 4 out "tsm"
	case 5 out "folder"
	case 6 out "member f"
