out
str s="abc abc"
str rx="(?C)c"
REPLACERX x

sel list("1[]2[]3")
	case 1
	out s.replacerx(rx ".")
	
	case 2
	x.fcallout=&callout2
	x.paramc=100
	x.repl="."
	out s.replacerx(rx x 4)
	
	case 3
	x.fcallout=&callout2
	x.paramc=100
	x.repl="."
	out s.replacerx(rx x)

out s
