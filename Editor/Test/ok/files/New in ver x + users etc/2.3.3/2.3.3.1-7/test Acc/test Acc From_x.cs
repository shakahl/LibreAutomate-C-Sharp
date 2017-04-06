Acc a

 ----
int w=win("Quick Macros - ok - [Macro1519]" "QM_Editor")
 a.Find(w "OUTLINEITEM" "Acc.FromMouse" "class=SysTreeView32" 0x1005)

 ----
 a.FromMouse
 a.FromXY(30 0.2 1)
 a.FromXYWindow(30 0.1 "" 0)

 ----
 int w=wait(10 WV win("" "Mozilla*WindowClass" "" 0x804))
  FFNode f.FindFF(w "INPUT" "" "" 0x1000)
  a.FromFFNode(f)
 FFNode f.FindFF(w "SPAN" "" "" 0x1000)
 a.FromFFNode(f 1)

 ----
 int w=wait(30 WV win("Google Išplėstinė paieška - Windows Internet Explorer" "IEFrame"))
  Htm el=htm("SELECT" "as_rights" "" w 0 8 0x221 30)
  a.FromHtm(el)
 Htm el=htm("SPAN" "" "" w 0 8 0x221 30)
 a.FromHtm(el 1)

 ----
 a.FromWindow(_hwndqm)
 a.FromWindow(id(2202 _hwndqm))

 ----
 a.FromFocus


a.showRECT
