 /Drag_drop_Dialog
function hwnd

str xmlfile="$desktop$\box2.xml"
if(!dir(xmlfile))
	_s="<box />"
	_s.setfile(xmlfile)

IXml x=CreateXml
IXmlNode re=x.Add("boxsettings")

int hctrl idctrl
for(idctrl 100 10000)
	hctrl=id(idctrl hwnd)
	if(!hctrl) continue ;;no more or deleted
	
	int px py cx cy; GetWinXY hctrl px py cx cy hwnd
	str s1(px) s2(py) s3(cx) s4(cy) s5.getwintext(hctrl) s6(idctrl)
	
	IXmlNode e=re.Add("BoxaA")
	e.Add("x" s1)
	e.Add("y" s2)
	e.Add("cx" s3)
	e.Add("cy" s4)
	e.Add("text" s5)
	e.SetAttribute("id" s6)

str s
x.ToString(s)
out s

x.ToFile(xmlfile)
