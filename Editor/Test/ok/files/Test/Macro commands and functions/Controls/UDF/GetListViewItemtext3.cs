str s
int hlv=id(1 "Program Manager")
int i n=SendMessage(hlv LVM_GETITEMCOUNT 0 0)
for i 0 n
	GetListViewItemText(hlv i &s)
	out s
