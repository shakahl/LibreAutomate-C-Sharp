 Select a function name eg in QM help and run this macro. If the name is not found in Categories function, shows message.
 Trims the name. Stores the name into the clipboard.

str s.getsel
s.trim
if(!s.len) ret
str ss.getmacro("Categories")
if(findw(ss s)>=0) ret
s.setclip
mes "Not found."
