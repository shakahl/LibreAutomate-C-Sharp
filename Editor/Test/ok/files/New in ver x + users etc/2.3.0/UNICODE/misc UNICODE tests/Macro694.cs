out
 _putenv "test=c:\abc\ąč ﯔﮥ qww"
WINAPI2.SetEnvironmentVariableW L"test2" +_s.unicode("c:\abc\ąč ﯔﮥ qww")
SpecFoldersMenu _hwndqm &_s
out _s
