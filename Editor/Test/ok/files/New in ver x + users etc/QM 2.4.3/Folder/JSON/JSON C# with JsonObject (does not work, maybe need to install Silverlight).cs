str sJson=
 {"IsMember" : true, "Name" : "John", "Age" : 24}

CsScript x
x.SetOptions("references=System.Json")
x.AddCode("")
IDispatch j=x.CreateObject("Json")
out j.Test(sJson)


#ret
//C# code
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Json; 

namespace Test_App
{     
  class Program   
  {        
     static void Main(string[] args)        
     {          string json = "{" + 
                               "\"Library\" : \"System.Json for .NET\"," +
                               "\"Author\" : { " +
                                            "\"Name\" : \"Bourassi Mohamed\"," +
                                            "\"Age\" : 24," +
                                            "\"Blog\" : \"bourassi_med89@yahoo\"" +
                             "}," + 
                               "\"Date\" : \"31/07/2013\"," +
                               "\"Tags\" : [\"Json\",\"C#\",\".NET\"]" +
                               "}";

            JsonObject jso = JsonObject.Parse(json); 

           // fetch data by key 
            JsonElement element = jso.GetElementByKey("Library"); 
            Console.WriteLine("Library : " + element.Value); 
              Console.WriteLine("---------------------------------------");

              // read data from json array 
              foreach (JsonElement el in jso)     
                 Console.WriteLine(el.Key + " : " + el.Value); 

              Console.ReadKey();
     }   
  }
 }  