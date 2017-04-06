 int h=LoadPictureFile("$my qm$\macro150.bmp" 0)
 int h=LoadPictureFile("$my pictures$\avatar_waterdrops.jpg" 0)
int h=LoadPictureFile("$my pictures$\avatar_ledynmetis.gif" 0)
out h
out SaveBitmap(h "$my qm$\macro150 - 3.bmp")
DeleteObject(h)
