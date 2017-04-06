 run "$qm$\qmcl.exe" "M ''ąč ﯔﮥ qww''" ;;ac?? qww
 run "$desktop$\shell32 ąč ﯔﮥ k.dll" ;;shell32 ac ?? k.dll

out
str s ss
s="$desktop$\shell32 ąč ﯔﮥ k.dll"
 s="firefox"

out dir(s)
out _s.searchpath(s)

out "------"
Dir d
foreach(d "$Desktop$\*" FE_Dir)
	str sPath=d.FileName(1)
	out sPath
	
