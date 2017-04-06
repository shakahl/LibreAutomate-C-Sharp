 /
function! hlv item subitem str&s

 Gets listview control item text.
 Returns 1 if not empty, 0 if empty.

 hlv - handle.
 item - 0-based row index.
 subitem - 0-based column index.
 s - receives text.


LVITEM li
li.pszText=s.all(260)
li.cchTextMax=260
li.iSubItem=subitem

_i=SendMessage(hlv LVM_GETITEMTEXT item &li)
s.fix(_i)
ret s.len!=0
