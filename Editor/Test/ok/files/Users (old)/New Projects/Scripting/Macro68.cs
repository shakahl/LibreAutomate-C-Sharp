
 VbsExec "msgbox ''First''[]msgbox ''Second''"

VbsAddCode "$desktop$\vbs.vbs" 2
 VbsExec "Func1 ''yes''"
out VbsFunc("Func1" "arg")
 VARIANT v=VbsFunc("Func0")
 out v.vt
 out v.lVal
