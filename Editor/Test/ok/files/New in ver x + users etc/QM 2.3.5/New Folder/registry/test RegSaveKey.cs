 need admin

 out _s.expandpath("$my qm$")

out
out SetPrivilege(SE_BACKUP_NAME)
str sf.expandpath("$temp$\RegSaveKey.txt")
del- sf; err
RegKey k.Open("Software\GinDi\QM2")
int hr=RegSaveKey(k sf 0)
if(hr) end _s.dllerror("" "" hr)
run sf
