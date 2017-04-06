 /Create C_macro dll
function $refFile str&sout [IStringMap'mrem] [flags] ;;flags: 1 all

str sd.getfile(refFile)

ARRAY(str) a; int i
str rx rx2 repl back
if(flags&1)
	rx="^dll C_macro (\w+) (.+)[] ;;([^\r\n]+)$"
	repl="dll ''$qm$\C_macro'' #$1 $2[] ;;$3"
	rx2="^dll \Q''$qm$\C_macro'' #\E(\w+) (.+)[] ;;([^\r\n]+)$"
else
	rx="^dll C_macro (\w+) (.+)[] ;;((?:Send|Post)Message[^\r\n]+)$"
	repl="dll ''$qm$\qmcomctl'' #$1 $2[] ;;$3"
	rx2="^dll \Q''$qm$\qmcomctl'' #\E(\w+) (.+)[] ;;((?:Send|Post)Message[^\r\n]+)$"

 need to replace C_macro in ref file
if(sd.replacerx(rx repl 8))
	 out sd
	back=refFile; UniqueFileName back; cop refFile back
	sd.setfile(refFile)

if(!findrx(sd rx2 0 4|8 a)) mes- "not found"
 ret

for i 0 a.len
	str& sn=a[1 i]
	str& sa=a[2 i]
	str& sm=a[3 i]
	if(mrem and mrem.Get(sn)) continue
	sa.findreplace(" " ", ")
	sm.findreplace("SendMessage(" "SendMessage((HWND)")
	sm.findreplace("PostMessage(" "PostMessage((HWND)")
	sout.formata("#undef %s[]__declspec(dllexport) int %s(%s) { return %s; }[]" a[1 i] a[1 i] sa sm)
