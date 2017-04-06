 Creates custom jump list.
 Works on Windows 7 and later.
 A jump list is the menu that you see when you right click a taskbar button or a pinned taskbar icon, also by a pinned Start Menu icon.
 If you run this in QM, it adds the jump list to the QM taskbar button or pinned icon. If in exe - to your exe's button/icon.

 At first call Begin.
 There are several types of categories in jump lists:
 1. Tasks. It is a predefined category name. To add items, call AddItem for each item, then call AddAsTasks.
 2. Custom categories. To add items, call AddItem for each item, then call AddAsCategory. Repeat this for aech category, if need multiple categories.
 3. Frequent and Recent. They are auto-generated categories. Use AppendKnownCategory.
 Finally call End.

 EXAMPLE

#compile "__JumpList"
JumpList x
x.Begin
 create Tasks
x.AddItem("run me")
x.AddItem("run me with CL" "" "v")
x.AddItem("-") ;;separator
x.AddItem("Macro1668" "qmcl.exe" "M ''Macro1668''" "$qm$\macro.ico" "Runs Macro1668 in QM")
x.AddAsTasks
 create custom category Programs
x.AddItem("Notepad" "$system$\notepad.exe")
x.AddItem("Calculator" "$system$\calc.exe")
x.AddAsCategory("Programs")
 create custom category Documents
x.AddItem("QM Help" "$qm$\qm2help.chm" "" "hh.exe")
x.AddAsCategory("Documents")
 Append Frequent and Recent.
x.AppendKnownCategory(1)
x.AppendKnownCategory(2)
x.End
