out
IXml x=CreateXml
IXmlNode r e

x.Add("?xml")
r=x.Root
 r.Insert(0 "?xml" "")
 r.Add("!DOCTYPE" "root")

e=x.Add("root")
e=e.Add("sub" "value")
e=e.SetAttribute("a" "attr value")

r.Insert(0 "!DOCTYPE" "root")

x.Add("?instr" "xxxx")
r.Insert(0 "?instr" "xxxx")

str s
x.ToString(s)
out s

 x.Clear
