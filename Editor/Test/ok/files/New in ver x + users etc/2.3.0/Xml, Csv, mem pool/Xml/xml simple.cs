out

IXml x=CreateXml

x.Add("elem").SetChild("child" "text").SetAttribute("a" "10")

str s
x.ToString(s)
out s
