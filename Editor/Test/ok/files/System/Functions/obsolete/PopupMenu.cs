 /
function# $items [x] [y] [!*flags] [accel] [hwndOwner]

 Shows popup menu. Returns 1-based index of selected item, or 0.
 Obsolete. Use <help>ShowMenu</help> or class MenuPopup.
 See also: <DynamicMenu>.

 items - list of [] delimited strings. You can add separators and submenus in the same way as in <help #IDH_POPUP>QM popup menus</help>.
 x y - menu position. Default: 0 0 (mouse position).
 flags - byte array, where each element can be combination of the following flags:
   1 disabled
   2 checked
   4 item type is radiocheck
 accel - if 1, you can use accelerator keys (characters, preceded by &). Also can contain TPM_ flags (documented in MSDN, look for TrackPopupMenuEx).
 hwndOwner - owner window. Must belong to current thread.

 bug: flags cannot be used with menus that have submenus.

 EXAMPLES
 sel PopupMenu("Item1[]Item2")
	 case 1 ...
	 case 2 ...
	 case else ret

 str fl.all(6 2 0)
 fl[1]=1
 fl[2]=2
 fl[4]=4
 fl[5]=4|2
 int i=PopupMenu("Normal[]Disabled[]Checked[]-[]Radio[]Radio checked" 0 0 fl)
 
 str s="1[]>2[]3[]4[]<[]>5[]>6[]7[]8[]<[]9"
 out PopupMenu(s 0 0 0 1)


 These flags can be used in accel:
 def TPM_RIGHTBUTTON 0x0002
 def TPM_CENTERALIGN 0x0004
 def TPM_RIGHTALIGN  0x0008
 def TPM_VCENTERALIGN    0x0010
 def TPM_BOTTOMALIGN     0x0020
 def TPM_VERTICAL        0x0040

str s=items
if(s.end("[]")) s.fix(s.len-2)
if(!s.len) ret
int hwnd hf hm r

if(hwndOwner) hwnd=hwndOwner
else
	hf=win
	hwnd=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 hf 0 _hinst 0)
	SetForegroundWindow hf; hf=0

if(x=0 and y=0) xm +&x
if(accel&1)
	if(!hwndOwner) hf=win
	SetForegroundWindow(hwnd)

hm=sub.Create(_i s flags)
r=TrackPopupMenuEx(hm accel~1|TPM_RETURNCMD x y hwnd 0)
DestroyMenu(hm)
if(!hwndOwner) DestroyWindow(hwnd)
0
if(hf) act hf
ret r


#sub Create
function# int&i str&items lpstr'fa [FINDWORDN*getlstate]

str ss.flags=1
lpstr s
int vert hm=CreatePopupMenu()
MENUITEMINFOW mii.cbSize=sizeof(MENUITEMINFOW)
if(getlstate) _getl=*getlstate

for i i 1000000
	vert=0
	 g2
	if(ss.getl(items -i)<0) break
	for(s ss s+ss.len) if(s[0]!9) break ;;skip tabs
	mii.fMask=MIIM_TYPE|MIIM_ID|MIIM_STATE
	mii.fState=0; mii.dwTypeData=0; mii.fType=vert
	sel s[0]
		case '-' mii.fType=MFT_SEPARATOR|vert
		case '|' vert=MFT_MENUBARBREAK; i+1; goto g2
		case '>'
			mii.fMask=MIIM_TYPE|MIIM_SUBMENU|MIIM_STATE
			mii.dwTypeData=@(s+1)
			i+1
			mii.hSubMenu=sub.Create(&i &items fa &_getl)
			goto g1
		case '<' i-1; goto gr
		case else
		mii.dwTypeData=@s
		 g1
		if(fa)
			int flags=fa[i]
			if(flags&1) mii.fState|MFS_DISABLED
			if(flags&2) mii.fState|MFS_CHECKED
			if(flags&4) mii.fType|MFT_RADIOCHECK
	mii.wID=i+1
	InsertMenuItemW(hm 65000 0 &mii)
 gr
if(getlstate) *getlstate=_getl
ret hm
