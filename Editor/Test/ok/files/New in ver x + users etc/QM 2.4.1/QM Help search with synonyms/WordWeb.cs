out
str words=
 accept
 access
 accidentally
 account
 achieved
 acquire
 action
 activate
 last
 late
 launch
 layout
 leave
 length
 limit
 line
 load

words.getmacro("Macro2204")

int w=act(win("WordWeb" "TTheDi"))
int hedit=id(1001 w) ;;editable text
int htab=child("" "TTabControl" w) ;;page tab list
int hlist=child("" "TListBox" w) ;;list
Acc _a alist.Find(hlist "LIST" "" "class=TListBox" 0x1004)
str tab tabs="Synonyms[]Type of"
int i n nPrev
str s ;;syn

spe 10
foreach s words
	 out "<><Z 0x8000>%s</Z>" s
	act hedit
	key (s) Y
	act htab
	 syn.all
	int nSyn=0
	foreach tab tabs
		_a.Find(htab "PAGETAB" tab "class=TTabControl" 0x5)
		if(_a.NotFound) continue
		_a.Select(3)
		 0.5; continue
		alist.elem=0
		rep(20) 0.01; n=alist.ChildCount; if(n!=nPrev) break
		nPrev=n;; out n
		for(alist.elem 1 n+1)
			str k=alist.Name
			if(findcs(k "][")>=0) k.gett(k 0 "][")
			k.lcase; k.trim("* ")
			 if(findw(syn k)>=0) continue
			 if(syn.len) syn+", "
			 syn+k
			if(findw(s k)>=0) continue
			s+", "; s+k
			nSyn+1
	 out syn
	if(!nSyn) out "<><c 0xff>%s</c>" s; continue
	out s

#ret
accept
11
24
access
8
22
accidentally
5
account
19
17
achieved
3
5
acquire
13
3
action
17
21
activate
9
5
