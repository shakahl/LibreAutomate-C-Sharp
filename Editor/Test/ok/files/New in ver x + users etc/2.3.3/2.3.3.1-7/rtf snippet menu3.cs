str dbFile="$my qm$\snippets.db3"
str s=RtfSnippets(3 dbFile "") ;;get list of snippet names from database

str ss=s
ss.replacerx("^(.+?)\..+[](\1\..+[])*" "$1[]" 1|8) ;;get categories

MenuPopup m
m.AddItems(ss 1) ;;create menu
m.AddItems("-[]Add snippet...[]Delete snippet...[]-[]Cancel" 10000)
int i=m.Show(0 0 1) ;;show menu and wait

sel i
	case [0,10004] ret ;;cancel
	
	case 10001 ;;add
	if(!inp(s "Snippet name. Can be category.name.[]RTF data will be copied from the active window." "QM - add or replace RTF snippet") or !s.len) ret
	RtfSnippets 0 dbFile s
	
	case 10002 ;;delete
	0.1
	s.replacerx("^(.+?)\..+[](\1\..+[])*" ">$1[]$0<[]" 1|8) ;;add submenus
	MenuPopup m2.AddItems(s 1)
	i=m2.Show; if(i=0) ret
	m2.GetItemText(i s)
	RtfSnippets 2 dbFile s
	
	case else ;;paste
	m.GetItemText(i ss) ;;get selected snippet name
	 if findrx(s F"^\Q{ss}\E\..+[](\2\..+[])*
	RtfSnippets 1 dbFile ss ;;get snippet from database, and paste

err+ mes _error.description
