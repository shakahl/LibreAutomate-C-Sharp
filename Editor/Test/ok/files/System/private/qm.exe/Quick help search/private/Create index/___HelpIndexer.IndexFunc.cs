 /
str uf
if(!rget(uf "user folders" "\chi")) rget uf "user folders" "Software\Gindi\qm2\User Nonadmin\chi" HKEY_LOCAL_MACHINE ;;HKLM fbc

ARRAY(int) ai; int i
str s s1
s.from("\System\Functions[]\System\Dialogs\Dialog Functions[]\System\Dialogs\Dialog Control Classes[]" uf)
foreach(s1 s) s1.trim; if(s1.len) EnumQmFolder s1 1 &sub.EnumQmFolderProc &ai; err out "%s (%s)" _error.description s1

m_curfile=0
for i 0 ai.len
	___CHI_FILE& f=m_af_func[]
	f.filename.getmacro(ai[i] 1); m_curfilename=f.filename
	 f.title=f.filename
	 out "--- %s ---" f.filename
	
	 get words of function name
	IndexFuncName(m_mw_func f.filename 20)
	
	 get help section
	s.getmacro(ai[i])
	if(s[0]=1) goto g1 ;;encrypted
	if(s.replacerx("(?:^function\b[^[];]*)(;;[^[]]+)?((?:[]|[ ;\t][^[]]*)*)(?s).*" "$1$2" 12)<0)
		s.replacerx("((?:[]|[ ;\t][^[]]*)*)(?s).*" "$1" 8)
		 out s
	s.replacerx("^ EXAMPLE(?s).*" "" 12)
	 out s
	
	s.trim("[] ;[9]/\")
	if(!s.len)
		 out f.filename
		goto g1
	
	 index first line of description
	if(findrx(s "^[ ;\t]?[A-Z][^[]]+" 0 8 s1)>=0)
		if(s1.beg("Return")) s1+" get"
		IndexText(m_mw_func s1 15 1)
	 else out s
	if(findrx(s "enumerate|store|populate" 0 1)>=0) s+" get"
	if(findrx(s "array|enumerate" 0 1)>=0) s+" list"
	
	 index whole text
	IndexText(m_mw_func s 1 1)
	
	 g1
	m_curfile+1

SaveLoad(1 3)


#sub EnumQmFolderProc
function# iid QMITEM&q level ARRAY(int)&ai

if(q.itype=5)
	sel(q.name) case ["private","obsolete"] ret 1
	ret
	 out q.name

 out q.name
ai[]=iid
