 men "&Edit\Select &All" "Untitled - Notepad"
 men 25 "Notepad"

 but "Match" "Find"
 but 1041 "Find"
 but id(1041 "Find")
 but child("Match" "Button" "Find")

 but id(1041 "Find")
 but+ id(1041 "Find")
 but% id(1056 "Find")
 if(but(id(1056 "Find")))
	 if(!but(child(1057 "&Down" "Button" "Find" 0x1)))
		 out 4

 CB_SelectItem id(1137 "Font") 2
 CB_SelectString id(1136 "Font") "Engravers"; err ret
 LB_SelectItem child("" "ComboLBox" "Font" 0x1) 0
 SelectTab(id(1066 "Options") 2)
 SelectTab(id(11525 "Find") 4); err ErrMsg(2)
 out 1
