out
str ss=
 $qm$\mouse.ico
 $qm$\New.ico
 $qm$\ontop.ico
 $qm$\Open.ico
 $qm$\password.ico
 $qm$\Paste.ico
 $qm$\image.ico
 $qm$\info.ico
 $qm$\inp.ico
 $qm$\js.ico
 $qm$\keyboard.ico
 $qm$\lightning.ico
 $qm$\list.ico
 $qm$\macro.ico
 $qm$\menu.ico
 $qm$\mes.ico
 $qm$\MIDI triggers.ico
int il=ImageList_Create(16 16 ILC_COLOR32|ILC_MASK 0 numlines(ss))
str s
foreach s ss
	int hi=GetFileIcon(s)
	if(!hi) end "failed: %s" 1 s
	ImageList_ReplaceIcon il -1 hi
	DestroyIcon hi

 IStream is
 int hg=GlobalAlloc(GMEM_MOVEABLE 0)
 CreateStreamOnHGlobal(hg 1 &is)
 ImageList_Write il is
 s.fromn(GlobalLock(hg) GlobalSize(hg))
 s.encrypt(4)
 GlobalUnlock hg

s.FromImageList(il)

out ImageList_GetImageCount(il)
ImageList_Destroy il

out s.len
out s

Q &q
il=s.ToImageList
Q &qq
outq
out ImageList_GetImageCount(il)

 Q &q
 __GdiHandle bm=LoadPictureFile("$qm$\de_ctrl.bmp")
 Q &qq
 outq
 out bm
