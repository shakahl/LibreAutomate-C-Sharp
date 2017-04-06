str dbFile="$my qm$\snippets.db3"
str s=RtfSnippets(3 dbFile "") ;;get list of snippet names from database

s.replacerx("^((.+?)\..+[](\2\..+[])*)" ">$2[]$1<[]" 1|8) ;;add submenus

MenuPopup m
m.AddItems(s 1) ;;create menu
m.AddItems("-[]Add snippet...[]Delete snippet...[]-[]Cancel" 10000)
int i=m.Show(0 0 1) ;;show menu and wait

sel i
	case [0,10004] ret ;;cancel
	
	case 10001 ;;add
	if(!inp(s "Snippet name. Can be category.name.[]RTF data will be copied from the active window." "QM - add or replace RTF snippet") or !s.len) ret
	RtfSnippets 0 dbFile s
	
	case 10002 ;;delete
	m.DeleteItems("10000-100004")
	i=m.Show; if(i=0) ret
	m.GetItemText(i s)
	RtfSnippets 2 dbFile s
	
	case else ;;paste
	m.GetItemText(i s) ;;get selected snippet name
	RtfSnippets 1 dbFile s ;;get snippet from database, and paste

err+ mes _error.description
