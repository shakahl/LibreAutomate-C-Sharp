 tim 0 ;; Cancel timer for this function
 str macroname.getmacro(getopt(itemid 3) 1)
 str typ.getmacro(getopt(itemid 3) 3)
 out "%s Name: %s, type: %s" _s.time("%H:%M:%S") macroname typ

openNextExe "explorer" "c:\windows\explorer.exe" "/e,"

