 \
function hDlg

DLG_SEARCHINFILES& d=+DT_GetControls(hDlg)

if(!d.e4fol.len) bee; ret
d.e4fol.expandpath
if(!d.e4fol.end("\")) d.e4fol+"\"

str sf sf2 s
if(!d.e8tex.len) ret
int i hlb=id(10 hDlg)
int fl2(d.c15Mat=0) rx(d.c12Reg=1) nrepl ni os(val(d.c21Onl))

if(d.c29Sea=1 and !rx) rx=1; d.e8tex-"\Q"; d.e8tex+"\E" ;;replacerx works with binary, findreplace does not

if os
	ni=SendMessage(hlb LB_GETSELCOUNT 0 0)
	if(!ni) ret
	ARRAY(int) si.create(ni)
	SendMessage hlb LB_GETSELITEMS ni &si[0]
else ni=SendMessage(hlb LB_GETCOUNT 0 0)

str ss=
 %i replacements will be made in
 
 %s
 
 Yes - replace.
 No - skip this file.
 Cancel - skip all files.
 
 Note: Make sure you have a backup copy of this file.

EnableWindow id(16 hDlg) 1
d.stop=0

for i 0 ni
	if(d.stop) break
	LB_GetItemText hlb iif(os si[i] i) sf
	sf2.from(d.e4fol sf)
	s.getfile(sf2); err mes "Cannot open %s" "" "" sf2; continue
	
	if(rx) nrepl=s.replacerx(d.e8tex d.e20rep fl2|8)
	else nrepl=s.findreplace(d.e8tex d.e20rep fl2)
	
	if(!nrepl) continue
	
	sel(mes(ss "" "YNC" nrepl sf))
		case 'N' continue
		case 'C' break
		
	s.setfile(sf2); err mes "Cannot save %s" "" "" sf2

err+ mes _error.description
EnableWindow id(16 hDlg) 0
