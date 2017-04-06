 /test ShowMenuStringFromFilesInFolder2
function $folderFiles str&sm ARRAY(str)&aPaths

 Creates string for ShowMenu from matching files in a folder and its subfolders.

 folderFiles - folder and filename wildcard string. See example.
 sm - receives string that can be passed to ShowMenu.
 aPaths - receives full paths of matching files.

 EXAMPLE
 str sm; ARRAY(str) a
 ShowMenuStringFromFilesInFolder2 "$documents$\*.txt" sm a
 int i=ShowMenu(sm)-1; if(i<0) ret
 out a[i]


sm.all
aPaths=0


#sub Folder
function 



#ret

sm.all
aPaths=0
int k level; str rel relPrev
Dir d
foreach(d folderFiles FE_Dir 32|64)
	rel=d.RelativePath
	k=d.Level
	 out "%i %s" k rel
	
	 end submenu if need (insert "<" line)
	if level
		for(level level k -1) sm.formata("%.*m<[]" level 9)
		 may be in other folder
		for level level 0 -1
			if(!StrCompareN(rel relPrev findt(rel level "\"))) break
			sm.formata("%.*m<[]" level 9)
	relPrev=rel
	
	 begin submenu if need (insert ">Folder" line)
	for level level k
		_s.gett(rel level "\")
		sm.formata("%.*m>%s[]" level 9 _s)
	
	 add normal item, with id = aPaths index + 1
	aPaths[]=d.FullPath
	sm.formata("%.*m%i %s[]" k 9 aPaths.len _s.getfilename(rel))

rep(level) sm.formata("%.*m<[]" level 9); level-1
