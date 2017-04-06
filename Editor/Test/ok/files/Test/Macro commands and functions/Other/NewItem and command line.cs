str a=
 MS "NewItem" A "temp_function" "function i; mes i; ret i*2" "Function" "" "\user\temp" 1
if(!run("qmcl.exe" a "" "" 0x400)) ret
a=
 MS "temp_function" A 5
out run("qmcl.exe" a "" "" 0x400)
