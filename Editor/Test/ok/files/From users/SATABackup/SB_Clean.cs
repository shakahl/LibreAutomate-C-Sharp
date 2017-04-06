function ARRAY(str)F2 ~Path2 ARRAY(str)Ext ARRAY(str)Atr
;; F2 = Filename with relative path
;; Path2 = Backup path
;; Atr =  s: string of atributes. View SB_getAttribute for more details

int i j days
str s1 s2 filename pattern
DATE filedate
DATE today.getclock
Dir d

;;Checking atributes.
int na nb nc
for _i 0 Atr.len
	na = SB_getAttribute(Atr[_i] 0)
	nb = SB_getAttribute(Atr[_i] 1)
	nc = SB_getAttribute(Atr[_i] 2)
	if (nb<10)
		nb = -1
	if (nc<10)
		nc = -1
	Atr[_i].format("%i,%i,%i" na nb nc)
;; End of check

;; Search and remove original files removed and his versions
for i F2.len-1 -1 -1
	filename.format("%s%s" Path2 F2[i])
	d.dir(filename)
	filedate = d.TimeModified2
	pattern = "\.v\d{2}($|\.)"
	if (findrx(filename pattern 0 1)<0) ;;Check if file is a deleted file
		_s.GetFilenameExt(filename)
		days = SB_getAttribute(Atr[SB_getIDExtension(&Ext _s)] 2)
		if (days >= 0)
			filedate = filedate + days
			if (filedate < today)
				s2.GetFilenameExt(filename)
				s1.getfilename(filename)
				if (s2.len>0)
					pattern.format("%s\.v\d\d\.%s" s1 s2)
				else
					pattern.format("%s\.v\d\d" s1)
				for j F2.len-1 -1 -1
					if (findrx(F2[j] pattern 0 1)>0)
						_s.format("%s%s" Path2 F2[j])
						del- _s
						F2.remove(j)
				reg.formata("%s - Removed %s[]" _s.time filename)
				del- filename
				F2.remove(i)

;; End of original files

;; Search and remove old versions
pattern="\.v\d{2}($|\.)"
for i F2.len-1 -1 -1
	filename.format("%s%s" Path2 F2[i])
	d.dir(filename)
	filedate = d.TimeModified2
	_s.GetFilenameExt(filename)
	days = SB_getAttribute(Atr[SB_getIDExtension(&Ext _s)] 1)
	if (days >= 0)
		filedate = filedate + days
		if (filedate < today)
			del- filename
			F2.remove(i)
;; Old versions removed
ret