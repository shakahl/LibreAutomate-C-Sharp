 \
function# iid FILTER&f

if &f
	if(GetWindowThreadProcessId(iif(f.hwnd2 f.hwnd2 f.hwnd) 0)=GetWindowThreadProcessId(_hwndqm 0)) ret -2
	if(!f.hwnd2 or f.hwnd2=f.hwnd) ret -2
	if(child=f.hwnd2) ret -2
	 outw f.hwnd2
	mac getopt(itemid) "" f.hwnd2
	ret -1

act iid; err

 ret iid	;; run the macro.
 ret macro	;; run other macro. Here 'macro' is its id or name.
 ret 0		;; don't run any macros.
 ret -1		;; don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		;; don't run this macro. Other macros with the same trigger can run.
