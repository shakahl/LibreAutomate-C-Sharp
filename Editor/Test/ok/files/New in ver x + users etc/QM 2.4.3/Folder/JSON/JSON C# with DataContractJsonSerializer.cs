 This would be the best in C#.
 But not practical in QM, because need to define a class in C# code for each JSON text, like 'public class User' in the example. We cannot create a QM function that would hide all the C# code.
 Tested with .NET 3.5 and 4, on Windows XP, 7 and 10.


str sJson=
 {"IsMember" : true, "Name" : "John", "Age" : 24}

CsScript x
x.SetOptions("references=System.Xml;System.Runtime.Serialization;System.ServiceModel.Web")
x.AddCode("")
IDispatch j=x.CreateObject("Json")
out j.Test(sJson)


#ret
//C# code
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

public class User
{
    public bool IsMember { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class Json
{
public string Test(string JSON)
{
 //read JSON
MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(JSON));
DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(User));
User user = (User)serializer.ReadObject(stream);

bool isMember = user.IsMember;
string name = user.Name;
int age = user.Age;
user.Age=100;

 //write JSON
MemoryStream stream2 = new MemoryStream();
serializer.WriteObject(stream2, user);
stream2.Position = 0;
StreamReader sr = new StreamReader(stream2);
return sr.ReadToEnd();
}
}
