 /
function! hlv item subitem str&s

 Gets listview control item text.
 Returns 1 if not empty, 0 if empty.

 hlv - handle.
 item - 0-based row index.
 subitem - 0-based column index.
 s - receives text.


LVITEMW li
BSTR b.alloc(260)
li.pszText=b
li.cchTextMax=260
li.iSubItem=subitem

SendMessageW(hlv LVM_GETITEMTEXTW item &li)
s.ansi(b)
ret s.len!=0
