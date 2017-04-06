out
IXml x=CreateXml

x.FromFile("$my qm$\test.xml")

IXmlNode e

 e=x.RootElement
e=x.Root
x.Delete(e)

x.Add("aaa")
 x.Root.Add("aaa")

str s
x.ToString(s)
out s
