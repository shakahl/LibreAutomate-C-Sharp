Dropdown :sub.Dropdown



#sub Dropdown m
int hhost=TriggerWindow
 outw hhost

str s=
 ,,1
 one,1
 two,2
 three,,1

ICsv x._create; x.FromString(s)

int R=ShowDropdownList(x 0 0 0 hhost)
out F"R=0x{R}"
 x.ToString(_s); out _s

int i
for i 1 x.RowCount
	if(x.CellInt(i 2)&1) out F"item {i-1} is checked"
