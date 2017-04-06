int x(5)
ARRAY(int)+ g_y

lock g_y
g_y.create(2)
g_y[1]=6
lock- g_y

mac "mac_t4" "" x

wait 2
lock g_y
out g_y[1]
lock- g_y
