 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

if(!ShowDialog(dd 0 0)) ret

#ret
 Dialog Editor tools:

 + Button
	OK
	Cancel
 + Check
	Option first
	Option Next
 + Static
	Icon
	Bitmap
	Horz line
	Vert line
 + Edit
	Multiline
	Password
	Digits
	Read-only
	Rich edit
 + Combo read-only
	Combo editable
	Combo simple
 + List
	Multisel
	Multicol
	Multicolsel
 Group
 Other...
 ActiveX...
 Web browser
 Grid
 Info
