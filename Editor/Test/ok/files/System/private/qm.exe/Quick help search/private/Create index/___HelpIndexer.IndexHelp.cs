 /
OnScreenDisplay "Creating QM help index..." -1 0 0 0 0 0 8 "osd_indexer"

 decompile chm

__TempFile dirhh.Init(0 "hh")
del- dirhh; err
mkdir dirhh
cop "$qm$\qm2help.chm" dirhh
run "hh.exe" "-decompile . qm2help.chm" "" dirhh 0x400

 index html files

str sp.from(dirhh "\*.html") s ss sss
Dir d; int i
m_curfile=0
foreach(d sp FE_Dir 0xC)
	___CHI_FILE& f=m_af_help[]
	f.filename=d.RelativePath; m_curfilename=f.filename
	 sel(f.filename 3) case ["*IDP_ACC.*","*CLASS*"] case else continue ;;debug
	 out "---------- %s -----------" f.filename
	s.getfile(d.FullPath)
	 get title
	sub.GetTagText(s "title" ss)
	sub.GetTagText(s "h1" f.title)
	if(ss.len and !(ss~f.title)) f.title.from(ss ": " f.title)
	 get words
	 at first index most important places to give higher scores
	 title
	IndexText(m_mw_help f.title 30)
	 h2
	ss.fix(0)
	for(i 0 s.len) i=sub.GetTagText(s "h2" sss i); if(i<0) break; else ss.addline(sss)
	if(ss.len) IndexText(m_mw_help ss 10 1)
	 contents
	IndexText(m_mw_help s 1 1)
	if(f.filename.begi("str\")) IndexText(m_mw_help "string str" 20); IndexText(m_mw_help "variable text" 5)
	
	m_curfile+1
	
	sel(f.filename 2) case ["*IDP_QMDLL.*"] sub.MultiFunc(f.filename s)

 save

SaveLoad(1 1)

OsdHide "osd_indexer"


#sub GetTagText
function# $s $tag str&ss [from]

int i=findrx(s F"(?s)<{tag}.*?>(.+?)</{tag}>" from 1 ss 1)
if(i<0) ret i
ss.replacerx("(?s)<.+?>")
ss.replacerx("\s+" " ")
ss.findreplace("&nbsp;" " ")
ss.findreplace("&quot;" "''")
ss.findreplace("&gt;" ">")
ss.findreplace("&lt;" "<")
ss.trim
 out ss
ret i


#sub MultiFunc c
function $filename $text

 Adds HTML help topic as multiple topics.
 Gets all parts that begin with <hr>, end with <hr> and contain <a name="FuncName">.

int i j k
str rx
FINDRX fr
ARRAY(str) a

i=find(text "<hr>"); if(i<0) ret
findrx "" "(?s)<a name=''(\w+)''>" 0 5|128 rx
rep ;;for each <hr>...<hr>
	i+4
	j=find(text "<hr>" i); if(j<0) break
	
	 out "%i %i" i j
	fr.ifrom=i; fr.ito=j
	if findrx(text rx fr 5 a) ;;get all <a name="FuncName">
		str txt.get(text i j-i)
		for k 0 a.len
			str& name=a[1 k]
			___CHI_FILE& f=m_af_help[]
			f.filename.from(filename "#" name); m_curfilename=f.filename
			f.title=name
			 out "---------- %s -----------" f.filename; out txt
			
			IndexFuncName(m_mw_help name 15) ;;get words of function name
			IndexText(m_mw_help txt 1 1) ;;contents
			
			m_curfile+1
	
	i=j
