function hImagelist firstImage lastImage

 Allows to use imagelist images for tray icons.

 hImagelist - imagelist handle. For example an __ImageList variable (you can call its function Load to load a bitmap file created with the QM imagelist editor).
 firstImage - 0-based index of the first image to get from the imagelist.
 lastImage - 0-based index of the last image to get from the imagelist.

 REMARKS
 Adds one or more icons from an imagelist to an internal array.
 Then you can show that icons with Modify.
 At first need to call AddIcon, even if the icon will not be used. Its index with Modify is 1, therefore imagelist icons are 2, 3 and so on.
 This function copies the images, therefore the imagelist can be destroyed immediately after calling it.

 EXAMPLE
 Tray t.AddIcon
 __ImageList k.Load(":10 $qm$\il_qm.bmp")
 t.UseImagelist(k 0 3)
 int i
 for i 0 4
	 t.Modify(2+i)
	 1


int i
for i firstImage lastImage+1
	__TRAYIC& r=m_a[]
	r.hicon=ImageList_GetIcon(hImagelist i 0)
