 /
function# FFNode&x level MYFFTYPE&y FFNODEINFO&ni
str s=x.HTML
if(find(s y.s)>=0) y.a[]=x
ret 1
