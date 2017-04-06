 /
function hwnd

sel PopupMenu(_s.getmacro("notemenu"))
	case 1 clo hwnd ;;close
	 -
	case 3 ShowNote ;;new
	case 4 ShowNote "?" ;;open
	case 5 if(ShowNote("?")) clo hwnd ;;close&open
	case 6 run _s.expandpath("$personal$\Notes"); err ;;notes
	case 7 mes "Cleanup is still not implemented."
	case 8 Note_OpenSave 1 hwnd ;;save
	case 9 Note_OpenSave 3 hwnd ;;save as
	 -
	case 11 Note_OpenSave 2 hwnd ;;load text
