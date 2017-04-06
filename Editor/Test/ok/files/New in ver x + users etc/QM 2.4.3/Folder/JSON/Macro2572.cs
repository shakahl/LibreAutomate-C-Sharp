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
