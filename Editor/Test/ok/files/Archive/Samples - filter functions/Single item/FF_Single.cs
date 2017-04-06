
 Example of filter function and macro in single item.
 When you press Ctrl+F11, this function is called as filter function.
 If QM window is active and editor has focus, this function runs again
 and shows message.

function# iid FILTER&f

if(iid) ;;runs as filter function
	if(f.hwnd2 and wintest(f.hwnd "" "QM_Editor") and GetWinId(f.hwnd2)=2210)
		ret iid
else ;;if iid is 0, runs as macro
	mes "FF_Single now is running as macro"
	