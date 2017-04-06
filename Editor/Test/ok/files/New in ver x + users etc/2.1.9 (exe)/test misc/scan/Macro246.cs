 mes 2 "" "q"
 ShowDialog("Dialog12")
 _s.getmacro("Dialog12")
 act "hhhhhhhhhhh"

 scan "Macro246.bmp" 0 0 0x3
 int h=LoadImage(0 _s.expandpath("$my qm$\Macro246.bmp") IMAGE_BITMAP 0 0 LR_LOADFROMFILE)
 out h
 scan h 0 0 0x3
 DeleteObject h

 int h=LoadImage(_hinst +133 IMAGE_ICON 0 0 0)
 out h
 scan h 0 0 0x3|64
 DestroyIcon h

 scan ":167" 0 0 0x3 ;;Options -> Triggers
scan ":133" 0 0 0x3|64
 lef
