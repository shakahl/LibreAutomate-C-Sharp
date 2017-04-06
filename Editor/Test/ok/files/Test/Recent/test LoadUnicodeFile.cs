out
str f
 f="$desktop$\test.txt"
 f="$desktop$\unicode utf-8.txt"
 f="$desktop$\unicode normal.txt"
f="$desktop$\unicode big endian.txt"
 f="$desktop$\unicode no BOM.txt"

str s
int r=s.LoadUnicodeFile(f)
out r
out s

f.insert("-new" f.len-4)
s.SaveUnicodeFile(f r)
run f
