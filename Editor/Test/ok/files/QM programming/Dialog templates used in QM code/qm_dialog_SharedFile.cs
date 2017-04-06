 \Dialog_Editor


 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 243 107 "Shared File Properties"
 3 Static 0x54000000 0x0 4 6 22 10 "File"
 4 Edit 0x54030080 0x200 28 4 212 13 ""
 5 Button 0x54012003 0x0 4 22 104 10 "Don't load"
 6 Button 0x54012003 0x0 4 34 104 10 "Load local read-only copy"
 7 QM_DlgInfo 0x54000000 0x20000 4 54 236 28 "The changes will be applied when opening current main file next time."
 8 QM_DlgInfo 0x44000000 0x20000 4 20 236 29 "<>If file is <b>:memory:</b>, deleted items are not saved.[]To save, specify a file, eg <b>$my qm$\\Deleted.qml</b>. It will be created if does not exist."
 1 Button 0x54030001 0x0 4 88 48 14 "OK"
 2 Button 0x54030000 0x0 56 88 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040000 "" "" "" ""

