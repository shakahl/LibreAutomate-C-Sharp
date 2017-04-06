class CTest str'a
CTest c="m"
CTest- t_ct="m"
CTest+ g_ct
out "local %i, thread %i, global %i" &c &t_ct &g_ct

 c.Test(3)
 Function26
 g_ct.Test(9)
 
 CTest a=g_ct

 act "kkkkkkkkkk"
 err out _error.description
 out _error.code
 _error.code=1
