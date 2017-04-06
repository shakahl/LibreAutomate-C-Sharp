 Select dialog variables (not using a type, 2 lines), and run this macro.
 Converts to C and stores into the clipboard.

str s
s.getsel

 out
 s=
  str controls = "4 6 7 8 9 1002 1102 1104 1105 1106 1107 1108 1109 1110 1202 1204 1205 1206 1208 1210 14 16 17 18 19 21 22 23 24 25 28 29"
  str cb4On o6One o7Dai o8Wee o9Mon e1002 e1102 c1104Sun c1105Mon c1106Tue c1107Wed c1108Thu c1109Fri c1110Sat e1202 o1204Day o1205On e1206 e1208 e1210 c14Syn c16Del cb17 c18Rep cb19 cb21 c22Sto c23Sto cb24 c25Exp c28Syn c29Ena

if(!s.beg("str controls") or numlines(s)!2) mes- "At first select dialog variables (str controls..., 2 lines)."

str s1.getl(s 1) s2.getl(s 0) s3

s1.findreplace(" " ", ")
s1.replace("[9]DlgVar controls, " 0 5)

s2.replace("[9][9]" 0 4)

s3.from("[9]enum CID {" s1+18 "};")
REPLACERX rx.frepl=&sub.Callback_str_replacerx
s3.replacerx("\b\w+?(\d+)(\w*)" rx)

s.from(s3 "[]" s1 ";[]" s2 ";[]")

s.setclip
out "Stored to clipboard:"
out s


#sub Callback_str_replacerx
function# REPLACERXCB&x

int i(x.vec[0].cpMin) j(x.vec[1].cpMin) k(x.vec[1].cpMax)
j-i; k-i
x.match.ucase(0 j)
x.match+"="; x.match.geta(x.match j k-j)
