QMITEM q
int itemId=qmitem("" 0 q 1); if(!itemId) ret

int hwndSecondaryEditor=id(2211 _hwndqm)
int isSplit=IsWindowVisible(hwndSecondaryEditor)
if(isSplit) int isSecondaryActive=(hwndSecondaryEditor=GetQmCodeEditor)

out F"isSplit={isSplit}, isSecondaryActive={isSecondaryActive}, QM item={q.name}"

int menuCmd
sel q.name 2
	case "* (right)" menuCmd=33546
	case "* (bottom)" menuCmd=33547
	case "* (primary)" menuCmd=33545
	case "* (primary&right)" menuCmd=33548
	case "* (primary&bottom)" menuCmd=33550
	case else
	if(isSplit) menuCmd=33545 ;;primary
if(!menuCmd) ret

int hwndMenuCommandReceiver=id(2213 _hwndqm)
men menuCmd hwndMenuCommandReceiver
