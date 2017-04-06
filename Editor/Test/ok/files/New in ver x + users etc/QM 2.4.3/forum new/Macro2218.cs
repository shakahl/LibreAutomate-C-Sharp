out
out GetEnvVar("test" _s)
RunConsole2 "setx.exe test ''MMMMMM'' /M" ;;set a system environment variable in registry for testing. /M is for all users, requires admin rights.
ReloadEnvVar "test"
out GetEnvVar("test" _s)
RunConsole2 "$my qm$\function271.exe"
