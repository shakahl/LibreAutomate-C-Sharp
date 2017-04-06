 /
function# &colorint [str&colorstr] [ccFlags]

 Shows Color dialog. Returns 1 on OK, 0 on cancel.

 colorint - receives color value.
 colorstr - receives color formatted as string.
 ccFlags - CC_x flags (see function code below, read about in MSDN library). Default or -1: CC_FULLOPEN|CC_RGBINIT.


type CHOOSECOLOR lStructSize hWndOwner hInstance rgbResult *lpCustColors flags lCustData lpfnHook $lpTemplateName
def CC_FULLOPEN      0x2
def CC_RGBINIT       0x1
 def CC_PREVENTFULLOPEN 0x00000004
 def CC_SHOWHELP 0x00000008
 def CC_ENABLEHOOK 0x00000010
 def CC_ENABLETEMPLATE 0x00000020
 def CC_ENABLETEMPLATEHANDLE 0x00000040
 def CC_SOLIDCOLOR 0x00000080
 def CC_ANYCOLOR 0x00000100
dll comdlg32 #ChooseColor CHOOSECOLOR*pChoosecolor

type __MYCOLORS ft c[16]; __MYCOLORS+ __mycolors
if(__mycolors.ft=0) __mycolors.ft=1; for(_i 0 16) __mycolors.c[_i]=0xa0a0a0

CHOOSECOLOR cc.lStructSize=sizeof(CHOOSECOLOR)
if(getopt(nargs)<3 or ccFlags=-1) cc.flags=CC_FULLOPEN|CC_RGBINIT; else cc.flags=ccFlags
cc.lpCustColors=&__mycolors.c
cc.rgbResult=0xa0a0a0
if(ChooseColor(&cc)=0) ret
if(&colorint) colorint=cc.rgbResult
if(&colorstr) colorstr.format("0x%X" cc.rgbResult)
ret 1
