 \Dialog_Editor

str controls = "1002 1101 1104 1105 1103"
str e1002txt c1101Sho c1104Bal c1105Don e1103tim

___DE_CONTROL& c=subs.GetControl(_hsel)
if c.cid
	e1002txt=c.tooltip
else if(c.tooltip.len)
	str s1 s2
	tok c.tooltip &s1 2 " "
	_i=val(s1); c1101Sho=_i&1; c1104Bal=_i&2!0; c1105Don=_i&4!0; e1103tim=s2

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 403 96 "Tooltip"
 1001 Static 0x54000000 0x0 4 4 48 10 "Text"
 1002 Edit 0x54231044 0x200 4 16 396 50 "txt"
 1101 Button 0x54012003 0x0 4 4 152 13 "Show always, even if dialog inactive"
 1104 Button 0x54012003 0x0 4 18 152 12 "Balloon"
 1105 Button 0x54012003 0x0 4 32 152 12 "Don't subclass controls"
 1102 Static 0x54000000 0x0 4 54 34 13 "Time, s"
 1103 Edit 0x54032000 0x200 40 52 36 14 "tim" "max 32"
 3 Button 0x54032000 0x0 104 76 18 14 "?"
 1 Button 0x54030001 0x4 4 76 48 14 "OK"
 2 Button 0x54030000 0x4 54 76 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030502 "*" "1" "" ""

if(!ShowDialog("" &sub.DlgProcTooltip &controls _hwnd 0 0 0 !c.cid)) ret
_Undo

if c.cid
	c.tooltip=e1002txt
else
	_i=TO_FlagsFromCheckboxes(0 c1101Sho 1 c1104Bal 2 c1105Don 4)
	c.tooltip=F"{_i} {e1103tim}"
	c.tooltip.trim
	if(c.tooltip="0") c.tooltip.all


#sub DlgProcTooltip
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DT_Page hDlg DT_GetParam(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	_s=
 Here you can specify tooltip text for a control (if a control is selected), or properties of all tooltips of this dialog (if dialog is selected).
;
 Tooltip text can begin with .flags, eg ".1 text". Flags:
      1 - use initial control rectangle. Use for controls that are transparent to mouse messages, eg a Static control without SS_NOTIFY style. Shows tooltip even if the control is hidden.
      2 - also add the tooltip to child controls of the control. Use eg with combobox.
;
 Read about tooltip properties in <help>__Tooltip.Create</help>.
	QmHelp _s 0 6
ret 1
