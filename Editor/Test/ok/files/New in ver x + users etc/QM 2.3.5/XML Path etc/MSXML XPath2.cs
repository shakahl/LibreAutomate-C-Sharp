out
lpstr sx=
 <?xml version="1.0" encoding="UTF-8"?>
 <!--comment-->
 <root>
 	<elem1 id="test" po="noooo">ABC</elem1>
 	<elem2>
 		<xyz>XYZ1</xyz>
 	</elem2>
 	<elem2>
 		<xyz>XYZ2</xyz>
 		<xyz>XYZ3</xyz>
	 	<elem4>
	 		<xyz>XYZ4<xyz>XYZ6</xyz></xyz>
	 		<xyz>XYZ5</xyz>
	 	</elem4>
 	</elem2>
 	<elem3 attr="35">GHI</elem3>
 	<elem3 attr="45">JKL</elem3>
 	<elem1 id="test2">ABC2</elem1>
    <!--comment2-->
    <?pi0 at="ttt" ABC?>
    <![CDATA[some code]]>
    just text
 </root>

typelib MSXML2 {F5078F18-C551-11D3-89B9-0000F81FE221} 3.0
MSXML2.DOMDocument doc._create
if(!doc.loadXML(sx)) IXMLDOMParseError e=doc.parseError; str re(e.reason) et(e.srcText); end F"{re}[][9]{et}"
MSXML2.IXMLDOMNode n

 MSXML2.IXMLDOMNodeList k=doc.selectNodes("root/elem3")
 MSXML2.IXMLDOMNodeList k=doc.selectNodes("root/elem2/xyz")
 MSXML2.IXMLDOMNodeList k=doc.selectNodes("root/elem1/@id")
 MSXML2.IXMLDOMNodeList k=doc.selectNodes("root/*")
 MSXML2.IXMLDOMNodeList k=doc.selectNodes("//xyz")
 MSXML2.IXMLDOMNodeList k=doc.selectNodes("//@id")
 MSXML2.IXMLDOMNodeList k=doc.selectNodes("//@id[.='test']")
 MSXML2.IXMLDOMNodeList k=doc.selectNodes("root/elem3[.='JKL']")
MSXML2.IXMLDOMNodeList k=doc.selectNodes("root/elem3[@attr='45']")
 MSXML2.IXMLDOMNodeList k=doc.getElementsByTagName("xyz")
foreach(n k) out n.text

 out doc.

 str s=doc.selectSingleNode("root/elem3").text
 str s=doc.selectSingleNode("root/elem2/elem4/xyz").text
 str s=doc.selectSingleNode("root/*/elem4/xyz").text
 str s=doc.selectSingleNode("root/elem2/../elem3").text ;;error if .. after /
 str s=doc.selectSingleNode("root/elem2/xyz").selectSingleNode("../..").nodeName
 str s=doc.selectSingleNode("root").selectSingleNode("/root/elem2").nodeName
 str s=doc.selectSingleNode("root").selectSingleNode("/*/elem2").nodeName
 str s=doc.selectSingleNode("root").selectSingleNode("/*").nodeName
 str s=doc.selectSingleNode("/*").nodeName
 str s=doc.selectSingleNode("?xml").nodeName ;;error
 out s
 
  more path examples
 
  get attribute
 s=doc.selectSingleNode("root/elem3/@attr").text
 out s
 
  get element with attribute attr=45
 s=doc.selectSingleNode("root/elem3[@attr=45]").text
 out s
 
  it is XPath. You can find information about it on the internet
