 Sets or clears file in Documents that can be used to identify that this
 computer and user account is my main computer/account.

str s="$documents$\Main PC.txt"
sel list("Set this computer/account as main[]Set as not main")
	case 1 _s="Used by QM2 function 'IsMainPC'."; _s.setfile(s)
	case 2 del- s
