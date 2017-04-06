function! [flags] ;;flags: 1 reset the "changed" state, 2 set the "changed" state

 Returns 1 if grid content is changed by the user (cells edited, rows added, etc). Else returns 0.

 Another way to track changes - LVN_QG_CHANGE notification, sent when user changes grid data (edits a cell, deleted a row, etc).
 The control is considered "changed" (and LVN_QG_CHANGE sent) only when the user changes its content. Not when you send a message (LVM_INSERTITEM, LVM_SORTITEMSEX, etc).


ret Send(GRID.LVM_QG_GETINFO GRID.QG_GETINFO_ISCHANGED flags)
