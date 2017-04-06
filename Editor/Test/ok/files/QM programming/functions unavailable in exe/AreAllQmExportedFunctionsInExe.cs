out
str s sm.getmacro("QmDll")

ARRAY(str) a; int i j
findrx(sm "(?s)[]dll ''qm\.exe''[\r\n]*(.+?[])[]" 0 5 a 1) ;;extract multiple instances of "dll ''qm.exe''[]..."

for(i 0 a.len) ;;join them
	if(a[0 i].beg(" ")) a[0 i][0]=9 ;;single line decl
	s+a[0 i]
 out s

findrx(s "^.+?(\w+)\b(?![*'\]])" 0 4|8 a 1) ;;extract function names
if(a.len!=numlines(s) or a.len<66) end "failed"

 get functions from dll.cpp
 str sd1.getfile("$qm$\dll.cpp") sd2; ARRAY(str) ad
 if(findrx(sd1 "(?s)//BEGIN AreAllQmExportedFunctionsInExe(.+)//END AreAllQmExportedFunctionsInExe" 0 0 sd2 1)<0) end "failed"
 tok sd2 ad -1 ",'' []"

 format macro code
sm=" /exe[][]int i[]"

for(i 0 a.len)
	 s=a[0 i]
	 for(j 0 ad.len) if(ad[j]=s) break
	 if(j=ad.len) sm.formata("i=&%s[]" s)
	sm.formata("i=&%s[]" a[0 i])

sm+"[] BEGIN PROJECT[];[] END PROJECT[]"

 out sm
mac newitem("" sm "Function" "" "" 4|128)

out "Info: Created and executed new macro. Error if a function is unavailable in exe.[][9]Info: QM dll functions that are unavailable in exe are declared with dll?."
