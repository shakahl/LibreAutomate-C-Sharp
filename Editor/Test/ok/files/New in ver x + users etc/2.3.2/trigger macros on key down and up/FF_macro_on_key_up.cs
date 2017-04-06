 /
function# iid FILTER&f

 ignore repeated key down events
int+ g_macro_key_down g_macro_key_time
if(g_macro_key_down and GetTickCount-g_macro_key_time<10000) ret iid
g_macro_key_down=1; g_macro_key_time=GetTickCount

mac "macro_on_key_down" ;;run on key down (now)
ret iid ;;run on key up

 ret iid	;; run the macro.
 ret macro	;; run other macro. Here 'macro' is its id or name.
 ret 0		;; don't run any macros.
 ret -1		;; don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		;; don't run this macro. Other macros with the same trigger can run.
