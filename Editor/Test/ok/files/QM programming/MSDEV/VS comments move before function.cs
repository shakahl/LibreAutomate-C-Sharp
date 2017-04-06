str s.getsel

if(!s.len) goto gNoSel
s.replacerx("^\t" "" 8)
if(!s.replacerx("^(\w.+\n{\r\n)((?:\t*//.+\r\n)+)(?:\r\n)?" "$2$1")) goto gNoSel

s.setsel
 out s

ret
 gNoSel
mes- "Please select beginning of a function with comments, and then run this macro." "" "x"
