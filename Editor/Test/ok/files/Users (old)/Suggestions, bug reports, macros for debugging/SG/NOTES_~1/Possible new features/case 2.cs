 CASE for long type
 ==================

 Can case statement be made to accept long integers like we can in C++?
 Currently if we do:

long x=0x1234567890
sel x
	case 0x1234567890: out "ok"

 it gives "Error in case:  expected int constant."