\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Note: This is outdated. After changing __IImageLibrary behavior it does not work. Need to edit something.

 dll "qm.exe" #__ImageListSave il $_file [flags] ;;flags: 1 IStream, 2 export bmp

str controls = "11 13 18 20 22 23 14 25 26 27"
str c11tim e13 c18All e20 e22 c23Non c14All c25Onl e26 e27
e13="c:\users\g\documents\my qm\blue.ico"
e20=1
e22="c:\users\g\documents\my qm\blue err.ico"
e26=16; e27=16
if(!ShowDialog("dlg_image_library" &dlg_image_library &controls _hwndqm)) ret

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 463 391 "Dialog"
 4 Button 0x54032000 0x0 250 4 48 14 "CreateNew"
 5 Button 0x54032000 0x0 250 20 48 14 "AddIcon"
 6 Button 0x54032000 0x0 250 52 48 14 "AddBitmap"
 7 Button 0x54032000 0x0 250 68 48 14 "Refresh"
 8 Button 0x54032000 0x0 250 36 48 14 "ReplaceIcon"
 2 Button 0x54030000 0x4 250 372 48 14 "Close"
 10 Button 0x54032000 0x0 250 232 48 14 "View XML"
 11 Button 0x54012003 0x0 302 70 48 12 "timer"
 12 Button 0x54032000 0x0 250 84 48 14 "Index"
 13 Edit 0x54030080 0x200 302 86 160 12 ""
 16 Button 0x54032000 0x0 250 148 48 14 "Count"
 17 Button 0x54032000 0x0 250 180 48 14 "Speed"
 18 Button 0x54012003 0x0 302 180 48 13 "All"
 19 Static 0x54000000 0x0 354 22 16 12 "rep"
 20 Edit 0x54030080 0x200 372 20 26 14 ""
 3 SysListView32 0x5403500D 0x200 0 0 248 382 ""
 21 Button 0x54032000 0x0 250 100 48 14 "Add error"
 22 Edit 0x54030080 0x200 302 100 160 14 ""
 23 Button 0x54012003 0x0 302 52 60 13 "Nonstandard"
 14 Button 0x54012003 0x0 302 22 48 12 "All"
 15 Button 0x54032000 0x0 250 116 48 14 "Add bmp res"
 24 Button 0x54032000 0x0 250 132 48 14 "Remove"
 25 Button 0x54012003 0x0 354 70 62 12 "Only modified"
 9 Static 0x54000000 0x0 302 4 24 13 "cx, cy"
 26 Edit 0x54030080 0x200 330 4 22 14 ""
 27 Edit 0x54030080 0x200 358 4 22 14 ""
 28 Button 0x54032000 0x0 250 298 48 14 "Misc test"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

 LVS_REPORT|LVS_OWNERDATA|LVS_SHOWSELALWAYS|LVS_SHAREIMAGELISTS

ret
 messages
sel message
	case WM_INITDIALOG
	__IImageLibrary- ilib=__CreateImageLibrary
	DIL_Load hDlg

	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	case WM_TIMER
	sel wParam
		case 1
		int- ti
		int nr=ilib.RefreshIcons(but(25 hDlg) ti 5)
		 out "%i %i %i" ti nr _hresult
		 for ti ti ti+nr
			 
		SendMessage id(3 hDlg) TVM_SETIMAGELIST 0 ilib.Himagelist
		ti+nr
		if(!nr)
			KillTimer hDlg 1
			out "refreshed"
ret
 messages2
int isel=SendMessage(id(3 hDlg) LVM_GETNEXTITEM -1 LVNI_SELECTED)
sel wParam
	case 4 ;;CreateNew
	str sx.getwintext(id(26 hDlg)) sy.getwintext(id(27 hDlg))
	ilib.CreateNew(val(sx) val(sy))
	DIL_Load hDlg 1
	
	case 5 ;;AddIcon
	DIL_AddIcons hDlg
	
	case 8 ;;ReplaceIcon
	ilib.ReplaceIcon(isel "shell32.dll,5") ;;error if in bitmap
	err mes "please select an icon"; ret
	DIL_Load hDlg 1
	
	case 6 ;;AddBitmap
	if(but(23 hDlg)) ilib.AddBitmap("$my qm$\Copy.gif")
	else ilib.AddBitmap("$qm$\toolbar1.bmp" 0xc0c0c0)
	DIL_Load hDlg 1
	
	case 7 ;;Refresh
	if(but(11 hDlg)) ;;use timer
		ti=0
		SetTimer hDlg 1 50 0
	else ;;refresh now
		int t1=perf
		ilib.RefreshIcons(but(25 hDlg))
		int t2=perf
		out t2-t1
		DIL_Load hDlg 1
	
	 this code uses timer
	
	case 10 ;;View XML
	run "$program files$\PSPad editor\PSPad.exe" "$desktop$\image library.xml"
	
	case 12 ;;Index
	_s.getwintext(id(13 hDlg))
	out ilib.ImageIndex(_s)
	
	case 16 ;;Count
	out ilib.Count
	
	case 17 ;;Speed
	int i n=ilib.Count
	lpstr sp
	ARRAY(lpstr) ap.create(n)
	
	if(but(18 hDlg)) ;;All
		out "speed of all images (%i):[]ImagePath, ImageIndex" n
		Q &q
		for(i 0 n) ap[i]=ilib.ImagePath(i)
		Q &qq
		for(i 0 n) ilib.ImageIndex(ap[i])
		Q &qqq
		outq
	else
		n-1
		out "speed of last image (%i):[]ImagePath, ImageIndex" n
		Q &q
		sp=ilib.ImagePath(n)
		Q &qq
		i=ilib.ImageIndex(sp)
		Q &qqq
		outq
		out "%i   %s=%i" n sp i
	
	case 21 ;;Add error
	ilib.AddIcon(_s.getwintext(id(22 hDlg)))
	err out _error.description
	ilib.AddBitmap(_s.getwintext(id(22 hDlg)))
	err out _error.description
	
	case 15 ;;Add bmp res
	ilib.AddBitmap(+129)
	DIL_Load hDlg 1
	
	case 24 ;;Remove
	ilib.RemoveIcon(isel)
	err mes "please select an icon"; ret
	DIL_Load hDlg 1
	
	case 28 ;;misc test
	 int hil=ilib.Himagelist
	  ImageList_Copy hil 2 hil 0 ILCF_SWAP
	 ImageList_Copy hil 2 hil 0 ILCF_MOVE
	 RedrawWindow id(3 hDlg) 0 0 RDW_INVALIDATE
ret 1
 messages3
NMHDR* nh=+lParam
sel(nh.code)
	case LVN_GETDISPINFOW
		NMLVDISPINFOW& di=+nh
		LVITEMW& li=di.item
		if(li.mask&LVIF_TEXT)
			BSTR-- b
			b=ilib.ImagePath(li.iItem)
			li.pszText=b
		if(li.mask&LVIF_IMAGE) li.iImage=li.iItem
