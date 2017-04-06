out
 __ImageList il.Create("$qm$\cut.ico[]$qm$\copy.ico[]$qm$\paste.ico")
 __ImageList il=__ImageListFromIcons("$qm$\cut.ico[]$qm$\copy.ico[]$qm$\paste.ico")
 out il
 out ImageList_GetImageCount(il)

 DT_SetMenuIcons "1=0 2=1 3=2" +il 2
 DT_SetMenuIcons "1=0 2=1 3=2" "$qm$\il_qm.bmp" 1
DT_SetMenuIcons "1=0 2=1 3=2" "$qm$\cut.ico[]$qm$\copy-no.ico[]$qm$\paste.ico"
out ShowMenu("1 one[]2 two[]3 three")

 out ListDialog("1 one[]2 two[]3 three" "" "" 0 0 0 0 0 0 "$qm$\il_qm.bmp")
 out ListDialog("1 one[]2 two[]3 three" "" "" 0 0 0 0 0 0 "$qm$\cut.ico[]$qm$\cop.ico[]$qm$\paste.ico")
 out ListDialog("1 one[]2 two[]3 three" "" "" 32 0 0 0 0 0 "shell32.dll,5[]shell32.dll,6[]shell32.dll,7[]")
