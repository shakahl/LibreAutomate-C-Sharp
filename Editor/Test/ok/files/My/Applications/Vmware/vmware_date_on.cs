 \
function event pid pidparent $name
 event: 1 started, 2 ended, 4 running

int h=wait(60 WV win(" - VMware Player" "VMPlayerFrame" pid)); err ret

str sd
sel _s.getwintext(h) 2
	case "*@Vista-64*" sd="1/1/2007"
	 case "*@Vista-32*" sd="24/10/2010"
	case "*@Win7-64*" sd="1/1/2010"
	case "*@2008*" sd="24/8/2009"
	 case "*@Win8-*" sd="09/01/2013"
	case else ret

VmwareDate sd pid
 30
 if(mes("Restore date now on host PC?" "" "OC")='O') VmwareDate "" pid

 We use process trigger instead of window because window triggers don't work with vmware player window because disabled in file properties.
