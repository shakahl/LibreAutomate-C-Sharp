function str&sout action iidMain ;;action: 0 open dialog, 1 save dialog

 Gets list of sub names.
 Formats for ListDialog, like "1In MainFunc[]2In sub.Sub1[]4In sub.Sub2".
 If action=0, gets only those containing DD. Else gets all, and appends "  (replace)" to those containing DD.

sout.all
int i
for i 0 a.len
	lpstr append=0
	if(a[i].dd.len) if(action) append="  (replace)"
	else if(!action) continue
	
	if(i=0) _s.getmacro(iidMain 1)
	else _s.from("sub." a[i].name)
	sout.formata("%iIn %s%s[]" i+1 _s append)
