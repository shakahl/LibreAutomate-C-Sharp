out
 del- "$myqm$\test\??????????.qme"
 MkDir "$temp$\qmexe"
str template.expandpath("$qm$\qmmacro.dll")
str sf.expandpath(F"$myqm$\test\{_s.RandomString(10 10 `a-z`)}.qme")
 str sf.expandpath(F"$temp$\qmexe\{_s.RandomString(10 10 `a-z`)}.qme")
FileCopy template sf 2
 SetAttr sf FILE_ATTRIBUTE_TEMPORARY|FILE_ATTRIBUTE_NOT_CONTENT_INDEXED
 __HFile f.Create(
str sd.all(4096 2)
outx GetAttr(template)
outx GetAttr(sf)
PF
rep 1000
	 cop- template sf; wait RandomNumber/50
	int h=BeginUpdateResourceW(@sf 1)
	if(!h) out "BeginUpdateResourceW"; continue
	if(!UpdateResourceW(h +RT_RCDATA +100 0 sd sd.len)) out "UpdateResourceW"
	int ec=GetLastError
	if(!EndUpdateResourceW(h 0))
		out "%i %i" ec GetLastError
		out _s.dllerror
		break
	 0.02
PN; PO

 rep 10000
	 __HFile h.Create(sf OPEN_EXISTING)
	 if(!h) out _s.dllerror; 0.01; continue
	 if(!WriteFile(h sd sd.len &_i 0)) out "WriteFile"
	 h.Close
