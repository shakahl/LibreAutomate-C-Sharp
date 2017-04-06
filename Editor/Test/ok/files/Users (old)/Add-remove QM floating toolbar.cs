sel list("Remove[]Add default[]Replace with a custom toolbar" "" "Add/remove QM floating toolbar")
	case 0 ret
	case 1 rset "" "Toolbar" "\Tools"
	case 2 rset "" "Toolbar" "\Tools" 0 -1
	case 3
	inp- _s "Toolbar name. Also can be a function."
	rset _s "Toolbar" "\Tools"

if(mes("Will be applied after restartying QM. Restart now?" "" "YN")='Y') shutdown -2
