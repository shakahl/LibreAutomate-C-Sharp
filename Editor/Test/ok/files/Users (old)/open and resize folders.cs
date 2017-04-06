spe 10
int w1
run "C:\" "" "" "" 0x2800 "C:" w1
int w2
run "D:\" "" "" "" 0x2800 "D:" w2
siz 0.5 1.0 w1 4
mov 0 0 w1 4
siz 0.5 1.0 w2 4
mov 0.5 0 w2 4
act w1
