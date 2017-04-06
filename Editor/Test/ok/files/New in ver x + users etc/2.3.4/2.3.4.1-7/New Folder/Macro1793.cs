out
 int v
 int& v
 int* v
 int+ v
 int- v
ARRAY(str) a

 out __LocalVarUniqueName("v" &_s "" "")

 out __LocalVarUniqueName("v" &_s "" "int")
 out __LocalVarUniqueName("v" &_s "" "str")
 out __LocalVarUniqueName("v" &_s "" "int*")
 out __LocalVarUniqueName("v" &_s "" "int&")
 out __LocalVarUniqueName("v" &_s "str v" "str")
 out __LocalVarUniqueName("v" &_s "int v" "str")

 out __LocalVarUniqueName("a" &_s "" "ARRAY(str)")
 out __LocalVarUniqueName("a" &_s "" "ARRAY(int)")
 out __LocalVarUniqueName("aa" &_s "ARRAY(int) aa" "ARRAY(int)")

 out __LocalVarUniqueName("n" &_s "ARRAY n" "")
 out __LocalVarUniqueName("n" &_s "ARRAY n" "ARRAY(int)")

 #if 0
 str act
 #endif
out __LocalVarUniqueName("act" &_s "" "")
out __LocalVarUniqueName("act" &_s "" "str")

out _s
