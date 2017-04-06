 int hm=CreatePopupMenu
 AppendMenu hm 0 1 "a"
 AppendMenu hm 0 2 "b"
 int i=TrackPopupMenuEx(hm TPM_RETURNCMD xm ym _hwndqm 0)
 DestroyMenu hm

out PopupMenu("a[]b")
