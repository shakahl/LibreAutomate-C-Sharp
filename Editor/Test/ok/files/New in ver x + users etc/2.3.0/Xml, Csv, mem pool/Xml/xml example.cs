 This example loads XML file and adds several new nodes.
out
IXml x=CreateXml

x.FromFile("$my qm$\test.xml") ;;load XML file
err out "Error: %s" x.XmlParsingError; ret ;;error if the file is corrupted

IXmlNode my=x.RootElement.Add("myelement") ;;add one new element as child of the root element
my.Add("mysubelement" "text of my subelement") ;;add its child element
my.Add("mysubelement2" "some text").SetAttribute("a" "my attribute") ;;add another child element and an attribute

str s
x.ToString(s) ;;compose xml string
out s
