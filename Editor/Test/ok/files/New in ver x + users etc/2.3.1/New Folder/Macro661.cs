out
typelib CSHttpClientLib {12CB8C40-6A6B-11D0-A74C-444553540000} 1.0
CSHttpClientLib.CSHttpClient c._create
c.RequestURL="http://www.quickmacros.com/form2.php"
str s="a=b&c=d"
VARIANT v=s
c.RequestBody=v
c.RequestHeaders=_s.format("Content-Type: application/x-www-form-urlencoded[]Content-Length: %i[]Cache-Control: no-cache[]" s.len)
rep 5
	int t1=timeGetTime
	c.Execute("POST")
	out timeGetTime-t1
	out c.ResponseBody(0)

