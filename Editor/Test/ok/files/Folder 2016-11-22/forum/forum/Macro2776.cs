 you can create this code with this dialog: floating toolbar -> Windows, controls -> Window/control actions -> Get windows (array)
ARRAY(int) a
GetMainWindows(a)
if(a.len=0) ret

 then select random array elament
int w=a[RandomInt(0 a.len-1)]

 view results
outw w
