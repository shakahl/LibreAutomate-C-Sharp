 /CHI_index_tips

str st.getfile("$qm$\tips.txt") sc s ss
int i=find(st "[][]")
sc.get(st i+4)
st.fix(i)

m_curfile=0
foreach s st
	___CHI_FILE& f=m_af_tips[]
	int n=tok(s &f.filename 2 "[9]/>" 2)
	f.filename.trim; m_curfilename=f.filename
	 out "--- %s: %s ---" f.filename f.title
	 index title
	IndexText(m_mw_tips f.title 15)
	  index contents
	ss.format("^/\Q%s\E[](?s)(.+?)([]/|\z)" f.filename)
	if(findrx(sc ss 0 8 s 1)<0)
		 out "no contents"
		goto g1
	 out s
	IndexText(m_mw_tips s 1 1) ;;all
	 g1
	m_curfile+1

SaveLoad(1 4)
