function~

 Captures window text and formats as single string.
 Returns the string.

 REMARKS
 This function calls Capture(WT_SORT|WT_SINGLE_COORD_SYSTEM|WT_SPLITMULTILINE|WT_JOIN|WT_NOCLIPTEXT).
 Init() must be called to set target window.

 EXAMPLE
  get text from user-selected window or rectangle
 out
 WindowText x.InitInteractive
 str s=x.CaptureToString
 out s


Capture(WT_SORT|WT_SINGLE_COORD_SYSTEM|WT_SPLITMULTILINE|WT_JOIN|WT_NOCLIPTEXT)
if(!n) ret

str s
int i y nSpaces ncInLine nSpIndent(1000000000) skipInvisible
double fontHeight spaceWidth

 if multiple windows, skip invisible text. Difficult to sort without mixing invisible text with text of other windows. Etc.
for(i 1 n) if(a[i].hwnd!=a[i-1].hwnd and a[i].hwnd!=GetParent(a[i-1].hwnd) and a[i-1].hwnd!=GetParent(a[i].hwnd)) skipInvisible=1; break ;;info: use GetParent for windows such as listview (header is child window)

 calc average font height and space char width
for(i 0 n) fontHeight+a[i].fontHeight
fontHeight/n; spaceWidth=fontHeight*0.33; if(!spaceWidth) spaceWidth=1 ;;optimized for Courier New

for i 0 n
	WTI& t=a[i]
	if(skipInvisible and t.flags&WTI_INVISIBLE) continue
	if s.len and t.rt.top+(t.fontHeight/2)>y ;;add new line; the array is sorted to match this
		ncInLine=0; s+"[]"
		if(t.rt.top>y+(t.fontHeight*0.8)) s+"[]" ;;add empty line
	nSpaces=t.rt.left/spaceWidth-ncInLine; if(nSpaces<=0) nSpaces=1
	if(!ncInLine and nSpaces<nSpIndent) nSpIndent=nSpaces ;;find minimal indent to remove
	_i=s.len
	s.formata("%.*m%s" nSpaces ' ' t.txt)
	BSTR b=s+_i; ncInLine+b.len
	y=t.rt.top+t.fontHeight

if(nSpIndent) s.replacerx(F"^ {{{nSpIndent}}" "" 8)

err+ int e=1

m_captured=0 ;;let next Find() recapture

if(e) end _error
ret s
