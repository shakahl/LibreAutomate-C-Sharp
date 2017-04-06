 Moves icons to "32", "24", "8" and "4" subfolders, skipping duplicates and icons that don't have 16px images.
 If enumsubfolders true, renames icons to i1 i2 and so on, to avoid overwriting

 str f="Q:\ico and bmp\ico2"
 int enumsubfolders=0

str f="Q:\ico and bmp\bmp\ico"
int enumsubfolders=1

 ----------------------

out
type NEWHEADER @wReserved @wResType @wResCount 
type ICONDIRENTRY !bWidth !bHeight !bColorCount !bReserved @wPlanes @wBitCount dwBytesInRes dwImageOffset 

str f1.from(f "\32") f2.from(f "\24") f3.from(f "\8") f4.from(f "\4") fd.from(f "\*.ico")
mkdir f1; mkdir f2; mkdir f3; mkdir f4

type STRCMPICO crc ~sp ~sn flags
ARRAY(STRCMPICO) a
Dir d
foreach(d fd FE_Dir iif(enumsubfolders 4 0))
	str sPath=d.FileName(1)
	str s.getfile(sPath) ss
	 out sPath
	
	NEWHEADER* ph=s
	ICONDIRENTRY* pi=ph+6
	int i flags(0) n=ph.wResCount
	for i 0 n
		ICONDIRENTRY& ide=pi[i]
		int offs=ide.dwImageOffset; err offs=0
		if(offs+sizeof(BITMAPINFOHEADER)+100>s.len or offs<sizeof(ICONDIRENTRY))
			 out "invalid: %s" d.FileName
			continue
		BITMAPINFOHEADER* bi=s+ide.dwImageOffset
		if(bi.biWidth!16) continue
		sel bi.biBitCount
			case 32 flags|1
			case 24 flags|2
			case 8 flags|4
			case 4 flags|8
			case else continue
	if(!flags) continue
	
	STRCMPICO& r=a[]
	r.flags=flags
	r.sp=sPath
	r.sn.getfilename(r.sp)
	sel(r.sn 3) case ["$*","am6*"] a.remove(a.len-1); continue
	r.crc=IconCrc(r.sp 0 0)
	err
		out "crc failed: %s" r.sn
		a.remove(a.len-1)

int j k
for i 0 a.len-1
	STRCMPICO& r1=a[i]
	if(!r1.sn.len) continue ;;removed duplicate
	for j i+1 a.len
		STRCMPICO& r2=a[j]
		if(!r2.sn.len) continue ;;removed duplicate
		if(r1.crc=r2.crc)
			 out "%s    %s" r1.sn r2.sn
			
			if(r1.sn.begi("ico")) goto g1 ;;skip r1
			r2.sn.all
	
	out "%i %s" r1.flags r1.sn
	
	if(r1.flags&1) s=f1
	else if(r1.flags&2) s=f2
	else if(r1.flags&4) s=f3
	else s=f4
	
	if(enumsubfolders) s.formata("\i%i.ico" k); k+1
	
	FileCopy r1.sp s
	
	 g1
	continue
