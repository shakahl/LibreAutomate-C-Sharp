
function int'buttonpos str'func

str toolbar = "WMB_TOOLBAR"
int RET
str TEXT

int hThread = mac(func "" w1 &RET &TEXT)
wait 0 H hThread

WMB_ReplaceText buttonpos TEXT toolbar	
