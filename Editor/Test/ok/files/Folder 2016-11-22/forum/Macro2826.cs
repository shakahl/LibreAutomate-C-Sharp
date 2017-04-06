out
str new
str xml =
 <?xml version="1.0" encoding="utf-8" ?>
 <resources>
	 <string name="a">abcde</string>
	 <string name="c">aabbcc</string>
	 <string name="d">
		 <a href="http://google.com">google</a>
	 </string>
	 <string name="b_0">bbaacc</string>
 </resources>

IXml x._create
x.FromString(xml)
ARRAY(IXmlNode) a
x.Path("resources" a)

ARRAY(IXmlNode) b
a[0].GetAll(1 b)
for(_i 0 b.len)
	XMLNODE p; b[_i].Properties(p)
	out "%.*m%i %s = %s" p.level 9 p.xtype p.name p.value
