 Use this macro to see what System functions are unavailable in exe.
 Skips what is documented as unavailable. Also skips obsolete.

out
str s=" /exe[][]"
 at first need to autodeclare classes from Classes2
ARRAY(str) a; int i
findrx(_s.getmacro("\System\Declarations\Classes2") "^class (\w+)" 0 4|8 a)
for(i 0 a.len) s.formata("%s v%i; " a[1 i] i)
 also some must be compiler first
s+"[]#compile Dde[]"

EnumQmFolder "\System\Functions" 1 &GetSysFunctionsProc &s
EnumQmFolder "\System\Dialogs\Dialog control classes" 1 &GetSysFunctionsProc &s
EnumQmFolder "\System\Dialogs\Dialog functions" 1 &GetSysFunctionsProc &s

s+"[] BEGIN PROJECT[];[] END PROJECT[]"

 out s
 #ret

 s.setclip
mac newitem("" s "Function" "" "" 4|128)

out "Info: Created and executed new macro. Will not compile if a function is unavailable."
