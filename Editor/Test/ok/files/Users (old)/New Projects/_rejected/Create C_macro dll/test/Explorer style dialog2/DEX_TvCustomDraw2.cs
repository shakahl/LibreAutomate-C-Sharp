 /DEX_Main3
function# NMTVCUSTOMDRAW*cd

 out cd.nmcd.dwDrawStage
sel cd.nmcd.dwDrawStage
	case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW ;;yes, notify me to draw items
	case CDDS_ITEMPREPAINT
	sel cd.nmcd.lItemlParam
		case 2001 cd.clrText=0xFF
		case 2002 cd.clrText=0xFF00
		case 2003 cd.clrText=0xFF0000
	case else ret
