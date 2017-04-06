 /
function# ___EH_CONTROLS&d str&sout [test]

sout=""
__strt ind
int f i

 wait
if(test) d.e45Wai=0; else d.e45Wai.N
 window
str winVar winFind; d.qmt10.Win(winVar 0 winFind)
 var
if(test) d.e37Var="Htm e"; else d.e37Var="{Htm e}" ;;caller will replace
if winFind.len ;;make win in separate line; will not be if test
	if(d.e45Wai!0) winFind.replacerx("=(.+)[]$" F"=wait({d.e45Wai} WV $1)[]" 4)
	sout=winFind

 index
if(d.e5Tag.len) if(d.e20Ind.len) ind=d.e20Ind
else if(d.e7Ind.len) ind=d.e7Ind
 name
if(d.c19Nam=1 and d.e11Nam.len)
	if(d.c29Use=1) f|1
	if(d.c31Reg=1) f|2
	i=val(d.cb25Nam); if(i>0) f|i<<8
else d.e11Nam=""
 HTML
if(d.c22Htm=1 and d.e14Htm.len)
	if(d.c26Use=1) f|4
	if(d.c32Reg=1) f|8
else d.e14Htm=""
 frame
if(d.c13Fra=1) d.e15Fra.S("0")
else d.e15Fra="''''"
 flags
if(test) f|0x1000 ;;get hwndies and don't wait
else if(d.c27Err=1) f|32
 ----
 format
sout+F"{d.e37Var}=htm({d.e5Tag.S} {d.e11Nam.S} {d.e14Htm.S} {winVar} {d.e15Fra} {ind.N} 0x{f} {d.e45Wai} {d.e16Nav.N})"
sub_to.Trim sout " '''' 0 '''' 0 0x0 0 0"

ret 1
