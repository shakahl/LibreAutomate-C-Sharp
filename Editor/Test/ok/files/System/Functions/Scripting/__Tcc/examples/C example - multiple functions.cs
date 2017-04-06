 This macro shows 2 ways of getting addresses of multiple C functions.
 _______________________________________________

 1. Access function addresses through the return value.
__Tcc x1
int* a=x1.Compile("" "func1[]func2")
out call(a[0]) + call(a[1] 3)
 _______________________________________________

 2. Define a class or type that includes a __Tcc variable and immediately following int variables for function addresses.
class CFunctions __Tcc'__c func1 func2

 Call Compile with flag 8, and it will store function addresses in the int variables.
CFunctions x2.__c.Compile("" "func1[]func2" 8)
out call(x2.func1) + call(x2.func2 3)
 _______________________________________________

#ret

int func1()
{
	return 1;
}

int func2(int y)
{
	return y*2;
}
