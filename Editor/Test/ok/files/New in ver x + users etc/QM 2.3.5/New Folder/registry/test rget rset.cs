out
int n
str s; int i; word w; byte b; double d; long k; RECT r

 n=rset("test" "string" "Test\Test2") ;;ok
 n=rset("test" "x" "Test\Test2" HKEY_CURRENT_USER) ;;ok
 n=rset("test" "x" "Test\Test2" HKEY_CLASSES_ROOT) ;;ok
 n=rset("test" "x" "Test\Test2" HKEY_LOCAL_MACHINE) ;;fails, probably cannot create keys at root
 n=rset("test" "x" "Software\Test" HKEY_LOCAL_MACHINE) ;;ok

 n=rset("" "x" "Software\Test" HKEY_LOCAL_MACHINE -1)
 n=rset("" "" "Software\Test" HKEY_LOCAL_MACHINE -2)
 n=rset("" "Test" "Software" HKEY_LOCAL_MACHINE -2)

 n=rset("value" "Test" "Software" "$desktop$\test.ini")
 n=rset("" "Test" "Software" "$desktop$\test.ini" -1)
 n=rset("" "" "Software" "$desktop$\test.ini" -2)

if(n) out n; else out _s.dllerror; ret
#ret
int hive=0
 str hive="$desktop$\test.ini"

 n=rset("test" "string" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 n=rset(100 "int" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 n=rset(10.5 "double" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 n=rset(1000000000000000 "long" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 w=20; n=rset(w "word" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 b=10; n=rset(b "byte" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
SetRect &r 1 2 3 4; n=rset(r "RECT" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret


 out rget(s "string" "Test\Test2" hive)
 n=rget(s "int" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 n=rget(s "double" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 n=rget(s "long" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 n=rget(s "word" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
 n=rget(s "byte" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
n=rget(s "RECT" "Test\Test2" hive); if(n) out n; else out _s.dllerror; ret
out s
 n=rget(i "int" "Test\Test2" hive); if(n) out n; out i; else out _s.dllerror; ret
 n=rget(d "double" "Test\Test2" hive); if(n) out n; out d; else out _s.dllerror; ret
 n=rget(k "long" "Test\Test2" hive); if(n) out n; out k; else out _s.dllerror; ret
 n=rget(w "word" "Test\Test2" hive); if(n) out n; out w; else out _s.dllerror; ret
 n=rget(b "byte" "Test\Test2" hive); if(n) out n; out b; else out _s.dllerror; ret
n=rget(r "RECT" "Test\Test2"); if(n) out n; zRECT r; else out _s.dllerror; ret

 n=rget(i "int_no" "Test\Test2" hive 10); if(n) out n; else out _s.dllerror
 n=rget(i "int" "Test\Test2_no" hive 10); if(n) out n; else out _s.dllerror
 out i
