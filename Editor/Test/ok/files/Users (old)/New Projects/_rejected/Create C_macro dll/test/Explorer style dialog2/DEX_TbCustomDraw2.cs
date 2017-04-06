 /DEX_Main3
function# NMTBCUSTOMDRAW*cd

 out cd.nmcd.dwDrawStage
sel cd.nmcd.dwDrawStage
	case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW ;;yes, notify me to draw items
	case CDDS_ITEMPREPAINT
	 cd.clrText=0xFFFFFF
	sel cd.nmcd.dwItemSpec
		case 1001 cd.clrText=0xFF
		case 1002 cd.clrText=0xFF00
		case 1003 cd.clrText=0xFF0000
	case else ret
