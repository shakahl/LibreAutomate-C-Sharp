 ShowTooltip "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" 30 0 0 0 1 "title"
 ShowTooltip "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" 30 0 0 0 1 "title" TTI_INFO
 __Hicon hi=GetFileIcon("qm.exe")
 ShowTooltip "Tooltip with title and icon.[]s" 10 xm ym 300 1 "title" hi
 ShowTooltip _s.getmacro 10 xm ym 0 1 "title" hi
 ShowTooltip _s.getmacro 10 xm ym 0 0 " "
 ShowTooltip "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" 30 0 0 0 1 "" hi
 ShowTooltip "Tooltip with title and icon.[]s" 10 xm ym 300 1 "title" "$qm$\qm.exe"

str s="Asynchronous balloon tooltip[]with title, icon and color."
 ShowTooltip s 2 xm ym 300
ShowTooltip s 10 xm ym 300 3 "title" "$qm$\info.ico"
 out 1
