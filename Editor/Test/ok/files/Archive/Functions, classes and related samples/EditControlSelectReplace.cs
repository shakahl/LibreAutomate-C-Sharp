 /
function hwnd ifrom ito [$replacetext]

 Selects and optionally replaces part of text of an Edit or rich edit control.
 The window can be inactive. It can belong to any process.

 hwnd - control handle.
 ifrom, ito - 0-based character index. Use 0/-1 to select all text. Use -1/-1 to remove selection. Use -2/-2 to move the text cursor to the end.
 replacetext - text that will replace the selection. Can be "" to remove the selected text. If omitted or 0, does not replace.


SendMessageW hwnd EM_SETSEL ifrom ito
if(replacetext) SendMessageW hwnd EM_REPLACESEL 1 _s.unicode(replacetext)
