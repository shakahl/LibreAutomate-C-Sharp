/exe
out
IXml x=CreateXml

x.FromFile("q:\app\test.xml")

 IXmlNode n=x.Path("*/*")
 IXmlNode n=x.Path("*/full/*/s") ;;not found
 IXmlNode n=x.Path("*/common/*")
 IXmlNode n=x.Path("*/common").Child("*")
 IXmlNode n=x.Path("*/common").Child("c")
 IXmlNode n=x.Path("*/common").Child("*" 2)
IXmlNode n=x.Path("*/common").Child("c" 2)

if(n) XmlOutItem n
