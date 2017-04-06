 cop "C:\Documents and Settings\a\Desktop\test.txt" "C:\Documents and Settings\All Users\Documents"
 cop+ "C:\Documents and Settings\a\Desktop\FromDesktop\List" "$Desktop$" 0x2C0|FOF_SILENT; err ErrMsg(2)
 del "C:\Documents and Settings\a\Desktop\List"
 ren* "C:\Documents and Settings\a\Desktop\ktest.txt" "ddd.txt"
 ren- "$Desktop$\test.txt[]$Desktop$\notes.txt" "$Desktop$\FromDesktop"
 ren "$Desktop$\*.txt" "C:\Documents and Settings\a\Desktop\FromDesktop"
 del- v
