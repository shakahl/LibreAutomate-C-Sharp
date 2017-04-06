str s=
 http://www.quickmacros.com/images/ss/qm.png
 http://www.quickmacros.com/images/ss/debug.png
 http://www.quickmacros.com/com/winapi2.zip
str saveFolder="$temp$\qm igmp"
ARRAY(str) a
IntGetFileMultiProgress s saveFolder a 0x10000
int i; for(i 0 a.len) out "'%s'  '%s'" a[0 i] a[1 i]
run saveFolder
