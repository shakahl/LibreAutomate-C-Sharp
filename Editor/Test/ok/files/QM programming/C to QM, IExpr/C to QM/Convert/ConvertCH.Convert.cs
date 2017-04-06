 /CtoQM

 Main function of converting declarations (except #define).

 out m_s
int hr
str s s0
ARRAY(str) a
m_pack=8

foreach s m_s
	s0=s
	
	 g1
	hr=0
	if(!s.len) continue
	
	sel s 2
		case "$*" ;;options for converter
		sel s 2
			case "$begin crt;" m_crt=1
			case "$end crt;" m_crt=0
			case "$file''*"
			m_file.get(s 6 s.len-8)
			 out "---- %s ----" m_file
			case "$pack*" hr=Pack(s+6)
			 case "$pack*" out s; hr=Pack(s+6); out m_pack; if(m_ps.len) outb m_ps m_ps.len
			case else out s ;;any comment
		
		case "class *"
		if(m_needclasses) s.replace("_struct_from_class" 0 5); goto g1
		if(s.beg("class __declspec(uuid(")) goto other ;;clsid
		
		case "template<*>*" ;;template
		
		case "*(*);" ;;function
		if(findrx(s "^_?_?inline ")=0) continue ;;inline function declaration
		if(findrx(s "^\w+ \w+=")=0) goto other
		hr=_function(s)
		
		case "*(*){*}*" ;;inline function definition, or class containing inline functions
		 if(!s.end("}")) out s
		if(m_needclasses and RemoveClassFunctionBodies(s)) goto g1
		
		case "*{*}*;" ;;struct, union, enum
		int td=s.beg("typedef "); if(td) s.get(s 8)
		hr=_type(s td); if(hr<0) goto other
		
		case "typedef *"
		 if(findrx(s "\b(\w+)\b \b\1\b")>0) out s
		if(findrx(s "^typedef (?:struct |union |enum |_struct_from_class )?(?:__declspec\(uuid\(''(.+?)''\)\))?(\w+) ?([\w,\*&]+);" 0 0 a))
			 out s ;;warning: skips typedefs like typedef LUID_AND_ATTRIBUTES LUID_AND_ATTRIBUTES_ARRAY[1];
			continue
		hr=_typedef(s a[2] a[3] 0)
		 if(a[1].len) out s ;;these guids will be added later (DEFINE_GUID)
		
		case ["using*"]
		
		case else
		if(s[0]>126 or s[0]<33) continue ;;some files have [26] character at the end
		 other
		hr=_other(s)
	
	if(!hr) continue
	
	 out "%i %s" hr s0
	out "Warning in %s: cannot convert: error %i:[][9]%s" _s.getfilename(m_file 1) hr s0

if(m_ps.len or m_pack!8) end "Invalid packing" 1
