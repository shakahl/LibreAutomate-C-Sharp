str regcode

out "Validating registration code, please wait ..."
str url s
url.from("http://www.server5734.com/validateqmcode.php" "?" regcode)
IntGetFile url s
err goto Get name. ;;if failed to access the web page, validate regcode locally.
if(s="valid") ret -1 ;;if web script returns text "valid" (single word, not html), unlock.
if(s.beg("<")) goto Get name. ;;html, probably error page, like The requested URL /validateqmcode.php was not found on this server.
if(s.len) out s ;;optionally display the reason why the regcode is invalid.
ret 0 ;;lock

 Get name.
out s
