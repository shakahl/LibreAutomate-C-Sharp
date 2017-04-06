function int'FirstLine int'LastLine str'List double&Amount

int i pos offset
double paid AmountAux
str a s sline

AmountAux=Amount

for i FirstLine LastLine
	sline.getl(List i)
	pos=findrx(sline " pays1 | notinterested | outofservice " 0 0 a)
	if pos<>-1
		s.left(sline pos-1)
		sel a
			case " pays1 " : offset=8
			case " notinterested " : offset=7
			case " outofservice " : offset=5
		s.get(sline pos+offset)
		if s="0.02"
			paid=0.02
		if s="0.01"
			paid=0.01
		AmountAux+paid

Amount=AmountAux