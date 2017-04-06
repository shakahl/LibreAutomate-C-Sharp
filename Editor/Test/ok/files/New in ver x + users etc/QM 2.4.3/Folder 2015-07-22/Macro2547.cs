/exe

 Shows drop-down list with check boxes and gets checked items from CSV.
str csv=
 ,,1
 one,1
 two,2
 three,,1

ICsv x._create; x.FromString(csv)

int R=ShowDropdownList(x)
out F"R=0x{R}"
 x.ToString(_s); out _s

int i
for i 1 x.RowCount
	if(x.CellInt(i 2)&1) out F"item {i-1} is checked"
