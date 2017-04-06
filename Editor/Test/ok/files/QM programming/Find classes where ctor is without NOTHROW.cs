 Run this multiple times, until there are no more classes with throwing ctors.
 Tip: to skip ctors where don't need NOTHROW, add a space at the beginning of the ctor line.

out
ARRAY(str) a
Dir d
foreach(d "$qm$\*.h" FE_Dir 4)
	str path=d.FullPath
	sel path 3
		case ["Q:\app\scintilla\*","Q:\app\TextCapture\libMinHook\*"] continue
	str data.getfile(d.FullPath);; err ...
	data.findreplace("NOINLINE "); data.findreplace("__declspec(noinline) ")
	int i=findrx(data "(?s)\bclass\s+(\w+)\s.+?\n\t+\1\(" 0 0 a)
	if(i<0) continue
	data[i+a[0].len]=0
	VS_link F"{path}({numlines(data)})"
	out a[0]
