 prepares preprocessed file
lpstr file="C:\Documents and Settings\a\Desktop\typelib\typelib.h"
str s.getfile(file)
s.findreplace("  " " " 8)
s.findreplace("[] []" "[]" 8)
s.findreplace("[][]" "[]" 8)
s.findreplace("__declspec(dllimport)" "")
s.findreplace("__stdcall" "")
s.findreplace(");" ");[]")
s.findreplace("[]typedef " "[][]typedef ")
s.setfile(file)
