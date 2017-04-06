 \Dialog_Editor
function [__u1] [_flags] [_favAction] [__u2] ;;flags: 1 not QM-owned

__MemBmp t_mb
int t_hwnd t_captured t_action t_image t_searchIn t_isClient t_isMask t_isIcon32 t_isMulti t_multiAll

sub.Dialog_Main 0 _flags _favAction 0


#sub Dialog_Main v
function# hDlg message wParam lParam
if(hDlg) goto messages

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 418 188 "Find Image" "0 10"
 23 ComboBox 0x54230243 0x0 4 4 154 213 "act" "Action"
 21 ComboBox 0x54230243 0x0 4 20 154 213 "ima" "Type and storage of the image to find"
 4 Edit 0x54030080 0x204 4 36 154 13 "fil" "Image resource, file or color, depending on the above.[]If empty, a suggested name will be created."
 6 Button 0x54032000 0x4 4 54 58 14 "Capture" "Take the image or color now from screen.[]On OK the image will be saved as resource or file."
 5 Button 0x54032000 0x4 64 54 58 14 "Use existing..." "Use an existing image resource or file"
 18 Button 0x44012003 0x0 124 54 34 12 "No SF" "Let the button give me normal path, not special folder name"
 3 Static 0x5400100D 0x20004 4 74 154 40 ""
 14 Static 0x44000000 0x4 4 124 90 13 "Timeout, s"
 15 Edit 0x44030080 0x204 110 122 48 15 "wai"
 25 Button 0x54012003 0x0 4 130 130 13 "Error if not found" "Uncheck if you want to use code like this: if(scan(...)) ... else ..."
 24 Button 0x54012003 0x0 4 142 130 12 "Move mouse to the found image"
 7 QM_Tools 0x54030000 0x10000 174 4 240 54 "1 0x1000"
 8 Button 0x54012006 0x0 176 68 114 13 "Window can be in background" "How to get pixel colors from the search area:[]Unchecked - get from screen. Slow if Aero enabled (Windows Vista/7/8/10).[]Checked - get from window. Fast if Aero enabled, else slow and may flicker.[]Indeterminate - get from window if Aero enabled, else from screen. Fast.[][]Aero is <1> on this computer."
 12 Button 0x5C032000 0x0 292 68 48 13 "View pixels" "View pixels that QM gets from the window/control when 'background' is checked.[]QM cannot get pixels from some windows. Then all pixels usually are black.[]If Aero not enabled or the window is DPI-scaled, cannot get pixels from window[]parts covered by other windows."
 27 Static 0x54000000 0x0 176 86 110 12 "Allowed color difference, 0-255"
 31 Edit 0x54030080 0x200 292 84 48 13 "dif" "Use if the on-screen image at run time may have slightly different colors than the captured image.[]Can be 0-255, but should be as small as possible.[]For icons recommended 8."
 22 ComboBox 0x54230243 0x0 176 100 110 213 "ind"
 32 Edit 0x44030080 0x200 292 100 48 13 "arr" "Use to find all matching images.[]Name of an existing or new ARRAY(RECT) variable."
 26 Edit 0x54030080 0x200 292 100 48 13 "mat" "1-based match index.[]For example, use 2 to find the second matching image."
 10 Static 0x54000000 0x4 176 118 110 12 "RECT variable for results"
 11 Edit 0x54030080 0x204 292 116 48 13 "rec" "Use if you want to get coordinates of the found image, or/and limit the search area.[]Name of an existing or new RECT variable."
 9 Button 0x54012003 0x0 344 116 64 16 "Also sets the search area" "Use the variable, if not empty, to limit the search area"
 19 Button 0x54032000 0x0 176 140 64 14 "More Options..."
 1 Button 0x54030001 0x4 4 169 48 14 "OK"
 2 Button 0x54030000 0x4 54 169 48 14 "Cancel"
 17 Button 0x54032000 0x4 104 169 16 14 "?"
 20 Button 0x54032000 0x0 122 169 48 14 "Test" "Test whether code created by this dialog will work, and how fast"
 16 Static 0x54000000 0x0 176 170 64 13 ""
 13 Static 0x54000010 0x20004 0 161 432 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""
dd.findreplace("<1>" iif(_winver<0x600 "unavailable" iif(!DwmIsCompositionEnabled(&_i)&&_i "enabled" "disabled")) 4)

