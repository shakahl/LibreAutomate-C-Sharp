 /
function# iid FILTER&f

 10
 out GetThreadPriority(GetCurrentThread)
Sleep 5000
 mes 1

ret iid
 ret macro	- run other macro. Here 'macro' is its id or name.
 ret 0		- don't run any macros.
 ret -1		- don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		- don't run this macro. Other macros with the same trigger can run.
