out
IXml x=CreateXml

IXmlNode r e e2 e3 a t
e=x.Add("root" "")

a=e.SetAttribute("an" "AV")
 a=e.SetAttribute("an" "")
 e.Value="text"
e.Attribute("an").Value="XXXXX"

 e=e.SetChild("elem" "")
 e=e.SetChild("elem2" "value2")
 e.SetChild("c1" "").SetChild("c2" "b").Path(".." 0).SetChild("c3" "c")

e2=e.SetChild("elem" "value")
e3=e2.SetChild("elem2" "value2")
 e.SetChild("elem" "") ;;error because has child tags
 e2.SetChild("" "")

 t=x.Path("root/elem")
 x.Delete(t)
 x.Delete(x.Path("root/elem"))

str s
x.ToString(s)
out s
