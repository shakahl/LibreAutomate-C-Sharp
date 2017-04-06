 This is quite good, and maybe not too much work to implement.
 Use type FUNCTION. Declare function address variable like FUNCTION(retType parameters) funcVar.
 Examples:

FUNCTION(int x $y) funcVar

type TestFunctions
	FUNCTION(- x $y)funcVar1
	FUNCTION($ x str&y)funcVar2

 Can implement using g_dll.
 Code 'FUNCTION(retType parameters) funcVar' would be like now 'dll- "" retType'funcVar parameters'.
 Code 'funcVar=address' would be like now '&DeclaredFunction=address'.

 ____________________________

 Or declare function type. Could try to implement through g_dll.
 Examples:

functype FUNCTYPE retType x $y
FUNCTYPE funcVar


