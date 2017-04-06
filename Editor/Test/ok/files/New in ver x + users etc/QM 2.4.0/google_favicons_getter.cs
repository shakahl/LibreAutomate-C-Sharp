\Dialog_Editor
str controls = "3"
str sb3
sb3="$temp qm$\favicon.png"
IntGetFile("http://www.google.com/s2/favicons?domain=www.quickmacros.com" sb3 16)

if(!ShowDialog("" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x5400000E 0x0 0 0 16 16 ""
 END DIALOG

#ret
int hsb=id(3 ToolbarHwnd)
int hb hbTemp=LoadPictureFile(FavIconFilePath)
SendMessage hsb STM_SETIMAGE IMAGE_BITMAP hbTemp
hb=SendMessage(hsb STM_GETIMAGE IMAGE_BITMAP 0)
if(hb!=hbTemp) DeleteObject hbTemp ;;the control copies the bitmap if alpha
__GdiHandle- t_favicon; t_favicon.Delete; t_favicon=hb ;;will auto-delete bitmap in dtor
