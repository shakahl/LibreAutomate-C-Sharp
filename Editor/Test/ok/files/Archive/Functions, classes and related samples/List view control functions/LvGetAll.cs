 /
function! hlv ARRAY(str)&a

 Gets text of all items of listview control.
 a will be 2-dim array. Dim 1 - columns. Dim 2 - rows.
 Returns 0 if error or empty. Else returns 1.

 EXAMPLE
 ARRAY(str) a
 LvGetAll hlv a
 str s.From2dimArray(a)
 mes s


int nc=LvGetColumnCount(hlv)
int nr=SendMessage(hlv LVM_GETITEMCOUNT 0 0)
if(!nr or !nc) ret
int r c

a.create(nc nr)
for r 0 nr
	for c 0 nc
		LvGetItemText hlv r c a[c r]

ret 1
