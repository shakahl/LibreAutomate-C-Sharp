 /
function ~f1 ~f2 [flags] [$frm] ;;flags: 1 include subfolders, 2 display full paths, 4 compare files

 Displays files that are in f1 but not in f2, and in f2 but not in f1.
 Also can compare files that have same name, and then display those that have different content.
 f1 and f2 can contain wildcard characters, eg "c:\x\*.txt".
 frm - a format string to be used to display a filename. Must contain single %s.

Dir d1 d2
IStringMap m1=CreateStringMap(1)
IStringMap m2=CreateStringMap(1)
if(findcs(f1 "*?")<0) f1+"\*"
if(findcs(f2 "*?")<0) f2+"\*"
str s fr1.getpath(f1) fr2.getpath(f2)
fr1.expandpath; fr2.expandpath
int fla siz1 siz2
if(flags&1) fla|4
if(!len(frm)) frm="%s"

foreach d1 f1 FE_Dir fla
	s.get(d1.FileName(1) fr1.len)
	m1.Add(s "")
	siz1+d1.FileSize

foreach d2 f2 FE_Dir fla
	s.get(d2.FileName(1) fr2.len)
	m2.Add(s "")
	siz2+d2.FileSize

out "[]Files in %s but not in %s:[]" f1 f2
m1.EnumBegin
rep
	if(!m1.EnumNext(s 0)) break
	if(!m2.Get(s))
		if(flags&2) s-fr1
		out _s.format(frm s)

out "[]Files in %s but not in %s:[]" f2 f1
m2.EnumBegin
rep
	if(!m2.EnumNext(s 0)) break
	if(!m1.Get(s))
		if(flags&2) s-fr2
		out _s.format(frm s)

if(flags&4)
	out "[]Modified files:[]"
	m1.EnumBegin
	rep
		if(!m1.EnumNext(s 0)) break
		if(!m2.Get(s)) continue
		str s1.from(fr1 s) s2.from(fr2 s) sd1 sd2
		sd1.getfile(s1); err out "Cannot open: %s" s1
		sd2.getfile(s2); err out "Cannot open: %s" s2
		if(sd1=sd2) continue
		if(flags&2) s=s2
		out _s.format(frm s)

out "[]Matching files in folder 1: count=%i, size=%i" m1.Count siz1
out "Matching files in folder 2: count=%i, size=%i" m2.Count siz2
out ""
