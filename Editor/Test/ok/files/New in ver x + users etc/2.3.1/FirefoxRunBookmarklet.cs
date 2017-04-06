 /
function $bookmarklet

 Runs bookmarklet in Firefox.

 bookmarklet - bookmarklet code. Must begin with "javascript:".


int w1=win(" - Mozilla Firefox" "MozillaUIWindowClass")
act w1
 select address bar, get its text, set bookmarklet text, Enter
key Ad
str sa.getsel
outp bookmarklet
key Y
 restore old text
key Ad
outp sa
key Z ;;do you know a firefox keyboard shortcut to focus current page as it was before focusing address bar?
