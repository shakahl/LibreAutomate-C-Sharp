 /
function $folder str&s [flags] ;;flags: 1 no submenus.

 Gets list of QM items in a QM folder and formats it like QM menu.
 Then you can use the list to show QM menu (DynamicMenu) or toolbar (DynamicToolbar). For toolbar, use flag 1.
 Folders named "private" and "System" are excluded.

 folder - folder name or path. Use "" to include all macros.
 s - variable that receives the list. If initially s is not empty, the list will be appended.

 EXAMPLES
 str s
 MenuFromQmFolder "Folder" s
 DynamicMenu(s)

 str s=" /siz0 150 700 /ssiz0 100 30 /set0 1|2|4|8|16|128[]"
 MenuFromQmFolder "Folder" s 1
 DynamicToolbar(s "TB from Folder")


ARRAY(QMITEMIDLEVEL) a
if(!GetQmItemsInFolder(folder &a)) end F"{ERR_MACRO} (folder)"

int i lev
for i 0 a.len
	QMITEMIDLEVEL r=a[i]
	QMITEM q; if(!qmitem(a[i].id 0 q 1)) end ERR_FAILED
	int f=q.itype=5
	
	 end submenu
	if r.level<lev
		if(flags&1) ;;no submenus
			if(!s.end("-[]")) s+"-[]"
		else
			for(_i lev r.level -1) s.formata("%.*m<[]" _i 9)
	
	 skip some folders
	if(f)
		sel(q.name)
			case ["private","System"]
			for(i i+1 a.len) if(a[i].level<=r.level) break
			i-1; goto g1
			case else ;;skip empty folders
			if(i=a.len-1 or a[i+1].level<=r.level) goto g1
	
	 format item
	q.name.escape(1)
	if(flags&1) ;;no submenus
		if(f) if(!s.end("-[]")) s+"-[]"
		else s.addline(q.name)
	else
		s.formata("%.*m%s%s[]" r.level 9 iif(f ">" "") q.name)
	
	 g1
	lev=r.level
