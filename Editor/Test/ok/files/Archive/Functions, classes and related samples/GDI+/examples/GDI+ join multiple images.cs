#compile "__Gdip"

 get list of files
ARRAY(str) a
GetFilesInFolder a "Q:\test" "\.(bmp|png|gif|jpg|jpeg|ico)$" 0x10000
 out af; ret

 calculate dimensions of destination image
int i w h width height
GdipImage im ;;used for source images
ARRAY(int) aa.create(a.len) ;;used for positions of images
for i 0 a.len
	if(!im.FromFile(a[i])) out "failed to load: %s" a[i]; continue
	aa[i]=height
	w=im.width; if(w>width) width=w
	h=im.height; height+h

 create empty destination image
GdipBitmap b.CreateEmpty(width height) ;;used to draw images and save
GdipGraphics g.FromImage(b) ;;need for GDIP functions

 draw images
for i 0 a.len
	if(w*h=0) continue ;;if failed to load above
	if(!im.FromFile(a[i])) out "failed to load: %s" a[i]; continue
	GDIP.GdipDrawImageI(g im 0 aa[i])

 save
if(!b.Save("$desktop$\test55.png")) end "saving"

 see what we have
run "$desktop$\test55.png"
