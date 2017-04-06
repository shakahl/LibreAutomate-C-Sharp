 dll "qm.exe"
	 __VarProp_Set !*varAddr value
	 #__VarProp_Remove !*varAddr
	 #__VarProp_Get !*varAddr [int*found]
	 #__VarProp_GetVar value [int*found]

out
int x
__VarProp_Set &x 7
out __VarProp_Get(&x)
out __VarProp_Get(&x &_i); out _i
out __VarProp_GetVar(7)
out __VarProp_GetVar(7 &_i); out _i
out __VarProp_GetVar(1)
out __VarProp_GetVar(1 &_i); out _i
out __VarProp_Remove(&x)
out __VarProp_Get(&x &_i); out _i
__VarProp_Set &x 8
out __VarProp_Get(&x)
__VarProp_Set &x 9
out __VarProp_Get(&x)
