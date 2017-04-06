__ImageList- il
il=ImageList_Create(16 16 ILC_MASK|ILC_COLOR32 0 8)

int hi
hi=GetWindowIcon(win("Quick"))
ImageList_ReplaceIcon(il -1 hi)
DestroyIcon(hi)
hi=GetWindowIcon(win("Notepad"))
ImageList_ReplaceIcon(il -1 hi)
DestroyIcon(hi)

SetThreadMenuIcons "100=0 101=1" +il 2

ShowMenu("100 a[]101 b")
