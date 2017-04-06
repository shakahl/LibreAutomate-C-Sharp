out
str sf.expandpath(F"$my qm$\{__FUNCTION__}.bmp")
str s.getfile(sf)
out s.len

s.encrypt(32)
out s.len

__GdiHandle hb=LoadPictureFile(sf)

#compile "__Gdip"
GdipBitmap im
if(!im.FromHBITMAP(hb)) end "error"
if(!im.Save("$my qm$\test\gdip.png")) end "error"
_s.getfile("$my qm$\test\gdip.png"); out _s.len

typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.SetPicture(BitmapHandleToIPicture(hb))
g.SaveFormat=GflAx.AX_PNG ;;use other constants to save in other formats
g.SaveBitmap(_s.expandpath("$my qm$\test\gflax.png"))
_s.getfile("$my qm$\test\gflax.png"); out _s.len

#ret
 treeview
61038
6598
5105
5062
 jungle
54854
55058
56761
34975
 desktop icon
7974
5585
5231
4040
 Word toolbar
7782
2314
1808
1733
 QM forum button "newtopic"
8566
1785
1422
1785
 other web button
18034
2723
2129
2305
 other web button
2630
1252
917
1470
 cPanel icon (web page)
2322
2270
2441
1984
