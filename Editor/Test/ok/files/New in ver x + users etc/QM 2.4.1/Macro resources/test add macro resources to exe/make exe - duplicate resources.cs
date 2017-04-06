 /exe
 #exe addfile "$my qm$\test\test.txt" "a" "image"
 #exe addfile "$my qm$\test\test.txt" "hEA3AC2B7" "image"
 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:a" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'
 1
  scan "image:a" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'
 scan "resource:<resource is here>image:hEA3AC2B7" 0 0 1|2

 #exe addfile "$my qm$\test\test.txt" 5
 str s=":5 $my qm$\test\test2.txt"
 #exe addfile "$my qm$\test\test.txt" "a" "image"
 #exe addfile "$my qm$\test\test.txt" "hEA3AC2B7" "image"

 out _s.getfile(":5")

 #exe addfile "$my qm$\test\test.txt" 5
 out _s.getfile(":5 $my qm$\test\test2.txt")
 #exe addfile "$my qm$\test\test.txt" 5 4
 str s45="resource:4:5"

 str s=":5 $my qm$\test\test.txt"
 str ss="&:5 $my qm$\test\test2.txt"

 BEGIN PROJECT
 main_function  make exe - duplicate resources
 exe_file  $my qm$\Macro2210.qmm
 flags  23
 guid  {85213A0E-614B-4E78-A289-12C24821CD75}
 END PROJECT
