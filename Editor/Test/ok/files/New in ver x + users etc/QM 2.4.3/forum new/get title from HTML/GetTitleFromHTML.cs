 /
function! $html str&title [flags] ;;flags: 1 html is file, 2 fast but unreliable

 Extracts title from <title> tag in HTML.
 Returns 1 if title found, 0 if not.

 html - HTML text. If flag 1 - full path of a local HTML file.
 title - variable that receives title text.
 flags:
   1 - html is HTML file path.
   2 - to extract title, use regular expression. Almost 1000 times faster, but unreliable, eg can extract <title> from comments or scripts. Without this flag uses HtmlDoc class to parse the HTML.

 EXAMPLE
 str title
 if(GetTitleFromHTML("Q:\Test\test.htm" title 1)) out title; else out "<NO TITLE>"


opt noerrorshere 1
if(flags&1) html=_s.getfile(html)

title.all
if flags&2
	if(findrx(html "(?si)<title.*?>(.+?)</title>" 0 0 title 1)<0) ret
	title.trim; title.replacerx("\s+" " ")
	ret 1

HtmlDoc d.InitFromText(html)
title=d.d.title
ret title.len!0
