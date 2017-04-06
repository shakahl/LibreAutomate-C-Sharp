out "macro 'FF close tab' started"

 set focus to firefox, because Flash player may have it, and then Ctrl+W would not work
 int w=win
 Acc a.Find(w "DOCUMENT" "" "" 0x3010 2)
 a.Select(1)

 key Cw ;;Ctrl+W
