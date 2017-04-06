out
def TEST 0
#if TEST
str s=
 	//___________________________________________
 
 	//Adds direct children of folder iParent to treeview, if not already added.
 	void AddFolderChildrenToTV(int iParent=0)
 	{
 		if(m_a[iParent].isTvFolderChildrenAdded) return;
 		for(int i=iParent+1; i<m_a.len; i++) if(m_a[i].iParent==iParent && !m_a[i].htvi)
 		{
 			ITEM& t=m_a[i];
 			t.htvi=dlg::TvAdd(m_tv, m_a[iParent].htvi, LPSTR_TEXTCALLBACK, i, -2, TVI_LAST, (t.flags&D_FOLDER)?1:0);
 		}
 		m_a[iParent].isTvFolderChildrenAdded=true;
 	}
;
#else
str s.getsel
#endif

if(!s.len) ret
ARRAY(str) a
str rx=
 ^((?:\s*(?://.*)?
 )*)(.+?\(.*\))\s*(\{(?s).*\})
if(findrx(s rx 0 0 a)<0) mes- "Incorrect selection. Select one functions with body. Can include comments and empty lines before it."
 out a[0]; ret

str cls
int c=child("" "ComboBox" win() 0x0 "accName=Scopes in *") ;;class combo in navigation bar
if(c) cls.getwintext(c); else cls="CLASS_UNKNOWN_WITHOUT_NAVIGATION_BAR"; out "Need navigation bar to get class. Options -> Text Editor -> C/C++ -> General. Or run macro 'Toggle navigation bar' from QM VS toolbar menu."

str sh scpp
sh.from(a[2] ";[]")

a[2].replacerx("=\w+" "/*$0*/") ;;remove default values of optional params
a[2].replacerx("^\s*(.* )?(~?\w+\(.*\))$" F"$1{cls}::$2" 4) ;;insert class
scpp=F"{a[1]}{a[2]}[]{a[3]}[]"
scpp.replacerx("^[\t ](.*[])" "$1" 8)
if(!scpp.beg("//___")) scpp-"//___________________________________________[][]"

#if TEST
 out sh
out scpp
#else
paste sh
key CT CE
paste scpp
1; key CT
#endif
