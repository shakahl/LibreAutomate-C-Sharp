 /
 Allows starting macro when mouse pointer is on certain accessible object.
 To see accessible object properties, use the Find Accessible Object dialog.
 Note: in some windows accessible object functions don't work from a filter function.

function# iid FILTER&f

if(!wintest(f.hwnd "WindowName" "WindowClass")) ret -2 ;;change window name and class
Acc a=acc(mouse)
if(a.a and acctest(a "Name" "ROLE" 0 "classname" "" 0)) ret iid ;;change string values and optionally flags. Use "" for string values that must not be evaluated.
ret -2

 EXAMPLE
 ...
 if(!wintest(f.hwnd "Quick Macros" "QM_Editor")) ret
 Acc a=acc(mouse)
 if(a.a and acctest(a "Options" "PUSHBUTTON" 0 "ToolbarWindow32" "" 0)) ret iid
