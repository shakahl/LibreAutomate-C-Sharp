 /
function Acc&a level lParam
str s=a.Name
rep(level) s-" "
 out s
ARRAY(str)& ar=+lParam
ar[ar.redim(-1)]=s
ret 1
