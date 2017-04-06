str s=FirefoxGetAddress
 out s
s.replacerx("^.+\bv=(.+?)(?=&|$)" "$1" 4)
 out s
run "http://share.xmarks.com/folder/bookmarks/JQk2OrPIPd"
int w=wait(20 WV win("Video - Muzika (Xmarks shared folder) - Mozilla Firefox" "MozillaWindowClass"))
err mes- "Failed. If does not open xmarks in new tab, restart firefox."
act w
FirefoxWait
1
key Cf
0.5
paste s
