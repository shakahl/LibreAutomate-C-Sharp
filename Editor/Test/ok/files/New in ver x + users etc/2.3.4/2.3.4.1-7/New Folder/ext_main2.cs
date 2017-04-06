out
str sf="$qm$\ext-.exe"
cop- "$qm$\ext.exe" sf

str sd.getfile(sf)

IMAGE_DOS_HEADER* dos=+sd
IMAGE_NT_HEADERS* nt=sd+dos.e_lfanew
int nSec=nt.FileHeader.NumberOfSections;
IMAGE_OPTIONAL_HEADER32& o=nt.OptionalHeader;
IMAGE_SECTION_HEADER* psec=(&o+sizeof(IMAGE_OPTIONAL_HEADER32));
int i
for i 0 nSec
	IMAGE_SECTION_HEADER& sec=psec[i]
	lpstr secName=&sec.Name
	out secName
	 out sec.PointerToRawData
	 out sec.SizeOfRawData
	sel secName 3
		case ".rdata" ;;continue
		 case ".data" continue
		 case ".rsrc" continue
		case else continue
	
	 memset sd+sec.PointerToRawData 0 sec.SizeOfRawData
	
	byte* m=sd+sec.PointerToRawData
	int n=sec.SizeOfRawData
	 memset m 0 n*0.80
	
	int offs=n*0.76
	m+offs
	outx sec.PointerToRawData+offs
	 n*0.0005
	n=20
	m+11*20
	outb m n 1
	memset m 0 n

 ret
sd.setfile(sf)
 ret
ext_send
