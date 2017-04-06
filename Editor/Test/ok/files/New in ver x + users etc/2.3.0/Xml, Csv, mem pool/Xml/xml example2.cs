 This example shows how to create new XML document, add elements, attributes, find and get values.

out
IXml x=CreateXml

x.Add("?xml") ;;add xml declaration (optional)
IXmlNode re=x.Add("rootelem") ;;add root element (XML must have exactly 1 root element)
re.Add("child" "text").SetAttribute("a" "10") ;;add child element with text and 1 attribute

IXmlNode e=re.Add("elem2") ;;add another child element
e.Add("cc" "text of cc") ;;add child of child
e=e.Add("cc2") ;;add another child of child
e.SetAttribute("a" "AAA") ;;add attribute
e.SetAttribute("b" "BBB") ;;add another attribute

str v1=re.ChildValue("child") ;;get value of a child (same as re.Child("child").Value)
e=x.Path("rootelem/elem2/cc2/@b") ;;find a node by path
str v2=e.Value ;;get its value
out v1
out v2

out "-----"
str s
x.ToString(s) ;;compose xml string
out s
