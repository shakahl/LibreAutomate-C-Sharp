 Calls RegCloseKey and clears this variable.

if(hkey and hkey&0xffffff00!=0x80000000) RegCloseKey hkey
hkey=0
