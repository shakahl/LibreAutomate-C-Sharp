 /get dll names
function $dumpbin_exe $lib_list str&sout [flags] [$dll_map] ;;flags: 1 include subfolders, 2 include system dlls for libs

 Extracts dll function names from lib or/and dll file(s).
 Run this function before ConvertCtoQM and save its output (sout) to dest_file_fdn.txt.
 Read more in ConvertCtoQM.
 Requires dumpbin.exe.

 dumpbin_exe - full path of dumpbin.exe
 lib_list - one or more lib or/and dll files. Can contain wildcard characters. Examples: "C:\mylibdir\file.lib", "$system$\*.dll".
 sout - receives the list of extracted functions-dlls. The list is appended.
 dll_map - a list of library filenames and corresponding dll filenames. Must be with extensions.
    Used to get dll filenames that don't match lib filenames.
    This function does not parse lib files to extract true dll names, and by default uses lib filename for dll filename.


str s ss sdb sdn
int i j n1 n2
Dir d
ARRAY(str) a ;;af
IStringMap m=CreateStringMap(1)
if(len(dll_map)) m.AddList(dll_map "")
lpstr dln

sdb.expandpath(dumpbin_exe)
foreach ss lib_list
	if(!ss.len) continue
	foreach(d ss FE_Dir iif(flags&1 4 0))
		if(a.len=i)
			a.redim(-1)
			a[i].format("''%s'' /EXPORTS" sdb)
			 a[i].format("''%s'' /HEADERS /EXPORTS" sdb) ;;headers contain dll names, but slow and requires much memory.
		a[i].formata(" ''%s''" d.FileName(1))
		n1+1
		if(flags&2 and matchw(d.FileName "*.lib" 1))
			sdn=d.FileName; sdn.set("dll" sdn.len-3)
			if(sdn.searchpath)
				a[i].formata(" ''%s''" sdn)
				n1+1
		if(a[i].len>9700) i+1
if(!n1) goto g1

 Wsh.WshShell objShell._create
 for i 0 a.len ;;possibly exec multiple times because dumpbin.exe fails if the command line is too long
	  out a[i]
	 Wsh.WshExec objExec = objShell.Exec(a[i])
	 s+objExec.StdOut.ReadAll
 objExec=0
for i 0 a.len ;;possibly exec multiple times because dumpbin.exe fails if the command line is too long
	RunConsole2 a[i] _s; s+_s

 out s.len
 out s
 s.setfile("$desktop$\test.bin"); run "$desktop$\test.bin"
 ret

rep
	i=findrx(s "^Dump of file (.+\\(.+?\..+?))[][]File Type: (LIBRARY|DLL)[][](?s)(.+?)(?=[12]|[]  Summary[])" i 9 a); if(i<0) break
	 i=findrx(s "^Dump of file .+\\(.+?\..+?)[][]File Type: (LIBRARY|DLL)[][](?s)(.+?)(?=[12])" i 9 a)
	 err out i; out s.len; ret
	if(i<0) break
	 out i
	i+a[0].len
	 out a[2]
	 out a[4]
	 continue
	int islib=a[3]~"LIBRARY"
	lpstr rx
	if(islib) rx="[] +ordinal +name[][](?s)(.+)"
	else rx="[] +ordinal +hint +RVA +name[][](?s)(.+)"
	if(findrx(a[4] rx 0 0 ss 1)<0)
		 out "------[]%s[]%s" a[2] a[4]
		continue
	 out "------[]%s[]%s" a[2] ss
	
	 if(islib) af[af.redim(-1)]=a[1]
	
	if(islib)
		dln=m.Get(a[2])
		if(dln) a[2]=dln; else a[2].fix((a[2].len-4))
	else if(a[2].endi(".dll")) a[2].fix((a[2].len-4))
	if(findrx(a[2] "\W")>=0) a[2]-"''"; a[2]+"''";; out a[2]
	
	str sr.format("$1 %s" a[2])
	if(islib) rx="^.{18}[_\?]?([A-Z_]\w*)(?:@\d+|@@\S+|)(?: .*)?(?=[])"
	else rx="^.{26}(\??[A-Z_]\w*(?:@\d+|@@\S+|))(?: .*)?(?=[])"
	ss.replacerx(rx sr 9)
	if(!islib) ss.replacerx("^.{26}\[NONAME\].*[]" "" 8)
	 if(findrx(ss "^ .+[]" 0 8)>=0) _s=ss; out _s.replacerx("^[^ ].+?[]" "" 8); out "%s[]%s" a[2] _s
	ss.replacerx("^ .+[]" "" 8)
	if(!islib and ss.beg("?"))
		ss.replacerx("^(\?(\w+)\S*) (.+)[]" "$2 $3 [$1][]" 8) ;;decorated dll -> undecorated dll decorated
	 out ss
	sout+ss
	
	n2+1

  get dll names
 for i 0 af.len
	 str& f=af[i]
	 if(!f.endi(".lib")) continue
	  out f
	 s.getfile(f); s.findreplace("" " " 32)
	 ss.getfilename(f); ss+"."
	 j=find(s ss 0 1); if(j<0) continue
	 j+ss.len
	 if(!q_strnicmp(s+j "dll" 3)) continue
	 ss.geta(s j 3)
	 out ss
	 ... (not finished)

 g1
out "Found %i matching files. Extracted %i functions from %i files."  n1 numlines(sout) n2
