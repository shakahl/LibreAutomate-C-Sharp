 Select dialog definition (not including BEGIN DIALOG etc), and run this macro.
 It converts the definition to C string and stores into the clipboard.

str s.getsel
if(!s.len) mes- "At first select dialog definition (not including BEGIN DIALOG etc)."
s.findreplace("\" "\\")
s.findreplace("''" "\''")
s.replacerx("(?m)^ ([^[]]+)$" "[9][9]''$1\r\n''")
s.fix(s.len-7); s+"'';[]"
s.setclip
 out "Stored to clipboard:"
 out s
