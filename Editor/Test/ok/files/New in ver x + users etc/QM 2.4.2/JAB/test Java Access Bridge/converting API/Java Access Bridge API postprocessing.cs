out
int hm=LoadLibrary("WindowsAccessBridge")
 int hm=LoadLibrary("WindowsAccessBridge-32") ;;on 64-bit Windows
ARRAY(str) a
str sm.getmacro("jab.txt")

out sm.replacerx("^type \w+ = .+[]( ;;.+[])*" "" 8)
 out sm.findreplace("type JOBJECT64 __" "type JOBJECT64 :int")
out sm.findreplace("type JOBJECT64 __[]") ;;declare not in ref
out sm.replacerx("^type (\w+) JOBJECT64'__$" "type $1 :JOBJECT64" 8)
out sm.replacerx("^type .+[] ;;.+[]" "" 8)
out sm.replacerx("^def \w+[]" "" 8)
out sm.replacerx("^def (null|JNIEXPORT) .+[]" "" 8)
out sm.findreplace(": function" ": function[c]")
sm.findreplace("dll ???" "zll- %jab_dll%") ;;for sorting

out sm.len
if(!findrx(sm "^\w[^[]]+([] ;;[^[]]+)*" 0 4|8 a)) end "failed"
out a.len
a.sort(2)
int i
sm=""
for i 0 a.len
	str& s=a[0 i]
	sel s[0]
		case 'z' ;;dll
		s[0]='d'
		str fn fnn
		int j=findrx(s "dll- %jab_dll% \S*?(\w+)( |$)" 0 0 fn 1)
		if(j<0) end "failed"
		sel fn 3
			case "initializeAccessBridge" s="dll- %jab_dll% [Windows_run]initializeAccessBridge"
			case "shutdownAccessBridge" continue
			case "JNI_*" continue
			case else
			if !GetProcAddress(hm fn)
				 out fn
				fnn=fn; fnn[0]=tolower(fn[0])
				if !GetProcAddress(hm fnn)
					 out fn
					fnn+"FP"
					if !GetProcAddress(hm fnn)
						out fn ;;not found
						continue
				s.insert(F"[{fnn}]" 15)
		
		case 'd' ;;def
		if findrx(s "[\x1-\x1f\x80-\xff]")>=0 ;;added 1-2 def with garbage, probably bug in converter
			 out s
			continue
		
		case 't' ;;type
		sel s 2
			case ["type AccessBridgeFPs *","type AllPackages *","type JavaInitiatedPackages *","type WindowsInitiatedPackages *"] continue
			case "type AccessibleActions *" s.replace("1][] ;;note: allocate memory for 256 AccessibleActionInfo: AccessibleActions* a=_s.all(sizeof(AccessibleActionInfo)*256+4)" s.len-4)
			case ["type JNI*","type JavaVM*","type jvalue *"] continue
	
	sm.addline(s)
out sm.len
sm-"#ret[]"

sm.setmacro("__jab_api")
