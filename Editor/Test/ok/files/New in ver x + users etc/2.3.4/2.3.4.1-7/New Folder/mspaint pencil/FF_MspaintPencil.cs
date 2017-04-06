 /
function# iid FILTER&f

if(!wintest(f.hwnd "Paint" "MSPaintApp")) ret -2
Q &q
Acc a.Find(f.hwnd "PUSHBUTTON" "Pencil" "class=NetUIHWND" 0x1005)
Q &qq
outq
if(a.State&STATE_SYSTEM_PRESSED) ret iid

ret -2

 ret iid	;; run the macro.
 ret macro	;; run other macro. Here 'macro' is its id or name.
 ret 0		;; don't run any macros.
 ret -1		;; don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		;; don't run this macro. Other macros with the same trigger can run.
