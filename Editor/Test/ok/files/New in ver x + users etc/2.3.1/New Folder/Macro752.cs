IXml xml=CreateXml
 xml.Add("x").SetAttributeInt("a" 0xffffffff)
_i=0xffffffff
xml.Add("x").SetAttributeInt("a" _i)

xml.ToString(_s)
out _s

 out xml.RootElement.