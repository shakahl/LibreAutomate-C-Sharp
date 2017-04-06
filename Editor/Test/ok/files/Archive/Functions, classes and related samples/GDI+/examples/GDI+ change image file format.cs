#compile "__Gdip"
GdipImage im
if(!im.FromFile("q:\test\app_55.png")) ret
out im.Save("$desktop$\app_55.jpg")
