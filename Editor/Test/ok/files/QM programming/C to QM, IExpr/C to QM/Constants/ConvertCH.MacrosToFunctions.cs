 /CtoQM

 Declares C function-style macros as dll functions.
 For example
 #define ListView_SetSelectedColumn(hwnd,iCol) SNDMSG(...)
 declares as
 dll C_macro ListView_SetSelectedColumn hwnd iCol
  ;;SendMessage(hwnd,(0x1000+140),iCol,0)


ARRAY(str) a2
int j good; str s name v

m_mcf.EnumBegin
rep
	if(!m_mcf.EnumNext(name v)) break
	good=1
	
	ExpandMacros(v)
	v.replacerx(m_rx.sp1); v.replacerx(m_rx.sp2) ;;remove spaces
	if(findrx(v "^\(([^\)]*)\)\(?(?:\([A-Z_]\w+\**\))*\:*([A-Z_]\w+\(.*\))$" 0 1 a2)<0)
		if(findrx(v "^\(([^\)]*)\)(.*)$" 0 1 a2)<0) continue
		 out "%s %s" name v
		good=0
	str& par=a2[1]; par.findreplace("," " ")
	str& r=a2[2]
	if(good)
		UnCast(r 1)
		r.replacerx(m_rx.numberL "$1")
		r.replacerx("(?<=[,\(])\((\w+)\)(?=[,\)])" "$1")
		
		str fn.gett(r 0); j=fn.len
		if(!m_mf.Get(fn))
			if(!fn.end("A")) goto g1
			if(!m_mf.Get(fn.fix(fn.len-1))) goto g1
			if(name.end("A")) name.fix(name.len-1); m_mc.Remove(name)
			
		 out "%s%s[]%s[]" name par r
		s.format("dll C_macro %s %s[] ;;%s%s" name par fn r+j)
		 out "%s[]" s
		AddToMap(m_mf name s)
	else
		 g1
		 out "%s(%s) %s" name par r
		s.format("dll C_macro %s %s[] ;;%s" name par r)
		AddToMap(m_mo name s)
		
