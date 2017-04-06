function test item $txt
str s
int w1

sel test
	case 1
	w1=id(1 win("My QM" "CabinetWClass"))
	rep
		if(!GetListViewItemText(w1 item s) or !s.beg(txt)) out "failed"
	
	case 2
	w1=id(40961 win("My QM" "CabinetWClass"))
	rep
		if(!GetStatusBarText(w1 item s) or !s.beg(txt)) out "failed"
	
