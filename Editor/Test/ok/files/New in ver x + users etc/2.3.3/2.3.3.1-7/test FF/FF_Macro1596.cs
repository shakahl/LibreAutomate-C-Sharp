 /
function# iid FILTER&f

out "ff %i" iid
 ret
 ret -1
if(iid=qmitem("key 2")) ret iid
if(iid=qmitem("mouse 2")) ret iid
if(iid=qmitem("window 4")) ret iid
ret -2
 ret -3
 ret iid
 ret "key 3"
 ret 4

 ret iid	;; run the macro.
 ret macro	;; run other macro. Here 'macro' is its id or name.
 ret 0		;; don't run any macros.
 ret -1		;; don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		;; don't run this macro. Other macros with the same trigger can run.
