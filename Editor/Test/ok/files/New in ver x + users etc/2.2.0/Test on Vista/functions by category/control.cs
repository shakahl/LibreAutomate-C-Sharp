 Acc a=acc("5" "PUSHBUTTON" win("Calculator" "SciCalc") "Button" "" 0x1001)
 out a.Name
 a.DoDefaultAction

 Acc a=acc("More information" "LINK" win("Internet Explorer cannot display the webpage - Windows Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1001)
  a.DoDefaultAction
 a.Mouse(1)

 act id(140 "Calc")

 act "Calc"
 but 140 "Calc"
 but id(129 "Calc")

 control

 Htm el=htm("A" "More information" "" win("Internet Explorer cannot display the webpage - Windows Internet Explorer" "IEFrame") 0 2 0x21)
 el.Click

 int h=child("" "ComboLBox" win("Font" "#32770") 0x9 220 94)
 act h
 LB_SelectItem  2
 out LB_FindItem(h "Bold")

 act "Calc"
 scan "Macro452.bmp" 0 0 0x3
 act "Notepad"
 scan "Macro452.bmp" win("Untitled - Notepad" "Notepad") 0 0x3
