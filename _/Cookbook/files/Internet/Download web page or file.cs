/// Use class <see cref="HttpClient"/>.

using System.Net.Http;

/// Download web page. See also recipe <+recipe>Parse HTML<>.

using var c1 = new HttpClient();
var html = c1.GetStringAsync("https://www.example.com").Result;
print.it(html);

/// Download file.

var url = "http://speedtest.ftp.otenet.gr/files/test10Mb.db";
var file = folders.Downloads + pathname.getName(url);

using var c2 = new HttpClient();
using (var s1 = c2.GetStreamAsync(url).Result) {
	using var s2 = File.Create(file);
	s1.CopyTo(s2);
}