 /exe
 out 1
int+ g_i=7
out g_i

ARRAY(str)+ g_a.create(2)
g_a[1]="test"
out g_a[1]

class TestGlobalInExe str's i
 TestGlobalInExe+ x
 x.s="yyyy"
ARRAY(TestGlobalInExe)+ g_g.create(2)
g_g[1].s="xxxxxxxx"
1
