out
lpstr sx=
 <root>
 	<elem1>ABC</elem1>
 	<elem2>
 		<xyz>XYZ</xyz>
 	</elem2>
 	<elem3 attr="35">GHI</elem3>
 	<elem3 attr="45">JKL</elem3>
 </root>

typelib MSXML2 {F5078F18-C551-11D3-89B9-0000F81FE221} 3.0
MSXML2.DOMDocument doc._create
doc.loadXML(sx)

 you can navigate to an XML node using doc.function.function...
str s=doc.documentElement.firstChild.nextSibling.firstChild.nodeName
out s

 ...or by path
s=doc.selectSingleNode("root/elem3").text
out s

 more path examples

 get attribute
s=doc.selectSingleNode("root/elem3/@attr").text
out s

 get element with attribute attr=45
s=doc.selectSingleNode("root/elem3[@attr=45]").text
out s

 it is XPath. You can find information about it on the internet
