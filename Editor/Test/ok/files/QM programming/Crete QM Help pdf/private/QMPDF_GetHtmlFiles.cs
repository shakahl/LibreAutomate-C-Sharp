 /Create QM Help pdf
function $htmlDir str&sHtmlFiles ARRAY(str)&aFiles [flags] ;;flags: 1 create TOC

 Extracts paths of html files from hhc file etc.
 Appends paths to sHtmlFiles, to use with wkhtmltopdf.
 Adds relative file paths to aFiles.


ARRAY(str) a aa acss
int i j sLen=sHtmlFiles.len
str hhc=dir(F"{htmlDir}\*.hhc")
str s.getfile(F"{htmlDir}\{hhc}")
 out s
str rx="<LI>\s*<OBJECT type=''text/sitemap''>\s*<param name=''Name'' value=''([^'']+)''>\s*<param name=''Local'' value=''([^'']+)''>\s*</OBJECT>"

 extract paths of html files from hhc
if(!findrx(s rx 0 1|4 a)) end "failed to parse hhc file"
for i 0 a.len
	str& sr=a[2 i]
	sr.findreplace("/" "\")
	if(find(sHtmlFiles F"\{sr}''" 0 1)>=0) continue ;;skip duplicate TOC items
	sHtmlFiles.formata(" ''%s\%s''" htmlDir sr)
	aFiles[]=sr

 create TOC from hhc. Don't need if wkhtmltopdf creates TOC.
if flags&1
	str toc tocFile(F"{htmlDir}\toc.html") head
	 transform hhc format to html
	if(findrx(s "(?s)<UL>.+</UL>" 0 1 toc)<0) end "failed to parse hhc file"
	toc.replacerx("<LI>\s*<OBJECT type=''text/sitemap''>\s*<param name=''Name'' value=''([^'']+)''>\s*</OBJECT>"    "<LI>$1" 1) ;;items with no link
	toc.replacerx(rx "<LI><a href=''$2#top''>$1</a>" 1) ;;items with link
	 header etc
	head="<html><head>"
	GetFilesInFolder acss htmlDir "*.css"; for(i 0 acss.len) head.formata("<link rel=StyleSheet href=''%s''>" _s.getfilename(acss[i] 1))
	head+"</head><body><h1>Contents</h1>[]"
	toc-head; toc+"[]</body></html>"
	 add topic numbers
	for i 0 aFiles.len
		_s=aFiles[i]; _s.findreplace("\" "/")
		toc.replacerx(F"<LI><a href=''{_s}''>" F"$0 {i+1}. " 1)
	 save
	 out toc
	toc.setfile(tocFile)
	sHtmlFiles.insert(F" ''{tocFile}''" sLen)

 add html files that are not in hhc
GetFilesInFolder aa htmlDir ".+\.html?$" 0x10004
for i 0 aa.len
	_s=F" ''{aa[i]}''"
	if find(sHtmlFiles _s 0 1)<0
		 out _s
		sel(_s 3) case "*\IDP_E_SERVICES.html''" continue ;;unwanted here; Help fixed in next QM.
		sHtmlFiles+_s
		aFiles[]=aa[i]+len(htmlDir)+1

 out sHtmlFiles
 out aFiles
