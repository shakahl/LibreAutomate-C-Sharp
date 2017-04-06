int w=win("Options" "#32770")
Acc a.Find(w "COMBOBOX" "" "class=ComboBox[]id=1571" 0x1005)
a.CbSelect("User") ;;or a.CbSelect(1)

 int w=win("Form1" "WindowsForms10.Window.8.app.0.33c0d9d")
 Acc a.Find(w "COMBOBOX" "" "class=*.COMBOBOX.*[]wfName=comboBox1" 0x1005)
 a.CbSelect("seven")
