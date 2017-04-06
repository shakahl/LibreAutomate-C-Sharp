str s=
 http://www.quickmacros.com/images/ss/qm.png
 http://www.quickmacros.com/images/ss/debug.png
 http://www.quickmacros.com/com/winapi2.zip
ARRAY(str) a
IntGetFileMultiProgress s "" a 0x10000
int i; for(i 0 a.len) out "'%s'  %i" a[0 i] a[1 i].len
