 I agree.

//////////////////////////////

 Constant Expressions
 ====================


 If i have statements like:

def YBASEY 103
def YBLOCK 45	
def YFIRSTR (YBASEY+(YBLOCK/2))

 Qm calculates YFIRSTR everytime it encounters it, instead of assigning, only one time,
 the final value from calculation of (YBASEY+(YBLOCK/2)).  This seems inefficient, since 
 the value of YFIRSTR cannot change and need be calculated only once--the first time it 
 is defined.

 Also, in status bar, when carot is over YFIRSTR, Qm i think should show final value
 alongside with "(YBASEY+(YBLOCK/2))"
