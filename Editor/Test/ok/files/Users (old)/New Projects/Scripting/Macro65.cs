
 VbsExec "msgbox ''First''[]msgbox ''Second''"

VbsAddCode "VBS code" 5
 VbsExec "Func1 ''yes''"
out VbsFunc("Func1" "arg")
 VARIANT v=VbsFunc("Func0")
 out v.vt
 out v.lVal

out VbsFunc("Func2" "arg")

