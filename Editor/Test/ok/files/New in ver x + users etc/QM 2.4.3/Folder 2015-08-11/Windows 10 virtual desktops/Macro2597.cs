int w wo

 w=_hwndqm
 w=win("Options")
wo=win("Notepad") ;;desktop 2
 wo=win("Firefox") ;;desktop 1

 w=win("Dependency Walker" "Afx:*" "" 0x4)
 w=win("TB MAIN" "QM_toolbar")
 w=win("Registry Editor" "RegEdit_RegEdit")
w=win("Document1 - Microsoft Word" "OpusApp")
 w=win("Dialog88" "#32770")


out MoveWindowToDesktopOf(w wo)
