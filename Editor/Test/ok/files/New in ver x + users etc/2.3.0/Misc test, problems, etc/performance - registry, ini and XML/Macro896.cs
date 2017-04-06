str ini="$Common AppData$\GinDi\Quick Macros\HelpIndex\chi.ini"
Q &q
rset "vvalue" "sec" "nname" ini
Q &qq
str s
rget s "sec" "nname" ini
Q &qqq
outq
out s
