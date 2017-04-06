 /
function hDlg $xfile

 clear listbox
int h=id(9 hDlg)
SendMessage h LB_RESETCONTENT 0 0
 load file and get all <i> nodes
IXml x=CreateXml; x.FromFile(xfile)
ARRAY(IXmlNode) a; int i
x.Path("x/i" a)
 populate listbox
for i 0 a.len
	LB_Add h a[i].AttributeValue("u") i
