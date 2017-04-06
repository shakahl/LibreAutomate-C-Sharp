 Displays QM declarations available in file1 but not in file2.

 lpstr file1="$qm$\old winapiqm.txt"
 lpstr file1="$qm$\winapiqm2.txt"
 lpstr file2="$qm$\winapiqmaz.txt"
lpstr file1="$qm$\winapiqm.txt"
lpstr file2="$qm$\winapiqm3.txt"
int what=1 ;;1 functions, 2 types, 3 constants, 4 interfaces

out
str s ss sss name h
s.getfile(file1)
ss.getfile(file2)

str rx; lpstr rxx
sel what
	case 1 rxx="^dll +\''?\S+\''? +.*?(\w+)(?= |$)"
	case 2 rxx="^type +(\w+)"
	case 3 rxx="^def +(\w+)"
	case 4 rxx="^interface#? +(\w+)"
findrx("" rxx 0 128 rx)

int no nm nd
IStringMap m=CreateStringMap(0)
foreach sss ss ;;h
	if(findrx(sss rx 0 0 name 1)<0) continue
	if(what=1)
		if(find(sss "???")>0)
			sel name 2
				case ["D3*","NtGdi*","TSPI_*","Sms*"]
				case else nd+1;; out sss
	m.Add(name sss); err continue

foreach sss s ;;winapiqm2
	if(findrx(sss rx 0 0 name 1)<0) continue
	no+1
	h=m.Get(name)
	if(!h)
		out sss ;;does not exist in headers
		nm+1
		continue

out "in file1: %i" no
out "in file2: %i" m.Count
out "missing in file2: %i" nm
if(what=1) out "no dll name: %i" nd

