out
str s=
 <a>
 	<b/>
 	<c></c>
 	<d/>
 	<e/>
 </a>

IXml x=CreateXml
x.FromString(s)

IXmlNode a b c d e n
a=x.RootElement
b=a.Child("b")
c=a.Child("c")
d=a.Child("d")
e=a.Child("e")
out "%i %i %i %i %i" a b c d e


n=c.Add("n" "text")
 n=a.Insert(e "n" "text")
 n=c.Insert(0 "n" "text")
 x.Delete(d)

d.Move(a b)
 d.Move(a c)
 d.Move(a d)
 d.Move(a e)
 d.Move(c n)
 d.Move(c 0)
 d.Move(0 0)
 d.Move(n 0)
 a.Move(0 0)

 XmlOut x 1

str ss
x.ToString(ss)
out ss
