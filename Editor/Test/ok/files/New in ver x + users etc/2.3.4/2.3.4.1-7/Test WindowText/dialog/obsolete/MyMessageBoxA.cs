 /
function# hwnd $txt $cap flags

out txt
_s=txt; strrev _s; txt=_s
 ret 0
ret call(fnMessageBoxA hwnd txt cap flags)
