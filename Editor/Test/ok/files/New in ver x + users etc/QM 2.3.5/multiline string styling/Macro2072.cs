out
_s=
F
 aa {_i} bb
 ccc
 dd "dd" { "sss" }
 ee {_i+33%100%%c} ee
 ff { _s.from("A" 5) }
;
out _s

_s=F"aaa {_s.from(`aaa`)} bbb"

 just { _s.from("A" 5) } comments
"fff"; _s
act "gggg" g
_s.from(1) 