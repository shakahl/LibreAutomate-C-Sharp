 /
function# int&colorInt [str&colorStr] [hwndOwner]

 Shows Color dialog.
 Returns 1 on OK, 0 on Cancel.

 colorInt - receives color value in format 0xBBGGRR.
 colorStr - receives color formatted as string.
 hwndOwner (QM 2.3.4) - handle of owner window or 0.

 EXAMPLE
 int col
 if(ColorDialog(col))
	 out "0x%X" col


type ___MYCOLORS ft c[16]
___MYCOLORS+ ___mycolors
if(___mycolors.ft=0) ___mycolors.ft=1; for(_i 0 16) ___mycolors.c[_i]=0xa0a0a0

CHOOSECOLOR cc.lStructSize=sizeof(CHOOSECOLOR)
cc.hWndOwner=hwndOwner
cc.flags=CC_FULLOPEN | CC_RGBINIT
cc.lpCustColors=&___mycolors.c
cc.rgbResult=0xa0a0a0
if(!ChooseColor(&cc)) 0; ret
if(&colorInt) colorInt=cc.rgbResult
if(&colorStr) colorStr.format("0x%X" cc.rgbResult)
0
ret 1
