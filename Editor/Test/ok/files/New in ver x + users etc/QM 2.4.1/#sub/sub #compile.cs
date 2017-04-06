
 #opt err 1
 #opt fff 1

 #compile sub.Test
#compile* "sub.Test"
 #compile sub_Test
 sub_Test

 #compile sub.Test3
 sub.Test3
 #compile+ "sub"

 int+ g_y
 out g_y
 #set g_y sub.Test2
out 5

#sub Test
def BBB 8
out __FUNCTION__

#sub Test2
function#
ret 8
def BBB2 82

#sub Test3 m
bb
