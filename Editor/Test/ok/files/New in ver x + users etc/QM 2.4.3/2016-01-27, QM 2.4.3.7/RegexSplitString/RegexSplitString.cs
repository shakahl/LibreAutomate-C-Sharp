 /
function# $s $rx [ARRAY(str)&as] [ARRAY(POINT)&ap] [findrxFlags]

 Gets parts of string separated by substrings that match a regular expression.
 Returns the number of tokens (array length).

 s - string.
 rx - regular expression (separator).
 as - variable that receives string parts. Can be omitted or 0.
 ap - variable that receives positions of string parts in s. Can be omitted or 0.
 findrxFlags - <help>findrx</help> flags. The function uses findrx to find separators; adds flag 4 (find all).

 REMARKS
 The arrays always will have the number of found separators + 1. If s begins or ends with separator, then the arrays will have an empty element at the beginning or end.

 EXAMPLE
 out
 str s="aa bb  cc[9]dd"
 ARRAY(str) as; ARRAY(POINT) ap
 out RegexSplitString(s "\s+" as ap)
 int i
 for i 0 as.len
	 out F"[{as[i]}]"
	 out F"{ap[i].x} {ap[i].y}"


opt noerrorshere

if(&as) as=0
if(&ap) ap=0

ARRAY(POINT) _a
findrx(s rx 0 findrxFlags|4 _a)

int i nSep(_a.len) nTok(nSep+1) iFrom iTo(len(s))

if(&as) as.create(nTok)
if(&ap) ap.create(nTok)
for i 0 nSep
	POINT& r=_a[0 i]; int _to=r.x
	if(&ap) ap[i].x=iFrom; ap[i].y=_to
	if(&as and _to>iFrom) as[i].get(s iFrom _to-iFrom)
	iFrom=r.y

if(&ap) ap[i].x=iFrom; ap[i].y=iTo
if(&as and iTo>iFrom) as[i].get(s iFrom iTo-iFrom)

ret nTok
