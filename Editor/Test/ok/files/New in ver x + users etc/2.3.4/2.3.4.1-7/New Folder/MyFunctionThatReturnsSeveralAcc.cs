 /
function Acc&OK Acc&DefineColor Acc&CloseColor


OK=0; DefineColor=0; CloseColor=0 ;;clear the variables, in case the variables are already set and this function will not set them for some reason

IStringMap m=CreateStringMap

int w=win("Color" "#32770")
Acc a.Find(w "WINDOW" "Color" "class=#32770" 0x1005)

ARRAY(Acc) l
str role name
str data
int x y cx cy i

a.GetChildObjects(l -1)
for i 0 l.len
	l[i].Role(role)
	name=l[i].Name
	l[i].Location(x y cx cy)
	if role="PUSHBUTTON"
		if name="OK"
			m.IntAdd("OK" i)
			OK=l[i]
		if name="Define Custom Colors >>"
			m.IntAdd("Define" i)
			DefineColor=l[i]
		if name="Close"
			m.IntAdd("Close" i)
			CloseColor=l[i]
	data.formata(" %i : %s : %s : %i %i %i %i[]" i role name x y cx cy)
