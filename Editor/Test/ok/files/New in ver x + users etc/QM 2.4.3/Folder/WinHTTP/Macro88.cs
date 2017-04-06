out
 str s rh
 out IntGetFileWH("http://www.quickmacros.com/index.html" s rh)
 out s
 out rh

str s
int status=IntGetFileWH("http://www.quickmacros.com/index.html" s)
if(status!=200) end F"faled, status={status}"
out s
