 \
function hDlg

DLG_SEARCHINFILES& d=+DT_GetControls(hDlg)

if(!d.e4fol.len) bee; ret
d.e4fol.expandpath
if(!d.e4fol.end("\")) d.e4fol+"\"
str sp

ARRAY(lpstr) a
if(d.e6fil.len) tok d.e6fil a -1 "|" 1
if(!d.e6fil.len or a.len>1) sp.from(d.e4fol "*")
else sp.from(d.e4fol d.e6fil)

int i hlb(id(10 hDlg)) fl fl2(d.c15Mat=0) fs nfound
if(d.c13Inc=1) fl|4

DATE date1 date2; SYSTEMTIME st1 st2
if(SendDlgItemMessage(hDlg 26 DTM_GETSYSTEMTIME 0 &st1)=GDT_VALID) date1.fromsystemtime(st1)
if(SendDlgItemMessage(hDlg 27 DTM_GETSYSTEMTIME 0 &st2)=GDT_VALID) date2.fromsystemtime(st2)
if(val(d.cb25dat)) fl|16

EnableWindow id(9 hDlg) 0
EnableWindow id(16 hDlg) 1
EnableWindow id(19 hDlg) 0
SendMessage hlb LB_RESETCONTENT 0 0
d.insearch=1; d.stop=0

Dir dd; str sPath s
foreach(dd sp FE_Dir fl date1 date2)
	if(d.stop) break
	if(a.len>1)
		sPath=dd.FileName
		for(i 0 a.len) if(matchw(sPath a[i] 1)) break
		if(i=a.len) continue
	sPath=dd.FileName(1)
	
	if(d.e8tex.len)
		fs=dd.FileSize
		if(fs=0) continue
		else if(fs>100*1024*1024) out "%s skipped because > 100 MB" sPath; continue
		s.getfile(sPath); err out "cannot open %s" sPath; continue
		
		if(d.c29Sea=1) s.findreplace("" "[5]" 32)
		else if(len(s)<s.len) continue ;;binary
		
		if(d.c12Reg=1) if(findrx(s d.e8tex 0 8|fl2)<0) continue
		else if(find(s d.e8tex 0 fl2)<0) continue
	
	LB_Add hlb sPath+d.e4fol.len
	nfound+1

err+ mes _error.description
EnableWindow id(9 hDlg) 1
EnableWindow id(16 hDlg) 0
if(nfound) EnableWindow id(19 hDlg) 1
d.insearch=0
