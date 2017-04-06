 Opens selected links.
 Works with IE, does not work with Mozilla (can't get base url).

str s su
int i j

 get selection in HTML format
s.getsel(0 "HTML Format")
if(!s.len) mes- "Please at first select one or more links"
 out s

 find base url for relative links
if(findrx(s "^(SourceURL:)(.+/).*$" 0 9 su 2)<0) ret
j=find(su "/cgi-bin/" 0 1)+1; if(j) su.fix(j)
 out su

 extract links
ARRAY(str) a
lpstr linkrx="(?si)<A\s+[^>]*href\s*=\s*[''']?([^''' >]+)[''' >]"
if(!findrx(s linkrx 0 4 a)) ret

 filter unuseful links and scripts, make full urls, open
for i 0 a.len
	 out a[0 i]
	s=a[1 i]
	s.replacerx("\s+")
	if(s[0]='/') s.get(s 1)
	 out s
	j=findc(s '#')
	if(j=0) continue ;;within this page
	else if(j>0) s.fix(j)
	if(findrx(s "^\w+:")<0) s-su ;;relative url
	else if(!s.begi("http")) continue ;;mailto, javascript, etc
	 out s
	run s
