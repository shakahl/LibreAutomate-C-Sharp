 int x=4
 int i=SelInt(x 2 4 6)
 out i ;;2

 str x="Test"
 int i=SelStr(1 x "best" "test" "this")
 out i ;;9

 str x="September"
 int i=SelStr(1|4 x "jan" "feb" "mar" "apr" "may" "jun" "jul" "aug" "sep" "oct" "nov" "dec")
 out i ;;9

 str x="September"
 int i=SelStr(1|2 x "jan*" "feb*" "mar*" "apr*" "may*" "jun*" "jul*" "aug*" "sep*" "oct*" "nov*" "dec*")
 out i ;;9

 str x="file.GIF"
 int i=SelStr(1|8 x ".bmp" ".jpg" ".gif")
 out i ;;9

 str x="one, two"
 int i=SelStr(1|16 x "." "," ";")
 out i ;;9

 str x="one, two"
 int i=SelStr(1|16 x "." "," ";")
 out i ;;9

str x="File.txt"
int i=SelStr(1|32 x "best" "^.+\.\w{3}$" "this")
out i ;;9
