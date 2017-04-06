 Gets clipboard text, then:
 	Prefixes each line with ///, adds <code><![CDATA[...]]> tags, replaces < to $lt; etc.
    If begins with ///, does vice versa.
 Then pastes.

str s.getclip
if(!s.len) ret
if findrx(s "^\s*///")==0
	s.replacerx("(?m)^[ \t]*/// *")
	s.replacerx("\s*(<example>\s*)?(<code>\s*)?(<!\[CDATA\[\s*)?" "" 4)
	s.replacerx("\s*(\]\]>\s*)?(</code>\s*)?(</example>)?[]$" "[]" 4)
	s.findreplace("&lt;" "<" )
	s.findreplace("&gt;" ">" )
	s.findreplace("&amp;" "&" )
	paste s
	ret

  don't need to escape characters, we'll use CDATA
 s.findreplace("&" "&amp;")
 s.findreplace("<" "&lt;")
 s.findreplace(">" "&gt;")

ARRAY(str) a=s; int i j ntab
s="/// <code><![CDATA[[]"
for i 0 a.len
	str& r=a[i]
	 remove tabs
	j=findcn(r "[9]"); if(j<0) j=r.len
	if(i=0) ntab=j; else if(j>ntab) j=ntab
	 add ///
	s.formata("/// %s[]" r+j)

s+"/// ]]></code>[]"

sel ShowMenu("1 With <example>[]2 Without[]0 Cancel")
	case 1 s-"/// <example>[]"; s+"/// </example>[]"
	case 0 ret

paste s; err OnScreenDisplay "Failed to paste"
