 /
function# $items [~text] [$caption] [x] [y] [timeoutS] [default] [flags] [hwndOwner] ;;items: "item1[]item2[]...".    flags: 1 activate, 2 item IDs

 Shows list box dialog.
 Returns 1-based index of selected item, or 0 on Cancel.
 Obsolete, use <help>ListDialog</help>.

 items - list of items, like "item1[]item2[]item3". Item that begins with & is initially selected. If empty, list is not displayed.
 text - text above list.
 caption - dialog title bar text.
 x, y - coordinates. If <0, relative to the right or bottom edge. If 0 (default), at screen center.
 timeoutS - max time (seconds) to show dialog. Default: 0 (infinite). To stop countdown, click the list box.
 default - return this value on timeout.
 flags:
   1 - activate dialog.
   2 - each line begins with a number (item ID) that will be returned instead of line index.
 hwndOwner (QM 2.2.1) - owner window handle.


if(hwndOwner) flags|1
ret ListDialog(items text caption flags&3^1|16 hwndOwner x y timeoutS default)
