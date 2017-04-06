function [reset] ;;reset: 0 set selected icon, 1 reset icon

 Sets or resets icon of multiple macros.
 Open QM Icons dialog, select an icon and run this macro.
 Click each macro for which want to set the icon.
 To end this macro, click somewhere not in the list of macros.
 Recommended trigger: toolbar attached to the Icons dialog.

OnScreenDisplay "Click each macro in the list of macros in QM window.[]To end, click somewhere not in the list.[]Middle-click items to close." -1 0 0 "" 12 0x800000 1|4|8

int w=win("Icons" "#32770" _hwndqm 32)
if(!w) ret
spe 10
rep
	wait 0 ML
	int c=child(mouse)
	if(!c or !childtest(c "" "" _hwndqm 0 "id=2202")) break
	act id(1050 w)
	'VD(#2+reset)Y
