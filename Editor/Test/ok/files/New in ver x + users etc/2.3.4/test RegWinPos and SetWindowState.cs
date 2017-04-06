int h=win("DialogRWP" "#32770")
 max h
 hid- h
 hid h
 1
 clo h

 min h
 res h

 hid h
 1
  hid- h
 SetWindowState h 5

 act h
 1
 SetWindowState h 6
 SetWindowState h 7
 SetWindowState h 2

SetWindowState h 4

 SetWindowState h 4
 SetWindowState h 4 1

 SetWindowState h 11 1
 hid- h

 Q &q
 rep(10) min h; res h
 Q &qq
 rep(10) WinSetState h SW_SHOWMINIMIZED; WinSetState h SW_RESTORE
 Q &qqq
 outq

#ret
def SW_HIDE 0
def SW_SHOWNORMAL 1
def SW_SHOWMINIMIZED 2
def SW_SHOWMAXIMIZED 3
def SW_MAXIMIZE 3
def SW_SHOWNOACTIVATE 4
def SW_SHOW 5
def SW_MINIMIZE 6
def SW_SHOWMINNOACTIVE 7
def SW_SHOWNA 8
def SW_RESTORE 9
def SW_SHOWDEFAULT 10
def SW_FORCEMINIMIZE 11
def SW_MAX 11
