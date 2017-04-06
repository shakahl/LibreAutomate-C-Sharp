int w=act(win("Sqlite Test" "#32770"))

dll "qm.exe" #TestNewitem [$name] [$text] [$templ] [$trigger] [$folder] [flags] [debugFind]

 out TestNewitem("ni")
 out TestNewitem("ni" "tttext" "Function" "Ck" "f1")
 out TestNewitem("ni" "tttext" "Function" "Ck" "\Temp\f11" 128|16)
 out TestNewitem("Cl.ni" "tttext" "Member" "" "f1" 4|8)
 out TestNewitem("" "" "NextWindow" "" "f1" 0)
 out TestNewitem("new name" "new text" "NextWindow" "" "f1" 0)
 out TestNewitem("" "" "" "" "" 0)
 out TestNewitem("test" "new text" "" "" "" 1)
 out TestNewitem("test" "new text 2" "" "" "\f1\m1" 1)
 out TestNewitem("test" "new text 5" "" "" "\f1\m1" 3)
 1
 out TestNewitem("test" "new text 5" "" "" "\f1\m1" 32|64)

PF
_i=TestNewitem("nimm" "tttext" "Function" "" "" 1|128)
 _i=TestNewitem("nimm" "tttext" "Function" "" "" 1)
 _i=TestNewitem(0 "tttext" "Function" "" "\sqlite export" 1 "testR")
PN;PO
out _i

#ret
newitem