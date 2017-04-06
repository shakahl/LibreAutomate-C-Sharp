function $errstr

if(matchw(errstr "<string>:*"))
	errstr+9
	if(m_iid and isdigit(errstr[0]) and m_flags&16=0) ;;add link
		int i j; str s1 s2
		s1.getmacro(m_iid 1); err
		i=val(errstr 0 j)
		errstr=s2.format("<><open ''%s /L%i''>%i</open>%s" s1 i i errstr+j)

m_ew.addline(errstr)
if(m_flags&16=0) out errstr
