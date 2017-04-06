 \Dialog_Editor
function# str&s [$text] [$caption] [$default] [x] [y] ;;obsolete, use InputBox

 Input box. Similar to inp, but you can set position.
 Requires QM 2.1.8 or later.

 EXAMPLE
 str s
 if(!inp2(s "text" "caption" "" 0 -10)) end
 out s


str controls = "0 3 4"
str d0 st3 e4
d0=iif(len(caption) caption "QM Input")
st3=text
e4=default
if(!ShowDialog("inp2" 0 &controls 0 0 0 0 0 x y)) ret
s=e4
ret 1

 BEGIN DIALOG
 0 "" 0x90C80A4A 0x188 0 0 222 68 ""
 3 Static 0x54000000 0x0 6 6 216 12 ""
 4 Edit 0x54030080 0x200 6 24 214 14 ""
 1 Button 0x54030001 0x4 118 50 48 14 "OK"
 2 Button 0x54030000 0x4 170 50 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""
