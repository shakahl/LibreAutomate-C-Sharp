function! [str&getLink]

 Returns 1 if this object is of Firefox or Chrome, and ISimpleDOMNode registry key does not exist.

int w=GetAncestor(child(a) 2)
int isFF; if(wintest(w "Firefox" "Mozilla*WindowClass")) isFF=1; else if(wintest(w "Chrome" "Chrome*")) isFF=2
err+
if(!isFF or rget(_s "" "Interface\{1814CEEB-49E2-407F-AF99-FA755A7D2607}\ProxyStubClsid32" HKEY_CLASSES_ROOT)) ret
if &getLink
	if(isFF=2) getLink="This function works with Chrome only if Firefox is installed."
	else getLink="If using portable Firefox, <link ''http://www.quickmacros.com/forum/viewtopic.php?f=1&t=5551''>look here</link>."
ret 1
