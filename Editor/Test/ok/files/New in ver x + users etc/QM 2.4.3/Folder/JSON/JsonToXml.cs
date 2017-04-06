 /
function'IXml $JSON [flags] ;;flags: 1 display XML text in QM output

 Converts JSON text to XML and returns IXml object.

 REMARKS
 On Windows XP SP2 and Vista must be installed .NET 3.5 or later. Older OS are not supported.

 EXAMPLE
 out
 str JSON=
  {
     "hello": "world",
     "t": true ,
     "f": false,
     "n": null,
     "i": 123,
     "pi": 3.14,
     "Address": { "City": "New York", "State": "NY" },
     "a": [1, 2, 3, 4]
  }
 IXml x=JsonToXml(JSON 1)
 IXmlNode r=x.RootElement
  get simple
 out r.Child("hello").Value
  get with XPath
 out r.Path("Address/State").Value
  get array
 ARRAY(IXmlNode) a; r.Path("a/*" a)
 int i; for(i 0 a.len) out a[i].Value


opt noerrorshere 1
CsScript x.SetOptions("references=System.Xml;System.Runtime.Serialization;System.ServiceModel.Web")
x.AddCode("")
_s=x.Call("ToXml" JSON)
if(flags&1) out _s
IXml k._create
k.FromString(_s)
ret k


#ret
using System;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;

public class Json
{
static public string ToXml(string JSON)
{
XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(JSON), new System.Xml.XmlDictionaryReaderQuotas());
return XElement.Load(reader).ToString();
}

}
