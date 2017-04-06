out
_s="default"
 if(!InputBox(_s 0)) ret
 InputBox(_s 1 "The macro will end on Cancel")
 if(!InputBox(_s 2)) ret
 if(!InputBox(_s 4)) ret
 if(!InputBox(_s 8)) ret
 if(!InputBox(_s 16)) ret
 if(!InputBox(_s 64 "" "" 0 -10 100)) ret
 if(!InputBox(_s 0 "des[]des" "cap ''quot''")) ret
 if(!InputBox(_s 0 "" "" _hwndqm)) ret
 if(!InputBox(_s 0 "" "Input box at screen right-bottm" 0 -1 -1)) ret
 _i=1; if(!InputBox(_s 0 "" "" 0 0 0 _i "Var")) ret
 if(!InputBox(_s 0 "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww" "" 0 0 0 0 "" 10)) ret
 if(!InputBox(_s 0 "" "Input box with timeout" 0 0 0 0 "" 10)) ret
if(!InputBox(_s 32 "" "" 0 0 0 0 "" 5 &Dialog122)) ret
 _i=InputBox(_s 128|2 "" "" 0 0 0 0 "" 5); if(!_i) ret
out _s
 out _i

