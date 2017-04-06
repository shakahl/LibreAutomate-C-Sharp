out

str s
lpstr nul
 out rget(s nul "k" "$desktop$\rset.ini" "def") ;;works
out rget(s nul nul "$desktop$\rset.ini" "def") ;;does not work because default section name is set to "QM"
outb s s.len 1
