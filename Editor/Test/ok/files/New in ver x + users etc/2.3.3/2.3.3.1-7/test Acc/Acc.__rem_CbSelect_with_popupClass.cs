 /test Acc CbSelect in FF
function `item [flags] [$popupClass] ;;1 always show popup, 2 method1 (Office), 4 method2 (Firefox), 8 method3 (standard CB)

 Selects combo box item.

 item - item text. Exact or with *?. Case insensitive. Or 0-based index.
 flags:
   1 - always show popup list (temporarily).
   2 - use different method. Need this for MS Office toolbars.
   4 - use faster method. Can be used with Firefox and some other windows. It works not with all objects.
   8 - use different method with standard combo box. Also you can try flags 9.
 popupClass - if used, tries to show popup list and find/select item in it. Use when the combo box itself does not have child accessible objects. If does not work, try flag 4 or 2.

 NOTES
 This object must have role COMBOBOX.
 Works not in all windows.
 Fast and reliable with:
   Standard combo box controls.
   Internet Explorer and IE-based web browsers/controls.
 Slower and less reliable with:
   Firefox.
   MS Office dialogs.
   MS Office toolbars. Use flag 2. Window must be active.
   Opera. Window must be active.
   Windows 7 Wordpad and similar windows. Use flag 4 and popupClass "Net UI Tool Window".
 In most windows sets focus to the combo box.
 In some windows works only if the window is active.

 Errors: ERR_INIT, ERR_ITEM, ERR_FAILED (failed to select).

 Added in QM 2.3.3.


if(!a) end ERR_INIT

Htm e
int b=__HtmlObj(e)

if b=1 ;;IE
	if flags&1
		Acc ab.Find(a "PUSHBUTTON") ;;DoDefaultAction and e.Click don't work
		if(ab.a) ab.DoDefaultAction; ab.DoDefaultAction
	e.CbSelect(item 2)
	err end iif(_error.code=ERRC_ITEM ERR_ITEM ERR_FAILED)
	ret

int hc isSimpleCB isPopup
Acc acb
hc=child(a)

if empty(popupClass)
	acb=this
	isSimpleCB=!b and SendMessage(hc CB_GETITEMHEIGHT -1 0)
	
	if flags&1
		Select(1); err ;;info: it activates window of standard CB. Works without this, but then activates later.
		if(isSimpleCB) SendMessage(hc CB_SHOWDROPDOWN 1 0); if(flags&8=0) SendMessage(hc CB_SHOWDROPDOWN 0 0) ;;info: DoDefaultAction fails
		else DoDefaultAction
	else if(isSimpleCB and flags&2=0) SendMessage(GetParent(hc) WM_COMMAND CBN_DROPDOWN<<16|GetWinId(hc) hc) ;;for auto-fill controls
else
	isPopup=1
	flags|1
	ab.Find(a "PUSHBUTTON")
	if(ab.a) ab.DoDefaultAction; else DoDefaultAction
	acb.FromWindow(wait(10 WV win("" popupClass)))

Acc ai
STRINT si
sel item.vt
	case VT_I4
	si.i=item.lVal+1
	ai.Find(acb.a "LISTITEM" "" "" 0x10 0 si.i)
	
	case VT_BSTR
	si.s=item
	ai.Find(acb.a "LISTITEM" "" "" 0x10 0 0 "" &Acc_CbSelect_callback &si) ;;info: callback gets item index

if(!ai.a) end ERR_ITEM

if isSimpleCB and flags&8=0
	CB_SelectItem hc si.i-1
	ret

if(flags&4) ai.DoDefaultAction; ret

if b ;;FF
	_i=child(0 0 hc 1); if(_i) hc=_i ;;get MozillaWindowClass. Initially hc is MozillaContentWindowClass. In FF4 - main window.

if flags&1=0
	Select(1); err
	if(flags&2=0 and !b) DoDefaultAction; err flags|2 ;;show popup list. In Office would defocus instead.

if(flags&2=0) PostMessage hc WM_KEYDOWN VK_HOME 0; else rep(a.ChildCount) PostMessage hc WM_KEYDOWN VK_UP 0 ;;Home does not work in Office toolbars
rep(si.i-1) PostMessage hc WM_KEYDOWN VK_DOWN 0
if !isSimpleCB or flags&1
	PostMessage hc WM_KEYDOWN VK_RETURN 0
	PostMessage hc WM_KEYUP VK_RETURN 0
 tested: SendMessage does not work. In FF works sometimes, but is not sync.

rep(2) SendMessage hc 0 0 0; 0.01
wait -2

err+ end ERR_FAILED

 notes:
 ai.DoDefaultAction etc don't work. In FF selects, but does notify FF; then keyboard navigation does not work.
 In windows other than IE/FF/simpleCB:
   In most windows works well only with popup list (this.DoDefaultAction).
   Opera: Must be active window. Without popup each time fires event.
   Chrome: COMBOBOX does not have children. Also does not work with popupClass.
 In FF could post first char several times. Faster but not reliable. Or could post PageDown, but difficult.
