out
 ----
 int w=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Windows Internet Explorer" "IEFrame"))
 Acc a.Find(w "LINK" "Europos[]žemėlapiai" "" 0x3001 10)
 ----
int w=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
Acc a.FindFF(w "A" "Europos<BR>žemėlapiai" "" 0x1001 10)

str tag innerHTML outerHTML pageHTML pageURL pageTitle pageText
a.WebProp(tag innerHTML outerHTML)
a.WebPageProp(pageURL pageTitle pageHTML pageText)
int what=3
sel what
	case 0
	out tag
	out innerHTML
	out outerHTML
	out a.WebAttribute("href")
	
	case 1
	out pageURL
	out pageTitle
	
	case 2
	out pageHTML
	
	case 3
	out pageText
