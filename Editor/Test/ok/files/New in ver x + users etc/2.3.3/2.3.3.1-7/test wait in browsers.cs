out
lpstr url

 NORMAL
 url="http://www.quickmacros.com"
 url="http://www.wlw.de/sse/MainServlet?sprache=de&land=DE&suchbegriff=Robert&anzeige=firma"
 url="http://gallery.expression.microsoft.com/en-us/TextboxFocusStates?SRC=Home" ;;Silverlight test page
 url="http://www.google.lt/url?sa=t&source=web&cd=7&ved=0CFgQFjAG&url=http%3A%2F%2Fwww.senoji.vpu.lt%2Fbibl%2Felvpu%2F15259.pdf&ei=Iu3bTMrZCYyVOuW8qegI&usg=AFQjCNGlz3ltDrZ6XlH4abiqiAN40yNXRA" ;;SOCIOLOGIJA

 TOO FAST
 url="http://www.yahoo.com"
 url="http://www.mozilla.org/access/windows/at-apis"
 url="http://www.viasat.lt/tv-guide/" ;;viasat.lt TV programa

 TOO SLOW
 url="http://www.download.com/"

 SOMETIMES HUNGS
 url="http://pazintys.draugas.lt/narys.cfm?narys=364028"

 TESTING NOW
 url="http://www.elektronika.lt/"

int w1=FirefoxWait(url)
 int w1=FirefoxWait(url 1)
 FirefoxWait F"-new-window ''{url}''"
 ChromeWait url
 OperaWait url
 OperaWait F"-newwindow ''{url}''"

 act "Firefox"
 key F5
 int w1=FirefoxWait

 act "Chrome"
 key F5
 ChromeWait

 act "Opera"
 key F5
 OperaWait

OnScreenDisplay "loaded" 1 1 1 "" 0 0xff 1

 Acc a=acc(": The XML-based language used by Firefox and Mozilla to develop the UI. Similar to HTML in that it can be combined with CSS and Javascript to make powerful applications. Contains more desktop-style widgets than HTML and follows a box layout model, rather than being text-flow based. In the future more standalone applications will use XUL via " "TEXT" w1 "" "" 0x3891 0x40 0x20000040)
 out a.Name
