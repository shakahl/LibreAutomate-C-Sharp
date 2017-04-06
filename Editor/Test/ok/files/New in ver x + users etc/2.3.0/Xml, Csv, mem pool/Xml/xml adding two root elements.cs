out
IXml x=CreateXml

 x.Root.Add("root").Add("elem" "value").SetAttribute("a" "AAA")
x.Add("root").Add("elem" "value").SetAttribute("a" "AAA")

str s
x.ToString(s)
out s
