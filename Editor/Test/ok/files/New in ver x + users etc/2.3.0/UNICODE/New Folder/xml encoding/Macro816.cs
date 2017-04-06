IXml x=CreateXml
x.Add("?xml").SetAttribute("encoding" "UTF-8")
 x.Add("?xml").SetAttribute("encoding" "windows-1252")
x.Add("elem").Add("e" "aaa ąčę ᶚݐᵉᵊᵺﺵﺶﺷﺷ")

x.ToString(_s)
out _s

 x.ToFile("$my qm$\test.xml")
 
 out _s.getfile("$my qm$\test.xml")
