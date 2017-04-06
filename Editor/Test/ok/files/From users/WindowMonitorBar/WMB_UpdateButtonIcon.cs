
function int'buttonpos str'func [str'trueico] [str'falseico]

str toolbar = "WMB_TOOLBAR"
int RET
str+ icon;

// Call Check function and wait for it to finish
int hThread = mac(func "" w1 &RET)
wait 0 H hThread

// Change icons
if(trueico && falseico)
	
	if(RET) icon = trueico
	else icon = falseico
	PostMessage win(toolbar "QM_TOOLBAR") WM_APP buttonpos 0