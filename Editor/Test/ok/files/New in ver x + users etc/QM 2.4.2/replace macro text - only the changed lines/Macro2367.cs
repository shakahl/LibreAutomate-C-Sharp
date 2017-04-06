out
dll "qm.exe" #GetTextDiffRange $sa $sb CHARRANGE&ca CHARRANGE&cb

str sa=
 one
 two
 three
;

str sb=
 one
 two
 three
;

CHARRANGE ca cb
sel GetTextDiffRange(sa sb ca cb)
	case 0 out "completely different"
	case 1 out "equal"
	case 2
	out "different: %i %i    %i %i" ca.cpMin ca.cpMax cb.cpMin cb.cpMax
	