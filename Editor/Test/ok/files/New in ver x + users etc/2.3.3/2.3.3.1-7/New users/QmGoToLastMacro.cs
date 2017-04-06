 Shows list of recent macros, and opens selected macro.


act _hwndqm
 g1
Acc a=acc("Recent*" "OUTLINEITEM" _hwndqm "id=2214 SysTreeView32" "0" 0x1005)
err
	men 33304 _hwndqm ;;View Active Items
	0.5
	goto g1

str s ss
rep
	a.Navigate("next"); err break
	s=a.Name
	if(s!="QmGoToLastMacro") ss.addline(s)

int i=list(ss "Click to open." "QM - Recent Macros")
if(!i) ret
s.getl(ss i-1)
mac+ s

err+
