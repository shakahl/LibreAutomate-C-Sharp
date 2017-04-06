 _s="[39]'"

str b="[39][39]"
 out b

def EMPTYSTRINGSQ "[39][39]"

sel b 9
	case "ON": b="OFF"
	case "OFF": b="ON"
	case "True" : b="False"
	case "False" : b="True"
	case "NULL" : b="[39][39]"
	case "[39][39]" : b="NULL" ;;QM bug: unescapes case strings 2 times
	 case else if(b="[39][39]") b="NULL" ;;workaround
	 case EMPTYSTRINGSQ : b="NULL" ;;workaround 2

out b

 also:
 #out "[39][39]"
 #exe addfile "[39][39]" "[39][39]" "[39][39]"
 dll "[39][39]" Moo
