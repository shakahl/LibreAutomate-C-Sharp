 /
function# ___REPLACEALL&r hwnd [justtest]

if(r.Edit4=r.Edit3 or r.Edit3.len=0) ret

int+ ___replaceoptions=(r.Button6="1")|((r.Button5="1")<<1)|((r.Button9="1")<<2)|0x100

opt waitmsg 1
act hwnd
0.1
if(r.Button8!="1") key CH CSE Ca; 0.2 ;;select all

int n=Replace(r.Edit3 r.Edit4 0 r.Button5="1" r.Button6!"1" r.Button9="1" justtest)
if(n and r.Button8!="1") 0.1; key CH
ret n
