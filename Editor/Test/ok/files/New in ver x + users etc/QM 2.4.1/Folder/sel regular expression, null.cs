out
int r
 str s="Test"
str s="Nest"
 sel s
	 case "a" r=1
	 case "test" r=2
	 case else r=0
 sel s 3
	 case "a" r=1
	 case "te*" r=2
	 case else r=0

PF
sel s 7
	case "a" r=1
	case "te*" r=2
	case "$^me.*$" r=3
	case "$^me.*$" r=3
	case "$^me.*$" r=3
	case "$^me.*$" r=3
	case ["pe*","$^ne.*$"] r=4
	case else r=0
PN;PO

 sel s 5
	 case "a" r=1
	 case "a.+"
	 case ".+g"
	 case "(^$)|(^\-1$)|(^-$)|(^([0-9]){0,4}$)"
	 case "$^te..$" r=2
	 case else r=0

 str s;;=""
 sel s 8
	 case "" r=1
	  case 0 r=0

out r
