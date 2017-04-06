 /dialog_QM_Tools
function# str&winVar str&winFind [flags] ;;flags: 1 use _i for handle, return 0 if window field empty

 Gets text from window and control fields. Validates, declares window handle variable etc.
 Returns 1 if successful, 0 if mw_what<1.

 winVar - receives handle or a function that returns handle.
   If mw_what==1 (window), receives "handle". If window field is empty - "win()".
   If mw_what==2 (control), receives "id(...)", "child(...)" or "handle".
 winFind - receives "int handle=win(...)", if need, else will be empty.
 flags:
   1 - use _i for window handle, instead of declaring variable.
   2 - if window field empty, return 0.

 Text format depends on m_flags. Look in macro "__ToolsControl help". Does not depend on Test.

winVar.all; winFind.all
if(m_flags&0x200) flags|1; m_flags~0x200
if(mw_what<1) ret
__strt sw v vd s

sw.getwintext(mw_heW)
 what is sw?
if(!sw.len) if(m_flags&128 or flags&2) ret; else v="win()"
else if(!findrx(sw "^win(\(.*\))?$")) ;;win
else if(m_flags&256=0 and matchw(sw "(?*)")) v.get(sw 1 sw.len-2); v.N ;;handle etc
else if(m_flags&256=0 and sw.VarExists("int")) v=sw ;;handle
else ;;string
	sw.escape(1)
	if(sw[0]='+') sw=F"win('''' ''{sw+1}'')" ;;+class
	else sw=F"win(''{sw}'')" ;;name

if(v.len)
else if(flags&1) winFind=F"_i={sw}"; v="_i"
else if(m_flags&3 and (m_flags&2 or mw_what=1)) v=sw
else winFind=F"{vd.VD(`int w` v)}={sw}"

sel mw_what
	case 1
	winVar=v
	
	case 2
	s.getwintext(mw_heC)
	if(!s.len) s="child()"
	else if(s.replacerx("(.+) \{window\}" F"$1 {v}" 4)>=0)
	else if(val(s 0 _i) and _i=s.len) s=F"id({s} {v})"
	else s.N
	winVar=s

ret 1
