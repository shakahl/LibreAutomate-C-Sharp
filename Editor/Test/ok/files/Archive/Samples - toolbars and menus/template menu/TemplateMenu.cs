 \
function# $template [$tempitemname] [sync]

 Shows popup menu created at run time.
 Replaces every {menuname} in template text with submenu created
   from a menu (QM item) whose name is menuname.
 {menuname} must be at the beginning of a line. It can be preceded
   by tabs. If it is followed by more text in the same line, the
   menu name is not inserted.
 If menu menuname does not exist, removes the line.

 template - menu text (if multiline), or existing menu name (if single line).
 tempitemname - the name of the temporary menu. Default: "temp_menu".
 sync - if nonzero, waits and returns 1-based index of selected line.


str s sn st sr si; int i
ARRAY(CHARRANGE) a

if(findc(template 10)<0) s.getmacro(template)
else s=template

if(findrx(s "^\t*\{(.+?)\}([^[]]*)$" 0 12 a))
	for i a.len-1 -1 -1
		CHARRANGE r(a[0 i]) rn(a[1 i]) ri(a[2 i])
		sn.get(s rn.cpMin rn.cpMax-rn.cpMin)
		st.getmacro(sn); err s.remove(r.cpMin r.cpMax-r.cpMin+2); continue
		st.rtrim("[]")
		if(ri.cpMax>ri.cpMin) si.get(s ri.cpMin ri.cpMax-ri.cpMin); sn=""; else si=""
		sr.format(">%s%s[]%s[]<" sn si st)
		s.replace(sr r.cpMin r.cpMax-r.cpMin)

 out s
ret DynamicMenu(s tempitemname sync)
err+ end _error
