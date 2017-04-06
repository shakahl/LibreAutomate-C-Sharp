 /Create QM Help pdf
function $htmlDir ARRAY(str)&aFiles [flags] ;;flags: 1 create TOC

 Replaces something in css and html files.


str s sf
int i j

 css
sf=F"{htmlDir}\qm-help.css"
s.getfile(sf)
err int noCss=1
if !noCss
	s.replacerx("^(div.expand *\{[^\}]*)\bdisplay: none;" "$1" 1|4|8) ;;remove 'display: none;' from div.expand
	s.replacerx("^pre *\{" "$0[][9]white-space: pre-wrap; word-wrap:break-word;" 1|4|8) ;;wrap lines in <pre>
 out s
s.setfile(sf)

 html
for i 0 aFiles.len
	sf=F"{htmlDir}\{aFiles[i]}"
	s.getfile(sf)
	
	 add topic number to <title> of all html files; later will display in header. Also add the numbers to all <a> of all html files.
	if flags&1
		s.replacerx("<title>" F"$0 {i+1}. " 1|4)
		for j 0 aFiles.len ;;slow, adds ~7 s in Debug config. But simple.
			str& sr=aFiles[j]
			_i=findcr(sr '\')+1
			s.replacerx(F"\bhref=''[^'']*\b\Q{sr+_i}\E(?:\#\w+)?''[^>]*>[^<]+" F"$0 ({j+1})" 1)
	
	 fix wkhtmltopdf bug: does not show gif. Convert to bmp.
	str rx="\bsrc=''(([^'']+.)gif)''"
	ARRAY(str) ag
	if findrx(s rx 0 1|4 ag)
		for j 0 ag.len
			str sip.getpath(sf F"\{ag[1 j]}")
			if sip.searchpath
				int hb=LoadPictureFile(sip)
				SaveBitmap(hb sip.set("bmp" sip.len-3))
		s.replacerx(rx "src=''$2bmp''" 1)
	
	 fix wkhtmltopdf bug: broken links to COM folder. Replace to Com.
	s.replacerx("(\bhref=''\.\./C)OM(/\w+\.html'')" "$1om$2")
	
	 fix wkhtmltopdf bug: links to multipage topics jump to the middle page of the topic. Add #top.
	s.replacerx("(<a[^>]+?\bhref=''[^''\#:?>]+)''" "$1#top''" 1)
	s.replacerx("<body[^>]*>" "$0<a name=''top''></a>" 1|4)
	
	s.setfile(sf)
