function! $regkey [hive] [access]

 Opens an existing registry key for using with Windows API registry functions (read, write, enumerate subkeys/values, etc).
 Returns: 1 success, 0 failed.

 regkey, hive - key and hive, like with <help>rget</help>.
   QM 2.3.0. If regkey is "", uses hive as key. Then access is ignored.
 access - registry access that is used with <help>RegOpenKeyEx</help>. If omitted or 0, uses KEY_ALL_ACCESS. If not going to set values or create subkeys, use KEY_READ.

 REMARKS
 The key will be automatically closed when destroying the variable.

 See also: <RegGetSubkeys> and other registry functions.


if(hkey) Close
if(!access) access=KEY_ALL_ACCESS
if(hive&HKEY_64BIT) hive~HKEY_64BIT; access|KEY_WOW64_64KEY
if(!hive) hive=HKEY_CURRENT_USER
if(empty(regkey)) hkey=hive; ret 1
if(regkey[0]='\') regkey=_s.from("Software\Gindi\QM2\User" regkey)

ret !RegOpenKeyExW(hive @regkey 0 access &hkey)

 QM 2.3.0.8, undocumented, little tested: To access 64-bit key, add flag HKEY_64BIT to hive.
