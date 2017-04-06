dll advapi32 #RegRenameKey hKey @*lpSubKeyName @*lpNewKeyName

PF
RegKey k
if(RegCreateKeyExW(HKEY_CURRENT_USER @"Software\GinDi\QM2-backup" 0 0 0 KEY_WRITE 0 &k 0)) end "failed"
PN
_i=SHCopyKeyW(HKEY_CURRENT_USER @"Software\GinDi\QM2" k 0); if(_i) end _s.dllerror("" "" _i)
PN
_i=SHDeleteKeyW(HKEY_CURRENT_USER @"Software\GinDi\QM2"); if(_i) end _s.dllerror("" "" _i)
PN; PO
