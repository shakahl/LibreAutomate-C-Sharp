 /ParseMapFile2
function $_file [minsize] [sort] [flags] ;;sort: 0 by address, 1 by size.  flags: 1 scroll to top, 2 no crt, 4 only crt, 8 less info

str sa.getfile(_file)
 out sa

type PMF segment a1 a2 size ~name ~lib !isfunc !isstatic !isunknownsize
ARRAY(PMF) a

int isstatic=32
str s
foreach s sa
	ARRAY(str) b
	if(findrx(s "^ ([[:xdigit:]]{4}):([[:xdigit:]]{8})  +(\S+) +([[:xdigit:]]{8}) (.) . (\S+)$" 0 0 b)<0)
		 out s
		if(s=" Static symbols") isstatic='s'
		continue
	 out s
	int seg=strtoul(b[1] 0 16)
	if(seg=0) continue
	PMF& r=a[]
	r.segment=seg
	r.a1=strtoul(b[2] 0 16)
	r.a2=strtoul(b[4] 0 16)-0x00401000
	r.name=b[3]
	r.lib=b[6]
	r.isfunc=b[5][0]
	r.isstatic=isstatic
	r.isunknownsize=32

 sort by .a2
a.sort(0 PMF_sort 0)

 calc sizes
int i
for i 0 a.len-1
	&r=a[i]
	PMF& r2=a[i+1]
	r.size=r2.a2-r.a2
	if(r.segment!=r2.segment) r.isunknownsize='?'

 sort by size
sel sort
	case 1 a.sort(0 PMF_sort 1)

 out, filter
int thesef thesed otherf otherd
for i 0 a.len
	&r=a[i]
	if(flags&6)
		int libc=r.lib.begi("libc") or r.lib.beg("<") or r.lib.endi(".dll")
		if(flags&2) if(libc) continue
		else if(!libc) continue
	if(r.size<minsize)
		if(r.isfunc='f') otherf+r.size; else otherd+r.size
		continue
	if(r.isfunc='f') thesef+r.size; else thesed+r.size
	
	if(flags&8) out "%-4i %c  %-50s    %s" r.size r.isunknownsize r.name r.lib
	else out "seg=%i a1=%-5i a2=%-5i    size=%-4i %c   %c %c    %-50s    %s" r.segment r.a1 r.a2 r.size r.isunknownsize r.isfunc r.isstatic r.name r.lib

out "[]Size of objects of size >= %i:  code=%i  data=%i  all=%i" minsize thesef thesed thesef+thesed
out "Size of objects of size  <% i:  code=%i  data=%i  all=%i" minsize otherf otherd otherf+otherd

if(flags&6) out "[]note: %s" iif(flags&2 "crt objects excluded" "included only crt objects")

if(flags&1)
	0.1
	SendMessage id(2201 _hwndqm) WM_VSCROLL SB_TOP 0
