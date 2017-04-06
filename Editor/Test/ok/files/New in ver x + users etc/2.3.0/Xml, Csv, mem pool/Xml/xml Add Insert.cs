out
IXml x=CreateXml

IXmlNode r e e2 a
a=x.Add("?xml")
a.SetAttribute("encoding" "u")
r=x.Add("root")
r.SetAttribute("r" "r")
e=r.Add("elem" "value")
 e=r.Add("elem")
r.Add("elem2").Add("elem3" "v3")
 a=e.SetAttribute("a" "aaa")
 a=e.SetAttribute("b" "bbb")

 e.SetValue("TEEEEEEXT")
 a.SetValue("BBBBBBBB")
 r.SetChild("elem" "****")
 r.SetChild("elem2" "****")
 e.SetAttribute("b" "$$$$$$$")
e.SetAttribute("c" "BBBBBBBB")

 e=r.Add("x")
 e.Add("y" "value")
 a=e.SetAttribute("b" "xxx")
 e.SetAttribute("c" "$$$$$$$")
 
 e=r.Add("z")
 e=r.Add("za")
 a=e.SetAttribute("zzz" "ZZZ")

r=x.Root
r.Add("!-" "add")
e.Add("!-" "add")
r.Insert(0 "!-" "insert")
e.Insert(0 "!-" "insert")
 r.Insert(e "!-" "mmmm") ;;error because e is not child of r
r.Insert(r.FirstChild.Next "!-" "after second child")
e.Insert(0 "!-")

e=r.Path("root/elem2")
e2=e.Insert(0 "tyu" "ok")
e2=e.Insert(e2 "tyu2" "ok2")

 x.Clear

 XmlOut x 1

str s
x.ToString(s)
out s

 x.FromString(s)
 x.ToString(_s)
 out _s
