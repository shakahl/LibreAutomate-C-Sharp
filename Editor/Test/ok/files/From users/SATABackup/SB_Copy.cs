function ~F1 ~Path1 ~Path2 #Num [#Version]
;; F1 = Filename with relative path = Result
;; Path1 = Source
;; Path2 = Destination
;; Num = Max.Versions available
;; Version = Current version. For recursivity

;; Note: If Max number of versions has been decreased, AND the file has been modified,
;;       deletes older copies.

if(!Version) Version=0
str name = SB_getVersionName(F1 Path2 Version 2)

str a b
Dir d
if (d.dir(name)) ;; name exists.
	if (Version < Num)
		SB_Copy(F1 Path1 Path2 Num (Version+1))
	else
		SB_Copy(F1 Path1 Path2 Num (Version+1))
		del- name
		 reg.formata("%s - Deleted %s[]" _s.time name)
if (Version > Num)
	ret
if (Version > 0)
	a = SB_getVersionName(F1 Path2 (Version-1) 2)
	b = name
	ren a b FOF_SILENT
if(Version=0)
	a.format("%s%s" Path1 F1)
	b.format("%s%s" Path2 F1)
	reg.formata("%s - Copied %s[]" _s.time name)
	mkdir _s.getpath(b)
	cop a b FOF_SILENT
ret