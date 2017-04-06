function hwndIE [$frame]

 Initializes the variable from web page that is currently open in Internet Explorer or IE-based web browser.
 Error if something fails, eg hwndIE is 0 or frame not found.

 hwndIE - web browser window handle.
 frame - frame string, in case there are frames in the page. Same as with <help>htm</help> ('Find html element' dialog).


Htm el=htm("HTML" "" "" hwndIE frame 0 32)
InitFromHtm(el)
err+ end _error
