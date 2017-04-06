 No alpha. Instead use Editor.Test.DevTools.CreatePngImagelistFileFromIconFiles_il_tb() etc.

out
str files png

files=
 .cs
 $qm$\folder.ico
 $qm$\folder_open.ico
png="Q:\app\Catkeys\Editor\Resources\il_tv.png"

files=
 $qm$\new.ico
 $qm$\properties.ico
 $qm$\save.ico
 $qm$\icons\run.ico
 $qm$\icons\compile.ico
 $qm$\deb next.ico
 $qm$\icons\deb into.ico
 $qm$\icons\deb out.ico
 $qm$\deb cursor.ico
 $qm$\deb run.ico
 $qm$\deb end.ico
 $qm$\undo.ico
 $qm$\redo.ico
 $qm$\cut.ico
 $qm$\copy.ico
 $qm$\paste.ico
 $qm$\icons\back.ico
 $qm$\icons\active_items.ico
 $qm$\icons\images.ico
 $qm$\icons\annotations.ico
 $qm$\help.ico
 $qm$\droparrow.ico
 $qm$\icons\record.ico
 $qm$\find.ico
 $qm$\icons\mm.ico
 $qm$\icons\tags.ico
 $qm$\icons\resources.ico
 $qm$\icons\icons.ico
 $qm$\options.ico
 $qm$\icons\output.ico
 $qm$\tip.ico
 $qm$\icons\tip_book.ico
 $qm$\delete.ico
 $qm$\icons\back2.ico
 $qm$\open.ico
 $qm$\icons\floating.ico
 $qm$\icons\clone dialog.ico
 $qm$\dialog.ico
png="Q:\app\Catkeys\Editor\Resources\il_tb.png"

sub.CreatePngImagelistFromIcons files png
run png


#sub CreatePngImagelistFromIcons
function $iconFiles $pngFile [imageSize]

if(imageSize=0) imageSize=16

#compile "__Gdip"
GdipBitmap b.CreateEmpty(imageSize*numlines(iconFiles) imageSize)
GdipGraphics g.FromImage(b)
str s
int x
foreach s iconFiles
	int hi=GetFileIcon(s)
	GdipBitmap im.FromHICON(hi)
	DestroyIcon hi
	 im.Save(pngFile); ret
	_hresult=GDIP.GdipDrawImageI(g im x 0)
	if(_hresult) end "failed"
	x+imageSize

b.Save(pngFile)
