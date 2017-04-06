out
str sx=
 <x>
 <i a="aaaa">xxxxx</i>
 <j b="bbbb">yyyyy</j>
 </x>

IXml xml=CreateXml
xml.FromString(sx)

IXmlNode n=xml.Path("x/i")
str v vv
v="DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA DATA"
v.unicode
 v.getfile("$desktop$\led.txt")
n.ValueBinarySet(v 0)
n.ValueBinaryGet(vv)
vv.ansi
out vv

 xml.RootElement.Add("![").ValueBinarySet(v)

str s
xml.ToString(s)
out s
