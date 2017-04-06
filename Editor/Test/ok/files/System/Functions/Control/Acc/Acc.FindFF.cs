function! `hwnd $tag [$textOrHTML] [$attributes] [flags] [^waits] [matchIndex] [$navig] [*cbFunc] [cbParam] ;;flags: 1 text *, 2 text rx, 4 att *, 8 attr rx, 64 direct child, 128 reverse, 0x1000 error if not found, 0x2000 search in UI, 0x10000 document always busy

 Finds accessible object in web page, and initializes this variable.
 Works with Firefox, Thunderbird and other windows that use Gecko engine. If Firefox is installed, also works with Chrome, except in frames. Read more in Remarks.
 Returns (QM 2.4.2): 1 found, 0 not found. If flag 0x1000, error if not found.

 hwnd - where to search.
   Can be one of:
     Window handle, name or +class. If "" - active window. Must be top-level window (Firefox etc).
     IAccessible variable (accessible object). If you have Acc a, pass a.a.
     FFNode variable.
 tag - HTML tag (eg "A"). Use "" for text nodes.
   Can begin with node type, like "1 A" or "3".
 textOrHTML - text or inner HTML.
   For element nodes (node type 1), it is inner HTML.
   For other nodes (text, comment), it is text.
 attributes - one or more attributes in CSV format, using "=" as separator. Example: "src=images/x.png[]alt=''alt text''".
 waits - max number of seconds to wait for the node (eg if the web page is still not loaded). Error on timeout. If omitted or 0, does not wait.
 matchIndex - 1-based index of matched node. Use when there are several nodes that match other properties.
 navig - post-navigation string. Same as with acc.
 cbFunc - address of <help "Callback_Acc_FindFF">callback function</help> that will be called for each node in the tree, until it returns 0.
   A template is available in menu -> File -> New -> Templates.
   Callback functions can be used when you need to: 1. Get array of matching nodes. 2. Compare more properties of nodes. 3. Display tree of nodes. 4. Make faster by skipping some big useless nodes. 5. ...

 REMARKS
 This function is similar to <help>Acc.Find</help> and <help>acc</help>, but uses different searching method and is faster.
 It finds <help "FFNode Help">HTML node</help> COM object (calls <help>FFNode.FindFF</help>), and converts it to accessible object.
 To capture nodes, use the 'Find accessible object' dialog. If 'as Firefox node' is checked, it creates code for this function.

 Works only with 32-bit program versions.
 If using portable Firefox, <link "http://www.quickmacros.com/forum/viewtopic.php?f=1&t=5551">look here</link>.

 Firefox and Thunderbird support nodes in whole window, not only in web content.
   If hwnd is window, this function searches only in web content (in DOCUMENT accessible object). Only in the visible tab.
   If used flag 0x2000, searches in window (in APPLICATION accessible object) and skips web content (all tabs). Also may skip bookmarks.
   If hwnd is accessible object or other node that is not in web content, skips web content.

 Added in: QM 2.3.3.

 ERRORS
 ERR_OBJECT - not found (if flag 0x1000).
 ERR_HWND, ERR_WINDOW - hwnd invalid, or window not found.
 ERR_BADARG - attributes CSV invalid; hwnd invalid when used IAccessible or FFNode.
 ERR_OBJECTGET - the found Firefox node object cannot be converted to accessible object.


FFNode f
if(!f.FindFF(hwnd tag textOrHTML attributes flags waits matchIndex cbFunc cbParam)) ret
FromFFNode(f)
if !empty(navig)
	if(flags&0x1000) Navigate(navig) ;;error if not found
	else if(a) this=acc(this navig) ;;no error if not found

ret this.a!0

err+ end _error
