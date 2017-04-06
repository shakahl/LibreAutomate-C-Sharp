 /
function str&s i j

for i i j 4
	int* p=s+i
	 outx *p
	 if(*p=0x42490) continue ;;Process32NextW
	if(*p) *p=0x63698
