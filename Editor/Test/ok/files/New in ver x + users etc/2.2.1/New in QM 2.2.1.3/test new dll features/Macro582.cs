dll msvcrt #sprintf $buffer $format ... ;;2 or more arguments
 dll msvcrt #sprintf $buffer [$format] ... ;;2 or more arguments

str s.all(100)
sprintf s "%s %i" "string" 5
 sprintf s
s.fix
out s
