out
 int w=win("" "QM_Editor")
 int c=id(2202 w) ;;outline
 TreeViewFindItem(c "Dialog159" 0x100)

 32-bit
 int w=win("Microsoft Spy++" "Afx:*" "" 0x4)
 int c=child("" "SysTreeView32" w 0x0 "id=59648") ;;outline
 PF
 TreeViewFindItem(c "Dialog159" 0x100)
 PN;PO

 64-bit
 int w=win("FileZilla" "wxWindowNR")
 int c=id(-31782 w) ;;outline
 int w=win("" "CabinetWClass")
 int c=id(100 w) ;;outline
 int w=win("Registry Editor" "RegEdit_RegEdit") ;;fails (long handles)
 int c=id(1 w) ;;outline
 int w=win("Task Scheduler" "MMCMainFrame")
 int c=id(12785 w) ;;outline
 out IsWindow64Bit(c)
 PF
 TreeViewFindItem(c "Dialog159" 0x100)
 PN;PO

 test how VirtualAllocEx works after it allocates whole address space.
