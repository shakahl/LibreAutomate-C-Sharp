 /
function toRight n str&s ;;private

 Sends Shift+Left/Right to move caret by n positions depending on s.
 Properly handles newlines and Unicode.


if(toRight) _s.left(s n); else _s.right(s n)
_s.findreplace("[]" "[10]")
BSTR b=_s; n=b.len

#if QMVER>=0x02030307
if(toRight) key SR(#n); else key SL(#n)
#else
rep(n) if(toRight) key SR; else key SL
#endif
