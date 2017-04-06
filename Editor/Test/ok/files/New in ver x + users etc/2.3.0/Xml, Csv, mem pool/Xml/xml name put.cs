out
str sx=
 <x>
 <i a="aaaa">xxxxx</i>
 <j b="bbbb">yyyyy</j>
 </x>

IXml xml=CreateXml
xml.FromString(sx)
xml.Path("x/i").Name="newname"
xml.Path("x/j/@b").Name="AA"

str s
xml.ToString(s)
out s
