/// To post data, use class <see cref="internet"/> or <see cref="HttpClient"/>. To work with JSON can be used class <see cref="JsonNode"/>.

using System.Net.Http;
//using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http.Json;

/// Post web form data.

var s1 = internet.httpPost("https://www.w3schools.com/action_page.php", new HttpPostField[] {
	new("name1", "value1"),
	new("name2", "value2"),
});
print.it(s1);

/// Post web form data that includes a file (upload).

var s2 = internet.httpPost("https://www.w3schools.com/action_page.php", new HttpPostField[] {
	new("name1", "value1"),
	new("name2", null, @"Q:\Test\screenshot.png", "image/png"),
});
print.it(s2);

/// Post web form data with <b>HttpClient</b>.

var post2 = new KeyValuePair<string, string>[] {
	new("name1", "value1"),
	new("name2", "value2"),
};
using var c1 = new HttpClient();
var r1 = c1.PostAsync("https://httpbin.org/post", new FormUrlEncodedContent(post2)).Result;
r1.EnsureSuccessStatusCode();
string s3 = r1.Content.ReadAsStringAsync().Result;
print.it(s3);

/// Get values from JSON response.

var j = JsonNode.Parse(s3);
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
string s4 = r.Content.ReadAsStringAsync().Result;
print.it(s4);

//record MyData(int i, string s);

/// Look for more info and examples on the internet.
