 /
function hwnd $txt


if(empty(txt)) ret
opt keychar 1
SendKeysToWindow hwnd key((txt))

err+ end _error