str s sout; int i test

str controls = "23 21 4 18 15 25 24 7 8 31 22 32 26 11 9"
__strt cb23act cb21ima e4fil c18No e15wai c25Err c24Mov qmt7 c8Win e31dif cb22ind e32arr e26mat e11rec c9Als

TO_FavSel wParam cb23act "Find[]Wait for[]Wait until disappears[]Wait until something changes"
cb21ima="&Image[]Image from a .bmp file[]Icon from an .ico file[]Color"
cb22ind="&Match #[]Array variable for all results"
c24Mov=1; c25Err=1; e32arr="a"; c9Als=1
t_isClient=1

int hwndOwner=iif(message&1 0 _hwndqm)
int style; if(!hwndOwner and GetWinStyle(win 1)&WS_EX_TOPMOST) style|DS_SYSMODAL
if(!ShowDialog(dd &sub.Dialog_Main &controls hwndOwner 0 style)) ret

 gtest

int flags waitFor(t_action)

flags=TO_FlagsFromCheckboxes(0 c25Err 2 c24Mov 1 c9Als 128); flags^128
if(t_isClient) flags|16
if(t_isMask) flags|4
if(t_isIcon32) flags|32
sel(val(c8Win)) case 1 flags|0x1100; case 2 flags|0x1000
if(waitFor) flags~2; if(waitFor>1) flags~1; if(waitFor=3) flags~4|32; e26mat=0; cb22ind=0
if(waitFor=1 and flags&1) flags|0x400
sel(t_image) case [2,3] flags~4
if(t_image=2) flags|64; if(!e31dif.len) out "Info: scan may not find some icons if allowed color difference is 0. Recommended difference 8."
else flags~32
int searchIn; if(!test) searchIn=t_searchIn; if(searchIn=2) flags|0x200; flags~0x1511

