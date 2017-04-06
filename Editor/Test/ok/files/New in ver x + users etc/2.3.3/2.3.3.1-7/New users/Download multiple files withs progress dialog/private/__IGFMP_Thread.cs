 \test IntGetMultiProgress
function hDlg

type __IGFMP_CB hDlg sizeAll sizeRead
__IGFMP_CB c.hDlg=hDlg

 get parameters, and copy to local var, because p may become invalid
__IGFMP* p=+DT_GetParam(hDlg)
str url sd ss saveFolder(p.saveFolder); int i hlb(id(3 hDlg)) flags(p.flags~32) inetflags(p.inetflags)
ARRAY(str) a ar
a=p.urlList
ar.create(2 a.len); for(i 0 a.len) ar[0 i]=a[i]

 create folder
if !empty(saveFolder)
	mkdir saveFolder; err mes- _error.description
	if(!saveFolder.end("\")) saveFolder+"\"
	flags|16
else flags~16

 calc total file size
if flags&0x10000
	flags~0x10000
	for i 0 a.len
		if(!IsWindow(hDlg)) ret
		c.sizeAll+IntGetFileSize(a[i] inetflags); err c.sizeAll=0; break
if c.sizeAll
	_s.format("Total %i KB" c.sizeAll/1024); _s.setwintext(id(5 c.hDlg))
	hid- id(6 hDlg); hid- id(5 hDlg)

for i 0 a.len
	 download
	LB_SelectItem(hlb i)
	if flags&16
		sd.gett(a[i] 0 "?"); sd.getfilename(sd 1)
		sd-saveFolder; sd.UniqueFileName
	IntGetFile a[i] sd flags inetflags 0 &__IGFMP_Callback &c
	err
		if(_error.description.end("cancel")) ret
		sel mes(F"Failed to download '{a[i]}'. Error:  {_error.description}[][]Continue?" "" "ARI")
			case 'A' ret
			case 'R' goto download
			case 'I' continue
	ar[1 i].Swap(sd)

if(!IsWindow(hDlg)) ret
p.ar.psa=ar.psa; ar.psa=0

0.5
