BSTR b="ąč ﯔﮥ k"
WINAPI2.SetEnvironmentVariableW(L"unicode" b)

str s
out s.expandpath("%unicode%")
 out s.expandpath("kkkk %unicode%")
 out s.expandpath("kkkk %unicode%" 1)
