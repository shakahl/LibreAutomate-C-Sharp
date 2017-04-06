 /
function `item flags ;;flags: 1 focus, 2 select, 4 extend selection, 8 add to selection, 16 unselect.

 Selects list item.

 item - item text. Exact or with *?. Case insensitive. Or 0-based index.

 NOTES
 This object must have role LIST.
 Works not in all windows.
 Works with:
   Standard list box and list view controls.
   Internet Explorer and IE-based web browsers/controls.
   Firefox. Can be used only flag 2.

 Errors: ERR_INIT, ERR_ITEM, ERR_FAILED (failed to select).

 Added in QM 2.3.3.


if(!a) end ERR_INIT

Acc ai
sel item.vt
	case VT_I4
	ai.Find(a "LISTITEM" "" "" 0x10 0 item.lVal+1)
	
	case VT_BSTR
	ai.Find(a "LISTITEM" item "" 0x11)

if(!ai.a) end ERR_ITEM

if flags&31=2
	FFNode f.FromAcc(this); err
	if f
		ai.DoDefaultAction
		ret

ai.Select(flags)

err+ end ERR_FAILED

 notes:
 In Firefox Select() does not work.
