 Shows how to use function str.replacerx() to find-replace text.

 str.replacerx() is similar to str.findreplace().
str x="Sunday Monday Tuesday"
x.findreplace("Monday" "(here was Monday)")
out x
str y="Sunday Monday Tuesday"
y.replacerx("Monday" "(here was Monday)")
out y

 But it can find not only exact text. It can find text like findrx().
y="file123 456.txt"
y.replacerx("\d+" "(here was a number)")
out y

 What if not found?
y="file.txt"
int n=y.replacerx("\d+" "(here was a number)") ;;replacerx returns number of found matches
if(n=0) out "not found"

 How to use submatches in the replacement string?
y="file01-02-2000.txt"
y.replacerx("(\d{2})-(\d{2})-(\d{4})" "$3-$1-$2") ;;in the replacement string, $number can be used for a submatch
out y ;;converted date from mm-dd-yyyy to yyyy-mm-dd format
