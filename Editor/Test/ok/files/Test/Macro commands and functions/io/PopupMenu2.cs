str s = "&A)First item[]&B) Second Item[]&C) Third Item"
sel PopupMenu(s 400 300 0 1)
	case 1 out "A"
	case 2 out "B"
	case 3 out "C"
	case else out "Cancel"

sel PopupMenu("Item1[]Item2")
	case 1 out 1
	case 2 out 2
	case else ret

str fl.set(0 0 6)
fl[1]=1
fl[2]=2
fl[4]=4
fl[5]=4|2
int i=PopupMenu("Normal[]Disabled[]Checked[]-[]Radio[]Radio checked" 0 0 fl)

s="1[]>2[]3[]4[]<[]>5[]>6[]7[]8[]<[]9"
out PopupMenu(s 0 0 0 1)
