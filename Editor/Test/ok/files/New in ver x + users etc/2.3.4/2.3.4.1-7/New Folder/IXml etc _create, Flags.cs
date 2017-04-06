out
IStringMap a._create
ICsv b._create
IXml c._create

 a.Flags=1
 out a.Flags
 a.AddList("one 1[]two 2")
 out a.Get("Two")


 b.FromString("one, 1[]two, 2")
 out b.Cell(1 1)


c.Flags=1
out c.Flags
c.FromString("<x><k>kk[]kk</k></x>")
 out c.RootElement.FirstChild.Value
outb c.RootElement.FirstChild.Value 5 1

