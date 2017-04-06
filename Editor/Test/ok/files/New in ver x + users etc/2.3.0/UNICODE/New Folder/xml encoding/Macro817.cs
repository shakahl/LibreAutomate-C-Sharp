out
str s=
 <?xml version="1.0" encoding="windows-1257" ?>
 <r>text ᶚݐᵉᵊᵺﺵﺶﺷﺷ</r>

IXml x=CreateXml()
x.FromString(s)
out x.RootElement.Value
 <?xml version="1.0" encoding="windows-1252" ?>
 <?xml version="1.0" encoding="iso-8859-15" ?>
