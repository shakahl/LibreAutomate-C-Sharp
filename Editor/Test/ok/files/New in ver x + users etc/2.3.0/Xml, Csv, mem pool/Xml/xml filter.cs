out
str s=
 <x>
 <i name="yoy">1</i>
 <i name="put">2</i>
 <i name="put">3</i>
 <i name="puts">32</i>
 <ij name="puts">33</ij>
 <i id="10">4</i>
 <i id="0xA">5</i>
 <i id="11">6</i>
 <i name="">7</i>
 <i><c>1t</c></i>
 <i><c>2t</c></i>
 <i><c></c></i>
 <i id='12'><c>3t</c></i>
 <i id='15'><c k='8/8'>4t</c></i>
 <![CDATA[CD1]]>
 <![CDATA[CD2]]>
 </x>

IXml x=CreateXml
x.FromString(s)
ARRAY(IXmlNode) a

x.Path("x/i[@name='Put']" &a)
 x.Path("x/i[@name!='Put']" &a)

 x.Path("x/i[@name*='pu*']" &a)
 x.Path("x/i[@name!*='pu*']" &a)

 x.Path("x/i[@name*='*']" &a)
 x.Path("x/i[@name!*='*']" &a)

 x.Path("x/i[@id#='10']" &a)
 x.Path("x/i[@id!#='10']" &a)

 x.Path("x/i[*='3*']" &a)
 x.Path("x/*[*='3*']" &a)
 x.Path("x/i*[*='3*']" &a)

 x.Path("x/i[c='1t']" &a)
 x.Path("x/i[c!='1t']" &a)

 x.Path("x/i[c*='*']" &a)

 x.Path("x/i[c#='1t']" &a)

 x.Path("x/i[@id#='12']/c" &a)

 x.Path("x/i[@id#='12/']" &a)
 x.Path("x/i[@id#='15']/c[@k='8/8']" &a)

 x.Path("x/i/c[@k='8/8']" &a) ;;fails because searches only in the first i

 x.Path("x/![[='CD2']" &a)
 x.Path("x/![[*='CD*']" &a)

out "---"
 out a.len
int i
for i 0 a.len
	s=a[i].Value
	if(!s.len) s=a[i].ChildValue("*")
	out s
out "---"
