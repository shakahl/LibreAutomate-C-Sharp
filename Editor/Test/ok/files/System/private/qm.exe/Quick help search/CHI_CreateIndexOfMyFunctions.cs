 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 225 163 "Create Help Index"
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54030000 0x4 56 146 48 14 "Cancel"
 3 Edit 0x54231044 0x200 2 56 222 52 ""
 4 Static 0x54000000 0x0 2 2 222 52 "This will rebuild index of help search words.[][]You can optionally add some of your QM folders to the index. Enter folder paths in the list below. Example: \My Functions. QM will search in function names and help sections (comments before code).[]"
 5 Static 0x54000000 0x0 2 120 222 20 "These settings are common to all QM files."
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

str controls = "3"
str e3
if(!rget(e3 "user folders" "\chi")) rget e3 "user folders" "Software\Gindi\qm2\User Nonadmin\chi" HKEY_LOCAL_MACHINE ;;HKLM fbc
if(!ShowDialog(dd 0 &controls _hwndqm)) ret
rset e3 "user folders" "\chi"

CHI_CreateIndex

 todo:
 Store indexes of functions in qml files (system functions - in system.qml).
 Store Help, Tips and Tools in QM common appdata. If in setup - in $qm$.
 Include Help index in QM setup instead of creating on each user's PC.
