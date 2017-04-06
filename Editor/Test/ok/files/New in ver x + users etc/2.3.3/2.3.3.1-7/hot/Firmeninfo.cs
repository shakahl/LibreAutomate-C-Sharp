out

str url="http://www.wlw.de/sse/MainServlet?sprache=de&land=DE&suchbegriff=Robert&anzeige=firma"

 download page with results
 g1
HtmlDoc hd.InitFromWeb(url)
str html=hd.GetHtml
 out html; ret

 get all Firmeninfo URLs
ARRAY(str) a
if(!findrx(html "(?s)<A .+? href=''([^''<>]+)''>Firmeninfos</A>" 0 1|4 a 1)) ShowText "results failed" hd.GetText; ret

 for each link ...
str email kontakt
int i
for i 0 a.len
	url.from("http://www.wlw.de" a[1 i])
	url.findreplace("&amp;" "&")
	 out url; break
	hd.InitFromWeb(url) ;;download Firmeninfo page
	str s=hd.GetText
	 out s; break
	if(findrx(s "^E-Mail:(.+)[]" 0 8 email 1)<0) email=""
	if(findrx(s "^Erstkontakt:(.+)[]" 0 8 kontakt 1)<0) kontakt=""
	
	if(empty(email) and empty(kontakt)) ShowText "info failed" s; ret
	
	out F"email=''{email}'', kontakt=''{kontakt}''"

 is there next page of results? Find the -> arrow image and extract its url.
if(findrx(html "<A href=''(.+?)''.*?><IMG title=''vor zur nächsten Seite''" 0 1 url 1)<0) ret
url.findreplace("&amp;" "&")
url-"http://www.wlw.de"
 out url
goto g1