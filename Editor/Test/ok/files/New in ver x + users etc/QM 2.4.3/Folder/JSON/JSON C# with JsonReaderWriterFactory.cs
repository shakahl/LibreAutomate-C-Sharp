 This is is slightly slower, but much better for use in QM.
 Tested with .NET 3.5 and 4, on Windows XP, 7 and 10.


str sJson=
 { "Name": "Jon Smith", "Address": { "City": "New York", "State": "NY" }, "Age": 42 }

CsScript x
x.SetOptions("references=System.Xml;System.Runtime.Serialization;System.ServiceModel.Web")
x.AddCode("")
IDispatch j=x.CreateObject("Json")
out j.Test(sJson)


#ret
//C# code
using System;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Xml.XPath;

public class Json
{
public string Test(string JSON)
{
var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(JSON), new System.Xml.XmlDictionaryReaderQuotas());

var root = XElement.Load(jsonReader);
Console.WriteLine(root.XPathSelectElement("Name").Value);
Console.WriteLine(root.XPathSelectElement("Address/State").Value);

return JSON;
}
}
