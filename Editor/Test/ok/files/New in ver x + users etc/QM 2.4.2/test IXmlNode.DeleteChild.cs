out
str s=
 <a>
 <b at="kk" bt="mm">
 <c>ggg</c>
 <c><d>mm</d></c>
 text1
 <c1>ggg</c1>
 text2
 <c2>ggg</c2>
 </b>
 </a>


IXml x._create
x.FromString(s)

IXmlNode n=x.Path("a/b")
 n.DeleteChild("c*")
n.DeleteChild("c[d]")
out _hresult

x.ToString(s); out s
