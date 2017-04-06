__ImageList il.Load("$qm$\il_qm.bmp")
__ImageList il2.Load("$qm$\il_icons.bmp")
il.AppendImageList(il2)

int i n=ImageList_GetImageCount(il)
str s; for(i 0 n) s.addline(F"{i}")
ListDialog(s "" "" 0 0 0 0 0 0 il)
