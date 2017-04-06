 /
function# iid QMITEM&q level __MF_DATA&d 
str icon
int f=q.itype=5
if(f)
	sel(q.name) case ["private"] ret 1
	 sel(q.name) case ["private","System"] ret 1	
	if(!SendMessage(d.htv TVM_GETNEXTITEM TVGN_CHILD q.htvi)) ret 1

if(d.flags&1) ;;no submenus
	if(f or level<d.level) if(!d.sp.end("-[]")) d.sp.formata("-[]")
	if(!f) d.sp.formata("%s :mac ''%s''[]" q.name q.name); 
else
	d.st.all(level 2 9)
	d.slt.fix(0)
	rep(d.level-level) 
		d.slt+d.st
		d.slt+"[9]<[]"
	
	if(f) 
		d.sp.formata("%s%s>%s * $qm$\folder.ico * 0[]" d.slt d.st q.name)
	else 
		sel q.itype
			case 0
				icon="macro.ico"
			case 1
				icon="function.ico"
			case 2
				icon="menu.ico"		
			
			
			
		d.sp.formata("%s%s%s :mac+ ''%s'' * $qm$\%s[]" d.slt d.st q.name q.name icon)

d.level=level