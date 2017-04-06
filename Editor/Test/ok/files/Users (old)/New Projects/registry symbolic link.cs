 Creates, but regedit cannot use it.

RegKey k
#if 1 ;;1 create, 0 delete

if(RegCreateKeyExW(HKEY_CURRENT_USER @"Software\GinDi\QM2\User\symlink" 0 0 REG_OPTION_CREATE_LINK|REG_OPTION_VOLATILE KEY_ALL_ACCESS 0 &k &_i)) end "failed"
 BSTR b="\Registry\User\Software\Google" ;;regedit: cannot find target key
 BSTR b="\Registry\G\Software\Google" ;;RegSetValueExW fails
 BSTR b="\Registry\Machine\Software\GinDi" ;;regedit: access denied
BSTR b="\Registry\User\AppEvents" ;;regedit: cannot find target key
if(RegSetValueExW(k L"SymbolicLinkValue" 0 REG_LINK b.pstr b.len*2)) end "failed"

#else

if(RegOpenKeyExW(HKEY_CURRENT_USER @"Software\GinDi\QM2\User\symlink" REG_OPTION_OPEN_LINK KEY_ALL_ACCESS &k)) end "failed"
dll ntdll #ZwDeleteKey hKey
if(ZwDeleteKey(k)) end "failed"
