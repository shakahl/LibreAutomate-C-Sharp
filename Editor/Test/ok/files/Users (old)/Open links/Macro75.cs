ClearOutput
act "Maxthon"

str s sf sd data
MSHTML.IHTMLDocument3 doc=htm(win)
MSHTML.IHTMLElement el
foreach el doc.getElementsByTagName("A")
	s=el.getAttribute("href" 0)
	if(!s.begi("http")) continue ;;javascript, anchor, mailto, etc
	 out s
	
	int i=findrx(s "(?<=//)\w"); if(i<0) continue
	sf.from("$desktop$\" s+i)
	sf.findreplace("/" "\")
	sd.getpath(sd "")
	 out sd
	mkdir sd
	if(IntGetFile(s data))
		data.setfile(sf)
	
	

act _hwndqm
