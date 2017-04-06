 \Dialog_Editor
type ___ILE
	hmain hlv hcb
	__IImageLibrary'ilib
	~filename ~filexml ~filebmp ~undoxml ~undobmp ~folder
	BSTR'dispB0 dispI0 BSTR'dispB1 dispI1
	!loaded
___ILE z

str dd=
 BEGIN DIALOG
 1 "" 0x90CC0A48 0x0 0 0 330 214 "ImageList Editor"
 3 SysListView32 0x54031049 0x200 0 20 330 194 ""
 8 ComboBox 0x54230343 0x0 2 2 96 213 "" "Create or edit an imagelist.[]The list shows files in current imagelists folder. You can change it in More Actions."
 10 Static 0x54000000 0x0 102 4 60 14 ""
 5 Button 0x5C032000 0x0 180 2 42 14 "Add" "Add icon from the Icons dialog."
 4 Button 0x5C032000 0x0 222 2 42 14 "Edit" "Cut/Copy/Paste icons in the imagelist.[]Or you can right click an icon."
 6 Button 0x54032000 0x0 264 2 66 14 "More Actions"
 END DIALOG

if(!ShowDialog(dd &sub.Dialog 0 _hwndqm)) ret


#sub Dialog v
function# hDlg message wParam lParam

if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\image.ico[]ImageListEditor")) ret wParam
sel message
	case WM_INITDIALOG
	sub.Init hDlg
	TO_ButtonsAddArrow hDlg "4 6"
	DT_SetAutoSizeControls hDlg "3s"
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	case WM_QM_DRAGDROP
	_i=z.ilib.Count
	QMDRAGDROPINFO& di=+lParam
	foreach(_s di.files) z.ilib.AddIcon(_s); err
	sub.Save _i; err
ret
 messages2
sel wParam
	case CBN_DROPDOWN<<16|8
	sub.FillCB
	
	case CBN_SELENDOK<<16|8
	_i=CB_SelectedItem(lParam)
	CB_GetItemText(lParam _i _s)
	if(_s.begi(" <N"))
		SendMessage lParam CB_SETCURSEL CB_FindItem(lParam z.filename 0 1) 0
		sub.New(_s)
	else sub.OpenOrClose(_s)
	SetFocus z.hlv
	
	case CBN_SELENDCANCEL<<16|8
	PostMessage lParam CB_SETCURSEL CB_FindItem(lParam z.filename 0 1) 0
	SetFocus z.hlv
	
	case 5
	sub.AddOrReplace
	SetFocus z.hlv
	
	case 4
	sub.MenuEdit
	SetFocus z.hlv
	
	case 6
	sub.MenuManage
	SetFocus z.hlv
	
ret 1
 messages3
NMHDR* nh=+lParam
sel(nh.code)
	case LVN_GETDISPINFOW
	if(!z.loaded) ret
	NMLVDISPINFOW& ndi=+nh
	LVITEMW& li=ndi.item
	int i=li.iItem
	if(li.mask&LVIF_TEXT)
		sel li.iSubItem
			case 0
			if(i!=z.dispI0) z.dispI0=i; z.dispB0=z.ilib.ImagePath(i)
			li.pszText=z.dispB0
			case 1
			if(i!=z.dispI1) z.dispI1=i; _s=li.iItem; z.dispB1=_s
			li.pszText=z.dispB1
	if(li.mask&LVIF_IMAGE) li.iImage=li.iItem
	
	 case LVN_ITEMCHANGED
	case LVN_KEYDOWN
	if(!z.loaded) ret
	NMLVKEYDOWN& nk=+nh
	sel GetMod
		case 0
		if(nk.wVKey=VK_DELETE) sub.Edit 1
		case 2
		sel(nk.wVKey) case 'X' _i=2; case 'C' _i=3; case 'V' _i=4; case else ret
		sub.Edit _i
	case NM_RCLICK
	sub.MenuEdit


#sub Init v
function hDlg

z.hmain=hDlg
z.ilib=__CreateImageLibrary
rget z.folder "folder" "\ILE" 0 "$my qm$\imagelists"

z.hcb=id(8 z.hmain)
z.hlv=id(3 z.hmain)
int es=LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP
SendMessage z.hlv LVM_SETEXTENDEDLISTVIEWSTYLE es es
TO_LvAddCol z.hlv 0 "File" 90
TO_LvAddCol z.hlv 1 "#" 10
TO_LvAdjustColumnWidth z.hlv 2

QmRegisterDropTarget z.hlv z.hmain 16|32


#sub New v
function# ~name

str controls = "3 6"
str e3nam e6siz

e3nam.getmacro("" 1)
e6siz=16
 g1
lpstr dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 239 87 "New imagelist"
 4 Static 0x54000000 0x0 2 4 26 10 "Name"
 3 Edit 0x54030080 0x200 30 2 74 14 "nam"
 5 Static 0x54000000 0x0 120 4 94 10 "Width and height of images"
 6 Edit 0x54032000 0x200 216 2 18 14 "siz"
 1 Button 0x54030001 0x4 4 70 48 14 "OK"
 2 Button 0x54030000 0x4 54 70 48 14 "Cancel"
 7 QM_DlgInfo 0x54000000 0x20000 2 22 234 42 ""
 END DIALOG

