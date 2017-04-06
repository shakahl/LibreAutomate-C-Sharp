out
str s.getfile("$qm$\test.xml")
 s.findreplace("?????" "ᶚݐᵉᵊᵺﺵﺶﺷﺷ")

IXml x=CreateXml()
x.FromString(s)
out x.RootElement.Child("utf8").Value
 <?xml version="1.0" encoding="windows-1252" ?>
 <?xml version="1.0" encoding="iso-8859-15" ?>
