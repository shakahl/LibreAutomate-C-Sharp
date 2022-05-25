using System.Net.Http;
//using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http.Json;

print.clear();

internet.http.DefaultRequestHeaders.Add("User-Agent", "Script/1.0");
internet.http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

var s1 = internet.http.Get("https://httpbin.org/anything", headers: new[] { "Accept: application/json, text/json", "Cookie: mmm=nnn; xxx=yyy;" }).Text();
print.it(s1);
