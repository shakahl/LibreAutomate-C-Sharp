 \Dialog_Editor

str dd=
 BEGIN DIALOG
 1 "" 0x90C802C8 0x0 310 0 114 112 "After"
 3 Edit 0x54032080 0x200 8 8 48 14 "d"
 11 msctls_updown32 0x500000A6 0x0 100 8 11 14 "d"
 4 Static 0x54000200 0x0 60 8 48 12 "days"
 5 Edit 0x50032080 0x200 8 28 48 14 "h"
 12 msctls_updown32 0x500000A6 0x0 100 28 11 14 "h"
 6 Static 0x54000200 0x0 60 28 48 13 "hours"
 7 Edit 0x54032080 0x200 8 48 48 14 "m"
 13 msctls_updown32 0x500000A6 0x0 100 48 11 14 "m"
 8 Static 0x54000200 0x0 60 48 48 12 "minutes"
 9 Edit 0x54032080 0x200 8 68 48 14 "s"
 14 msctls_updown32 0x500000A6 0x0 100 68 11 14 "s"
 10 Static 0x54000200 0x0 60 68 48 13 "seconds"
 1 Button 0x54030001 0x4 8 92 48 14 "OK"
 2 Button 0x54030000 0x4 60 92 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "*" "" "" ""

str controls = "3 11 5 12 7 13 9 14"
str e3d ud11d e5h ud12h e7m ud13m e9s ud14s
if(!ShowDialog(dd 0 &controls)) ret
