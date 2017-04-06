 /
function# iid FILTER&f

out f.tmon

if ym<ScreenHeight/2
	out 1
	 mac "m1"
else
	out 2
	 mac "m1"

 ret iid	- run the macro.
 ret macro	- run other macro. Here 'macro' is its id or name.
 ret 0		- don't run any macros.
 ret -1		- don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		- don't run this macro. Other macros with the same trigger can run.
