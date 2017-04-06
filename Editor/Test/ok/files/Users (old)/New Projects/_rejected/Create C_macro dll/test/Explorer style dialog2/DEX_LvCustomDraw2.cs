 /DEX_Main3
function# NMLVCUSTOMDRAW*cd

 out cd.nmcd.dwDrawStage
sel cd.nmcd.dwDrawStage
	case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW ;;yes, notify me to draw items
	case CDDS_ITEMPREPAINT
	cd.clrTextBk=iif(cd.nmcd.dwItemSpec&1 0xC0FFC0 0xC0FFFF) ;;makes alternating row colors
	case else ret