if(!ShowDialog(dd &sub.NewDlg &controls z.hmain)) ret
if(!e3nam.len) goto g1
err-

e3nam.ReplaceInvalidFilenameCharacters("_")

mkdir z.folder
str sfxml sfbmp
sfxml.format("%s\%s.xml" z.folder e3nam)
sfbmp.format("%s\%s.bmp" z.folder e3nam); sfbmp.expandpath(sfbmp 2)
if(FileExists(sfxml) or FileExists(sfbmp)) mes "Already exists. Use different name."; goto g1

z.ilib.CreateNew(val(e6siz) sfxml sfbmp)
sub.FillCB
CB_SelectString z.hcb e3nam ;;open the new imagelist

ret 1

err+
	mes F"Failed. {_error.description}" "" "!"
	goto g1


#sub NewDlg v
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG goto g1
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3
	 g1
	str s.getwintext(id(3 hDlg))
	str ss.format("Will be created these files:[]%s\%s.xml[]%s\%s.bmp[]The xml file is used only by the imagelist editor." z.folder s z.folder s)
	ss.setwintext(id(7 hDlg))
ret 1


#sub OpenOrClose v
function [$name]

 out "open %s" name

SendMessage z.hlv LVM_SETITEMCOUNT 0 0

z.loaded=!empty(name)
if(z.loaded)
	z.filename=name
	z.filexml.format("%s\%s.xml" z.folder name)
	z.filebmp.format("%s\%s.bmp" z.folder name)
	z.ilib.Load(z.filexml)
	z.undoxml.getfile(z.filexml)
	z.undobmp.getfile(z.filebmp); err
	int il=z.ilib.GetImageList
else ;;just close this
	z.filename.all
	z.filexml.all
	z.filebmp.all
	z.undoxml.all
	z.undobmp.all
	z.ilib=__CreateImageLibrary

SendMessage z.hlv LVM_SETIMAGELIST LVSIL_SMALL il
z.dispI0=-1; z.dispI1=-1
SendMessage z.hlv LVM_SETITEMCOUNT z.ilib.Count 0
TO_LvAdjustColumnWidth z.hlv 2

TO_Enable z.hmain "4 5" z.loaded

err+
	mes F"Failed. {_error.description}" "" "!"
	if(z.loaded) CB_SelectItem z.hcb -1 ;;empty
	else clo z.hmain


#sub AddOrReplace v
function [flags] [img] ;;flags: 1 replace

int hwnd=win("Icons*" "#32770" "qm" 0x9)
lpstr s=+GetProp(hwnd "qm_iconpath")
int i inewicon(-1)

if(empty(s))
	if('O'=mes("Select an icon in the Icons dialog. OK will open the dialog.[][]Other ways to add icons:[]Drag and drop files.[]Copy/paste files.[]Copy/paste list of file paths.[][]Icons are added to the end, except when pasting. Use Cut/Paste to reorder." "" "OCi")) men 2006 _hwndqm ;;Icons...
	ret

i=z.ilib.ImageIndex(s)

if(flags&1=0) ;;add
	if(i>=0) goto g1
	inewicon=z.ilib.AddIcon(s)
else ;;replace
	if(i>=0 and i!=img) goto g1
	inewicon=z.ilib.ReplaceIcon(img s)

sub.Save inewicon

err+
	mes F"Failed. {_error.description}" "" "!"
ret
 g1
mes "The icon already exists in the imagelist." "" "!"


#sub MenuManage v
str s
str sm=
 1 Refresh icons
 2 Undo all...
 3 Delete this imagelist...
 4 Create code
 -
 6 Open imagelists folder
 7 Change imagelists folder...
 -
 9 Convert bitmap to icons...
 -
 11 Help

MenuPopup m.AddItems(sm)
if(!z.loaded) m.DisableItems("1-4")

sel m.Show(z.hmain)
	case 1 ;;Refresh icons
	z.ilib.RefreshIcons
	sub.Save -1
	
	case 2 ;;Undo all
	if('O'!=mes("Are you sure?" "" "!OC")) ret
	z.undoxml.setfile(z.filexml)
	z.undobmp.setfile(z.filebmp)
	z.ilib.Load(z.filexml)
	sub.Save -1
	
	case 3 ;;Delete this imagelist
	if('O'!=mes("Are you sure?" "" "!OC")) ret
	del z.filexml
	del z.filebmp; err
	CB_SelectItem z.hcb -1
	
	case 4 ;;Create code
	sub.Code
	
	case 6 ;;Open imagelists folder
	run z.folder
	
	case 7 ;;Change imagelists folder
	if(!BrowseForFolder(s z.folder 4) or !rset(_s.expandpath(s 2) "folder" "\ILE")) ret
	SendMessage z.hcb CB_RESETCONTENT 0 0
	z.folder=s
	sub.OpenOrClose
	
	case 9 ;;Convert bitmap to icons
	if(!OpenSaveDialog(0 s "bmp[]*.bmp")) ret
	if(!__BitmapToIcons(s "$temp qm$" -1 1)) mes "Failed.[][]The bitmap must contain 1 or more images horizontally, each of equal width and height. Image size must be 8-256, divisible by 8. Colors must be 4, 8, 24 or 32 bit." "" "!"
	
	case 11 ;;Help
	s=
	 Add icons and click 'Create code' in the menu. The variable can be used as imagelist handle.
	;
	 Usually imagelists are used with toolbar, listview, treeview and some other controls. It is documented in the MSDN Library.
	;
	 The variable must exist while the imagelist is used by controls, therefore it should not be local. Make sure that controls don't destroy the imagelist.
	;
	 If you are creating exe, check 'Auto add files...' to add the imagelist to it.
	QmHelp s 0 6

