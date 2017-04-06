int e=RegLoadKey(HKEY_USERS "xpfix" _s.expandpath("$system$\config"))
if(e) end _s.dllerror("" "" e)
rset "," "some value" "xpfix\microsoft\windows nt\current version\winlogon" HKEY_USERS
RegUnLoadKey(HKEY_USERS "xpfix")
