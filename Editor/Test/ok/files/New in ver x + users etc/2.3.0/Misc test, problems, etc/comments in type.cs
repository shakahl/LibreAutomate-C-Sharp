type TC1 c d ;;comm
type TC2
	a ;;comm
	b ;;comm
type TC3
	a ;;comm
	;;only comm
	b ;;comm
 type TC4
	 a ;;comm
	  only comm
	 b ;;comm
type TC5 ;;comm
	a ;;comm
	b ;;comm

TC1 a
TC2 b
TC3 c
c.a=3
c.b=4
out c.a
out c.b
 ICsv