out
 Htm el=htm("BODY" "" "" win("Microsoft Authenticode technology - Maxthon" "IEFrame") 0 0 0x20)
 str s=el.Text

 str s
 Dir d
 foreach(d "$qm$\htmlhelp\*.html" FE_Dir 0x4)
	 str sPath=d.FileName(1)
	  out sPath
	 str ss.getfile(d.FileName(1));; err ...
	 s+ss

str s.getfile("$qm$\winapiqm.txt")
s.replacerx("\b[0-9a-z]\w*\b" "")

ARRAY(str) as
out tok(s as)

ARRAY(VARIANT) av.create(3 as.len)
int i
for i 0 as.len
	strrev as[i]
	 out as[i]
	av[1 i]=as[i]

int- t_cb
Q &q
SortVariantArray2Dim av 1
Q &qq
outq
out t_cb

for i 0 av.len
	out av[1 i]
