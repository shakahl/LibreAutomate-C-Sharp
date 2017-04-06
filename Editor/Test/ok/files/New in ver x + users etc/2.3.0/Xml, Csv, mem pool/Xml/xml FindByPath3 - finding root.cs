out
 out _s.getfile("$my qm$\test.xml"); ret

IXml x=CreateXml

x.FromFile("$my qm$\test.xml")

IXmlNode e
ARRAY(IXmlNode) a
int i

e=x.Path("styling" a)
 e=x.Path("elem")
out "'%s' '%s'[]---" e.Name e.Value
e=e.Path(".." a)

out "--------"
if(e) out "'%s' '%s'[]---" e.Name e.Value
for i 0 a.len
	e=a[i]
	out "'%s' '%s'" e.Name e.Value

 str s
 x.ToString(s)
 out s
