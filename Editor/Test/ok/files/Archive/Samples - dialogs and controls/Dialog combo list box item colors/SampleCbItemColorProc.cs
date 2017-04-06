 /
function# CBITEMCOLOR&c

 This is a sample callback function that can be used with CB_ItemColor.
 Shows how to set combo/list box item background and text colors depending on item text.
 The function will be called for each item when displaying it.
 c contains various info that can be useful to draw the item.
 To set colors, change c.bkColor and/or c.textColor.
 Also you can change c.dtFlags and c.text.

 c.text - item text. In/out.
 c.bkColor - item background color. In/out.
 c.textColor - item text color. In/out.
 c.hwnd - control handle.
 c.item - item index.
 c.itemData - item data. To set item data, use message CB_SETITEMDATA or LB_SETITEMDATA, documented in MSDN.
 c.selected - 1 if the item is selected, 0 if not.
 c.isLB - 1 if the control is listbox, 0 if combobox.
 c.dtFlags - DrawText flags. Documented in the MSDN Library on the Internet. In/out.
 c.param - an user-defined value, passed to CB_ItemColor.


if(c.selected) ret

sel c.text
	case "yellow background" c.bkColor=0xa0ffff
	case "blue text" c.textColor=0xff0000
	case "black & green" c.bkColor=0; c.textColor=0x00ff00
