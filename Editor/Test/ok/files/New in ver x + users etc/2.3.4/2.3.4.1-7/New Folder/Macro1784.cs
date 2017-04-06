IStringMap m=CreateStringMap

int w=win("Calculator" "SciCalc")
Acc a.Find(w "WINDOW" "Calculator" "class=SciCalc" 0x1005)
 Acc a.Find(w "WINDOW" "Calculator" "class=CalcFrame" 0x1005)

ARRAY(Acc) b
str name
str role
str data
int i
int x y cx cy

a.GetChildObjects(b -1 "" "" "" 16)
for i 0 b.len
	name=b[i].Name
	b[i].Role(role)
	b[i].Location(x y cx cy)

	if role="PUSHBUTTON"
		if x=291 and y=125 and cx=65 and cy=29
			m.IntAdd("Backspace" i) ;;give name "Backspace" to this array element

	data.formata("%i %s %s %i %i %i %i[]" i role name x y cx cy)


 now you can map name to index
if m.IntGet("Backspace" i) ;;if "Backspace" exists
	out i
	b[i].Mouse
