 /
function! str&path

str rk ver
rk="SOFTWARE\JavaSoft\Java Runtime Environment"
if(!rget(ver "CurrentVersion" rk HKEY_LOCAL_MACHINE)) ret
rk+"\"; rk+ver
if(!rget(path "JavaHome" rk HKEY_LOCAL_MACHINE)) ret
ret 1
