 SetEnvVar "desk" "C:\Users\G\Desktop"
 out LoadLibrary("%desk%\qmplus2.dll") ;;no

SetDllDirectory _s.expandpath("$desktop$")
out LoadLibrary("qmplus2.dll")
