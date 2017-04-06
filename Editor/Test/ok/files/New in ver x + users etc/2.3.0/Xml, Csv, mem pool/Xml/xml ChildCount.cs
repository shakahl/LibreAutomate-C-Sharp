out
str s=
 <x>
 <a/>
 <b/>
 <a/>
 <c/>
 </x>

IXml xml=CreateXml
IXmlNode n=xml.FromString(s)
out n.ChildCount
