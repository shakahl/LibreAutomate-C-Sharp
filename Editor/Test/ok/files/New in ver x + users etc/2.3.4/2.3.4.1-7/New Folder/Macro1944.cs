#compile "__Gdip"
GdipImage im
if(!im.FromFile("$program files$\Last.fm\data\app_55.png")) ret
out im.Save("$desktop$\app_55.jpg")
run "$desktop$\app_55.jpg"
