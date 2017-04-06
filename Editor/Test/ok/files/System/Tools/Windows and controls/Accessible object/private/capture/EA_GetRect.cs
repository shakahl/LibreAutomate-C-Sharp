 /
function! Acc&a RECT&r [flags] ;;flags: 1 can change to parent

 g1
RECT _r; r=_r
a.Location(r.left r.top r.right r.bottom)
err if(flags&1) a.Navigate("parent"); goto g1; else ret

r.right+r.left; r.bottom+r.top
ret 1

err+
