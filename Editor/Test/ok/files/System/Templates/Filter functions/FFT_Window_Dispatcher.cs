 /
 Starts different macros when different windows are active.
 Setup:
 Assign trigger and this filter function to one of macros,
 or to this function. Other macros should not have trigger.
 Edit the list of window names. Change macro names. Add/delete case statements.

function# iid FILTER&f

sel wintest(f.hwnd "Window1[]Window2[]Window3..." "" "" 16)
	case 1 ret "Macro1"
	case 2 ret "Macro2"
	case 3 ret "Macro3"
	 ...

ret -2
