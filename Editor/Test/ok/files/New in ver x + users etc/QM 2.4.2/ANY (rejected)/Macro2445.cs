out
 int w=win("" "Shell_TrayWnd")
 Acc a1.Find(w "PUSHBUTTON" "" "class=MSTaskListWClass" 0x1005)
 TestANY a1.a.Parent

 type MYY a b c d e
 MYY m
 TestANY m
 VARIANT v=5
 TestANY ~v
 TestANY @"str"

 TestANY ExcelRow(5)
 TestANY acc(mouse)

 ARRAY(str) a="one[]two"; TestANY a
 ARRAY(POINT*) ap; TestANY ap
 TestANY &a
 TestANY &ap
 TestANY RetArrayStr
 TestANY key("a")


 call &sub.Test 1 2 3
 call &sub.Test 0 0 0
 call &sub.Test RetArrayStr RetArrayStr RetArrayStr ;;bug, nevermind

sub.Test "ttt" 5 4.2

#sub Test
function ANY'x ANY'y ANY'z

sub.Test2 x y z

#sub Test2
function ANY'x ANY'y ANY'z

out "0x%X %s" x.ta x.ts
out "0x%X %s" y.ta y.ts
out "0x%X %s" z.ta z.ts


 function x y z
  function ARRAY(str)x ARRAY(str)y ARRAY(str)z
 
 out "%i %i %i" x y z

