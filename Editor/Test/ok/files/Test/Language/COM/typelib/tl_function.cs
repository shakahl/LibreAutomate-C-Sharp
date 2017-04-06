typelib olelib {3181A65A-CC39-4CDE-A4DF-2E889E6F1AF1} 1.4

 out lstrlenA("kkk")
out VT_I2
SYSTEMTIME st
long ft
GetSystemTime &st
SystemTimeToFileTime &st +&ft ;;uncomment in WinFunctions
out ft
FileTimeToSystemTime +&ft &st ;;uncomment in WinFunctions

out "%i %i %i" st.wYear st.wMonth st.wDay
 str s1="kkk"
 str s2="ppp"
 MoveMemoryToLPSTR s2 s1 3
 out s2
