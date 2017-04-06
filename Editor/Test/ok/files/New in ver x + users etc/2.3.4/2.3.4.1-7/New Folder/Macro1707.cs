out
#compile "__WaitWinMulti"
WaitWinMulti x
x.AddMsgBox("QM Message" "1")
x.AddMsgBox("QM Message" "*Two*")
sel x.WaitActive
	case 1 out 1
	case 2 out 2
