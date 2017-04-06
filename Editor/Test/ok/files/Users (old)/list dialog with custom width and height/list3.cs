 /Dialog_Editor
function# $items [$text] [$caption] [hwndowner] [x] [y] [lbwidth] [lbheight]

 Shows dialog with list box, similar to the list() function.
 Returns 1-based index of selected item, or 0 on Cancel.

 items - list of items.
 text - text above the list.
 caption - dialog title bar text.
 hwndowner - owner window handle.
 x y - dialog coordinates. If 0 - screen center. If negative, relative to the right or bottom of the work area.
 lbwidth lbheight - listbox control width in dialog box units.
   Dialog box units depend on system font that is used for dialogs, and usually are about 2 times bigger than pixels. Different horizontally and vertically.
   Default (if omitted or 0): 150 75.

 EXAMPLE
 sel list3("one[]two[]three")
	 case 1
	 out 1
	 case 2
	 out 2
	 case 3
	 out 3
	 case else
	 ret


str f=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x108 0 0 %i %i ""
 3 ListBox 0x54230101 0x200 0 24 %i %i ""
 2 Button 0x54030000 0x4 0 %i %i 14 "Cancel"
 4 Edit 0x54030844 0x0 4 4 122 21 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030007 "" "" ""

if(!lbwidth) lbwidth=150
if(!lbheight) lbheight=75
str dd.format(f lbwidth lbheight+24+14 lbwidth lbheight lbheight+24 lbwidth)

str controls = "0 3 4"
str dlg lb3 e4
lb3=items
e4=text
dlg=iif(empty(caption) "QM - Select" caption)
ret ShowDialog(dd &list3_dlg &controls hwndowner 0 0 0 0 x y)
