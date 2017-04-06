out
IXml x=CreateXml

x.FromFile("$my qm$\test.xml")

out x.Root.FirstChild.Name

x.Path("styling/default/font").Value="Arial"

str s
x.ToString(s)
out s

