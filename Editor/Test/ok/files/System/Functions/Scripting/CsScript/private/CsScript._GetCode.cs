function$ $code str&ss str&sourceFile int&flags

 Errors: <..>

opt noerrorshere 1

if empty(code)
	str fs f; GetCallStack &fs 3
	foreach(f fs) sel(f 2) case ["CsScript.*","CsExec","CsFunc","VbExec","VbFunc","<*"] case else code=f; goto g1

if findc(code 10)<0
	sel code 2
		case "macro:?*"
		code+6
		 g1
		Scripting_GetCodeFromMacro code ss
		sourceFile.from(">" code)
		code=ss
		
		case ["$*","%*","?:\*","\\*",":*"]
		sourceFile=code
		code=ss.getfile(sourceFile.expandpath)

if(empty(code)) end "script empty" 8
else if(flags&0x10000) if(findw(code "class")<0) flags|1

flags&0xffff
ret code
