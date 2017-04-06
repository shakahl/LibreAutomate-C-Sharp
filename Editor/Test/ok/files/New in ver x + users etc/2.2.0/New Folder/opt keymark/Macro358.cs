out
act "Notepad"; err run "Notepad"; 1
key CaX
0.5
int+ g_ki
g_ki=!g_ki; tls0=g_ki
out tls0
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST
spe
 str s.getmacro("TB Main")
str s ss.all(100 2 'a')
rep(20) s+ss; s+"[]"
key (s)
 a