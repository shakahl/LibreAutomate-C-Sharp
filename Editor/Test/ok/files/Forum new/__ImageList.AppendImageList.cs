function il

int hi i n=ImageList_GetImageCount(il)
for i 0 n
	hi=ImageList_GetIcon(il i 0)
	ImageList_ReplaceIcon this.handle -1 hi
	DestroyIcon hi
