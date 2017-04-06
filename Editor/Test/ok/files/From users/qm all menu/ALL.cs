 Trigger: @11

str s
MenuFromQmFolder2 "" s
DynamicMenu(s)
 out s

 s.setmacro("macro_menu")
  if(mac("macro_menu"))
 int l=mac("macro_menu")
	  GetLastSelectedMenuItem 0 &l 0
	 out l
 str m.getmacro(l 1)
 out m