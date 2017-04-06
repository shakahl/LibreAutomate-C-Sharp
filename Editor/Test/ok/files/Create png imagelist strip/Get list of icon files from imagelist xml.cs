out
str files
IXml x._create
x.FromFile("$qm$\il_qm.xml")
ARRAY(IXmlNode) a; int i
x.Path("imagelib/files/i" a)
for i 0 a.len
	files.addline(a[i].Value)
out files
files.setclip
