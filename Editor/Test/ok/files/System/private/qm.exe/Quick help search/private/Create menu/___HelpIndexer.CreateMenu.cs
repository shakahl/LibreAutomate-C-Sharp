function $query

 out
 out query

int i fi

 create/load index
if(!m_mw_help)
	Init
	 auto create index for this QM version
	int verport=_portable<<31|QMVER
	if(!rget(i "qmver" "\chi") or i!=verport or !FileExists(m_dir 1))
		CHI_CreateIndex 1
		rset verport "qmver" "\chi"
	 load index files
	SaveLoad(0 1)
	SaveLoad(0 2)
	SaveLoad(0 3)
	SaveLoad(0 4)
	LoadSynonyms
	 auto clear the object when QM hidden
	if(&this=&___chi_indexer) tim 600 sub.Timer

 menu options
str sm

 get words in query
str sq=query
sq.stem(3)
ARRAY(str) aq
tok sq aq -1 " "
for i aq.len-1 -1 -1
	str& k=aq[i]
	if(m_mstop.Get(k)) aq.remove(i)

 search
type ___CHI_FILESCORE fi score
ARRAY(___CHI_FILESCORE) af
ARRAY(int) astat
if(!GetFilesThatContainQuery(aq af astat)) goto stat

 sort
af.sort(0 sub.Sort)

 format menu
str slabel s ss
for i 0 af.len
	fi=af[i].fi
	 out "%i %i %s" fi af[i].score m_af_help[fi].title
	___CHI_FILE& f
	if(fi<10000) &f=m_af_help[fi]
	else if(fi<20000) &f=m_af_tools[fi-10000]
	else if(fi<30000) &f=m_af_func[fi-20000]
	else if(fi<40000) &f=m_af_tips[fi-30000]
	 out "%i    %s    %s" fi f.filename f.title
	slabel=f.title
	slabel.escape(1)
	slabel.findreplace(":" "[91]58]")
	lpstr amp=iif(i<9 "&" "")
	if(fi<10000) ;;help
		sm.formata("%s%i. %s :__QmHelp(0 ''::\%s'') * hh.exe[]" amp i+1 slabel f.filename)
	else if(fi<20000) ;;tools
		s.gett(f.filename)
		if(s.len=f.filename.len) ;;just open dialog
			sel(s 2)
				case ["EA_Main","EH_Main"] ss.format("mac ''%s'' '''' %i" s _hwndqm)
				case else ss.format("mac ''%s''" s)
		else ss.format("TO_Fav ''%s'' %i" s val(f.filename+s.len+1)) ;;open dialog and select action
		sm.formata("%s%i. %s :%s * $qm$\dialog.ico[]" amp i+1 slabel ss)
	else if(fi<30000) ;;functions
		sm.formata("%s%i. %s :QmHelp ''%s'' * $qm$\function.ico[]" amp i+1 f.filename f.filename)
	else if(fi<40000) ;;tips
		sm.formata("%s%i. %s :QmHelp ''%s'' 0 3 * $qm$\tip.ico[]" amp i+1 slabel f.filename)

 out sm

 if many, make submenu
if(af.len>16)
	sm.insert(">More[]" findl(sm 16))
	sm+"<[]"

 if 0, show
if(af.len=0)
	 stat
	sm+">A word not found *user32.dll,4[]"
	for(i 0 aq.len) sm.formata("%i %s :mes ''%i help topics and tools contain word %s.'' '''' ''i'' *user32.dll,%i[]" astat[i] aq[i] astat[i] aq[i] iif(astat[i] 4 1))
	sm+"<[]"

 weblinks
str s1 s2 s3
for(i 0 aq.len) s1.formata("%s* " aq[i].rtrim("i")) ;;qm forum search keywords
s1.fix(s1.len-1); s1.escape(9)
s2=query; s2.escape(9)
aq.sort; s3=aq; s3.trim; s3.findreplace("[]" " ")
lpstr s4=
 -
 &Forum :run "http://www.quickmacros.com/forum/search.php?keywords=%s"
 &Google :run "https://www.google.com/search?q=%s"
sm.formata(s4 s1 s2 s3 query)
 out sm

RECT r; GetWindowRect id(2216 _hwndqm) &r
mac newitem("temp_menu_indexer" sm "Menu" "" "" 1|128) "" r.left r.bottom

 note:
 Google search in QM help does not work well. Either sorting is not optimal or does not find at all.
 In forum Google also often does not find some topics. Also, results are outdated. Updates maybe 1 time/week.
 dropped:
 &Add to Wish List :CHI_WishList "%s" "%s"


#sub Sort
function# param ___CHI_FILESCORE&a ___CHI_FILESCORE&b

if(a.score>b.score) ret -1
if(a.score<b.score) ret 1


#sub Timer
if(!hid(_hwndqm)) ret
tim
___chi_indexer.Clear
