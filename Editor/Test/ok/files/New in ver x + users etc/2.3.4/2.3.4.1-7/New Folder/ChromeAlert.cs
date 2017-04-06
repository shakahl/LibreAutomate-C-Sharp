function hwnd idObject idChild
 Acc a.FromEvent(hwnd idObject idChild); err out "error"
outw hwnd; ret
 Q &q
 int c=wait(5 WC child("" "Chrome_RenderWidgetHostHWND" hwnd))
 Q &qq
 outq
 outw c
 SendMessage(c WM_GETOBJECT OBJID_CLIENT 1)

 $a 2 1 0 "Chrome_WidgetWin_1"
