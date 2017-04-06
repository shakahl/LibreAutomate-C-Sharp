 /
 Starts different macros when you press Shift+F10 in different windows.

function# iid FILTER&f

sel wintest(f.hwnd "Notepad[]Calculator[]Quick Macros" "" "" 16)
	case 0 ret
	case 1 ret "Notepad Window"
	case 2 ret "Calculator Window"
	case 3 ret "QM Window"
	