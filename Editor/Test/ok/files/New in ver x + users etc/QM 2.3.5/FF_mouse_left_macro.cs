 /
function# iid FILTER&f

 This function runs on left mouse button down.
 If global variable g_mouse_left_macro is not 0, it disables the click and instead will run macro "mouse left macro".

 Note: this function must be a filter function. Open Properties, Function properties, and check the checkbox.

int+ g_mouse_left_macro
if(g_mouse_left_macro) ret iid

ret -2 ;;default

 ret iid	;; run the macro.
 ret macro	;; run other macro. Here 'macro' is its id or name.
 ret 0		;; don't run any macros.
 ret -1		;; don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		;; don't run this macro. Other macros with the same trigger can run.
