out
IXml x=CreateXml

 x.FromFile("$my qm$\test.xml")

str s.getfile("$my qm$\test.xml")
x.FromString(s)

str ss

 x.ToString(ss)

x.ToFile("$my qm$\test.xml")
ss.getfile("$my qm$\test.xml")

out ss
