out
IXml x=CreateXml(2)
x.FromFile("$my qm$\test.xml")
IXmlNode e

 e=x.Path("?xml/@version")
 e=x.Path("styling")
e=x.Path("styling/default/font")
 e=x.Path("styling/lang")
 e=x.Path("styling/default")

out e.Name
out e.Value
out e.Type
e.UserData=99
out e.UserData

 e.Properties ;;used in XmlOut and XmlOutItem
 e.XmlDoc ;;used in XmlOutItem

 out e.AttributeValue("name")
 out e.Attribute("name").Value
 out e.ChildValue("font")
 out e.Child("font").Value

out e.SetAttribute("aname" "avalue").Value()
