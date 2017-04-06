function# $name

 Finds combo box item and returns 0-based index.
 Returns -1 if not found.

 name - item text. Wildcard, case-insensitive.


STRINT si.s=name; si.i=-1
Acc ai.Find(this.a "LISTITEM" "" "" 0x10 0 0 "" &sub.Callback &si)
if(!ai.a) ret -1
ret si.i


#sub Callback
function# Acc&ai level STRINT&r

r.i+1
str s=ai.Name
if(!s.len and r.s.len) s=ai.Value ;;Chrome
ret !matchw(s r.s 1)
err ret 1
