 /
function# [~name] [~text] [hwndowner] [$template] [flags] ;;flags: 1 don't load from file.

 Shows a sticky note based on a QM toolbar (can be window-attached).
 This project is unfinished.

 name - name for note-toolbar and associated text file.
  If name is not specified, creates new note with unique
  name (you can Save As ...). If name is "?", promts to
  open an existing note.
 text - initial text. If not used, will load from saved file.
 hwndowner - handle of owner window or 0.
 template - name of toolbar from which this note will inherit
  toolbar style and buttons. Alternatively, you can use common
  template. Store its name into global str variable _note_template.
  Default template toolbar is "note_template".

str s t; int templ templexist

if(!name.len) name=Note_Unique("Note")
else if(name="?")
	mkdir s.expandpath("$personal$\Notes")
	name=""
	if(!OpenSaveDialog(0 name "Text files[]*.txt[]All files[]*.*[]" "txt" &s)) ret
	name.getfilename(name)
else if(findcs(name "/\|:*?<>''[][9]")>=0) end "the name contains invalid characters"

str+ _note_template
if(!empty(template)) templ=1
else if(_note_template.len) template=_note_template
else if(qmitem(_s.from("\User\Notes\" name) 5)) template=name; templexist=1
else template="note_template"
t.getmacro(template); err out _error.description; ret

lpstr t1=" /hook Note_TBProc"
if(!t.beg(" /")) t-"[]"
if(find(t t1)<0) t-t1; templexist=0

if(!templexist)
	s.format("%s :Note_Menu val(_command) * default menu icon.ico[]" name)
	int i=findc(t 10); if(i<0) t+"[]"; i=t.len; else i+1
	t.insert(s i s.len)

int ni h; str cmd
if(hwndowner) cmd=hwndowner
ni=newitem(name t "Toolbar" "" "\User\Notes" 1); err end _error
h=mac(ni cmd); err ret
if(!h) ret

SetProp(h "note_name" +_strdup(s.getmacro(ni 1)))
if(templ) SetProp(h "note_template" +_strdup(template))

if(text.len)
	text.setwintext(id(1 h))
	SendMessage id(1 h) EM_SETMODIFY 1 0
else if(flags&1=0) Note_OpenSave 0 h
ret h
