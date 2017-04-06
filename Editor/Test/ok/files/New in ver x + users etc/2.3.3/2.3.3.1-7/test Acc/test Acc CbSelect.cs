 ----
 int w=win("Font" "#32770")
  act w
 Acc a.Find(w "COMBOBOX" "Script:" "class=ComboBox[]id=1140" 0x1005)
 a.CbSelect("Cyr*" 0)
  a.CbSelect(2)
 ----
 int w=win("Options" "#32770")
  act w
 Acc a.Find(w "COMBOBOX" "UAC: run as[10](Vista/7)" "class=ComboBox[]id=1571" 0x1005)
 a.CbSelect("User")
  a.CbSelect("User" 1)
  a.CbSelect("User" 0 "ComboLBox")
 ----
 int w=win("Options" "#32770")
  act w
 Acc a.Find(w "COMBOBOX" "web: default browser window class" "class=ComboBox[]id=1282" 0x1005)
 a.CbSelect("IEFrame")

 ----
 int w=wait(10 WV win("" "IEFrame"))
  act w
 Acc a.Find(w "COMBOBOX" "" "a:id=skpm_miestas" 0x3004 10)
 a.CbSelect("Rok*")
  a.CbSelect(4)

 ----
 int w=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Opera" "OperaWindowClass"))
 act w
 Acc a.Find(w "COMBOBOX" "" "" 0x3000 10)
 a.CbSelect("Rok*")
  a.CbSelect(4)

 ----
 int w=win("Options" "bosa_sdm_Microsoft Office Word 11.0")
 Acc a.Find(w "COMBOBOX" "Save Word files as:" "class=bosa_sdm_Microsoft Office Word 11.0" 0x1005)
 a.CbSelect("Single*" 0)
  a.CbSelect("Rich*")
  a.CbSelect(1)
 ----
 int w=win("Document1 - Microsoft Word" "OpusApp")
 act w
 Acc a.Find(w "COMBOBOX" "Zoom:" "class=MsoCommandBar" 0x1005)
 a.CbSelect("50%" 2)
 ----
 int w=win("Document1 - Microsoft Word" "OpusApp")
 act w
 Acc a.Find(w "COMBOBOX" "Font:" "class=MsoCommandBar" 0x1005)
 a.CbSelect("Times*" 2)

 ----
 int w=win("Adobe Dreamweaver CS4 - [IDH_WHATSNEW2.html]" "_macr_dreamweaver_frame_window_")
 act w
 Acc a.Find(w "COMBOBOX" "Format" "class=ComboBox[]id=10352" 0x1005)
 a.CbSelect("Heading 1")
 ----
 int w=win("Adobe Dreamweaver CS4 - [IDH_WHATSNEW2.html]" "_macr_dreamweaver_frame_window_")
 act w
 Acc a.Find(w "COMBOBOX" "" "class=ComboBox[]id=590" 0x1004)
  a.CbSelect("blue" 8) ;;does not work well without flag 1
 a.CbSelect("None" 9)
  a.CbSelect("dll" 9)

 ----
 int w=win("Document - WordPad" "WordPadClass")
 act w
 Acc a.Find(w "COMBOBOX" "Font family" "class=NetUIHWND" 0x1005)
 a.CbSelect("Courier New") ;;does not work

