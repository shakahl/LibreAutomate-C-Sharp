 rep(4) mac "eat_cpu"
 atend sub.Atend; 0.1
Acc a

 int w=win("Options" "#32770")
 a.Find(w "COMBOBOX" "" "class=ComboBox[]id=1577" 0x1004)
  a.Find(w "COMBOBOX" "Layout of toolbars" "class=ComboBox[]id=3089" 0x1005)
  a.CbSelect(2)
  a.CbSelect("Bigger")
 a.CbSelect("Bigger" 1)

 int w=win("SwingSet2" "SunAwtFrame")
 act w
 a.Find(w "combo box" "Hair:" "" 0x1001)
  a.CbSelect(3)
 a.CbSelect("Lara")
  a.CbSelect("Lara" 1)

 int w=win("Global Options jEdit: General" "SunAwtDialog")
 act w
 a.Find(w "combo box" "If open files are changed on disk:" "" 0x1001)
  a.CbSelect(1)
 a.CbSelect("prompt")
  a.CbSelect("prompt" 1)

 int w=win("Global Options jEdit: Editing" "SunAwtDialog")
 act w
 a.Find(w "combo box" "Select 0 for text area width (soft wrap only)" "" 0x1001)
 a.CbSelect("72" 2|1)

 int w=win("Untitled 1 - OpenOffice Writer" "SALFRAME")
 act w
 a.Find(w "combo box" "Font Size" "" 0x1001)
  a.Select(1) ;;does not work
  a.CbSelect(1 2)
 a.CbSelect("10" 2)

 int w=win("Document1 - Microsoft Word" "OpusApp")
 act w
 a.Find(w "COMBOBOX" "Zoom:" "class=MsoCommandBar" 0x1005)
 a.CbSelect("Page Width" 2)

 int w=wait(3 WV win("Tryit Editor v2.0 - Internet Explorer" "IEFrame"))
 act w
 a.Find(w "COMBOBOX" "" "" 0x3004 3)
 a.CbSelect("Opel")

 int w=wait(3 WV win("Tryit Editor v2.0 - Mozilla Firefox" "Mozilla*WindowClass" "" 0x4))
 act w
 a.FindFF(w "SELECT" "" "" 0x1001 3)
  a.CbSelect("Opel")
 a.CbSelect("Opel" 1)

 int w=wait(3 WV win("Quick Macros Forum • Edit post - Mozilla Firefox" "Mozilla*WindowClass" "" 0x4))
 act w
 a.FindFF(w "SELECT" "" "name=addbbcode20" 0x1084 3)
 a.CbSelect("Small")

 int w=wait(3 WV win("Tryit Editor v2.0 - Google Chrome" "Chrome_WidgetWin_1"))
 act w
 a.Find(w "COMBOBOX" "" "" 0x3004 3)
 a.CbSelect("Opel")
  a.CbSelect("Opel" 1)
  a.CbSelect(2 1)
  a.CbSelect("Opel" 2)
  a.CbSelect("Opel" 4)
  a.CbSelect(2)

 int w=wait(3 WV win("Quick Macros Forum • Post a reply - Google Chrome" "Chrome_WidgetWin_1"))
 act w
 a.FindFF(w "select" "Normal" "" 0x1001 3)
 a.CbSelect("Small" 1)


#sub Atend
shutdown -6 0 "eat_cpu"
