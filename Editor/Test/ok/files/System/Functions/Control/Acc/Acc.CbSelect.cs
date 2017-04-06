function `item [flags] ;;1 always show popup, 2 method2, 4 method3 (Firefox/Chrome), 8 method4

 Selects combo box item.

 item - item text. Exact or with *?. Case insensitive. Or 0-based index.
 flags:
   1 - always show popup list (temporarily).
   2 - use different method. Need this for MS Office toolbar comboboxes and some other non-standard comboboxes, especially if editable. Also try flags 2|1.
   4 - use different method. Faster. Can be used with Firefox, Chrome and some other windows. Works not with all objects.
   8 - use different method with standard combo box. Also you can try flags 8|1.

 REMARKS
 This object must have role COMBOBOX.
 Works not in all windows.
 If does not work, try various flags and flag combinations, for example 1|2.
 Fast and reliable with:
   Standard combo box controls.
   Internet Explorer and IE-based web browsers/controls.
 Slower and less reliable with:
   Firefox, Chrome.
   MS Office dialogs.
   MS Office toolbars. Use flag 2. Window must be active.
   Java windows. Use flags 2 or 2|1 for editable comboboxes.
 Does not work with:
   Windows 7 Wordpad and similar windows.
 In most windows sets focus to the combo box.
 In some windows works only if the window is active.
 The autodelay (spe) may be applied, depending on browser and flags.

 Added in: QM 2.3.3.

 Errors: ERR_INIT, ERR_ITEM, ERR_FAILED (failed to select).


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

int hc isSimpleCB
hc=child(a)
isSimpleCB=!b and SendMessage(hc CB_GETITEMHEIGHT -1 0)
if(b=2 and WinTest(hc "Chrome_*")) b=3

if flags&1 and b!3 ;;in Chrome do later, now clears items
	Select(1); err ;;info: it activates window of standard CB. Works without this, but then activates later.
	if(isSimpleCB) SendMessage(hc CB_SHOWDROPDOWN 1 0); if(flags&8=0) SendMessage(hc CB_SHOWDROPDOWN 0 0) ;;info: DoDefaultAction fails
	else DoDefaultAction
else if isSimpleCB and flags&2=0
	SendMessage(GetParent(hc) WM_COMMAND CBN_DROPDOWN<<16|GetWinId(hc) hc) ;;for auto-fill controls

 get list object
Acc ai aList; str role
ai.Find(a "LISTITEM" "" "" 0x10)
if ai.a
	ai.Navigate("pa" aList)
	role="LISTITEM"
else ;;maybe "list/label" in Java. Or 0 items, or not combobox.
	aList.Find(a "list" "" "" 0x10)
	if(!aList.a) aList=this

STRINT si
sel item.vt
	case VT_I4
	si.i=item.lVal+1
	ai.Find(aList.a role "" "" 0x50 0 si.i)
	
	case VT_BSTR
	si.s=item
	ai.Find(aList.a role "" "" 0x50 0 0 "" &sub.Callback &si) ;;info: callback gets item index
	
	case else end ERR_BADARG
if(!ai.a) end ERR_ITEM

if isSimpleCB and flags&8=0
	CB_SelectItem hc si.i-1
	ret

if(flags&4) ai.DoDefaultAction; ret

if flags&1
	if b=3
		Select(1); err
		DoDefaultAction ;;tested: Chrome sometimes changes STATE_SYSTEM_COLLAPSED, sometimes not
else
	Select(1); err
	if(flags&2=0 and !b) DoDefaultAction; err flags|2 ;;show popup list. In Office would defocus instead.

 keys
if(flags&2=0) sub.PostKey hc VK_HOME; else rep(aList.ChildCount) sub.PostKey hc VK_UP ;;Home does not work in Office toolbars
rep(si.i-1) sub.PostKey hc VK_DOWN

 need Enter to close popup (flag 1) or submit
if flags&1=0 and !isSimpleCB
	if(b!3) flags|1 ;;in Chrome Enter shows CB list
if flags&1
	if(b=3) 0.1 ;;in Chrome async, may not close if too early
	sub.PostKey hc VK_RETURN

rep(2) SendMessage hc 0 0 0; 0.01
wait -2

err+ end ERR_FAILED


#sub Callback
function# Acc&ai level STRINT&r

r.i+1
str s=ai.Name
if(!s.len and r.s.len) s=ai.Value ;;Chrome
ret !matchw(s r.s 1)
err ret 1


#sub PostKey
function h vk
PostMessage h WM_KEYDOWN vk 0
PostMessage h WM_KEYUP vk 0
 tested: SendMessage does not work. PostMessage works better than key.


 notes:
 ai.DoDefaultAction etc don't work. In FF selects, but does notify FF; then keyboard navigation does not work.
 In windows other than IE/FF/Chrome/simpleCB:
   In most windows works well only with popup list (this.DoDefaultAction).
 In FF could post first char several times. Faster but not reliable. Or could post PageDown, but difficult.

 For listboxes:
 With simple listbox and IE, can use Select().
 In Firefox Select() does not work. To select single item, can use DoDefaultAction or this func.
