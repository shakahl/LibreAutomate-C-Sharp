dll "qm.exe" #_FindRX_Simple $subject $*pattern [fl] [POINT*results]

str s="one two"
lpstr rx="t(w|y)"
ARRAY(POINT) a.create(2)

 out _FindRX_Simple(s &rx)
out _FindRX_Simple(s &rx 0 &a[0])
out "%i %i" a[0].x a[0].y
out "%i %i" a[1].x a[1].y
