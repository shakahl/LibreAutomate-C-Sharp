Assume the UDF name is Func.

We pass address of destination (destPtr) to Func.
	If var=Func, destPtr is &var.
	If Func, destPtr is &autoVar.
	If UdfFunc Func, destPtr is &arg.
	If OtherFunc Func, destPtr is &arg. Add destPtr to cd.
	If ret Func, ...

On ret:
	If ret local, swap destPtr with local. Don't do it if class with spec functions, as it may have unexpected behavior, eg user may want to have normal dtor when Func exits, but the variable is empty etc.
	Else copy *destPtr=retVal. (normal, not memcpy).
	Or Func can access destDir directly, using spec var _ret.
	