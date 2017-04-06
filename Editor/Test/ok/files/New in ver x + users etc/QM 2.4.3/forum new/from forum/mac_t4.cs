/
function x ARRAY(int)&y

ARRAY(int)+ g_y

out x

lock g_y
out g_y[1]
g_y[1]=7
lock- g_y

wait 1
