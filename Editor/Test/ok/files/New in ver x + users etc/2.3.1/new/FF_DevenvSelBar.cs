 /
function# iid FILTER&f

 runs the macro if right clicked on selection bar in devenv

if(!wintest(f.hwnd "Microsoft Visual C++" "wndclass_desked_gsk")) ret -2
if(!IsWindowEnabled(f.hwnd)) ret -2
if(!childtest(f.hwnd2 "" "VsTextEditPane")) ret -2
POINT p; xm p f.hwnd2 1
if(p.x>14) ret -2
ret iid

 ret iid	- run the macro.
 ret macro	- run other macro. Here 'macro' is its id or name.
 ret 0		- don't run any macros.
 ret -1		- don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		- don't run this macro. Other macros with the same trigger can run.
