int w=wait(3 WV win("- Google Chrome" "Chrome_WidgetWin_1"))
Acc a.Find(w "DOCUMENT" "" "" 0x3001)
out a.Name

 out a.ChildCount
 ARRAY(Acc) b; int i
 a.GetChildObjects(b)
 out b.len
 for(i 0 b.len) b[i].Role(_s); out _s


 IDispatch d=a.a.Child(1)
 Acc b; b.a=d
  b.IA2
 b.Role(_s); out _s


 TODO: enable in:
 "Find acc" dialog.
 Acc.FromMouse etc.
