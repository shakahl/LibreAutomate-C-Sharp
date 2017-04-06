 /dialog_QM_Tools
function wParam lParam

int cid(wParam&0xffff) msg(wParam>>16)
sel wParam
	
	 WINDOW
	
	case [510,511,512] if(but(lParam)) _WinSelect(wParam-510) ;;option Screen, Window, Control
	 note: don't use 'uncheck' notification, because of Windows bug: unchecked option button sends 'uncheck' notification when receives focus
	
	case EN_SETFOCUS<<16|500 if(mw_what<1) _WinSelect(1)
	case EN_SETFOCUS<<16|501 if(mw_what<2) _WinSelect(2)
	case CBN_DROPDOWN<<16|500 _WinList
	case EN_CHANGE<<16|500 SendMessageW m_hparent __TWN_WINDOWCHANGED 0 mw_heW
	 case EN_CHANGE<<16|501 ;;currently don't need
	
	case 521 _WinTest ;;Test
	
	case 522 ;;Edit...
	sel mw_what
		case 1 _WinDialog
		case 2 _WinChildDialog
