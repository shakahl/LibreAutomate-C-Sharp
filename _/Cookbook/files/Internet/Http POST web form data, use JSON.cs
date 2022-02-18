/// To post data and use JSON, use classes <see cref="HttpClient"/> and <see cref="JsonNode"/>.

using System.Net.Http;
//using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http.Json;

/// Post web form data.

var a = new KeyValuePair<string, string>[] {
	new("name1", "value1"),
	new("name2", "value2"),
};
using var c1 = new HttpClient();
var r1 = c1.PostAsync("https://httpbin.org/post", new FormUrlEncodedContent(a)).Result;
r1.EnsureSuccessStatusCode();
string s1 = r1.Content.ReadAsStringAsync().Result;
print.it(s1);

/// Get values from JSON response.

var j = JsonNode.Parse(s1);
var p1 = (string)j["origin"];
print.it(p1);
string p2 = (string)j["headers"]["Content-Type"];
print.it(p2);

/// Post an object as JSON.

//var v = new MyData(10, "test"); //create an object of some type
var v = new { //or create an object of anonymous type
	one = 10,
	two = "test",
	three = new string[] { "a", "b" },
};

using var c2 = new HttpClient();
var r = c2.PostAsJsonAsync("https://httpbin.org/post", v).Result;
r.EnsureSuccessStatusCode();
string s = r.Content.ReadAsStringAsync().Result;
print.it(s);

//record MyData(int i, string s);

/// Look for more info and examples on the internet.
