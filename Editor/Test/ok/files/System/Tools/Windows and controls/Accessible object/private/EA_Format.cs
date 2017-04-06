 /
function# ___EA_CONTROLS&d str&sout [test]

___EA- dA

sout=""
int f isFF=d.c6as="1"

 navigation
if(d.c23Nav=1) d.e45Nav.trim; else d.e45Nav=""
d.e45Nav.S
 wait
if(test) d.e27Wai=0; else if(!d.e27Wai.len and dA.isFirefoxDocAlwaysBusy) d.e27Wai=2; else d.e27Wai.N
 matchindex
if(d.c24Mat=1) d.e51mat.N; else d.e51mat=0
 window
str winVar winFind; d.qmt10.Win(winVar 0 winFind)
 var
if(test) d.e37Var="Acc a"; else d.e37Var="{Acc a}" ;;caller will replace
if winFind.len ;;make win in separate line; will not be if test
	if(d.e27Wai!0) winFind.replacerx("=(.+)[]$" F"=wait({d.e27Wai} WV $1)[]" 4)
	sout=winFind

 flags common to Find and FindFF
if(d.c56in=1) f|128
if(d.c47Err=1 and !test) f|0x1000
if(d.c52in=1) f|0x2000
if(dA.isFirefoxDocAlwaysBusy) f|0x10000

if(isFF) sub.FormatFF(d sout test winVar f~0x2000); ret 1


 role
d.cb1012Rol.CbItem(1)
 name
if(d.c1019Nam=1 and d.e1011Nam.len)
	d.e1011Nam.S
	if(d.c1029Use=1) f|1
	if(d.c1031Reg=1) f|2
else d.e1011Nam="''''"
 other prop
if d.qmg1014x.len
	if(d.c1026Use=1) f|4
	if(d.c1032Reg=1) f|8
 flags
if(d.c1053inv=1) f|16
if(d.c1054use=1) f|32


if !dA.o.GetCheck("acc")
	 format Find()
	sout+F"{d.e37Var}.Find({winVar} {d.cb1012Rol} {d.e1011Nam} {d.qmg1014x.CSV} 0x{f} {d.e27Wai} {d.e51mat} {d.e45Nav})"
	sub_to.Trim sout " 0 0 ''''"
else
	 format acc(). Try to make compatible with QM 2.3.2 and older.
	__strt sClass sVal sXY sWarn; int i f2(f) qm233
	ICsv c._create; c.FromString(d.qmg1014x)
	for i 0 c.RowCount
		_s=c.Cell(i 1)
		sel c.Cell(i 0)
			case "class" sClass=_s
			case "id" if(sClass.len) sClass=F"id={_s} {sClass}"; else sClass=F"id={_s}"
			case "value" sVal=_s
			case "descr" if(!sVal.len) sVal=_s; f2|0x400; else qm233=1
			case "state" sXY=_s; f2|0x800
			case "xy" f2|0x100; if(!sXY.len) sXY=_s; else qm233=1
			case else qm233=1
	
	if qm233
		f2|0x4000; f2~0xC00
		sVal=d.qmg1014x.CSV; sClass=""; sXY="0 0"
		sWarn="This acc is not compatible with QM 2.3.2 and older. Uncheck some properties."
	else
		sVal.S
		if(!sXY.len) sXY="0 0"
		if(f2&0x2000) sWarn="'in web page' will be ignored in QM 2.3.2 and older."
	if(sWarn.len) mes F"{sWarn}[][]You see this warning because 'Use acc' is checked in Options of this dialog." "" "!"
	
	sout+F"{d.e37Var}=acc({d.e1011Nam} {d.cb1012Rol} {winVar} {sClass.S} {sVal} 0x{f2} {sXY} {d.e45Nav} {d.e27Wai} {d.e51mat})"
	sub_to.Trim sout " '''' '''' '''' '''' 0x0 0 0 '''' 0 0"

ret 1


#sub FormatFF
function ___EA_CONTROLS&d str&sout test $winVar f

 text
if(d.c1101Tex=1 and d.e1157Tex.len)
	if(d.c1102Use=1) f|1
	if(d.c1103Reg=1) f|2
else d.e1157Tex=""
 attributes
if d.qmg1155x.len
	if(d.c1105Use=1) f|4
	if(d.c1106Reg=1) f|8

 format FindFF
if(test) sout+"isFF=1[]"
sout+F"{d.e37Var}.FindFF({winVar} {d.e1161Tag.S} {d.e1157Tex.S} {d.qmg1155x.CSV} 0x{f} {d.e27Wai} {d.e51mat} {d.e45Nav})"
sub_to.Trim sout " 0 0 ''''"
