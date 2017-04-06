 /
function hmenu $regkey $regname firstitemid

MenuDeleteItems hmenu
str s; int i
rget s regname regkey
ARRAY(str) a=s
for i 0 a.len
	AppendMenuW hmenu 0 firstitemid+i @a[a.len-i-1]