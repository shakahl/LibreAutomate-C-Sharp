 /Drag_drop_Dialog
function hwnd create ;;create: 1 on WM_INITDIALOG, 0 otherwise

str xmlfile="$desktop$\box2.xml"
if(!dir(xmlfile)) ret

IXml xml=CreateXml
xml.FromFile(xmlfile)
ARRAY(IXmlNode) a
xml.Path("boxsettings/BoxaA" a)

int i
for i 0 a.len
	IXmlNode& n=a[i]
	str s1=n.ChildValue("x")
	str s2=n.ChildValue("y")
	str s3=n.ChildValue("cx")
	str s4=n.ChildValue("cy")
	str s5=n.ChildValue("text")
	str s6=n.AttributeValue("id")
	
	int hctrl idctrl(val(s6))
	if(idctrl<100 or idctrl>60000) continue ;;corrupted xml
	if(create)
		int style=WS_VSCROLL|ES_AUTOVSCROLL|ES_MULTILINE|ES_WANTRETURN|WS_TABSTOP|WS_GROUP|WS_CLIPSIBLINGS
		hctrl=CreateControl(WS_EX_CLIENTEDGE "Edit" 0 style 0 0 0 0 hwnd idctrl)
	else hctrl=id(idctrl hwnd)
	
	MoveWindow hctrl val(s1) val(s2) val(s3) val(s4) hwnd
	s5.setwintext(hctrl)
