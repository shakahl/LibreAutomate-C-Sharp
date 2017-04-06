str var1="value1"
str var2="value2"
str result result2 sd
 urlencode
var1.escape(9)
var2.escape(9)
 format
sd.format("var1=%s&var2=%s" var1 var2)
 post
IntPost "http://www.lkjhguytre.com/folder/test.php" sd result
ShowText "" result ;;just for debugging
 urlencode result
result.escape(9)
 append result
sd.formata("&result=%s" result)
 post
IntPost "http://www.lkjhguytre.com/folder/test.php" sd result2
ShowText "" result2 ;;just for debugging
