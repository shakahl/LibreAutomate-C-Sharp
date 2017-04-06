 this probably did not work, don't remember

RegKey k
#if 1

if(RegCreateKeyExW(HKEY_CURRENT_USER @"Software\GinDi\QM2\User\symlink" 0 0 REG_OPTION_CREATE_LINK|REG_OPTION_VOLATILE KEY_ALL_ACCESS 0 &k 0)) end "failed"
BSTR b="\Registry\USER\Software\Licenses"
if(RegSetValueExW(k L"SymbolicLinkValue" 0 REG_LINK b.pstr b.len*2+2)) end "failed"

#else

if(RegOpenKeyExW(HKEY_CURRENT_USER @"Software\GinDi\QM2\User\symlink" REG_OPTION_OPEN_LINK KEY_ALL_ACCESS &k)) end "failed"
dll ntdll #ZwDeleteKey hKey
out ZwDeleteKey(k)
