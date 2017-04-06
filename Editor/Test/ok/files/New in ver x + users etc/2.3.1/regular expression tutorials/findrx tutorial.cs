 Shows how to use function findrx() to find and extract text.

 findrx() is is similar to find().
int i
i=find("Sunday Monday Tuesday" "Monday") ;;find Monday
out i
i=findrx("Sunday Monday Tuesday" "Monday") ;;find Monday
out i

 But findrx() can find not only exact text.
 The second argument (regular expression) can contain special characters (metacharacters) that match certain characters or conditions.
i=findrx("file578.txt" "\d+") ;;find a number. Here \d matches a digit, and + means "one or more". So, \d+ matches one or more digits.
out i

 How to extract the found text?
str s
i=findrx("file578.txt" "\d+" 0 0 s) ;;pass a str variable as 5-th argument, and it will be populated with the match
out i
out s

 What if not found?
i=findrx("file.txt" "\d+") ;;if not found, returns -1
if(i<0) out "not found"

 When whole string must match:
i=findrx("578" "^\d+$") ;;here ^ and $ mean "beginning" and "end"
out i
i=findrx("file578.txt" "^\d+$") ;;does not match
out i

 Find a number where it is whole word:
i=findrx("file123 456.txt" "\b\d+\b" 0 0 s) ;;here \b means "word boundary". Word characters are alphanumeric ASCII characters and _.
out i
out s

 Find a date in a filename:
i=findrx("file01-02-2000.txt" "\d{2}-\d{2}-\d{4}" 0 0 s) ;;here \d{2} means "2 digits"
out i
out s

 If need only year:
i=findrx("file01-02-2000.txt" "\d{2}-\d{2}-(\d{4})" 0 0 s 1) ;;6-th argument tells which submatch (part enclosed in ()) to get into s
out i
out s

 If need month, day and year:
ARRAY(str) a
i=findrx("file01-02-2000.txt" "(\d{2})-(\d{2})-(\d{4})" 0 0 a)
out a[0] ;;whole match
out a[1] ;;submatch 1 (month)
out a[2] ;;submatch 2 (day)
out a[3] ;;submatch 3 (year)

 Find substring consisting of specified characters:
i=findrx("xxx 0x3ea5 yyy" "0x[\dabcdef]+" 0 0 s) ;;here [] is used to specify characters, \d is digit, + means "1 or more". So it finds 0x followed by 1 or more digits and characters abcdef.
out i
out s

 How to use \^$()[].+? and other metacharacters as ordinary characters? (all metacharacters are listed in "Regular expression syntax" topic in QM Help)
i=findrx("file.txt" ".+\.txt" 0 0 s) ;;insert \ before each such character. Here the first . means "any character", however the second . is just . because preceded by \.
out s
i=findrx("xxx file.txt yyy" "\Qfile.txt\E" 0 0 s) ;;or enclose part of regular expression in \Q and \E
out s
