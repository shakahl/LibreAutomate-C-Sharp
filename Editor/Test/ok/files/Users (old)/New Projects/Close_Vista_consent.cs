 List of bitmap files for programs. An image must contain
   the part of the UAC window with program icon and text
   at right of it, including path. To capture images, you
   can use the Find Image dialog from the floating toolbar.
   Edit the list.
str bmps=
 uac - regedit.bmp
 uac - mmc.bmp


int hwnd=val(_command)

 show Details, because it is more secure to check path too
key Ad

 wait a while, because image appears not immediately
0.5

 find matching bitmap
str s
foreach(s bmps) if(scan(s hwnd 0 16)) goto close
ret ;;not found

 close
key A{ca} ;;Alt+C or Alt+A, depending on consent window type
