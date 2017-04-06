out

 download web page
str url="http://www.quickmacros.com/forum"
IntGetFile url _s
  or
 HtmlDoc hd.InitFromWeb(url)

 find newest cookie from the website
Dir d; long ft _ft; str cookie
foreach(d "$Cookies$\g@quickmacros[*].txt" FE_Dir)
	_ft=d.TimeModified
	if(_ft>ft) ft=_ft; cookie=d.FileName(1)

 results
out cookie
str s.getfile(cookie)
out s
