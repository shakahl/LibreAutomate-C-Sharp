 /
function action hwnd ;;action: 0 open (auto), 1 save (auto), 2 load text from file, 3 save as.

str s sf sd
lpstr nn=+GetProp(hwnd "note_name")
if(nn) s=nn; else s.getwintext(hwnd)
sd.expandpath("$personal$\Notes")
mkdir sd
sf.format("%s\%s.txt" sd s)
int he=id(1 hwnd)

sel action
	case 0
	if(!dir(sf)) ret
	 g0
	s.getfile(sf); err mes _error.description "Error" "x"; ret
	s.setwintext(he)

	case 1
	s.getwintext(he)
	s.setfile(sf); err mes _error.description "Error" "x"
	
	case 2 if(TO_Browse3(0 0 "notedir" sd "Text files[]*.txt[]All files[]*.*[]" "txt" sf)) goto g0
	
	case 3
	if(!inp(sf "Name:" "Save As") || !sf.len || s~sf) ret
	int hwnd2=ShowNote(sf s.getwintext(he) GetToolbarOwner(hwnd) +GetProp(hwnd "note_template"))
	if(!hwnd2) ret
	Note_OpenSave 1 hwnd2
	SendMessage he EM_SETMODIFY 0 0
	clo hwnd

