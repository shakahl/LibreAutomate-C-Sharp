
 Dialog definition. To edit it, click the 'Dialog Editor' button on the toolbar.
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Static 0x54000200 0x0 8 8 48 12 "Edit"
 4 Edit 0x54030080 0x200 64 8 72 12 ""
 5 Button 0x54012003 0x0 144 8 72 12 "Check"
 6 Static 0x54000200 0x0 8 28 48 13 "ComboBox"
 7 ComboBox 0x54230243 0x0 64 28 72 213 ""
 8 ComboBox 0x54230242 0x0 144 28 72 213 ""
 9 Static 0x54000000 0x0 8 48 48 12 "ListBox"
 10 ListBox 0x54230101 0x200 64 48 72 32 "Single-selection" "Single-selection listbox"
 11 ListBox 0x54230109 0x200 144 48 72 32 "Multi-selection" "Multi-selection listbox"
 12 Static 0x54000000 0x0 8 88 48 12 "Text"
 13 Static 0x54000003 0x0 64 88 16 16 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

 Variables for dialog controls. The Dialog Editor updates these 2 lines of code. Don't edit directly.
str controls = "4 5 7 8 10 11 13"
str e4 c5Che cb7 cb8 lb10Sin lb11Mul si13

 You can assign values to some variables to initialize corresponding controls.
 The Dialog Editor does not use this code; just updates variable names if changed.
 Example:
e4="edit me" ;;add some text to the Edit control
c5Che=1 ;;make the Check control checked
cb7="&Zero[]One[]Two" ;;add 3 items to the ComboBox control. Use & to select an item.
cb8=cb7
lb10Sin=cb7
lb11Mul="Zero[]&One[]&Two" ;;add 3 items to the multi-select ListBox control. Use & to select an item.
si13="$qm$\info.ico" ;;show an icon file in the static icon control. For 32x32 icon prepend &.

 Show the dialog, wait until the user closes it. Exit on Cancel. You can edit this line, for example pass more arguments.
if(!ShowDialog(dd 0 &controls)) ret

 Now you have control values in the variables.
 Example:
out "Edit: %s" e4
int cChecked=val(c5Che); out "Check: %s" iif(cChecked "checked" "unchecked")
int cbSelectedIndex=val(cb7); out "ComboBox1: item %i selected" cbSelectedIndex
out "ComboBox2: %s" cb8+findt(cb8 1) ;;cb8 is like "index text". To get the text part can be used findt(cb8 1).
int lbSelectedIndex=val(lb10Sin); out "ListBox1: item %i selected" lbSelectedIndex
out "ListBox2:"; int i; for(i 0 lb11Mul.len) if(lb11Mul[i]='1') out "[9]item %i selected" i ;;lb11Mul is like "101", where '1' are selected items.

 More help: click the ? button in the Dialog Editor.
