out
IXml x=CreateXml

x.FromFile("$my qm$\test.xml")

IXmlNode e
 e=x.Path("styling/full")
e=x.Path("styling/common")
 e=x.Path("styling/full/@b")
 e=x.Path("styling/full/@a")
 e=x.Path("styling/lang/styles")

 e=e.Attribute("b")
 e=e.Attribute("*")
 e=e.Child("c")
 e=e.FirstChild()
 e=e.LastChild()
 e=e.Parent()
 e=e.Prev()
 e=e.Next()

if(e) out "'%s' '%s'[]---" e.Name e.Value