err+
	mes F"Failed. {_error.description}" "" "!"


#sub MenuEdit v
if(!z.loaded) ret
str sm=
 Cut	Ctrl+X
 Copy	Ctrl+C
 Paste	Ctrl+V

MenuPopup m.AddItems(sm 1)
if(SendMessage(z.hlv LVM_GETNEXTITEM -1 LVNI_SELECTED)<0) m.DisableItems("1-2")

int cmd=m.Show(z.hmain)
if(!cmd) ret

sub.Edit cmd+1

err+
	mes F"Failed. {_error.description}" "" "!"


#sub Edit v
function cmd

ARRAY(int) a
int i ok
str s ss

sel cmd
	case [1,2,3] ;;Delete, Cut, Copy
	if(!sub.LvGetSelected(a)) ret
	for(i 0 a.len) s.addline(z.ilib.ImagePath(a[i]))
	if(cmd!1) s.setclip
	if(cmd!3)
		for(i a.len-1 -1 -1) z.ilib.RemoveIcon(a[i])
		sub.Save -1
	
	case 4 ;;Paste
	ss.getclip
	if(!ss.len) ARRAY(str) ac; GetClipboardFiles ac; ss=ac; if(!ss.len) ret
	if(!sub.LvGetSelected(a 2|8)) ret
	int n=z.ilib.Count
	i=iif(a.len a[0] n)
	
	foreach s ss
		z.ilib.InsertIcon(i s); err continue
		i+1; ok=1
	if(ok) sub.Save i

err+
	mes F"Failed. {_error.description}" "" "!"


#sub FillCB v
SendMessage z.hcb CB_RESETCONTENT 0 0
CB_Add(z.hcb " <New...>") ;;must be first in sorted cb
Dir d
str s1.format("%s\*.xml" z.folder) s2
foreach(d s1 FE_Dir)
	s2.getfile(d.FullPath 0 10); err continue
	if(s2!"<imagelib ") continue
	s2=d.FileName; s2.fix(s2.len-4)
	CB_Add(z.hcb s2)


#sub Save v
function inewicon ;;inewicon: if >=0, ensures visible

z.ilib.Save; err end _error

z.dispI0=-1; z.dispI1=-1
SendMessage z.hlv LVM_SETITEMCOUNT z.ilib.Count 0
TO_LvAdjustColumnWidth z.hlv 2
if(inewicon>=0) SendMessage z.hlv LVM_ENSUREVISIBLE inewicon 0

err+ end _error


#sub LvGetSelected v
function! ARRAY(int)&a [flags] ;;flags: 2 err if multi, 4 err if empty, 8 ret 1 if empty

a=0
int i=-1
rep
	i=SendMessage(z.hlv LVM_GETNEXTITEM i LVNI_SELECTED)
	if(i<0) break
	a[]=i

if((flags&2 and a.len>1) or (flags&4 and !a.len)) mes "Please select 1 image." "" "!"; ret
ret a.len!0 or flags&8


#sub Code v
str s ss sp
int i
ARRAY(str) a

 loading code

s.expandpath(z.filebmp 2)
i=Crc32(s s.len)&0x7fff
ss.format(" Imagelist loading code. Add under case WM_INITDIALOG.[][9]__ImageList- t_imagelist.Load('':%i %s'')" i s)

 image constants

sp.getfilename(z.filebmp)
sp.ucase
sp.replacerx("[^0-9_A-Z]" "_"); if(isdigit(sp[0])) sp-"I"

for i 0 z.ilib.Count
	s.getfilename(z.ilib.ImagePath(i))
	s.ucase
	s.replacerx("[^0-9_A-Z]" "_")
	a[]=s

sel GetMod
	case 0 ;;in QM
	ss+="[][] Image constants"
	for i 0 a.len
		ss.formata("[]def %s_%s %i" sp a[i] i)
	
	case 2 ;;in C++
	ss+="[][]//Image constants in C++[]enum ENUM_IMAGES[]{"
	for i 0 a.len
		ss.formata("[][9]%s_%s=%i," sp a[i] i)
	ss+"[]};"

out ss
