/exe
out
 out list("One[]Two WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAA")
 out list("One[]Two" "" "" 0 0 0 0 1)
 out list("One[]Two" "" "" 0 0 0 0 0 _hwndqm)
 out list("One[]Two" "" "" 0 0 0 0 0 win)
 out list("" "One[]Two" "cap ''tion''")
 out list("&One[]Two" "" "" 0 0 5)
 out list("One[]Two[]Three" "" "" 0 0 5 2 1)
 out list("10one[]20 Two" "" "" 0 0 3 20 2)
 out list("" "One[]Two" "" 0 0 1234567890)
 out list("a[]b[]c" "text" "cap" -1 -1 5 2 1)
 out list("1[]2[]3[]4[]5[]6[]7[]&8[]9[]")
 for(_i 1 90) _s.formata("%i[]" _i)
 out list(_s "" 0 0 0 0 0 4)


 out ListDialog("One[]Two WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAA")
 _s="One[]Two WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAAAAAA[]Three"
 out ListDialog(_s _s "Long text" 8 0 100 300)
 out ListDialog("One[]Two WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAA" 0 0 64 0 0 -20)
 out ListDialog("One[]Two WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW AAAAAAA" 0 0 128 0)
 out ListDialog("One[]Two" "" "" 1)
 out ListDialog("One[]Two" "" "" 0 _hwndqm)
 out ListDialog("One[]Two" "" "" 0 win)
 out ListDialog("" "One[]Two" "cap ''tion''")
 out ListDialog("&One[]Two" "" "" 0 0 0 0 5)
 out ListDialog("One[]Two" "" "" 0 0 0 0 3 2)
 out ListDialog("10one[]20 Two" "" "" 2)
 out ListDialog("10one[]20 Two" "" "" 2 0 0 0 3 20)
 out ListDialog("k" "One[]Two" "" 8 0 0 0 1234567890)
 out ListDialog("a[]b[]c" "text" "cap" 0 0 -1 -1 5 2)
 for(_i 0 15) _s.formata("%i and some text[]" _i+1)
 out ListDialog(_s)
 out ListDialog(_s 0 0 0 0 0 0 1000)

 out ListDialog("One[]Two" "" "" 0 0 0 0 0 0 "$qm$\keyboard.ico[]$qm$\paste.ico")
 out ListDialog("1 One[]0two" "" "" 4 0 0 0 0 0 "$qm$\keyboard.ico[]$qm$\paste.ico")
 out ListDialog("One[]Two[]Three" "" "" 0 0 0 0 0 0 "$qm$\qm.ico[]$system$\shell32.dll,7[]$qm$\winapi.txt")
 out ListDialog("One[]Two[]Three[]Four" "" "" 32 0 0 0 0 0 "$qm$\qm.ico[]$system$\shell32.dll,7[]$qm$\winapi.txt[]$qm$\paste.ico")
 out ListDialog("One[]Two" "" "" 0 0 0 0 0 0 "$qm$\il_qm.bmp")
 out ListDialog("4 One[]14 Two" "" "" 4 0 0 0 0 0 "$qm$\il_qm.bmp")
 __ImageList il.Load("$qm$\il_qm.bmp")
 __ImageList il.Create("$qm$\keyboard.ico[]$qm$\paste.ico" 16)
 out ListDialog("One[]Two" "" "" 0 0 0 0 0 0 il)
out ListDialog("0 One[]1Two[]2Three[]1Four" "" "" 4|8 0 0 0 0 0 "$qm$\il_test32.bmp")
 out ListDialog("10 0 One[]20 1Two[]30 2Three[]40 1Four" "" "" 2|4|8 0 0 0 0 0 "$qm$\il_test32.bmp")
 out ListDialog("One[]Two" "" "" 0 0 0 0 0 0 "$qm$\il_test24.bmp")
 out ListDialog("One[]Two[]Three" "" "" 0 0 0 0 0 0 "resource:<dialog_resource_imagelist>image:il_dlg")
 for(_i 0 16) _s.formata("%i and some text[]" _i+1)
 out ListDialog(_s "" "" 0 0 0 0 100 0 il)

#ret

 BEGIN PROJECT
 main_function  Macro2219
 exe_file  $my qm$\Macro2219.qmm
 flags  6
 guid  {1F6BF2DA-C6D9-41B8-81DA-1C30F07C3D1C}
 END PROJECT
