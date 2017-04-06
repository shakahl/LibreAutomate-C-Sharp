 /GFA test
 /GetFuncArgsFromDoc
function# VsHelp.DExploreAppObj&dex $fname str&args crt

str sfn=fname
if(crt) sfn+" function"
 dex.DisplayTopicFromKeyword(sfn); err ret

int i; str s ss rx
int hwnd=win("" "wndclass_desked_gsk")
int hin=child("" "hx_winclass_vlist" hwnd)
int hir=child("Index Results*" "GenericPane" hwnd 0x1)
int hcb=id(1001 child(2454 "" "ComboBox" hwnd 0x5))
dex.Index
act hcb
s.setwintext(hcb); s=fname; s.setsel(0 hcb)
key Y
Acc ai=acc("Help Index Control" "LIST" hin "hx_winclass_vlist" "" 0x1001)
for i 0 20
	ai.Selection
	s=ai.Name
	ss.gett(s 0)
	if(ss~fname) break
	0.01
if(i=20) ret

 g1
rx.format("Index Results for %s - * topics found" s)
for(i 0 50) s.getwintext(hir); if(matchw(s rx)) break; else 0.01
if(i=50) goto g2

Acc a=acc("" "LIST" hir "SysListView32" "" 0x1000)
for a.elem 1 1000000
	ss=a.Name; err break
	if(!s.end(" 1 topics found"))
		rx.format("^%s(?i)(?! Method)" fname)
		if(findrx(ss rx)<0) continue
		a.Mouse(4)

	 0.5
	Htm el=htm("BODY" "" "" hwnd 0 0 0x20 10)
	s=el.Text
	 out s
	rx.format("(?s)\b%s\s*\((.+?)\);" fname)
	int found=findrx(s rx 0 0 ss 1)>=0
	if(!found and crt)
		rx.format("(?s)\b_%s\s*\((.+?)\);" fname) ;;try with _ prefix
		found=findrx(s rx 0 0 ss 1)>=0
	if(!found) continue
	
	ss.replacerx("//.*$" "" 8)
	ss.replacerx("[[][9]]+" " ")
	ss.trim
	ss.replacerx(" +(?=\W)"); ss.replacerx("(?<=\W) +") ;;remove spaces
	
	ss.replacerx("\(.*\b(\w+) *\) *\(.+?\)" "$1") ;;callback function
	ss.replacerx("\b(\w+) *\(.+?\)" "$1") ;;callback function
	ss.replacerx("\[,(.+?)\]" ",$1") ;;optional
	
	 if(ss.end("...")) args.format("%s ;;%s" fname ss); ret 1
	if(ss.end("...")) ss.rtrim("., ") ;;now qm supports ... . However I did not test this macro after disabling this line. Maybe needs to change something more.
	if(!ss.len) args=fname; ret 1
	
	if(!ss.replacerx(".+?\b(\w+) *(,|$)" " $1")) ret
	
	args.from(fname ss)
	ret 1
 g2
ai.Selection
ai.elem+1
s=ai.Name
ss.gett(s 0)
if(ss~fname=0) ret
ai.Mouse(4)
key DV
goto g1