if waitFor!3
	if t_captured=1 and !test
		err-
		
		sel t_image
			case 0 ;;save in resource
			if(!sub_scan.BitmapToResource(t_mb.bm e4fil)) goto gfs
			
			case 1 ;;save in file
			if(!e4fil.len) e4fil.getmacro("" 1)
			if(!e4fil.endi(".bmp")) e4fil.s+".bmp"
			s.expandpath(e4fil)
			i=findc(s '\')+1; if(!i) s-"$my qm$\"
			if(FileExists(s))
				sel(sub_sys.MsgBox(_hwndqm F"{s}[][]This file already exists. Delete?[][]Click No to make unique filename." "" "YNC!"))
					case 'Y' del s
					case 'C' ret
					case 'N' s.UniqueFileName; _s.getfilename(s 1); e4fil.fix(i); e4fil.s+_s
			if(!SaveBitmap(t_mb.bm s)) goto gfs
			out F"<>Info: the image has been saved in {sub_to.FormatLinkToSelectFileInExplorer(s)}"
		
		err+
			 gfs
			sub_sys.MsgBox _hwndqm "Failed to save."; ret
	
	sel t_image
		case 0 sel(e4fil.s 2) case ["image:*","resource:*"] case else if(t_captured=1) e4fil.s-"image:"
		case 3 e4fil.s-"color:"

str winVar
__strt vd sDecl sFlags

sel searchIn
	case 0 qmt7.Win(winVar "0")
	case 1 winVar="AccVariable"
	case 2 winVar="hBitmap"

int useArr=val(cb22ind); if(!useArr) e26mat.N
if test
	if(t_captured=1) e4fil="_bm"
	else
		if t_image=0 and !e4fil.beg("resource:<")
			e4fil.replacerx("(resource:)?(.+)" F"resource:<{_s.getmacro(`` 1)}>$2" 4)
		e4fil.S
	flags|2|128; flags~0x401; e11rec="r"; waitFor=0; if(useArr) e26mat=0
else 
	if(waitFor=3) e4fil="''''"; t_isMulti=0; else int fileIsVar; e4fil.S("???" fileIsVar)
	sDecl.VD("RECT 0[]" e11rec)
	if e11rec!0
		if(flags&128=0) sout=F" sample code, shows how to set the RECT variable to limit the search area[]SetRect &{e11rec} 50 50 100 100 ;;coordinates must be relative to the main search area - screen, window/control, window/control client area, or accessible object"
		sout.addline(F" sample code, shows how to use the RECT variable containing results[]out ''%i %i'' {e11rec}.left {e11rec}.top ;;screen coordinates" 1)
	if useArr
		e26mat=e32arr; sDecl.s+vd.VD("ARRAY(RECT) a[]" e26mat)
		sout.addline(F" sample code, shows how to use the array[]int i[]for i 0 {e26mat}.len[][9]RECT& rr={e26mat}[i][][9]mou rr.left rr.top[][9]0.5" 1)
	if t_isMulti
		e4fil.from(iif(fileIsVar F"F''{{{e4fil}}" e4fil.rtrim("''")) "[91]]add[91]]more[91]]images[91]]here''")
		if(t_multiAll) flags|0x800

s=F"scan {e4fil} {winVar} {e11rec} {sFlags.Flags(flags)} {e31dif.N} {e26mat}"
sub_to.Trim s " 0 0 0 0 0"

if(waitFor) s=F"wait {e15wai.N} {`-`+(waitFor=1)}S{s+4}"
else if(flags&2=0) s=F"if(scan({s+5}))[][9]"

sel searchIn
	case 0 qmt7.WinEnd(s)
	 case 1
	case 2 s-"__GdiHandle hBitmap[]hBitmap.Delete; hBitmap=LoadPictureFile(''file'')[]"

if test
	OsdHide "TO_Scan"
	ret sub_to.Test(hDlg s iif(e4fil="_bm" t_mb.bm 0) 0 16)

s-sDecl

 sub_to.TestDialog s
act _hwndqm; err
InsertStatement s
if(sout.len) out "<><code>%s</code>" sout

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "23[]$qm$\image.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	QmRegisterDropTarget id(4 hDlg) hDlg 48
	sub.SetNameCueBanner hDlg
	
	case WM_COMMAND goto messages2
	case WM_QM_DRAGDROP
	if(TO_DropFiles(hDlg +lParam "4")) sub.Open hDlg 1
	
	case WM_APP ;;link click in Resources dialog
	sub.Open hDlg 0 +lParam wParam
	ret DT_Ret(hDlg 1) ;;close Resources dialog
	
	case WM_DRAWITEM
	DRAWITEMSTRUCT& d=+lParam
	FillRect(d.hDC &d.rcItem COLOR_APPWORKSPACE+1)
	if(t_mb.bm) BitBlt(d.hDC 0 0 d.rcItem.right d.rcItem.bottom t_mb.dc 0 0 SRCCOPY)
	
	case __TWN_WINDOWCHANGED if(wParam!t_hwnd) t_hwnd=wParam; if(wParam) PostMessage hDlg WM_APP+5 0 0
	case WM_APP+5 sub_scan.CanGetPixels t_hwnd hDlg 8
ret
 messages2
sel wParam
	case 6 sub.Capture hDlg
	case 5 sub.Open hDlg
	case 17 QmHelp "IDP_SCAN"
	
	case 8 ;;background
	TO_Enable hDlg "12" but(lParam)
	PostMessage hDlg WM_APP+5 0 0 ;;check bad window
	case 12 ;;View pixels
	int fl hw=SendDlgItemMessage(hDlg 7 __TWM_GETCAPTUREDHWND 0 0)
	if(!IsWindow(hw)) mes "Need a window." "" "i"; ret
	fl=t_isClient<<4; sel(IsDlgButtonChecked(hDlg 8)) case 1 fl|0x1100; case 2 fl|0x1000
	
	sub_scan.ViewPixels hw fl
	
	case [CBN_SELENDOK<<16|21,CBN_SELENDOK<<16|23,CBN_SELENDOK<<16|22] sub.HideControls hDlg
	
	case 19 ;;More Options
	if sub.Dialog_MoreOptions(hDlg)
		TO_Show hDlg "7" t_searchIn=0 ;;window/control
		TO_Show hDlg "8" t_searchIn!2 ;;background
		sub.HideControls hDlg ;;hide Mouse if hbitmap
	
	case 20 ;;Test
	if t_action!3 and t_mb.bm
		SendDlgItemMessage hDlg 7 __TWM_SETFLAGS 0x200 1 ;;don't declare var; the control unsets the flag
		DT_GetControls(hDlg &controls)
		test=1; goto gtest
ret 1

#opt nowarnings 1


#sub HideControls v
function hDlg

int ac=TO_Selected(hDlg 23)
int im=TO_Selected(hDlg 21)
TO_Show hDlg "3 4 20-22 26 32" ac!3
if ac=3
	TO_Show hDlg "5 6 18" 0
else
	TO_Show hDlg "-6" im=2 ;;Capture
	TO_Show hDlg "5" im!3 ;;Open
	TO_Show hDlg "18" im=1||im=2 ;;no SF
	TO_Show hDlg "-26 32" TO_Selected(hDlg 22) ;;index/ARRAY
TO_Show hDlg "25" ac=0 ;;error
TO_Show hDlg "24" ac<2&&t_searchIn!2 ;;mm
TO_Show hDlg "14 15" ac ;;time

if im!t_image
	if im>1 or t_image>1 or !t_captured
		t_captured=0
		t_mb.Delete; InvalidateRect id(3 hDlg) 0 0
	TO_SetText "" hDlg 4 4
	sub.SetNameCueBanner hDlg
t_action=ac; t_image=im


#sub SetNameCueBanner
function hDlg
sel TO_Selected(hDlg 21)
	case 0 _s=" Resource name (can be empty)"
	case 3 _s=" Color (0xBBGGRR)"
	case else _s=" File name"
SendDlgItemMessage hDlg 4 EM_SETCUEBANNER 0 @_s


#sub Dialog_MoreOptions v
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x90C802C8 0x0 80 30 240 112 "Find Image - More Options" "0 20"
 3 Static 0x54000000 0x0 8 10 54 12 "Search in"
 4 ComboBox 0x54230243 0x0 64 8 168 213 ""
 28 Button 0x54012003 0x0 64 24 168 10 "Client area (the 'Capture' tool sets this)" "Don't search in window border, caption, standard menubar and scrollbars."
 29 Button 0x54012003 0x0 8 40 224 10 "Ignore image pixels of top-left pixel color" "When searching, don't compare image pixels that have color of the top-left pixel.[]Tip: You can save to a .bmp file and edit in Paint or other program."
 30 Button 0x44012003 0x0 8 40 70 10 "Use 32x32 icon"
 5 Button 0x54012003 0x0 8 56 224 10 "Use a list of images; this image is the first"
 6 ComboBox 0x5C230243 0x0 64 68 168 213 ""
 1 Button 0x54030001 0x4 8 92 48 14 "OK"
 2 Button 0x54030000 0x4 60 92 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "4 28 29 30 5 6"
str cb4 c28Cli c29Ign c30Use c5Use cb6
cb4="The window, control or screen[]An accessible object (Acc variable)"; if(t_action=0) cb4+"[]An image file (bmp,png,jpg,gif) or handle"
c28Cli=t_isClient
c29Ign=t_isMask
c30Use=t_isIcon32
c5Use=t_isMulti; cb6="Find any image and return its 1-based index[]Must find all images"
if(!ShowDialog(dd &sub.DlgProc_MoreOptions &controls hwndOwner 0x100)) ret
t_searchIn=val(cb4)
t_isClient=val(c28Cli)
t_isMask=val(c29Ign)
t_isIcon32=val(c30Use)
t_isMulti=val(c5Use); t_multiAll+val(cb6)

ret 1


#sub DlgProc_MoreOptions v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	CB_SelectItem id(4 hDlg) t_searchIn
	CB_SelectItem id(6 hDlg) t_multiAll
	if t_action=3
		TO_Show hDlg "29 30 5" 0
	else
		TO_Show hDlg "29" t_image<2 ;;mask
		TO_Show hDlg "30" t_image=2 ;;32 icon
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_SELENDOK<<16|4
	_i=CB_SelectedItem(lParam)
	TO_Show hDlg "28" _i!2
	
	case 5
	TO_Enable hDlg "6" but(lParam)
ret 1


#sub Capture v
function hDlg

int color
str s
RECT r

__MinimizeDialog _m.Minimize(hDlg 1); 0.5

if(!CaptureImageOrColor(color (t_image=3) 0 "" r)) ret

if t_image=3 ;;color
	t_captured=2
	RECT rc; GetClientRect id(3 hDlg) &rc
	t_mb.Create(rc.right rc.bottom)
	__GdiHandle b=CreateSolidBrush(color); FillRect t_mb.dc &rc b
	s.format("0x%06X" color)
else
	t_captured=1
	t_mb.Attach(color)
	s.getwintext(id(4 hDlg))
	sel t_image
		case 0 ;;resource
		 if(s.len and !findrx(s "^h[0-9A-F]+$")) s=""
		s="" ;;clear always, eg may be "resource:...". User can Undo if need.
		case 1 ;;bmp file
		if(!s.len) s.getmacro("" 1)
		if(!s.endi(".bmp")) s+".bmp"

TO_SetText s hDlg 4 4

InvalidateRect id(3 hDlg) 0 0

 fill window selector control
int h=SendDlgItemMessage(hDlg 7 __TWM_DRAGDROP 1 &r)
if(!h or h=-1) ret

 use control, window or screen?
int i hh
if(t_image=3) hh=h; else POINT pp.x=r.right; pp.y=r.bottom; hh=child(pp.x pp.y 0); if(!hh) hh=win(pp.x pp.y)
if(hh=h) i=1+IsChildWindow(h); else h=GetAncestor(h 2); i=GetAncestor(hh 2)=h
SendDlgItemMessage hDlg 7 __TWM_SELECT i 0

 client area?
if i
	DpiGetWindowRect h &rc 8
	if(t_image=3) t_isClient=PtInRect(&rc r.left r.top); else t_isClient=sub_to.IsRectInRect(rc r)


#sub Open v
function hDlg [!onDrop] [$resName] [resMacro]

str s ss

if onDrop
	s.getwintext(id(4 hDlg))
	int im=iif(s.endi(".bmp") 1 2)
	if(im!t_image) CB_SelectItem id(21 hDlg) im

if resName ;;resource - on link click in Resources dialog
	if(t_image) CB_SelectItem id(21 hDlg) 0
	t_mb.Attach(LoadPictureFile(resName))
	s=resName
	int iid=qmitem
	if(iid!resMacro)
	else if(!findrx(resName "^(resource:<.+?>)image:" 0 0 _i 1)) s.get(s _i)
	else s.replacerx("<.+?>" "" 4)
else if t_image=0 ;;resource - open Resources dialog
	__ResourcesDialog hDlg WM_APP
	ret
else
	int isBmp=t_image=1
	if !onDrop
		if(!sub_to.FileDialog(hDlg 0 "scandir" "$my qm$" iif(isBmp "Bitmap files[]*.bmp[]" "Icon files[]*.ico[]All files (get icon)[]*.*[]") iif(isBmp "bmp" "ico") s)) ret
	ss.expandpath(s)
	
	if isBmp
		int bm=LoadImageW(0 @ss IMAGE_BITMAP 0 0 LR_LOADFROMFILE); if(!bm) bee; ret
		t_mb.Attach(bm)
	else
		__Hicon hi=GetFileIcon(ss); if(!hi) bee; ret
		t_mb.Create(16 16)
		RECT r; r.right=16; r.bottom=16
		FillRect t_mb.dc &r COLOR_BTNFACE+1
		DrawIconEx(t_mb.dc 0 0 hi 16 16 0 0 DI_NORMAL)
	
	if(s.begi("$my qm$\")) s.get(s 8) ;;better compatible with exe

TO_SetText s hDlg 4

t_captured=0
InvalidateRect id(3 hDlg) 0 0
err+


 test code
#ret
function hDlg ~statement bm

__GdiHandle _bm
if(bm) _bm=CopyImage(bm 0 0 0 0)

spe 100
__MinimizeDialog _m.Minimize(hDlg 1); 0.5
 gRetry
RECT r
#ret

__OnScreenRect osr
InflateRect &r 3 3
int i
for(i 0 6) osr.Show(i&1*3 r); 0.25

err+
	_s="[][]<a id=''100''>Retry with Shift</a>"
	if(_error.code=ERRC_OBJECT) i=mes(F"<>Image not found.{_s}" "Test - not found" "!")
	else i=mes(F"<>{_error.description}{_s}" "Test - error" "!")
	sel i
		case 100
		sub_to.WaitForShift "Press Shift when ready"
		goto gRetry
