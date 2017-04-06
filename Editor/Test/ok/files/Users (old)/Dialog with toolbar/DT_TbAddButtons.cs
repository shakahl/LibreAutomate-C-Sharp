 /
function hwndTb firstBtnId $stringList $bitmapFile [styles] [flags] [maskColor] [ARRAY(int)&iconHandles] [defaultIcon] ;;flags: 1 icons

 Adds buttons to ToolbarWindow32 control.
 Call this function from dialog procedure, on WM_INITDIALOG. On WM_DESTROY, call DT_TbOnDestroy.

 hwndTb - toolbar control handle.
 firstBtnId - id of first button. Used for notifications in the same way as id of Button control.
 stringList - list of button label strings.
   Empty lines also add buttons.
   - adds separator. Separator size can be specified after -. Separators don't have id and image.
 bitmapFile - bmp file containing horizontally aligned 16x16 images. Color maskColor will be transparent.
 styles - toolbar styles to add.
 flags - 1 - bitmapFile is list of icon files.
 maskColor - color in bmp file that will be transparent. Default or 0 - magenta (0xff00ff).
 iconHandles - array that contains icon handles. If used and not 0, bitmapFile and maskColor are ignored. You can destroy icons immediately after calling this function.


if(styles) SetWinStyle hwndTb styles 1

int nBtn=numlines(stringList)

 create image list

int il hi j
str s
if(!defaultIcon) defaultIcon=_dialogicon
if(&iconHandles)
	il=ImageList_Create(16 16 WINAPI.ILC_MASK|WINAPI.ILC_COLOR32 0 iconHandles.len)
	for j 0 iconHandles.len
		hi=iconHandles[j]
		if(!hi) hi=defaultIcon
		ImageList_ReplaceIcon(il -1 hi)
else if(flags&1)
	il=ImageList_Create(16 16 WINAPI.ILC_MASK|WINAPI.ILC_COLOR32 0 nBtn)
	foreach s bitmapFile
		hi=GetIcon(s)
		if(!hi) hi=defaultIcon
		ImageList_ReplaceIcon(il -1 hi)
		if(hi!=defaultIcon) DestroyIcon hi
else
	if(!maskColor) maskColor=0xFF00FF
	il=ImageList_LoadImage(0 _s.expandpath(bitmapFile) 16 0 maskColor IMAGE_BITMAP LR_LOADFROMFILE)
SendMessage hwndTb WINAPI.TB_SETIMAGELIST 0 il

 create array of TBBUTTON structures

type TBBUTTON iBitmap idCommand !fsState !fsStyle !bReserved[2] dwData iString

ARRAY(TBBUTTON) a.create(nBtn)
int i ii
foreach s stringList
	TBBUTTON& t=a[i]
	if(s.beg("-"))
		t.fsStyle=WINAPI.TBSTYLE_SEP ;;separator
		t.iBitmap=val(s+1)
	else
		t.idCommand=firstBtnId+ii
		t.iBitmap=ii
		if(s.len) t.iString=SendMessage(hwndTb TB_ADDSTRINGA 0 s) ;;note: the string must be terminated with two 0. str variables normally have two 0, but if you will use unicode...
		else t.iString=-1
		t.fsState=WINAPI.TBSTATE_ENABLED
		ii+1
	i+1

SendMessage(hwndTb WINAPI.TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
SendMessage(hwndTb WINAPI.TB_ADDBUTTONS a.len &a[0])
