out
#compile "__Gdip"
__GdipBitmap b
if(!b.Load("q:\test\app_55.png")) ret
 if(!b.Load("$qm$\folder.ico")) ret
 if(!b.Load("$desktop$\app_55.bmp")) ret
 if(!b.Load("$desktop$\app_55.jpg")) ret
 if(!b.Load("$desktop$\app_55.gif")) ret
 if(!b.Load("$desktop$\app_55.tif")) ret

out b.Save("$desktop$\app_55.png")
out b.Save("$desktop$\app_55.bmp")
out b.Save("$desktop$\app_55.jpg")
out b.Save("$desktop$\app_55.gif")
out b.Save("$desktop$\app_55.tif")
 out b.Save("$desktop$\app_55.txt")
